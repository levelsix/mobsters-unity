using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSGoonCircleIcon : MonoBehaviour {

	[SerializeField]
	UILabel goonName;

	[SerializeField]
	UISprite barBg;

	[SerializeField]
	MSFillBar hpbar;
	
	[SerializeField]
	UI2DSprite icon;
	
	[SerializeField]
	UISprite background;

	static readonly Dictionary<Element, string> backgroundElementDict = new Dictionary<Element, string>()
	{
		{Element.DARK, "nightteam"},
		{Element.FIRE, "fireteam"},
		{Element.EARTH, "earthteam"},
		{Element.LIGHT, "lightteam"},
		{Element.WATER, "waterteam"}
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
			barBg.alpha = 1;

			string mobsterPrefix = MSUtil.StripExtensions (monster.monster.imagePrefix);
			StartCoroutine(MSAtlasUtil.instance.SetSprite(mobsterPrefix, mobsterPrefix + "Thumbnail", icon));
			
			hpbar.fill = ((float)monster.currHP) / monster.maxHP;
		}
	}
}
