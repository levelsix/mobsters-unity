using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSGoonTeamCard : MonoBehaviour {

	[SerializeField]
	UISprite bar;

	[SerializeField]
	UISprite barBG;

	[SerializeField]
	UISprite dottedBorder;

	[SerializeField]
	MSFillBar fillBar;

	[SerializeField]
	UILabel barLabel;

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel emptyLabel;

	[SerializeField]
	UILabel bottomLabel;

	[SerializeField]
	MSUIHelper number;

	bool firstTime = true;

	public PZMonster goon;

	const string EMPTY_SLOT_BOTTOM_LABEL = "Tap to Add";

	const string HEALING_BOTTOM_LABEL = "Slot Open";

	static readonly Dictionary<Element, string> healthBarForElements = new Dictionary<Element, string>()
	{
		{Element.DARK, "nightteamhealthbar"},
		{Element.FIRE, "fireteamhealthbar"},
		{Element.EARTH, "earthteamhealthbar"},
		{Element.LIGHT, "lightteamhealthbar"},
		{Element.WATER, "waterteamhealthbar"}
	};

	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory += OnMonsterRemovedFromInventory;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory -= OnMonsterRemovedFromInventory;
	}

	public void Init(PZMonster goon)
	{
		Setup (goon, false);
	}

	public void InitLab(PZMonster goon)
	{
		Setup (goon, true);

		bottomLabel.text = " ";
	}

	void Setup(PZMonster goon, bool instant)
	{
		if (goon != null && goon.monster != null && goon.monster.monsterId > 0)
		{
			nameLabel.text = goon.monster.displayName;
			if (goon.isHealing)
			{
				nameLabel.text += "([ff0000]Healing[-])";
				bottomLabel.text = HEALING_BOTTOM_LABEL;

				if (instant)
				{
					bottomLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
					nameLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
					barBG.GetComponent<MSUIHelper>().ResetAlpha(false);
					emptyLabel.GetComponent<MSUIHelper>().ResetAlpha(false);
					number.ResetAlpha(false);
				}
				else
				{
					bottomLabel.GetComponent<MSUIHelper>().Fade(true);
					nameLabel.GetComponent<MSUIHelper>().Fade(true);
					barBG.GetComponent<MSUIHelper>().Fade(false);
					emptyLabel.GetComponent<MSUIHelper>().Fade(false);
					number.Fade(false);
				}
			}
			else
			{
				fillBar.fill = ((float)goon.currHP) / goon.maxHP;
				barLabel.text = goon.currHP + "/" + goon.maxHP;
				bottomLabel.text = " ";

				if (instant)
				{
					bottomLabel.GetComponent<MSUIHelper>().ResetAlpha(false);
					nameLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
					barBG.GetComponent<MSUIHelper>().ResetAlpha(true);
					emptyLabel.GetComponent<MSUIHelper>().ResetAlpha(false);
					number.ResetAlpha(false);
				}
				else
				{
					bottomLabel.GetComponent<MSUIHelper>().Fade(false);
					nameLabel.GetComponent<MSUIHelper>().Fade(true);
					barBG.GetComponent<MSUIHelper>().Fade(true);
					emptyLabel.GetComponent<MSUIHelper>().Fade(false);
					number.ResetAlpha(false);
				}
			}
		}
		else
		{
			bottomLabel.text = EMPTY_SLOT_BOTTOM_LABEL;
			bar.alpha = 0;
			barBG.alpha = 0;

			if (instant)
			{
				bottomLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
				nameLabel.GetComponent<MSUIHelper>().ResetAlpha(false);
				barBG.GetComponent<MSUIHelper>().ResetAlpha(false);
				emptyLabel.GetComponent<MSUIHelper>().ResetAlpha(true);
				number.ResetAlpha(true);
			}
			else
			{
				bottomLabel.GetComponent<MSUIHelper>().Fade(true);
				nameLabel.GetComponent<MSUIHelper>().Fade(false);
				barBG.GetComponent<MSUIHelper>().Fade(false);
				emptyLabel.GetComponent<MSUIHelper>().Fade(true);
				number.ResetAlpha(true);
			}
		}

		SetAsFirstEmpty(this == MSTeamScreen.instance.firstOpenCard);
		
		this.goon = goon;
	}

	/// <summary>
	/// Removes this mobster from the team
	/// Assigned in the editor to the remove button
	/// </summary>
	public void RemoveButton()
	{
		MSMonsterManager.instance.RemoveFromTeam(goon);
	}

	/// <summary>
	/// Only the first empty team card should have its bottom label visible.
	/// </summary>
	public void SetAsFirstEmpty(bool isFirst)
	{
		bottomLabel.GetComponent<MSUIHelper>().Fade(isFirst);
	}

	void OnMonsterRemovedFromInventory(long userMonsterId)
	{
		if (goon.userMonster.userMonsterId == userMonsterId)
		{
			Setup (null, true);
		}
	}

	void OnTeamChanged(PZMonster monster)
	{
		SetAsFirstEmpty(this == MSTeamScreen.instance.firstOpenCard);
	}

}
