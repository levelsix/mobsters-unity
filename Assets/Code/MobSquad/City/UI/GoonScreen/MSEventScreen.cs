using UnityEngine;
using System.Collections;
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

	Color darkColor;
	Color color;
	Color lightColor;

	const string EVENT_BG = "eventbigbg";
	const string EVENT_BG_CAP = "eventbigbgcap";
	const string EVENT_TAG = "eventtag";

	void Awake()
	{
		buttonTask = enterButton.GetComponent<MSTaskable>();
	}

	public override void Init()
	{

	}

	public void Init(Color darkColor, Color color, Color lightColor, PersistentEventProto pEvent)
	{
		this.darkColor = darkColor;
		this.color = color;
		this.lightColor = lightColor;

		eventTitle.gradientBottom = lightColor;
		eventTitle.effectColor = darkColor;

		description.gradientBottom = lightColor;
		description.effectColor = darkColor;

		time.gradientBottom = lightColor;
		time.effectColor = darkColor;

		if(buttonTask == null)
		{
			buttonTask = enterButton.GetComponent<MSTaskable>();
		}
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
	}

	public override bool IsAvailable()
	{
		return true;
	}

}
