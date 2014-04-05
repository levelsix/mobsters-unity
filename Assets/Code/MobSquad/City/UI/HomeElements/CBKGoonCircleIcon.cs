using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKGoonCircleIcon : MonoBehaviour {

	[SerializeField]
	UILabel goonName;

	[SerializeField]
	UISprite barBg;

	[SerializeField]
	CBKFillBar hpbar;

	[SerializeField]
	UISprite bar;
	
	[SerializeField]
	UISprite icon;
	
	[SerializeField]
	UISprite background;
	
	static readonly Dictionary<MonsterProto.MonsterElement, string> ringElementDict = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARK, "nightcardhealthbar"},
		{MonsterProto.MonsterElement.FIRE, "firecardhealthbar"},
		{MonsterProto.MonsterElement.GRASS, "earthcardhealthbar"},
		{MonsterProto.MonsterElement.LIGHT, "lightcardhealthbar"},
		{MonsterProto.MonsterElement.WATER, "watercardhealthbar"}
	};

	static readonly Dictionary<MonsterProto.MonsterElement, string> backgroundElementDict = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARK, "nightteam"},
		{MonsterProto.MonsterElement.FIRE, "fireteam"},
		{MonsterProto.MonsterElement.GRASS, "earthteam"},
		{MonsterProto.MonsterElement.LIGHT, "lightteam"},
		{MonsterProto.MonsterElement.WATER, "waterteam"}
	};

	const string emptyBackground = "teamempty";
	const string fullBackground = "memberbg";
	
	public void Init(PZMonster monster)
	{
		if (monster == null || monster.monster == null || monster.monster.monsterId == 0)
		{
			goonName.text = "Slot Empty";
			background.spriteName = emptyBackground;
			barBg.alpha = 0;
			icon.alpha = 0;
		}
		else
		{
			goonName.text = monster.monster.displayName;
			background.spriteName = backgroundElementDict[monster.monster.monsterElement];
			icon.alpha = 1;
			
			icon.spriteName = MSUtil.StripExtensions(monster.monster.imagePrefix) + "Thumbnail";

			UISpriteData spDat = icon.GetAtlasSprite();
			if (spDat != null)
			{
				icon.width = spDat.width;
				icon.height = spDat.height;
			}
			
			hpbar.fill = ((float)monster.currHP) / monster.maxHP;
			bar.spriteName = ringElementDict[monster.monster.monsterElement];
		}
	}
}
