using UnityEngine;
using System.Collections;

public class MSPier : MSBuildingFrame {

	MSBuilding pier;

	void Awake()
	{
		pier = GetComponent<MSBuilding>();
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
