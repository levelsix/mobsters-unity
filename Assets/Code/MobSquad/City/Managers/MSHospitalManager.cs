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

	public long timeLeft
	{
		get
		{
			if (healingMonsters.Count == 0)
			{
				return 0;
			}
			return lastFinishTime - MSUtil.timeNowMillis;
		}
	}

	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		MSActionManager.Clan.OnEndClanHelp += DealWithEndHelp;
		MSActionManager.Clan.OnGiveClanHelp += DealWithGiveHelp;
	}

	void OnDisable()
	{
		MSActionManager.Clan.OnEndClanHelp -= DealWithEndHelp;
		MSActionManager.Clan.OnGiveClanHelp -= DealWithGiveHelp;
	}

	void DealWithGiveHelp(GiveClanHelpResponseProto response, bool self)
	{
		if(!self)
		{
			foreach(ClanHelpProto help in response.clanHelps)
			{
				if(help.helpType == GameActionType.HEAL)
				{
					foreach(PZMonster monster in healingMonsters)
					{
						if(monster.currActiveHelp.clanHelpUuid.Equals(help.clanHelpUuid))
						{
							monster.currActiveHelp = help;
							break;
						}
					}
				}
			}
		}
		RearrangeHealingQueue();
	}

	void DealWithEndHelp(EndClanHelpResponseProto response, bool self)
	{
		if(self)
		{
			foreach(PZMonster monster in healingMonsters)
			{
				if(response.clanHelpUuids.Contains(monster.currActiveHelp.clanHelpUuid))
				{
					monster.currActiveHelp = null;
				}
			}
		}
	}

	public void Init(List<UserMonsterHealingProto> healing)
	{
		PZMonster mon;
		foreach (var item in healing) 
		{
			mon = MSMonsterManager.instance.userMonsters.Find(x => x.userMonster.userMonsterUuid.Equals(item.userMonsterUuid));
			mon.healingMonster = item;
			MSHospitalManager.instance.healingMonsters.Add(mon);
		}
	}

	#region Hospitals

	public void AssignHospital(MSBuilding building)
	{
		MSHospital hospital = hospitals.Find(x=>x.userBuildingData.userStructUuid.Equals(building.userStructProto.userStructUuid));

		if (hospital == null)
		{
			hospital = new MSHospital();
			hospitals.Add(hospital);
		}

		building.hospital = hospital;
		hospital.InitFromBuilding(building);

	}

	public void RemoveHospital(MSBuilding building)
	{
		hospitals.RemoveAll(x=>x.userBuildingData.userStructUuid.Equals(building.userStructProto.userStructUuid));
	}

	public MSHospital NextHospital(MSHospital hospital)
	{
		for (int i = 0; i < hospitals.Count; i++) 
		{
			if (hospitals[i] == hospital)
			{
				if (i+1 < hospitals.Count) return hospitals[i+1];
				return null;
			}
		}
		return null;
	}

	public MSHospital PreviousHospital(MSHospital hospital)
	{
		for (int i = 0; i < hospitals.Count; i++) 
		{
			if (hospitals[i] == hospital)
			{
				if (i > 0) return hospitals[i-1];
				return null;
			}
		}
		return null;
	}

	#endregion

	public void InitHealers()
	{
		initialized = true;
		string hosp = "Hospitals:\n";
		foreach (var item in hospitals) {
			hosp += item.userBuildingData.userStructUuid + "\n";
			}
		Debug.Log(hosp);

		foreach (var item in healingMonsters) 
		{
			Debug.Log("Finding hospital: " + item.healingMonster.userHospitalStructUuid);
			hospitals.Find(x=>x.userBuildingData.userStructUuid.Equals(item.healingMonster.userHospitalStructUuid)).AddExistingToon(item);
		}


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

	public Coroutine DoSendHealRequest()
	{
		return StartCoroutine(SendHealRequest());
	}

	public IEnumerator SendHealRequest ()
	{
		if (MSTutorialManager.instance.inTutorial)
		{
			healRequestProto = null;
			yield break;
		}

		if (healRequestProto == null)
		{
			yield break;
		}
		
		if (healRequestProto.umhNew.Count == 0
		    && healRequestProto.umhUpdate.Count == 0
		    && healRequestProto.umhDelete.Count == 0
		    && healRequestProto.umchp.Count == 0)
		{
			yield break;
		}

		yield return MSResourceManager.instance.RunCollectResources();
		
		healRequestProto.sender = MSWhiteboard.localMupWithResources;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(healRequestProto, (int)EventProtocolRequest.C_HEAL_MONSTER_EVENT);

		healRequestProto = null;

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		HealMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as HealMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != HealMonsterResponseProto.HealMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem sending heal request: " + response.status.ToString());
		}
	}
	
	bool SomeMonsterFinishedHealing()
	{
		foreach (var item in MSHospitalManager.instance.healingMonsters) 
		{
			if (item.healTimeLeftMillis <= 0)
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

		bool anyDone = false;
		foreach (var item in hospitals) 
		{
			anyDone = anyDone || CheckHospital(item);
		}

		if (anyDone)
		{
			DoSendHealRequest();
		}
	}

	bool CheckHospital(MSHospital hospital)
	{
		bool anyDone = false;
		while (hospital.healQueue.Count > 0 && IsHealDone(hospital.healQueue[0]))
		{
			anyDone = true;
			CompleteHeal(hospital.healQueue[0]);
		}
		return anyDone;
	}

	bool IsHealDone(PZMonster monster)
	{
		if (monster.isHealing && monster.healTimeLeftMillis <= 0)
		{
			Debug.Log("Shit's done, bro");
		}

		return monster.isHealing && monster.healTimeLeftMillis <= 0;
	}

	public void TrySpeedUpHeal(MSLoadLock loadLock, List<PZMonster> monsters, int gems)
	{
		StartCoroutine(SpeedUpHeal(loadLock, monsters, gems));
	}

	IEnumerator SpeedUpHeal(MSLoadLock loadLock, List<PZMonster> healingMonsters, int cost)
	{	
		loadLock.Lock();

		if (!MSTutorialManager.instance.inTutorial)
		{
			if (healRequestProto == null)
			{
				PrepareNewHealRequest();
			}

			healRequestProto.isSpeedup = true;
			healRequestProto.gemsForSpeedup = cost;
			
			UserMonsterCurrentHealthProto health;
			foreach (var item in healingMonsters) 
			{
				health = new UserMonsterCurrentHealthProto();
				health.userMonsterUuid = item.userMonster.userMonsterUuid;
				health.currentHealth = item.maxHP;
				healRequestProto.umchp.Add(health);
			}

			int tagNum = UMQNetworkManager.instance.SendRequest(healRequestProto, (int)EventProtocolRequest.C_HEAL_MONSTER_EVENT);
			
			healRequestProto = null;
			
			while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
			{
				yield return null;
			}
			
			HealMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as HealMonsterResponseProto;
			UMQNetworkManager.responseDict.Remove(tagNum);

			if (response.status == HealMonsterResponseProto.HealMonsterStatus.SUCCESS)
			{
				while(healingMonsters.Count > 0)
				{
					CompleteHeal(healingMonsters[0]);
				}
			}
			else
			{
				MSPopupManager.instance.CreatePopup("Error", "Sorry, a problem happened with the server!",
				                                    new string[] {"Okay"}, new string[] {"greenmenuoption"}, new Action[] {MSActionManager.Popup.CloseTopPopupLayer});
			}
		}
		else
		{
			while(MSHospitalManager.instance.healingMonsters.Count > 0)
			{
				CompleteHeal(MSHospitalManager.instance.healingMonsters[0]);
			}
			
			RearrangeHealingQueue();
		}

		loadLock.Unlock();
	}
	
	void CompleteHeal(PZMonster monster)
	{
		UserMonsterCurrentHealthProto health = new UserMonsterCurrentHealthProto();
		health.userMonsterUuid = monster.userMonster.userMonsterUuid;
		health.currentHealth = monster.maxHP;
		healRequestProto.umchp.Add (health);

		monster.currHospital.RemoveToonFromQueue(monster);
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

		if (MSActionManager.Goon.OnHealQueueChanged != null)
		{
			MSActionManager.Goon.OnHealQueueChanged();
		}
	}
	
	
	/// <summary>
	/// Adds a monster to the healing queue.
	/// Function shorts if there are no available hospitals.
	/// </summary>
	/// <param name="monster">Monster.</param>
	public bool AddToHealQueue(PZMonster monster, MSHospital hospital, bool useGems = false)
	{
		if (hospital.healQueue.Count >= hospital.queueSize)
		{
			MSActionManager.Popup.DisplayRedError("Healing Queue Full");
			return false;
		}

		if (useGems)
		{
			int gemCost = Mathf.CeilToInt((monster.healCost - MSResourceManager.resources[ResourceType.CASH]) * MSWhiteboard.constants.gemsPerResource);
			if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost))
			{
				AddToHealQueue(monster, hospital, MSResourceManager.instance.SpendAll(ResourceType.CASH), gemCost);
				return true;
			}
		}
		else if (MSResourceManager.instance.Spend(ResourceType.CASH, monster.healCost, delegate{AddToHealQueue(monster, hospital, true);}))
		{
			AddToHealQueue(monster, hospital, monster.healCost);
			return true;
		}
		return false;
	}

	void AddToHealQueue(PZMonster monster, MSHospital hospital, int cash, int gems = 0)
	{
		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}
		
		monster.healingMonster = new UserMonsterHealingProto();
		if (!MSTutorialManager.instance.inTutorial)
		{
			monster.healingMonster.userUuid = MSWhiteboard.localMup.userUuid;
			monster.healingMonster.userMonsterUuid = monster.userMonster.userMonsterUuid;
		}
		monster.healingMonster.healthProgress = 0;

		//GetSoonestHospital(hospitals).AddToonToQueue(monster);
		hospital.AddToonToQueue(monster);

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

		gemCost += MSMath.GemsForTime(lastTime - MSUtil.timeNowMillis, false);

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
		
