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

	[SerializeField]
	int iconHeight;

	const string GEM_SPRITE_NAME = "diamond";
	const string CASH_SPRITE_NAME = "moneystack";
	const string OIL_SPRITE_NAME = "oilicon";
	
	void Init(string spriteName, string labelText, Color textColor)
	{
		label.text = labelText;
		label.color = textColor;
		label.MarkAsChanged();

		//Set the icon, and resize it to get original ratio at preset height
		icon.spriteName = spriteName;
		icon.keepAspectRatio = UIWidget.AspectRatioSource.Free;
		icon.MakePixelPerfect();
		icon.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
		icon.height = iconHeight;
	}

	public void InitCash(int amount)
	{
		Init (CASH_SPRITE_NAME, amount.ToString(), MSColors.cashTextColor);
	}

	public void InitOil(int amount)
	{
		Init (OIL_SPRITE_NAME, amount.ToString(), MSColors.oilTextColor);
	}

	public void InitGem(int amount)
	{
		Init (GEM_SPRITE_NAME, amount.ToString(), MSColors.gemTextColor);
	}

	public void InitMonster(int monsterId)
	{
		MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(monsterId);

		Init(monster.quality.ToString().ToLower() + (monster.numPuzzlePieces > 1 ? "piece" : "capsule"),
		     monster.quality.ToString().ToUpper(), 
		     MSColors.qualityColors[monster.quality]);

	}

	public void InitItem(int itemId)
	{
		ItemProto item = MSDataManager.instance.Get<ItemProto>(itemId);
		Init(MSUtil.StripExtensions(item.imgName), item.name, Color.black);
		if (item.itemType == ItemType.SPEED_UP) {
			icon.height = (int)(icon.height * 1.5f);
		}
	}
}
