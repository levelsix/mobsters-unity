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

	[SerializeField] UILabel freeLabel;

	public MSFillBar bar;

	[SerializeField] MSBuilding building;

	float newTime = 0;

	const float CYCLE_TIME = 3f;
	const float FADE_TIME = 0.3f;

	IEnumerator fadeRoutine;

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
			CheckFreeBar();
			
			bg.gameObject.SetActive(true);
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
			CheckFreeBar();
			
			bg.gameObject.SetActive(true);
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
			CheckFreeBar();
			bg.gameObject.SetActive(true);
			label.text = MSUtil.TimeStringShort(building.hospital.goon.healTimeLeftMillis);
			bar.fill = building.hospital.goon.healProgressPercentage;
		}
		//This if statement is for if a building suddenly is no longer under construction the bar fills quickly
		else if(bar.fill < 1f && bg.gameObject.activeSelf)
		{

			if(fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
				freeLabel.gameObject.SetActive(false);
				label.alpha = 1f;
				fadeRoutine = null;
			}

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
			
			bg.gameObject.SetActive(false);

			if(building.obstacle != null)
			{
				StartCoroutine(building.obstacle.EndingAnimation());
			}
			else if (building.upgrade.OnFinishUpgrade != null)
			{
				building.upgrade.OnFinishUpgrade();
			}
		}
		else if (bg.gameObject.activeSelf)
		{
			bg.gameObject.SetActive(false);
		}
	}

	// TODO: change to return a bool on succesfully set bar to freebar
	void CheckFreeBar()
	{
		//hospital logic
		if(building.hospital != null && MSMath.GemsForTime(MSHealScreen.instance.timeLeft, true) == 0)
		{
			SetBarFree();
		}

		else if(MSMath.GemsForTime( building.upgrade.timeRemaining, true) == 0 && building.obstacle == null)
		{
			SetBarFree();
		}
		else
		{
			freeLabel.gameObject.SetActive(false);
		}
	}

	void SetBarFree()
	{
		foreach (var item in caps) 
		{
			item.spriteName = "instantcap";
		}
		barSprite.spriteName = "instantmiddle";

		if(fadeRoutine == null)
		{
			fadeRoutine = TextFadeAnimation();
			StartCoroutine(fadeRoutine);
		}
	}

	IEnumerator TextFadeAnimation()
	{
		bool showingFree = false;
		float fadeTime = 0;
		float cycleTime = 0;
		freeLabel.gameObject.SetActive(true);
		while(bg.gameObject.activeSelf)
		{

			if(cycleTime < CYCLE_TIME)
			{
				cycleTime += Time.deltaTime;
			}
			else if(fadeTime < FADE_TIME)
			{
				fadeTime += Time.deltaTime;
				if(showingFree)
				{
					label.alpha = fadeTime / FADE_TIME;
					freeLabel.alpha = 1f - fadeTime / FADE_TIME;
				}
				else
				{
					freeLabel.alpha = fadeTime / FADE_TIME;
					label.alpha = 1f - fadeTime / FADE_TIME;
				}
			}
			else
			{
				showingFree = !showingFree;
				fadeTime = 0f;
				cycleTime = 0f;
			}

			yield return null;
		}
		freeLabel.gameObject.SetActive(false);
		label.alpha = 1f;
		fadeRoutine = null;
	}
}
