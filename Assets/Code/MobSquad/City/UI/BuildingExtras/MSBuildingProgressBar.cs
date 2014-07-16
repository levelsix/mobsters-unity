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

	[SerializeField] MSFillBar bar;

	[SerializeField] MSBuilding building;

	float newTime = 0;

	bool isConstructing
	{
		get
		{
			return (building.upgrade != null && !building.upgrade.isComplete);
		}
	}

	bool isHealing
	{
		get
		{
			return false;
		}
	}

	public bool upgradeBarFull
	{
		get
		{
			return bar.fill >= 1f;
		}
	}
	
	public bool upgrading = false;

	void Update () 
	{
		if (isConstructing && building.upgrade.timeRemaining >= 0)
		{
			upgrading = true;
			building.hoverIcon.gameObject.SetActive(false);
			foreach (var item in caps) 
			{
				item.spriteName = "buildingcap";
			}
			barSprite.spriteName = "buildingmiddle";
			
			bg.alpha = 1;
			label.text = building.upgrade.timeLeftString;
			bar.fill = building.upgrade.progress;
		}
		else if (building.obstacle != null && building.obstacle.isRemoving)
		{
			foreach (var item in caps) 
			{
				item.spriteName = "buildingcap";
			}
			barSprite.spriteName = "buildingmiddle";
			
			bg.alpha = 1;
			label.text = MSUtil.TimeStringShort(building.obstacle.millisLeft);
			bar.fill = building.obstacle.progress;
		}
		else if (building.hospital != null && building.hospital.goon != null && building.hospital.goon.monster != null && building.hospital.goon.monster.monsterId > 0)
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
		else if(bar.fill < 1f && bg.alpha > 0f)
		{
			if(newTime == 0){
				newTime = building.upgrade.timeRemaining;
			}

			building.hoverIcon.gameObject.SetActive(false);
			
			bar.fill = bar.fill + 1f * Time.deltaTime;
			
			label.text = MSUtil.TimeStringShort((long)(newTime * (1f - bar.fill)));
			if(bar.fill >= 1f)
			{
				bar.fill = 1f;
			}
		}
		else if(bar.fill >= 1f && upgrading)
		{
			newTime = 0;

			upgrading = false;

			bg.alpha = 0;

			if (building.upgrade.OnFinishUpgrade != null)
			{
				building.upgrade.OnFinishUpgrade();
			}
		}
		else
		{
			bg.alpha = 0;
		}
	}
}
