using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[RequireComponent (typeof (MSSimplePoolable))]
public class MSQuestRewardBox : MonoBehaviour {

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UILabel label;

	public void Pool ()
	{
		GetComponent<MSSimplePoolable>().Pool();
	}
	
	public void InitXP(int amount)
	{
		label.text = "+" + amount;
		label.color = MSColors.oilTextColor;
		icon.spriteName = "levelicon";
		icon.MakePixelPerfect();
	}
	
	public void InitCash(int amount)
	{
		label.text = "$" + amount;
		label.color = MSColors.cashTextColor;
		icon.spriteName = "moneystack";
		icon.MakePixelPerfect();
	}

	public void InitOil(int amount)
	{
		label.text = amount.ToString();
		label.color = MSColors.oilTextColor;
	}
	
	public void InitDiamond(int amount)
	{
		label.text = amount.ToString();
		label.color = MSColors.gemTextColor;
		icon.spriteName = "diamond";
		icon.MakePixelPerfect();
	}
	
	public void InitEnemy(int monsterId)
	{
		InitEnemy(MSDataManager.instance.Get<MonsterProto>(monsterId));
	}
	
	public void InitEnemy(MonsterProto monster)
	{
		label.text = monster.quality.ToString();
		label.color = MSColors.qualityColors[monster.quality];
		icon.spriteName = monster.quality.ToString().ToLower();
		if (monster.numPuzzlePieces > 1)
		{
			icon.spriteName += "piece";
		}
		else
		{
			icon.spriteName += "capsule";
		}
		icon.MakePixelPerfect();
	}
	
}
