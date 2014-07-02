using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSMapTaskButton : MonoBehaviour {

	TaskMapElementProto mapTask;

	Transform trans;

	const string OPEN_CITY = "opencitypin";
	const string CLOSED_CITY = "closedcitypin";

	void Awake(){
		trans = transform;
	}

	public void initTaskButton(TaskMapElementProto task){
		trans.localPosition = new Vector3(task.xPos, task.yPos, 0f);
		mapTask = task;
	}

	public void OnClick(){
		Debug.Log ("clikc on task " + mapTask.taskId);
	}
}
