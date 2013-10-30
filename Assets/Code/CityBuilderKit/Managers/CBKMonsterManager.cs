using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBK monster manager.
/// Keeps track of what monsters are healing, what monsters are 
/// </summary>
public class CBKMonsterManager : MonoBehaviour {
	
	public Dictionary<long, PZMonster> userMonsters = new Dictionary<long, PZMonster>();
	
	public PZMonster[] userTeam = new PZMonster[TEAM_SLOTS];
	
	public List<PZMonster> healingMonsters = new List<PZMonster>();
	
	public List<PZMonster> enhancementFeeders = new List<PZMonster>();
	
	public PZMonster currentEnhancementMonster;

	public const int TEAM_SLOTS = 3;
	
	private int _monstersCount;
	
	public int monstersOnTeam
	{
		get
		{
			return _monstersCount;
		}
	}
	
	SubmitMonsterEnhancementRequestProto enhanceRequestProto;
	
	HealMonsterRequestProto healRequestProto;
	
	CombineUserMonsterPiecesRequestProto combineRequestProto;
	
	public static CBKMonsterManager instance;
	
	void Awake()
	{
		instance = this;
	}
	
	public void Init(List<FullUserMonsterProto> monsters, List<UserMonsterHealingProto> healing)
	{
		PZMonster mon;
		_monstersCount = 0;
		foreach (FullUserMonsterProto item in monsters) 
		{
			mon = new PZMonster(item);
			Debug.Log("Adding monster " + item.monsterId);
			userMonsters.Add(item.userMonsterId, mon);
			if (item.teamSlotNum > 0)
			{
				userTeam[(item.teamSlotNum-1)] = mon; //Fucking off-by-ones.
				_monstersCount++;
			}
		}
		foreach (UserMonsterHealingProto item in healing) 
		{
			mon = userMonsters[item.userMonsterId];
			mon.healingMonster = item;
			healingMonsters.Add(mon);
		}
		
		healingMonsters.Sort(new HealingMonsterSorter());
	}
	
	void OnEnable()
	{
		CBKEventManager.Popup.CloseAllPopups += SendRequests;
	}
	
	void OnDisable()
	{
		CBKEventManager.Popup.CloseAllPopups -= SendRequests;
	}
	
	void Update()
	{
		CheckHealingMonsters();
		
		CheckEnhancingMonsters();
	}
	
	void PrepareNewHealRequest()
	{
		healRequestProto = new HealMonsterRequestProto();
		healRequestProto.sender = CBKWhiteboard.localMup;
	}
	
	void PrepareNewEnhanceRequest()
	{
		enhanceRequestProto = new SubmitMonsterEnhancementRequestProto();
		enhanceRequestProto.sender = CBKWhiteboard.localMup;
	}
	
	void PrepareNewCombinePiecesRequest()
	{
		combineRequestProto = new CombineUserMonsterPiecesRequestProto();
		combineRequestProto.sender = CBKWhiteboard.localMup;
	}
	
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
	}
	
	void SendStartHealRequest ()
	{
		if (healRequestProto == null)
		{
			return;
		}
		
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
			
			if (enhancementFeeders.Count == 0)
			{
				currentEnhancementMonster.enhancement = null;
			}
		}
	}
	
	void CheckHealingMonsters()
	{
		if (healingMonsters.Count > 0 && healingMonsters[0].finishHealTimeMillis <= CBKUtil.timeNowMillis)
		{
			HealMonsterWaitTimeCompleteRequestProto request = new HealMonsterWaitTimeCompleteRequestProto();
			request.sender = CBKWhiteboard.localMup;
			request.isSpeedup = false;
			UserMonsterCurrentHealthProto health;
			while (healingMonsters.Count > 0 && healingMonsters[0].healTimeLeft <= 0) 
			{
				health = new UserMonsterCurrentHealthProto();
				health.userMonsterId = healingMonsters[0].healingMonster.userMonsterId;
				health.currentHealth = healingMonsters[0].maxHP;
				request.umchp.Add(health);
				CompleteHeal(healingMonsters[0]);
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
	
	void StartMonsterCombine(PZMonster monster)
	{
		if (monster.monster.minutesToCombinePieces == 0)
		{
			CombineMonster(monster);
		}
		//TODO: Combining list
	}
	
	void CombineMonster(PZMonster monster)
	{
		if (combineRequestProto == null)
		{
			PrepareNewCombinePiecesRequest();
		}
		
		monster.userMonster.isComplete = true;
		
		combineRequestProto.userMonsterIds.Add(monster.userMonster.userMonsterId);
		
	}
	
	public void AddToHealQueue(PZMonster monster)
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		monster.healingMonster = new UserMonsterHealingProto();
		monster.healingMonster.userId = CBKWhiteboard.localMup.userId;
		monster.healingMonster.userMonsterId = monster.userMonster.userMonsterId;
		if (healingMonsters.Count == 0)
		{
			monster.healingMonster.expectedStartTimeMillis = CBKUtil.timeNowMillis;
		}
		else
		{
			monster.healingMonster.expectedStartTimeMillis = healingMonsters[healingMonsters.Count-1].finishHealTimeMillis;
		}
		
		healingMonsters.Add (monster);
		
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
		
		CBKResourceManager.instance.Spend(CBKResourceManager.ResourceType.FREE, monster.healCost);
		
		if (CBKEventManager.Goon.OnHealQueueChanged != null)
		{
			CBKEventManager.Goon.OnHealQueueChanged();
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
		}
		
		healRequestProto.cashChange -= monster.healCost;
		
		monster.healingMonster = null;
		
		for (; i < healingMonsters.Count; i++) 
		{
			if (i==0)
			{
				healingMonsters[i].healingMonster.expectedStartTimeMillis = CBKUtil.timeNowMillis;
			}
			else
			{
				healingMonsters[i].healingMonster.expectedStartTimeMillis = healingMonsters[i-1].finishHealTimeMillis;
			}
			healRequestProto.umhUpdate.Add (healingMonsters[i].healingMonster);
		}
		
		CBKResourceManager.instance.Collect(CBKResourceManager.ResourceType.FREE, monster.healCost);
		
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
		if (currentEnhancementMonster == null)
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
			
			CBKResourceManager.instance.Spend(CBKResourceManager.ResourceType.FREE, monster.enhanceXP);
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
		
		CBKResourceManager.instance.Collect(CBKResourceManager.ResourceType.FREE, monster.enhanceXP);
		
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
					

