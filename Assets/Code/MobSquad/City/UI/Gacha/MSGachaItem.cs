using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// CBKGachaItem
/// @author Rob Giusti
/// </summary>
public class MSGachaItem : MonoBehaviour {
	
	BoosterPackProto pack;
	
	MSLoopingElement looper;
	
	int[] chances;
	
	int maxChance;
	
	[SerializeField]
	UISprite icon;
	
	[SerializeField]
	public UILabel label;
	
	[SerializeField]
	MSGachaSpinner spinner;
	
	void Awake()
	{
		looper = GetComponent<MSLoopingElement>();
		looper.onLoop += OnLoop;
	}
	
	void OnLoop(bool left)
	{
		PickItem();
		spinner.lastToLoop = this;
	}
	
	public void Init(BoosterPackProto pack)
	{
		this.pack = pack;
		chances = new int[pack.displayItems.Count];
		maxChance = 0;
		for (int i = 0; i < pack.displayItems.Count; i++)
		{
			maxChance += pack.displayItems[i].quantity;
			chances[i] = pack.displayItems[i].quantity;
		}
		
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
	
	public void Setup(BoosterItemProto item)
	{
		if (item.monsterId == 0)
		{
			icon.spriteName = "diamond";
			label.text = item.gemReward.ToString();
			label.color = MSColors.gemTextColor;
		}
		else
		{
			MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(item.monsterId);
			
			label.text = monster.quality.ToString();
			string rarity = monster.quality.ToString().ToLower();
			if (item.isComplete)
			{
				icon.spriteName = rarity + "ball";
			}
			else
			{
				icon.spriteName = rarity + "piece";
			}
			icon.MakePixelPerfect();
			label.color = MSColors.qualityColors[monster.quality];
		}
	}
	
	public void Setup(BoosterDisplayItemProto item)
	{
		if (item.isMonster)
		{
			label.text = item.quality.ToString();
			string rarity = item.quality.ToString().ToLower();
			if (item.isComplete)
			{
				icon.spriteName = rarity + "ball";
			}
			else
			{
				icon.spriteName = rarity + "piece";
			}
			label.color = MSColors.qualityColors[item.quality];
		}
		else
		{
			icon.spriteName = "diamond";
			label.text = item.gemReward.ToString ();
			label.color = MSColors.gemTextColor;
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
		transform.localPosition += new Vector3(drag.x, drag.y, 0);
	}
}