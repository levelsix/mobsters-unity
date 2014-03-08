using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSHospitalManager
/// </summary>
public class MSHospitalManager : MonoBehaviour {

	public static MSHospitalManager instance;

	public List<PZMonster> healingMonsters = new List<PZMonster>();

	public List<CBKBuilding> hospitals = new List<CBKBuilding>();

	public int queueSize = 0;
	
	HealMonsterRequestProto healRequestProto = null;

	public bool initalized = false;

	void Awake()
	{
		instance = this;
	}

	public void Init(List<UserMonsterHealingProto> healing)
	{
		PZMonster mon;
		foreach (var item in healing) 
		{
			mon = MSMonsterManager.instance.userMonsters.Find(x => x.userMonster.userMonsterId == item.userMonsterId);
			mon.healingMonster = item;
			MSHospitalManager.instance.healingMonsters.Add(mon);
		}
	}

	#region Hospitals

	public void ClearHospitals()
	{
		hospitals.Clear();
		queueSize = 0;
	}

	public void AddHospital(CBKBuilding building)
	{
		hospitals.Add(building);
		queueSize += building.combinedProto.hospital.queueSize;
	}

	public void RemoveHospital(CBKBuilding building)
	{
		hospitals.RemoveAll(x=>x==building);
		queueSize -= building.combinedProto.hospital.queueSize;
	}

	#endregion

	public void InitHealers()
	{
		initalized = true;
		foreach (var item in healingMonsters) 
		{
			DetermineHealTime(item);
		}
		MSHospitalManager.instance.healingMonsters.OrderBy(x=>x.healingMonster.priority);
	}

	/// <summary>
	/// Prepares a new heal request.
	/// </summary>
	void PrepareNewHealRequest()
	{
		healRequestProto = new HealMonsterRequestProto();
		healRequestProto.sender = MSWhiteboard.localMupWithResources;
		healRequestProto.isSpeedup = false;
	}
	
	void SendHealRequest ()
	{
		if (healRequestProto == null)
		{
			return;
		}
		
		if (healRequestProto.umhNew.Count == 0
		    && healRequestProto.umhUpdate.Count == 0
		    && healRequestProto.umhDelete.Count == 0
		    && healRequestProto.umchp.Count == 0)
		{
			return;
		}
		
		healRequestProto.sender = MSWhiteboard.localMupWithResources;
		
		UMQNetworkManager.instance.SendRequest(healRequestProto, (int)EventProtocolRequest.C_HEAL_MONSTER_EVENT, DealWithHealStartResponse);
		
		if (MSActionManager.Goon.OnHealQueueChanged != null)
		{
			MSActionManager.Goon.OnHealQueueChanged();
		}
		
		healRequestProto = null;
		
	}
	
	bool SomeMonsterFinishedHealing()
	{
		foreach (var item in MSHospitalManager.instance.healingMonsters) 
		{
			if (item.finishHealTimeMillis <= MSUtil.timeNowMillis)
			{
				return true;
			}
		}
		return false;
	}
	
