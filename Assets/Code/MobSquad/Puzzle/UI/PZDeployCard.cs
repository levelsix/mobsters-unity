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
		monster = goon;
		
		background.spriteName = backgroundDict[goon.monster.monsterElement];

		
		string goonPrefix = MSUtil.StripExtensions (goon.monster.imagePrefix);
		MSSpriteUtil.instance.SetSprite(goonPrefix, goonPrefix + "Thumbnail", goonSprite);
		
		bar.fillAmount = (float)goon.currHP / goon.maxHP;
		
		button.isEnabled = (goon.currHP > 0);
		
		background.alpha = 1;
		bar.alpha = 1;
		goonSprite.alpha = 1;
	}
	
	public void InitEmpty()
	{
		monster = null;
		background.spriteName = EMPTY_BOX;
		bar.alpha = 0;
		goonSprite.alpha = 0;
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
