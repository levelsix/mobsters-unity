using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public static class TutorialStrings
{
	
	
	
	#region Post-Combat Dialogue
	public const string THANKS_ZARK_DIALOGUE = "Whew! That was a close one. Thanks for the help Zark!";
	public const string KINDA_DYING_DIALOGUE = "No problem buddy, but in case you didn't notice, your nephew is kinda... dying.";
	public const string HEAD_TO_HOSPITAL_DIALOGUE = "Let's head to the Hospital and get him healed right up. Follow the magical floating arrows to begin.";
	
	public const string TAP_CARD_DIALOGUE = "Tap on Swaggy Steve to insert him into the healing queue.";
	
	public const string CHEST_OF_CASH_DIALOGUE = "If you're going to fight Lil' Kim, you're going to need a war chest of cash.";
	
	public const string BUILD_CASH_PRINTER_DIALOGUE = "What better way to make money than to print it? Build a Cash Printer now!";
	public const string AFTER_CASH_PRINTER_DIALOGUE = "Nice job! The printer can only store a small amount of cash, so we'll need a Vault to stash the rest of it.";
	
	public const string BUILD_CASH_VAULT_DIALOGUE = "Amazon doesn’t ship to secret islands, so let’s construct one ourselves now.";
	public const string AFTER_CASH_VAULT_DIALOGUE = "Good work! The Vault will protect your money from being stolen, so remember to upgrade it!";
	
	public const string BEFORE_OIL_SILO_DIALOGUE = "Another important resource is Oil, which is used to upgrade your Mobsters and building.";
	public const string BUILD_OIL_SILO_DIALOGUE = "We'll need a place to store the oil you drill, so construct an Oil Silo now!";
	public const string AFTER_OIL_SILO_DIALOGUE = "Great job! This silo will now protect your oil from being stolen in battle.";
	
	public const string ISLAND_BASE_DIALOGUE = "Your island is starting to look like a real secret base! There's just one last thing...)";
	public const string THIS_IS_CRAZY_DIALOGUE = "I just met you, and this is crazy, but here's my friend request, so add me maybe?";
	
	public const string FACEBOOK_DID_JOIN_DIALOGUE = "Hurray! I know that we're besties now, but what was your name again?";
	public const string FACEBOOK_NOT_JOIN_DIALOGUE = "Playing hard to get huh? I can play that game too. What was your name again?";
	
	public const string BIRTH_CERTIFICATE_DIALOGUE = "Is that really on your birth certificate? Seems legit I guess.";
	public const string GO_RECRUIT_DIALOGUE = "Yippee! Now let's go recruit some Mobsters to join your team.";
	#endregion
}

/// <summary>
/// @author Rob Giusti
/// MSTutorialManager
/// </summary>
public class MSTutorialManager : MonoBehaviour 
{
	public static MSTutorialManager instance;

	public bool inTutorial = false;

	bool clicked = true;

	bool waitingForTurn = true;
	
	bool waitForFacebook = true;

	bool didJoinFacebook = true;

	bool waitingForUsername = true;

	public bool UiBlock
	{
		get
		{
			return currUi != null;
		}
	}

	public GameObject currUi = null;

	public TutorialUI TutorialUI;

	public bool holdUpEndingCombat = false;

	MonsterProto userMobster;
	MonsterProto guide;
	MonsterProto zark;
	MonsterProto enemyOne;
	MonsterProto enemyTwo;
	MonsterProto enemyBoss;

	MSUnit userUnit;
	MSUnit guideUnit;
	MSUnit zarkUnit;
	MSUnit enemyOneUnit;
	MSUnit enemyTwoUnit;
	MSUnit bossUnit;

	PZCombatUnit userCombatant;
	PZCombatUnit enemyOneCombatant;
	PZCombatUnit enemyTwoCombatant;
	PZCombatUnit bossCombatant;

	void Awake()
	{
		instance = this;
	}

	[ContextMenu ("Click Dialogue")]
	public void SkipDialogue()
	{
		MSActionManager.UI.OnDialogueClicked();
	}

	[ContextMenu ("Start First Tutorial")]
	public void StartBeginningTutorial()
	{
		SetupTutorial();
		StartCoroutine(PostCombat_SendOnFirstMission());
	}

