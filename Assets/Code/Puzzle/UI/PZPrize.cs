using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// PZPrize
/// </summary>
public class PZPrize : MonoBehaviour {

	[SerializeField]
	UISprite border;

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UILabel label;

	[SerializeField]
	Color xpColor;

	[SerializeField]
	Color cashColor;

	static readonly Dictionary<MonsterProto.MonsterQuality, Color> textColors = new Dictionary<MonsterProto.MonsterQuality, Color>()
	{
		{MonsterProto.MonsterQuality.COMMON, Color.grey},
		{MonsterProto.MonsterQuality.EPIC, new Color(.6f, .2f, .9f)},
		{MonsterProto.MonsterQuality.EVO, Color.red},
		{MonsterProto.MonsterQuality.LEGENDARY, Color.red},
		{MonsterProto.MonsterQuality.RARE, new Color(.3f, .3f, 1)},
		{MonsterProto.MonsterQuality.ULTRA, Color.yellow}
	};

	public void InitXP(int amount)
	{
		label.text = "+" + amount;
		label.color = xpColor;
		border.spriteName = "expfound";
		icon.spriteName = "xp";
		FixIcon();
	}

	public void InitCash(int amount)
	{
		label.text = "$" + amount;
		label.color = cashColor;
		border.spriteName = "cashfound";
		icon.spriteName = "moneystack";
		FixIcon();
	}

	public void InitEnemy(MonsterProto monster)
	{
		label.text = monster.quality.ToString();
		string rarity = monster.quality.ToString().ToLower();
		border.spriteName = rarity + "found";
		icon.spriteName = rarity;
		if (monster.numPuzzlePieces > 1)
		{
			icon.spriteName += "piece";
		}
		else
		{
			icon.spriteName += "capsule";
		}
		FixIcon();
	}

	void FixIcon()
	{
		UISpriteData data = icon.GetAtlasSprite();
		if (data != null)
		{
			icon.width = data.width;
			icon.height = data.height;
		}
	}
}
