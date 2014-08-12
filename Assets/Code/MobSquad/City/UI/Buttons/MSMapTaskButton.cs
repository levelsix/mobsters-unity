using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSMapTaskButton : MonoBehaviour {


	Transform trans;

	MSTaskable taskable;

	[SerializeField]
	TweenScale scaleTween;

	public UIButton button;

	[SerializeField]
	UILabel buttonLabel;

	[SerializeField]
	UILabel levelTitle;

	[SerializeField]
	UISprite glowRing;

	[SerializeField]
	UI2DSprite bossSprite;

	TaskMapElementProto _mapTask;
	
	public TaskMapElementProto mapTask
	{
		set
		{
			_mapTask = value;
			if(_mapTask.boss)
			{
				bossSprite.alpha = 1f;
				MSSpriteUtil.instance.SetSprite(_mapTask.bossImgName.Substring(0,_mapTask.bossImgName.Length - "Map2.png".Length),
				                                MSUtil.StripExtensions(_mapTask.bossImgName),
				                                bossSprite);
				bossSprite.MarkAsChanged();
				bossSprite.MakePixelPerfect();
			}
			else
			{
				bossSprite.alpha = 0f;
			}
		}
		
		get
		{
			return _mapTask;
		}
	}

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
	}

	void OnEnable(){
//		halo.alpha = 0f;
		levelTitle.alpha = 0f;
		glowRing.gameObject.SetActive(false);
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

		buttonLabel.alpha = mapTask.boss?0f:1f;
		
		buttonLabel.depth = GetComponent<UISprite> ().depth + 1;
//		halo.depth = buttonLabel.depth - 2;
//		shadow.depth = buttonLabel.depth - 2;
		glowRing.depth = buttonLabel.depth - 2;
		bossSprite.depth = buttonLabel.depth + 1;

		buttonLabel.text = task.mapElementId.ToString();
		levelTitle.text = MSDataManager.instance.Get<FullTaskProto> (task.taskId).name;
		levelTitle.depth = buttonLabel.depth;
	}

	void Deselect(TaskMapElementProto mapTask, MSMapTaskButton.TaskStatusType status){
		if(mapTask.mapElementId != _mapTask.mapElementId)
		{
//			halo.alpha = 0f;
			glowRing.gameObject.SetActive(false);
			levelTitle.alpha = 0f;
			TweenScale.Begin(gameObject,0.5f, new Vector3(0.5f,0.5f,0.5f));
		}
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