//		MSHospital lastHospital = GetSoonestHospital(hospitals);
//
//		long lastStartTime = Math.Max(monster.queueTime, lastHospital.completeTime);
//		monster.queueTime = lastStartTime;
//		lastHospital.completeTime = CalculateFinishTime(monster, lastHospital, progress, lastStartTime);
//		monster.finishTime = lastHospital.completeTime;
		
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
//		float progress = monster.healingMonster.healthProgress;
//		
//		#region Debug
//
//		/*
//		string str = "Listing Hospitals";
//		foreach (var hos in hospitals) 
//		{
//			str += "\n" + hos.building.completeTime + ", " + hos.userBuildingData.userStructId;
//		}
//		Debug.Log(str);
//		*/
//
//		#endregion
//		
//		MSHospital lastHospital = GetSoonestHospital(hospitals);
//		monster.userHospitalID = lastHospital.userBuildingData.userStructUuid;
//
//		long lastStartTime = Math.Max(monster.healingMonster.queuedTimeMillis, lastHospital.completeTime);
//		monster.healingMonster.queuedTimeMillis = lastStartTime;
//		lastHospital.completeTime = monster.finishHealTimeMillis;
//
//		Debug.Log("Finish Time: " + monster.finishHealTimeMillis + ", Now: " + MSUtil.timeNowMillis);
//
//		#region Debug2
//
//		/*
//		str = "Scheduled heal for " + monster.monster.displayName;
//		str += "\nNow: " + MSUtil.timeNowMillis;
//		str += "\nHealth to heal: " + (monster.maxHP - monster.currHP) + ", Progress: " + monster.healingMonster.healthProgress;
//		str += "\n"  + monster.healStartTime + " Start";
//		foreach (var hospitalTime in monster.hospitalTimes) {
//			str += "\n" + hospitalTime.startTime + " Hospital " + hospitalTime.hospital.userBuildingData.userStructId;
//		}
//		str += "\n" + monster.finishHealTimeMillis + " Finish";
//		//Debug.Log(str);
//		*/
//
//		#endregion
		
	}

	long CalculateFinishTime(HospitalItem monster, MSHospital hospital, float progress, long startTime)
	{
		float healthLeftToHeal = monster.healthToHeal - progress;
		int millis = Mathf.CeilToInt(healthLeftToHeal / hospital.proto.healthPerSecond * 1000);
		millis -= (int)monster.helpTime;

		Debug.Log("Calculating finish time for " + monster.userMonsterId + "\nAt hospital " + hospital.userBuildingData.userStructUuid
		          + "\nProgress: " + progress + "\nStart time: " + startTime + "\nFinish should be: " + (startTime+millis));
		
		return startTime + millis;
	}

	long CalculateFinishTime(PZMonster monster, MSHospital hospital, float progress, long startTime)
	{
		float healthLeftToHeal = monster.maxHP - progress - monster.currHP;
		int millis = Mathf.CeilToInt(healthLeftToHeal / hospital.proto.healthPerSecond * 1000);
		millis -= (int)monster.helpTime;

		Debug.Log("Calculating finish time for " + monster.monster.displayName + "\nAt hospital " + hospital.userBuildingData.userStructUuid
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
			long time = (MSUtil.timeNowMillis - monster.hospitalTimes[i].startTime) / 1000;
			progress += time * monster.hospitalTimes[i].hospital.proto.healthPerSecond;
			//Debug.Log("Time: " + time + "\nSpeed: " + monster.hospitalTimes[i].hospital.proto.healthPerSecond + "\nProgress: " + progress);

		}

		if (progress != monster.healingMonster.healthProgress)
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
			if (soonest == null || hos.finishTime < soonest.finishTime || (hos.finishTime == soonest.finishTime
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
		string str = "Trying to find sooner hospital that will finish before " + lastHospital.finishTime + " with a faster speed than " + lastHospital.proto.healthPerSecond;
		MSHospital soonest = null;
		foreach (var hos in hospitals) 
		{
			//if (hos.completeTime < MSUtil.timeNowMillis) hos.completeTime = MSUtil.timeNowMillis;
			if (hos == lastHospital) continue;
			str += "\nChecking " + hos.userBuildingData.userStructUuid + ": " + hos.finishTime + ", " + hos.proto.healthPerSecond;
			if (hos.proto.healthPerSecond > lastHospital.proto.healthPerSecond
			    && hos.finishTime < lastHospital.finishTime)
			{
				str += " Faster!";
				//If this building is sooner, or just as soon and faster, than the current soonest
				if (soonest == null || hos.finishTime < soonest.finishTime || (hos.finishTime == soonest.finishTime
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
//		UpdateAllProgress();
		foreach (MSHospital hospital in hospitals) 
		{
			hospital.RecalculateQueue();
		}
//		int priority = 1;
//		foreach (PZMonster monster in MSHospitalManager.instance.healingMonsters) 
//		{
//			monster.healingMonster.priority = priority++;
//			DetermineHealTime(monster, hospitals);
//		}
	}
	
	public void RemoveFromHealQueue(PZMonster monster)
	{
		//Debug.Log("Removing monster: " + monster.monster.name);

		if (healRequestProto == null)
		{
			PrepareNewHealRequest();
		}

		monster.currHospital.RemoveToonFromQueue(monster);
		monster.currHospital.RecalculateQueue();
		
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
		
		healRequestProto.cashChange += monster.healCost;
		
		monster.healingMonster = null;
		
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

	void Update()
	{
		if (initialized)
		{
			CheckHealingMonsters();
		}
	}

	void OnApplicationQuit()
	{
		DoSendHealRequest();
	}
}

public class HospitalItem
{
	public string userMonsterId;
	public float healthProgress;
	public long queueTime;
	public int healthToHeal;
	public long finishTime;
	public long helpTime;
	public List<HospitalTime> hospitalTimes = new List<HospitalTime>();

	public HospitalItem(PZMonster monster)
	{
		if (monster.healingMonster != null && !monster.healingMonster.userMonsterUuid.Equals(""))
		{
			healthProgress = monster.healingMonster.healthProgress;
			queueTime = monster.healingMonster.queuedTimeMillis;
		}
		else
		{
			healthProgress = 0;
			queueTime = MSUtil.timeNowMillis;
		}

		userMonsterId = monster.userMonster.userMonsterUuid;
		healthToHeal = monster.maxHP - monster.currHP;
		helpTime = monster.helpTime;
	}
}
