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

	const string gemSpriteName = "diamond";
	const string cashSpriteName = "moneystack";
	const string oilSpriteName = "oilicon";

	void Init(string spriteName, string labelText, Color textColor)
	{
		icon.spriteName = spriteName;
		label.text = labelText;
		label.color = textColor;
	}

	public void InitCash(int amount)
	{
		Init (cashSpriteName, amount.ToString(), MSColors.cashTextColor);
	}

	public void InitOil(int amount)
	{
		Init (oilSpriteName, amount.ToString(), MSColors.oilTextColor);
	}

	public void InitGem(int amount)
	{
		Init (gemSpriteName, amount.ToString(), MSColors.gemTextColor);
	}

	public void InitMonster(int monsterId)
	{
		MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(monsterId);

		Init(monster.quality.ToString().ToLower() + (monster.numPuzzlePieces > 1 ? "piece" : "capsule"),
		     monster.quality.ToString().ToUpper(), 
		     MSColors.qualityColors[monster.quality]);

	}
}
