using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System.Globalization;

public class MSProfileMobster : MonoBehaviour {

	#region UI Elements

	[SerializeField]
	GameObject hasMonsterElements;

	[SerializeField]
	GameObject noMonsterElements;

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel levelLabel;

	[SerializeField]
	UILabel hpLabel;

	[SerializeField]
	UISprite elementSprite;

	[SerializeField]
	UILabel elementLabel;

	[SerializeField]
	UILabel speedLabel;

	[SerializeField]
	UILabel attackLabel;

	[SerializeField]
	UISprite qualitySprite;

	[SerializeField]
	UI2DSprite mobster;

	#endregion

	public void Init(FullUserMonsterProto userMonster)
	{
		if (userMonster != null && userMonster.monsterId > 0)
		{
			PZMonster monster = new PZMonster(userMonster);
			Init (monster);
		}
		else
		{
			InitEmpty();
		}
	}

	public void Init(PZMonster monster)
	{
		if (monster != null)
		{
			hasMonsterElements.SetActive(true);
			noMonsterElements.SetActive(false);
			nameLabel.text = monster.monster.displayName;
			levelLabel.text = "LEVEL " + monster.userMonster.currentLvl;
			hpLabel.text = monster.MaxHPAtLevel(monster.userMonster.currentLvl).ToString();
			qualitySprite.spriteName = "battle" + monster.monster.quality.ToString().ToLower() + "tag";
			qualitySprite.MakePixelPerfect();
			elementSprite.spriteName = monster.monster.monsterElement.ToString().ToLower() + "orb";
			elementSprite.MakePixelPerfect();
			elementLabel.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monster.monster.monsterElement.ToString());
			speedLabel.text = monster.SpeedAtLevel(monster.userMonster.currentLvl).ToString();
			attackLabel.text = monster.TotalAttackAtLevel(monster.userMonster.currentLvl).ToString();

			MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix, monster.monster.imagePrefix + "Character", mobster); 
		}
		else
		{
			InitEmpty();
		}
	}

	void InitEmpty()
	{	
		hasMonsterElements.SetActive(false);
		noMonsterElements.SetActive(true);
	}
}
