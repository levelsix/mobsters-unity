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
	UISprite background;

	[SerializeField]
	UISprite icon;

	[SerializeField]
	public UILabel label;

	[SerializeField]
	UI2DSprite sprite2D;

	[SerializeField]
	UISprite rarityTag;

	[SerializeField]
	UILabel pieceLabel;

	[SerializeField]
	Color xpColor;

	[SerializeField]
	Color cashColor;

	public UISprite sprite;

	TweenPosition tweenPos;

	TweenAlpha tweenAlpha;

	const string LOST_ITEM_BORDER = "youlostitemborder";
	const string LOST_ITEM_BG = "youlostitembg";

	const string WON_ITEM_BORDER = "youwonitemborder";
	const string WON_ITEM_BG = "youwonitembg";

	void Awake(){
		tweenPos = GetComponent<TweenPosition> ();
		tweenAlpha = GetComponent<TweenAlpha> ();
		sprite = GetComponent<UISprite> ();
	}

	void OnEnable(){
		if (sprite2D != null)
		{
			sprite2D.alpha = 0f;
		}
		if (label != null)
		{
//			label.alpha = 0f;
		}
		if (rarityTag != null)
		{
			rarityTag.alpha = 0f;
		}
		if (pieceLabel != null)
		{
			pieceLabel.alpha = 0f;
		}
		if (background != null)
		{
			background.spriteName = WON_ITEM_BG;
		}
	}

	void init()
	{
		icon.alpha = 1f;
		if(border != null)
		{
			border.spriteName = WON_ITEM_BORDER;
		}
	}

	public void InitXP(int amount)
	{
		init();
		label.text = "+" + amount;
		label.color = xpColor;
		icon.spriteName = "xp";
		icon.MakePixelPerfect();
	}

	public void InitOil(int amount)
	{
		init();
		label.text = amount.ToString();
		label.color = MSColors.oilTextColor;
		icon.spriteName = "oilicon";
		icon.MakePixelPerfect();
	}

	public void InitCash(int amount)
	{
		init();
		label.text = "$" + amount;
		label.color = cashColor;
		icon.spriteName = "moneystack";
		icon.MakePixelPerfect();
	}

	public void InitDiamond(int amount)
	{
		init();
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
		init();
		icon.alpha = 0f;
		label.text = monster.quality.ToString();
		label.alpha = 0f;
		string rarity = monster.quality.ToString().ToLower();
		if (monster.numPuzzlePieces > 1)
		{
			pieceLabel.alpha = 1f;
		}
		MSSpriteUtil.instance.SetSprite(monster.imagePrefix,monster.imagePrefix+"Thumbnail",sprite2D);
		sprite2D.MakePixelPerfect();
		sprite2D.depth = icon.depth + 1;
		rarityTag.alpha = 1f;
		rarityTag.spriteName = "battle" + rarity + "tag";
		rarityTag.MakePixelPerfect();
	}

	public void InitItem(ItemProto item){
		init();
		label.text = item.name;
		if (border != null)
		{
			//if (item.borderImgName != null) {
			//	border.spriteName = item.borderImgName;
			//} else {
			//	border.spriteName = "";
			//}
		}
		icon.spriteName = item.imgName.Substring (0, item.imgName.Length - ".png".Length);
	}

	/// <summary>
	/// If a player's team is blacked out then we show them what they lost
	/// </summary>
	public void SetToLostPrize()
	{
		if(background != null)
		{
			background.spriteName = LOST_ITEM_BG;
		}
		if(border != null)
		{
			border.spriteName = LOST_ITEM_BORDER;
		}
	}

	/// <summary>
	/// Starts the tween animations for collected items to slide in and be collected
	/// </summary>
	/// <param name="position">The position that the tween will end with</param>
	public void SlideIn(Vector3 position){

		Vector3 startPostition = new Vector3 (position.x + 100, position.y, position.z);
		tweenPos.from = startPostition;
		tweenPos.to = position;

//		float time = (startPostition.x - position.x) / 400f;

//		tweenPos.duration = time;

		tweenPos.ResetToBeginning ();
		tweenPos.PlayForward ();

		tweenAlpha.ResetToBeginning ();
		tweenAlpha.PlayForward ();

		//Debug.LogWarning (time);
	}
}
