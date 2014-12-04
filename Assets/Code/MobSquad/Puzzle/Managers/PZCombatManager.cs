using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// PZ combat manager, which keeps track of the units involved in the combat
/// and manages the progression of individual combats
/// </summary>
using System;


public class PZCombatManager : MonoBehaviour {

	/// <summary>
	/// The static instance of this combat
	/// </summary>
	public static PZCombatManager instance;

	[SerializeField]
	Camera camera;

	public BattleStats battleStats = new BattleStats();

	public bool pvpMode = false;

	[SerializeField]
	GameObject quitPvpButton;

	public bool raidMode = false;

	public bool waitingForTutorialSignal = false;

	public UIPanel combatPanel;

	/// <summary>
	/// The player goonies that they brought into combat with them.
	/// </summary>
	public List<PZMonster> playerGoonies = new List<PZMonster>();
	
	/// <summary>
	/// The remaining enemies. This is populated during level loading,
	/// and dequeued whenever we need another enemy. When this is empty
	/// and an enemy is defeated, the dungeon is complete.
	/// </summary>
	public Queue<PZMonster> enemies = new Queue<PZMonster>();

	public List<PZMonster> defeatedEnemies = new List<PZMonster>();

	/// <summary>
	/// The player's active goonie, who deals and takes damage.
	/// </summary>
	public PZCombatUnit activePlayer;
	
	/// <summary>
	/// The active enemy being fought and dealt damage by the player
	/// </summary>
	public PZCombatUnit activeEnemy;

	public PZCombatUnit[] backupPvPEnemies;

	/// <summary>
	/// The unit prefab which will be used as the template to generate
	/// enemies.
	/// </summary>
	[SerializeField]
	MSUnit unitPrefab;

	[SerializeField]
	PZForfeitWords forfeit;

	/// <summary>
	/// The scrolling background, which will be scrolled appropriately
	/// as the player's unit moves from enemy to enemy
	/// </summary>
	[SerializeField]
	public PZScrollingBackground background;
	
	/// <summary>
	/// The combat parent. Generated units will use this as their parent
	/// to keep the heriarchy organized, as well as ensure that coordiantes
	/// won't get messed up
	/// </summary>
	[SerializeField]
	Transform combatParent;

	[HideInInspector]
	public PZCrate crate;

	List<string> playersSeen = new List<string>();

	const float playerXFromSideThreshold = 130;

	Queue<int> riggedAttacks;

	float playerXPos
	{
		get
		{
			float xPos = -(Screen.width * (640f / Screen.height) / 2) 
				+ playerXFromSideThreshold;// * Mathf.Min(1, (Screen.height / 640f));
			//Debug.LogWarning("Player X pos: " + xPos);
			return xPos;
		}
	}

	//const float enemyXFromRightThreshold = -380;

	float enemyXPos
	{
		get
		{
			return playerXPos + 150;
		}
	}

	Vector3 enemyStartPosition;
	
	[SerializeField]
	PZDeployPopup deployPopup;
	
	/// <summary>
	/// The popup that displays when the player fails a mission
	/// </summary>
	[SerializeField]
	PZWinLosePopup winLosePopup;

	[SerializeField]
	PZAttackWords attackWords;

	[SerializeField]
	UISprite effectiveness;

	[SerializeField]
	UITweener attackWordsTweenPos;
	
	public TweenAlpha boardTint;

	public TweenPosition boardMove;

	[SerializeField]
	UITweener screenTint;

	[SerializeField]
	TweenAlpha bloodSplatter;

	[SerializeField]
	MSPvpUI pvpUI;

	[SerializeField]
	UILabel mobsterCounter;

	public PZSkillIndicator playerSkillIndicator;

	[SerializeField]
	PZSkillIndicator enemySkillIndicator;

	[SerializeField]
	PZSkillAnimator enemySkillAnimator;

	[SerializeField]
	PZSkillAnimator playerSkillAnimator;

	public PZTurnDisplay turnDisplay;

	PZCombatSave save;

	int currPlayerDamage = 0;

	public int currTurn = 0;

	public int playerTurns = 3;

	public int nextPvpDefenderIndex = 0;

	[SerializeField] float pvpSpeedMux;

	PvpProto defender;

	public float recoilDistance = 3;

	public float recoilTime = .4f;

	bool waiting = false;

	[SerializeField] float MELEE_ATTACK_DISTANCE = 60;

	const float BLEED_ONCE_THRESH = 0.5f;
	const float BLEED_CONT_THRESH = 0.3f;

	const float BALLIN_SCORE = 2.30f;
	const float CANT_TOUCH_THIS_SCORE = 3.00f;
	const float HAMMERTIME_SCORE = 4.20f;
	const float MAKE_IT_RAIN_SCORE = 6.00f;

	const string BALLIN_SPRITE_NAME = "ballin";
	const string CANT_TOUCH_THIS_SPRITE_NAME = "canttouchthis";
	const string HAMMERTIME_SPRITE_NAME = "hammertime";

	const string MAKE_IT_RAIN_PREFIX = "mir";

	const int MAX_SHOTS = 5;
	
	const float BOMB_SPACING = 50;
	
	const int NUM_BOMBS = 5;

	public const int MATCH_MONEY = 100;

	public int pvpMatchCost
	{
		get
		{
			return MSBuildingManager.currTownHall.pvpQueueCashCost;
		}
	}

	public UILabel prizeQuantityLabel;

	[SerializeField]
	PZMonsterIntro intro;

	public int totalEnemies = 0;

	int revives = 0;

	public float forfeitChance;
	const float FORFEIT_GAIN_RATE = 0.25f;
	const float FORFEIT_START_CHANCE = 1f;

	int savedHealth = -1;

	/// <summary>
	/// The player skill points.
	/// </summary>
	public int playerSkillPoints = 0;

	/// <summary>
	/// If the enemy has an auto skill, this ticks up whenever the player matches their element gem
	/// When this hits the right number, their skill activates
	/// </summary>
	public int enemySkillPoints = 0;

	/// <summary>
	/// Whether we're currently in a quick-attack.
	/// Used to stop ReturnFromPlayerAttack to skip the rest of the player turn.
	/// </summary>
	bool quickAttack = false;

	public List<PZGem> bombs = new List<PZGem>();

	bool firstTimeUserWonTask = false;

	bool playerSkillReady
	{
		get
		{
			return playerOffSkill != null && playerOffSkill.skillId > 0
				&& playerSkillPoints >= playerOffSkill.orbCost;
		}
	}

	public int forceEnemySkill;
	public int forcePlayerSkill;

	bool enemyCakeKid
	{
		get
		{
			return activeEnemy.monster != null
				&& enemyDefSkill != null
					&& enemyDefSkill.type == SkillType.CAKE_DROP;
		}
	}

	bool combatActive = false;

	public SkillProto enemyDefSkill
	{
		get
		{
			if (forceEnemySkill > 0)
			{
				return MSDataManager.instance.Get<SkillProto>(forceEnemySkill);
			}
			return activeEnemy.monster.defensiveSkill;
		}
	}

	public SkillProto playerOffSkill
	{
		get
		{
			if (forcePlayerSkill > 0)
			{
				return MSDataManager.instance.Get<SkillProto>(forcePlayerSkill);
			}
			return activePlayer.monster.offensiveSkill;
		}
	}

	[SerializeField] float bombSkillAnimationTimeBetweenBombs = .2f;
	[SerializeField] float bombSkillAnimationXRangeForBombs = 10;

	int poisonDamage = 0;

	[SerializeField] Color roidColor;

	public Action hijackPlayerFlinch;

	PZBomb lastBomb;

	/// <summary>
	/// Awake this instance. Set up instance reference.
	/// </summary>
	void Awake()
	{
		instance = this;
	}
	
	void OnEnable()
	{
		MSActionManager.Scene.OnCity += OnCity;
		MSActionManager.Puzzle.OnDeploy += OnDeploy;
		activePlayer.OnDeath += OnPlayerDeath;
		activeEnemy.OnDeath += OnEnemyDeath;

		MSActionManager.Clan.OnRaidMonsterAttacked += OnRaidEnemyAttacked;
		MSActionManager.Clan.OnRaidMonsterDied += OnRaidEnemyDefeated;

		MSActionManager.Puzzle.OnNewPlayerRound += DoRevealCounter;
	}
	
	void OnDisable()
	{
		MSActionManager.Scene.OnCity -= OnCity;
		MSActionManager.Puzzle.OnDeploy -= OnDeploy;
		activePlayer.OnDeath -= OnPlayerDeath;
		activeEnemy.OnDeath -= OnEnemyDeath;
		
		MSActionManager.Clan.OnRaidMonsterAttacked -= OnRaidEnemyAttacked;
		MSActionManager.Clan.OnRaidMonsterDied -= OnRaidEnemyDefeated;

		MSActionManager.Puzzle.OnNewPlayerRound -= DoRevealCounter;
	}

	void OnCity()
	{
		pvpMode = false;
		raidMode = false;
		combatActive = false;
		PZCombatSave.Delete();
	}
	
	void ResetBattleStats()
	{
		battleStats.combos = 0;
		battleStats.grenades = 0;
		battleStats.orbs = new int[] {0, 0, 0, 0, 0, 0};
		battleStats.rainbows = 0;
		battleStats.rockets = 0;
		battleStats.damageTaken = 0;
		battleStats.monstersDefeated = 0;

		revives = 0;
	}

	void DoRevealCounter()
	{
		StartCoroutine(RevealCounter());
	}

	IEnumerator RevealCounter()
	{
		yield return new WaitForSeconds(0.5f);

		mobsterCounter.MakePixelPerfect();

		mobsterCounter.GetComponent<PZMobsterCounter>().MoveToMidPoint();
		TweenAlpha.Begin(mobsterCounter.gameObject, 0.3f, 1f);

	}

	void PreInit()
	{
		bombs.Clear();

		riggedAttacks = null;

		ResetBattleStats();

		TintBoard();
		
		StopBleeding();
		
		activeEnemy.GoToStartPos();
		activeEnemy.alive = false;
		activePlayer.GoToStartPos();
		activePlayer.alive = false;
		activePlayer.monster = null;

		backupPvPEnemies[0].GoToStartPos();
		backupPvPEnemies[1].GoToStartPos();
		
		enemies.Clear();
		defeatedEnemies.Clear();
		playerGoonies.Clear ();
		currTurn = 0;
		currPlayerDamage = 0;

		winLosePopup.gameObject.SetActive(false);

		if (activeEnemy != null && activeEnemy.unit != null)
		{
			Color temp = activeEnemy.unit.sprite.color;
			activeEnemy.unit.sprite.color = new Color(temp.r, temp.g, temp.b, 0);
		}
		
		foreach (PZMonster monster in MSMonsterManager.instance.userTeam)
		{
			if (monster != null && monster.monster != null && monster.monster.monsterId > 0)
			{
				playerGoonies.Add(monster);
			}
		}
	}

