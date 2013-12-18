using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBK monster manager.
/// Keeps track of what monsters are healing, what monsters are 
/// </summary>
using System;


public class CBKMonsterManager : MonoBehaviour {
	
	public static Dictionary<long, PZMonster> userMonsters = new Dictionary<long, PZMonster>();
	
	public static PZMonster[] userTeam = new PZMonster[TEAM_SLOTS];
	
	public static List<PZMonster> healingMonsters = new List<PZMonster>();
	
	public static List<PZMonster> enhancementFeeders = new List<PZMonster>();
	
	public static List<PZMonster> combiningMonsters = new List<PZMonster>();
	
	public static PZMonster currentEnhancementMonster;

	public static int totalResidenceSlots;

	public const int TEAM_SLOTS = 3;

	private static int _monstersCount;
	
	public static int monstersOnTeam
	{
		get
		{
			return _monstersCount;
		}
	}
	
	SubmitMonsterEnhancementRequestProto enhanceRequestProto = null;
	
	HealMonsterRequestProto healRequestProto = null;
	
	CombineUserMonsterPiecesRequestProto combineRequestProto = null;
	
	public static CBKMonsterManager instance;
	
	void Awake()
	{
		instance = this;
	}

	/// <summary>
	/// Sets up the monster lists according to data from the StartupResponse
	/// </summary>
	/// <param name="monsters">All user monsters</param>
	/// <param name="healing">List of monsters currently healing</param>
	/// <param name="enhancement">List of monsters enhancing and being used for enhancements</param>
	public void Init(List<FullUserMonsterProto> monsters, List<UserMonsterHealingProto> healing, UserEnhancementProto enhancement)
	{
		PZMonster mon;
		_monstersCount = 0;
		foreach (FullUserMonsterProto item in monsters) 
		{
			mon = new PZMonster(item);
			//Debug.Log("Adding monster " + item.userMonsterId);
			userMonsters.Add(item.userMonsterId, mon);
			if (item.teamSlotNum > 0)
			{
				userTeam[(item.teamSlotNum-1)] = mon; //Fucking off-by-ones.
				_monstersCount++;
			}
			if (!item.isComplete && item.numPieces >= mon.monster.numPuzzlePieces)
			{
				combiningMonsters.Add(mon);
			}
		}
		foreach (UserMonsterHealingProto item in healing) 
		{
			mon = userMonsters[item.userMonsterId];
			mon.healingMonster = item;
			healingMonsters.Add(mon);
		}
		if (enhancement != null)
		{
			currentEnhancementMonster = userMonsters[enhancement.baseMonster.userMonsterId];
			currentEnhancementMonster.enhancement = enhancement.baseMonster;
			
			Debug.Log("Ehancement Base: " + currentEnhancementMonster.enhancement.userMonsterId);
			
			foreach (UserEnhancementItemProto item in enhancement.feeders) 
			{
				mon = userMonsters[item.userMonsterId];
				mon.enhancement = item;
				enhancementFeeders.Add (mon);
			}
		}
		
		healingMonsters.Sort(new HealingMonsterSorter());
		
		if (CBKEventManager.Goon.OnTeamChanged != null)
		{
			CBKEventManager.Goon.OnTeamChanged();
		}
	}
	
	void OnEnable()
	{
		CBKEventManager.Popup.CloseAllPopups += SendRequests;
	}
	
	void OnDisable()
	{
		CBKEventManager.Popup.CloseAllPopups -= SendRequests;
	}

	/// <summary>
	/// Update this instance.
	/// TODO: Change this to a method that only gets called every second.
	/// We don't need to check all of these things every step, since they run on a second-to-second basis anyways.
	/// </summary>
	void Update()
	{
		CheckHealingMonsters();
		
		CheckEnhancingMonsters();
		
		CheckCombiningMonsters();
	}

