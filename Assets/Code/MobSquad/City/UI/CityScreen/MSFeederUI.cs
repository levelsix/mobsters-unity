using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSFeederUI
/// </summary>
public class MSFeederUI : MonoBehaviour {

	[SerializeField]
	GameObject feederObjects;

	[SerializeField]
	GameObject noFeederObjects;

	[SerializeField]
	UISprite background;

	[SerializeField]
	UISprite banner;

	[SerializeField]
	UILabel eventName;

	[SerializeField]
	UILabel timeLeft;

	[SerializeField]
	MSTaskable taskable;

	[SerializeField]
	Transform swapButton;

	PersistentEventProto.EventType currEventType;

	const string noFeederBackground = "blackandwhitebanner";

	Dictionary<MonsterProto.MonsterElement, string> backgroundSprites = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARK, "nightbanner"},
		{MonsterProto.MonsterElement.LIGHT, "lightbanner"},
		{MonsterProto.MonsterElement.FIRE, "firebanner"},
		{MonsterProto.MonsterElement.WATER, "waterbanner"},
		{MonsterProto.MonsterElement.GRASS, "earthbanner"}
	};

	Dictionary<MonsterProto.MonsterElement, string> bannerSprites = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARK, "nightdailylab"},
		{MonsterProto.MonsterElement.LIGHT, "lightdailylab"},
		{MonsterProto.MonsterElement.FIRE, "firedailylab"},
		{MonsterProto.MonsterElement.WATER, "waterdailylab"},
		{MonsterProto.MonsterElement.GRASS, "earthdailylab"}
	};

	void OnEnable()
	{
		currEventType = PersistentEventProto.EventType.EVOLUTION;
		swapButton.localScale = Vector3.one;
		Init ();
	}

	public void Init()
	{
		PersistentEventProto proto = MSEventManager.instance.GetActiveEvent(currEventType);
		if (proto != null)
		{
			feederObjects.SetActive(true);
			noFeederObjects.SetActive(false);

			background.spriteName = backgroundSprites[proto.monsterElement];
			banner.spriteName = bannerSprites[proto.monsterElement];

			FullTaskProto task = MSDataManager.instance.Get<FullTaskProto>(proto.taskId);
			eventName.text = task.name;
			taskable.Init(task);
		}
		else
		{
			feederObjects.SetActive(false);
			noFeederObjects.SetActive(true);
			background.spriteName = noFeederBackground;
		}
	}

	public void SwitchEventType()
	{
		if (currEventType == PersistentEventProto.EventType.EVOLUTION)
		{
			currEventType = PersistentEventProto.EventType.ENHANCE;
			swapButton.localScale = new Vector3(-1,1,1);
		}
		else
		{
			currEventType = PersistentEventProto.EventType.EVOLUTION;
			swapButton.localScale = Vector3.one;
		}
		Init ();
	}
}
