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
	UILabel descriptionLabel;

	[SerializeField]
	UILabel[] damageLabels;

	[SerializeField]
	UITweener infoTween;

	[SerializeField]
	MSUIHelper backButton;

	[SerializeField]
	UIButton heartButton;

	[SerializeField]
	MSUIHelper heartHelper;

	PZMonster currMonster;

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

		backButton.TurnOff();
		backButton.ResetAlpha(false);

		heartButton.normalSprite = heartButton.GetComponent<UISprite>().spriteName = (currMonster.monster.monsterId == MSWhiteboard.localUser.avatarMonsterId) ?
			"activeheart" : "inactiveheart";
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
					}
				}
			);
		}

	}
}
