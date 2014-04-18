using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSGoonTeamCard : MonoBehaviour {

	[SerializeField]
	MSMiniGoonBox portrait;

	[SerializeField]
	UISprite bar;

	[SerializeField]
	UISprite barBG;

	[SerializeField]
	UISprite dottedBorder;

	[SerializeField]
	MSFillBar fillBar;

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel bottomLabel;

	[SerializeField]
	Color tintColor;

	const string EMPTY_SLOT_NAME_LABEL = "Team Slot Open";
	const string EMPTY_SLOT_BOTTOM_LABEL = "Tap [33ff33]+[-] to add";

	const string HEALING_BOTTOM_LABEL = "Slot Open";

	static readonly Dictionary<MonsterProto.MonsterElement, string> healthBarForElements = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARK, "nightteamhealthbar"},
		{MonsterProto.MonsterElement.FIRE, "fireteamhealthbar"},
		{MonsterProto.MonsterElement.GRASS, "earthteamhealthbar"},
		{MonsterProto.MonsterElement.LIGHT, "lightteamhealthbar"},
		{MonsterProto.MonsterElement.WATER, "waterteamhealthbar"}
	};

	public void InitHeal(PZMonster goon)
	{
		/*
		if (goon != null)
		{
			portrait.removeButton.able = true;
			portrait.removeButton.icon.alpha = 1;
		}
		else
		{
			portrait.removeButton.able = false;
			portrait.removeButton.icon.alpha = 0;
		}
		*/

		Init (goon);
	}

	public void InitLab(PZMonster goon)
	{
		/*
		portrait.removeButton.able = false;
		portrait.removeButton.icon.alpha = 0;
		*/
		portrait.removeButton.gameObj.SetActive(false);

		Init (goon);

		bottomLabel.text = " ";
	}

	public void Init(PZMonster goon)
	{
		portrait.Init(goon, true);

		if (goon != null)
		{
			nameLabel.text = goon.monster.displayName;
			if (goon.isHealing)
			{
				bar.alpha = 0;
				barBG.alpha = 0;
				bottomLabel.text = HEALING_BOTTOM_LABEL;
				dottedBorder.alpha = 1;
				portrait.background.alpha = 1;
				portrait.background.color = tintColor;
				portrait.goonPortrait.color = Color.white;
			}
			else
			{
				bar.alpha = 1;
				barBG.alpha = 1;
				bar.spriteName = healthBarForElements[goon.monster.monsterElement];
				fillBar.fill = ((float)goon.currHP) / goon.maxHP;
				bottomLabel.text = " ";
				dottedBorder.alpha = 0;
				portrait.background.alpha = 1;
				portrait.background.color = Color.white;
				portrait.goonPortrait.color = Color.white;
			}
		}
		else
		{
			nameLabel.text = EMPTY_SLOT_NAME_LABEL;
			bottomLabel.text = EMPTY_SLOT_BOTTOM_LABEL;
			dottedBorder.alpha = 1;
			portrait.background.alpha = 0;
			bar.alpha = 0;
			barBG.alpha = 0;
		}
	}

}
