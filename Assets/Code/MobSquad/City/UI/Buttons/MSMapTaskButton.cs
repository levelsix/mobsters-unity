using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSMapTaskButton : MonoBehaviour {

	TaskMapElementProto mapTask;

	Transform trans;

	MSTaskable taskable;

	const string OPEN_CITY = "opencitypin";
	const string CLOSED_CITY = "closedcitypin";

	void Awake(){
		trans = transform;
		taskable = GetComponent<MSTaskable> ();
	}

	public void initTaskButton(TaskMapElementProto task){
		trans.localPosition = new Vector3(task.xPos, task.yPos, 0f);
		mapTask = task;
		FullTaskProto fullTask = MSDataManager.instance.Get<FullTaskProto> (mapTask.taskId);
		if (fullTask != null) {
			taskable.Init(fullTask);
		}
	}

	public void OnClick(){
		taskable.EngageTask ();
	}
}
