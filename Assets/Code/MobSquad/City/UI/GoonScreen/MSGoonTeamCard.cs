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
	UILabel emptyLabel;

	[SerializeField]
	UILabel bottomLabel;

	[SerializeField]
	Color tintColor;

	[SerializeField]
	UITweener[] addTweens;

	[SerializeField]
	UITweener[] removeTweens;

	bool firstTime = true;

	public PZMonster goon;

	const string EMPTY_SLOT_NAME_LABEL = "Team Slot Open";
	const string EMPTY_SLOT_BOTTOM_LABEL = "Tap [33ff33]+[-] to add";

	const string HEALING_BOTTOM_LABEL = "Slot Open";

	static readonly Dictionary<Element, string> healthBarForElements = new Dictionary<Element, string>()
	{
		{Element.DARK, "nightteamhealthbar"},
		{Element.FIRE, "fireteamhealthbar"},
		{Element.EARTH, "earthteamhealthbar"},
		{Element.LIGHT, "lightteamhealthbar"},
		{Element.WATER, "waterteamhealthbar"}
	};

	public void AddAnimation()
	{
		foreach (var item in addTweens) 
		{
			item.ResetToBeginning();
			item.PlayForward();
		}
	}

	public void RemoveAnimation()
	{
		foreach (var item in removeTweens) 
		{
			item.ResetToBeginning();
			item.PlayForward();
		}
	}

	public void InitHeal(PZMonster goon)
	{
		//Stupid little case that we don't want animations when the player is first
		//entering the game
		//Every other time, it should animate
		if (firstTime)
		{
			Init (goon, true);
			portrait.Init (goon);
			if (goon == null || goon.monster.monsterId == 0)
			{
				portrait.background.alpha = 0;
			}
			firstTime = false;
			return;
		}

		if (this.goon != goon)
		{
			if (goon == null)
			{
				RemoveAnimation();
			}
			else
			{
				AddAnimation();
			}
		}
		Init (goon, false);
	}

	public void InitLab(PZMonster goon)
	{
		/*
		portrait.removeButton.able = false;
		portrait.removeButton.icon.alpha = 0;
		*/
		portrait.removeButton.gameObj.SetActive(false);

		Init (goon, true);

		bottomLabel.text = " ";
	}

	public void Init(PZMonster goon, bool instant)
	{
		if (goon != null && goon.monster != null && goon.monster.monsterId > 0)
		{
			portrait.Init(goon, true);
			nameLabel.text = goon.monster.displayName;
			if (goon.isHealing)
			{
				nameLabel.text += "([ff0000]Healing[-])";
				bottomLabel.text = HEALING_BOTTOM_LABEL;
				portrait.background.alpha = .5f;
				portrait.goonPortrait.color = Color.white;

				if (instant)
				{
					bottomLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
					nameLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
					barBG.GetComponent<MSUIHelper>().ResetAlpha(false);
					emptyLabel.GetComponent<MSUIHelper>().ResetAlpha(false);
				}
				else
				{
					bottomLabel.GetComponent<MSUIHelper>().Fade(true);
					nameLabel.GetComponent<MSUIHelper>().Fade(true);
					barBG.GetComponent<MSUIHelper>().Fade(false);
					emptyLabel.GetComponent<MSUIHelper>().Fade(false);
				}
			}
			else
			{
				fillBar.fill = ((float)goon.currHP) / goon.maxHP;
				bottomLabel.text = " ";
				portrait.background.alpha = 1;
				portrait.goonPortrait.color = Color.white;

				if (instant)
				{
					bottomLabel.GetComponent<MSUIHelper>().ResetAlpha(false);
					nameLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
					barBG.GetComponent<MSUIHelper>().ResetAlpha(true);
					emptyLabel.GetComponent<MSUIHelper>().ResetAlpha(false);
				}
				else
				{
					bottomLabel.GetComponent<MSUIHelper>().Fade(false);
					nameLabel.GetComponent<MSUIHelper>().Fade(true);
					barBG.GetComponent<MSUIHelper>().Fade(true);
					emptyLabel.GetComponent<MSUIHelper>().Fade(false);
				}
			}
		}
		else
		{
			nameLabel.text = EMPTY_SLOT_NAME_LABEL;
			bottomLabel.text = EMPTY_SLOT_BOTTOM_LABEL;
			bar.alpha = 0;
			barBG.alpha = 0;

			if (instant)
			{
				bottomLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
				nameLabel.GetComponent<MSUIHelper>().ResetAlpha(false);
				barBG.GetComponent<MSUIHelper>().ResetAlpha(false);
				emptyLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
			}
			else
			{
				bottomLabel.GetComponent<MSUIHelper>().Fade(true);
				nameLabel.GetComponent<MSUIHelper>().Fade(false);
				barBG.GetComponent<MSUIHelper>().Fade(false);
				emptyLabel.GetComponent<MSUIHelper>().Fade(true);
			}
		}
		
		
		this.goon = goon;
	}

}
