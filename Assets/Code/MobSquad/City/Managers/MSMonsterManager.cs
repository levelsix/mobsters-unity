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

	public bool currEnhancementMonsterOnServer = false;

	public int totalResidenceSlots;

	public const int TEAM_SLOTS = 3;

	private static int _monstersCount;
	
	public static int monstersOwned
	{
		get
		{
			return _monstersCount;
		}
	}

	public bool isEnhancing
	{
		get
		{
			return currentEnhancementMonster != null
				&& currentEnhancementMonster.monster != null
					&& currentEnhancementMonster.monster.monsterId > 0;
		}
	}
	
//	SubmitMonsterEnhancementRequestProto enhanceRequestProto = new SubmitMonsterEnhancementRequestProto();
	EnhanceMonsterRequestProto enhanceRequestProto = new EnhanceMonsterRequestProto();
	
	CombineUserMonsterPiecesRequestProto combineRequestProto = null;

	SellUserMonsterRequestProto sellRequest = null;

	public static MSMonsterManager instance;

//	public long lastEnhance
//	{
//		get
//		{
//			long last = 0;
//			foreach (var item in enhancementFeeders) {
//				last = Math.Max(last, item.finishEnhanceTime);
//			}
//			return last;
//		}
//	}
	
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
		enhanceRequestProto.uep = new UserEnhancementProto();

		PZMonster mon;
		_monstersCount = 0;
		userMonsters.Clear();
		for (int i = 0; i < TEAM_SLOTS; i++) 
		{
			userTeam[i] = null;
		}
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

			currEnhancementMonsterOnServer = true;
			
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
	// TODO: Change this to a method that only gets called every second.
	/// We don't need to check all of these things every step, since they run on a second-to-second basis anyways.
	/// </summary>
	void Update()
	{
		//monster don't run on time any more so this is unneccisary un-nescisary unnessisary
//		CheckEnhancingMonsters();
		
		CheckCombiningMonsters();
	}

	public List<PZMonster> GetMonstersByMonsterId(long monsterId, long notMonster = 0)
	{
		return userMonsters.FindAll(x=> x.monster.monsterId == monsterId && x.userMonster.userMonsterId != notMonster);
	}

	public void RemoveMonster(long userMonsterId)
	{
		userMonsters.RemoveAll(x=>x.userMonster.userMonsterId==userMonsterId);
		if (MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory != null)
		{
			MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory(userMonsterId);
		}
		if (MSActionManager.Goon.OnMonsterListChanged != null)
		{
			MSActionManager.Goon.OnMonsterListChanged();
		}
	}

	/// <summary>
	/// Updates or adds the specified monster.
	/// Monsters are updated with new pieces when the player collects monster pieces
	/// New monsters have just had their first piece collected, so they need a new entry
	/// in userMonsters
	/// </summary>
	/// <param name="monster">Monster.</param>
	public PZMonster UpdateOrAdd(FullUserMonsterProto monster)
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
		if (MSActionManager.Goon.OnMonsterListChanged != null)
		{
			MSActionManager.Goon.OnMonsterListChanged();
		}
		return mon;
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
		if (MSActionManager.Goon.OnMonsterListChanged != null)
		{
			MSActionManager.Goon.OnMonsterListChanged();
		}
	}

	public Coroutine RestrictMonster(long userMonsterId)
	{
		return StartCoroutine(Restrict(userMonsterId));
	}

	IEnumerator Restrict(long userMonsterId)
	{
		RestrictUserMonsterRequestProto request = new RestrictUserMonsterRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.userMonsterIds.Add(userMonsterId);
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RESTRICT_USER_MONSTER_EVENT);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		RestrictUserMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as RestrictUserMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != RestrictUserMonsterResponseProto.RestrictUserMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem restricting monster: " + response.status.ToString());
		}
	}

	public Coroutine UnrestrictMonster(long userMonsterId)
	{
		return StartCoroutine(Unrestrict(userMonsterId));
	}
	
	IEnumerator Unrestrict(long userMonsterId)
	{
		UnrestrictUserMonsterRequestProto request = new UnrestrictUserMonsterRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.userMonsterIds.Add(userMonsterId);
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UNRESTRICT_USER_MONSTER_EVENT);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		UnrestrictUserMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as UnrestrictUserMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != UnrestrictUserMonsterResponseProto.UnrestrictUserMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem unrestricting monster: " + response.status.ToString());
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

		if (MSActionManager.Quest.OnMonstersSold != null)
		{
			MSActionManager.Quest.OnMonstersSold(monsters.Count);
		}

		UMQNetworkManager.instance.SendRequest(sellRequest, (int)EventProtocolRequest.C_SELL_USER_MONSTER_EVENT, DealWithSellResponse);
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
				
				return i+1;
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
			MSSceneManager.instance.ReconnectPopup();
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
			MSSceneManager.instance.ReconnectPopup();
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
	
	void SendCombineRequest()
	{
		UMQNetworkManager.instance.SendRequest(combineRequestProto, (int)EventProtocolRequest.C_COMBINE_USER_MONSTER_PIECES_EVENT, DealWithCombineResponse);
		combineRequestProto = null;
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
				CombineMonster(item);
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
	
//	#endregion
//
//	#region Sorters
//	
//	private class EnhancingMonsterSorter : Comparer<PZMonster>
//	{
//		public override int Compare (PZMonster x, PZMonster y)
//		{
//			return x.enhancement.expectedStartTimeMillis.CompareTo(y.enhancement.expectedStartTimeMillis);
//		}
//	}
//	
//	#endregion
//
//	#region Enhancement
//	
//	IEnumerator SendCompleteEnhanceRequest(EnhancementWaitTimeCompleteRequestProto request)
//	{
//		yield return DoSendStartEnhanceRequest();
//
//		Debug.Log("UMCEP: EXP: " + request.umcep.expectedExperience + ", LVL: " + request.umcep.expectedLevel);
//
//		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT, null);
//
//		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
//		{
//			yield return null;
//		}
//		
//		EnhancementWaitTimeCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as EnhancementWaitTimeCompleteResponseProto;
//		UMQNetworkManager.responseDict.Remove(tagNum);
//		
//		if (response.status != EnhancementWaitTimeCompleteResponseProto.EnhancementWaitTimeCompleteStatus.SUCCESS)
//		{
//			Debug.LogError("Enhance Wait Complete done messed: " + response.status.ToString());
//		}
//		else
//		{
//			if (enhancementFeeders.Count == 0 && !MSDoEnhanceScreen.instance.gameObject.activeSelf)
//			{
//				RemoveFromEnhanceQueue(currentEnhancementMonster);
//			}
//			
//			DoSendStartEnhanceRequest();
//
//		}
//		
//	}

//	IEnumerator SendCompleteEnhanceRequest(EnhanceMonsterRequestProto request)
//	{
//		yield return DoSendStartEnhanceRequest();
//				Debug.Log("UMCEP: EXP: " + request.umcep.expectedExperience + ", LVL: " + request.umcep.expectedLevel);
//	
//		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT, null);
//	
//		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
//		{
//			yield return null;
//		}
//			
//		EnhancementWaitTimeCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as EnhancementWaitTimeCompleteResponseProto;
//		UMQNetworkManager.responseDict.Remove(tagNum);
//			
//		if (response.status != EnhancementWaitTimeCompleteResponseProto.EnhancementWaitTimeCompleteStatus.SUCCESS)
//		{
//			Debug.LogError("Enhance Wait Complete done messed: " + response.status.ToString());
//		}
//		else
//		{
//			if (enhancementFeeders.Count == 0 && !MSDoEnhanceScreen.instance.gameObject.activeSelf)
//			{
//				RemoveFromEnhanceQueue(currentEnhancementMonster);
//			}
//				
//			DoSendStartEnhanceRequest();
//	
//		}
//			
//	}
//
//	public Coroutine DoSendStartEnhanceRequest()
//	{
//		return StartCoroutine(SendStartEnhanceRequest());
//	}
//	
//	public IEnumerator SendStartEnhanceRequest ()
//	{
//		enhanceRequestProto.sender = MSWhiteboard.localMupWithResources;
//
//		if (enhanceRequestProto.ueipNew.Count == 0
//		    && enhanceRequestProto.ueipUpdate.Count == 0
//		    && enhanceRequestProto.ueipDelete.Count == 0)
//		{
//			yield break;
//		}
//
//		string str = "Start enhance request:";
//		foreach (var item in enhanceRequestProto.ueipNew) 
//		{
//			str += "\nNew: " + item.userMonsterId;
//		}
//		foreach (var item in enhanceRequestProto.ueipUpdate) 
//		{
//			str += "\nUpdate: " + item.userMonsterId;
//		}
//		foreach (var item in enhanceRequestProto.ueipDelete) 
//		{
//			str += "\nDelete: " + item.userMonsterId;
//		}
//		Debug.LogWarning(str);
//
//		//If we're only adding the enhancement mobster, don't send it unless
//		//there are feeders too
//		if (enhanceRequestProto.ueipNew.Count == 1
//		    && enhanceRequestProto.ueipNew[0].userMonsterId == currentEnhancementMonster.userMonster.userMonsterId)
//		{
//			yield break;
//		}
//		
//		int tagNum = UMQNetworkManager.instance.SendRequest(enhanceRequestProto, (int)EventProtocolRequest.C_SUBMIT_MONSTER_ENHANCEMENT_EVENT);
//
//		enhanceRequestProto.ueipNew.Clear();
//		enhanceRequestProto.ueipUpdate.Clear();
//		enhanceRequestProto.ueipDelete.Clear();
//
//		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
//		{
//			yield return null;
//		}
//		
//		SubmitMonsterEnhancementResponseProto response = UMQNetworkManager.responseDict[tagNum] as SubmitMonsterEnhancementResponseProto;
//		UMQNetworkManager.responseDict.Remove(tagNum);
//		
//		if (response.status != SubmitMonsterEnhancementResponseProto.SubmitMonsterEnhancementStatus.SUCCESS)
//		{
//			Debug.LogError("Problem sending enhance request: " + response.status.ToString ());
//		}
//
//	}
	
	int Feed (PZMonster feeder)
	{
		currentEnhancementMonster.GainXP(feeder.enhanceXP);
		enhancementFeeders.Remove(feeder);
		RemoveMonster(feeder.userMonster.userMonsterId);
		if(MSActionManager.Goon.OnFinnishFeeding != null)
		{
			MSActionManager.Goon.OnFinnishFeeding(feeder);
		}
		return feeder.enhanceXP;
	}

	//enhancing no longer runs on timers
//	void CheckEnhancingMonsters()
//	{
//		if (enhancementFeeders.Count > 0 && enhancementFeeders[0].finishEnhanceTime <= MSUtil.timeNowMillis)
//		{
//			
//			EnhancementWaitTimeCompleteRequestProto request = new EnhancementWaitTimeCompleteRequestProto();
//			request.sender = MSWhiteboard.localMup;
//			request.isSpeedup = false;
//			
//			PZMonster feeder;
//			int enhancePoints = 0;
//			while (enhancementFeeders.Count > 0 && enhancementFeeders[0].finishEnhanceTime <= MSUtil.timeNowMillis)
//			{
//				feeder = enhancementFeeders[0];
//				enhancePoints += Feed (feeder);
//				
//				request.userMonsterIds.Add(feeder.userMonster.userMonsterId);
//			}
//			
//			request.umcep = currentEnhancementMonster.GetCurrentExpProto();
//			
//			StartCoroutine(SendCompleteEnhanceRequest(request));
//
//			if (MSActionManager.Quest.OnMonsterEnhanced != null)
//			{
//				MSActionManager.Quest.OnMonsterEnhanced(enhancePoints);
//			}
//		}
//	}

//	public void DoSpeedUpEnhance(int cost, MSLoadLock loadLock)
//	{
//		StartCoroutine(SpeedUpEnhance(cost, loadLock));
//	}
//
//	public IEnumerator SpeedUpEnhance(int cost, MSLoadLock loadLock)
//	{
//		loadLock.Lock();
//
//		EnhancementWaitTimeCompleteRequestProto request = new EnhancementWaitTimeCompleteRequestProto();
//		request.sender = MSWhiteboard.localMup;
//		request.isSpeedup = true;
//		request.gemsForSpeedup = cost;
//		
//		request.umcep = currentEnhancementMonster.GetCurrentExpProto();
//		foreach (var item in enhancementFeeders) 
//		{
//			request.userMonsterIds.Add(item.userMonster.userMonsterId);
//			request.umcep.expectedExperience += item.enhanceXP;
//		}
//
//		request.umcep.expectedLevel = (int)currentEnhancementMonster.LevelWithFeeders(enhancementFeeders);
//		request.umcep.expectedHp = currentEnhancementMonster.MaxHPAtLevel(request.umcep.expectedLevel);
//
//		currentEnhancementMonster.enhancement = null;
//		
//		yield return StartCoroutine(SendCompleteEnhanceRequest(request));
//
//		while(enhancementFeeders.Count > 0)
//		{
//			Feed(enhancementFeeders[0]);
//		}
//
//		if (MSActionManager.Goon.OnEnhanceQueueChanged != null)
//		{
//			MSActionManager.Goon.OnEnhanceQueueChanged();
//		}
//
//		loadLock.Unlock();
//	}

	public void DoEnhanceRequest(int oilCost, int gemCost, MSLoadLock loadLock)
	{
		enhanceRequestProto.oilChange = -oilCost;
		enhanceRequestProto.gemsSpent = gemCost;
		//		UserEnhancementProto
		enhanceRequestProto.uep.baseMonster = currentEnhancementMonster.enhancement;
		enhanceRequestProto.uep.feeders.Clear();
		enhanceRequestProto.uep.userId = MSWhiteboard.localUser.userId;

//		Debug.Log("enhancing " + currentEnhancementMonster.monster.displayName);
		int exp = 0;
		foreach(PZMonster feeder in enhancementFeeders)
		{
//			Debug.Log("feeder: " + feeder.monster.displayName);
			exp += feeder.enhanceXP;
			enhanceRequestProto.uep.feeders.Add(feeder.enhancement);
		}
//		Debug.Log("costs " + oilCost + ":oil  " + gemCost + ":gems");
		
		//		enhanceRequestProto current exp proto
		enhanceRequestProto.enhancingResult = currentEnhancementMonster.GetCurrentExpProto();
		enhanceRequestProto.enhancingResult.expectedExperience += exp;
		enhanceRequestProto.enhancingResult.expectedLevel = (int)currentEnhancementMonster.LevelWithFeeders(enhancementFeeders);
		enhanceRequestProto.enhancingResult.expectedHp = currentEnhancementMonster.MaxHPAtLevel(enhanceRequestProto.enhancingResult.expectedLevel);
		//		MinimumUserProto
		enhanceRequestProto.sender = MSWhiteboard.localMup;

		StartCoroutine(EnhanceRequest(loadLock));
	}

	public IEnumerator EnhanceRequest(MSLoadLock loadLock)
	{
		yield return null;
		loadLock.Lock();
		EnhanceMonsterRequestProto request = enhanceRequestProto;
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ENHANCE_MONSTER_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		EnhanceMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as EnhanceMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != EnhanceMonsterResponseProto.EnhanceMonsterStatus.SUCCESS)
		{
			Debug.LogError("Sending Enhance failed : " + response.status.ToString());
		}
		else
		{
			while(enhancementFeeders.Count > 0)
			{
				enhancementFeeders.ToString();
				Feed(enhancementFeeders[0]);
			}
			
			if (MSActionManager.Goon.OnEnhanceQueueChanged != null)
			{
				MSActionManager.Goon.OnEnhanceQueueChanged();
			}
			
			if (MSActionManager.Quest.OnMonsterEnhanced != null)
			{
				MSActionManager.Quest.OnMonsterEnhanced(enhanceRequestProto.enhancingResult.expectedExperience);
			}
		}
		loadLock.Unlock();
	}

	public bool AddToEnhanceQueue(PZMonster monster, Action successCallback = null, bool useGems = false)
	{
		monster.enhancement = new UserEnhancementItemProto();
		monster.enhancement.userMonsterId = monster.userMonster.userMonsterId;
		monster.enhancement.enhancingCost = monster.enhanceCost;
		
		//If this is the new base monster, set it up as such
		if (currentEnhancementMonster == null || currentEnhancementMonster.monster == null || currentEnhancementMonster.monster.monsterId == 0)
		{
			monster.enhancement.expectedStartTimeMillis = 0;
			currentEnhancementMonster = monster;
			currEnhancementMonsterOnServer = false;
		}
		else
		{
			if (enhancementFeeders.Count >= MSBuildingManager.enhanceLabs[0].combinedProto.lab.queueSize)
			{
				MSActionManager.Popup.DisplayError("Enhance queue full!");
				return false;
			}

			if (!currEnhancementMonsterOnServer)
			{
				currEnhancementMonsterOnServer = true;
				//no longer use protos with timer for enhance
//				enhanceRequestProto.ueipNew.Add(currentEnhancementMonster.enhancement);
			}

			//no longer spends resources when adding to list
//			if (useGems)
//			{
//				int gemCost = Mathf.CeilToInt((currentEnhancementMonster.enhanceCost - MSResourceManager.resources[ResourceType.OIL]) * MSWhiteboard.constants.gemsPerResource);
//				if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost))
//				{
//					AddToEnhanceQueue(monster, MSResourceManager.instance.SpendAll(ResourceType.OIL), gemCost);
//				}
//			}
//			else if (MSResourceManager.instance.Spend(ResourceType.OIL, currentEnhancementMonster.enhanceCost, delegate{AddToEnhanceQueue(monster, successCallback, true);}))
//			{
				AddToEnhanceQueue(monster, currentEnhancementMonster.enhanceCost);
//			}
//			else
//			{
//				return false;
//			}
		}
		return true;

	}

	void AddToEnhanceQueue(PZMonster monster, int oil, int gems = 0)
	{
		//no more timer
//		if (enhancementFeeders.Count == 0)
//		{
//			monster.enhancement.expectedStartTimeMillis = MSUtil.timeNowMillis;
//		}
//		else
//		{
//			monster.enhancement.expectedStartTimeMillis = enhancementFeeders[enhancementFeeders.Count-1].finishEnhanceTime;
//		}
		enhancementFeeders.Add(monster);
		enhanceRequestProto.oilChange -= oil;
		enhanceRequestProto.gemsSpent += gems;
		
		if (MSActionManager.Goon.OnMonsterAddQueue != null)
		{
			MSActionManager.Goon.OnMonsterAddQueue(monster);
		}
		//no longer use protos with timer for enhance
//		if (enhanceRequestProto.ueipDelete.Contains(monster.enhancement))
//		{
//			enhanceRequestProto.ueipDelete.Remove(monster.enhancement);
//			enhanceRequestProto.ueipUpdate.Add(monster.enhancement);
//		}
//		else if (!enhanceRequestProto.ueipNew.Contains(monster.enhancement)) //Just in case of redundancies...
//		{
//			enhanceRequestProto.ueipNew.Add(monster.enhancement);
//		}
		

		if (MSActionManager.Goon.OnEnhanceQueueChanged != null)
		{
			MSActionManager.Goon.OnEnhanceQueueChanged();
		}
	}
	
	public void RemoveFromEnhanceQueue(PZMonster monster)
	{
		int i;
		for (i = 0; i < enhancementFeeders.Count; i++) 
		{
			if (enhancementFeeders[i] == monster)
			{
				enhancementFeeders.RemoveAt(i);
				break;
			}
		}

		//no longer use protos with timer in enhance
//		if (monster != currentEnhancementMonster || currEnhancementMonsterOnServer)
//		{
//			if (enhanceRequestProto.ueipNew.Contains(monster.enhancement))
//			{
//				enhanceRequestProto.ueipNew.Remove(monster.enhancement);
//			}
//			else
//			{
//				enhanceRequestProto.ueipDelete.Add(monster.enhancement);
//			}
//		}
		
		//Update the rest of the feeders
		//removed because we don't use this proto any more
//		PZMonster feeder;
//		for (; i < enhancementFeeders.Count; i++) 
//		{
//			feeder = enhancementFeeders[i];
//			enhanceRequestProto.ueipUpdate.Add(feeder.enhancement);
//			//no more timer
////			if (i == 0)
////			{
////				feeder.enhancement.expectedStartTimeMillis = MSUtil.timeNowMillis;
////			}
////			else
////			{
////				feeder.enhancement.expectedStartTimeMillis = enhancementFeeders[i-1].finishEnhanceTime;
////			}
//		}
		
		monster.enhancement = null;

		if (monster == currentEnhancementMonster)
		{
			currentEnhancementMonster = null;
		}
		else
		{
			enhanceRequestProto.oilChange += currentEnhancementMonster.enhanceCost;

			//no need to refund the cost with not timer
			//MSResourceManager.instance.Collect(ResourceType.OIL, monster.enhanceXP);

			//if (enhancementFeeders.Count == 0)
			//{
				//RemoveFromEnhanceQueue(currentEnhancementMonster);
			//}
		}

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
		//these protos aren't being used any more
//		//Remove from the back; more efficient
//		while(enhancementFeeders.Count > 0)
//		{
//			RemoveFromEnhanceQueue(enhancementFeeders[enhancementFeeders.Count-1]);
//		}
//		
//		if (enhanceRequestProto.ueipNew.Contains(currentEnhancementMonster.enhancement))
//		{
//			enhanceRequestProto.ueipNew.Remove(currentEnhancementMonster.enhancement);
//		}
//		else
//		{
//			enhanceRequestProto.ueipDelete.Add(currentEnhancementMonster.enhancement);
//		}

		enhancementFeeders.Clear();
		
//		currentEnhancementMonster.enhancement = null;
		
		currentEnhancementMonster = null;
		
		if (MSActionManager.Goon.OnEnhanceQueueChanged != null)
		{
			MSActionManager.Goon.OnEnhanceQueueChanged();
		}
		
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

	public void SetMobsterAsAvatar(int monsterId)
	{
		SetAvatarMonsterRequestProto request = new SetAvatarMonsterRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.monsterId = monsterId;

		MSWhiteboard.localUser.avatarMonsterId = monsterId;
		MSWhiteboard.localMup.avatarMonsterId = monsterId;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_SET_AVATAR_MONSTER_EVENT, DealWithSetAvatarResponse);
	}

	void DealWithSetAvatarResponse(int tagNum)
	{
		SetAvatarMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as SetAvatarMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != SetAvatarMonsterResponseProto.SetAvatarMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem setting player avatar: " + response.status.ToString());
		}
	}


	void OnPopupClosed()
	{
		SendRequests();
	}
	
	void SendRequests()
	{
	}
	
	void OnApplicationQuit()
	{
		SendRequests();
	}

	[ContextMenu ("Debug Dump")]
	public void DebugDump()
	{
		foreach (var item in userMonsters) 
		{
			Debug.Log("Monster: " + item.monster.displayName
			          + "Level: " + item.userMonster.currentLvl
			          + "Status: " + item.monsterStatus);
		}
	}
}
					

