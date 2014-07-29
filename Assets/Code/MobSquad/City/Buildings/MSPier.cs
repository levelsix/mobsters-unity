using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSPier : MSBuildingFrame {

	MSBuilding pier;
	MSProgressBar bar;

	const long SECONDS_IN_MINUTE = 60;

	const long MILISECONDS_IN_MINUTE = 1000 * SECONDS_IN_MINUTE;

	void Awake()
	{
		pier = GetComponent<MSBuilding>();
	}

	void OnEnable()
	{

		if(bar == null)
		{
			bar = MSPoolManager.instance.Get<MSProgressBar>(MSPrefabList.instance.progressBar, transform);
			bar.gameObject.SetActive(false);
		}

		MSActionManager.MiniJob.OnStartMiniJob += InitProgressBar;

		if(MSMiniJobManager.instance.currActiveJob != null)
		{
			InitProgressBar(MSMiniJobManager.instance.currActiveJob);
		}
	}

	void InitProgressBar(UserMiniJobProto job)
	{
		CheckTag();
		Debug.Log("conversion:" + job.durationMinutes + " * " + MILISECONDS_IN_MINUTE + " = " + job.durationMinutes * MILISECONDS_IN_MINUTE);
		bar.init(job.timeStarted, (long)job.durationMinutes * MILISECONDS_IN_MINUTE);
	}
	

	public override void CheckTag(){

		if(bubbleIcon != null){
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
