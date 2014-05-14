﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobEntry
/// </summary>
[RequireComponent (typeof (MSSimplePoolable))]
public class MSMiniJobEntry : MonoBehaviour {

	[SerializeField]
	MSMiniJobReward rewardPrefab;

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UISprite jobRarityLabel;

	[SerializeField]
	MSUIHelper arrow;

	[SerializeField]
	UILabel timeLeftLabel;

	[SerializeField]
	MSUIHelper buttonHelper;

	[SerializeField]
	UISprite button;

	[SerializeField]
	UILabel buttonLabel;

	[SerializeField]
	UIGrid rewardGrid;

	[SerializeField]
	MSUIHelper rootHelper;

	enum EntryMode {IDLE, WAITING, COMPLETE};

	EntryMode currMode;

	List<MSMiniJobReward> rewards = new List<MSMiniJobReward>();

	public UserMiniJobProto job;

	MSMiniJobPopup popup;

	public void Init(UserMiniJobProto userMiniJob, MSMiniJobPopup popup)
	{
		this.popup = popup;
		job = userMiniJob;

		nameLabel.text = userMiniJob.miniJob.name;
		nameLabel.color = MSColors.qualityColors[userMiniJob.miniJob.quality];
		jobRarityLabel.spriteName = userMiniJob.miniJob.quality.ToString().ToLower() + "job";

		DetermineRewards (userMiniJob.miniJob);

		if (userMiniJob.timeStarted == 0)
		{
			currMode = EntryMode.IDLE;
			arrow.ResetAlpha(true);
			buttonHelper.TurnOff();
		}
	}

	void SetupButton()
	{
		arrow.ResetAlpha(false);
		buttonHelper.TurnOn();
		buttonHelper.ResetAlpha(true);

		if (currMode == EntryMode.COMPLETE)
		{
			timeLeftLabel.color = MSColors.cashTextColor;
			timeLeftLabel.text = "COMPLETE!";
			button.spriteName = "greenmenuoption";
			buttonLabel.text = "Collect!";
		}
		else if (currMode == EntryMode.WAITING)
		{
			timeLeftLabel.color = Color.black;
			button.spriteName = "purplemenuoption";
			StartCoroutine(UpdateFinishButtonLabel());
		}
	}

	IEnumerator UpdateFinishButtonLabel()
	{
		while (currMode == EntryMode.WAITING)
		{
			timeLeftLabel.text = MSUtil.TimeStringShort(MSMiniJobManager.instance.timeLeft);
			buttonLabel.text = "Finish\n(g) " + MSMiniJobManager.instance.gemsToFinish;

			if (MSMiniJobManager.instance.isCompleted)
			{
				currMode = EntryMode.COMPLETE;
				SetupButton();
			}

			yield return new WaitForSeconds(1);
		}
	}

	void DetermineRewards(MiniJobProto miniJob)
	{
		MSMiniJobReward reward;
		if (miniJob.cashReward > 0)
		{
			reward = AddReward();
			reward.InitCash(miniJob.cashReward);
		}
		if (miniJob.oilReward > 0)
		{
			reward = AddReward();
			reward.InitOil(miniJob.oilReward);
		}
		if (miniJob.gemReward > 0)
		{
			reward = AddReward();
			reward.InitGem(miniJob.gemReward);
		}
		if (miniJob.monsterIdReward > 0)
		{
			reward = AddReward();
			reward.InitMonster(miniJob.monsterIdReward);
		}
	}

	MSMiniJobReward AddReward()
	{
		MSMiniJobReward reward = (MSPoolManager.instance.Get(rewardPrefab.GetComponent<MSSimplePoolable>(),
		                                                    Vector3.zero,
		                                                    rewardGrid.transform) as MSSimplePoolable).GetComponent<MSMiniJobReward>();
		reward.transform.localScale = Vector3.one;
		reward.transform.localPosition = Vector3.zero;
		rewards.Add (reward);
		return reward;
	}

	void OnClick()
	{
		if (currMode == EntryMode.IDLE)
		{
			popup.OnJobClicked(job);
		}
	}

	public void OnButtonClick()
	{
		if (currMode == EntryMode.WAITING)
		{
			RushComplete();
		}
		else
		{
			Collect();
		}
	}

	void RushComplete()
	{
		MSMiniJobManager.instance.CompleteCurrentJobWithGems();
	}

	void Collect()
	{
		//TODO: Block leaving the popup or reclicking the button until collect complete
		MSMiniJobManager.instance.RedeemCurrJob();
	}

	void OnJobStarted(UserMiniJobProto job)
	{
		if (job == this.job)
		{
			currMode = EntryMode.WAITING;
			buttonHelper.TurnOn();
			buttonHelper.ResetAlpha(true);

		}
	}

	void OnJobFinished(UserMiniJobProto job)
	{
		if (job == this.job)
		{
			StartCoroutine(FadeOutAndPool());
		}
	}

	IEnumerator FadeOutAndPool()
	{
		UIGrid grid = transform.parent.GetComponent<UIGrid>();
		transform.parent = transform.parent.parent;
		if (grid != null)
		{
			grid.Reposition();
		}

		TweenAlpha fade = rootHelper.FadeOut();
		while(fade.tweenFactor < 1)
		{
			yield return null;
		}
		Pool();
	}

	public void Pool()
	{
		foreach (var item in rewards) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}
		rewards.Clear();
		GetComponent<MSSimplePoolable>().Pool();
	}

}
