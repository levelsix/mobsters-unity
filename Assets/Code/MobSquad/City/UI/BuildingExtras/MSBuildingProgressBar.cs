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

	[SerializeField] MSChatAvatar miniAvatar;

	/// <summary>
	/// If true, miniAvatar won't be updated with a new image
	/// </summary>
	bool avatarSet = false;

	long _newTime = 0;

	long newTime{
		get
		{
			return _newTime;
		}
		set
		{
			_newTime = value;
			newTimeChecker = (int)value;
		}
	}

	int newTimeChecker = 0;

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

	void OnEnable()
	{
		MSActionManager.Goon.OnHealQueueChanged += ResetMiniAvatar;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnHealQueueChanged -= ResetMiniAvatar;
	}

	void ResetMiniAvatar()
	{
		avatarSet = false;
		miniAvatar.gameObject.SetActive(false);
	}

	void Update () 
	{
		if (isConstructing && building.upgrade.timeRemaining >= 0)
		{
			miniAvatar.gameObject.SetActive(false);
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
			miniAvatar.gameObject.SetActive(false);
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
			if(!avatarSet)
			{
				avatarSet = true;
				miniAvatar.gameObject.SetActive(true);
				miniAvatar.Init(building.hospital.goon.monster);
			}

			foreach (var item in caps) 
			{
				item.spriteName = "healingcap";
			}
			barSprite.spriteName = "healingmiddle";
			CheckFreeBar();
			bg.gameObject.SetActive(true);
			label.text = MSUtil.TimeStringShort(MSUtil.timeUntil(building.hospital.completeTime));
			bar.fill = building.hospital.goon.healProgressPercentage;
		}
		//This if statement is for if a building suddenly is no longer under construction the bar fills quickly
		else if(bar.fill < 1f && bg.gameObject.activeSelf && upgrading)
		{

			if(fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
				freeLabel.gameObject.SetActive(false);
				label.alpha = 1f;
				fadeRoutine = null;
			}

			if(newTime == 0){
				if(building.obstacle != null)
				{
					newTime = building.obstacle.millisLeft;
				}
				else
				{
					newTime = building.upgrade.timeRemaining;
				}
			}

			building.hoverIcon.gameObject.SetActive(false);
			
			bar.fill += 1f * Time.deltaTime;

			if(bar.fill >= 1f)
			{
				bar.fill = 1f;
			}

			label.text = MSUtil.TimeStringShort((long)(newTime * (1f - bar.fill)));
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
			newTime = 0;
			bg.gameObject.SetActive(false);
		}
	}

	// TODO: change to return a bool on succesfully set bar to freebar
	void CheckFreeBar()
	{
		//hospital logic
		if(building.hospital != null && MSMath.GemsForTime(MSUtil.timeUntil(building.hospital.completeTime), true) == 0 && !upgrading)
		{
			SetBarFree(delegate { MSActionManager.Popup.DisplayPurpleError("Healing is now free!"); });
		}

		else if(MSMath.GemsForTime( building.upgrade.timeRemaining, true) == 0 && building.obstacle == null && building.upgrade.timeRemaining >= 0)
		{
			SetBarFree();
		}
		else
		{
			freeLabel.alpha = 0f;
			label.alpha = 1f;
			if(fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
				fadeRoutine = null;
			}
		}
	}

	void SetBarFree(Action FirstRun = null)
	{
		foreach (var item in caps) 
		{
			item.spriteName = "instantcap";
		}
		barSprite.spriteName = "instantmiddle";

		if(fadeRoutine == null)
		{
			fadeRoutine = TextFadeAnimation();
			if(FirstRun != null)
			{
				FirstRun();
			}
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
				if(showingFree)
				{
					freeLabel.alpha = 1f;
					label.alpha = 0f;
				}
				else
				{
					freeLabel.alpha = 0f;
					label.alpha = 1f;
				}
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
