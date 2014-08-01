using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// MSDoEvolveScreen
/// @author Rob Giusti
/// </summary>
public class MSDoEvolveScreen : MSFunctionalScreen 
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

	string redString
	{
		get
		{
			return MSColors.nguiColorHexString(redColor);
		}
	}
	
	string greenString
	{
		get
		{
			return MSColors.nguiColorHexString(greenColor);
		}
	}

	PZMonster monster;
	PZMonster buddy;
	PZMonster sci;

	long scientistId;

	public override bool IsAvailable ()
	{
		return MSEvolutionManager.instance.hasEvolution;
	}

	void OnDisable()
	{
		if (!MSEvolutionManager.instance.isEvolving)
		{
			MSEvolutionManager.instance.currEvolution = null;
		}
	}

	public override void Init()
	{
		monster = MSEvolutionManager.instance.evoMonster;
		buddy = MSEvolutionManager.instance.buddy;

		bool userMonsterLeveled = monster.userMonster.currentLvl >= monster.monster.maxLevel;
		bool hasBuddy = buddy != null;
		bool buddyLeveled = hasBuddy && buddy.userMonster.currentLvl >= monster.monster.maxLevel;
		sci = null;
		foreach (var item in MSMonsterManager.instance.GetMonstersByMonsterId(monster.monster.evolutionCatalystMonsterId)) 
		{
			if (item != monster && item != buddy)
			{
				sci = item;
				break;
			}
		}
		bool hasScientist = sci != null && sci.monster.monsterId > 0;
		bool isReady = userMonsterLeveled && buddyLeveled && hasScientist;

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
		MSSpriteUtil.instance.SetSprite(sci.monster.displayName,
		                                sci.monster.displayName + "Character",
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
					+ sci.monster.shorterName + "(Evo " + sci.monster.evolutionLevel + ").[-]";
		}
		else
		{
			string firstCharacterColorString = userMonsterLeveled ? greenString : redString;
			string buddyColorString = buddyLeveled ? greenString : redString;
			string sciColorString = hasScientist ? greenString : redString;
			bigBottomLabel.text = "[000000]To create " + evoMonster.shorterName + ", you need to combine a "
				+ firstCharacterColorString + monster.monster.shorterName + " L" + monster.monster.maxLevel + "[-], another " + buddyColorString
				+ monster.monster.shorterName + " L" + monster.monster.maxLevel + "[-], and a " + sciColorString +
				sci.monster.shorterName + " (Evo " + sci.monster.evolutionLevel + ")[-].";
		}
	}
	
	public void Back()
	{
		MSPopupManager.instance.popups.goonScreen.DoShiftLeft(false);
	}

	public void Button()
	{
		if (MSEvolutionManager.instance.hasEvolution)
		{
			MSEvolutionManager.instance.FinishWithGems();
		}
		else
		{
			MSEvolutionManager.instance.StartEvolution();
		}
	}
}
