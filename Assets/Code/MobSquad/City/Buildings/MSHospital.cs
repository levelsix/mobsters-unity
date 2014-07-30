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
	
	Animator animator = null;

	PZMonster _goon = null;

	public long completeTime = 0;

	public PZMonster goon
	{
		set
		{
			if (value != _goon)
			{
				SetGoon(value);
			}
		}
		get
		{
			return _goon;
		}
	}

	public MSHospital()
	{
		MSActionManager.Goon.OnHealQueueChanged += OnHealingQueueChange;
		MSActionManager.Scene.OnCity += OnHealingQueueChange;
	}

	public MSHospital(MSHospital hospital)
	{
		this.building = hospital.building;
		this.proto = hospital.proto;
		this.userBuildingData = hospital.userBuildingData;
		this.completeTime = 0;
		MSActionManager.Goon.OnHealQueueChanged += OnHealingQueueChange;
	}

	public void InitFromBuilding(MSBuilding building)
	{
		this.building = building;
		proto = building.combinedProto.hospital;
		userBuildingData = building.userStructProto;
		animator = building.sprite.GetComponent<Animator>();
		SetGoon(goon);
	}

	void SetGoon(PZMonster goon)
	{
		_goon = goon;
		if (building == null || animator == null)
		{
			return;
		}
		if (_goon == null)
		{
			animator.SetBool("Healing", false);
			building.overlayUnit.gameObject.SetActive(false);
		}
		else
		{
			building.overlayUnit.gameObject.SetActive(true);
			animator.SetBool("Healing", true);
			building.overlayUnit.Init(_goon);
		}
	}

	void OnHealingQueueChange()
	{
		goon = MSHospitalManager.instance.healingMonsters.Find(x=>x.hospitalTimes[0].hospital == this);
	}
}
