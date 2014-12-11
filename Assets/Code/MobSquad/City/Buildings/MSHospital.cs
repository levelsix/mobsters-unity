using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSHospital
/// </summary>
public class MSHospital {

	public MSBuilding building = null;

	/// <summary>
	/// All of the hospital information.
	/// TODO: Make sure this gets updated on level-up
	/// </summary>
	public HospitalProto proto;

	/// <summary>
	/// The user building data.
	/// We need to keep ahold of this so that we can keep track
	/// of which MSHospital belongs to which MSBuilding when we
	/// return to the player's city
	/// </summary>
	public FullUserStructureProto userBuildingData;

	public List<PZMonster> healQueue = new List<PZMonster>();
	
	Animator animator = null;

	public int queueSize
	{
		get
		{
			if (building == null) return 0;
			return building.combinedProto.hospital.queueSize;
		}
	}

	public long finishTime
	{
		get
		{
			if (healQueue.Count == 0)
			{
				return MSUtil.timeNowMillis;
			}
			return healQueue[healQueue.Count-1].finishHealTimeMillis;
		}
	}

	public long timeLeft
	{
		get
		{
			return finishTime - MSUtil.timeNowMillis;
		}
	}

	public int gemsToFinish
	{
		get
		{
			return MSMath.GemsForTime(timeLeft, true);
		}
	}

	public int completeTimeInt;

	public PZMonster goon
	{
		get
		{
			if (healQueue.Count == 0) return null;
			return healQueue[0];
		}
	}

	public MSHospital()
	{
		MSActionManager.Scene.OnCity += SetGoon;
	}

	public MSHospital(MSHospital hospital)
	{
		this.building = hospital.building;
		this.proto = hospital.proto;
		this.userBuildingData = hospital.userBuildingData;
	}

	public void InitFromBuilding(MSBuilding building)
	{
		this.building = building;
		proto = building.combinedProto.hospital;
		userBuildingData = building.userStructProto;
		animator = building.sprite.GetComponent<Animator>();
		SetGoon();
	}

	void SetGoon()
	{
		if (building == null || animator == null)
		{
			return;
		}
		if (goon == null)
		{
			animator.SetBool("Healing", false);
			building.overlayUnit.gameObject.SetActive(false);
		}
		else
		{
			building.overlayUnit.gameObject.SetActive(true);
			animator.SetBool("Healing", true);
			building.overlayUnit.Init(goon);
		}
	}

	public void AddExistingToon(PZMonster toon)
	{
		healQueue.Add (toon);
		SetGoon();
	}

	public void AddToonToQueue(PZMonster toon)
	{
		toon.healingMonster.userHospitalStructUuid = building.userStructProto.userStructUuid;
		toon.healingMonster.queuedTimeMillis = finishTime;
		toon.healingMonster.priority = healQueue.Count;
		healQueue.Add(toon);
		SetGoon();
	}

	public void RemoveToonFromQueue(PZMonster monster)
	{
		healQueue.Remove(monster);
		RecalculateQueue();
		SetGoon();
	}

	public void RecalculateQueue()
	{
		for (int i = 0; i < healQueue.Count; i++) 
		{
			switch(i)
			{
			case 0:
				healQueue[i].healingMonster.queuedTimeMillis = Math.Min(MSUtil.timeNowMillis, healQueue[i].healingMonster.queuedTimeMillis);
				break;
			default:
				healQueue[i].healingMonster.queuedTimeMillis = healQueue[i-1].finishHealTimeMillis;
				break;
			}
			healQueue[i].healingMonster.priority = i;
		}
	}
}