	/// <summary>
	/// Prepares a new heal request.
	/// </summary>
	void PrepareNewHealRequest()
	{
		healRequestProto = new HealMonsterRequestProto();
		healRequestProto.sender = CBKWhiteboard.localMup;
	}

	/// <summary>
	/// Prepares a new enhance request.
	/// </summary>
	void PrepareNewEnhanceRequest()
	{
		Debug.Log("Preparing Enhance Request");
		enhanceRequestProto = new SubmitMonsterEnhancementRequestProto();
		enhanceRequestProto.sender = CBKWhiteboard.localMup;
	}

	/// <summary>
	/// Prepares a new combine pieces request.
	/// </summary>
	void PrepareNewCombinePiecesRequest()
	{
		combineRequestProto = new CombineUserMonsterPiecesRequestProto();
		combineRequestProto.sender = CBKWhiteboard.localMup;
	}

	/// <summary>
	/// Sends the complete heal request.
	/// </summary>
	/// <returns>The complete heal request.</returns>
	/// <param name="request">Request.</param>
	IEnumerator SendCompleteHealRequest(HealMonsterWaitTimeCompleteRequestProto request)
	{
		if (healRequestProto != null)
		{
			SendStartHealRequest();
			while (healRequestProto != null)
			{
				yield return null;
			}
		}
		
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_HEAL_MONSTER_WAIT_TIME_COMPLETE_EVENT, DealWithHealCompleteResponse);
		
