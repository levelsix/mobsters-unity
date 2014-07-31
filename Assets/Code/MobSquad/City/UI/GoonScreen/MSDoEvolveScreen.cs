using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// MSDoEvolveScreen
/// @author Rob Giusti
/// </summary>
public class MSDoEvolveScreen : MonoBehaviour 
{
	#region UI Elements

	[SerializeField]
	UI2DSprite firstCharacter;

	[SerializeField]
	UILabel firstCharacterName;

	[SerializeField]
	UI2DSprite secondCharacter;

	[SerializeField]
	UILabel secondCharacterName;

	[SerializeField]
	UI2DSprite scientistCharacter;

	[SerializeField]
	UILabel scientistName;

	[SerializeField]
	UI2DSprite finalProductCharacter;

	[SerializeField]
	UILabel finalProductName;

	[SerializeField]
	UILabel bigBottomLabel;

	[SerializeField]
	UILabel totalTime;

	[SerializeField]
	UILabel evolutionCost;

	[SerializeField]
	UIButton buttonSprite;

	[SerializeField]
	Color redColor;

	[SerializeField]
	Color greenColor;

	#endregion

	[SerializeField]
	float doesNotHaveAlpha = .5f;

	long scientistId;

	void Init(PZMonster monster, PZMonster buddy)
	{
		bool userMonsterLeveled = monster.userMonster.currentLvl >= monster.monster.maxLevel;
		bool hasBuddy = buddy != null;
		bool buddyLeveled = hasBuddy && buddy.userMonster.currentLvl >= monster.monster.maxLevel;
		scientistId = 0;
		foreach (var item in MSMonsterManager.instance.GetMonstersByMonsterId(monster.monster.evolutionCatalystMonsterId)) 
		{
			if (item != monster && item != buddy)
			{
				scientistId = item.monster.monsterId;
				break;
			}
		}
		bool hasScientist = scientistId > 0;
		bool isReady = userMonsterLeveled && buddyLeveled && hasScientist;

		MonsterProto scientist = MSDataManager.instance.Get<MonsterProto>(monster.monster.evolutionCatalystMonsterId);
		MonsterProto evoMonster = MSDataManager.instance.Get<MonsterProto>(monster.monster.evolutionMonsterId);

		secondCharacter.color = hasBuddy ? Color.white : Color.black;
		scientistCharacter.color = hasBuddy ? Color.white : Color.black;

		MSSpriteUtil.instance.SetSprite(monster.monster.displayName,
		                                monster.monster.displayName + "Character",
		                                firstCharacter,
		                                userMonsterLeveled ? 1f : doesNotHaveAlpha);
		MSSpriteUtil.instance.SetSprite(monster.monster.displayName,
		                                monster.monster.displayName + "Character",
		                                secondCharacter,
		                                buddyLeveled ? 1f : doesNotHaveAlpha);
		MSSpriteUtil.instance.SetSprite(scientist.displayName,
		                                scientist.displayName + "Character",
		                               	scientistCharacter,
		                                hasScientist ? 1f : doesNotHaveAlpha);
		MSSpriteUtil.instance.SetSprite(evoMonster.displayName,
		                                evoMonster.displayName + "Character",
		                                finalProductCharacter,
		                                isReady ? 1f : doesNotHaveAlpha);

		buttonSprite.enabled = isReady;

		if (isReady)
		{
			bigBottomLabel.text = MSColors.nguiColorHexString(greenColor) + "You have all the pieces to create "
				+ evoMonster.shorterName + "\n" + monster.monster.shorterName + " L" + monster.monster.maxLevel
				+ ", another " + monster.monster.shorterName + " L" + monster.monster.maxLevel + ", and a "
					+ scientist.shorterName + "(Evo " + scientist.evolutionLevel + ").[-]";
		}
		else
		{

		}
	}

	public void Back()
	{

	}

	public void Start()
	{

	}
}
