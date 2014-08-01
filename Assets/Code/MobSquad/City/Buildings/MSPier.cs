using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSPier : MSBuildingFrame {
	
	MSBuilding pier;
	MSProgressBar bar;
	MSSimplePoolable doneIcon;
	
	const long SECONDS_IN_MINUTE = 60;
	
	const long MILISECONDS_IN_MINUTE = 1000 * SECONDS_IN_MINUTE;
	
	readonly Vector3 buildingAngle = new Vector3(45f,45f,0f);
	readonly Vector3 buildingScale = new Vector3(0.02f, 0.02f, 0.02f);
	
	void Awake()
	{
		pier = GetComponent<MSBuilding>();
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
		if(MSMiniJobManager.instance.currActiveJob != null)
		{
			InitProgressBar(MSMiniJobManager.instance.currActiveJob);
		}
		CheckTag();
	}
	
	void InitProgressBar(UserMiniJobProto job)
	{
		if(bubbleIcon != null)
		{
			bubbleIcon.gameObject.SetActive(true);
			bubbleIcon.spriteName = MSMiniJobManager.instance.currActiveJob.miniJob.quality.ToString().ToLower() + "job";
			bubbleIcon.MakePixelPerfect();
		}

		if(job.timeCompleted == 0)
		{
			bar.init(job.timeStarted, (long)job.durationMinutes * MILISECONDS_IN_MINUTE);
		}
	}
	
	void SpawnJobDoneIcon(){
		if(doneIcon == null)
		{
			doneIcon = MSPoolManager.instance.Get<MSSimplePoolable>(MSPrefabList.instance.miniJobDone, pier.trans);
			doneIcon.transf.localScale = buildingScale;
			doneIcon.transf.localEulerAngles = buildingAngle;
			doneIcon.transf.localPosition = new Vector3(0f,-0.5f,0f);
		}
		doneIcon.gameObject.SetActive(true);
	}
	
	
	public override void CheckTag(){

		if(bubbleIcon != null)
		{
			bubbleIcon.gameObject.SetActive(false);
		}

		if(bar!= null && !bar.isActiveTimeFrame)
		{
			if(MSMiniJobManager.instance.isCompleted)//There is a finished job
			{
				SpawnJobDoneIcon();
			}
			else //there are no active jobs
			{
				if(doneIcon != null)
				{
					doneIcon.gameObject.SetActive(false);
				}
				if(bubbleIcon != null)
				{
					bubbleIcon.gameObject.SetActive(false);
					if(pier.combinedProto.structInfo.level == 0)
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
			}
		}
		else //there is an active job
		{
			if(doneIcon != null)
			{
				doneIcon.gameObject.SetActive(false);
			}

			if(bubbleIcon != null)
			{
				bubbleIcon.gameObject.SetActive(true);
				bubbleIcon.spriteName = MSMiniJobManager.instance.currActiveJob.miniJob.quality.ToString().ToLower() + "job";
				bubbleIcon.MakePixelPerfect();
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