	void SetupTutorial()
	{
		
		userMobster = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.startingMonsterId);
		guide = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.guideMonsterId);
		zark = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.markZMonsterId);
		enemyOne = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.enemyMonsterId);
		enemyTwo = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.enemyMonsterIdTwo);
		enemyBoss = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.enemyBossMonsterId);


		inTutorial = true;

		/*
		userMobster = MSDataManager.instance.Get<MonsterProto>(2011);
		guide = MSDataManager.instance.Get<MonsterProto>(4);
		zark = MSDataManager.instance.Get<MonsterProto>(16);
		enemyOne = MSDataManager.instance.Get<MonsterProto>(2010);
		enemyTwo = MSDataManager.instance.Get<MonsterProto>(19);
		enemyBoss = MSDataManager.instance.Get<MonsterProto>(31);
		*/

		userUnit = MSBuildingManager.instance.MakeTutorialUnit(userMobster.monsterId, Vector2.zero, 0);
		guideUnit = MSBuildingManager.instance.MakeTutorialUnit(guide.monsterId, Vector2.zero, 1);
		zarkUnit = MSBuildingManager.instance.MakeTutorialUnit(zark.monsterId, Vector2.zero, 2);
	}

	void CleanUpUnits()
	{
		//TODO: Pool out all of the Units that we made
		//TODO: Give the player their normal unit, and Zark if necessary
	}

	IEnumerator PreCombat_MainTutorial()
	{
		//Debug, until the full tutorial works, skip to later in it
		StartCoroutine(PostCombat_MainTutorial());

		yield return null;
	}

	#region Post Combat Tutorial

	IEnumerator PostCombat_MainTutorial()
	{
		//yield return StartCoroutine(PostCombat_DialogueAndRunaway());
		//yield return StartCoroutine(PostCombat_HealMobsterTutorial());
		//yield return StartCoroutine(PostCombat_BuildBuildings());
		yield return StartCoroutine(PostCombat_FacebookLogon());
		yield return StartCoroutine(PostCombat_Username());
		yield return StartCoroutine(PostCombat_SendOnFirstMission());
	}

	IEnumerator PostCombat_DialogueAndRunaway()
	{
		yield return null;
	}

	IEnumerator PostCombat_HealMobsterTutorial()
	{
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, guide.imagePrefix, guide.imagePrefix + "Character", guide.displayName, TutorialStrings.THANKS_ZARK_DIALOGUE, true));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.KINDA_DYING_DIALOGUE, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.HEAD_TO_HOSPITAL_DIALOGUE, true));

		//Get to the hospital
		MSHospital hospital = MSHospitalManager.instance.hospitals[0];

		MSTownCamera.instance.DoCenterOnGroundPos(hospital.building.trans.position);

		currUi = hospital.building.gameObj;
		MSTutorialArrow.instance.Init(hospital.building.sprite.transform, 180, MSValues.Direction.NORTH, .02f);
		yield return StartCoroutine(WaitForClick());
		
		//Heal taskbutton
		yield return StartCoroutine(DoUIStep(
			MSTaskBar.instance.taskButtons.Find(x => x.currMode == MSTaskButton.Mode.HEAL).gameObj,
			150, MSValues.Direction.EAST));

		//Healing dialogue
		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.TAP_CARD_DIALOGUE, false));

		//Find the Goon to heal
		yield return StartCoroutine(DoUIStep(MSHealScreen.instance.grid.cards[0].gameObject, 125, MSValues.Direction.EAST));

		//Finish the heal
		yield return StartCoroutine(DoUIStep(TutorialUI.finishHealButton, 105, MSValues.Direction.NORTH));

		//Close the menu
		yield return StartCoroutine(DoUIStep(TutorialUI.closeHealMenuButton, 50, MSValues.Direction.WEST));

	}

	IEnumerator PostCombat_BuildBuildings()
	{
		//Chest of cash dialogue
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.CHEST_OF_CASH_DIALOGUE, true, false));

		//Build cash printer dialogue
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.BUILD_CASH_PRINTER_DIALOGUE, true));

		yield return StartCoroutine(BuildStructure(StructureInfoProto.StructType.RESOURCE_GENERATOR, ResourceType.OIL));

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.AFTER_CASH_PRINTER_DIALOGUE, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.BUILD_CASH_VAULT_DIALOGUE, true));

		//Build Vault
		yield return StartCoroutine(BuildStructure(StructureInfoProto.StructType.RESOURCE_STORAGE, ResourceType.OIL));

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.AFTER_CASH_VAULT_DIALOGUE, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.BEFORE_OIL_SILO_DIALOGUE, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.BUILD_OIL_SILO_DIALOGUE, true));
		
		//Build Silo
		yield return StartCoroutine(BuildStructure(StructureInfoProto.StructType.RESOURCE_STORAGE, ResourceType.CASH));

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.AFTER_OIL_SILO_DIALOGUE, true, false));

	}

	IEnumerator PostCombat_FacebookLogon()
	{
		MSTownCamera.instance.DoCenterOnGroundPos(zarkUnit.transf.position, .5f);

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.ISLAND_BASE_DIALOGUE, true, false));

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.THIS_IS_CRAZY_DIALOGUE, true));

		MSActionManager.Popup.OnPopup(TutorialUI.facebookPopup);
		waitForFacebook = true;
		while (waitForFacebook)
		{
			yield return null;
		}
		if (didJoinFacebook)
		{
			yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.FACEBOOK_DID_JOIN_DIALOGUE, true));
		}
		else
		{
			yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.FACEBOOK_NOT_JOIN_DIALOGUE, true));
		}
	}
	
	public void OnMakeFacebookDecision(bool didJoin)
	{
		waitForFacebook = false;
		didJoinFacebook = didJoin;
	}

	IEnumerator PostCombat_Username()
	{
		waitingForUsername = true;
		MSActionManager.Popup.OnPopup(TutorialUI.usernamePopup);
		while (waitingForUsername)
		{
			yield return null;
		}

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.BIRTH_CERTIFICATE_DIALOGUE, true, false));
	}

	public void OnUsernameEnter()
	{
		waitingForUsername = false;
	}



	IEnumerator PostCombat_SendOnFirstMission()
	{
		yield return null;
		//Dialogue about birth cert

		//Attack dialogue
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.GO_RECRUIT_DIALOGUE, true));
		
		//Attack button
		currUi = TutorialUI.attackButton;
		MSTutorialArrow.instance.Init(currUi.transform, 150, MSValues.Direction.NORTH);
		yield return StartCoroutine(WaitForClick());
		
		CleanUpUnits();
		
		//Enter button
		currUi = TutorialUI.enterButton;
		MSTutorialArrow.instance.Init(currUi.transform, 150, MSValues.Direction.NORTH);
		yield return StartCoroutine(WaitForClick());

		inTutorial = false;
	}

	#endregion

	#region Coroutines to do things

	IEnumerator DoDialogue(MSDialogueUI dialogueUI,
	                       string bundleName,
	                       string imageName,
	                       string mobsterName, 
	                       string dialogue,
	                       bool wait,
	                       bool fullExit = true)
	{
		yield return dialogueUI.RunDialogue(bundleName, imageName, mobsterName, dialogue, wait);
		if (wait)
		{
			MSActionManager.UI.OnDialogueClicked += OnClick;
			yield return StartCoroutine(WaitForClick());
			MSActionManager.UI.OnDialogueClicked -= OnClick;
			if (fullExit)
			{
				yield return dialogueUI.RunPushOut();
			}
			else
			{
				yield return dialogueUI.RunDialogueOut();
			}
			dialogueUI.clickbox.SetActive(false);
		}
	}

	IEnumerator DoUIStep(GameObject ui, float distance, MSValues.Direction direction)
	{
		currUi = ui;
		MSTutorialArrow.instance.Init(ui.transform, distance, direction);
		yield return StartCoroutine(WaitForClick());
		currUi = null;
		MSTutorialArrow.instance.gameObject.SetActive(false);
	}

	IEnumerator WaitForClick()
	{
		clicked = false;
		Debug.Log("Not clicked");
		while (!clicked)
		{
			yield return null;
		}
		Debug.Log("Clicked");
		currUi = null;
	}

	IEnumerator WaitForTurn()
	{
		waitingForTurn = true;
		MSActionManager.Puzzle.OnTurnChange += TurnHappen;
		while (waitingForTurn)
		{
			yield return null;
		}
		MSActionManager.Puzzle.OnTurnChange -= TurnHappen;
	}

	void TurnHappen(int turn)
	{
		waitingForTurn = false;
	}

	IEnumerator BuildStructure(StructureInfoProto.StructType structType, ResourceType buildResource)
	{
		//Shop
		yield return StartCoroutine(DoUIStep(TutorialUI.shopButton, 150, MSValues.Direction.NORTH));
		
		//Cash printer card
		yield return StartCoroutine(DoUIStep(
			MSBuildingMenu.instance.cards.Find(x=>x.building.structInfo.structType == structType && x.building.structInfo.buildResourceType == buildResource).gameObject,
			160, MSValues.Direction.EAST));
		
		//Confirm
		currUi = MSBuildingManager.instance.hoveringToBuild.confirmationButtons.transform.GetChild(0).gameObject;
		yield return StartCoroutine(WaitForClick());
		
		//There's a short bit of time where we're waiting for the
		//building confirmation to happen. When it ends, hoveringToBuild gets
		//set to null, so we wait on that.
		while (MSBuildingManager.instance.hoveringToBuild != null)
		{
			yield return null;
		}
		
		//Finish
		yield return StartCoroutine(DoUIStep(
			MSTaskBar.instance.taskButtons.Find(x => x.currMode == MSTaskButton.Mode.FINISH).gameObj,
			150, MSValues.Direction.EAST));
	}

	public void OnClick()
	{
		clicked = true;
	}

	#endregion
}

[Serializable]
public class TutorialUI
{
	public UIWidget dialogueClickbox;

	public UISprite arrow;
	
	public GameObject finishHealButton;
	public GameObject closeHealMenuButton;

	public GameObject shopButton;

	public MSPopup facebookPopup;

	public MSPopup usernamePopup;

	public GameObject attackButton;
	public GameObject enterButton;

	public MSDialogueUI leftDialogue;
	public MSDialogueUI rightDialogue;
	public MSDialogueUI puzzleDialogue;
}




