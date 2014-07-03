using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSMapTaskButton : MonoBehaviour {

	TaskMapElementProto mapTask;

	Transform trans;

	MSTaskable taskable;

	public UIButton button;

	[SerializeField]
	UILabel buttonLabel;

	bool _open;

	public bool Open{
		get{
			return _open;		
		}
		set{
			_open = value;
			if(value){
				button.normalSprite = OPEN_CITY;
				buttonLabel.alpha = 1f;
			}else{
				button.normalSprite = CLOSED_CITY;
				buttonLabel.alpha = 0f;
			}
		}
	}

	const string OPEN_CITY = "opencitypin";
	const string CLOSED_CITY = "lockedcitypin";

	void Awake(){
		trans = transform;
		taskable = GetComponent<MSTaskable> ();
		button = GetComponent<UIButton> ();
	}

	public void initTaskButton(TaskMapElementProto task){
		trans.localPosition = new Vector3(task.xPos, task.yPos, 0f);
		mapTask = task;
		FullTaskProto fullTask = MSDataManager.instance.Get<FullTaskProto> (mapTask.taskId);
		if (fullTask != null) {
			taskable.Init(fullTask);
		}
		buttonLabel.depth = GetComponent<UISprite> ().depth + 1;
		buttonLabel.text = task.mapElementId.ToString();
	}

	public void OnClick(){
		Debug.Log ("print");
	}
}
