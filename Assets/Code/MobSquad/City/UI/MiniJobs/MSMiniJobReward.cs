using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobReward
/// </summary>
[RequireComponent (typeof (MSSimplePoolable))]
public class MSMiniJobReward : MonoBehaviour {

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UILabel label;

	const string GEM_SPRITE_NAME = "diamond";
	const string CASH_SPRITE_NAME = "moneystack";
	const string OIL_SPRITE_NAME = "oilicon";

	readonly Vector3 RESOURCE_SCALE = new Vector3(0.5f, 0.5f, 1f);
	readonly Vector3 ITEM_SCALE = Vector3.one;

	void Init(string spriteName, string labelText, Color textColor)
	{
		icon.spriteName = spriteName;
		label.text = labelText;
		label.color = textColor;
		label.MarkAsChanged();
		icon.MakePixelPerfect();
	}

	public void InitCash(int amount)
	{
		Init (CASH_SPRITE_NAME, amount.ToString(), MSColors.cashTextColor);
		icon.transform.localScale = RESOURCE_SCALE;
	}

	public void InitOil(int amount)
	{
		Init (OIL_SPRITE_NAME, amount.ToString(), MSColors.oilTextColor);
		icon.transform.localScale = RESOURCE_SCALE;
	}

	public void InitGem(int amount)
	{
		Init (GEM_SPRITE_NAME, amount.ToString(), MSColors.gemTextColor);
		icon.transform.localScale = RESOURCE_SCALE;
	}

	public void InitMonster(int monsterId)
	{
		MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(monsterId);

		Init(monster.quality.ToString().ToLower() + (monster.numPuzzlePieces > 1 ? "piece" : "capsule"),
		     monster.quality.ToString().ToUpper(), 
		     MSColors.qualityColors[monster.quality]);
		icon.transform.localScale = ITEM_SCALE;

	}

	public void InitItem(int itemId)
	{
		ItemProto item = MSDataManager.instance.Get<ItemProto>(itemId);
		Init(MSUtil.StripExtensions(item.imgName), item.name, Color.black);
		icon.transform.localScale = ITEM_SCALE;
	}
}
