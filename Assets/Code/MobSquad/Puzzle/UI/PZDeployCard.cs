using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using System.Collections.Generic;

public class PZDeployCard : MonoBehaviour {
	
	[SerializeField]
	UISprite background;
	
	[SerializeField]
	UISprite goonSprite;
	
	[SerializeField]
	UISprite bar;
	
	UIButton button;
	
	PZMonster monster;
	
	static readonly Dictionary<MonsterProto.MonsterElement, string> backgroundDict = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARKNESS, "nightteam"},
		{MonsterProto.MonsterElement.FIRE, "fireteam"},
		{MonsterProto.MonsterElement.GRASS, "earthteam"},
		{MonsterProto.MonsterElement.LIGHTNING, "lightteam"},
		{MonsterProto.MonsterElement.WATER, "waterteam"}
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

		goonSprite.spriteName = MSUtil.StripExtensions(goon.monster.imagePrefix) + "Thumbnail";

		UISpriteData spriteData = goonSprite.GetAtlasSprite();
		if (spriteData != null)
		{
			goonSprite.width = spriteData.width;
			goonSprite.height = spriteData.height;
		}
		
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
