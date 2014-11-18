﻿using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSPier : MSBuildingFrame {

	MSProgressBar bar;
	MSSimplePoolable doneIcon;
	
	void Awake()
	{
		base.Awake();
		bar = MSPoolManager.instance.Get<MSProgressBar>(MSPrefabList.instance.progressBar, transform);
		bar.transform.localPosition = new Vector3(0f,3.2f,0f);
		bar.transform.localEulerAngles = buildingAngle;
		bar.transform.localScale = buildingScale;
		bar.gameObject.SetActive(false);
	}
	
	void OnEnable()
	{
		InitPeir();

		MSActionManager.MiniJob.OnMiniJobBegin += InitProgressBar;
		MSActionManager.MiniJob.OnMiniJobGemsComplete += bar.FastComplete;
		MSActionManager.MiniJob.OnMiniJobComplete += CheckTag;
		//Assumption that restock consistently happens after this OnEnable call
		MSActionManager.MiniJob.OnMiniJobRestock += CheckTag;
		MSActionManager.MiniJob.OnMiniJobRedeem += CheckTag;

		FirstFrameCheck();
	}

	void OnDisable()
	{
		MSActionManager.MiniJob.OnMiniJobBegin -= InitProgressBar;
		MSActionManager.MiniJob.OnMiniJobGemsComplete -= bar.FastComplete;
		MSActionManager.MiniJob.OnMiniJobComplete -= CheckTag;
		MSActionManager.MiniJob.OnMiniJobRestock -= CheckTag;
		MSActionManager.MiniJob.OnMiniJobRedeem -= CheckTag;
	}

	void InitPeir()
	{
		if(MSMiniJobManager.instance.currActiveJob != null && MSMiniJobManager.instance.currActiveJob.userMiniJobId > 0)
		{
			InitProgressBar(MSMiniJobManager.instance.currActiveJob);
		}
	}
	
	void InitProgressBar(UserMiniJobProto job)
	{
		if(bubbleIcon != null && Precheck())
		{
			bubbleIcon.gameObject.SetActive(true);
			bubbleIcon.spriteName = job.miniJob.quality.ToString().ToLower() + "job";
			bubbleIcon.MakePixelPerfect();
		}

		if(job.timeCompleted == 0)
		{
			bar.init(job.timeStarted, MSMiniJobManager.instance.timeLeft, false);
		}
	}
	
	void SpawnJobDoneIcon(){
		if(doneIcon == null)
		{
			doneIcon = MSPoolManager.instance.Get<MSSimplePoolable>(MSPrefabList.instance.miniJobDone, building.trans);
			doneIcon.transf.localScale = buildingScale;
			doneIcon.transf.localEulerAngles = buildingAngle;
			doneIcon.transf.localPosition = new Vector3(0f,-0.5f,0f);
		}
		doneIcon.gameObject.SetActive(true);
	}
	
	[ContextMenu("checkTag")]
	public override void CheckTag(){

		bubbleIcon.gameObject.SetActive(false);
		if(Precheck())
		{
			if(MSMiniJobManager.instance.isCompleted)//There is a finished job
			{
				SpawnJobDoneIcon();
			}
			else if(MSMiniJobManager.instance.currActiveJob == null || MSMiniJobManager.instance.currActiveJob.miniJob == null) //there are no active jobs
			{
				if(doneIcon != null)
				{
					doneIcon.gameObject.SetActive(false);
				}

				bubbleIcon.gameObject.SetActive(false);
				if(building.combinedProto.structInfo.level == 0)
				{
					bubbleIcon.gameObject.SetActive(true);
					bubbleIcon.spriteName = "fixbubble";
					bubbleIcon.MakePixelPerfect();
				}
				else if(MSMiniJobManager.instance.userMiniJobs.Count > 0)
				{
					bubbleIcon.gameObject.SetActive(true);
					bubbleIcon.spriteName = "minijobsredbubble" + MSMiniJobManager.instance.userMiniJobs.Count;
					bubbleIcon.MakePixelPerfect();
				}
			}
			else
			{
				Debug.LogError(MSMiniJobManager.instance.currActiveJob.miniJob.name);
				
				if(doneIcon != null)
				{
					doneIcon.gameObject.SetActive(false);
				}
				
				if(MSMiniJobManager.instance.currActiveJob.miniJob != null)
				{
					bubbleIcon.gameObject.SetActive(true);
					bubbleIcon.spriteName = MSMiniJobManager.instance.currActiveJob.miniJob.quality.ToString().ToLower() + "job";
					bubbleIcon.MakePixelPerfect();
				}
			}
		}
	}
	
	/// <summary>
	/// CHEATS BITCH
	/// </summary>
	[ContextMenu("Force Fast Complete")]
	public void CheatSpeedFinish()
	{
		bar.FastComplete();
	}
	
}
