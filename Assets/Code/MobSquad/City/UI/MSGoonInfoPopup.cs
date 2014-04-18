using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	UILabel headerLabel;

	[SerializeField]
	UILabel healthLabel;

	[SerializeField]
	MSFillBar healthBar;

	[SerializeField]
	UILabel qualityLabel;

	[SerializeField]
	UISprite elementSprite;

	[SerializeField]
	UILabel enhancementLabel;

	[SerializeField]
	UILabel attackLabel;

	[SerializeField]
	UILabel descriptionLabel;

	[SerializeField]
	UILabel sellLabel;

	[SerializeField]
	UILabel[] damageLabels;

	[SerializeField]
	UITweener infoTween;

	[SerializeField]
	GameObject backButton;

	PZMonster currMonster;

	public void Init(PZMonster monster)
	{
		currMonster = monster;

		mobsterSprite.sprite2D = MSAtlasUtil.instance.GetMobsterSprite(monster.monster.imagePrefix);

		if (mobsterSprite.sprite2D != null)
		{
			mobsterSprite.height = (int)mobsterSprite.sprite2D.textureRect.height;
			mobsterSprite.width = (int)mobsterSprite.sprite2D.textureRect.width;
		}

		headerLabel.text = monster.monster.displayName;

		healthLabel.text = monster.userMonster.currentHealth + "/" + monster.maxHP;

		healthBar.fill = ((float)monster.userMonster.currentHealth) / monster.maxHP;

		qualityLabel.text = monster.monster.quality.ToString();

		elementSprite.spriteName = monster.monster.monsterElement.ToString().ToLower() + "orb";

		enhancementLabel.text = "Lvl " + monster.userMonster.currentLvl;

		attackLabel.text = monster.totalDamage.ToString();

		sellLabel.text = "SELL\n$" + monster.sellValue.ToString();

		for (int i = 0; i < damageLabels.Length; i++)
		{
			damageLabels[i].text = ":  " + monster.attackDamages[i].ToString();
		}

		infoTween.ResetToBeginning();

		backButton.SetActive(false);
	}

	public void Sell()
	{
		MSMonsterManager.instance.SellMonster(currMonster);
		MSActionManager.Popup.CloseTopPopupLayer();
	}
}
