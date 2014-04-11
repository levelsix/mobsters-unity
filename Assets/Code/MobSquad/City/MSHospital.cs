﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSHospital
/// </summary>
public class MSHospital : MonoBehaviour {

	MSBuilding building;
	
	Animator animator;

	PZMonster _goon;

	public PZMonster goon
	{
		set
		{
			if (value != _goon)
			{
				_goon = value;
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
		}
		get
		{
			return _goon;
		}
	}

	void Awake()
	{
		building = GetComponent<MSBuilding>();
		animator = building.sprite.GetComponent<Animator>();
		goon = null;
		building.overlayUnit.gameObject.SetActive(false);
	}

	void OnDestroy()
	{
		if (building != null)
		{
			building.overlayUnit.gameObject.SetActive(false);
		}
	}
}
