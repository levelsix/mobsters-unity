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

	MSCityUnit userUnit;
	MSCityUnit guideUnit;
	MSCityUnit zarkUnit;
	MSCityUnit enemyOneUnit;
	MSCityUnit enemyTwoUnit;
	MSCityUnit bossUnit;

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
		StartCoroutine(PreCombat_MainTutorial());
	}

	void SetupTutorial()
	{
		userMobster = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.startingMonsterId);
		guide = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.guideMonsterId);
		zark = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.markZMonsterId);
		enemyOne = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.enemyMonsterId);
		enemyTwo = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.enemyMonsterIdTwo);
		enemyBoss = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.enemyBossMonsterId);
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
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, guide.imagePrefix + "Character", guide.displayName, TutorialStrings.THANKS_ZARK_DIALOGUE, true));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.KINDA_DYING_DIALOGUE, true));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix + "Character", zark.displayName, TutorialStrings.HEAD_TO_HOSPITAL_DIALOGUE, true));

		//Get to the hospital
		MSHospital hospital = MSHospitalManager.instance.hospitals[0];

		MSTownCamera.instance.DoCenterOnGroundPos(hospital.building.trans.position);

		currUi = hospital.building.gameObj;
		MSTutorialArrow.instance.Init(currUi.transform, 5, MSValues.Direction.NORTH);

		yield return StartCoroutine(WaitForClick());

		//Find the right TaskButton
		currUi = MSTaskBar.instance.taskButtons.Find(x => x.currMode == MSTaskButton.Mode.HEAL).gameObj;
		MSTutorialArrow.instance.Init(currUi.transform, 5, MSValues.Direction.EAST);
		yield return StartCoroutine(WaitForClick());

		/*
		//Trigger the facebookPopup
		Action<bool> facebookAction = delegate (bool didJoin) 
		{
			waitForFacebook = false;
			didJoinFacebook = didJoin;
		};
		MSActionManager.Tutorial.OnMakeFacebookDecision += facebookAction;
		while (waitForFacebook)
		{
			yield return null;
		}
		MSActionManager.Tutorial.OnMakeFacebookDecision -= facebookAction;
		*/

	}

	IEnumerator WaitForFacebook()
	{
		yield return null;
	}

	void OnMakeFacebookDecision(bool didJoin)
	{

	}

	IEnumerator AfterFacebookResponse()
	{
		yield return null;
		//Dialogue asking for name

		//Trigger username popup
	}

	void OnUsernameEnter()
	{
		StartCoroutine(SendOnFirstMission());
	}

	IEnumerator SendOnFirstMission()
	{
		yield return null;
		//Dialogue about birth cert

		//Let's recruit mobsters

		//Show attack button

		//UI: Attack button

		//UI: Enter button

		//End tutorial
	}

	#endregion

	#region Coroutines to do things

	IEnumerator DoDialogue(MSDialogueUI dialogueUI, 
	                       string imageName, 
	                       string mobsterName, 
	                       string dialogue,
	                       bool wait)
	{
		yield return StartCoroutine(dialogueUI.BringInMobster(imageName, mobsterName, dialogue));
		if (wait)
		{
			MSActionManager.UI.OnDialogueClicked += OnClick;
			yield return StartCoroutine(WaitForClick());
			MSActionManager.UI.OnDialogueClicked -= OnClick;
		}
	}

	IEnumerator DoUIStep(GameObject ui, float distance, MSValues.Direction direction)
	{
		currUi = ui;
		MSTutorialArrow.instance.Init(ui.transform, distance, direction);
		clicked = false;
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

	void OnClick()
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

	public GameObject shopButton;
	public GameObject attackButton;
	public GameObject enterButton;

	public MSDialogueUI leftDialogue;
	public MSDialogueUI rightDialogue;
	public MSDialogueUI puzzleDialogue;
}




