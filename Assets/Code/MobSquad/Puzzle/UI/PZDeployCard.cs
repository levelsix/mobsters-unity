using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using System.Collections.Generic;

public class PZDeployCard : MonoBehaviour {
	
	[SerializeField]
	UISprite background;
	
	[SerializeField]
	UI2DSprite goonSprite;
	
	[SerializeField]
	UISprite bar;

	[SerializeField]
	UILabel hpLabel;
	
	UIButton button;
	
	PZMonster monster;
	
	static readonly Dictionary<Element, string> backgroundDict = new Dictionary<Element, string>()
	{
		{Element.DARK, "nightteam"},
		{Element.FIRE, "fireteam"},
		{Element.EARTH, "earthteam"},
		{Element.LIGHT, "lightteam"},
		{Element.WATER, "waterteam"}
	};

	const string EMPTY_BOX = "teamempty";
	
	void Awake()
	{
		button = GetComponent<UIButton>();
	}
	
	public void Init(PZMonster goon)
	{
		//Debug.Log("Init slot " + goon.userMonster.teamSlotNum + ": " + goon.monster.displayName);

		monster = goon;
		
		button.normalSprite = background.spriteName = MSGoonCard.smallBackgrounds[goon.monster.monsterElement];
		
		string goonPrefix = MSUtil.StripExtensions (goon.monster.imagePrefix);
		MSSpriteUtil.instance.SetSprite(goonPrefix, goonPrefix + "Card", goonSprite, 1, Resize);
		
		bar.fillAmount = (float)goon.currHP / goon.maxHP;
		
		button.isEnabled = (goon.currHP > 0);
		goonSprite.color = button.isEnabled ? Color.white : Color.black;
		
		background.alpha = 1;
		bar.alpha = 1;
		goonSprite.alpha = 1;

		hpLabel.text = goon.currHP + "/" + goon.maxHP;
	}

	void Resize()
	{
		if (goonSprite.width > background.width || goonSprite.height > background.height)
		{
			float ratio;
			if (goonSprite.width > goonSprite.height)
			{
				ratio = background.width/((float)goonSprite.width);
			}
			else
			{
				ratio = background.height/((float)goonSprite.height);
			}
			goonSprite.width = Mathf.FloorToInt(goonSprite.width*ratio);
			goonSprite.height = Mathf.FloorToInt(goonSprite.height*ratio);
		}
	}
	
	public void InitEmpty()
	{
//		Debug.Log("Init empty");

		monster = null;
		button.normalSprite = background.spriteName = EMPTY_BOX;
		bar.alpha = 0;
		goonSprite.alpha = 0;
		hpLabel.text = " ";
	}
	
	public void OnClick()
	{
		if (monster != null)
		{
//			Debug.Log("Deploy!");
			MSActionManager.Puzzle.OnDeploy(monster);
		}
	}
}
