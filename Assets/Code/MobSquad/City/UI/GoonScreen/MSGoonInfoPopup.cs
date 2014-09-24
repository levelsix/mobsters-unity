using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKGoonInfoPopup
/// </summary>
public class MSGoonInfoPopup : MonoBehaviour {

	[SerializeField]
	UI2DSprite mobsterSprite;

	[SerializeField]
	UILabel mobsterNamelabel;

	[SerializeField]
	UILabel healthLabel;

	[SerializeField]
	MSFillBar healthBar;

	[SerializeField]
	UISprite qualitySprite;

	[SerializeField]
	UISprite elementSprite;

	[SerializeField]
	UILabel elementLabel;

	[SerializeField]
	UILabel enhancementLabel;

	[SerializeField]
	UILabel attackLabel;

	[SerializeField]
	UILabel[] damageLabels;

	[SerializeField]
	UITweener infoTween;

	[SerializeField]
	MSUIHelper backButton;

	[SerializeField]
	UIButton heartButton;

	[SerializeField]
	UIButton restrictedButton;

	[SerializeField]
	MSUIHelper heartHelper;

	[SerializeField]
	MSUIHelper restrictedHelper;

	[SerializeField]
	GameObject noSkills;

	[SerializeField]
	GameObject hasSkills;

	[SerializeField]
	MSSkillInfo offensiveSkill;

	[SerializeField]
	MSSkillInfo defensiveSkill;

	PZMonster currMonster;

	const string RESTRICTED_SPRITENAME = "lockedactive";
	const string UNRESTRICTED_SPRITENAME = "lockedinactive";

	public void Init(PZMonster monster)
	{
		currMonster = monster;

		MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix, monster.monster.imagePrefix + "Character", mobsterSprite);

		mobsterNamelabel.text = monster.monster.displayName;

		healthLabel.text = monster.userMonster.currentHealth + "/" + monster.maxHP;

		healthBar.fill = ((float)monster.userMonster.currentHealth) / monster.maxHP;

		qualitySprite.spriteName = "battle" + monster.monster.quality.ToString().ToLower() + "tag";

		elementSprite.spriteName = monster.monster.monsterElement.ToString().ToLower() + "orb";

		elementLabel.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monster.monster.monsterElement.ToString());

		enhancementLabel.text = monster.userMonster.currentLvl.ToString();

		attackLabel.text = monster.totalDamage.ToString();

		for (int i = 0; i < damageLabels.Length; i++)
		{
			damageLabels[i].text = monster.attackDamages[i].ToString();
		}

		infoTween.Sample(0, false);

		heartHelper.TurnOn();
		heartHelper.ResetAlpha(true);

		restrictedHelper.TurnOn();
		restrictedHelper.ResetAlpha(true);

		backButton.TurnOff();
		backButton.ResetAlpha(false);

		SetHeartSprite();
		SetRestrictSprite();

		if (monster.offensiveSkill != null || monster.defensiveSkill != null)
		{
			hasSkills.SetActive(true);
			noSkills.SetActive(false);
			if (monster.offensiveSkill != null)
			{
				offensiveSkill.Init(monster.offensiveSkill.skillId, true);
			}
			else
			{
				offensiveSkill.Init(0, false);
			}
			if (monster.defensiveSkill != null)
			{
				defensiveSkill.Init(monster.defensiveSkill.skillId, monster.offensiveSkill == null);
			}
			else
			{
				defensiveSkill.Init(0, false);
			}
		}
		else
		{
			hasSkills.SetActive(false);
			noSkills.SetActive(true);
		}
	}

	public void SetMobsterAsAvatar()
	{
		if (currMonster.monster.monsterId != MSWhiteboard.localUser.avatarMonsterId)
		{
			MSPopupManager.instance.CreatePopup(
				"Set Avatar?",
				"Would you like to make " + currMonster.monster.displayName + " your avatar?",
				new string[] {"Cancel", "Yup!"},
				new string[] {"greymenuoption", "greenmenuoption"},
				new Action[] {
					MSActionManager.Popup.CloseTopPopupLayer, 
					delegate { 
						MSActionManager.Popup.CloseTopPopupLayer();
						MSMonsterManager.instance.SetMobsterAsAvatar(currMonster.monster.monsterId);
						SetHeartSprite();
					}
				}
			);
		}
	}

	public void FlipRestricted()
	{
		currMonster.restricted = !currMonster.restricted;
		SetRestrictSprite();
	}

	void SetHeartSprite()
	{
		heartButton.normalSprite = heartButton.GetComponent<UISprite>().spriteName = (currMonster.monster.monsterId == MSWhiteboard.localUser.avatarMonsterId) ?
			"avataractive" : "avatarinactive";
	}

	void SetRestrictSprite()
	{
		restrictedButton.normalSprite = restrictedButton.GetComponent<UISprite>().spriteName = currMonster.restricted ? RESTRICTED_SPRITENAME : UNRESTRICTED_SPRITENAME;
	}
}
