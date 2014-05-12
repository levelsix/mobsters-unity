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


public class MSMonsterManager : MonoBehaviour {
	
	public List<PZMonster> userMonsters = new List<PZMonster>();
	
	public PZMonster[] userTeam = new PZMonster[TEAM_SLOTS];
	
	public List<PZMonster> enhancementFeeders = new List<PZMonster>();
	
	public static List<PZMonster> combiningMonsters = new List<PZMonster>();
	
	public PZMonster currentEnhancementMonster;

	public int totalResidenceSlots;

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
	
	CombineUserMonsterPiecesRequestProto combineRequestProto = null;

	SellUserMonsterRequestProto sellRequest = null;

	public static MSMonsterManager instance;

	public long lastEnhance
	{
		get
		{
			long last = 0;
			foreach (var item in enhancementFeeders) {
				last = Math.Max(last, item.finishEnhanceTime);
			}
			return last;
		}
	}
	
	void Awake()
	{
		if (instance != null)
		{
			Destroy (gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
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
			userMonsters.Add(mon);
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

		MSHospitalManager.instance.Init(healing);


		if (enhancement != null)
		{
			currentEnhancementMonster = userMonsters.Find(x => x.userMonster.userMonsterId == enhancement.baseMonster.userMonsterId);
			currentEnhancementMonster.enhancement = enhancement.baseMonster;
			
			//Debug.Log("Ehancement Base: " + currentEnhancementMonster.enhancement.userMonsterId);
			
			foreach (UserEnhancementItemProto item in enhancement.feeders) 
			{
				mon = userMonsters.Find(x=>x.userMonster.userMonsterId==item.userMonsterId);
				mon.enhancement = item;
				enhancementFeeders.Add (mon);
			}
		}

		
		if (MSActionManager.Goon.OnTeamChanged != null)
		{
			MSActionManager.Goon.OnTeamChanged();
		}
	}
	
	void OnEnable()
	{
		MSActionManager.Popup.CloseAllPopups += SendRequests;
	}
	
	void OnDisable()
	{
		MSActionManager.Popup.CloseAllPopups -= SendRequests;
	}

	/// <summary>
	/// Update this instance.
	/// TODO: Change this to a method that only gets called every second.
	/// We don't need to check all of these things every step, since they run on a second-to-second basis anyways.
	/// </summary>
	void Update()
	{	
		CheckEnhancingMonsters();
		
		CheckCombiningMonsters();
	}

	public List<PZMonster> GetMonstersByMonsterId(long monsterId, long notMonster = 0)
	{
		List<PZMonster> list = new List<PZMonster>();
		foreach (var item in userMonsters) 
		{
			if (item.monster.monsterId == monsterId && item.userMonster.userMonsterId != notMonster)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public void RemoveMonster(long userMonsterId)
	{
		userMonsters.RemoveAll(x=>x.userMonster.userMonsterId==userMonsterId);
		if (MSActionManager.Goon.OnMonsterRemoved != null)
		{
			MSActionManager.Goon.OnMonsterRemoved(userMonsterId);
		}
		if (MSActionManager.Goon.OnMonsterListChanged != null)
		{
			MSActionManager.Goon.OnMonsterListChanged();
		}
	}

	#region Selling

	public void SellMonsters(List<PZMonster> monsters)
	{
		SellUserMonsterRequestProto sellRequest = new SellUserMonsterRequestProto();
		sellRequest.sender = MSWhiteboard.localMupWithResources;

		foreach (var item in monsters) 
		{
			MSResourceManager.instance.Collect(ResourceType.CASH, item.sellValue);
			sellRequest.sales.Add(item.GetSellProto());
			RemoveMonster(item.userMonster.userMonsterId);
		}

		UMQNetworkManager.instance.SendRequest(sellRequest, (int)EventProtocolRequest.C_SELL_USER_MONSTER_EVENT, DealWithSellResponse);
	}

	void PrepareNewSellRequest()
	{
		sellRequest = new SellUserMonsterRequestProto();
		sellRequest.sender = MSWhiteboard.localMupWithResources;
	}

	public void SellMonster(PZMonster monster)
	{
		MSResourceManager.instance.Collect(ResourceType.CASH, monster.sellValue);

		if (sellRequest == null)
		{
			PrepareNewSellRequest();
		}

		MinimumUserMonsterSellProto sell = new MinimumUserMonsterSellProto();
		sell.userMonsterId = monster.userMonster.userMonsterId;
		sell.cashAmount = monster.sellValue;

		sellRequest.sales.Add(sell);

		RemoveMonster(monster.userMonster.userMonsterId);
	}

	void SendSellRequest()
	{
		if (sellRequest==null)
		{
			return;
		}
		UMQNetworkManager.instance.SendRequest(sellRequest, (int)EventProtocolRequest.C_SELL_USER_MONSTER_EVENT, DealWithSellResponse);
		sellRequest = null;
	}

	void DealWithSellResponse(int tagNum)
	{
		SellUserMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as SellUserMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != SellUserMonsterResponseProto.SellUserMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem selling monsters: " + response.status.ToString());
		}
	}

	#endregion

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
				request.sender = MSWhiteboard.localMup;
				request.userMonsterId = monster.userMonster.userMonsterId;
				request.teamSlotNum = (i+1);
				UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ADD_MONSTER_TO_BATTLE_TEAM_EVENT, DealWithAddResponse);
				
				if (MSActionManager.Goon.OnMonsterAddTeam != null)
				{
					MSActionManager.Goon.OnMonsterAddTeam(monster);
				}
				
				if (MSActionManager.Goon.OnTeamChanged != null)
				{
					MSActionManager.Goon.OnTeamChanged();
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
		request.sender = MSWhiteboard.localMup;
		request.userMonsterId = monster.userMonster.userMonsterId;
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REMOVE_MONSTER_FROM_BATTLE_TEAM_EVENT, DealWithRemoveResponse);
		
		monster.userMonster.teamSlotNum = 0;
		
		_monstersCount--;
		
		if (MSActionManager.Goon.OnMonsterRemoveTeam != null)
		{
			MSActionManager.Goon.OnMonsterRemoveTeam(monster);
		}
		
		if (MSActionManager.Goon.OnTeamChanged != null)
		{
			MSActionManager.Goon.OnTeamChanged();
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

	#region Combining
	
	/// <summary>
	/// Prepares a new combine pieces request.
	/// </summary>
	void PrepareNewCombinePiecesRequest()
	{
		combineRequestProto = new CombineUserMonsterPiecesRequestProto();
		combineRequestProto.sender = MSWhiteboard.localMup;
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
		combineRequestProto = null;
	}
	
	/// <summary>
	/// Updates or adds the specified monster.
	/// Monsters are updated with new pieces when the player collects monster pieces
	/// New monsters have just had their first piece collected, so they need a new entry
	/// in userMonsters
	/// </summary>
	/// <param name="monster">Monster.</param>
	public void UpdateOrAdd(FullUserMonsterProto monster)
	{
		PZMonster mon;
		if (userMonsters.Find(x=>x.userMonster.userMonsterId == monster.userMonsterId) != null)
		{
			Debug.Log("Updating monster: " + monster.userMonsterId);
			mon = userMonsters.Find(x=>x.userMonster.userMonsterId==monster.userMonsterId);
			mon.UpdateUserMonster(monster);
		}
		else
		{
			Debug.Log("Adding monster: " + monster.monsterId);
			mon = new PZMonster(monster);
			userMonsters.Add(new PZMonster(monster));
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
			monster.userMonster.combineStartTime = MSUtil.timeNowMillis;
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
	
	#endregion

	#region Sorters
	
	private class EnhancingMonsterSorter : Comparer<PZMonster>
	{
		public override int Compare (PZMonster x, PZMonster y)
		{
			return x.enhancement.expectedStartTimeMillis.CompareTo(y.enhancement.expectedStartTimeMillis);
		}
	}
	
	#endregion

	#region Enhancement
	
	/// <summary>
	/// Prepares a new enhance request.
	/// </summary>
	void PrepareNewEnhanceRequest()
	{
		Debug.Log("Preparing Enhance Request");
		enhanceRequestProto = new SubmitMonsterEnhancementRequestProto();
		enhanceRequestProto.sender = MSWhiteboard.localMupWithResources;
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

		Debug.Log("UMCEP: EXP: " + request.umcep.expectedExperience + ", LVL: " + request.umcep.expectedLevel);

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
	
	void SendStartEnhanceRequest ()
	{
		if (enhanceRequestProto == null)
		{
			return;
		}

		string str = "Start enhance request:";
		foreach (var item in enhanceRequestProto.ueipNew) 
		{
			str += "\nNew: " + item.userMonsterId;
		}
		foreach (var item in enhanceRequestProto.ueipUpdate) 
		{
			str += "\nUpdate: " + item.userMonsterId;
		}
		foreach (var item in enhanceRequestProto.ueipDelete) 
		{
			str += "\nDelete: " + item.userMonsterId;
		}
		Debug.LogWarning(str);
		
		UMQNetworkManager.instance.SendRequest(enhanceRequestProto, (int)EventProtocolRequest.C_SUBMIT_MONSTER_ENHANCEMENT_EVENT, DealWithEnhanceStartResponse);
		
		if (MSActionManager.Goon.OnEnhanceQueueChanged != null)
		{
			MSActionManager.Goon.OnEnhanceQueueChanged();
		}
	}
	
	void Feed (PZMonster feeder)
	{
		currentEnhancementMonster.GainXP(feeder.enhanceXP);
		enhancementFeeders.Remove(feeder);
		RemoveMonster(feeder.userMonster.userMonsterId);
	}
	
	void CheckEnhancingMonsters()
	{
		if (enhancementFeeders.Count > 0 && enhancementFeeders[0].finishEnhanceTime <= MSUtil.timeNowMillis)
		{
			
			EnhancementWaitTimeCompleteRequestProto request = new EnhancementWaitTimeCompleteRequestProto();
			request.sender = MSWhiteboard.localMup;
			request.isSpeedup = false;
			
			PZMonster feeder;
			while (enhancementFeeders.Count > 0 && enhancementFeeders[0].finishEnhanceTime <= MSUtil.timeNowMillis)
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
	
	public void SpeedUpEnhance(int cost)
	{
		EnhancementWaitTimeCompleteRequestProto request = new EnhancementWaitTimeCompleteRequestProto();
		request.sender = MSWhiteboard.localMup;
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
	
	void DealWithEnhanceCompleteResponse(int tagNum)
	{
		EnhancementWaitTimeCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as EnhancementWaitTimeCompleteResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != EnhancementWaitTimeCompleteResponseProto.EnhancementWaitTimeCompleteStatus.SUCCESS)
		{
			Debug.LogError("Problem completing enhancement: " + response.status.ToString());
		}
		
		if (MSActionManager.Goon.OnEnhanceQueueChanged != null)
		{
			MSActionManager.Goon.OnEnhanceQueueChanged();
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
		else if (MSResourceManager.instance.Spend(ResourceType.OIL, monster.enhanceXP, delegate{AddToEnhanceQueue(monster);}))
		{
			if (enhancementFeeders.Count == 0)
			{
				monster.enhancement.expectedStartTimeMillis = MSUtil.timeNowMillis;
			}
			else
			{
				monster.enhancement.expectedStartTimeMillis = enhancementFeeders[enhancementFeeders.Count-1].finishEnhanceTime;
			}
			enhancementFeeders.Add(monster);
			enhanceRequestProto.oilChange -= monster.enhanceXP;

			if (MSActionManager.Goon.OnMonsterAddQueue != null)
			{
				MSActionManager.Goon.OnMonsterAddQueue(monster);
			}
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
		

		if (MSActionManager.Goon.OnEnhanceQueueChanged != null)
		{
			MSActionManager.Goon.OnEnhanceQueueChanged();
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
				feeder.enhancement.expectedStartTimeMillis = MSUtil.timeNowMillis;
			}
			else
			{
				feeder.enhancement.expectedStartTimeMillis = enhancementFeeders[i-1].finishEnhanceTime;
			}
		}
		
		monster.enhancement = null;
		
		enhanceRequestProto.oilChange += monster.enhanceXP;
		
		MSResourceManager.instance.Collect(ResourceType.OIL, monster.enhanceXP);


		if (MSActionManager.Goon.OnMonsterRemoveQueue != null)
		{
			MSActionManager.Goon.OnMonsterRemoveQueue(monster);
		}
		if (MSActionManager.Goon.OnEnhanceQueueChanged != null)
		{
			MSActionManager.Goon.OnEnhanceQueueChanged();
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
		
		if (MSActionManager.Goon.OnEnhanceQueueChanged != null)
		{
			MSActionManager.Goon.OnEnhanceQueueChanged();
		}
		
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
		
		if (MSActionManager.Goon.OnHealQueueChanged != null)
		{
			MSActionManager.Goon.OnHealQueueChanged();
		}
	}

	#endregion

	#region Evolution

	public PZMonster FindEvolutionBuddy(PZMonster monster)
	{
		if (monster.userMonster.currentLvl < monster.monster.maxLevel)
		{
			return null;
		}
		foreach (var item in GetMonstersByMonsterId(monster.monster.monsterId)) 
		{
			if (item.userMonster.currentLvl >= monster.monster.maxLevel 
			    && item.userMonster.userMonsterId > monster.userMonster.userMonsterId)
			{
				return item;
			}
		}
		return null;
	}

	#endregion

	void OnPopupClosed()
	{
		SendRequests();
	}
	
	void SendRequests()
	{
		SendStartEnhanceRequest();
		SendSellRequest();
	}
	
	void OnApplicationQuit()
	{
		SendRequests();
	}
}
					

