using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSMapTaskButton : MonoBehaviour {

	public TaskMapElementProto mapTask;

	Transform trans;

	MSTaskable taskable;

	public UIButton button;

	[SerializeField]
	UILabel buttonLabel;

	[SerializeField]
	UISprite halo;

	[SerializeField]
	UILabel levelTitle;

	[SerializeField]
	UISprite shadow;

	[SerializeField]
	Color unlockedTextColor;

	[SerializeField]
	Color lockedTextColor;

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
				if(mapTask.boss)
				{
					button.normalSprite = OPEN_BOSS;
				}
				else
				{
					button.normalSprite = OPEN_CITY;
				}
				levelTitle.color = unlockedTextColor;
				buttonLabel.alpha = 1f;
			}else{
				if(mapTask.boss)
				{
					button.normalSprite = CLOSED_BOSS;
				}
				else
				{
					button.normalSprite = CLOSED_CITY;
				}
				levelTitle.color = lockedTextColor;
				buttonLabel.alpha = 0f;
			}
			levelTitle.alpha = 0f;
		}
	}

	const string OPEN_CITY = "opencitypin";
	const string CLOSED_CITY = "lockedcitypin";

	const string OPEN_BOSS = "bosspin";
	const string CLOSED_BOSS = "bosspinlocked";

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
		if(task.boss)
		{
			Debug.Log("boss"+task.mapElementId);
		}

		trans.localPosition = new Vector3(task.xPos, task.yPos, 0f);
		mapTask = task;

		buttonLabel.depth = GetComponent<UISprite> ().depth + 1;
		halo.depth = buttonLabel.depth - 2;
		shadow.depth = buttonLabel.depth - 2;

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
