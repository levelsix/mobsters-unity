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

	[SerializeField]
	Color evolveDisabledTextColor;

	[SerializeField]
	Color evolveEnabledTextColor;

	[SerializeField]
	Color finishTextColor;

	[SerializeField]
	MSLoadLock loadLock;

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

	void OnEnable()
	{
		MSActionManager.Goon.OnEvolutionComplete += OnEvoComplete;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnEvolutionComplete -= OnEvoComplete;
		MSEvolutionManager.instance.tempEvolution = null;
	}

	public override void Init()
	{
		monster = MSEvolutionManager.instance.tempEvoMonster;
		buddy = MSEvolutionManager.instance.tempBuddy;

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

		MonsterProto scientist = MSDataManager.instance.Get<MonsterProto>(monster.monster.evolutionCatalystMonsterId);
		MonsterProto evoMonster = MSDataManager.instance.Get<MonsterProto>(monster.monster.evolutionMonsterId);

		secondCharacter.color = hasBuddy ? Color.white : Color.black;
		scientistCharacter.color = hasBuddy ? Color.white : Color.black;

		MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix,
		                                monster.monster.imagePrefix + "Character",
		                                firstCharacter,
		                                userMonsterLeveled ? 1f : doesNotHaveAlpha);
		firstCharacterName.text = "[000000]" + monster.monster.shorterName + " [0000ff]L" + monster.monster.maxLevel;
		MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix,
		                                monster.monster.imagePrefix + "Character",
		                                secondCharacter,
		                                buddyLeveled ? 1f : doesNotHaveAlpha);
		secondCharacterName.text = "[000000]" + monster.monster.shorterName + " [0000ff]L" + monster.monster.maxLevel;
		MSSpriteUtil.instance.SetSprite(scientist.imagePrefix,
		                                scientist.imagePrefix + "Character",
		                               	scientistCharacter,
		                                hasScientist ? 1f : doesNotHaveAlpha);
		scientistName.text = "[000000]" + scientist.shorterName + " (Evo " + scientist.evolutionLevel + ")";
		MSSpriteUtil.instance.SetSprite(evoMonster.imagePrefix,
		                                evoMonster.imagePrefix + "Character",
		                                finalProductCharacter,
		                                isReady ? 1f : doesNotHaveAlpha);
		finalProductName.text = "[000000]" + evoMonster.shorterName;

		buttonSprite.isEnabled = isReady;

		if (isReady)
		{
			bigBottomLabel.text = MSColors.nguiColorHexString(greenColor) + "You have all the pieces to create "
				+ evoMonster.shorterName + "\n" + monster.monster.shorterName + " L" + monster.monster.maxLevel
				+ ", another " + monster.monster.shorterName + " L" + monster.monster.maxLevel + ", and a "
					+ scientist.displayName + "(Evo " + scientist.evolutionLevel + ").[-]";
		}
		else
		{
			string firstCharacterColorString = userMonsterLeveled ? greenString : redString;
			string buddyColorString = buddyLeveled ? greenString : redString;
			string sciColorString = hasScientist ? greenString : redString;
			bigBottomLabel.text = "[000000]To create " + evoMonster.shorterName + ", you need to combine a "
				+ firstCharacterColorString + monster.monster.shorterName + " L" + monster.monster.maxLevel + "[-], another " + buddyColorString
				+ monster.monster.shorterName + " L" + monster.monster.maxLevel + "[-], and a " + sciColorString +
					scientist.displayName + " (Evo " + scientist.evolutionLevel + ")[-].";
		}

		totalTime.text = MSUtil.TimeStringMed(evoMonster.minutesToEvolve*60000);
		evolutionCost.text = "Evolve\n(o) " + evoMonster.evolutionCost;
	}

	void Update()
	{
		if (MSEvolutionManager.instance.isEvolving)
		{
			totalTime.text = MSUtil.TimeStringMed(MSEvolutionManager.instance.timeLeftMillis);
			buttonSprite.normalSprite = "purplemenuoption";
			evolutionCost.text = "Finish\n(g) " + MSEvolutionManager.instance.gemsToFinish;
		}
	}
	
	public void Back()
	{
		MSEvolutionManager.instance.tempEvolution = null;
		MSPopupManager.instance.popups.goonScreen.DoShiftLeft(false);
	}

	public void Button()
	{
//		Debug.Log("Button");
		if (MSEvolutionManager.instance.isEvolving)
		{
			MSEvolutionManager.instance.FinishWithGems(loadLock);
			//MSPopupManager.instance.popups.goonScreen.DoShiftLeft(false);
		}
		else
		{
			MSEvolutionManager.instance.StartEvolution(loadLock);
		}
	}

	void OnEvoComplete(PZMonster monster)
	{
		MSPopupManager.instance.popups.goonScreen.DoShiftLeft(false);
	}
}