	void CheckHealingMonsters()
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		if (SomeMonsterFinishedHealing())
		{
			UserMonsterCurrentHealthProto health;
			for (int i = 0; i < MSHospitalManager.instance.healingMonsters.Count;) 
			{
				if (MSHospitalManager.instance.healingMonsters[i].finishHealTimeMillis <= MSUtil.timeNowMillis)
				{
					health = new UserMonsterCurrentHealthProto();
					health.userMonsterId = MSHospitalManager.instance.healingMonsters[i].healingMonster.userMonsterId;
					health.currentHealth = MSHospitalManager.instance.healingMonsters[i].maxHP;
					healRequestProto.umchp.Add(health);
					CompleteHeal(MSHospitalManager.instance.healingMonsters[i]);
				}
				else
				{
					i++;
				}
			}
			SendHealRequest();
		}
	}
	
	public void SpeedUpHeal(int cost)
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		UserMonsterCurrentHealthProto health;
		PZMonster item;
		while(MSHospitalManager.instance.healingMonsters.Count > 0)
		{
			item = MSHospitalManager.instance.healingMonsters[0];
			health = new UserMonsterCurrentHealthProto();
			health.userMonsterId = item.userMonster.userMonsterId;
			health.currentHealth = item.maxHP;
			healRequestProto.umchp.Add(health);
			Debug.Log("Healing: " + health.userMonsterId);
			CompleteHeal(item);
		}
		SendHealRequest();
	}
	
	void CompleteHeal(PZMonster monster)
	{
		MSHospitalManager.instance.healingMonsters.Remove(monster);
		monster.healingMonster = null;
		monster.currHP = monster.maxHP;
	}
	
	
	/// <summary>
	/// Adds a monster to the healing queue.
	/// Function shorts if there are no available hospitals.
	/// </summary>
	/// <param name="monster">Monster.</param>
	public void AddToHealQueue(PZMonster monster)
	{
		if (hospitals.Count == 0)
		{
			return;
		}
		if (healingMonsters.Count >= queueSize)
		{
			CBKGoonScreen.instance.DisplayErrorMessage("Healing Queue Full");
			return;
		}
		
		if (MSResourceManager.instance.Spend(ResourceType.CASH, monster.healCost, delegate{AddToHealQueue(monster);}))
		{
			
			if (healRequestProto == null)
			{
				PrepareNewHealRequest();
			}
			
			monster.healingMonster = new UserMonsterHealingProto();
			monster.healingMonster.userId = MSWhiteboard.localMup.userId;
			monster.healingMonster.userMonsterId = monster.userMonster.userMonsterId;
			monster.healingMonster.healthProgress = 0;
			monster.healingMonster.queuedTimeMillis = MSUtil.timeNowMillis;
			
			MSHospitalManager.instance.healingMonsters.Add (monster);
			
			monster.healingMonster.priority = MSHospitalManager.instance.healingMonsters.Count;
			
			DetermineHealTime(monster);
			
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
			
			if (MSActionManager.Goon.OnHealQueueChanged != null)
			{
				MSActionManager.Goon.OnHealQueueChanged();
			}
			
		}
	}
	
	/// <summary>
	/// Determines what hospitals this monster will use and how long it will take to heal.
	/// After being run, monster.finishHealTimeMillis will be updated to reflect when it will finish,
	/// and all utilized hospitals will have their completeTime update to refect their use
	/// </summary>
	/// <param name="monster">Monster.</param>
	void DetermineHealTime(PZMonster monster)
	{
		monster.hospitalTimes.Clear();
		
		float progress = monster.healingMonster.healthProgress;
		
		#region Debug
		
		string str = "Listing Hospitals";
		foreach (var hos in hospitals) 
		{
			str += "\n" + hos.completeTime + ", " + hos.id;
		}
		Debug.Log(str);
		
		#endregion
		
		CBKBuilding lastHospital = GetSoonestHospital();
		long lastStartTime = Math.Max(monster.healingMonster.queuedTimeMillis, lastHospital.completeTime);
		monster.healingMonster.queuedTimeMillis = lastStartTime;
		lastHospital.completeTime = CalculateFinishTime(monster, lastHospital, progress, lastStartTime);
		monster.finishHealTimeMillis = lastHospital.completeTime;
		
		monster.hospitalTimes.Add(new HospitalTime(lastHospital, lastStartTime));
		
		for (CBKBuilding nextHospital = GetSoonestFasterHospital(lastHospital);
		     nextHospital != null;
		     nextHospital = GetSoonestFasterHospital(lastHospital))
		{
			lastHospital.completeTime = nextHospital.completeTime;
			progress += (lastHospital.completeTime - lastStartTime) / 1000 * lastHospital.combinedProto.hospital.healthPerSecond;
			lastStartTime = nextHospital.completeTime;
			nextHospital.completeTime = CalculateFinishTime(monster, nextHospital, progress, lastStartTime);
			monster.finishHealTimeMillis = nextHospital.completeTime;
			lastHospital = nextHospital;
			monster.hospitalTimes.Add(new HospitalTime(lastHospital, lastStartTime));
		}
		
		#region Debug2
		str = "Scheduled heal for " + monster.monster.displayName;
		str += "\nNow: " + MSUtil.timeNowMillis;
		str += "\nHealth to heal: " + (monster.maxHP - monster.currHP) + ", Progress: " + monster.healingMonster.healthProgress;
		str += "\n"  + monster.healStartTime + " Start";
		foreach (var hospitalTime in monster.hospitalTimes) {
			str += "\n" + hospitalTime.startTime + " Hospital " + hospitalTime.hospital.id;
		}
		str += "\n" + monster.finishHealTimeMillis + " Finish";
		Debug.Log(str);
		#endregion
		
	}
	
	long CalculateFinishTime(PZMonster monster, CBKBuilding hospital, float progress, long startTime)
	{
		float healthLeftToHeal = monster.maxHP - progress - monster.currHP;
		int millis = Mathf.CeilToInt(healthLeftToHeal / hospital.combinedProto.hospital.healthPerSecond * 1000);
		return startTime + millis;
	}
	
	void UpdateProgress(PZMonster monster)
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		float progress = (monster.maxHP - monster.currHP) * monster.healingMonster.healthProgress;
		for (int i = 0; i < monster.hospitalTimes.Count && monster.hospitalTimes[i].startTime < MSUtil.timeNowMillis; i++) 
		{
			if (i < monster.hospitalTimes.Count - 1 && MSUtil.timeNowMillis > monster.hospitalTimes[i+1].startTime)
			{
				progress += (monster.hospitalTimes[i+1].startTime - monster.hospitalTimes[i].startTime) / 1000 * monster.hospitalTimes[i].hospital.combinedProto.hospital.healthPerSecond;
			}
			else
			{
				progress += (MSUtil.timeNowMillis - monster.hospitalTimes[i].startTime) / 1000 * monster.hospitalTimes[i].hospital.combinedProto.hospital.healthPerSecond;
			}
		}
		if (progress > 0)
		{
			monster.healingMonster.healthProgress = progress;
			if (!healRequestProto.umhUpdate.Contains(monster.healingMonster))
			{
				healRequestProto.umhUpdate.Add(monster.healingMonster);
			}
			Debug.Log("Updated progress for " + monster.monster.name + ", Progress: " + monster.healingMonster.healthProgress);
		}
		monster.healingMonster.queuedTimeMillis = MSUtil.timeNowMillis;
		
	}
	
	void UpdateAllProgress()
	{
		foreach (var monster in MSHospitalManager.instance.healingMonsters) 
		{
			UpdateProgress(monster);
		}
	}
	
	/// <summary>
	/// Gets the soonest available hospital
	/// </summary>
	/// <returns>The soonest hospital.</returns>
	CBKBuilding GetSoonestHospital()
	{
		CBKBuilding soonest = null;
		foreach (var building in hospitals) 
		{
			//If this building is sooner, or just as soon and faster, than the current soonest
			if (soonest == null || building.completeTime < soonest.completeTime || (building.completeTime == soonest.completeTime
			                                                                        && building.combinedProto.hospital.healthPerSecond > soonest.combinedProto.hospital.healthPerSecond))
			{
				soonest = building;
			}
		}
		return soonest;
	}
	
	/// <summary>
	/// Gets the soonest hospital that is faster than the hospital that is currently being used for healing.
	/// Used to see if we can speed up a monster's healing time by switching it to another hospital mid-heal.
	/// Preconditions: lastHospital has its completeTime set to the time that it would take to finish healing the current
	/// monster 
	/// </summary>
	/// <returns>The soonest faster hospital.</returns>
	/// <param name="lastHospital">Last hospital.</param>
	CBKBuilding GetSoonestFasterHospital(CBKBuilding lastHospital)
	{
		string str = "Trying to find sooner hospital that will finish before " + lastHospital.completeTime + " with a faster speed than " + lastHospital.combinedProto.hospital.healthPerSecond;
		CBKBuilding soonest = null;
		foreach (var building in hospitals) 
		{
			if (building == lastHospital) continue;
			str += "\nChecking " + building.id + ": " + building.completeTime + ", " + building.combinedProto.hospital.healthPerSecond;
			if (building.combinedProto.hospital.healthPerSecond > lastHospital.combinedProto.hospital.healthPerSecond
			    && building.completeTime < lastHospital.completeTime)
			{
				str += " Faster!";
				//If this building is sooner, or just as soon and faster, than the current soonest
				if (soonest == null || building.completeTime < soonest.completeTime || (building.completeTime == soonest.completeTime
				                                                                        && building.combinedProto.hospital.healthPerSecond > soonest.combinedProto.hospital.healthPerSecond))
				{
					str += " Soonest!";
					soonest = building;
				}
			}
			else
			{
				str += " Slower!";
			}
		}
		Debug.LogWarning(str);
		return soonest;
	}
	
	/// <summary>
	/// Called whenever we add new monsters to the healing queue or the number of monsters
	/// in the healing queue changes. This will take into account multiple hospitals. 
	/// When deciding which hospital to assign to which monster, it will choose the 
	/// hospital which will finish the monster's healing the earliest.
	/// </summary>
	void RearrangeHealingQueue()
	{
		UpdateAllProgress();
		foreach (CBKBuilding hospital in hospitals) 
		{
			hospital.completeTime = 0;
		}
		int priority = 1;
		foreach (PZMonster monster in MSHospitalManager.instance.healingMonsters) 
		{
			monster.healingMonster.priority = priority++;
			DetermineHealTime(monster);
		}
	}
	
	public void RemoveFromHealQueue(PZMonster monster)
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		/*
		int i;
		for (i = 0; i < healingMonsters.Count; i++) 
		{
			if (healingMonsters[i] == monster)
			{
				break;
			}
		}
		healingMonsters.RemoveAt(i);
		*/
		
		MSHospitalManager.instance.healingMonsters.Remove(monster);
		
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
		
		RearrangeHealingQueue();
		
		MSResourceManager.instance.Collect(ResourceType.CASH, monster.healCost);
		
		if (MSActionManager.Goon.OnHealQueueChanged != null)
		{
			MSActionManager.Goon.OnHealQueueChanged();
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
	}

	void Update()
	{
		CheckHealingMonsters();
	}

	void OnApplicationQuit()
	{
		SendHealRequest();
	}
}