		if (CBKEventManager.Goon.OnHealQueueChanged != null)
		{
			CBKEventManager.Goon.OnHealQueueChanged();
		}
	}
	
	IEnumerator SendCompleteEnhanceRequest(EnhancementWaitTimeCompleteRequestProto request)
	{
		if (enhanceRequestProto != null)
		{
			SendStartEnhanceRequest();
			while (enhanceRequestProto != null)
			{
				yield return null;
			}
		}
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		EnhancementWaitTimeCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as EnhancementWaitTimeCompleteResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != EnhancementWaitTimeCompleteResponseProto.EnhancementWaitTimeCompleteStatus.SUCCESS)
		{
			Debug.LogError("Enhance Wait Complete done messed: " + response.status.ToString());
		}
		
	}
	
	void SendStartHealRequest ()
	{
		if (healRequestProto == null)
		{
			return;
		}

		string str = "Sending Heal Request: ";
		foreach (var item in healRequestProto.umhNew) 
		{
			//str += "\nNew Monster: " + item.userMonsterId + "," + item.priority + ", " + item.userHospitalStructId + ", " + item.healthProgress + ", " + userMonsters[item.userMonsterId].healingMonster.expectedStartTimeMillis + ", " + userMonsters[item.userMonsterId].finishHealTimeMillis;  
		}
		foreach (var item in healRequestProto.umhUpdate) 
		{
			//str += "\nUpdate Monster: " + item.userMonsterId + ", " + item.priority + ", " + item.userHospitalStructId + ", " + item.healthProgress;  
		}
		foreach (var item in healRequestProto.umhDelete) 
		{
			//str += "\nDelete Monster: " + item.userMonsterId;
		}

		//Debug.LogWarning(str);
		
		UMQNetworkManager.instance.SendRequest(healRequestProto, (int)EventProtocolRequest.C_HEAL_MONSTER_EVENT, DealWithHealStartResponse);
		
		if (CBKEventManager.Goon.OnHealQueueChanged != null)
		{
			CBKEventManager.Goon.OnHealQueueChanged();
		}
		
	}
	
	void SendStartEnhanceRequest ()
	{
		if (enhanceRequestProto == null)
		{
			return;
		}
		
		UMQNetworkManager.instance.SendRequest(enhanceRequestProto, (int)EventProtocolRequest.C_SUBMIT_MONSTER_ENHANCEMENT_EVENT, DealWithEnhanceStartResponse);
		
		if (CBKEventManager.Goon.OnEnhanceQueueChanged != null)
		{
			CBKEventManager.Goon.OnEnhanceQueueChanged();
		}
	}

	void Feed (PZMonster feeder)
	{
		currentEnhancementMonster.GainXP(feeder.enhanceXP);
		enhancementFeeders.Remove(feeder);
		userMonsters.Remove(feeder.userMonster.userMonsterId);
	}
	
	void CheckEnhancingMonsters()
	{
		if (enhancementFeeders.Count > 0 && enhancementFeeders[0].finishEnhanceTime <= CBKUtil.timeNowMillis)
		{
			
			EnhancementWaitTimeCompleteRequestProto request = new EnhancementWaitTimeCompleteRequestProto();
			request.sender = CBKWhiteboard.localMup;
			request.isSpeedup = false;
			
			PZMonster feeder;
			while (enhancementFeeders.Count > 0 && enhancementFeeders[0].finishEnhanceTime <= CBKUtil.timeNowMillis)
			{
				feeder = enhancementFeeders[0];
				Feed (feeder);
				
				request.userMonsterIds.Add(feeder.userMonster.userMonsterId);
			}
			
			request.umcep = currentEnhancementMonster.GetCurrentExpProto();
			
			StartCoroutine(SendCompleteEnhanceRequest(request));
			
			/*
			if (enhancementFeeders.Count == 0)
			{
				currentEnhancementMonster.enhancement = null;
				currentEnhancementMonster = null;
			}
			*/
		}
	}

	bool SomeMonsterFinishedHealing()
	{
		foreach (var item in healingMonsters) 
		{
			if (item.finishHealTimeMillis <= CBKUtil.timeNowMillis)
			{
				return true;
			}
		}
		return false;
	}
	
	void CheckHealingMonsters()
	{
		if (SomeMonsterFinishedHealing())
		{
			HealMonsterWaitTimeCompleteRequestProto request = new HealMonsterWaitTimeCompleteRequestProto();
			request.sender = CBKWhiteboard.localMup;
			request.isSpeedup = false;
			UserMonsterCurrentHealthProto health;
			for (int i = 0; i < healingMonsters.Count;) 
			{
				if (healingMonsters[i].finishHealTimeMillis <= CBKUtil.timeNowMillis)
				{
					health = new UserMonsterCurrentHealthProto();
					health.userMonsterId = healingMonsters[i].healingMonster.userMonsterId;
					health.currentHealth = healingMonsters[i].maxHP;
					request.umchp.Add(health);
					CompleteHeal(healingMonsters[i]);
				}
				else
				{
					i++;
				}
			}
			StartCoroutine(SendCompleteHealRequest (request));
		}
	}
	
	public void SpeedUpHeal(int cost)
	{
		HealMonsterWaitTimeCompleteRequestProto request = new HealMonsterWaitTimeCompleteRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.isSpeedup = true;
		request.gemsForSpeedup = cost;
		
		UserMonsterCurrentHealthProto health;
		PZMonster item;
		while(healingMonsters.Count > 0)
		{
			item = healingMonsters[0];
			health = new UserMonsterCurrentHealthProto();
			health.userMonsterId = item.userMonster.userMonsterId;
			health.currentHealth = item.maxHP;
			request.umchp.Add(health);
			Debug.Log("Healing: " + health.userMonsterId);
			CompleteHeal(item);
		}
		StartCoroutine(SendCompleteHealRequest(request));
	}
	
	public void SpeedUpEnhance(int cost)
	{
		EnhancementWaitTimeCompleteRequestProto request = new EnhancementWaitTimeCompleteRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.isSpeedup = true;
		request.gemsForSpeedup = cost;
		
		PZMonster item;
		while(enhancementFeeders.Count > 0)
		{
			item = enhancementFeeders[0];
			Feed(item);
			request.userMonsterIds.Add(item.userMonster.userMonsterId);
		}
		request.umcep = currentEnhancementMonster.GetCurrentExpProto();
		currentEnhancementMonster.enhancement = null;
		
		StartCoroutine(SendCompleteEnhanceRequest(request));
	}
	
	void CompleteHeal(PZMonster monster)
	{
		healingMonsters.Remove(monster);
		monster.healingMonster = null;
		monster.currHP = monster.maxHP;
	}
	
	void DealWithEnhanceCompleteResponse(int tagNum)
	{
		EnhancementWaitTimeCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as EnhancementWaitTimeCompleteResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != EnhancementWaitTimeCompleteResponseProto.EnhancementWaitTimeCompleteStatus.SUCCESS)
		{
			Debug.LogError("Problem completing enhancement: " + response.status.ToString());
		}
		
		if (CBKEventManager.Goon.OnEnhanceQueueChanged != null)
		{
			CBKEventManager.Goon.OnEnhanceQueueChanged();
		}
	}
			
	void DealWithHealCompleteResponse(int tagNum)
	{
		HealMonsterWaitTimeCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as HealMonsterWaitTimeCompleteResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != HealMonsterWaitTimeCompleteResponseProto.HealMonsterWaitTimeCompleteStatus.SUCCESS)
		{
			Debug.LogError("Problem completing heal: " + response.status.ToString());
		}
	}

	/// <summary>
	/// Called during Dugeon Complete, uses the list of monsters sent in EndDungeonResponse
	/// to add new monsters and update incomplete monsters with new pieces
	/// </summary>
	/// <param name="monsters">Monsters.</param>
	public void UpdateOrAddAll(List<FullUserMonsterProto> monsters)
	{
		foreach (FullUserMonsterProto item in monsters) 
		{
			UpdateOrAdd(item);
		}
		if (combineRequestProto != null)
		{
			SendCombineRequest();
		}
	}
	
	void SendCombineRequest()
	{
		UMQNetworkManager.instance.SendRequest(combineRequestProto, (int)EventProtocolRequest.C_COMBINE_USER_MONSTER_PIECES_EVENT, DealWithCombineResponse);
	}

	/// <summary>
	/// Updates or adds the specified monster.
	/// Monsters are updated with new pieces when the player collects monster pieces
	/// New monsters have just had their first piece collected, so they need a new entry
	/// in userMonsters
	/// </summary>
	/// <param name="monster">Monster.</param>
	void UpdateOrAdd(FullUserMonsterProto monster)
	{
		PZMonster mon;
		if (userMonsters.ContainsKey(monster.userMonsterId))
		{
			Debug.Log("Updating monster: " + monster.userMonsterId);
			userMonsters[monster.userMonsterId].UpdateUserMonster(monster);
			mon = userMonsters[monster.userMonsterId];
		}
		else
		{
			Debug.Log("Adding monster: " + monster.monsterId);
			mon = new PZMonster(monster);
			userMonsters.Add(monster.userMonsterId, new PZMonster(monster));
		}
		
		if (!mon.userMonster.isComplete && mon.userMonster.numPieces == mon.monster.numPuzzlePieces)
		{
			StartMonsterCombine(mon);
		}
	}

	/// <summary>
	/// Starts the monster combine, given that enough pieces
	/// are present.
	/// </summary>
	/// <param name="monster">Monster.</param>
	void StartMonsterCombine(PZMonster monster)
	{
		if (monster.monster.minutesToCombinePieces == 0)
		{
			CombineMonster(monster);
		}
		else
		{
			monster.userMonster.combineStartTime = CBKUtil.timeNowMillis;
			combiningMonsters.Add(monster);
		}
	}

	/// <summary>
	/// Turns an incomplete monster into a complete monster.
	/// Called after the combination time has passed.
	/// </summary>
	/// <param name="monster">Monster.</param>
	void CombineMonster(PZMonster monster)
	{
		if (combineRequestProto == null)
		{
			PrepareNewCombinePiecesRequest();
		}
		
		monster.userMonster.isComplete = true;
		
		combineRequestProto.userMonsterIds.Add(monster.userMonster.userMonsterId);
		
		if (combiningMonsters.Contains (monster))
		{
			combiningMonsters.Remove(monster);
		}
	}

	/// <summary>
	/// Checks through the list of monsters that have enough pieces to see if we
	/// need to complete any of them.
	/// </summary>
	void CheckCombiningMonsters()
	{
		if (combineRequestProto == null)
		{
			for (int i = combiningMonsters.Count - 1; i >= 0; i--){
				PZMonster item = combiningMonsters[i];
				if (item.combineTimeLeft <= 0)
				{
					CombineMonster(item);
				}
			}
			if (combineRequestProto != null)
			{
				SendCombineRequest();
			}
		}
	}
	
	public void SpeedUpCombine(PZMonster monster)
	{
		combiningMonsters.Remove(monster);
		
		PrepareNewCombinePiecesRequest();
		
		combineRequestProto.userMonsterIds.Add(monster.userMonster.userMonsterId);
		
		combineRequestProto.gemCost = monster.combineFinishGems;
		
		SendCombineRequest();
	}


	/// <summary>
	/// Adds a monster to the healing queue.
	/// Function shorts if there are no available hospitals.
	/// </summary>
	/// <param name="monster">Monster.</param>
	public void AddToHealQueue(PZMonster monster)
	{
		if (CBKBuildingManager.hospitals.Count == 0)
		{
			return;
		}

		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		monster.healingMonster = new UserMonsterHealingProto();
		monster.healingMonster.userId = CBKWhiteboard.localMup.userId;
		monster.healingMonster.userMonsterId = monster.userMonster.userMonsterId;
		monster.healingMonster.userHospitalStructId = 0;
		monster.healingMonster.healthProgress = 0;

		healingMonsters.Add (monster);
		
		RearrangeHealingQueue();
		
		if (healRequestProto.umhDelete.Contains(monster.healingMonster))
		{
			healRequestProto.umhDelete.Remove(monster.healingMonster);
			healRequestProto.umhUpdate.Add (monster.healingMonster);
		}
		else
		{
			healRequestProto.umhNew.Add(monster.healingMonster);
		}
		
		healRequestProto.cashChange += monster.healCost;
		
		CBKResourceManager.instance.Spend(ResourceType.CASH, monster.healCost);
		
		if (CBKEventManager.Goon.OnHealQueueChanged != null)
		{
			CBKEventManager.Goon.OnHealQueueChanged();
		}
	}

	/// <summary>
	/// Called whenever we add new monsters to the healing queue or the number of monsters
	/// in the healing queue changes. This will take into account multiple hospitals. 
	/// When deciding which hospital to assign to which monster, it will choose the 
	/// hospital which will finish the monster's healing the earliest.
	/// </summary>
	public void RearrangeHealingQueue()
	{
		CBKBuilding chosenHospital;
		long completeTimeAtChosen;
		foreach (CBKBuilding hospital in CBKBuildingManager.hospitals) 
		{
			hospital.completeTime = CBKUtil.timeNowMillis;
		}
		int priority = 1;
		foreach (PZMonster monster in healingMonsters) 
		{
			monster.healingMonster.priority = priority++;
			//If it already had a hospital assigned, find that hospital and adjust its health progress appropriately
			if (monster.healingMonster.userHospitalStructId > 0) 
			{
				if (CBKUtil.timeNowMillis > monster.healingMonster.expectedStartTimeMillis)
				{
					monster.healingMonster.healthProgress += (int)((CBKUtil.timeNowMillis - monster.healingMonster.expectedStartTimeMillis) / 1000 
						* CBKBuildingManager.instance.GetHospital(monster.healingMonster.userHospitalStructId).healthPerSecond);
				}
			}
			chosenHospital = null;
			completeTimeAtChosen = long.MaxValue;
			foreach (CBKBuilding hospital in CBKBuildingManager.hospitals) 
			{
				long timeToCompleteHere = (long)((monster.maxHP - (monster.currHP + monster.healingMonster.healthProgress)) 
				                                / hospital.combinedProto.hospital.healthPerSecond) * 1000
												+ hospital.completeTime;
				if (timeToCompleteHere < completeTimeAtChosen)
				{
					chosenHospital = hospital;
					completeTimeAtChosen = timeToCompleteHere;
				}
			}
			monster.healingMonster.expectedStartTimeMillis = chosenHospital.completeTime;
			chosenHospital.completeTime = completeTimeAtChosen;
			monster.healingMonster.userHospitalStructId = chosenHospital.userStructProto.userStructId;
		}
	}
	
	public void RemoveFromHealQueue(PZMonster monster)
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		int i;
		for (i = 0; i < healingMonsters.Count; i++) 
		{
			if (healingMonsters[i] == monster)
			{
				break;
			}
		}
		healingMonsters.RemoveAt(i);
		
		if (healRequestProto.umhNew.Contains(monster.healingMonster))
		{
			healRequestProto.umhNew.Remove(monster.healingMonster);
		}
		else
		{
			healRequestProto.umhDelete.Add(monster.healingMonster);
			if (healRequestProto.umhUpdate.Contains(monster.healingMonster))
			{
				healRequestProto.umhUpdate.Remove(monster.healingMonster);
			}
		}
		
		healRequestProto.cashChange -= monster.healCost;
		
		monster.healingMonster = null;

		//Set the hospital ID's of all the rest of the monsters in the queue to 0 so that they get rearranged properly
		for (; i < healingMonsters.Count; i++) 
		{
			if (!healRequestProto.umhNew.Contains(healingMonsters[i].healingMonster))
			{
				healRequestProto.umhUpdate.Add (healingMonsters[i].healingMonster);
			}
		}

		RearrangeHealingQueue();
		
		CBKResourceManager.instance.Collect(ResourceType.CASH, monster.healCost);
		
		if (CBKEventManager.Goon.OnHealQueueChanged != null)
		{
			CBKEventManager.Goon.OnHealQueueChanged();
		}
	}
	
	public void AddToEnhanceQueue(PZMonster monster)
	{
		if (enhanceRequestProto == null)
		{
			PrepareNewEnhanceRequest();
		}
		
		monster.enhancement = new UserEnhancementItemProto();
		monster.enhancement.userMonsterId = monster.userMonster.userMonsterId;
		
		//If this is the new base monster, set it up as such
		if (currentEnhancementMonster == null || currentEnhancementMonster.monster == null || currentEnhancementMonster.monster.monsterId == 0)
		{
			monster.enhancement.expectedStartTimeMillis = 0;
			currentEnhancementMonster = monster;
		}
		else
		{
			if (enhancementFeeders.Count == 0)
			{
				monster.enhancement.expectedStartTimeMillis = CBKUtil.timeNowMillis;
			}
			else
			{
				monster.enhancement.expectedStartTimeMillis = enhancementFeeders[enhancementFeeders.Count-1].finishEnhanceTime;
			}
			enhancementFeeders.Add(monster);
			enhanceRequestProto.cashChange -= monster.enhanceXP;
			
			CBKResourceManager.instance.Spend(ResourceType.CASH, monster.enhanceXP);
		}
		
		if (enhanceRequestProto.ueipDelete.Contains(monster.enhancement))
		{
			enhanceRequestProto.ueipDelete.Remove(monster.enhancement);
			enhanceRequestProto.ueipUpdate.Add(monster.enhancement);
		}
		else
		{
			enhanceRequestProto.ueipNew.Add(monster.enhancement);
		}
		
		
		if (CBKEventManager.Goon.OnEnhanceQueueChanged != null)
		{
			CBKEventManager.Goon.OnEnhanceQueueChanged();
		}
	}
	
	public void RemoveFromEnhanceQueue(PZMonster monster)
	{
		if (enhanceRequestProto == null)
		{
			PrepareNewEnhanceRequest();
		}
		
		int i;
		for (i = 0; i < enhancementFeeders.Count; i++) 
		{
			if (enhancementFeeders[i] == monster)
			{
				enhancementFeeders.RemoveAt(i);
				break;
			}
		}
		
		if (enhanceRequestProto.ueipNew.Contains(monster.enhancement))
		{
			enhanceRequestProto.ueipNew.Remove(monster.enhancement);
		}
		else
		{
			enhanceRequestProto.ueipDelete.Add(monster.enhancement);
		}
		
		//Update the rest of the feeders
		PZMonster feeder;
		for (; i < enhancementFeeders.Count; i++) 
		{
			feeder = enhancementFeeders[i];
			enhanceRequestProto.ueipUpdate.Add(feeder.enhancement);
			if (i == 0)
			{
				feeder.enhancement.expectedStartTimeMillis = CBKUtil.timeNowMillis;
			}
			else
			{
				feeder.enhancement.expectedStartTimeMillis = enhancementFeeders[i-1].finishEnhanceTime;
			}
		}
		
		monster.enhancement = null;
		
		enhanceRequestProto.cashChange += monster.enhanceXP;
		
		CBKResourceManager.instance.Collect(ResourceType.CASH, monster.enhanceXP);
		
		if (CBKEventManager.Goon.OnEnhanceQueueChanged != null)
		{
			CBKEventManager.Goon.OnEnhanceQueueChanged();
		}
	}
	
	/// <summary>
	/// Clears the enhance queue.
	/// Use this when the base monster is removed, removing all the feeders
	/// along with it.
	/// </summary>
	public void ClearEnhanceQueue()
	{
		if (enhanceRequestProto == null)
		{
			PrepareNewEnhanceRequest ();
		}
		
		//Remove from the back; more efficient
		while(enhancementFeeders.Count > 0)
		{
			RemoveFromEnhanceQueue(enhancementFeeders[enhancementFeeders.Count-1]);
		}
		
		if (enhanceRequestProto.ueipNew.Contains(currentEnhancementMonster.enhancement))
		{
			enhanceRequestProto.ueipNew.Remove(currentEnhancementMonster.enhancement);
		}
		else
		{
			enhanceRequestProto.ueipDelete.Add(currentEnhancementMonster.enhancement);
		}
		
		currentEnhancementMonster.enhancement = null;
		
		currentEnhancementMonster = null;
		
		if (CBKEventManager.Goon.OnEnhanceQueueChanged != null)
		{
			CBKEventManager.Goon.OnEnhanceQueueChanged();
		}
		
	}
	
	void DealWithHealStartResponse(int tagNum)
	{
		HealMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as HealMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != HealMonsterResponseProto.HealMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem sending heal request: " + response.status.ToString());
		}
		
		healRequestProto = null;
	}
	
	void DealWithEnhanceStartResponse(int tagNum)
	{
		SubmitMonsterEnhancementResponseProto response = UMQNetworkManager.responseDict[tagNum] as SubmitMonsterEnhancementResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != SubmitMonsterEnhancementResponseProto.SubmitMonsterEnhancementStatus.SUCCESS)
		{
			Debug.LogError("Problem sending enhance request: " + response.status.ToString ());
		}
		
		enhanceRequestProto = null;
	}
	
	void DealWithCombineResponse(int tagNum)
	{
		CombineUserMonsterPiecesResponseProto response = UMQNetworkManager.responseDict[tagNum] as CombineUserMonsterPiecesResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != CombineUserMonsterPiecesResponseProto.CombineUserMonsterPiecesStatus.SUCCESS)
		{
			Debug.LogError("Problem combining pieces: " + response.status.ToString());
		}
		
		if (CBKEventManager.Goon.OnHealQueueChanged != null)
		{
			CBKEventManager.Goon.OnHealQueueChanged();
		}
		
		combineRequestProto = null;
	}
	
	#region Team Management
	
	public int AddToTeam(PZMonster monster)
	{
		if (_monstersCount == TEAM_SLOTS)
		{
			return 0;
		}
		for (int i = 0; i < userTeam.Length; i++) 
		{
			if (userTeam[i] == null || userTeam[i].monster.monsterId <= 0)
			{
				userTeam[i] = monster;
				monster.userMonster.teamSlotNum = (i+1); //Off by one
				_monstersCount++;
				
				AddMonsterToBattleTeamRequestProto request = new AddMonsterToBattleTeamRequestProto();
				request.sender = CBKWhiteboard.localMup;
				request.userMonsterId = monster.userMonster.userMonsterId;
				request.teamSlotNum = (i+1);
				UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ADD_MONSTER_TO_BATTLE_TEAM_EVENT, DealWithAddResponse);
				
				if (CBKEventManager.Goon.OnMonsterAddTeam != null)
				{
					CBKEventManager.Goon.OnMonsterAddTeam(monster);
				}
				
				if (CBKEventManager.Goon.OnTeamChanged != null)
				{
					CBKEventManager.Goon.OnTeamChanged();
				}
				
				return i;
			}
		}
		return 0;
	}	
	
	void DealWithAddResponse(int tagNum)
	{
		AddMonsterToBattleTeamResponseProto response = UMQNetworkManager.responseDict[tagNum] as AddMonsterToBattleTeamResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != AddMonsterToBattleTeamResponseProto.AddMonsterToBattleTeamStatus.SUCCESS)
		{
			Debug.LogError("Problem adding monster to team: " + response.status.ToString());
		}
	}
	
	public void RemoveFromTeam(PZMonster monster)
	{
		userTeam[(monster.userMonster.teamSlotNum-1)] = null;
		
		RemoveMonsterFromBattleTeamRequestProto request = new RemoveMonsterFromBattleTeamRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.userMonsterId = monster.userMonster.userMonsterId;
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REMOVE_MONSTER_FROM_BATTLE_TEAM_EVENT, DealWithRemoveResponse);
		
		monster.userMonster.teamSlotNum = 0;
		
		_monstersCount--;
		
		if (CBKEventManager.Goon.OnMonsterRemoveTeam != null)
		{
			CBKEventManager.Goon.OnMonsterRemoveTeam(monster);
		}
		
		if (CBKEventManager.Goon.OnTeamChanged != null)
		{
			CBKEventManager.Goon.OnTeamChanged();
		}
	}
	
	void DealWithRemoveResponse(int tagNum)
	{
		RemoveMonsterFromBattleTeamResponseProto response = UMQNetworkManager.responseDict[tagNum] as RemoveMonsterFromBattleTeamResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != RemoveMonsterFromBattleTeamResponseProto.RemoveMonsterFromBattleTeamStatus.SUCCESS)
		{
			Debug.LogError("Problem removing monster from team: " + response.status.ToString());
		}
	}
	
	#endregion
	
	#region Sorters
	
	private class HealingMonsterSorter : Comparer<PZMonster>
	{
		public override int Compare (PZMonster x, PZMonster y)
		{
			return x.healingMonster.expectedStartTimeMillis.CompareTo(y.healingMonster.expectedStartTimeMillis);
		}
	}
	
	private class EnhancingMonsterSorter : Comparer<PZMonster>
	{
		public override int Compare (PZMonster x, PZMonster y)
		{
			return x.enhancement.expectedStartTimeMillis.CompareTo(y.enhancement.expectedStartTimeMillis);
		}
	}
	
	#endregion
	
	void OnPopupClosed()
	{
		SendRequests();
	}
	
	void SendRequests()
	{
		SendStartHealRequest();
		SendStartEnhanceRequest();
	}
	
	void OnApplicationQuit()
	{
		SendRequests();
	}
}
					

