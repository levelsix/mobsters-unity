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
	public UISprite border;

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UILabel label;

	[SerializeField]
	Color xpColor;

	[SerializeField]
	Color cashColor;

	Transform trans;

	public UISprite sprite;

	TweenPosition tweenPos;

	TweenAlpha tweenAlpha;

	static readonly Dictionary<Quality, Color> textColors = new Dictionary<Quality, Color>()
	{
		{Quality.COMMON, Color.grey},
		{Quality.EPIC, new Color(.6f, .2f, .9f)},
		{Quality.EVO, Color.red},
		{Quality.LEGENDARY, Color.red},
		{Quality.RARE, new Color(.3f, .3f, 1)},
		{Quality.ULTRA, Color.yellow}
	};

	void Awake(){
		trans = GetComponent<Transform>();
		tweenPos = GetComponent<TweenPosition> ();
		tweenAlpha = GetComponent<TweenAlpha> ();
		sprite = GetComponent<UISprite> ();
	}

	void OnEnable(){
		Color newColor = label.color;
		newColor.a = 0f;
		label.color = newColor;
	}

	public void InitXP(int amount)
	{
		border.spriteName = "expfound";
		label.text = "+" + amount;
		label.color = xpColor;
		icon.spriteName = "xp";
		icon.MakePixelPerfect();
	}

	public void InitOil(int amount)
	{
		label.text = amount.ToString();
		label.color = MSColors.oilTextColor;
		icon.spriteName = "oil";
		icon.MakePixelPerfect();
	}

	public void InitCash(int amount)
	{
		border.spriteName = "cashfound";
		label.text = "$" + amount;
		label.color = cashColor;
		icon.spriteName = "moneystack";
		icon.MakePixelPerfect();
	}

	public void InitDiamond(int amount)
	{
		border.spriteName = "";
		label.text = amount.ToString();
		label.color = new Color(.4f, .2f, .6f);
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
		icon.MakePixelPerfect();
	}

	public void InitItem(ItemProto item){
		label.text = item.name;
		if (item.borderImgName != null) {
			border.spriteName = item.borderImgName;
		} else {
			border.spriteName = "";
		}
		icon.spriteName = item.imgName.Substring (0, item.imgName.Length - ".png".Length);
	}

	/// <summary>
	/// Starts the tween animations for collected items to slide in and be collected
	/// </summary>
	/// <param name="position">The position that the tween will end with</param>
	public void SlideIn(Vector3 position){

		Vector3 startPostition = new Vector3 (340, position.y, position.z);
		tweenPos.from = startPostition;
		tweenPos.to = position;

		float time = (startPostition.x - position.x) / 400f;

		tweenPos.duration = time;

		tweenPos.enabled = true;
		tweenPos.ResetToBeginning ();
		tweenPos.PlayForward ();

		tweenAlpha.ResetToBeginning ();
		tweenAlpha.PlayForward ();

		Debug.LogWarning (time);
	}
}
