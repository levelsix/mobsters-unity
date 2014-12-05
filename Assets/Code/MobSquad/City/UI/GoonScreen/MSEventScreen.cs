using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSEventScreen : MSFunctionalScreen {

	[SerializeField]
	UISprite bg;

	[SerializeField]
	UISprite bgLeftCap;

	[SerializeField]
	UISprite bgRightCap;

	[SerializeField]
	UISprite tag;

	[SerializeField]
	UILabel eventTitle;

	[SerializeField]
	UILabel description;

	[SerializeField]
	UIButton enterButton;
	MSTaskable buttonTask;

	[SerializeField]
	UILabel timeLabel;

	[SerializeField]
	UILabel time;

	[SerializeField]
	UILabel topTitle;

	[SerializeField]
	UI2DSprite characterSprite;

	Color darkColor;
	Color color;
	Color lightColor;

	PersistentEventProto persEvent;

	const string EVENT_BG = "eventbigbg";
	const string EVENT_BG_CAP = "eventbigbgcap";
	const string EVENT_TAG = "eventtag";

	void Awake()
	{
		buttonTask = enterButton.GetComponent<MSTaskable>();
	}

	void OnEnable()
	{
		MSActionManager.Dungeon.OnBeginEventDungeonSuccess += EnterEvent;
	}

	void OnDisable()
	{
		MSActionManager.Dungeon.OnBeginEventDungeonSuccess -= EnterEvent;
	}
	
	public override void Init()
	{

	}

	public void Init(Color darkColor, Color color, Color lightColor, PersistentEventProto pEvent)
	{
		persEvent = pEvent;
		if(buttonTask == null)
		{
			buttonTask = enterButton.GetComponent<MSTaskable>();
		}

		this.darkColor = darkColor;
		this.color = color;
		this.lightColor = lightColor;

		eventTitle.gradientBottom = lightColor;
		eventTitle.effectColor = darkColor;

		description.gradientBottom = lightColor;
		description.effectColor = darkColor;

		timeLabel.gradientBottom = lightColor;
		timeLabel.effectColor = darkColor;

		time.gradientBottom = lightColor;
		time.effectColor = darkColor;

		buttonTask.GetComponent<MSActionButton>().onClick = null;
		if(MSEventManager.instance.IsOnCooldown(pEvent))
		{
			timeLabel.text = "REENTER IN:";
			time.text = MSUtil.TimeStringShort(MSEventManager.instance.GetRemainingCoolDown(pEvent));
			buttonTask.GetComponent<MSActionButton>().onClick += CoolDownWithGems;
		}
		else
		{
			timeLabel.text = "ENDS IN:";
			float minutes = pEvent.startHour * 60 + pEvent.eventDurationMinutes - DateTime.Now.Hour * 60 + DateTime.Now.Minute;
			float hours = Mathf.Floor(minutes / 60);
			minutes -= Mathf.Floor(hours * 60);
			time.text = hours + "H " + minutes + "M";
			buttonTask.GetComponent<MSActionButton>().onClick += ClickEnterEvent;
		}

		buttonTask.locked = true;
		buttonTask.task = MSDataManager.instance.Get<FullTaskProto>(pEvent.taskId);

		eventTitle.text = buttonTask.task.name;
		topTitle.text = buttonTask.task.name;

		switch(pEvent.monsterElement)
		{
		case Element.LIGHT:
			bg.spriteName = "light" + EVENT_BG;
			bgLeftCap.spriteName = "light" + EVENT_BG_CAP;
			bgRightCap.spriteName = "light" + EVENT_BG_CAP;
			tag.spriteName = "light" + EVENT_TAG;
			break;
		case Element.EARTH:
			bg.spriteName = "earth" + EVENT_BG;
			bgLeftCap.spriteName = "earth" + EVENT_BG_CAP;
			bgRightCap.spriteName = "earth" + EVENT_BG_CAP;
			tag.spriteName = "earth" + EVENT_TAG;
			break;
		case Element.WATER:
			bg.spriteName = "water" + EVENT_BG;
			bgLeftCap.spriteName = "water" + EVENT_BG_CAP;
			bgRightCap.spriteName = "water" + EVENT_BG_CAP;
			tag.spriteName = "water" + EVENT_TAG;
			break;
		case Element.DARK:
			bg.spriteName = "night" + EVENT_BG;
			bgLeftCap.spriteName = "night" + EVENT_BG_CAP;
			bgRightCap.spriteName = "night" + EVENT_BG_CAP;
			tag.spriteName = "night" + EVENT_TAG;
			break;
		case Element.FIRE:
			bg.spriteName = "fire" + EVENT_BG;
			bgLeftCap.spriteName = "fire" + EVENT_BG_CAP;
			bgRightCap.spriteName = "fire" + EVENT_BG_CAP;
			tag.spriteName = "fire" + EVENT_TAG;
			break;
		default:
			Debug.LogError("non valid event element specified");
			break;
		}

		if(pEvent.type == PersistentEventProto.EventType.ENHANCE)
		{
			switch(pEvent.monsterElement)
			{
			case Element.LIGHT:
				MSSpriteUtil.instance.SetSprite("CakeKid4T1","CakeKid4T1Character", characterSprite);
				break;
			case Element.EARTH:
				MSSpriteUtil.instance.SetSprite("CakeKid2T1","CakeKid2T1Character", characterSprite);
				break;
			case Element.WATER:
				MSSpriteUtil.instance.SetSprite("CakeKid3T1","CakeKid3T1Character", characterSprite);
				break;
			case Element.DARK:
				MSSpriteUtil.instance.SetSprite("CakeKid5T1","CakeKid5T1Character", characterSprite);
				break;
			case Element.FIRE:
				MSSpriteUtil.instance.SetSprite("CakeKid1T1","CakeKid1T1Character", characterSprite);
				break;
			default:
				Debug.LogError("non valid event element specified");
				break;
			}
		}
		else if(pEvent.type == PersistentEventProto.EventType.EVOLUTION)
		{
			switch(pEvent.monsterElement)
			{
			case Element.LIGHT:
				MSSpriteUtil.instance.SetSprite("Scientist4T1","Scientist4T1Character", characterSprite);
				break;
			case Element.EARTH:
				MSSpriteUtil.instance.SetSprite("Scientist2T1","Scientist2T1Character", characterSprite);
				break;
			case Element.WATER:
				MSSpriteUtil.instance.SetSprite("Scientist3T1","Scientist3T1Character", characterSprite);
				break;
			case Element.DARK:
				MSSpriteUtil.instance.SetSprite("Scientist5T1","Scientist5T1Character", characterSprite);
				break;
			case Element.FIRE:
				MSSpriteUtil.instance.SetSprite("Scientist1T1","Scientist1T1Character", characterSprite);
				break;
			default:
				Debug.LogError("non valid event element specified");
				break;
			}
		}
	}

	void ClickEnterEvent()
	{
		enterButton.GetComponent<MSLoadLock>().Lock();
		MSEventManager.instance.DoBeginDungeonRequest(persEvent, 0, enterButton.GetComponent<MSLoadLock>().Unlock);
	}

	void CoolDownWithGems()
	{
		if(MSResourceManager.instance.Spend(ResourceType.GEMS, MSMath.GemsForTime(MSEventManager.instance.GetRemainingCoolDown(persEvent),false)))
		{
			enterButton.GetComponent<MSLoadLock>().Lock();
			MSEventManager.instance.DoBeginDungeonRequest(persEvent,
			                                              MSMath.GemsForTime(MSEventManager.instance.GetRemainingCoolDown(persEvent),false),
			                                              enterButton.GetComponent<MSLoadLock>().Unlock);
		}
	}

	void EnterEvent()
	{
		buttonTask.EngageTask();
	}

	public override bool IsAvailable()
	{
		return true;
	}

}
