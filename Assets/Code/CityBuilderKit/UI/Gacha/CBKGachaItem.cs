using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// CBKGachaItem
/// @author Rob Giusti
/// </summary>
public class CBKGachaItem : MonoBehaviour {

	public Transform trans;

	BoosterPackProto pack;

	public CBKLoopingElement looper;

	public TweenPosition tween;

	int[] chances;

	int maxChance;

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UISprite background;

	[SerializeField]
	public UILabel label;

	public CBKGachaSpinner spinner;

	void Awake()
	{
		trans = transform;

		looper = GetComponent<CBKLoopingElement>();
		looper.onLoop += OnLoop;

		tween = GetComponent<TweenPosition>();
	}

	void OnLoop()
	{
		PickItem();
		spinner.lastToLoop = this;
	}

	public void Init(BoosterPackProto pack)
	{
		this.pack = pack;
		chances = new int[pack.displayItems.Count];
		for (int i = 0; i < pack.displayItems.Count; i++)
		{
			maxChance += pack.displayItems[i].quantity;
			chances[i] = pack.displayItems[i].quantity;
		}

		Debug.Log (chances);

		PickItem();
	}

	void PickItem()
	{
		int choice = UnityEngine.Random.Range(0, maxChance);
		foreach (var item in pack.displayItems) 
		{
			if (item.quantity > choice)
			{
				Setup(item);
				break;
			}
			choice -= item.quantity;
		}
	}

	void Setup(BoosterDisplayItemProto item)
	{
		if (item.isMonster)
		{
			label.text = item.quality.ToString();
			string rarity = item.quality.ToString().ToLower();
			background.spriteName = "gacha" + rarity + "bg";
			if (item.isComplete)
			{
				icon.spriteName = rarity + "capsule";
			}
			else
			{
				icon.spriteName = rarity + "piece";
			}
		}
		else
		{
			background.spriteName = "gachagemsbg";
			icon.spriteName = "diamond";
			label.text = item.gemReward + " GEMS";
		}

		UISpriteData data = icon.GetAtlasSprite();
		if (data != null)
		{
			icon.height = data.height;
			icon.width = data.width;
		}
	}

	public void Drag(Vector2 drag)
	{
		trans.localPosition += new Vector3(drag.x, drag.y, 0);
	}
}
