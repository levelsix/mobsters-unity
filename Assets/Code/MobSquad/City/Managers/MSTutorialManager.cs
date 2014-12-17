using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public static class TutorialStrings
{
	#region Pre-Combat Dialogue
	public const string HEY_BOSS = "Hey boss! We've been expecting you!";
	public const string EVIL_DICTATOR = "An evil dictator named Lil' Kim has taken over the world and it's up to you to stop him!";
	public const string HOPEFULLY_DONT_FIND = "Hopefully they don't find...";

	public const string PEASANT_SQUAD = "Well well well... you peasants think you can start a new squad, under my watch?";
	public const string EGGSPECT = "Heh, did you really EGG-spect us not to find you here?";

	public const string SEND_NEPHEW = "Oh no. It's Lil' Kim! Send my nephew into battle, Boss! I don't like him anyways.";

	public const string YOLO = "Yolo.";
	#endregion

	#region Combat Dialogue
	public const string WANTS_A_PIECE = "Who wants a piece of Swaggy?";

	public const string AINT_CHICKEN = "Lemme at 'em boss, I ain't chicken.";
	public const string DOT_DOT_DOT = "......";
	public const string MAKE_ME_FRY = "OW! Can't take a joke chicken? Don't make me fry...";
	public const string GO_PEPE = "Enough you two! Take care of this degenerate, Pete!";

	public const string MOVIN_ORBS = "Yo dawg, movin' orbs ain't my style. Help a brotha out.";
	public const string SMOOTH_MOVE = "Smooth move homie! The more orbs you break, the stronger I get.";
	public const string LAST_MOVE = "Dope! You got one last move before I make it rain. You got this!";

	public const string CHICKENS_WORK = "Sigh, never leave a man to do a chicken's work. Take him out Drumstix.";

	public const string CREATE_POWERUP = "Yo, this chicken is savage. Create a power-up by matching 4 orbs.";
	public const string SWIPE_POWERUP = "Swipe the striped orb down to activate the power-up.";
	public const string BALLIN = "BALLIN'! You got one last move homie.";

	public const string NOLO = "Yolo... ain't... the... motto...";
	public const string POKE = "*Poke*";
	public const string HEY_BUDDY = "Hey buddy, you don't look so good. Would you \"Like\" me to help you out?";

	public const string UPDATE_BOOKFACE = "Oops, let me update my BookFace status before we begin.";
	public const string ZARKS_STATUS = "\"Currently saving a stranger who got owned by a chicken. #LOL #GoodGuyZark\"";
	public const string TWELVE_LIKES = "Heh, 12 likes already. Alright, let's do this.";
	#endregion
	
	#region Post-Combat Dialogue
	public const string BEAT_A_CHICKEN = "Ohhh, bet you feel like a big man beating a chicken. This isn’t over, I’ll be back!";

	public const string THANKS_ZARK_DIALOGUE = "Whew! That was a close one. Thanks for the help Zark!";
	public const string KINDA_DYING_DIALOGUE = "No problem buddy, but in case you didn't notice, your nephew is kinda... dying.";
	public const string HEAD_TO_HOSPITAL_DIALOGUE = "Let's head to the Hospital and get him healed right up. Follow the magical floating arrows to begin.";
	
	public const string TAP_CARD_DIALOGUE = "Tap on Swaggy Steve to insert him into the healing queue.";
	
	public const string CHEST_OF_CASH_DIALOGUE = "If you're going to fight Lil' Kim and his men, you're going to need a war chest of cash.";
	
	public const string BUILD_CASH_PRINTER_DIALOGUE = "What better way to make money than to print it? Build a Cash Printer now!";
	public const string AFTER_CASH_PRINTER_DIALOGUE = "Nice job! The printer can only store a small amount of cash, so we'll need a Vault to stash the rest of it.";
	
	public const string BUILD_CASH_VAULT_DIALOGUE = "Amazon doesn’t ship to secret islands, so let’s construct one ourselves now.";
	public const string AFTER_CASH_VAULT_DIALOGUE = "Good work! The Vault will protect your money from being stolen, so remember to upgrade it!";
	
	public const string BEFORE_OIL_SILO_DIALOGUE = "Another important resource is Oil, which is used to upgrade your Toons and buildings.";
	public const string BUILD_OIL_SILO_DIALOGUE = "We'll need a place to store the oil you drill, so construct an Oil Silo now!";
	public const string AFTER_OIL_SILO_DIALOGUE = "Great job! This silo will now protect your oil from being stolen in battle.";
	
	public const string ISLAND_BASE_DIALOGUE = "Your island is starting to look like a real secret base! There's just one last thing...)";
	public const string THIS_IS_CRAZY_DIALOGUE = "I just met you, and this is crazy, but here's my friend request, so add me maybe?";
	
	public const string FACEBOOK_DID_JOIN_DIALOGUE = "Hurray! I know that we're besties now, but what was your name again?";
	public const string FACEBOOK_NOT_JOIN_DIALOGUE = "Playing hard to get huh? I can play that game too. What was your name again?";
	
	public const string BIRTH_CERTIFICATE_DIALOGUE = "Is that really on your birth certificate? Seems legit I guess.";
	public const string GO_RECRUIT_DIALOGUE = "Yippee! Now let's go recruit some Toons to join your team.";
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

	/// <summary>
	/// If the player logs onto facebook, and we find another
	/// account, we short-circuit the tutorial
	/// </summary>
	bool facebookHadAccount = false;

	bool waitingForUsername = true;

	/// <summary>
	/// The hijack flinch flag.
	/// Since we need to have Drumstix attack
	/// Pistol Pepe, we're creating a hook for Flinch
	/// to check against, and if we need to hijack that flinch
	/// it'll redirect back here
	/// </summary>
	public bool hijackFlinch = false;

	public bool firstCombat = true;

	public bool UiBlock
	{
		get
		{
			return currUi != null;
		}
	}

	public GameObject currUi = null;

	public TutorialUI TutorialUI;

	public TutorialValues TutorialValues;

	[SerializeField] Vector3 startCameraPos;

	[SerializeField] Vector3 pierCameraPos;

	[SerializeField] Vector3 showdownCameraPos;

	Vector3 combatPosition;

	Vector3 userUnitReturnPosition;

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

	#region Unit Paths
	static readonly List<MSGridNode> enemyOneEnterPath = new List<MSGridNode>()
	{
		new MSGridNode(16, .5f, MSValues.Direction.NORTH),
		new MSGridNode(14.5f, .5f, MSValues.Direction.WEST)
	};

	static readonly List<MSGridNode> enemyTwoEnterPath = new List<MSGridNode>()
	{
		new MSGridNode(16, .5f, MSValues.Direction.NORTH),
		new MSGridNode(17.5f, .5f, MSValues.Direction.EAST)
	};

	static readonly List<MSGridNode> enemyBossEnterPath = new List<MSGridNode>()
	{
		new MSGridNode(16, 2, MSValues.Direction.NORTH)
	};

	static readonly List<MSGridNode> guideRetreatPath  = new List<MSGridNode>()
	{
		new MSGridNode(19, 5, MSValues.Direction.EAST),
		new MSGridNode(19, 7, MSValues.Direction.NORTH),
		new MSGridNode(22, 7, MSValues.Direction.EAST),
		new MSGridNode(22, 5, MSValues.Direction.SOUTH)
	};

	static readonly List<MSGridNode> swaggyEnterPath = new List<MSGridNode>()
	{
		new MSGridNode(13, 10, MSValues.Direction.EAST),
		new MSGridNode(13, 7, MSValues.Direction.SOUTH),
		new MSGridNode(16, 7, MSValues.Direction.EAST),
		new MSGridNode(16, 5, MSValues.Direction.SOUTH)
	};

	static readonly List<MSGridNode> enemyBossRetreatPath = new List<MSGridNode>()
	{
		new MSGridNode(16, -9, MSValues.Direction.SOUTH)
	};
	
	static readonly List<MSGridNode> enemyOneRetreatPath = new List<MSGridNode>()
	{
		new MSGridNode(16, 1, MSValues.Direction.EAST),
		new MSGridNode(16, -9, MSValues.Direction.SOUTH)
	};
	
	static readonly List<MSGridNode> enemyTwoRetreatPath = new List<MSGridNode>()
	{
		new MSGridNode(16, 1, MSValues.Direction.WEST),
		new MSGridNode(16, -9, MSValues.Direction.SOUTH)
	};

	static readonly List<MSGridNode> guideReturnPath  = new List<MSGridNode>()
	{
		new MSGridNode(22, 7, MSValues.Direction.NORTH),
		new MSGridNode(16, 7, MSValues.Direction.WEST)
	};

	#endregion

	#region Puzzle Spaces
	static readonly List<Vector2> turn1move1 = new List<Vector2>()
	{
		new Vector2(2, 2), new Vector2(3, 1), new Vector2(3, 2), new Vector2(3, 3)
	};

	static readonly Vector2 turn1move1hintStart = new Vector2(2, 2);
	static readonly Vector2 turn1move1hintEnd = new Vector2(3, 2);

	static readonly List<Vector2> turn1move2 = new List<Vector2>()
	{
		new Vector2(3, 1), new Vector2(3, 2), new Vector2(3, 3), new Vector2(3, 4)
	};

	static readonly Vector2 turn1move2hintStart = new Vector2(3, 4);
	static readonly Vector2 turn1move2hintEnd = new Vector2(3, 3);

	static readonly List<Vector2> turn2move1 = new List<Vector2>()
	{
		new Vector2(2, 4), new Vector2(3, 4), new Vector2(4, 4), new Vector2(5, 4), new Vector2(4, 5)
	};

	static readonly Vector2 turn2move1hintStart = new Vector2(4, 5);
	static readonly Vector2 turn2move1hintEnd = new Vector2(4, 4);

	static readonly List<Vector2> turn2move2 = new List<Vector2>()
	{
		new Vector2(3, 3), new Vector2(4, 3), new Vector2(5, 3), new Vector2(4, 4)
	};

	static readonly Vector2 turn2move2hintStart = new Vector2(4, 4);
	static readonly Vector2 turn2move2hintEnd = new Vector2(4, 3);

	#endregion


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
		StartCoroutine(SetupTutorial());
	}

	IEnumerator SetupTutorial()
	{
		inTutorial = true;

		MSResourceManager.resources[ResourceType.CASH] = MSWhiteboard.tutorialConstants.cashInit;
		MSResourceManager.resources[ResourceType.OIL] = MSWhiteboard.tutorialConstants.oilInit;
		MSResourceManager.resources[ResourceType.GEMS] = MSWhiteboard.tutorialConstants.gemsInit;

		foreach (var item in TutorialUI.disableHUD) 
		{
			item.SetActive(false);
		}

		//Make tutorial units
//		Debug.LogWarning("Tutorial mobster id: " + MSWhiteboard.tutorialConstants.startingMonsterId);
		userMobster = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.startingMonsterId);
		guide = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.guideMonsterId);
		zark = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.markZMonsterId);
		enemyOne = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.enemyMonsterId);
		enemyTwo = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.enemyMonsterIdTwo);
		enemyBoss = MSDataManager.instance.Get<MonsterProto>(MSWhiteboard.tutorialConstants.enemyBossMonsterId);

		userUnit = MSBuildingManager.instance.MakeTutorialUnit(userMobster.monsterId, new Vector2(7, 10), 0);
		userUnit.cityUnit.speed = TutorialValues.swaggyMoveSpeed;

		guideUnit = MSBuildingManager.instance.MakeTutorialUnit(guide.monsterId, new Vector2(16, 5), 1);
		guideUnit.cityUnit.speed = TutorialValues.guidMoveSpeed;

		zarkUnit = MSBuildingManager.instance.MakeTutorialUnit(zark.monsterId, new Vector2(14.5f, 4.5f), 2);
		zarkUnit.direction = MSValues.Direction.SOUTH;
		zarkUnit.gameObject.SetActive(false);

		enemyOneUnit = MSBuildingManager.instance.MakeTutorialUnit (enemyOne.monsterId, TutorialValues.enemyStartPos, 3);
		enemyTwoUnit = MSBuildingManager.instance.MakeTutorialUnit (enemyTwo.monsterId, TutorialValues.enemyStartPos, 4);
		bossUnit = MSBuildingManager.instance.MakeTutorialUnit (enemyBoss.monsterId, TutorialValues.enemyStartPos, 5);


		PZMonster userMonster = new PZMonster(userMobster, 1);
		userMonster.userMonster = new FullUserMonsterProto();
		userMonster.userMonster.userMonsterUuid = "1";
		userMonster.userMonster.isComplete = true;

		MSMonsterManager.instance.userTeam[0] = userMonster;
		MSMonsterManager.instance.userMonsters.Add(userMonster);
		MSMonsterManager.instance.userTeam[1] = new PZMonster(zark, 15);

		//Make tutorial buildings
		List<MSBuilding> buildings = new List<MSBuilding>();
		for (int i = 0; i < MSWhiteboard.tutorialConstants.tutorialStructures.Count; i++) 
		{
			TutorialStructProto tutStruct = MSWhiteboard.tutorialConstants.tutorialStructures[i];
			MSBuilding building = MSBuildingManager.instance.MakeTutorialBuilding(tutStruct, i);
			switch (building.combinedProto.structInfo.structType)
			{
			case StructureInfoProto.StructType.TOWN_HALL:
				MSBuildingManager.townHall = building;
				break;
			case StructureInfoProto.StructType.HOSPITAL:
				MSHospitalManager.instance.hospitals.Add(building.hospital);
				break;
			case StructureInfoProto.StructType.TEAM_CENTER:
				MSBuildingManager.teamCenter = building;
				break;
			case StructureInfoProto.StructType.RESIDENCE:
				MSBuildingManager.residences.Add (building);
				break;
			case StructureInfoProto.StructType.RESOURCE_GENERATOR:
				MSBuildingManager.collectors.Add(building);
				break;
			default:
				break;
			}
			building.userStructProto.isComplete = true;
			buildings.Add(building);
		}
		foreach (var item in MSWhiteboard.tutorialConstants.tutorialObstacles) 
		{
			buildings.Add(MSBuildingManager.instance.MakeTutorialObstacle(item));
		}

		foreach (var item in buildings) 
		{
			while (!item.loadedSprite)
			{
				yield return null;
			}
		}

		MSMonsterManager.instance.totalResidenceSlots = MSBuildingManager.instance.GetMonsterSlotCount();

		TutorialUI.boat.position = MSGridManager.instance.GridToWorld(TutorialValues.boatStartPos);
		TutorialUI.boat.gameObject.SetActive(true);

		enemyOneUnit.cityUnit.speed = enemyTwoUnit.cityUnit.speed = bossUnit.cityUnit.speed = TutorialValues.enemyMoveSpeed;
		enemyOneUnit.alpha = 0;
		enemyTwoUnit.alpha = 0;
		bossUnit.alpha = 0;

		MSActionManager.Scene.OnCity();

		
		StartCoroutine(MainTutorial());

	}

	void CleanUpTutorial()
	{
		TutorialUI.boat.gameObject.SetActive(false);

		RecycleCityUnit(userUnit);
		RecycleCityUnit(guideUnit);
		RecycleCityUnit(zarkUnit);
		RecycleCityUnit(enemyOneUnit);
		RecycleCityUnit(enemyTwoUnit);
		RecycleCityUnit(bossUnit);

		PZCombatManager.instance.activeEnemy = bossCombatant;
		PZCombatManager.instance.backupPvPEnemies[0] = enemyTwoCombatant;
		PZCombatManager.instance.backupPvPEnemies[1] = enemyOneCombatant;

	}

	void RecycleCityUnit(MSUnit unit)
	{
		unit.cityUnit.speed = 3;
		unit.cityUnit.jumpNode = null;
		unit.Pool();
	}

	IEnumerator MainTutorial()
	{
		yield return StartCoroutine(PreCombat_MainTutorial());
		yield return StartCoroutine(Combat_MainTutorial());
		yield return StartCoroutine(PostCombat_MainTutorial());
	}

	#region Pre Combat Tutorial

	IEnumerator PreCombat_MainTutorial()
	{
		MSTownCamera.instance.SlideToPos(startCameraPos, TutorialValues.cameraSize, 0);
		
		guideUnit.direction = MSValues.Direction.WEST;
		guideUnit.animat = MSUnit.AnimationType.STAY;

		currUi = TutorialUI.leftDialogue.clickbox;

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, guide.imagePrefix, guide.imagePrefix + "TutBig", guide.displayName, TutorialStrings.HEY_BOSS, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, guide.imagePrefix, guide.imagePrefix + "TutBig", guide.displayName, TutorialStrings.EVIL_DICTATOR, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, guide.imagePrefix, guide.imagePrefix + "TutBig", guide.displayName, TutorialStrings.HOPEFULLY_DONT_FIND, true));
		
		guideUnit.direction = MSValues.Direction.SOUTH;
		guideUnit.animat = MSUnit.AnimationType.IDLE;

		yield return MSTownCamera.instance.SlideToPos(pierCameraPos, TutorialValues.cameraSize, TutorialValues.panToBoatTime);

		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.boatScene);
		yield return StartCoroutine (MoveBoat(TutorialValues.boatStartPos, TutorialValues.boatDockPos));

		enemyOneUnit.cityUnit.TutorialPath(enemyOneEnterPath);
		enemyOneUnit.DoJump(TutorialValues.enemyEnterJumpHeight, TutorialValues.enemyEnterJumpTime);
		enemyOneUnit.DoFade(true, TutorialValues.enemyEnterJumpTime);
		yield return new WaitForSeconds(TutorialValues.enemyEnterJumpTime/2);
		enemyTwoUnit.cityUnit.TutorialPath(enemyTwoEnterPath);
		enemyTwoUnit.DoJump(TutorialValues.enemyEnterJumpHeight, TutorialValues.enemyEnterJumpTime);
		enemyTwoUnit.DoFade(true, TutorialValues.enemyEnterJumpTime);
		yield return new WaitForSeconds(TutorialValues.enemyEnterJumpTime/2);
		bossUnit.cityUnit.TutorialPath (enemyBossEnterPath);
		bossUnit.DoJump(TutorialValues.enemyEnterJumpHeight, TutorialValues.enemyEnterJumpTime);
		bossUnit.DoFade(true, TutorialValues.enemyEnterJumpTime);

		MSTownCamera.instance.SlideToPos(showdownCameraPos, TutorialValues.cameraSize, TutorialValues.panFromBoatTime);

		while (enemyOneUnit.cityUnit.moving)
		{
			yield return null;
		}
		enemyOneUnit.direction = MSValues.Direction.NORTH;
		while (enemyTwoUnit.cityUnit.moving)
		{
			yield return null;
		}
		enemyTwoUnit.direction = MSValues.Direction.NORTH;
		while(bossUnit.cityUnit.moving)
		{
			yield return null;
		}

		yield return StartCoroutine(DoDialogue(TutorialUI.rightDialogue, enemyBoss.imagePrefix, enemyBoss.imagePrefix + "ArmsCrossed", enemyBoss.displayName, TutorialStrings.PEASANT_SQUAD, true));
		
		yield return enemyTwoUnit.DoJump(TutorialValues.hopHeight, TutorialValues.hopTime);
		yield return enemyTwoUnit.DoJump(TutorialValues.hopHeight, TutorialValues.hopTime);

		yield return StartCoroutine(DoDialogue(TutorialUI.rightDialogue, enemyTwo.imagePrefix, enemyTwo.imagePrefix + "TutBig", enemyTwo.displayName, TutorialStrings.EGGSPECT, true));

		yield return guideUnit.DoJump(TutorialValues.hopHeight, TutorialValues.hopTime);

		guideUnit.cityUnit.TutorialPath(guideRetreatPath);

		while (guideUnit.cityUnit.moving)
		{
			yield return null;
		}
		guideUnit.direction = MSValues.Direction.WEST;

		guideUnit.anim.SetTrigger("Kneel");

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, guide.imagePrefix, guide.imagePrefix + "Crouch", guide.displayName, TutorialStrings.SEND_NEPHEW, true));

		userUnit.cityUnit.TutorialPath(swaggyEnterPath);

		for (int i = 0; i < TutorialValues.swaggyHopCount; i++) 
		{
			yield return userUnit.DoJump(TutorialValues.swaggyHopHeight, TutorialValues.swaggyHopTime);
		}

		while (userUnit.cityUnit.moving)
		{
			yield return null;
		}

		userUnit.animat = MSUnit.AnimationType.STAY;

		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, userMobster.imagePrefix, userMobster.imagePrefix + "ArmsCrossed", userMobster.displayName, TutorialStrings.YOLO, false));

		TutorialUI.fightButton.SetActive(true);
		currUi = TutorialUI.fightButton;
		yield return StartCoroutine(WaitForClick());
		userUnit.animat = MSUnit.AnimationType.IDLE;
		TutorialUI.leftDialogue.RunPushOut();

		userUnitReturnPosition = userUnit.transf.localPosition;

		userUnit.cityUnit.SetTarget(new MSGridNode(13, 1, MSValues.Direction.SOUTH));
		userUnit.cityUnit.moving = true;
		userUnit.DoJump(1.5f, 1);
		yield return new WaitForSeconds(.3f);

		TutorialUI.fightButton.SetActive(false);
	}

	#endregion

	#region Combat Tutorial

	IEnumerator Combat_MainTutorial()
	{
		Combat_Setup();

		yield return StartCoroutine(Combat_IntroFirstEnemy());
		yield return StartCoroutine(Combat_FirstFight());
		yield return StartCoroutine(Combat_IntroSecondFight());
		yield return StartCoroutine(Combat_SecondFightPartOne());
		yield return StartCoroutine(Combat_BringInZark());
		yield return StartCoroutine(Combat_ZarkFights());
	}

	IEnumerator Combat_IntroFirstEnemy()
	{

		yield return PZCombatManager.instance.RunScrollToNextEnemy();

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, userMobster.imagePrefix, userMobster.imagePrefix + "TutBig", userMobster.displayName, TutorialStrings.WANTS_A_PIECE, true));
		
		yield return enemyOneCombatant.unit.DoJump (TutorialValues.hopHeight * TutorialValues.puzzlePixelMod, TutorialValues.hopTime);
		yield return enemyOneCombatant.unit.DoJump (TutorialValues.hopHeight * TutorialValues.puzzlePixelMod, TutorialValues.hopTime);

		yield return StartCoroutine(DoDialogue(TutorialUI.rightDialogue, enemyOne.imagePrefix, enemyOne.imagePrefix + "TutBig", enemyOne.displayName, TutorialStrings.AINT_CHICKEN, true));
		yield return StartCoroutine(DoDialogue(TutorialUI.rightDialogue, enemyTwo.imagePrefix, enemyTwo.imagePrefix + "TutBig", enemyTwo.displayName, TutorialStrings.DOT_DOT_DOT, true));

		enemyTwoCombatant.unit.direction = MSValues.Direction.SOUTH;
		enemyTwoCombatant.unit.animat = MSUnit.AnimationType.ATTACK;
		yield return StartCoroutine(WaitForFlinch());
		enemyOneCombatant.unit.direction = MSValues.Direction.NORTH;
		enemyOneCombatant.unit.animat = MSUnit.AnimationType.FLINCH;
		MSPoolManager.instance.Get(MSPrefabList.instance.flinchParticle, enemyOneCombatant.unit.transf.position);
		yield return new WaitForSeconds(.4f);
		enemyOneCombatant.unit.animat = MSUnit.AnimationType.IDLE;
		enemyTwoCombatant.unit.animat = MSUnit.AnimationType.IDLE;

		yield return StartCoroutine(DoDialogue(TutorialUI.rightDialogue, enemyOne.imagePrefix, enemyOne.imagePrefix + "TutBig", enemyOne.displayName, TutorialStrings.MAKE_ME_FRY, true));

		bossCombatant.unit.direction = MSValues.Direction.EAST;
		yield return bossCombatant.unit.DoJump(TutorialValues.bossStompHeight * TutorialValues.puzzlePixelMod, TutorialValues.bossStompTime);
		yield return TutorialUI.cameraShake.RunShake();
		enemyOneCombatant.unit.direction = MSValues.Direction.WEST;
		enemyTwoCombatant.unit.direction = MSValues.Direction.WEST;

		yield return StartCoroutine(DoDialogue(TutorialUI.rightDialogue, enemyBoss.imagePrefix, enemyBoss.imagePrefix + "TutBig", enemyBoss.displayName, TutorialStrings.GO_PEPE, true));

		combatPosition = bossCombatant.transform.localPosition;

		bossCombatant.RunAdvanceTo(bossCombatant.startingPos.x, -PZScrollingBackground.instance.direction, PZScrollingBackground.instance.scrollSpeed * 2.5f, true);

		Vector3 enemyTwoPosition = enemyTwoCombatant.transform.localPosition;
		yield return enemyTwoCombatant.RunAdvanceTo((enemyTwoPosition.x + enemyTwoCombatant.startingPos.x)/2, -PZScrollingBackground.instance.direction, PZScrollingBackground.instance.scrollSpeed * 2.5f, true);

		yield return enemyTwoCombatant.RunAdvanceTo(enemyTwoPosition.x, -PZScrollingBackground.instance.direction, PZScrollingBackground.instance.scrollSpeed * 3.5f, true);

		enemyTwoCombatant.unit.direction = MSValues.Direction.SOUTH;
		enemyTwoCombatant.unit.animat = MSUnit.AnimationType.ATTACK;
		yield return StartCoroutine(WaitForFlinch());
		enemyOneCombatant.unit.direction = MSValues.Direction.NORTH;
		enemyOneCombatant.unit.animat = MSUnit.AnimationType.FLINCH;
		MSPoolManager.instance.Get(MSPrefabList.instance.flinchParticle, enemyOneCombatant.unit.transf.position);
		yield return new WaitForSeconds(.4f);
		enemyOneCombatant.unit.animat = MSUnit.AnimationType.IDLE;
		enemyTwoCombatant.unit.animat = MSUnit.AnimationType.IDLE;

		enemyTwoCombatant.RunAdvanceTo(enemyTwoCombatant.startingPos.x, -PZScrollingBackground.instance.direction, PZScrollingBackground.instance.scrollSpeed * 3.5f, true);
		
		enemyOneCombatant.unit.direction = MSValues.Direction.WEST;
		yield return StartCoroutine(enemyOneCombatant.MoveTo(combatPosition, 150, true));
	}

	IEnumerator Combat_FirstFight()
	{
		PZCombatManager.instance.activeEnemy = enemyOneCombatant;
		PZCombatManager.instance.backupPvPEnemies[1] = bossCombatant;

		PZCombatManager.instance.boardMove.PlayForward();
		PZCombatManager.instance.boardTint.FadeOut();
		PZPuzzleManager.instance.swapLock = 0;

		yield return PZCombatManager.instance.turnDisplay.RunInit(userCombatant.monster, enemyOneCombatant.monster);
		PZCombatManager.instance.RunPickNextTurn(false);

		PZPuzzleManager.instance.BlockBoard(turn1move1);
		PZPuzzleManager.instance.CustomHintGems(turn1move1);
		TutorialUI.hintHand.Init(turn1move1hintStart, turn1move1hintEnd);
		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, userMobster.imagePrefix, userMobster.imagePrefix + "ArmsCrossed", userMobster.displayName, TutorialStrings.MOVIN_ORBS, false));
		yield return StartCoroutine(WaitForTurn());

		PZPuzzleManager.instance.BlockBoard(turn1move2);
		PZPuzzleManager.instance.CustomHintGems(turn1move2);
		TutorialUI.hintHand.Init(turn1move2hintStart, turn1move2hintEnd);
		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, userMobster.imagePrefix, userMobster.imagePrefix + "TutBig", userMobster.displayName, TutorialStrings.SMOOTH_MOVE, false));
		yield return StartCoroutine(WaitForTurn());

		TutorialUI.leftDialogue.canBeClicked = true;
		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, userMobster.imagePrefix, userMobster.imagePrefix + "TutBig", userMobster.displayName, TutorialStrings.LAST_MOVE, false));
		yield return StartCoroutine(WaitForTurn());

		PZPuzzleManager.instance.swapLock++;
		PZCombatManager.instance.boardTint.FadeIn();
	}

	IEnumerator Combat_IntroSecondFight()
	{
		enemyTwoCombatant.unit.direction = bossCombatant.unit.direction = MSValues.Direction.WEST;

		PZCombatManager.instance.activeEnemy = bossCombatant;
		yield return PZCombatManager.instance.RunScrollToNextEnemy();
		PZCombatManager.instance.boardTint.FadeIn();

		yield return StartCoroutine(DoDialogue(TutorialUI.rightDialogue, enemyBoss.imagePrefix, enemyBoss.imagePrefix + "Facepalm", enemyBoss.displayName, TutorialStrings.CHICKENS_WORK, true));

		PZCombatManager.instance.activeEnemy = enemyTwoCombatant;
		PZCombatManager.instance.backupPvPEnemies[1] = bossCombatant;

		yield return enemyTwoCombatant.unit.DoJump (TutorialValues.hopHeight * TutorialValues.puzzlePixelMod, TutorialValues.hopTime);
		yield return enemyTwoCombatant.unit.DoJump (TutorialValues.hopHeight * TutorialValues.puzzlePixelMod, TutorialValues.hopTime);
		
		bossCombatant.RunAdvanceTo(bossCombatant.startingPos.x, -PZScrollingBackground.instance.direction, PZScrollingBackground.instance.scrollSpeed * 2.5f, true);

		yield return StartCoroutine(enemyTwoCombatant.MoveTo(combatPosition, 150, true));
	}

	IEnumerator Combat_SecondFightPartOne()
	{
		PZCombatManager.instance.boardTint.FadeOut();
		PZPuzzleManager.instance.swapLock = 0;
		
		yield return PZCombatManager.instance.turnDisplay.RunInit(userCombatant.monster, enemyTwoCombatant.monster);
		PZCombatManager.instance.RunPickNextTurn(false);

		PZPuzzleManager.instance.BlockBoard(turn2move1);
		PZPuzzleManager.instance.CustomHintGems(turn2move1);
		TutorialUI.hintHand.Init(turn2move1hintStart, turn2move1hintEnd);
		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, userMobster.imagePrefix, userMobster.imagePrefix + "ArmsCrossed", userMobster.displayName, TutorialStrings.CREATE_POWERUP, false));
		yield return StartCoroutine(WaitForTurn());
		
		PZPuzzleManager.instance.BlockBoard(turn2move2);
		PZPuzzleManager.instance.CustomHintGems(turn2move2);
		TutorialUI.hintHand.Init(turn2move2hintStart, turn2move2hintEnd);
		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, userMobster.imagePrefix, userMobster.imagePrefix + "TutBig", userMobster.displayName, TutorialStrings.SWIPE_POWERUP, false));
		yield return StartCoroutine(WaitForTurn());
		
		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, userMobster.imagePrefix, userMobster.imagePrefix + "TutBig", userMobster.displayName, TutorialStrings.BALLIN, false));
		yield return StartCoroutine(WaitForTurn());

		PZPuzzleManager.instance.swapLock++;
		PZCombatManager.instance.boardTint.FadeIn();
	}

	IEnumerator Combat_BringInZark()
	{
		userCombatant.unit.anim.SetTrigger("Kneel");

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, userMobster.imagePrefix, userMobster.imagePrefix + "Hurt", userMobster.displayName, TutorialStrings.NOLO, true));
		
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.POKE, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.HEY_BUDDY, true));

		TutorialUI.swapButton.gameObject.SetActive(true);
		TutorialUI.swapButton.Show();

		yield return StartCoroutine(DoUIStep(TutorialUI.swapButton.gameObject, 105, MSValues.Direction.EAST));
		
		yield return StartCoroutine(DoUIStep(TutorialUI.swapForZark, 105, MSValues.Direction.NORTH));
		userCombatant.unit.anim.SetTrigger("Kneel");

		TutorialUI.swapButton.gameObject.SetActive(false);

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.UPDATE_BOOKFACE, true));
		yield return StartCoroutine(DoDialogue(TutorialUI.rightDialogue, enemyTwo.imagePrefix, enemyTwo.imagePrefix + "TutBig", enemyTwo.displayName, TutorialStrings.DOT_DOT_DOT, true));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.ZARKS_STATUS, true, false));
		TutorialUI.leftDialogue.canBeClicked = true;
		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.TWELVE_LIKES, false));
	}

	IEnumerator Combat_ZarkFights()
	{
		PZCombatManager.instance.boardTint.FadeOut();
		PZPuzzleManager.instance.swapLock = 0;

		yield return StartCoroutine(WaitForTurn());
		yield return StartCoroutine(WaitForTurn());
		yield return StartCoroutine(WaitForTurn());

		yield return userCombatant.RunAdvanceTo(userCombatant.startingPos.x, -PZScrollingBackground.instance.direction, PZScrollingBackground.instance.scrollSpeed * 2.5f, true);
	}

	IEnumerator WaitForFlinch()
	{
		PZCombatManager.instance.hijackPlayerFlinch = HijackFlinch;
		hijackFlinch = true;
		while (hijackFlinch)
		{
			yield return null;
		}
	}

	public void HijackFlinch()
	{
		hijackFlinch = false;
	}

	void Combat_Setup()
	{
		MSActionManager.Scene.OnPuzzle();
		PZCombatManager.instance.InitTutorial(enemyBoss, enemyOne, enemyTwo);

		userCombatant = PZCombatManager.instance.activePlayer;

		enemyOneCombatant = PZCombatManager.instance.backupPvPEnemies[1];
		enemyTwoCombatant = PZCombatManager.instance.backupPvPEnemies[0];
		bossCombatant = PZCombatManager.instance.activeEnemy;
		enemyOneCombatant.unit.direction = enemyTwoCombatant.unit.direction = bossCombatant.unit.direction = MSValues.Direction.WEST;

		PZPuzzleManager.instance.InitBoard(6, 6, "Tutorial/TutorialBattle1Layout");
	}

	public bool SummonNextEnemy()
	{
		return true;
	}

	#endregion

	#region Post Combat Tutorial

	IEnumerator PostCombat_MainTutorial()
	{
		PostCombat_Setup();
		yield return StartCoroutine(PostCombat_DialogueAndRunaway());
		yield return StartCoroutine(PostCombat_HealMobsterTutorial());
		yield return StartCoroutine(PostCombat_BuildBuildings());
		yield return StartCoroutine(PostCombat_FacebookLogon());
		if (!facebookHadAccount)
		{
			yield return StartCoroutine(PostCombat_Username());
			yield return StartCoroutine(PostCombat_SendOnFirstMission());
		}
		inTutorial = false;

		foreach (var item in TutorialUI.disableHUD) 
		{
			item.SetActive(true);
		}
	}

	void PostCombat_Setup()
	{
		MSActionManager.Scene.OnCity();
		MSTownCamera.instance.SlideToPos(showdownCameraPos, 6, 0);
		zarkUnit.gameObject.SetActive(true);

		userUnit.transf.localPosition = userUnitReturnPosition;
		userUnit.cityUnit.moving = false;
		userUnit.animat = MSUnit.AnimationType.IDLE;
		userUnit.anim.SetTrigger("Kneel");
		userUnit.sprite.transform.localPosition = new Vector3(userUnit.sprite.transform.localPosition.x,
		                                                        0,
		                                                        userUnit.sprite.transform.localPosition.z);
	}

	IEnumerator WaitUntilReadyToJump(MSUnit unit)
	{
		while (!ReadyToJump (unit))
		{
			yield return null;
		}
	}

	bool ReadyToJump(MSUnit unit)
	{
//		Debug.Log(unit.transf.position.z + " vs " + MSGridManager.instance.GridToWorld(TutorialValues.enemyExitJumpPosition).z);
		return unit.transf.position.z < MSGridManager.instance.GridToWorld(TutorialValues.enemyExitJumpPosition).z;
	}

	IEnumerator PostCombat_DialogueAndRunaway()
	{
		yield return StartCoroutine(DoDialogue(TutorialUI.rightDialogue, enemyBoss.imagePrefix, enemyBoss.imagePrefix + "TutBig", enemyBoss.displayName, TutorialStrings.BEAT_A_CHICKEN, true));

		bossUnit.cityUnit.TutorialPath (enemyBossRetreatPath);
		bossUnit.cityUnit.jumpNode = new MSGridNode(TutorialValues.enemyExitJumpPosition, MSValues.Direction.SOUTH);
		yield return new WaitForSeconds(.25f);
		enemyTwoUnit.cityUnit.TutorialPath(enemyTwoRetreatPath);
		enemyTwoUnit.cityUnit.jumpNode = new MSGridNode(TutorialValues.enemyExitJumpPosition + new Vector2(-1, 0), MSValues.Direction.SOUTH);
		yield return new WaitForSeconds(.25f);
		enemyOneUnit.cityUnit.TutorialPath(enemyOneRetreatPath);
		enemyOneUnit.cityUnit.jumpNode = new MSGridNode(TutorialValues.enemyExitJumpPosition + new Vector2(1, 0), MSValues.Direction.SOUTH);

		while (enemyOneUnit.cityUnit.moving)
		{
			yield return null;
		}

		StartCoroutine(MoveBoat(TutorialValues.boatDockPos, TutorialValues.boatExitPos));

		guideUnit.cityUnit.TutorialPath(guideReturnPath);
		while (guideUnit.cityUnit.moving)
		{
			yield return null;
		}

		guideUnit.direction = MSValues.Direction.SOUTH;
		zarkUnit.direction = MSValues.Direction.NORTH;

	}

	IEnumerator PostCombat_HealMobsterTutorial()
	{
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, guide.imagePrefix, guide.imagePrefix + "TutBig", guide.displayName, TutorialStrings.THANKS_ZARK_DIALOGUE, true));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.KINDA_DYING_DIALOGUE, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.HEAD_TO_HOSPITAL_DIALOGUE, true));

		zarkUnit.direction = MSValues.Direction.NORTH;
		guideUnit.direction = MSValues.Direction.NORTH;

		userUnit.anim.SetTrigger("Kneel");
		userUnit.direction = MSValues.Direction.NORTH;

		//Get to the hospital
		MSHospital hospital = MSHospitalManager.instance.hospitals[0];

		MSTownCamera.instance.DoCenterOnGroundPos(hospital.building.trans.position, TutorialValues.panToHospitalTime);

		currUi = hospital.building.gameObj;
		MSTutorialArrow.instance.Init(hospital.building.sprite.transform, 180, MSValues.Direction.NORTH, .02f);
		yield return StartCoroutine(WaitForClick());

		MoveUnitsOutOfTheWay();

		//Heal taskbutton
		MSTaskButton healButton;
		do
		{
			Debug.Log("Dafuq");
			yield return null;
			healButton = MSTaskBar.instance.taskButtons.Find(x => x.currMode == MSTaskButton.Mode.HEAL);
		}while(healButton == null);

		yield return StartCoroutine(DoUIStep(
			healButton.gameObj,
			150, MSValues.Direction.EAST));

		//Healing dialogue
		StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.TAP_CARD_DIALOGUE, false));

		//Find the Goon to heal
		yield return StartCoroutine(DoUIStep(MSHealScreen.instance.grid.cards[0].gameObject, 125, MSValues.Direction.EAST));

		TutorialUI.leftDialogue.RunPushOut();

		//Finish the heal
		yield return StartCoroutine(DoUIStep(MSHealScreen.instance.currQueue.button.gameObject, 105, MSValues.Direction.NORTH));

		//Close the menu
		yield return StartCoroutine(DoUIStep(TutorialUI.closeHealMenuButton, 50, MSValues.Direction.WEST));

	}

	void MoveUnitsOutOfTheWay()
	{
		zarkUnit.transf.localPosition = MSGridManager.instance.GridToWorld(TutorialValues.zarkOutsidePos);
		userUnit.transf.localPosition = MSGridManager.instance.GridToWorld(TutorialValues.userMobsterOutsidePos);
		guideUnit.transf.localPosition = MSGridManager.instance.GridToWorld(TutorialValues.guidOutsidePos);

		TutorialUI.boat.gameObject.SetActive(false);
	}

	IEnumerator PostCombat_BuildBuildings()
	{
		//Chest of cash dialogue
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.CHEST_OF_CASH_DIALOGUE, true, false));

		foreach (var item in TutorialUI.resourceBars) 
		{
			item.SetActive(true);
		}
		TutorialUI.shopButton.SetActive(true);

		MSResourceManager.instance.DetermineResourceMaxima();

		//Build cash printer dialogue
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.BUILD_CASH_PRINTER_DIALOGUE, true));

		yield return StartCoroutine(BuildStructure(StructureInfoProto.StructType.RESOURCE_GENERATOR, ResourceType.OIL));

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.AFTER_CASH_PRINTER_DIALOGUE, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.BUILD_CASH_VAULT_DIALOGUE, true));

		//Build Vault
		yield return StartCoroutine(BuildStructure(StructureInfoProto.StructType.RESOURCE_STORAGE, ResourceType.OIL));

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.AFTER_CASH_VAULT_DIALOGUE, true, false));

		MSTownCamera.instance.DoCenterOnGroundPos(MSBuildingManager.collectors[0].trans.position, TutorialValues.panToHospitalTime);

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.BEFORE_OIL_SILO_DIALOGUE, true, false));
		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.BUILD_OIL_SILO_DIALOGUE, true));
		
		//Build Silo
		yield return StartCoroutine(BuildStructure(StructureInfoProto.StructType.RESOURCE_STORAGE, ResourceType.CASH));

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.AFTER_OIL_SILO_DIALOGUE, true, false));

	}

	IEnumerator PostCombat_FacebookLogon()
	{
		zarkUnit.animat = MSUnit.AnimationType.STAY;

		MSTownCamera.instance.DoCenterOnGroundPos(zarkUnit.transf.position, TutorialValues.panToZarkTime);

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.ISLAND_BASE_DIALOGUE, true, false));

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.THIS_IS_CRAZY_DIALOGUE, true));

		currUi = null;
		MSActionManager.Popup.OnPopup(TutorialUI.facebookPopup);
		waitForFacebook = true;
		while (waitForFacebook)
		{
			yield return null;
		}
		if (didJoinFacebook)
		{
			yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.FACEBOOK_DID_JOIN_DIALOGUE, true));
		}
		else
		{
			yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.FACEBOOK_NOT_JOIN_DIALOGUE, true));
		}
	}
	
	public void OnMakeFacebookDecision(bool didJoin)
	{
		waitForFacebook = false;
		didJoinFacebook = didJoin;
	}

	IEnumerator PostCombat_Username()
	{
		currUi = null;
		waitingForUsername = true;
		MSActionManager.Popup.OnPopup(TutorialUI.usernamePopup);
		while (waitingForUsername)
		{
			yield return null;
		}

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, zark.imagePrefix, zark.imagePrefix + "TutBig", zark.displayName, TutorialStrings.BIRTH_CERTIFICATE_DIALOGUE, true));
	}

	public void OnUsernameEnter()
	{
		waitingForUsername = false;
		MSMapManager.instance.InitMapTasks(null);
	}

	IEnumerator PostCombat_SendOnFirstMission()
	{
		guideUnit.direction = MSValues.Direction.SOUTH;
		guideUnit.animat = MSUnit.AnimationType.STAY;

		yield return StartCoroutine(DoDialogue(TutorialUI.leftDialogue, guide.imagePrefix, guide.imagePrefix + "TutBig", guide.displayName, TutorialStrings.GO_RECRUIT_DIALOGUE, true));

		TutorialUI.attackButton.gameObject.SetActive(true);

		//Attack button
		currUi = TutorialUI.attackButton;
		MSTutorialArrow.instance.Init(currUi.transform, 150, MSValues.Direction.NORTH);
		yield return StartCoroutine(WaitForClick());
		
		CleanUpTutorial();
		
		//Enter button
		currUi = TutorialUI.enterButton;
		MSTutorialArrow.instance.Init(currUi.transform, 150, MSValues.Direction.NORTH);
		yield return StartCoroutine(WaitForClick());
		currUi = null;
		inTutorial = false;
		MSTutorialArrow.instance.gameObject.SetActive(false);

		MSBuildingManager.instance.DoLoadPlayerCity(false);
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
			currUi = dialogueUI.clickbox;
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
		//Debug.Log("Not clicked");
		while (!clicked)
		{
			yield return null;
		}
		//Debug.Log("Clicked");
		//currUi = null;
	}

	IEnumerator WaitForTurn()
	{
		currUi = null;
		waitingForTurn = true;
		MSActionManager.Puzzle.OnTurnChange += TurnHappen;
		while (waitingForTurn)
		{
			yield return null;
		}
		//Debug.Log("Turned!");
		MSActionManager.Puzzle.OnTurnChange -= TurnHappen;
	}

	IEnumerator MoveBoat(Vector3 start, Vector3 end)
	{
		start = MSGridManager.instance.GridToWorld(start);
		end = MSGridManager.instance.GridToWorld(end);
		float time = (start - end).magnitude / TutorialValues.boatSpeed;
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime / time;
			TutorialUI.boat.position = Vector3.Lerp(start, end, t);
			yield return null;
		}
		TutorialUI.boat.position = end;
	}

	public void TurnHappen(int turnsLeft)
	{
//		Debug.Log("Turns left: " + turnsLeft);
		if (turnsLeft > 0)
		{
			waitingForTurn = false;
		}
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
public class TutorialValues
{
	public float cameraSize;

	public Vector2 enemyStartPos;

	public Vector2 boatStartPos;

	public Vector2 boatDockPos;

	public Vector2 boatExitPos;

	public Vector2 userMobsterOutsidePos;

	public Vector2 guidOutsidePos;

	public Vector2 zarkOutsidePos;

	public float boatSpeed;

	public float enemyEnterJumpHeight;

	public float enemyEnterJumpTime;

	public Vector2 enemyExitJumpPosition;

	public float hopHeight;

	public float hopTime;

	public float swaggyHopHeight;

	public float swaggyHopTime;

	public float swaggyHopCount;

	public float enemyMoveSpeed;

	public float swaggyMoveSpeed;

	public float guidMoveSpeed;

	public float bossStompHeight;

	public float bossStompTime;

	public float puzzlePixelMod;

	public float panToBoatTime;

	public float panFromBoatTime;

	public float panToHospitalTime;

	public float panToZarkTime;
}

[Serializable]
public class TutorialUI
{
	public UIWidget dialogueClickbox;

	public UISprite arrow;

	public GameObject[] disableHUD;

	public GameObject[] resourceBars;

	public Transform boat;

	public GameObject fightButton;

	public PZSwapButton swapButton;

	public GameObject swapForZark;
	
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

	public PZTutorialHand hintHand;

	public MSCameraShake cameraShake;
}




