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

	[SerializeField]
	UISprite halo;

	[SerializeField]
	UILabel levelTitle;

	public enum TaskStatusType
	{
		Locked,
		Completed,
		Undefeated
	}

	TaskStatusType _status;

	public TaskStatusType Status{
		get{
			return _status;		
		}
		set{
			_status = value;
			if(value == TaskStatusType.Completed || value == TaskStatusType.Undefeated){
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
		button = GetComponent<UIButton> ();
	}

	void OnEnable(){
		halo.alpha = 0f;
		levelTitle.alpha = 0f;
		MSActionManager.Map.OnMapTaskClicked += Deselect;
	}

	void OnDisable(){
		MSActionManager.Map.OnMapTaskClicked -= Deselect;
	}

	public void initTaskButton(TaskMapElementProto task){
		trans.localPosition = new Vector3(task.xPos, task.yPos, 0f);
		mapTask = task;

		buttonLabel.depth = GetComponent<UISprite> ().depth + 1;
		halo.depth = buttonLabel.depth - 2;

		buttonLabel.text = task.mapElementId.ToString();
		levelTitle.text = MSDataManager.instance.Get<FullTaskProto> (task.taskId).name;
		levelTitle.depth = buttonLabel.depth;
	}

	void Deselect(TaskMapElementProto mapTask, MSMapTaskButton.TaskStatusType status){
		halo.alpha = 0f;
		levelTitle.alpha = 0f;
	}

	public void OnClick(){
		MSActionManager.Map.OnMapTaskClicked (mapTask, Status);
		if (Status != TaskStatusType.Locked) {
			halo.alpha = 1f;
		}
		levelTitle.alpha = 1f;
	}
}