	public void PreInitTask()
	{
		playerSkillIndicator.ShutOff();
		enemySkillIndicator.ShutOff();

		prizeQuantityLabel.text = "0";

		PreInit();

		boardMove.Sample(0,false);
		boardMove.PlayForward();
		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.boardSlideIn);
		
		pvpUI.Reset();
		pvpMode = false;
		raidMode = false;

		activePlayer.Init(playerGoonies.Find(x=>x.currHP>0));

		playerSkillIndicator.Init(playerOffSkill, activePlayer.monster.monster.monsterElement);
		playerSkillPoints = 0;

		waiting = true;
		StartCoroutine(ScrollToNextEnemy());

	}
	
	/// <summary>
	/// Start this instance. Gets the combat going.
	/// </summary>
	public void InitTask()
	{
		combatActive = true;

		BeginDungeonResponseProto dungeon = MSWhiteboard.loadedDungeon;

		forfeitChance = FORFEIT_START_CHANCE;
		MSActionManager.Puzzle.OnTurnChange(playerTurns - currTurn);
		
		#if UNITY_IPHONE || UNITY_ANDROID
		//Kamcord.StartRecording();
		#endif

		MSWhiteboard.currUserTaskUuid = dungeon.userTaskUuid;
		MSWhiteboard.currTaskId = dungeon.taskId;
		MSWhiteboard.currTaskStages = dungeon.tsp;

		Debug.LogWarning("Number of stages: " + dungeon.tsp.Count);

		totalEnemies = dungeon.tsp.Count;

		PZMonster mon;
		foreach (TaskStageProto stage in dungeon.tsp)
		{
			Debug.Log("Stage " + stage.stageId + ", Monster: " + stage.stageMonsters[0].monsterId);
			mon = new PZMonster(stage.stageMonsters[0]);
			enemies.Enqueue(mon);
		}

		waiting = false;

		//Lock swap until deploy
		PZPuzzleManager.instance.swapLock += 1;
		//Debug.LogWarning("Start lock");
	}

	public Coroutine RunInitLoadedTask(MinimumUserTaskProto minTask, List<TaskStageProto> stages)
	{
		return StartCoroutine(InitLoadedTask(minTask, stages));
	}

	IEnumerator InitLoadedTask(MinimumUserTaskProto minTask, List<TaskStageProto> stages)
	{
		FullTaskProto fullTask = MSDataManager.instance.Get<FullTaskProto>(minTask.taskId);

		combatActive = true;

		playerSkillIndicator.ShutOff();
		enemySkillIndicator.ShutOff();

		MSWhiteboard.currUserTaskUuid = minTask.userTaskUuid;
		MSWhiteboard.currTaskStages = stages;
		MSWhiteboard.currTaskId = minTask.taskId;

		save = PZCombatSave.Load();
		if (save != null && !save.userTaskUuid.Equals(minTask.userTaskUuid))
		{
			save = null;
			Debug.LogWarning("Save data not for right task!");
		}

#if UNITY_IPHONE || UNITY_ANDROID
		//Kamcord.StartRecording();
#endif
		PreInit ();
		
		boardMove.Sample(0,false);
		boardMove.PlayForward();
		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.boardSlideIn);
		
		pvpUI.Reset();

		if (save != null)
		{
			totalEnemies = save.totalEnemies;
			defeatedEnemies = save.defeatedEnemies;//the save only contains enemies that dropped something
			prizeQuantityLabel.text = defeatedEnemies.Count.ToString();
			activePlayer.Init(playerGoonies.Find(x=>x.userMonster.userMonsterUuid.Equals(save.activePlayerUserMonsterUuid)));
			PZCombatScheduler.instance.turns = save.turns;
			PZCombatScheduler.instance.currInd = Mathf.Max(save.currTurnIndex-1, 0);

			forfeitChance = save.forfeitChance;
			
			currTurn = save.currTurn;
			if(MSActionManager.Puzzle.OnTurnChange != null)
			{
				MSActionManager.Puzzle.OnTurnChange(playerTurns - currTurn);
			}

			currPlayerDamage = save.currPlayerDamage;

			savedHealth = save.activeEnemyHealth;
			
			PZPuzzleManager.instance.InitBoardFromSave(save, minTask);

			enemySkillPoints = save.enemySkillPoints;

			if (save.playerSkillSave.skillActive && playerOffSkill != null)
			{
				switch(playerOffSkill.type)
				{
				case SkillType.SHIELD:
					activePlayer.shieldHealth = save.playerSkillSave.shieldHealth;
					activePlayer.tweenShield.BringIn();
					break;
				case SkillType.ROID_RAGE:
					StartCoroutine(GainRage (activePlayer, playerOffSkill));
					break;
				case SkillType.MOMENTUM:
					activePlayer.damageMultiplier = save.playerSkillSave.damageMultiplier;
					break;
				}
			}
		}
		else
		{
			activePlayer.Init(playerGoonies.Find(x=>x.currHP>0));
			forfeitChance = FORFEIT_START_CHANCE;

			currTurn = 0;
			currPlayerDamage = 0;
			savedHealth = -1;
			enemySkillPoints = 0;

			PZPuzzleManager.instance.InitBoard(fullTask.boardWidth, fullTask.boardHeight);
		}

		for (int i = 0; i < stages.Count; i++)
		{
			if (stages[i].stageId == minTask.curTaskStageId)
			{
				for (int j = i; j < stages.Count; j++)
				{
					enemies.Enqueue(new PZMonster(stages[j].stageMonsters[0]));
				}
			}
		}

		PZScrollingBackground.instance.SetBackgrounds(fullTask);
		
		yield return null;
		
		playerSkillIndicator.Init(playerOffSkill, activePlayer.monster.monster.monsterElement);

		if (save != null)
		{
			playerSkillPoints = save.playerSkillPoints;
			playerSkillIndicator.SetPoints(save.playerSkillPoints);
		}
		else
		{
			playerSkillPoints = 0;
			playerSkillIndicator.SetPoints(0);
		}

		yield return RunScrollToNextEnemy(save!=null);

		if (currTurn == playerTurns)
		{
			PlayerAttack();
			currTurn = 0;
			currPlayerDamage = 0;
		}
		else if (save != null)
		{
			RunPickNextTurn(false);
		}
	}

	public void InitPvp(int cash, int gems = 0)
	{
//		mobsterCounter.transform.parent.GetComponent<UIWidget>().alpha = 0f;
		combatActive = true;

		PreInit ();

		playersSeen.Clear();

		MSWhiteboard.loadedPvps = null;

		pvpUI.Reset();
		pvpMode = true;
		raidMode = false;

		activePlayer.Init(playerGoonies.Find(x=>x.currHP>0));
		activePlayer.GoToStartPos();
		
		playerSkillIndicator.Init(playerOffSkill, activePlayer.monster.monster.monsterElement);

		nextPvpDefenderIndex = 0;

		StartCoroutine(SpawnPvps(cash, gems));

		PZPuzzleManager.instance.swapLock += 1;
		//Debug.LogWarning("Start Lock");
	}

	public void InitRaid()
	{
		combatActive = true;

		PreInit();
		
		PZMonster mon;

		//We need to make sure we're using the right player team
		//This gets set up in init, so we're going to clear it and fix it here
		playerGoonies.Clear();
		foreach (var item in MSClanEventManager.instance.myTeam.currentTeam) 
		{
			mon = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(item.userMonsterUuid));
			if (mon != null)
			{
				playerGoonies.Add(mon);
			}
		}

		boardMove.Sample(0,false);
		boardMove.PlayForward();
		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.boardSlideIn);
		
		pvpUI.Reset();
		pvpMode = false;
		raidMode = true;

		MSClanEventManager.instance.GetCurrentStageMonsters().Sort((m1,m2)=>m1.crsmId.CompareTo(m2.crsmId));

		foreach (var item in MSClanEventManager.instance.GetCurrentStageMonsters()) 
		{
			if (item.crsmId >= MSClanEventManager.instance.currClanInfo.crsmId)
			{
				mon = new PZMonster(item);
				enemies.Enqueue(mon);
				if (item.crsmId == MSClanEventManager.instance.currClanInfo.crsmId)
				{
					mon.currHP -= MSClanEventManager.instance.currDamage;
				}
			}
		}
		//CBKEventManager.Popup.OnPopup(deployPopup.gameObject);
		deployPopup.Init(MSMonsterManager.instance.userTeam);
		
		//Lock swap until deploy
		PZPuzzleManager.instance.swapLock += 1;
		//Debug.LogWarning("Start Lock");
	}

	public void InitTutorial(MonsterProto boss, MonsterProto enemyOne, MonsterProto enemyTwo)
	{
		PreInit ();

		combatActive = true;

		activeEnemy.Init(new PZMonster(boss, 1));
		backupPvPEnemies[1].Init(new PZMonster(enemyOne, 1));
		backupPvPEnemies[0].Init(new PZMonster(enemyTwo, 1));
		activePlayer.Init(playerGoonies[0]);

		riggedAttacks = new Queue<int>();
		riggedAttacks.Enqueue(backupPvPEnemies[1].health + 7);
		riggedAttacks.Enqueue(34);

		boardMove.Sample(0, false);

		pvpUI.Reset();
		pvpMode = false;
		raidMode = false;

		//StartCoroutine(ScrollToNextEnemy());
	}


	public void SpawnNextPvp()
	{
		if (MSResourceManager.instance.Spend(ResourceType.CASH, pvpMatchCost, SpawnNextPvpWithGems))
	    {
			StartCoroutine(SpawnPvps(pvpMatchCost));
		}
	}

	void SpawnNextPvpWithGems()
	{
		int gemCost = Mathf.CeilToInt((pvpMatchCost - MSResourceManager.resources[ResourceType.CASH]) * MSWhiteboard.constants.gemsPerResource);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost))
		{
			StartCoroutine(SpawnPvps(MSResourceManager.instance.SpendAll(ResourceType.CASH), gemCost));
		}
	}

	IEnumerator SpawnPvps(int cash, int gems = 0)
	{
		pvpUI.Retract();

		SpendPvpMatchMoney(cash, gems);

		yield return null;
		yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed));
		Debug.Log("Finished player run");
		activePlayer.unit.animat = MSUnit.AnimationType.RUN;
		activePlayer.unit.direction = MSValues.Direction.EAST;
		background.StartScroll();

		yield return MSResourceManager.instance.RunCollectResources();

		int tagNum = 0;
		Coroutine waitForQueue = null;
		if (MSWhiteboard.loadedPvps == null || MSWhiteboard.loadedPvps.defenderInfoList.Count <= nextPvpDefenderIndex)
		{
			waitForQueue = StartCoroutine(QueueUpPvp());
		}

		Coroutine oneGuy = StartCoroutine(backupPvPEnemies[0].Retreat(-background.direction, background.scrollSpeed * pvpSpeedMux));
		Coroutine otherGuy = StartCoroutine(backupPvPEnemies[1].Retreat(-background.direction, background.scrollSpeed * pvpSpeedMux));
		yield return StartCoroutine((activeEnemy.Retreat(-background.direction, background.scrollSpeed * pvpSpeedMux)));

		yield return waitForQueue;
		yield return oneGuy;
		yield return otherGuy;
		
		Debug.Log("Finished retreat");

		//Init the monsters
		defender = MSWhiteboard.loadedPvps.defenderInfoList[nextPvpDefenderIndex++];

		activeEnemy.Init(defender.defenderMonsters[0]);
		if (defender.defenderMonsters.Count >= 2)
		{
			backupPvPEnemies[0].Init(defender.defenderMonsters[1]);
		}
		else
		{
			backupPvPEnemies[0].DeInit();
		}

		if (defender.defenderMonsters.Count >= 3)
		{
			backupPvPEnemies[1].Init(defender.defenderMonsters[2]);
		}
		else
		{
			backupPvPEnemies[1].DeInit();
		}

		while (!activeEnemy.unit.hasSprite || !backupPvPEnemies[0].unit.hasSprite || !backupPvPEnemies[1].unit.hasSprite)
		{
			yield return null;
		}
		yield return null;

		//Move the monsters out
		StartCoroutine(backupPvPEnemies[1].AdvanceTo(enemyXPos + 130, -background.direction, background.scrollSpeed * pvpSpeedMux));
		StartCoroutine(backupPvPEnemies[0].AdvanceTo(enemyXPos + 20, -background.direction, background.scrollSpeed * pvpSpeedMux));
		yield return StartCoroutine(activeEnemy.AdvanceTo(enemyXPos, -background.direction, background.scrollSpeed * pvpSpeedMux));

		background.StopScroll();
		activePlayer.unit.animat = MSUnit.AnimationType.IDLE;

		//Bring in the UI
		pvpUI.Init(defender);

	}

	public void StartPvp()
	{
		StartCoroutine(SendBeginPvpRequest());

		//StartCoroutine(RetreatPvpsForBattle());
		StartCoroutine(backupPvPEnemies[0].Retreat(-background.direction, background.scrollSpeed));
		StartCoroutine(backupPvPEnemies[1].Retreat(-background.direction, background.scrollSpeed));
		//Add the backup enemies to the enemy queue
		if (backupPvPEnemies[0].monster != null)
		{
			enemies.Enqueue(backupPvPEnemies[0].monster);
		}
		if (backupPvPEnemies[1].monster != null)
		{
			enemies.Enqueue(backupPvPEnemies[1].monster);
		}

		PZCombatScheduler.instance.Schedule(activePlayer.monster, activeEnemy.monster);
		turnDisplay.RunInit(activePlayer.monster, activeEnemy.monster);

		pvpUI.Retract();
		PZPuzzleManager.instance.InitBoard();

		enemySkillIndicator.Init(enemyDefSkill, activeEnemy.monster.monster.monsterElement);

		mobsterCounter.text = "ENEMY " + "1" + "/" + (enemies.Count + 1);

		StartCoroutine(TweenInPvp());
	}

	IEnumerator TweenInPvp()
	{
		TweenAlpha.Begin(mobsterCounter.transform.parent.gameObject, 1f ,1f);

		boardMove.Sample(0, false);
		boardMove.delay = 1f;
		boardMove.PlayForward();
		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.boardSlideIn);
		boardMove.delay = 0f;

		boardTint.Sample(1, false);
		//boardTint.PlayReverse();
		//PZPuzzleManager.instance.swapLock = 0;

		while (boardMove.tweenFactor < 1 || turnDisplay.moveInTween.tweenFactor < 1)
		{
			yield return null;
		}

		RunPickNextTurn(false);
	}

	IEnumerator SendBeginPvpRequest()
	{
		BeginPvpBattleRequestProto request = new BeginPvpBattleRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.senderElo = MSWhiteboard.localUser.pvpLeagueInfo.elo;
		request.attackStartTime = MSUtil.timeNowMillis;
		request.enemy = defender;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_PVP_BATTLE_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		BeginPvpBattleResponseProto response = UMQNetworkManager.responseDict[tagNum] as BeginPvpBattleResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != BeginPvpBattleResponseProto.BeginPvpBattleStatus.SUCCESS)
		{
			Debug.LogError("Problem beginning PVP: " + response.status.ToString());
		}
	}

	public void SpendPvpMatchMoney(int cash, int gems = 0)
	{
		UpdateUserCurrencyRequestProto request = new UpdateUserCurrencyRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.cashSpent = -cash;
		request.gemsSpent = -gems;
		request.reason = "pvp";

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UPDATE_USER_CURRENCY_EVENT, DealWithCurrencyResponse);
	}

	void DealWithCurrencyResponse(int tagNum)
	{
		UpdateUserCurrencyResponseProto response = UMQNetworkManager.responseDict[tagNum] as UpdateUserCurrencyResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != UpdateUserCurrencyResponseProto.UpdateUserCurrencyStatus.SUCCESS)
		{
			Debug.LogError("Problem changing user currency: " + response.status.ToString());
		}
		
	}

	void OnDeploy(PZMonster monster)
	{
		if (!MSTutorialManager.instance.inTutorial)
		{
			UntintBoard();
		}

		if (monster != activePlayer.monster)
		{
			Debug.Log ("Actually deploying");
			int rigTurn = 0;
			if (activePlayer.alive) rigTurn = -1;
			if (MSTutorialManager.instance.inTutorial) rigTurn = 1;

			if (!enemyCakeKid)
			{
				PZCombatScheduler.instance.Schedule(monster, activeEnemy.monster, rigTurn);
			}

			StartCoroutine(SwapCharacters(monster));

			playerSkillPoints = 0;
		}
		else
		{
			if (MSActionManager.Puzzle.ForceShowSwap != null)
			{
				MSActionManager.Puzzle.ForceShowSwap();
			}
		}
	}
	
	void OnEnemyDeath()
	{
		battleStats.monstersDefeated++;
		defeatedEnemies.Add(activeEnemy.monster);
		enemySkillIndicator.FadeOut();
		EnemySkillOnDeath();
		StartCoroutine(ScrollToNextEnemy());
	}
	
	void OnPlayerDeath()
	{
		Debug.Log("Lock: Player death");
		//PZPuzzleManager.instance.swapLock += 1;

		TintBoard();

		foreach (var goon in playerGoonies)
		{
			if (goon.currHP > 0)
			{
				//CBKEventManager.Popup.OnPopup(deployPopup.gameObject);
				deployPopup.Init(MSMonsterManager.instance.userTeam);
				return;
			}
		}

		ActivateLoseMenu (true);
	}

	public void RunPlayerForfeit()
	{
		StartCoroutine(OnPlayerForfeit());
	}

	IEnumerator OnPlayerForfeit(){
		bool forfeitSuccess = UnityEngine.Random.value <= forfeitChance;
		PZPuzzleManager.instance.swapLock++;
		//Debug.LogWarning("Forfeit Lock");
		Vector3 center = new Vector3((activeEnemy.transform.position.x + activePlayer.transform.position.x) / 2f,
		                             (activeEnemy.transform.position.y + activePlayer.transform.position.y) / 2f,
		                             activePlayer.transform.position.z);
		forfeit.SetParentPosition(center);
//		yield return StartCoroutine(forfeit.Animate (forfeitSuccess));
		if (forfeitSuccess) {
//			yield return StartCoroutine(activePlayer.Retreat(-background.direction, background.scrollSpeed));
			ActivateLoseMenu();
		} else {
			forfeitChance += FORFEIT_GAIN_RATE;
			if(forfeitChance > 1f)
			{
				forfeitChance = 1f;
			}
			yield return RunPickNextTurn(true);
		}
		PZPuzzleManager.instance.swapLock--;
		//Debug.LogWarning("Forfeit unlock");
	}

	public void ActivateLoseMenu(bool blackOut = false)
	{
		revives++;
		winLosePopup.gameObject.SetActive(true);
		winLosePopup.tweener.ResetToBeginning();
		winLosePopup.tweener.GetComponent<UITweener>().PlayForward();

		if(blackOut)
		{
			int cash = 0;
			int oil = 0;
			int xp = 0;
			List<MonsterProto> pieces = new List<MonsterProto>();
			List<ItemProto> items = new List<ItemProto>();
			if (pvpMode)
			{
				cash = defender.prospectiveCashWinnings;
				oil = defender.prospectiveOilWinnings;
			}
			else
			{
				foreach (var item in defeatedEnemies) 
				{
					cash += item.taskMonster.cashReward;
					xp += item.taskMonster.expReward;
					oil += item.taskMonster.oilReward;
					
					//if an enemy would have dropped an item and a capsule, it just drops an item instead
					if (item.taskMonster.itemId > 0){
						items.Add(MSDataManager.instance.Get<ItemProto>(item.taskMonster.itemId));
					}
					else if (item.taskMonster.puzzlePieceDropped)
					{
						pieces.Add( MSDataManager.instance.Get<MonsterProto>(item.monster.monsterId));
					}
				}
			}
			winLosePopup.InitBlackOut(xp, cash, oil, pieces, items);
		}
		else
		{
			winLosePopup.InitLose();
		}
		
		StartCoroutine(SendEndResult(false));
	}

	void OnRaidEnemyAttacked(AttackClanRaidMonsterResponseProto response)
	{
		if (raidMode && !response.sender.userUuid.Equals(MSWhiteboard.localMup.userUuid))
		{
			StartCoroutine(activeEnemy.TakeDamage(response.dmgDealt));
		}
	}

	void OnRaidEnemyDefeated(AttackClanRaidMonsterResponseProto response)
	{
		OnRaidEnemyAttacked(response);
	}

	/// <summary>
	/// Moves the current player character back to the start position, re-inits the character 
	/// as the new monster, and then moves it back into place. 
	/// </summary>
	/// <returns>The characters.</returns>
	IEnumerator SwapCharacters(PZMonster swapTo)
	{
		//PZPuzzleManager.instance.swapLock += 1;
		Debug.LogWarning("Swap Lock");

		yield return StartCoroutine(activePlayer.Retreat(-background.direction, background.scrollSpeed*3.5f));

		activePlayer.Init(swapTo);

		currTurn = 0;
		currPlayerDamage = 0;
		if (MSActionManager.Puzzle.OnTurnChange != null)
		{
			MSActionManager.Puzzle.OnTurnChange(playerTurns - currTurn);
		}
		
		CheckBleed(activePlayer);
		
		yield return null;

		yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed*3.5f));
		
		playerSkillIndicator.Init(playerOffSkill, swapTo.monster.monsterElement);

		yield return turnDisplay.RunInit(swapTo, activeEnemy.monster);

		if (activeEnemy.alive)
		{
			RunPickNextTurn(false);
		}
		else
		{
			RunScrollToNextEnemy();
		}
	}

	public Coroutine RunScrollToNextEnemy(bool fromLoad = false)
	{
		return StartCoroutine(ScrollToNextEnemy(fromLoad));
	}

	/// <summary>
	/// Spawns a new enemy and scrolls the background until
	/// that enemy is in its place.
	/// </summary>
	IEnumerator ScrollToNextEnemy(bool fromLoad = false)
	{
		Debug.Log("Start scroll");

		turnDisplay.DoMoveOut();

		TintBoard();

		//PZPuzzleManager.instance.swapLock += 1;
		Debug.LogWarning("Scoll to next enemy Lock");

		while(!activePlayer.unit.hasSprite)
		{
			yield return null;
		}

		yield return StartCoroutine (activePlayer.AdvanceTo (playerXPos, -background.direction, background.scrollSpeed, false));
		activePlayer.unit.direction = MSValues.Direction.EAST;

		MSSoundManager.instance.Loop (MSSoundManager.instance.walking);

		yield return StartCoroutine (activePlayer.AdvanceTo (playerXPos, -background.direction, background.scrollSpeed, false));

		while (waiting) {
			background.Scroll ();
			yield return null;
		}

		if (MSTutorialManager.instance.inTutorial)
		{

			if (MSTutorialManager.instance.firstCombat)
			{
				background.StartScroll();

				StartCoroutine(backupPvPEnemies[1].AdvanceTo(enemyXPos + 130, -background.direction, background.scrollSpeed * 1.5f));
				StartCoroutine(backupPvPEnemies[0].AdvanceTo(enemyXPos + 20, -background.direction, background.scrollSpeed * 1.5f));
				yield return StartCoroutine(activeEnemy.AdvanceTo(enemyXPos, -background.direction, background.scrollSpeed * 1.5f));
				
				background.StopScroll();
				MSTutorialManager.instance.firstCombat = false;
			}
			else
			{
				while (activeEnemy.transform.localPosition.x > enemyXPos)
				{
					background.Scroll(new MSUnit[] {activeEnemy.unit, backupPvPEnemies[0].unit});
					yield return null;
				}
			}

			//turnDisplay.Init(activePlayer.monster, activeEnemy.monster);

			PZCombatScheduler.instance.Schedule(1, 1, 1);

			activePlayer.unit.animat = MSUnit.AnimationType.IDLE;
			
			MSSoundManager.instance.StopLoop();

			yield break;
		}
		else if (enemies.Count > 0) 
		{
			enemySkillPoints = 0;

			activeEnemy.OnClick();

			activeEnemy.GoToStartPos ();
			activeEnemy.Init (enemies.Dequeue ());

			if (enemyDefSkill != null &&  enemyDefSkill.type == SkillType.CAKE_DROP)
			{
				activeEnemy.monster.speed = enemyDefSkill.properties.Find(x=>x.name=="INITIAL_SPEED").skillValue;
			}

			while (!activeEnemy.unit.hasSprite)
			{
				background.Scroll();
				yield return null;
			}

			if (fromLoad)
			{
				PZCombatScheduler.instance.player = activePlayer.monster;
				PZCombatScheduler.instance.enemy = activeEnemy.monster;

				if (enemyDefSkill != null)
				{
					switch (enemyDefSkill.type)
					{
					case SkillType.BOMBS:
						EnemySetupBombs();
						break;
					case SkillType.CAKE_DROP:
						PZPuzzleManager.instance.SetupForCakes(enemyDefSkill);
						activeEnemy.monster.speed = save.enemySpeed;
						break;
					}
				}
				
				if (save.enemySkillSave.skillActive && enemyDefSkill != null)
				{
					switch(enemyDefSkill.type)
					{
					case SkillType.SHIELD:
						activeEnemy.shieldHealth = save.enemySkillSave.shieldHealth;
						activeEnemy.tweenShield.BringIn();
						break;
					case SkillType.ROID_RAGE:
						StartCoroutine(GainRage (activeEnemy, enemyDefSkill));
						break;
					case SkillType.MOMENTUM:
						activeEnemy.damageMultiplier = save.enemySkillSave.damageMultiplier;
						break;
					}
				}
			}
			else
			{
				PZCombatScheduler.instance.Schedule(activePlayer.monster, activeEnemy.monster, enemyCakeKid ? 1 : 0);
			}

			if (!pvpMode)
			{
				UpdateUserTaskStage(MSWhiteboard.currTaskStages.Find(x=>x.stageMonsters[0] == activeEnemy.monster.taskMonster).stageId);
			}

			activeEnemy.unit.direction = MSValues.Direction.WEST;
			activeEnemy.unit.animat = MSUnit.AnimationType.IDLE;

			if (savedHealth >= 0)
			{
				activeEnemy.health = savedHealth;
				savedHealth = -1;
			}

			//Save();
			int currEnemyNumber = totalEnemies - enemies.Count;

			mobsterCounter.text = "ENEMY " + currEnemyNumber + "/" + totalEnemies;
			mobsterCounter.MakePixelPerfect();

			intro.Init (activeEnemy.monster, currEnemyNumber, totalEnemies);
			intro.PlayAnimation ();
		} 
		else if (!activeEnemy.alive) 
		{
			activeEnemy.GoToStartPos ();
			StartCoroutine (SendEndResult (true));

			if (!pvpMode && !raidMode) 
			{
				if (MSActionManager.Quest.OnTaskCompleted != null) 
				{
					MSActionManager.Quest.OnTaskCompleted ();
				}
			}
		}

		activePlayer.unit.animat = MSUnit.AnimationType.RUN;

		if (!activeEnemy.alive) 
		{
			StartCoroutine(DelayedWinLosePopup(2f));
		}

		while(activeEnemy.unit.transf.localPosition.x > enemyXPos)
		{
			background.Scroll(activeEnemy.unit);
			yield return null;
		}

		enemySkillIndicator.Init(enemyDefSkill, activeEnemy.monster.monster.monsterElement);
		enemySkillIndicator.SetPoints(0);

		activePlayer.unit.animat = MSUnit.AnimationType.IDLE;

		if (activeEnemy.alive && activePlayer.alive)
		{
			if (!fromLoad)
			{
				yield return StartCoroutine(EnemySkillOnBegin());
			}

			yield return turnDisplay.RunInit(activePlayer.monster, activeEnemy.monster);
		}

		MSSoundManager.instance.StopLoop();

		if (!fromLoad && activeEnemy.alive && activePlayer.alive)
		{
			RunPickNextTurn(false);
		}

		PZPuzzleManager.instance.swapLock = activeEnemy.alive ? 0 : 1;
		//Debug.LogWarning("Lock/Unlock: Enemy Alive after scroll");
	}

	public Coroutine RunPickNextTurn(bool shiftTurnDisplay)
	{
		return StartCoroutine(PickNextTurn(shiftTurnDisplay));
	}

	IEnumerator PickNextTurn(bool shiftTurnDisplay)
	{
		if (activePlayer.alive && activeEnemy.alive)
		{
			if (shiftTurnDisplay)
			{
				yield return turnDisplay.RunOnNextTurn();
			}
			switch (PZCombatScheduler.instance.GetNextMove())
			{
			case CombatTurn.PLAYER:
				StartPlayerTurn();
				break;
			case CombatTurn.ENEMY:
				StartCoroutine(EnemyAttack());
				break;
			}
		}
		else if (MSTutorialManager.instance.inTutorial)
		{
			MSTutorialManager.instance.TurnHappen(3);
		}

		if (!MSTutorialManager.instance.inTutorial)
		{
			//Save ();
		}
	}

	void StartPlayerTurn()
	{
		if (currTurn == playerTurns) currTurn = 0;

		if (MSActionManager.Puzzle.OnTurnChange != null)
		{
			MSActionManager.Puzzle.OnTurnChange(playerTurns-currTurn);
		}
		if (MSActionManager.Puzzle.OnNewPlayerRound != null)
		{
			MSActionManager.Puzzle.OnNewPlayerRound();
		}

		if (playerOffSkill != null && playerOffSkill.type == SkillType.MOMENTUM)
		{
			StartCoroutine(GainMomentum(activePlayer, playerOffSkill));
		}
		
		if (MSActionManager.Puzzle.ForceShowSwap != null)
		{
			MSActionManager.Puzzle.ForceShowSwap();
		}

		if (!MSTutorialManager.instance.inTutorial)
		{
			UntintBoard();
		}

		PZPuzzleManager.instance.swapLock = 0;
		//Debug.LogWarning("Start Player Turn Unlock");
	}

	IEnumerator DelayedWinLosePopup(float seconds){

		yield return new WaitForSeconds (seconds);

		boardMove.PlayReverse ();
		winLosePopup.gameObject.SetActive(true);
		
		GetRewards();
		winLosePopup.tweener.ResetToBeginning();
		winLosePopup.tweener.PlayForward();
	}

	void GetRewards()
	{
		int cash = 0;
		int oil = 0;
		int xp = 0;
		List<MonsterProto> pieces = new List<MonsterProto>();
		List<ItemProto> items = new List<ItemProto>();
		if (pvpMode)
		{
			cash = defender.prospectiveCashWinnings;
			oil = defender.prospectiveOilWinnings;
		}
		else 
		{
			if (firstTimeUserWonTask)
			{
				foreach (TaskMapElementProto elem in MSDataManager.instance.GetAll<TaskMapElementProto>().Values)
				{
					if (elem.taskId == MSWhiteboard.currTaskId)
					{
						cash = elem.cashReward;
						oil = elem.oilReward;
					}
				}
			}
			foreach (var item in defeatedEnemies) 
			{
				xp += item.taskMonster.expReward;

				//if an enemy would have dropped an item and a capsule, it just drops an item instead
				if (item.taskMonster.itemId > 0){
					items.Add(MSDataManager.instance.Get<ItemProto>(item.taskMonster.itemId));
				}
				else if (item.taskMonster.puzzlePieceDropped)
				{
					pieces.Add(item.monster);
				}
			}
		}
		winLosePopup.InitWin(xp, cash, oil, pieces, items);
		MSResourceManager.instance.Collect(ResourceType.CASH, cash);
		MSResourceManager.instance.Collect (ResourceType.OIL, oil);
		MSResourceManager.instance.GainExp(xp);
	}
	
	IEnumerator SendEndResult(bool userWon)
	{
#if UNITY_ANDROID || UNITY_IPHONE
		//Kamcord.StopRecording();
#endif

		if (MSActionManager.Quest.OnBattleFinish != null)
		{
			MSActionManager.Quest.OnBattleFinish(battleStats);
		}

		if (pvpMode)
		{
			EndPvpBattleRequestProto request = new EndPvpBattleRequestProto();
			request.sender = MSWhiteboard.localMupWithResources;
			request.defenderUuid = defender.defender.minUserProto.userUuid;

			request.userAttacked = true;
			request.userWon = userWon;
			if (userWon)
			{
				request.oilChange = defender.prospectiveOilWinnings;
				request.cashChange = defender.prospectiveCashWinnings;

				MSResourceManager.instance.Collect(ResourceType.CASH, defender.prospectiveCashWinnings);
				MSResourceManager.instance.Collect (ResourceType.OIL, defender.prospectiveOilWinnings);

				if (MSActionManager.Pvp.OnPvpVictory != null)
				{
					MSActionManager.Pvp.OnPvpVictory(defender.prospectiveCashWinnings, defender.prospectiveOilWinnings);
				}
			}
			request.clientTime = MSUtil.timeNowMillis;

			int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_END_PVP_BATTLE_EVENT, null);

			while (!UMQNetworkManager.responseDict.ContainsKey (tagNum))
			{
				yield return null;
			}

			EndPvpBattleResponseProto response = UMQNetworkManager.responseDict[tagNum] as EndPvpBattleResponseProto;
			UMQNetworkManager.responseDict.Remove(tagNum);

			if (response.status != EndPvpBattleResponseProto.EndPvpBattleStatus.SUCCESS)
			{
				Debug.LogError("Problem ending PVP: " + response.status.ToString());
			}

		}
		else if (raidMode)
		{

		}
		else
		{
			EndDungeonRequestProto request = new EndDungeonRequestProto();
			request.sender = MSWhiteboard.localMupWithResources;
			request.userTaskUuid = MSWhiteboard.currUserTaskUuid;
			request.userWon = userWon;
			request.clientTime = MSUtil.timeNowMillis;

			if (userWon && !MSQuestManager.instance.taskDict.ContainsKey(MSWhiteboard.currTaskId))
			{
				int task = MSWhiteboard.currTaskId;
				MSQuestManager.instance.taskDict[task] = true;
				request.firstTimeUserWonTask = firstTimeUserWonTask = true;
			}
			else
			{
				firstTimeUserWonTask = false;
			}
			
			int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_END_DUNGEON_EVENT, null);
			
			while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
			{
				yield return null;
			}
			
			EndDungeonResponseProto response = UMQNetworkManager.responseDict[tagNum] as EndDungeonResponseProto;
			UMQNetworkManager.responseDict.Remove(tagNum);
			
			if (response.status == EndDungeonResponseProto.EndDungeonStatus.SUCCESS)
			{
				MSMonsterManager.instance.UpdateOrAddAll(response.updatedOrNew);
			}
			else
			{
				Debug.LogError("Problem in End Dungeon: " + response.status.ToString());
			}
		}
	}
	
	/// <summary>
	/// Called at the end of every full player turn
	/// Passes the gems and combo to the player's unit
	/// </summary>
	/// <param name='gemsBroken'>
	/// Gems broken.
	/// </param>
	/// <param name='combo'>
	/// Combo.
	/// </param>
	public void OnBreakGems(int[] gemsBroken, int combo)
	{
		for (int i = 0; i < gemsBroken.Length; i++) 
		{
			PZCombatManager.instance.battleStats.orbs[i] += gemsBroken[i];
		}

		PZCombatManager.instance.battleStats.combos += combo;

		int damage;
		Element element;
		activePlayer.DealDamage(gemsBroken, out damage, out element);

		currPlayerDamage += damage;

		++currTurn;

		if (MSActionManager.Puzzle.OnTurnChange != null)
		{
			MSActionManager.Puzzle.OnTurnChange(playerTurns - currTurn);
		}

		StartCoroutine(AfterBreakGems());
	}

	IEnumerator AfterBreakGems()
	{
		if (playerSkillReady && playerOffSkill.activationType == SkillActivationType.AUTO_ACTIVATED)
		{
			yield return RunPlayerSkill();
		}

		yield return StartCoroutine(EnemySkillAfterPlayerTurn());

		if (currTurn == playerTurns)
		{
			PlayerAttack();
			currPlayerDamage = 0;
			currTurn = 0;
		}
	}

	public void PlayerAttack()
	{
		StartCoroutine(PlayerAttackAnimationSequence(currPlayerDamage, activePlayer.monster.monster.monsterElement));
	}

	IEnumerator ShowAttackWords(float score)
	{
		screenTint.PlayForward();
		
		attackWords.gameObject.SetActive(true);

		if (score > MAKE_IT_RAIN_SCORE)
		{
			attackWords.MakeItRain();
			attackWords.GetComponent<PZRainbow>().Play();
			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.makeItRain);
		}
		else
		{
			if (score > HAMMERTIME_SCORE)
			{
				attackWords.HammerTime();
			}
			else if (score > CANT_TOUCH_THIS_SCORE)
			{
				attackWords.CantTouchThis();
			}
			else if (score > BALLIN_SCORE)
			{
				attackWords.Ballin();
			}
		}

		//commenting this out when we switch to labels, not sure of the purpose
