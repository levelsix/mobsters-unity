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
		
		button.normalSprite = background.spriteName = backgroundDict[goon.monster.monsterElement];
		
		string goonPrefix = MSUtil.StripExtensions (goon.monster.imagePrefix);
		MSSpriteUtil.instance.SetSprite(goonPrefix, goonPrefix + "Thumbnail", goonSprite);
		
		bar.fillAmount = (float)goon.currHP / goon.maxHP;
		
		button.isEnabled = (goon.currHP > 0);
		goonSprite.color = button.isEnabled ? Color.white : Color.black;
		
		background.alpha = 1;
		bar.alpha = 1;
		goonSprite.alpha = 1;

		hpLabel.text = goon.currHP + "/" + goon.maxHP;
	}
	
	public void InitEmpty()
	{
		Debug.Log("Init empty");

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
			Debug.Log("Deploy!");
			MSActionManager.Puzzle.OnDeploy(monster);
		}
	}
}
