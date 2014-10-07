using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSMapTaskPopup : MonoBehaviour {

	[SerializeField]
	UILabel level;

	[SerializeField]
	UILabel cashLabel;

	[SerializeField]
	UILabel oilLabel;

	[SerializeField]
	UILabel levelTitle;

	[SerializeField]
	UIButton button;

	[SerializeField]
	UISprite background;

	FullTaskProto task;

	const string CANCEL_BUTTON = "greysmallbutton";

	const string ACCEPT_BUTTON = "acceptrequest";

	const string LOCKED_BACKGROUND = "lockeddailylab";
	
	const string FIRE_BACKGROUND = "firedailylab";
	
	const string EARTH_BACKGROUND = "earthdailylab";
	
	const string LIGHT_BACKGROUND = "lightdailylab";
	
	const string NIGHT_BACKGROUND = "nightdailylab";
	
	const string WATER_BACKGROUND = "waterdailylab";

	const string RAINBOW_BACKGROUND = "rainbowbar";

	[HideInInspector]
	public Transform trans;

	[SerializeField]
	UISprite pvpBg;

	[SerializeField]
	UILabel eventStatus;

	[SerializeField]
	UILabel statusLabel;

	[SerializeField]
	UISprite eventIcon;
	
	[SerializeField]
	UISprite eventIconB;

	void Awake()
	{
		trans = transform;
		EventDelegate.Add(button.onClick, delegate {
			PZScrollingBackground.instance.SetBackgrounds(task);
		});
		background.width = (int)(MSMath.uiScreenWidth - pvpBg.width);
	}

	public void init(TaskMapElementProto mapTask, MSMapTaskButton.TaskStatusType statusType)
	{
		task = MSDataManager.instance.Get<FullTaskProto> (mapTask.taskId);
		init (task, mapTask.element);

		levelTitle.text = task.name;
		level.text = "Level " + mapTask.mapElementId;

		cashLabel.text = mapTask.cashReward.ToString();
		oilLabel.text = mapTask.oilReward.ToString();
		cashLabel.MarkAsChanged();
		oilLabel.MarkAsChanged();

		button.enabled = true;

		if (statusType == MSMapTaskButton.TaskStatusType.Completed)
		{
			button.normalSprite = ACCEPT_BUTTON;
			cashLabel.text = "0";
			oilLabel.text = "0";
//			status.text = "Completed";
		} 
//		else if(statusType == MSMapTaskButton.TaskStatusType.Undefeated)
//		{
//			button.normalSprite = ACCEPT_BUTTON;
//			status.text = "Undefeated";
//		}
		else if(statusType != MSMapTaskButton.TaskStatusType.Undefeated)
		{
//			status.text = "Locked";
			background.spriteName = LOCKED_BACKGROUND;
			button.normalSprite = CANCEL_BUTTON;
			button.GetComponent<MSTaskable> ().locked = true;
		}
	}

	public void init(PersistentEventProto pEvent)
	{
		task = MSDataManager.instance.Get<FullTaskProto>(pEvent.taskId);
		init (task, pEvent.monsterElement);
		float minutes = pEvent.startHour * 60 + pEvent.eventDurationMinutes - DateTime.Now.Hour * 60 + DateTime.Now.Minute;
		float hours = Mathf.Floor(minutes / 60);
		minutes -= Mathf.Floor(hours * 60);
		eventStatus.text = hours + "H " + minutes + "M";

		levelTitle.text = MSDataManager.instance.Get<FullTaskProto>(pEvent.taskId).name;

		switch(pEvent.type)
		{
		case PersistentEventProto.EventType.ENHANCE:
			eventIcon.spriteName = "FatBoy" + pEvent.monsterElement.ToString().ToLower();
			eventIconB.spriteName = pEvent.monsterElement.ToString().ToLower() + "feederevent";
			break;
		case PersistentEventProto.EventType.EVOLUTION:
			eventIcon.spriteName = "Scientist" + pEvent.monsterElement.ToString().ToLower();
			break;
		default:
			break;
		}
		statusLabel.MarkAsChanged();
		statusLabel.MakePixelPerfect();
	}
	
	public void init(FullTaskProto task, Element element)
	{
		button.GetComponent<MSTaskable> ().task = task;
		button.GetComponent<MSTaskable> ().locked = false;
		
		switch (element) {
		case Element.FIRE:
			background.spriteName = FIRE_BACKGROUND;
			break;
		case Element.EARTH:
			background.spriteName = EARTH_BACKGROUND;
			break;
		case Element.LIGHT:
			background.spriteName = LIGHT_BACKGROUND;
			break;
		case Element.DARK:
			background.spriteName = NIGHT_BACKGROUND;
			break;
		case Element.WATER:
			background.spriteName = WATER_BACKGROUND;
			break;
		case Element.NO_ELEMENT:
			background.spriteName = RAINBOW_BACKGROUND;
			break;
		default:
			background.spriteName = "";
			break;
		}

	}
}
