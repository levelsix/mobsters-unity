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

	[SerializeField] GameObject bg;

	[SerializeField] UISprite[] caps;

	[SerializeField] UISprite barSprite;

	[SerializeField] UILabel label;

	public MSFillBar bar;

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
			
			bg.SetActive(true);
			label.text = building.upgrade.timeLeftString;
			bar.fill = building.upgrade.progress;
		}
		else if (building.obstacle != null && building.obstacle.isRemoving && !building.obstacle.finished)
		{
			foreach (var item in caps) 
			{
				item.spriteName = "buildingcap";
			}
			barSprite.spriteName = "buildingmiddle";
			
			bg.SetActive(true);
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
			bg.SetActive(true);
			label.text = MSUtil.TimeStringShort(building.hospital.goon.healTimeLeftMillis);
			bar.fill = building.hospital.goon.healProgressPercentage;
		}
		//This if statement is for if a building suddenly is no longer under construction the bar fills quickly
		else if(bar.fill < 1f && bg.activeSelf)
		{
			if(newTime == 0){
				if(building.obstacle == null)
				{
					newTime = building.upgrade.timeRemaining;
				}
				else
				{
					newTime = building.obstacle.millisLeft;
				}
			}

			building.hoverIcon.gameObject.SetActive(false);
			
			bar.fill += 1f * Time.deltaTime;

			label.text = MSUtil.TimeStringShort((long)(newTime * (1f - bar.fill)));

			if(bar.fill >= 1f)
			{
				bar.fill = 1f;
			}
		}
		else if(bar.fill >= 1f && (upgrading || (building.obstacle != null && building.obstacle.isRemoving)))
		{
			newTime = 0;

			upgrading = false;

			bg.SetActive(false);

			if(building.obstacle != null)
			{
				StartCoroutine(building.obstacle.EndingAnimation());
			}
			else if (building.upgrade.OnFinishUpgrade != null)
			{
				building.upgrade.OnFinishUpgrade();
			}
		}
		else if (bg.activeSelf);
		{
			bg.SetActive(false);
		}
	}
}