//		UISpriteData data = attackWords.GetAtlasSprite();
//		attackWords.width = data.width;
//		attackWords.height = data.height;
		
		attackWordsTweenPos.Sample(0, false);
		attackWordsTweenPos.PlayForward();

		while(attackWordsTweenPos.tweenFactor < 1)
		{
			yield return null;
		}

		attackWords.gameObject.SetActive(false);

		screenTint.PlayReverse();
		
		if (score > MAKE_IT_RAIN_SCORE)
		{
			MakeItRain();
		}

	}
	
	void MakeItRain()
	{
		Transform plane = (MSPoolManager.instance.Get(MSPrefabList.instance.planePrefab, Vector3.zero) as MonoBehaviour).transform;
		plane.parent = combatParent;
		plane.localPosition = activePlayer.startingPos;
		plane.localScale = Vector3.one;

		for (int i = 0; i < NUM_BOMBS; i++) 
		{
			PZBomb bomb = BombAt(activeEnemy.unit.transf.localPosition.x - BOMB_SPACING * NUM_BOMBS / 2f + BOMB_SPACING * i, plane);
			if(i == 0)
			{
				bomb.camera = camera;
			}
			lastBomb = bomb;
		}
	}

	PZBomb BombAt(float x, Transform plane)
	{
		Transform bomb = (MSPoolManager.instance.Get(MSPrefabList.instance.bombPrefab, Vector3.zero, combatParent) as MonoBehaviour).transform;
		bomb.localPosition = new Vector3(x, Screen.height/2 + BOMB_SPACING);
		bomb.localScale = Vector3.one;
		bomb.GetComponent<PZBomb>().targetHeight = activeEnemy.unit.transf.localPosition.y + 
			(background.direction.y / background.direction.x) * (x - activeEnemy.unit.transf.localPosition.x);
		bomb.GetComponent<PZBomb> ().planeTrans = plane;
		return bomb.GetComponent<PZBomb>();
	}

	Coroutine TintBoard()
	{
		return StartCoroutine(DoTintBoard());
	}

	IEnumerator DoTintBoard()
	{
		boardTint.gameObject.SetActive(true);
		boardTint.PlayForward();
		while (boardTint.tweenFactor < 1) yield return null;
	}

	Coroutine UntintBoard()
	{
		return StartCoroutine(DoUntintBoard());
	}

	IEnumerator DoUntintBoard()
	{
		boardTint.PlayReverse();
		while(boardTint.tweenFactor > 0) yield return null;
		boardTint.gameObject.SetActive(false);
	}

	#region Blood

	void CheckBleed(PZCombatUnit player)
	{
		float perc = ((float)player.monster.currHP) / player.monster.maxHP;
		if (perc < BLEED_CONT_THRESH && perc > 0)
		{
			Bleed();
		}
		else if (perc < BLEED_ONCE_THRESH)
		{
			StartCoroutine(BleedOnce());
		}
		else
		{
			StopBleeding();
		}
	}

	IEnumerator BleedOnce()
	{
		bloodSplatter.from = 0;
		bloodSplatter.to = 1;
		bloodSplatter.duration = .5f;
		bloodSplatter.style = UITweener.Style.Once;
		bloodSplatter.ResetToBeginning();
		bloodSplatter.PlayForward();
		while (bloodSplatter.value < 1)
		{
			yield return null;
		}
		bloodSplatter.PlayReverse();
	}

	void Bleed()
	{
		bloodSplatter.from = .5f;
		bloodSplatter.to = 1;
		bloodSplatter.duration = 1;
		bloodSplatter.style = UITweener.Style.PingPong;
		bloodSplatter.ResetToBeginning();
		bloodSplatter.PlayForward();
	}

	void StopBleeding()
	{
		bloodSplatter.ResetToBeginning();
		bloodSplatter.from = 0;
		bloodSplatter.to = 0;
		bloodSplatter.value = 0;
	}

	#endregion

	#region Skills

	public void AddPlayerSkillPoint(int points = 1)
	{
		if (playerOffSkill != null
		    && playerOffSkill.skillId > 0
		    && playerOffSkill.activationType != SkillActivationType.PASSIVE)
		{
			playerSkillPoints += points;
			playerSkillIndicator.SetPoints(playerSkillPoints);
		}
	}

	public void AddEnemySkillPoint(int points = 1)
	{
		if (enemyDefSkill != null
		    && enemyDefSkill.skillId > 0
		    && (enemyDefSkill.activationType != SkillActivationType.PASSIVE || enemyDefSkill.type == SkillType.POISON))
		{
			enemySkillPoints += points;
			enemySkillIndicator.SetPoints(enemySkillPoints);
		}
	}

	public Coroutine RunPlayerSkill()
	{
		return StartCoroutine(PlayerSkill());
	}

	IEnumerator PlayerSkill()
	{
		PZPuzzleManager.instance.swapLock++;
		//Debug.LogWarning("Player Skill Lock");

		playerSkillPoints = 0;
		playerSkillIndicator.SetPoints(playerSkillPoints);

		if (playerOffSkill != null
		    && playerOffSkill.skillId > 0)
		{

			switch (playerOffSkill.type)
			{
			case SkillType.QUICK_ATTACK:
				yield return playerSkillAnimator.Animate(activePlayer.monster.monster, playerOffSkill);
				yield return StartCoroutine(PlayerQuickAttack((int)playerOffSkill.properties.Find(x=>x.name == "DAMAGE").skillValue));
				break;
			case SkillType.SHIELD:
				activePlayer.shieldHealth = (int)playerOffSkill.properties.Find(x=>x.name == "SHIELD_HP").skillValue;
				Coroutine skillAnimate = playerSkillAnimator.Animate(activePlayer.monster.monster, playerOffSkill);
				activePlayer.shieldLabel.Set(activePlayer.shieldHealth);
				yield return activePlayer.tweenShield.BringIn();
				yield return skillAnimate;
				break;
			case SkillType.ROID_RAGE:
				yield return playerSkillAnimator.Animate(activePlayer.monster.monster, playerOffSkill);
				StartCoroutine(GainRage(activePlayer, playerOffSkill));
				break;
			}
		}
		
		PZPuzzleManager.instance.swapLock--;
		//Debug.LogWarning("Player Skill Unlock");
	}

	IEnumerator PlayerQuickAttack(int damage)
	{
		PZPuzzleManager.instance.swapLock++;
		//Debug.LogWarning("Quick Attack Lock");

		MSAnimationEvents.curDamage = damage;
		MSAnimationEvents.curElement = Element.NO_ELEMENT;

		quickAttack = true;
		StartCoroutine(PlayerShoot(damage/activePlayer.monster.totalDamage));
		while (quickAttack)
		{
			yield return null;
		}
		PZPuzzleManager.instance.swapLock--;
		//Debug.LogWarning("Quick Attack Unlock");
	}

	IEnumerator EnemyQuickAttack(int damage)
	{
		yield return enemySkillAnimator.Animate(activeEnemy.monster.monster, enemyDefSkill);
		yield return StartCoroutine(EnemyShoot(damage, Element.NO_ELEMENT));
	}

	void EnemySetupBombs()
	{
		PZPuzzleManager.instance.canSpawnBombs = true;
		PZPuzzleManager.instance.maxBombs = (int)enemyDefSkill.properties.Find(x=>x.name == "MAX_BOMBS").skillValue;
		PZPuzzleManager.instance.minBombs = (int)enemyDefSkill.properties.Find(x=>x.name == "MIN_BOMBS").skillValue;
		PZPuzzleManager.instance.bombTicks = (int)enemyDefSkill.properties.Find(x=>x.name == "BOMB_COUNTER").skillValue;
		PZPuzzleManager.instance.bombChance = enemyDefSkill.properties.Find(x=>x.name == "BOMB_CHANCE").skillValue;
		PZPuzzleManager.instance.bombColor = ((int)activeEnemy.monster.monster.monsterElement) - 1;
		PZPuzzleManager.instance.bombDamage = (int)enemyDefSkill.properties.Find (x=>x.name == "BOMB_DAMAGE").skillValue;
	}

	IEnumerator EnemyTickBombs()
	{
		int numBombs = 0;
		int bombDamage = 0;
		for (int i = 0; i < bombs.Count; i++)
		{
			PZGem bomb = bombs[i];
			if (bomb.BombTick())
			{
				bombs.RemoveAt(i);
				i--;
				numBombs++;
				bombDamage += bomb.bombDamage;
				bomb.BombDetonate();
			}
		}
		if (numBombs > 0)
		{
			activePlayer.SendDamageUpdateToServer(bombDamage);
			yield return activeEnemy.unit.DoJump(50, .35f);
			yield return activeEnemy.unit.DoJump(50, .35f);
			yield return TintBoard();
			yield return StartCoroutine(EnemyDropBombsAnimation(numBombs, bombDamage));
			yield return StartCoroutine(activePlayer.TakeDamage(bombDamage, false));
			yield return UntintBoard();
			activePlayer.unit.animat = MSUnit.AnimationType.IDLE;
		}
	}
	
	IEnumerator EnemyDropBombsAnimation(int bombs, int damage)
	{
		PZBomb dropBomb = null;
		for (int i = 0; i < bombs; i++) 
		{
			dropBomb = BombAt(activePlayer.transform.localPosition.x + UnityEngine.Random.value * bombSkillAnimationXRangeForBombs*2 - bombSkillAnimationXRangeForBombs, null);
			if (i==0)
			{
				StartCoroutine(EnemyDropSingleBombAnimation(dropBomb, delegate { StartCoroutine(PlayerFlinchMoveBackwards()); }));
			}
			else
			{
				StartCoroutine(EnemyDropSingleBombAnimation(dropBomb));
			}
			yield return new WaitForSeconds(bombSkillAnimationTimeBetweenBombs);
		}

		if (dropBomb != null)
		{
			while (dropBomb.falling)
			{
				yield return null;
			}
		}

		yield return new WaitForSeconds(recoilTime);

		yield return StartCoroutine(PlayerReturnAfterFlinch());
	}

	IEnumerator EnemyDropSingleBombAnimation(PZBomb bomb, Action after = null)
	{
		yield return bomb.RunFall();
		if (after != null)
		{
			after();
		}
	}

	IEnumerator PoisonDamageAnimation(int numGems)
	{
		activePlayer.SendDamageUpdateToServer(numGems * poisonDamage);

		yield return enemySkillAnimator.AnimateDefensive(activeEnemy);

		//Tween in icon
		activePlayer.poisonIconScale.gameObject.SetActive(true);
		activePlayer.poisonIconScale.PlayForward();
		while (activePlayer.poisonIconScale.tweenFactor < 1) yield return null;

		activePlayer.TweenSpriteColor(Color.red, recoilTime);
		Coroutine tick = StartCoroutine(activePlayer.TakeDamage(numGems * poisonDamage));

		yield return StartCoroutine(PlayerFlinchMoveBackwards());
		yield return new WaitForSeconds(recoilTime);
		activePlayer.TweenSpriteColor(Color.white, recoilTime);
		activePlayer.poisonIconScale.PlayReverse();
		yield return StartCoroutine(PlayerReturnAfterFlinch());
		yield return tick;
		while (activePlayer.poisonIconScale.tweenFactor > 0) yield return null;
		activePlayer.poisonIconScale.gameObject.SetActive(false);
	}

	IEnumerator GainMomentum(PZCombatUnit unit, SkillProto skill)
	{
		unit.damageMultiplier += (skill.properties.Find (x=>x.name == "DAMAGE_MULTIPLIER").skillValue-1); //If we want the damage multipliers to stack *properly*, we have to do this stupid math -_-
		unit.spriteScale.Sample (0, true);
		float scale = skill.properties.Find(x=>x.name == "SIZE_MULTIPLIER").skillValue;
		unit.spriteScale.to = new Vector3(scale, scale, scale);
		unit.spriteScale.PlayForward();
		while (unit.spriteScale.tweenFactor < 1) yield return null;
		unit.spriteScale.PlayReverse();
		while (unit.spriteScale.tweenFactor > 0) yield return null;
	}

	IEnumerator GainRage(PZCombatUnit unit, SkillProto skill)
	{
		float damageBonus = (skill.properties.Find (x=>x.name == "DAMAGE_MULTIPLIER").skillValue-1);
		float scale = skill.properties.Find(x=>x.name == "SIZE_MULTIPLIER").skillValue;
		yield return StartCoroutine(GainRage(unit, damageBonus, scale));
	}

	IEnumerator GainRage(PZCombatUnit unit, float damageBonus, float scale)
	{
		unit.roiding = true;
		unit.damageMultiplier += damageBonus; //If we want the damage multipliers to stack *properly*, we have to do this stupid math -_-
		unit.spriteScale.Sample (0, true);
		unit.spriteScale.to = new Vector3(scale, scale, scale);
		unit.spriteScale.PlayForward();
		while (unit.spriteScale.tweenFactor < 1) yield return null;
		while(unit.roiding)
		{
			yield return unit.TweenSpriteColor(roidColor, .5f);
			yield return unit.TweenSpriteColor(Color.white, .5f);
		}
		unit.damageMultiplier -= damageBonus;
		unit.spriteScale.PlayReverse();
	}

	IEnumerator EnemySkillBeforeEnemyTurn()
	{
		if (
			enemyDefSkill != null
			&& enemyDefSkill.skillId > 0
		    )
		{
			switch (enemyDefSkill.type)
			{
			case SkillType.JELLY:
				enemySkillPoints++;
				if (enemySkillPoints >= enemyDefSkill.properties.Find(x=>x.name == "SPAWN_TURNS").skillValue)
				{
					yield return StartCoroutine(EnemyDoSkill());
				}
				break;
			case SkillType.MOMENTUM:
				yield return StartCoroutine(EnemyDoSkill());
				break;
			}
		}
	}

	/// <summary>
	/// Actives the enemy's ongoing skill.
	/// NOT for Initial effects (Initial jellies, bombs, etc.)
	/// PRECONDITION: Enemy has enough skill points to use ability.
	/// </summary>
	/// <returns>The do skill.</returns>
	IEnumerator EnemyDoSkill()
	{
		PZPuzzleManager.instance.swapLock++;
		//Debug.LogWarning("Enemy Skill Lock");
		
		yield return activeEnemy.unit.DoJump(50, .35f);
		yield return activeEnemy.unit.DoJump(50, .35f);
		yield return TintBoard();

		switch (enemyDefSkill.type)
		{
		case SkillType.QUICK_ATTACK:
			yield return StartCoroutine(EnemyQuickAttack((int)enemyDefSkill.properties.Find(x=>x.name == "DAMAGE").skillValue));
			break;
		case SkillType.ROID_RAGE:
			yield return enemySkillAnimator.Animate(activeEnemy.monster.monster, enemyDefSkill);
			StartCoroutine(GainRage(activeEnemy, enemyDefSkill));
			break;
		case SkillType.POISON:
			yield return enemySkillAnimator.Animate(activeEnemy.monster.monster, enemyDefSkill);
			yield return StartCoroutine(PoisonDamageAnimation(enemySkillPoints));
			break;
		case SkillType.MOMENTUM:
			yield return StartCoroutine(GainMomentum(activeEnemy, enemyDefSkill));
			break;
		case SkillType.SHIELD:
			activeEnemy.shieldHealth = (int)enemyDefSkill.properties.Find(x=>x.name=="SHIELD_HP").skillValue;
			Coroutine skillAnimate = enemySkillAnimator.Animate(activeEnemy.monster.monster, enemyDefSkill);
			activeEnemy.shieldLabel.Set(activeEnemy.shieldHealth);
			yield return activeEnemy.tweenShield.BringIn();
			yield return skillAnimate;
			break;
		case SkillType.JELLY:
			boardTint.gameObject.SetActive(false);
			List<Vector2> spaces = PZPuzzleManager.instance.SpawnJellies((int)enemyDefSkill.properties.Find(x=>x.name == "SPAWN_COUNT").skillValue);
			PZPuzzleManager.instance.BlockBoard(spaces);
			yield return new WaitForSeconds(1);
			PZPuzzleManager.instance.UnblockBoard();
			boardTint.gameObject.SetActive(true);
			break;
		}

		if (currTurn < playerTurns)
		{
			yield return UntintBoard();
		}
		
		enemySkillPoints = 0;
		enemySkillIndicator.SetPoints(0);

		PZPuzzleManager.instance.swapLock--;
		//Debug.LogWarning("Enemy Skill Unlock");
	}

	IEnumerator EnemySkillAfterPlayerTurn()
	{
		if (enemyDefSkill != null)
		{
			if (enemyDefSkill.activationType == SkillActivationType.AUTO_ACTIVATED
			    && enemySkillPoints >= enemyDefSkill.orbCost)
			{
				yield return StartCoroutine(EnemyDoSkill());
			}
			else if (enemyDefSkill.type == SkillType.POISON
			    && enemySkillPoints > 0)
			{
				yield return StartCoroutine(EnemyDoSkill());
			}
		}

		//This check is separate, since bombs can stay on the board for future combats
		if (bombs.Count > 0)
		{
			yield return StartCoroutine (EnemyTickBombs());
		}
	}

	IEnumerator EnemySkillOnBegin()
	{
		if (enemyDefSkill != null
		    && enemyDefSkill.skillId > 0
		    && enemyDefSkill.activationType == SkillActivationType.PASSIVE)
		{
			yield return activeEnemy.unit.DoJump(50, .35f);
			yield return activeEnemy.unit.DoJump(50, .35f);

			enemySkillAnimator.Animate(activeEnemy.monster.monster, enemyDefSkill);

			switch (enemyDefSkill.type)
			{
			case SkillType.JELLY:
				yield return TintBoard();
				boardTint.gameObject.SetActive(false);
				List<Vector2> spaces = PZPuzzleManager.instance.SpawnJellies((int)enemyDefSkill.properties.Find(x=>x.name == "INITIAL_COUNT").skillValue);
				PZPuzzleManager.instance.BlockBoard(spaces);
				yield return new WaitForSeconds(1);
				PZPuzzleManager.instance.UnblockBoard();
				boardTint.gameObject.SetActive(true);
				//yield return UntintBoard();
				break;
			case SkillType.CAKE_DROP:
				yield return TintBoard();
				PZPuzzleManager.instance.SetupForCakes(enemyDefSkill);
				boardTint.gameObject.SetActive(false);
				PZPuzzleManager.instance.BlockBoard();
				yield return PZPuzzleManager.instance.BakeCake();
				PZPuzzleManager.instance.cakes[0].Block(.3f);
				yield return new WaitForSeconds(.3f);
				PZPuzzleManager.instance.UnblockBoard();
				boardTint.gameObject.SetActive(true);
				yield return new WaitForSeconds(.8f);
				//yield return UntintBoard();
				break;
			case SkillType.BOMBS:
				yield return TintBoard();
				EnemySetupBombs();
				boardTint.gameObject.SetActive(false);
				List<PZGem> gemsToTurnBombs = PZPuzzleManager.instance.PickBombs((int)activeEnemy.monster.monster.monsterElement-1, ((int)enemyDefSkill.properties.Find(x=>x.name == "INITIAL_BOMBS").skillValue) - bombs.Count);
				foreach (var gem in gemsToTurnBombs) 
				{
					gem.MakeBomb();
					bombs.Add(gem);
				}
				PZPuzzleManager.instance.BlockBoard (gemsToTurnBombs);
				yield return new WaitForSeconds(.5f);
				foreach (var gem in gemsToTurnBombs) 
				{
					gem.Block(.3f);
				}
				yield return new WaitForSeconds(.3f);
				PZPuzzleManager.instance.UnblockBoard();
				boardTint.gameObject.SetActive(true);
				yield return new WaitForSeconds(.8f);
				break;
			case SkillType.POISON:
				PZPuzzleManager.instance.poisonColor = (int)activeEnemy.monster.monster.monsterElement-1;
				yield return TintBoard();
				poisonDamage = (int)enemyDefSkill.properties.Find(x=>x.name == "ORB_DAMAGE").skillValue;
				List<PZGem> gemsToTurnPoison = PZPuzzleManager.instance.PickPoisons((int)activeEnemy.monster.monster.monsterElement-1);
				foreach (var gem in gemsToTurnPoison) 
				{
					gem.MakePoison();
				}
				PZPuzzleManager.instance.BlockBoard(gemsToTurnPoison);
				yield return new WaitForSeconds(.5f);
				foreach (var gem in gemsToTurnPoison) 
				{
					gem.Block(.3f);
				}
				yield return new WaitForSeconds(.3f);
				PZPuzzleManager.instance.UnblockBoard();
				boardTint.gameObject.SetActive(true);
				yield return new WaitForSeconds(.8f);
				break;
			}
		}
	}

	void EnemySkillOnDeath()
	{
		if (enemyDefSkill != null)
		{
			switch (enemyDefSkill.type)
			{
			case SkillType.CAKE_DROP:
				PZPuzzleManager.instance.ResetCakes();
				break;
			case SkillType.BOMBS:
				PZPuzzleManager.instance.canSpawnBombs = false;
				break;
			case SkillType.POISON:
				foreach (var gem in PZPuzzleManager.instance.board) 
				{
					if (gem.gemType == PZGem.GemType.POISON)
					{
						gem.RevertFromPoison();
					}
				}
				PZPuzzleManager.instance.poisonColor = -1;
				break;
			}
		}
	}

	#endregion

	#region Attacks & Animations

	IEnumerator PlayerShoot(float score)
	{
		enemyStartPosition =  activeEnemy.unit.transf.localPosition;

		float strength = Mathf.Min(1, score/MAKE_IT_RAIN_SCORE);

		int shots = Mathf.Max(1, Mathf.RoundToInt(strength * MAX_SHOTS));
		float shotTime = 0.4166f;

		if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			shots = 1;
			shotTime = 0.4166f;//this my be different per character;
		}

		Vector3 enemyPos = activeEnemy.unit.transf.localPosition;


		MSAnimationEvents events = activePlayer.unit.anim.GetComponent<MSAnimationEvents> ();
		events.totalAttacks = shots;
		activePlayer.unit.animat = MSUnit.AnimationType.ATTACK;

		for (int i = 0; i < shots; i++) 
		{
			if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
			{
				yield return StartCoroutine(activePlayer.AdvanceTo(activeEnemy.transform.localPosition.x - 75, -background.direction, background.scrollSpeed * 4));
				activePlayer.unit.animat = MSUnit.AnimationType.ATTACK;
			}
		}
	}

	public IEnumerator ReturnPlayerAfterAttack()
	{
		Debug.Log("Here?");
		if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed * 4));
			activePlayer.unit.direction = MSValues.Direction.EAST;
		}
		else
		{
			activePlayer.unit.animat = MSUnit.AnimationType.IDLE;
		}

		//If we're doing the bombdrop animation, we want to wait until the last bomb's hit to start ticking
		if (lastBomb != null)
		{
			while (lastBomb.gameObject.activeSelf) yield return null;
		}

		yield return StartCoroutine(activeEnemy.TakeDamage(MSAnimationEvents.curDamage, MSAnimationEvents.curElement));

		if (quickAttack)
		{
			quickAttack = false;
			yield break;
		}
		RunPickNextTurn(true);

	}

	public IEnumerator PlayerFlinch()
	{
		if (enemyDefSkill != null && enemyDefSkill.type == SkillType.CAKE_DROP)
		{
			//Cake eating sound?
		}
		else if (activeEnemy.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.meleeHit);
		}
		else
		{
			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.pistol);
		}

		if (hijackPlayerFlinch != null)
		{
			hijackPlayerFlinch();
			hijackPlayerFlinch = null;
			yield break;
		}

		yield return StartCoroutine(PlayerFlinchMoveBackwards());

		yield return new WaitForSeconds(recoilTime);

		yield return StartCoroutine(PlayerReturnAfterFlinch());
	}

	IEnumerator PlayerFlinchMoveBackwards()
	{
		Vector3 playerPos = activePlayer.unit.transf.localPosition;
		
		activePlayer.unit.animat = MSUnit.AnimationType.FLINCH;
		
		MSPoolManager.instance.Get(MSPrefabList.instance.flinchParticle, activePlayer.unit.transf.position);
		
		//attack sound should be played in attack not flinch
		
		while (activePlayer.unit.transf.localPosition.x > playerPos.x - recoilDistance )//* (totalShots+1))
		{
			activePlayer.unit.transf.localPosition -= Time.deltaTime * recoilDistance / recoilTime * -background.direction;
			yield return null;
		}
	}

	IEnumerator PlayerReturnAfterFlinch()
	{
		yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed * 4));
		activePlayer.unit.direction = MSValues.Direction.EAST;
		activePlayer.unit.animat = MSUnit.AnimationType.IDLE;
	}

	public IEnumerator EnemyFlinch(int totalShots){

		if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.meleeHit);
		}
		else
		{
			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.pistol);
		}

		Vector3 enemyPos = activeEnemy.unit.transf.localPosition;

		activeEnemy.unit.animat = MSUnit.AnimationType.FLINCH;
		
		MSPoolManager.instance.Get(MSPrefabList.instance.flinchParticle, activeEnemy.unit.transf.position);

		while (activeEnemy.unit.transf.localPosition.x < enemyPos.x + recoilDistance )//* (totalShots+1))
		{
			activeEnemy.unit.transf.localPosition += Time.deltaTime * recoilDistance / recoilTime * -background.direction;
			yield return null;
		}
		
		yield return new WaitForSeconds(recoilTime-.2f);

		/*
		if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed * 4));
			activePlayer.unit.direction = MSValues.Direction.EAST;
		}
		*/

		//activeEnemy.unit.animat = MSUnit.AnimationType.IDLE;
	}

	public IEnumerator EnemyReturnToStartPosition(){


		Vector3 enemyPos = enemyStartPosition;
		activeEnemy.unit.animat = MSUnit.AnimationType.RUN;

		yield return new WaitForSeconds (0.1f);

		//this moves the mobster back to the original position
		while(activeEnemy.unit.transf.localPosition.x > enemyPos.x)
		{
			activeEnemy.unit.transf.localPosition -= Time.deltaTime * recoilDistance / recoilTime * -background.direction;// * 3;
			yield return null;
		}
		
		activePlayer.unit.animat = MSUnit.AnimationType.IDLE;
		activeEnemy.unit.animat = MSUnit.AnimationType.IDLE;
		activeEnemy.unit.transf.localPosition = enemyPos;

		activePlayer.roiding = false;

	}


	/// <summary>
	/// Runs through the sequence of animations that follow the player dealing
	/// damage
	/// </summary>
	/// <param name='damage'>
	/// Damage.
	/// </param>
	/// <param name='element'>
	/// Damage element.
	/// </param>
	IEnumerator PlayerAttackAnimationSequence(int damage, Element element)
	{
		TintBoard();

		if (MSTutorialManager.instance.inTutorial)
		{
			if (riggedAttacks.Count > 0)
			{
				damage = riggedAttacks.Dequeue();
			}
			else if (damage < activeEnemy.health)
			{
				damage = (int)(activeEnemy.health * 1.2f);
			}
		}

		PZPuzzleManager.instance.swapLock += 1;
		//Debug.LogWarning("Player attack lock");

		float score = damage/activePlayer.monster.totalDamage;

		damage = (int)(damage * activePlayer.damageMultiplier);

		//Do all of the attack damage calculations and save before actually doing the damage
		int futureEnemyHp = activeEnemy.HealthAfterDamage(damage, element);

		if (!MSTutorialManager.instance.inTutorial)
		{
			yield return StartCoroutine(ShowAttackWords(score));
		}
		
		yield return StartCoroutine(PlayerShoot(score));

		//These variables are stored so we can pass them back later
		MSAnimationEvents.curDamage = damage;
		MSAnimationEvents.curElement = element;

		//instead of waiting for the coroutine to end we call the following functions at the action called when the animation is over

//		yield return StartCoroutine(activeEnemy.TakeDamage(damage, element));
//
//		RunPickNextTurn(true);

	}

	public IEnumerator EnemyAttack()
	{
		yield return StartCoroutine(EnemySkillBeforeEnemyTurn());

		int enemyDamageWithElement = 0;

		int enemyDamage;
		if (raidMode)
		{
			enemyDamage = UnityEngine.Random.Range(activeEnemy.monster.raidMonster.minDmg, activeEnemy.monster.raidMonster.maxDmg);
		}
		else
		{
			enemyDamage = Mathf.RoundToInt(activeEnemy.monster.totalDamage * UnityEngine.Random.Range(1.0f, 4.0f));
		}
		enemyDamageWithElement = (int)(enemyDamage * MSUtil.GetTypeDamageMultiplier(activePlayer.monster.monster.monsterElement, activeEnemy.monster.monster.monsterElement) * activeEnemy.damageMultiplier);

		yield return StartCoroutine(EnemyShoot(enemyDamageWithElement, activeEnemy.monster.monster.monsterElement));
		
		RunPickNextTurn(true);
	}

	/// <summary>
	/// Handles the enemy shooting, with animation and sending the health update to the server.
	/// Called from within the Enemy Attack flow.
	/// Quick Attack skill also calls this directly, with a neutral element.
	/// </summary>
	/// <returns>The shoot.</returns>
	/// <param name="totalDamage">Total damage.</param>
	/// <param name="element">Enemy's element. If this is from the Quick Attack skill, this should be neutral.</param>
	IEnumerator EnemyShoot(int totalDamage, Element element)
	{
		if (MSTutorialManager.instance.inTutorial)
		{
			totalDamage = activePlayer.monster.currHP - 14;
		}
		else
		{
			activePlayer.SendDamageUpdateToServer(totalDamage);
		}
		battleStats.damageTaken += Mathf.Min (totalDamage, activePlayer.monster.currHP);
		
		if (enemyCakeKid || activeEnemy.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			yield return StartCoroutine(activeEnemy.AdvanceTo(activePlayer.transform.localPosition.x + MELEE_ATTACK_DISTANCE, -background.direction, background.scrollSpeed * 4));
		}
		
		if (enemyCakeKid)
		{
			activePlayer.SendDamageUpdateToServer(99999);
			StartCoroutine(activePlayer.TakeDamage(99999, false, false));
			StartCoroutine (activeEnemy.TakeDamage(99999, false, false));
			StartCoroutine(activeEnemy.Die(false));
			StartCoroutine(activePlayer.Die(true));
			PZPuzzleManager.instance.ResetCakes();
		}
		else
		{
			activeEnemy.unit.animat = MSUnit.AnimationType.ATTACK;
			yield return StartCoroutine(activeEnemy.unit.anim.GetComponent<MSAnimationEvents>().WaitForEndOfEnemyAttack());
			
			StartCoroutine(activePlayer.TakeDamage((int)totalDamage, element));
			
			CheckBleed(activePlayer);
			
			yield return new WaitForSeconds(.3f);
			
			if (activeEnemy.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
			{
				yield return StartCoroutine(activeEnemy.AdvanceTo(enemyXPos, -background.direction, background.scrollSpeed * 4));
				activeEnemy.unit.direction = MSValues.Direction.WEST;
			}
			
			activeEnemy.unit.animat = MSUnit.AnimationType.IDLE;

			activeEnemy.roiding = false;
		}
	}

	public IEnumerator QueueUpPvp()
	{
		if (MSWhiteboard.loadedPvps != null)
		{
			foreach (var item in MSWhiteboard.loadedPvps.defenderInfoList) 
			{
				playersSeen.Add(item.defender.minUserProto.userUuid);
			}
		}

		QueueUpRequestProto request = new QueueUpRequestProto();
		request.attacker = MSWhiteboard.localMup;
		request.attackerElo = MSWhiteboard.localUser.pvpLeagueInfo.elo;
		request.clientTime = MSUtil.timeNowMillis;

		//We seenUserIds is read-only, so we can't just do seenUserIds = playersSeen. Laaame.
		foreach (var item in playersSeen) 
		{
			request.seenUserUuids.Add(item);
		}
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_QUEUE_UP_EVENT, null);
		
		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		MSWhiteboard.loadedPvps = UMQNetworkManager.responseDict[tagNum] as QueueUpResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (MSWhiteboard.loadedPvps.status != QueueUpResponseProto.QueueUpStatus.SUCCESS)
		{
			Debug.LogError("Problem queueing up: " + MSWhiteboard.loadedPvps.status.ToString());
			MSSceneManager.instance.ReconnectPopup();
		}

		nextPvpDefenderIndex = 0;
	}

	#endregion

	public void RevivePopup()
	{
		int gemsToSpend = MSHospitalManager.instance.SimulateHealForRevive(playerGoonies, MSUtil.timeNowMillis) * revives;
		MSPopupManager.instance.CreatePopup(
			"Revive", 
			"Revive your toons?",
            new string[] {"Cancel", "(G) " + gemsToSpend},
			new string[] {"greymenuoption", "purplemenuoption"},
			new WaitFunction[] {MSUtil.QuickCloseTop, ReviveWithGems},
			"purple"
		);
		                                  
	}

	Coroutine RunRevive()
	{
		return StartCoroutine(ReviveWithGems());
	}

	IEnumerator ReviveWithGems()
	{
		int gemsToSpend = MSHospitalManager.instance.SimulateHealForRevive(playerGoonies, MSUtil.timeNowMillis) * revives;
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, 
		                                     gemsToSpend))
		{
			ReviveInDungeonRequestProto request = new ReviveInDungeonRequestProto();
			request.sender = MSWhiteboard.localMup;
			request.userTaskUuid = MSWhiteboard.currUserTaskUuid;
			request.clientTime = MSUtil.timeNowMillis;

			foreach (var item in playerGoonies) 
			{
				item.currHP = item.maxHP;
				request.reviveMe.Add(item.GetCurrentHealthProto());
			}

			request.gemsSpent = gemsToSpend;

			int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REVIVE_IN_DUNGEON_EVENT);

			while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
			{
				yield return null;
			}
			
			ReviveInDungeonResponseProto response = UMQNetworkManager.responseDict[tagNum] as ReviveInDungeonResponseProto;
			UMQNetworkManager.responseDict.Remove(tagNum);
			
			MSActionManager.Popup.CloseTopPopupLayer();

			if (response.status == ReviveInDungeonResponseProto.ReviveInDungeonStatus.SUCCESS)
			{
				activePlayer.GoToStartPos();
				activePlayer.alpha = 1;
				winLosePopup.gameObject.SetActive(false);
				activePlayer.Init(activePlayer.monster);
				yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed * 4));
				RunPickNextTurn(true);
			}
			else
			{
				MSSceneManager.instance.ReconnectPopup();
			}
		}
	}

	void OnTutorialContinue()
	{
		waitingForTutorialSignal = false;
	}

	/// <summary>
	/// Shows the sprite for when a effective or non effective element is used
	/// </summary>
	public void EffectiveAttack(bool effective)
	{
		Vector3 center = new Vector3((activeEnemy.transform.position.x + activePlayer.transform.position.x) / 2f,
		                             (activeEnemy.transform.position.y + activePlayer.transform.position.y) / 2f,
		                             activePlayer.transform.position.z);
		TweenAlpha alpha = effectiveness.GetComponent<TweenAlpha>();
		TweenScale scale = effectiveness.GetComponent<TweenScale>();
		effectiveness.transform.position = center;
		if(effective)
		{
			effectiveness.spriteName = "noteffective";
		}
		else
		{
			effectiveness.spriteName = "supereffective";
		}

		alpha.ResetToBeginning();
		alpha.PlayForward();
		scale.ResetToBeginning();
		scale.PlayForward();
	}

	public void Save()
	{

		List<PZMonster> defeatedMonstersToSave = new List<PZMonster>();

		foreach (var item in defeatedEnemies) 
		{
			//if an enemy would have dropped an item and a capsule, it just drops an item instead
			if (item.taskMonster.itemId > 0){
				defeatedMonstersToSave.Add(item);
			}
			else if (item.taskMonster.puzzlePieceDropped)
			{
				defeatedMonstersToSave.Add(item);
			}
		}

		new PZCombatSave(activePlayer.monster, activeEnemy.monster.currHP, PZPuzzleManager.instance.board,
		                 battleStats, forfeitChance, currTurn, currPlayerDamage,
		                 PZPuzzleManager.instance.boardWidth, PZPuzzleManager.instance.boardHeight,
		                 playerSkillPoints, enemySkillPoints, activePlayer, activeEnemy, defeatedMonstersToSave);
	}

	void UpdateUserTaskStage(int taskStageId)
	{
		//Debug.Log("Update user task stage: " + taskStageId);

		UpdateMonsterHealthRequestProto request = new UpdateMonsterHealthRequestProto();
		request.isUpdateTaskStageForUser = true;
		request.userTaskUuid = MSWhiteboard.currUserTaskUuid;
		request.nuTaskStageId = taskStageId;

		request.sender = MSWhiteboard.localMup;
		request.umchp.Add(activePlayer.monster.GetCurrentHealthProto());
		request.clientTime = MSUtil.timeNowMillis;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UPDATE_MONSTER_HEALTH_EVENT, null);
	}

	void DealWithUserTaskStageUpdateResponse(int tagNum)
	{
		UpdateMonsterHealthResponseProto response = UMQNetworkManager.responseDict[tagNum] as UpdateMonsterHealthResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != UpdateMonsterHealthResponseProto.UpdateMonsterHealthStatus.SUCCESS)
		{
			Debug.LogError("Problem updating user task stage: " + response.status.ToString());
		}
	}

	void OnApplicationPause(bool pause)
	{
		if (pause && combatActive && !pvpMode)
		{
			Save();
		}
	}

	void OnApplicationQuit()
	{
		if (combatActive && !pvpMode)
		{
			Save();
		}
	}

}

[System.Serializable]
public struct BattleStats
{
	public int[] orbs;
	public int grenades;
	public int rockets;
	public int rainbows;
	public int combos;
	public int damageTaken;
	public int monstersDefeated;
}
