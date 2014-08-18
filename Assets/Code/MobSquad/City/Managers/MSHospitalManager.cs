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

	public List<MSHospital> hospitals = new List<MSHospital>();

	public int queueSize = 0;
	
	HealMonsterRequestProto healRequestProto = null;

	public bool initialized = false;

	public long lastFinishTime
	{
		get
		{
			long last = 0;
			foreach (var item in healingMonsters) 
			{
				last = Math.Max(last, item.finishHealTimeMillis);
			}
			return last;
		}
	}

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

	public void AssignHospital(MSBuilding building)
	{
		MSHospital hospital = hospitals.Find(x=>x.userBuildingData.userStructId == building.userStructProto.userStructId);

		if (hospital == null)
		{
			hospital = new MSHospital();
			hospitals.Add(hospital);
			queueSize += building.combinedProto.hospital.queueSize;
		}

		building.hospital = hospital;
		hospital.InitFromBuilding(building);

	}

	public void RemoveHospital(MSBuilding building)
	{
		hospitals.RemoveAll(x=>x.userBuildingData.userStructId==building.userStructProto.userStructId);
		queueSize -= building.combinedProto.hospital.queueSize;
	}

	#endregion

	public void InitHealers()
	{
		initialized = true;
		foreach (var item in healingMonsters) 
		{
			DetermineHealTime(item, hospitals);
		}
		MSHospitalManager.instance.healingMonsters.Sort((m1, m2)=>m1.healingMonster.priority.CompareTo(m2.healingMonster.priority));
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
	
	public void SendHealRequest ()
	{
		if (MSTutorialManager.instance.inTutorial)
		{
			healRequestProto = null;
			return;
		}

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
					MSSlideUp.instance.QueueMonsterFinishHealing(MSHospitalManager.instance.healingMonsters[i]);

					MSHospitalManager.instance.healingMonsters[i].hospitalTimes[MSHospitalManager.instance.healingMonsters[i].hospitalTimes.Count-1].hospital.goon = null;

					health = new UserMonsterCurrentHealthProto();
					health.userMonsterId = MSHospitalManager.instance.healingMonsters[i].healingMonster.userMonsterId;
					health.currentHealth = MSHospitalManager.instance.healingMonsters[i].maxHP;
					healRequestProto.umchp.Add(health);
					if (MSActionManager.Goon.OnMonsterRemoveQueue != null)
					{
						MSActionManager.Goon.OnMonsterRemoveQueue(MSHospitalManager.instance.healingMonsters[i]);
					}
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

	public void TrySpeedUpHeal()
	{
		int gemCost = MSMath.GemsForTime(lastFinishTime - MSUtil.timeNowMillis);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost, TrySpeedUpHeal))
		{
			SpeedUpHeal(gemCost);
		}
	}
	
	void SpeedUpHeal(int cost)
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}

		healRequestProto.isSpeedup = true;
		healRequestProto.gemsForSpeedup = cost;

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

		if (MSActionManager.Goon.OnMonsterRemoveQueue != null)
		{
			MSActionManager.Goon.OnMonsterRemoveQueue(monster);
		}

		if (MSActionManager.Goon.OnMonsterFinishHeal != null)
		{
			MSActionManager.Goon.OnMonsterFinishHeal(monster);
		}
	}
	
	
	/// <summary>
	/// Adds a monster to the healing queue.
	/// Function shorts if there are no available hospitals.
	/// </summary>
	/// <param name="monster">Monster.</param>
	public bool AddToHealQueue(PZMonster monster, bool useGems = false)
	{
		if (hospitals.Count == 0 || healingMonsters.Count >= queueSize)
		{
			MSActionManager.Popup.DisplayError("Healing Queue Full");
			return false;
		}

		if (useGems)
		{
			int gemCost = Mathf.CeilToInt((monster.healCost - MSResourceManager.resources[ResourceType.CASH]) * MSWhiteboard.constants.gemsPerResource);
			if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost))
			{
				AddToHealQueue(monster, MSResourceManager.instance.SpendAll(ResourceType.CASH), gemCost);
				return true;
			}
		}
		else if (MSResourceManager.instance.Spend(ResourceType.CASH, monster.healCost, delegate{AddToHealQueue(monster, true);}))
		{
			AddToHealQueue(monster, monster.healCost);
			return true;
		}
		return false;
	}

	void AddToHealQueue(PZMonster monster, int cash, int gems = 0)
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		monster.healingMonster = new UserMonsterHealingProto();
		if (!MSTutorialManager.instance.inTutorial)
		{
			monster.healingMonster.userId = MSWhiteboard.localMup.userId;
			monster.healingMonster.userMonsterId = monster.userMonster.userMonsterId;
		}
		monster.healingMonster.healthProgress = 0;
		monster.healingMonster.queuedTimeMillis = MSUtil.timeNowMillis;
		
		MSHospitalManager.instance.healingMonsters.Add (monster);
		
		monster.healingMonster.priority = MSHospitalManager.instance.healingMonsters.Count;
		
		DetermineHealTime(monster, hospitals);
		
		if (healRequestProto.umhDelete.Contains(monster.healingMonster))
		{
			healRequestProto.umhDelete.Remove(monster.healingMonster);
			healRequestProto.umhUpdate.Add (monster.healingMonster);
		}
		else
		{
			healRequestProto.umhNew.Add(monster.healingMonster);
		}
		
		healRequestProto.cashChange -= cash;
		healRequestProto.gemCostForHealing += gems;
		
		if (MSActionManager.Goon.OnMonsterAddQueue != null)
		{
			MSActionManager.Goon.OnMonsterAddQueue(monster);
		}
		
		if (MSActionManager.Goon.OnHealQueueChanged != null)
		{
			MSActionManager.Goon.OnHealQueueChanged();
		}
	}

	public int SimulateHealForRevive(List<PZMonster> monsters, long startTime)
	{
		//Make hospital items for all the monsters, and calculate how much it
		//would cost to heal them now
		int cashCost = 0;
		List<HospitalItem> items = new List<HospitalItem>();
		foreach (var monster in monsters) 
		{
			items.Add(new HospitalItem(monster));
			cashCost += monster.healCost;
		}

		int gemCost = Mathf.CeilToInt(MSWhiteboard.constants.gemsPerResource * cashCost);

		//We need copies so that we don't fuck with the originals in the simulation
		List<MSHospital> hops = new List<MSHospital>();
		foreach (var hospital in hospitals) 
		{
			hops.Add (new MSHospital(hospital)); 
		}

		foreach (var item in items) 
		{
			DetermineHealTime(item, hops);
		}

		long lastTime = 0;
		foreach (var item in items) 
		{
			if (item.finishTime > lastTime)
			{
				lastTime = item.finishTime;
			}
		}

		gemCost += Mathf.CeilToInt(((float)((lastTime - MSUtil.timeNowMillis) / 60000))
		                           / MSWhiteboard.constants.minutesPerGem);

		return gemCost;
	}

	void DetermineHealTime(HospitalItem monster, List<MSHospital> hospitals)
	{
		monster.hospitalTimes.Clear();
		
		float progress = monster.healthProgress;
		
		#region Debug

		/*
		string str = "Listing Hospitals";
		foreach (var hos in hospitals) 
		{
			str += "\n" + hos.building.completeTime + ", " + hos.userBuildingData.userStructId;
		}
		Debug.Log(str);
		*/

		#endregion
		
		MSHospital lastHospital = GetSoonestHospital(hospitals);

		long lastStartTime = Math.Max(monster.queueTime, lastHospital.completeTime);
		monster.queueTime = lastStartTime;
		lastHospital.completeTime = CalculateFinishTime(monster, lastHospital, progress, lastStartTime);
		monster.finishTime = lastHospital.completeTime;
		
		monster.hospitalTimes.Add(new HospitalTime(lastHospital, lastStartTime));
		
		for (MSHospital nextHospital = GetSoonestFasterHospital(lastHospital, hospitals);
		     nextHospital != null;
		     nextHospital = GetSoonestFasterHospital(lastHospital, hospitals))
		{
			lastHospital.completeTime = nextHospital.completeTime;
			progress += (lastHospital.completeTime - lastStartTime) / 1000 * lastHospital.proto.healthPerSecond;
			lastStartTime = nextHospital.completeTime;
			nextHospital.completeTime = CalculateFinishTime(monster, nextHospital, progress, lastStartTime);
			monster.finishTime = nextHospital.completeTime;
			lastHospital = nextHospital;
			monster.hospitalTimes.Add(new HospitalTime(lastHospital, lastStartTime));
		}
		
		#region Debug2

		/*
		str = "Scheduled heal for " + monster.monster.displayName;
		str += "\nNow: " + MSUtil.timeNowMillis;
		str += "\nHealth to heal: " + (monster.maxHP - monster.currHP) + ", Progress: " + monster.healingMonster.healthProgress;
		str += "\n"  + monster.healStartTime + " Start";
		foreach (var hospitalTime in monster.hospitalTimes) {
			str += "\n" + hospitalTime.startTime + " Hospital " + hospitalTime.hospital.userBuildingData.userStructId;
		}
		str += "\n" + monster.finishHealTimeMillis + " Finish";
		//Debug.Log(str);
		*/

		#endregion
	}
	
	/// <summary>
	/// Determines what hospitals this monster will use and how long it will take to heal.
	/// After being run, monster.finishHealTimeMillis will be updated to reflect when it will finish,
	/// and all utilized hospitals will have their completeTime update to refect their use
	/// </summary>
	/// <param name="monster">Monster.</param>
	void DetermineHealTime(PZMonster monster, List<MSHospital> hospitals)
	{
		monster.hospitalTimes.Clear();
		
		float progress = monster.healingMonster.healthProgress;
		
		#region Debug

		/*
		string str = "Listing Hospitals";
		foreach (var hos in hospitals) 
		{
			str += "\n" + hos.building.completeTime + ", " + hos.userBuildingData.userStructId;
		}
		Debug.Log(str);
		*/

		#endregion
		
		MSHospital lastHospital = GetSoonestHospital(hospitals);

		/*
		if (lastHospital.completeTime <= MSUtil.timeNowMillis)
		{
			Debug.Log("Hospital: " + lastHospital.hospital);
			Debug.Log("Monster: " + monster);
			lastHospital.hospital.goon = monster;
		}
		*/

		long lastStartTime = Math.Max(monster.healingMonster.queuedTimeMillis, lastHospital.completeTime);
		monster.healingMonster.queuedTimeMillis = lastStartTime;
		lastHospital.completeTime = CalculateFinishTime(monster, lastHospital, progress, lastStartTime);
		monster.finishHealTimeMillis = lastHospital.completeTime;
		
		monster.hospitalTimes.Add(new HospitalTime(lastHospital, lastStartTime));
		
		for (MSHospital nextHospital = GetSoonestFasterHospital(lastHospital, hospitals);
		     nextHospital != null;
		     nextHospital = GetSoonestFasterHospital(lastHospital, hospitals))
		{
			lastHospital.completeTime = nextHospital.completeTime;
			progress += (lastHospital.completeTime - lastStartTime) / 1000 * lastHospital.proto.healthPerSecond;
			lastStartTime = nextHospital.completeTime;
			nextHospital.completeTime = CalculateFinishTime(monster, nextHospital, progress, lastStartTime);
			monster.finishHealTimeMillis = nextHospital.completeTime;
			lastHospital = nextHospital;
			monster.hospitalTimes.Add(new HospitalTime(lastHospital, lastStartTime));
		}

		Debug.Log("Finish Time: " + monster.finishHealTimeMillis + ", Now: " + MSUtil.timeNowMillis);

		#region Debug2

		/*
		str = "Scheduled heal for " + monster.monster.displayName;
		str += "\nNow: " + MSUtil.timeNowMillis;
		str += "\nHealth to heal: " + (monster.maxHP - monster.currHP) + ", Progress: " + monster.healingMonster.healthProgress;
		str += "\n"  + monster.healStartTime + " Start";
		foreach (var hospitalTime in monster.hospitalTimes) {
			str += "\n" + hospitalTime.startTime + " Hospital " + hospitalTime.hospital.userBuildingData.userStructId;
		}
		str += "\n" + monster.finishHealTimeMillis + " Finish";
		//Debug.Log(str);
		*/

		#endregion
		
	}

	long CalculateFinishTime(HospitalItem monster, MSHospital hospital, float progress, long startTime)
	{
		float healthLeftToHeal = monster.healthToHeal - progress;
		int millis = Mathf.CeilToInt(healthLeftToHeal / hospital.proto.healthPerSecond * 1000);
		
		Debug.Log("Calculating finish time for " + monster.userMonsterId + "\nAt hospital " + hospital.userBuildingData.userStructId
		          + "\nProgress: " + progress + "\nStart time: " + startTime + "\nFinish should be: " + (startTime+millis));
		
		return startTime + millis;
	}

	long CalculateFinishTime(PZMonster monster, MSHospital hospital, float progress, long startTime)
	{
		float healthLeftToHeal = monster.maxHP - progress - monster.currHP;
		int millis = Mathf.CeilToInt(healthLeftToHeal / hospital.proto.healthPerSecond * 1000);

		Debug.Log("Calculating finish time for " + monster.monster.displayName + "\nAt hospital " + hospital.userBuildingData.userStructId
			+ "\nProgress: " + progress + "\nStart time: " + startTime + "\nFinish should be: " + (startTime+millis));

		return startTime + millis;
	}
	
	void UpdateProgress(PZMonster monster)
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		float progress = monster.healingMonster.healthProgress;
		for (int i = 0; i < monster.hospitalTimes.Count && monster.hospitalTimes[i].startTime < MSUtil.timeNowMillis; i++) 
		{
			if (i < monster.hospitalTimes.Count - 1 && MSUtil.timeNowMillis > monster.hospitalTimes[i+1].startTime)
			{
				progress += (monster.hospitalTimes[i+1].startTime - monster.hospitalTimes[i].startTime) / 1000 * monster.hospitalTimes[i].hospital.proto.healthPerSecond;
				//Debug.Log("Progress: " + progress);
			}
			else
			{
				long time = (MSUtil.timeNowMillis - monster.hospitalTimes[i].startTime) / 1000;
				progress += (MSUtil.timeNowMillis - monster.hospitalTimes[i].startTime) / 1000 * monster.hospitalTimes[i].hospital.proto.healthPerSecond;
				//Debug.Log("Time: " + time + "\nSpeed: " + monster.hospitalTimes[i].hospital.proto.healthPerSecond + "\nProgress: " + progress);
			}
		}
		if (progress > 0)
		{
			monster.healingMonster.healthProgress = progress;
			if (!healRequestProto.umhUpdate.Contains(monster.healingMonster))
			{
				healRequestProto.umhUpdate.Add(monster.healingMonster);
			}
			//Debug.Log("Updated progress for " + monster.monster.name + ", Progress: " + monster.healingMonster.healthProgress);
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
	MSHospital GetSoonestHospital(List<MSHospital> hospitals)
	{
		MSHospital soonest = null;
		foreach (var hos in hospitals) 
		{
			//if (hos.completeTime < MSUtil.timeNowMillis) hos.completeTime = MSUtil.timeNowMillis;
			//If this building is sooner, or just as soon and faster, than the current soonest
			if (soonest == null || hos.completeTime < soonest.completeTime || (hos.completeTime == soonest.completeTime
			                                                                            && hos.proto.healthPerSecond > soonest.proto.healthPerSecond))
			{
				soonest = hos;
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
	MSHospital GetSoonestFasterHospital(MSHospital lastHospital, List<MSHospital> hospitals)
	{
		string str = "Trying to find sooner hospital that will finish before " + lastHospital.completeTime + " with a faster speed than " + lastHospital.proto.healthPerSecond;
		MSHospital soonest = null;
		foreach (var hos in hospitals) 
		{
			//if (hos.completeTime < MSUtil.timeNowMillis) hos.completeTime = MSUtil.timeNowMillis;
			if (hos == lastHospital) continue;
			str += "\nChecking " + hos.userBuildingData.userStructId + ": " + hos.completeTime + ", " + hos.proto.healthPerSecond;
			if (hos.proto.healthPerSecond > lastHospital.proto.healthPerSecond
			    && hos.completeTime < lastHospital.completeTime)
			{
				str += " Faster!";
				//If this building is sooner, or just as soon and faster, than the current soonest
				if (soonest == null || hos.completeTime < soonest.completeTime || (hos.completeTime == soonest.completeTime
				                                                                        && hos.proto.healthPerSecond > soonest.proto.healthPerSecond))
				{
					str += " Soonest!";
					soonest = hos;
				}
			}
			else
			{
				str += " Slower!";
			}
		}
		//Debug.LogWarning(str);
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
		foreach (MSHospital hospital in hospitals) 
		{
			hospital.completeTime = 0;
		}
		int priority = 1;
		foreach (PZMonster monster in MSHospitalManager.instance.healingMonsters) 
		{
			monster.healingMonster.priority = priority++;
			DetermineHealTime(monster, hospitals);
		}
	}
	
	public void RemoveFromHealQueue(PZMonster monster)
	{
		//Debug.Log("Removing monster: " + monster.monster.name);

		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
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

		if (MSActionManager.Goon.OnMonsterRemoveQueue != null)
		{
			MSActionManager.Goon.OnMonsterRemoveQueue(monster);
		}

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
		if (initialized)
		{
			CheckHealingMonsters();
		}
	}

	void OnApplicationQuit()
	{
		SendHealRequest();
	}
}

public class HospitalItem
{
	public long userMonsterId;
	public float healthProgress;
	public long queueTime;
	public int healthToHeal;
	public long finishTime;
	public List<HospitalTime> hospitalTimes = new List<HospitalTime>();

	public HospitalItem(PZMonster monster)
	{
		if (monster.healingMonster != null && monster.healingMonster.userMonsterId > 0)
		{
			healthProgress = monster.healingMonster.healthProgress;
			queueTime = monster.healingMonster.queuedTimeMillis;
		}
		else
		{
			healthProgress = 0;
			queueTime = MSUtil.timeNowMillis;
		}

		userMonsterId = monster.userMonster.userMonsterId;
		healthToHeal = monster.maxHP - monster.currHP;
	}
}
