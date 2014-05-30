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

	public void InitDiamond(int amount)
	{
		label.text = amount.ToString();
		label.color = new Color(.4f, .2f, .6f);
		border.spriteName = "";
		icon.spriteName = "diamond";
		FixIcon();
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
