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
	
	public const int TEAM_SLOTS = 3;
	
	private int _monstersCount;
	
	public int monstersOnTeam
	{
		get
		{
			return _monstersCount;
		}
	}
	
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
	
	void Update()
	{
		CheckHealingMonsters();
	}

	void SendHealRequest (HealMonsterWaitTimeCompleteRequestProto request)
	{
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_HEAL_MONSTER_WAIT_TIME_COMPLETE_EVENT, DealWithHealCompleteResponse);
		
		if (CBKEventManager.Goon.OnHealQueueChanged != null)
		{
			CBKEventManager.Goon.OnHealQueueChanged();
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
			SendHealRequest (request);
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
			CompleteHeal(item);
		}
		SendHealRequest(request);
	}
	
	void CompleteHeal(PZMonster monster)
	{
		healingMonsters.Remove(monster);
		monster.healingMonster = null;
		monster.currHP = monster.maxHP;
	}
			
	void DealWithHealCompleteResponse(int tagNum)
	{
		HealMonsterWaitTimeCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as HealMonsterWaitTimeCompleteResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != HealMonsterWaitTimeCompleteResponseProto.HealMonsterWaitTimeCompleteStatus.SUCCESS)
		{
			Debug.LogError("Problem healing with time: " + response.status.ToString());
		}
	}
	
	public void UpdateOrAddAll(List<FullUserMonsterProto> monsters)
	{
		foreach (FullUserMonsterProto item in monsters) 
		{
			UpdateOrAdd(item);
		}
	}
	
	public void UpdateOrAdd(FullUserMonsterProto monster)
	{
		if (userMonsters.ContainsKey(monster.userMonsterId))
		{
			Debug.Log("Updating monster: " + monster.userMonsterId);
			userMonsters[monster.userMonsterId].UpdateUserMonster(monster);
			
			
		}
		else
		{
			Debug.Log("Adding monster: " + monster.monsterId);
			userMonsters.Add(monster.userMonsterId, new PZMonster(monster));
		}
	}
	
	public void StartMonsterCombine(PZMonster monster)
	{
		
	}
	
	public void AddToHealQueue(PZMonster monster)
	{
		HealMonsterRequestProto request = new HealMonsterRequestProto();
		request.sender = CBKWhiteboard.localMup;
		
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
		
		request.umhNew.Add(monster.healingMonster);
		
		request.cashCost = monster.healCost;
		
		CBKResourceManager.instance.Spend(CBKResourceManager.ResourceType.FREE, request.cashCost);
		
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_HEAL_MONSTER_EVENT, DealWithHealResponse);
		
		if (CBKEventManager.Goon.OnHealQueueChanged != null)
		{
			CBKEventManager.Goon.OnHealQueueChanged();
		}
	}
	
	public void RemoveFromHealQueue(PZMonster monster)
	{
		HealMonsterRequestProto request = new HealMonsterRequestProto();
		request.sender = CBKWhiteboard.localMup;
		
		int i;
		for (i = 0; i < healingMonsters.Count; i++) 
		{
			if (healingMonsters[i] == monster)
			{
				request.umhDelete.Add(healingMonsters[i].healingMonster);
				break;
			}
		}
		healingMonsters.RemoveAt(i);
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
			request.umhUpdate.Add (healingMonsters[i].healingMonster);
		}
		
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_HEAL_MONSTER_EVENT, DealWithHealResponse);
		
		if (CBKEventManager.Goon.OnHealQueueChanged != null)
		{
			CBKEventManager.Goon.OnHealQueueChanged();
		}
	}
	
	void DealWithHealResponse(int tagNum)
	{
		HealMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as HealMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != HealMonsterResponseProto.HealMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem sending heal request: " + response.status.ToString());
		}
	}
	
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
	
	private class HealingMonsterSorter : Comparer<PZMonster>
	{
		public override int Compare (PZMonster x, PZMonster y)
		{
			return x.healingMonster.expectedStartTimeMillis.CompareTo(y.healingMonster.expectedStartTimeMillis);
		}
	}
	
}
					

