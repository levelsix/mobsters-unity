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
		{MonsterProto.MonsterElement.DARKNESS, "mininight"},
		{MonsterProto.MonsterElement.FIRE, "minifire"},
		{MonsterProto.MonsterElement.GRASS, "miniearth"},
		{MonsterProto.MonsterElement.LIGHTNING, "minisun"},
		{MonsterProto.MonsterElement.WATER, "miniwater"}
	};
	
	void Awake()
	{
		button = GetComponent<UIButton>();
	}
	
	public void Init(PZMonster goon)
	{
		monster = goon;
		
		background.spriteName = backgroundDict[goon.monster.element];
		
		//Get goon sprite from goon
		
		bar.fillAmount = (float)goon.currHP / goon.maxHP;
		
		button.isEnabled = (goon.currHP > 0);
		
		background.alpha = 1;
		bar.alpha = 1;
		goonSprite.alpha = 0;
	}
	
	public void InitEmpty()
	{
		monster = null;
		background.alpha = 0;
		bar.alpha = 0;
		goonSprite.alpha = 0;
	}
	
	public void OnClick()
	{
		if (monster != null)
		{
			Debug.Log("Deploy!");
			CBKEventManager.Puzzle.OnDeploy(monster);
		}
	}
}
