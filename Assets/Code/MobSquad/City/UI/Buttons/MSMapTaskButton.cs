using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSMapTaskButton : MonoBehaviour {

	public TaskMapElementProto mapTask;

	Transform trans;

	MSTaskable taskable;

	TweenScale scaleTween;

	public UIButton button;

	[SerializeField]
	UILabel buttonLabel;

	[SerializeField]
	UILabel levelTitle;

	[SerializeField]
	UISprite glowRing;

	public enum TaskStatusType
	{
		Locked,
		Completed,
		Undefeated
	}
	
	const string CLOSED_CITY = "lockedcity";
	const string CLOSED_BOSS = "lockedboss";

	TaskStatusType _status;

	public TaskStatusType Status{
		get{
			return _status;		
		}
		set{
			_status = value;
			if(value == TaskStatusType.Completed || value == TaskStatusType.Undefeated){
				SetOpenSprite();
				buttonLabel.alpha = 0.5f;
			}else{
				SetClosedSprite();
				buttonLabel.alpha = 0f;
			}
			levelTitle.alpha = 0f;
		}
	}

	void Awake(){
		trans = transform;
		button = GetComponent<UIButton> ();
		scaleTween = GetComponent<TweenScale>();
	}

	void OnEnable(){
//		halo.alpha = 0f;
		levelTitle.alpha = 0f;
		MSActionManager.Map.OnMapTaskClicked += Deselect;
	}

	void OnDisable(){
		MSActionManager.Map.OnMapTaskClicked -= Deselect;
	}

	public void SetOpenSprite()
	{
		switch(mapTask.element)
		{
		case Element.EARTH:
			button.normalSprite = "openearth";
			break;
		case Element.FIRE:
			button.normalSprite = "openfire";
			break;
		case Element.WATER:
			button.normalSprite = "openwater";
			break;
		case Element.ROCK:
			button.normalSprite = "openrock";
			break;
		case Element.DARK:
			button.normalSprite = "opendark";
			break;
		case Element.LIGHT:
			button.normalSprite = "openlight";
			break;
		case Element.NO_ELEMENT:
			button.normalSprite = "openrainbow";
			break;
		default:
			break;
		}
	}

	public void SetClosedSprite()
	{
		if(mapTask.boss)
		{
			button.normalSprite = CLOSED_BOSS;
		}
		else
		{
			button.normalSprite = CLOSED_CITY;
		}
	}

	public void initTaskButton(TaskMapElementProto task){
		if(task.boss)
		{
//			Debug.Log("boss"+task.mapElementId);
		}

		trans.localPosition = new Vector3(task.xPos, task.yPos, 0f);
		mapTask = task;

		buttonLabel.depth = GetComponent<UISprite> ().depth + 1;
//		halo.depth = buttonLabel.depth - 2;
//		shadow.depth = buttonLabel.depth - 2;
		glowRing.depth = buttonLabel.depth - 2;

		buttonLabel.text = task.mapElementId.ToString();
		levelTitle.text = MSDataManager.instance.Get<FullTaskProto> (task.taskId).name;
		levelTitle.depth = buttonLabel.depth;
	}

	void Deselect(TaskMapElementProto mapTask, MSMapTaskButton.TaskStatusType status){
//		halo.alpha = 0f;
		glowRing.gameObject.SetActive(false);
		levelTitle.alpha = 0f;
	}

	public void OnClick(){
		MSActionManager.Map.OnMapTaskClicked (mapTask, Status);
		if (Status != TaskStatusType.Locked) {
//			halo.alpha = 1f;
			glowRing.gameObject.SetActive(true);
		}
		levelTitle.alpha = 1f;

		scaleTween.ResetToBeginning();
		scaleTween.PlayForward();
	}
}
