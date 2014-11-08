using UnityEngine;
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
	UIButton button;

	[SerializeField]
	UILabel buttonLabel;

	[SerializeField]
	UIGrid rewardGrid;

	[SerializeField]
	MSUIHelper rootHelper;

	[SerializeField]
	UILabel totalTimeLabel;

	enum EntryMode {IDLE, WAITING, COMPLETE};

	EntryMode currMode;

	List<MSMiniJobReward> rewards = new List<MSMiniJobReward>();

	public UserMiniJobProto job;

	MSMiniJobPopup popup;

	public void Init(UserMiniJobProto userMiniJob, MSMiniJobPopup popup)
	{
		rootHelper.ResetAlpha(true);

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
			totalTimeLabel.text = MSUtil.TimeStringMed(job.durationSeconds * 1000);
		}
		else
		{
			if (userMiniJob.timeCompleted > 0)
			{
				currMode = EntryMode.COMPLETE;
			}
			else
			{
				currMode = EntryMode.WAITING;
			}

			SetupButton();
		}
	}

	void SetupButton()
	{
		arrow.ResetAlpha(false);
		buttonHelper.TurnOn();
		buttonHelper.ResetAlpha(true);

		buttonLabel.color = Color.white;
		buttonLabel.effectColor = new Color(0f,0f,0f,0.6f);

		totalTimeLabel.text = " ";

		if (currMode == EntryMode.COMPLETE)
		{
			timeLeftLabel.color = MSColors.cashTextColor;
			timeLeftLabel.text = "COMPLETE!";
			button.normalSprite = "greenmenuoption";
			buttonLabel.text = "Collect!";
			popup.curJobEntry = this;
		}
		else if (currMode == EntryMode.WAITING)
		{
			if(!MSClanManager.instance.HelpAlreadyRequested(ClanHelpType.MINI_JOB, (int)job.miniJob.quality, job.userMiniJobId))
			{
				SetupHelpButton();
			}
			else
			{
				SetupWaitingButton();
			}
			StartCoroutine(UpdateFinishTimer());
		}
	}

	void SetupHelpButton()
	{
		timeLeftLabel.color = Color.black;
		button.normalSprite = "orangemenuoption";
		buttonLabel.text = "Get Help";
		buttonLabel.effectColor = new Color(1f,1f,1f,0.6f);
		buttonLabel.color = new Color(195f/255f, 27f/255f, 0f, 1f);
		timeLeftLabel.text = MSUtil.TimeStringShort(MSMiniJobManager.instance.timeLeft);
	}

	void SetupWaitingButton()
	{
		timeLeftLabel.color = Color.black;
		button.normalSprite = "purplemenuoption";
		buttonLabel.effectColor = new Color(0f,0f,0f,0.6f);
		buttonLabel.color = Color.white;
		StartCoroutine(UpdateFinishButton());

	}

	IEnumerator UpdateFinishButton()
	{
		while (currMode == EntryMode.WAITING)
		{
			buttonLabel.text = "Finish\n(g) " + MSMiniJobManager.instance.gemsToFinish;
			
			if (MSMiniJobManager.instance.isCompleted)
			{
				currMode = EntryMode.COMPLETE;
				SetupButton();
			}
			yield return new WaitForSeconds(1);
		}
	}

	IEnumerator UpdateFinishTimer()
	{
		while (currMode == EntryMode.WAITING)
		{
			timeLeftLabel.text = MSUtil.TimeStringShort(MSMiniJobManager.instance.timeLeft);
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

		rewardGrid.Reposition();
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
		if(!MSClanManager.instance.HelpAlreadyRequested(ClanHelpType.MINI_JOB, (int)job.miniJob.quality, job.userMiniJobId))
		{
			button.GetComponent<MSLoadLock>().Lock();
			MSClanManager.instance.DoSolicitClanHelp(ClanHelpType.MINI_JOB,
			                                         (int)job.miniJob.quality,
			                                         job.userMiniJobId,
			                                         MSBuildingManager.clanHouse.combinedProto.clanHouse.maxHelpersPerSolicitation,
			                                         delegate {SetupWaitingButton(); button.GetComponent<MSLoadLock>().Unlock();});
		}
		else if (currMode == EntryMode.WAITING)
		{
			RushComplete();
		}
	}

	void RushComplete()
	{
		button.GetComponent<MSLoadLock>().Lock();
		MSMiniJobManager.instance.DoCompleteCurrentJobWithGems(button.GetComponent<MSLoadLock>().Unlock);
	}

	void OnJobStarted(UserMiniJobProto job)
	{
		if (job == this.job)
		{
			SetupWaitingButton();
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

		popup.jobEntries.Remove(this);
		popup.CheckNoJobs();

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
