using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSBuildingProgressBar
/// </summary>
public class MSBuildingProgressBar : MonoBehaviour {

	[SerializeField] UISprite bg;

	[SerializeField] UISprite[] caps;

	[SerializeField] UISprite barSprite;

	[SerializeField] UILabel label;

	[SerializeField] CBKFillBar bar;

	[SerializeField] MSBuilding building;

	bool isConstructing
	{
		get
		{
			return building.upgrade != null && building.upgrade.timeRemaining > 0;
		}
	}

	bool isHealing
	{
		get
		{
			return false;
		}
	}

	void Update () 
	{
		if (building.obstacle != null)
		{
			bg.alpha = 0;
		}
		else if (isConstructing)
		{
			foreach (var item in caps) 
			{
				item.spriteName = "buildingcap";
			}
			barSprite.spriteName = "buildingmiddle";

			bg.alpha = 1;
			label.text = building.upgrade.timeLeftString;
			bar.fill = building.upgrade.progress;
		}
		else if (building.hospital != null && building.hospital.goon != null)
		{
			foreach (var item in caps) 
			{
				item.spriteName = "healingcap";
			}
			barSprite.spriteName = "healingmiddle";
			bg.alpha = 1;
			label.text = MSUtil.TimeStringShort(building.hospital.goon.healTimeLeftMillis);
			bar.fill = building.hospital.goon.healProgressPercentage;
		}
		else
		{
			bg.alpha = 0;
		}
	}
}
