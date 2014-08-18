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
public class PZCombatManager : MonoBehaviour {

	/// <summary>
	/// The static instance of this combat
	/// </summary>
	public static PZCombatManager instance;

	public BattleStats battleStats = new BattleStats();

	public bool pvpMode = false;

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

	public PZCrate crate;

	List<int> playersSeen = new List<int>();

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

	bool wordsMoving = false;

	int currPlayerDamage = 0;

	public int currTurn = 0;

	public int playerTurns = 3;

	public int nextPvpDefenderIndex = 0;

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
			return MSBuildingManager.townHall.combinedProto.townHall.pvpQueueCashCost;
		}
	}

	public UILabel prizeQuantityLabel;

	[SerializeField]
	PZMonsterIntro intro;

	int revives = 0;

	public float forfeitChance;

	const float FORFEIT_START_CHANCE = 0.25F;

	int savedHealth = -1;

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
	}
	
	void OnDisable()
	{
		MSActionManager.Scene.OnCity -= OnCity;
		MSActionManager.Puzzle.OnDeploy -= OnDeploy;
		activePlayer.OnDeath -= OnPlayerDeath;
		activeEnemy.OnDeath -= OnEnemyDeath;
		
		MSActionManager.Clan.OnRaidMonsterAttacked -= OnRaidEnemyAttacked;
		MSActionManager.Clan.OnRaidMonsterDied -= OnRaidEnemyDefeated;
	}

	void OnCity()
	{
		pvpMode = false;
		raidMode = false;
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

	void PreInit()
	{
		riggedAttacks = null;

		ResetBattleStats();

		boardTint.PlayForward();
		
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
		prizeQuantityLabel.text = "0";

		PreInit();
		
		boardMove.Sample(0,false);
		boardMove.PlayForward();
		
		pvpUI.Reset();
		pvpMode = false;
		raidMode = false;

		activePlayer.Init(playerGoonies.Find(x=>x.currHP>0));

		waiting = true;
		StartCoroutine(ScrollToNextEnemy());

	}
	
	/// <summary>
	/// Start this instance. Gets the combat going.
	/// </summary>
	public void InitTask()
	{
		BeginDungeonResponseProto dungeon = MSWhiteboard.loadedDungeon;

		forfeitChance = FORFEIT_START_CHANCE;
		
		#if UNITY_IPHONE || UNITY_ANDROID
		Kamcord.StartRecording();
		#endif

		MSWhiteboard.currUserTaskId = dungeon.userTaskId;
		MSWhiteboard.currTaskId = dungeon.taskId;
		MSWhiteboard.currTaskStages = dungeon.tsp;

		Debug.LogWarning("Number of stages: " + dungeon.tsp.Count);

		mobsterCounter.MakePixelPerfect();

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
	}

	public Coroutine RunInitLoadedTask(MinimumUserTaskProto minTask, List<TaskStageProto> stages)
	{
		return StartCoroutine(InitLoadedTask(minTask, stages));
	}

	IEnumerator InitLoadedTask(MinimumUserTaskProto minTask, List<TaskStageProto> stages)
	{
		MSWhiteboard.currUserTaskId = minTask.userTaskId;
		MSWhiteboard.currTaskStages = stages;
		MSWhiteboard.currTaskId = minTask.taskId;

		PZCombatSave save = PZCombatSave.Load();

#if UNITY_IPHONE || UNITY_ANDROID
		Kamcord.StartRecording();
#endif
		PreInit ();
		
		boardMove.Sample(0,false);
		boardMove.PlayForward();
		
		pvpUI.Reset();

		forfeitChance = save.forfeitChance;

		currTurn = save.currTurn;
		currPlayerDamage = save.currPlayerDamage;

		activePlayer.Init(playerGoonies.Find(x=>x.userMonster.userMonsterId == save.activePlayerUserMonsterId));

		savedHealth = save.activeEnemyHealth;

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

		PZPuzzleManager.instance.InitBoardFromSave(save, minTask);

		PZScrollingBackground.instance.SetBackgrounds(MSDataManager.instance.Get<FullTaskProto>(minTask.taskId));

		yield return RunScrollToNextEnemy();

		if (currTurn == playerTurns)
		{
			Attack();
			currTurn = 0;
			currPlayerDamage = 0;
		}
	}

	public void InitPvp(int cash, int gems = 0)
	{
//		mobsterCounter.transform.parent.GetComponent<UIWidget>().alpha = 0f;

		PreInit ();

		playersSeen.Clear();

		MSWhiteboard.loadedPvps = null;

		pvpUI.Reset();
		pvpMode = true;
		raidMode = false;

		activePlayer.Init(playerGoonies.Find(x=>x.currHP>0));
		activePlayer.GoToStartPos();

		nextPvpDefenderIndex = 0;

		StartCoroutine(SpawnPvps(cash, gems));

		PZPuzzleManager.instance.swapLock += 1;
	}

	public void InitRaid()
	{
		PreInit();
		
		PZMonster mon;

		//We need to make sure we're using the right player team
		//This gets set up in init, so we're going to clear it and fix it here
		playerGoonies.Clear();
		foreach (var item in MSClanEventManager.instance.myTeam.currentTeam) 
		{
			mon = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId == item.userMonsterId);
			if (mon != null)
			{
				playerGoonies.Add(mon);
			}
		}

		boardMove.Sample(0,false);
		boardMove.PlayForward();
		
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
	}

	public void InitTutorial(MonsterProto boss, MonsterProto enemyOne, MonsterProto enemyTwo, Queue<int> riggedDamages)
	{
		PreInit ();

		activeEnemy.Init(new PZMonster(boss, 1));
		backupPvPEnemies[1].Init(new PZMonster(enemyOne, 1));
		backupPvPEnemies[0].Init(new PZMonster(enemyTwo, 1));
		activePlayer.Init(playerGoonies[0]);

		riggedAttacks = riggedDamages;

		boardMove.Sample(0, false);

		pvpUI.Reset();
		pvpMode = false;
		raidMode = false;

		//StartCoroutine(ScrollToNextEnemy());
	}


	public void SpawnNextPvp(bool useGems)
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

		int tagNum = 0;
		Coroutine waitForQueue = null;
		if (MSWhiteboard.loadedPvps == null || MSWhiteboard.loadedPvps.defenderInfoList.Count <= nextPvpDefenderIndex)
		{
			waitForQueue = StartCoroutine(QueueUpPvp());
		}

		Coroutine oneGuy = StartCoroutine(backupPvPEnemies[0].Retreat(-background.direction, background.scrollSpeed));
		Coroutine otherGuy = StartCoroutine(backupPvPEnemies[1].Retreat(-background.direction, background.scrollSpeed));
		yield return StartCoroutine((activeEnemy.Retreat(-background.direction, background.scrollSpeed)));

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

		//Move the monsters out
		StartCoroutine(backupPvPEnemies[1].AdvanceTo(enemyXPos + 130, -background.direction, background.scrollSpeed * 1.5f));
		StartCoroutine(backupPvPEnemies[0].AdvanceTo(enemyXPos + 20, -background.direction, background.scrollSpeed * 1.5f));
		yield return StartCoroutine(activeEnemy.AdvanceTo(enemyXPos, -background.direction, background.scrollSpeed * 1.5f));

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

		pvpUI.Retract();
		PZPuzzleManager.instance.InitBoard();

		boardMove.Sample(0, false);
		boardMove.delay = 1f;
		boardMove.PlayForward();
		boardMove.delay = 0f;

		boardTint.Sample(1, false);
		boardTint.PlayReverse();
		PZPuzzleManager.instance.swapLock = 0;
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
		request.cashSpent = cash;
		request.gemsSpent = gems;
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
		//CBKEventManager.Popup.CloseAllPopups();
		//Debug.Log("Deploying " + monster.userMonster.userId);

		if (!MSTutorialManager.instance.inTutorial)
		{
			boardTint.PlayReverse();
		}

		if (monster != activePlayer.monster)
		{
			Debug.Log ("Actually deploying");

			//if (activePlayer.alive)
			//{
				StartCoroutine(SwapCharacters(monster));
			//}
			//else
			//{
			//	activePlayer.Init(monster);
			//	activePlayer.GoToStartPos();
			//	CheckBleed(activePlayer);
			//	StartCoroutine(ScrollToNextEnemy());
			//}
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
		StartCoroutine(ScrollToNextEnemy());
	}
	
	void OnPlayerDeath()
	{
		//Debug.Log("Lock: Player death");
		PZPuzzleManager.instance.swapLock += 1;

		boardTint.PlayForward();
		
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
		bool forfeitSuccess = Random.value <= forfeitChance;
		PZPuzzleManager.instance.swapLock++;
		Vector3 center = new Vector3((activeEnemy.transform.position.x + activePlayer.transform.position.x) / 2f,
		                             (activeEnemy.transform.position.y + activePlayer.transform.position.y) / 2f,
		                             activePlayer.transform.position.z);
		forfeit.SetParentPosition(center);
		yield return StartCoroutine(forfeit.Animate (forfeitSuccess));
		if (forfeitSuccess) {
			yield return StartCoroutine(activePlayer.Retreat(-background.direction, background.scrollSpeed));
			ActivateLoseMenu();
		} else {
			forfeitChance *= 2f;
			yield return StartCoroutine(EnemyAttack(0));
		}
		PZPuzzleManager.instance.swapLock--;
	}

	public void ActivateLoseMenu(bool blackOut = false){
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
						pieces.Add(item.monster);
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
		if (raidMode && response.sender.userId != MSWhiteboard.localMup.userId)
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
		PZPuzzleManager.instance.swapLock += 1;

		yield return StartCoroutine(activePlayer.Retreat(-background.direction, background.scrollSpeed*3.5f));

		activePlayer.Init(swapTo);

		yield return null;

		yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed*3.5f));

		PZPuzzleManager.instance.swapLock = 0;
		
		if (MSActionManager.Puzzle.ForceShowSwap != null)
		{
			MSActionManager.Puzzle.ForceShowSwap();
		}
	}

	public Coroutine RunScrollToNextEnemy()
	{
		return StartCoroutine(ScrollToNextEnemy());
	}

	/// <summary>
	/// Spawns a new enemy and scrolls the background until
	/// that enemy is in its place.
	/// </summary>
	IEnumerator ScrollToNextEnemy()
	{
		if (boardTint.value <= .01f) {
				boardTint.PlayForward ();
		}

		PZPuzzleManager.instance.swapLock += 1;

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

			activePlayer.unit.animat = MSUnit.AnimationType.IDLE;
			
			MSSoundManager.instance.StopLoop();

			yield break;
		}
		else if (enemies.Count > 0) 
		{
			activeEnemy.OnClick();

			activeEnemy.GoToStartPos ();
			activeEnemy.Init (enemies.Dequeue ());

			UpdateUserTaskStage(MSWhiteboard.currTaskStages.Find(x=>x.stageMonsters[0] == activeEnemy.monster.taskMonster).stageId);

			activeEnemy.unit.direction = MSValues.Direction.WEST;
			activeEnemy.unit.animat = MSUnit.AnimationType.IDLE;

			if (savedHealth >= 0)
			{
				activeEnemy.health = savedHealth;
				savedHealth = -1;
			}

			Save();

			mobsterCounter.text = (defeatedEnemies.Count + 1) + "/" + (enemies.Count + 1 + defeatedEnemies.Count);
			mobsterCounter.MakePixelPerfect();
			TweenAlpha.Begin(mobsterCounter.transform.parent.gameObject, 1f ,1f);
			intro.Init (activeEnemy.monster, defeatedEnemies.Count + 1, enemies.Count + 1 + defeatedEnemies.Count);
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

		activePlayer.unit.animat = MSUnit.AnimationType.IDLE;

		MSSoundManager.instance.StopLoop();

		if (activeEnemy.alive && activePlayer.alive)
		{
			if (MSActionManager.Puzzle.ForceShowSwap != null)
			{
				MSActionManager.Puzzle.ForceShowSwap();
			}
			boardTint.PlayReverse();
		}

		if (MSActionManager.Puzzle.OnTurnChange != null)
		{
			MSActionManager.Puzzle.OnTurnChange(playerTurns);
		}

		PZPuzzleManager.instance.swapLock = activeEnemy.alive ? 0 : 1;
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
					pieces.Add(item.monster);
				}
			}
		}
		winLosePopup.InitWin(xp, cash, oil, pieces, items);
		MSResourceManager.instance.Collect(ResourceType.CASH, cash);
		MSResourceManager.instance.GainExp(xp);
	}
	
	IEnumerator SendEndResult(bool userWon)
	{
#if UNITY_ANDROID || UNITY_IPHONE
		Kamcord.StopRecording();
#endif

		if (MSActionManager.Quest.OnBattleFinish != null)
		{
			MSActionManager.Quest.OnBattleFinish(battleStats);
		}

		if (pvpMode)
		{
			EndPvpBattleRequestProto request = new EndPvpBattleRequestProto();
			request.sender = MSWhiteboard.localMupWithResources;
			request.defenderId = defender.defender.minUserProto.userId;

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
			request.userTaskId = MSWhiteboard.currUserTaskId;
			request.userWon = userWon;
			request.clientTime = MSUtil.timeNowMillis;

			if (userWon && !MSQuestManager.instance.taskDict.ContainsKey(MSWhiteboard.currTaskId))
			{
				int task = MSWhiteboard.currTaskId;
				MSQuestManager.instance.taskDict[task] = true;
				request.firstTimeUserWonTask = true;
				request.userBeatAllCityTasks = MSQuestManager.instance.HasFinishedAllTasksInCity(MSWhiteboard.cityID);
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
		
		if (currTurn == playerTurns)
		{
			Attack();
			currPlayerDamage = 0;
			currTurn = 0;
		}
		else if (!MSTutorialManager.instance.inTutorial)
		{
			Save ();
		}
	}

	public void Attack()
	{
		StartCoroutine(DamageAnimations(currPlayerDamage, activePlayer.monster.monster.monsterElement));
	}

	IEnumerator ShowAttackWords(float score)
	{
		screenTint.PlayForward();
		
		attackWords.gameObject.SetActive(true);

		if (score > MAKE_IT_RAIN_SCORE)
		{
			attackWords.MakeItRain();
			attackWords.GetComponent<PZRainbow>().Play();
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

	public void OnWordsFinishMoving()
	{
		wordsMoving = false;
	}
	
	void MakeItRain()
	{
		Transform plane = (MSPoolManager.instance.Get(MSPrefabList.instance.planePrefab, Vector3.zero) as MonoBehaviour).transform;
		plane.parent = combatParent;
		plane.localPosition = activePlayer.startingPos;
		plane.localScale = Vector3.one;

		for (int i = 0; i < NUM_BOMBS; i++) 
		{
			BombAt(activeEnemy.unit.transf.localPosition.x - BOMB_SPACING * NUM_BOMBS / 2f + BOMB_SPACING * i, plane);
		}
	}

	void BombAt(float x, Transform plane)
	{
		Transform bomb = (MSPoolManager.instance.Get(MSPrefabList.instance.bombPrefab, Vector3.zero, combatParent) as MonoBehaviour).transform;
		bomb.localPosition = new Vector3(x, Screen.height/2 + BOMB_SPACING);
		bomb.localScale = Vector3.one;
		bomb.GetComponent<PZBomb>().targetHeight = activeEnemy.unit.transf.localPosition.y + 
			(background.direction.y / background.direction.x) * (x - activeEnemy.unit.transf.localPosition.x);
		bomb.GetComponent<PZBomb> ().planeTrans = plane;
	}

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

	IEnumerator PlayerShoot(float score)
	{
		enemyStartPosition =  activeEnemy.unit.transf.localPosition;

		float strength = Mathf.Min(1, score/MAKE_IT_RAIN_SCORE);

		int shots = Mathf.RoundToInt(strength * MAX_SHOTS);
		float shotTime = 0.4166f;

		if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			shots = 1;
			shotTime = 0.4166f;//this my be different per character;
		}

		Vector3 enemyPos = activeEnemy.unit.transf.localPosition;

		activePlayer.unit.anim.GetComponent<MSAnimationEvents> ().totalAttacks = shots;
		activePlayer.unit.animat = MSUnit.AnimationType.ATTACK;

		for (int i = 0; i < shots; i++) {

			if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
			{
				yield return StartCoroutine(activePlayer.AdvanceTo(activeEnemy.transform.localPosition.x - 30, -background.direction, background.scrollSpeed * 4));
				activePlayer.unit.animat = MSUnit.AnimationType.ATTACK;
			}

			yield return new WaitForSeconds(shotTime);

			//When the animation gets to the frame where the gun fires, EnemyFlinch() is triggered

			if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
			{
				yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed * 4));
				activePlayer.unit.direction = MSValues.Direction.EAST;
			}
		}
	}

	public IEnumerator PlayerFlinch()
	{
		if (MSTutorialManager.instance.hijackFlinch)
		{
			MSTutorialManager.instance.HijackFlinch();
			yield break;
		}

		Vector3 playerPos = activePlayer.unit.transf.localPosition;

		activePlayer.unit.animat = MSUnit.AnimationType.FLINCH;

		MSPoolManager.instance.Get(MSPrefabList.instance.flinchParticle, activePlayer.unit.transf.position);

		//attack sound should be played in attack not flinch

		while (activePlayer.unit.transf.localPosition.x > playerPos.x - recoilDistance )//* (totalShots+1))
		{
			activePlayer.unit.transf.localPosition -= Time.deltaTime * recoilDistance / recoilTime * -background.direction;
			yield return null;
		}

		yield return new WaitForSeconds(recoilTime);

		if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed * 4));
			activePlayer.unit.direction = MSValues.Direction.EAST;
		}

		activePlayer.unit.animat = MSUnit.AnimationType.IDLE;
	}

	public IEnumerator EnemyFlinch(int totalShots){
		Vector3 enemyPos = activeEnemy.unit.transf.localPosition;

		activeEnemy.unit.animat = MSUnit.AnimationType.FLINCH;
		
		MSPoolManager.instance.Get(MSPrefabList.instance.flinchParticle, activeEnemy.unit.transf.position);

		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.pistol);
		
		while (activeEnemy.unit.transf.localPosition.x < enemyPos.x + recoilDistance )//* (totalShots+1))
		{
			activeEnemy.unit.transf.localPosition += Time.deltaTime * recoilDistance / recoilTime * -background.direction;
			yield return null;
		}
		
		yield return new WaitForSeconds(recoilTime-.2f);
		
		if (activePlayer.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed * 4));
			activePlayer.unit.direction = MSValues.Direction.EAST;
		}

		activeEnemy.unit.animat = MSUnit.AnimationType.IDLE;
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
	IEnumerator DamageAnimations(int damage, Element element)
	{
		boardTint.PlayForward();

		if (MSTutorialManager.instance.inTutorial && riggedAttacks.Count > 0)
		{
			damage = riggedAttacks.Dequeue();
		}

		//Debug.Log("Lock: Animating");
		PZPuzzleManager.instance.swapLock += 1;

		float score = damage/activePlayer.monster.totalDamage;

		//Do all of the attack damage calculations and save before actually doing the damage
		int futureEnemyHp = activeEnemy.HealthAfterDamage(damage, element);

		int enemyDamageWithElement = 0;
		if (futureEnemyHp > 0)
		{
			int enemyDamage;
			if (raidMode)
			{
				enemyDamage = Random.Range(activeEnemy.monster.raidMonster.minDmg, activeEnemy.monster.raidMonster.maxDmg);
			}
			else
			{
				enemyDamage = Mathf.RoundToInt(activeEnemy.monster.totalDamage * Random.Range(1.0f, 4.0f));
			}
			enemyDamageWithElement = (int)(enemyDamage * MSUtil.GetTypeDamageMultiplier(activePlayer.monster.monster.monsterElement, activeEnemy.monster.monster.monsterElement));
			if (MSTutorialManager.instance.inTutorial)
			{
				enemyDamageWithElement = activePlayer.monster.currHP - 14;
			}
			else
			{
				activePlayer.SendDamageUpdateToServer(enemyDamageWithElement);
			}
			battleStats.damageTaken += Mathf.Min (enemyDamageWithElement, activePlayer.monster.currHP);
		}

		if (!MSTutorialManager.instance.inTutorial)
		{
			Save();
		}

		yield return StartCoroutine(ShowAttackWords(score));
		
		yield return StartCoroutine(PlayerShoot(score));
		
		yield return StartCoroutine(activeEnemy.TakeDamage(damage, element));

		if (futureEnemyHp > 0)
		{
			StartCoroutine(EnemyAttack(enemyDamageWithElement));
		}
		
		//Debug.Log("Unlock: Done Animating");
		PZPuzzleManager.instance.swapLock -= 1;

		if (MSActionManager.Puzzle.OnTurnChange != null)
		{
			MSActionManager.Puzzle.OnTurnChange(playerTurns);
		}

		if (activeEnemy.alive)
		{
			if (MSActionManager.Puzzle.OnNewPlayerRound != null)
			{
				MSActionManager.Puzzle.OnNewPlayerRound();
			}
		}

		boardTint.PlayReverse();

	}

	public IEnumerator EnemyAttack(float damageToDeal)
	{
		if (activeEnemy.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			yield return StartCoroutine(activeEnemy.AdvanceTo(activePlayer.transform.localPosition.x + MELEE_ATTACK_DISTANCE, -background.direction, background.scrollSpeed * 4));
		}
		
		activeEnemy.unit.animat = MSUnit.AnimationType.ATTACK;
		yield return new WaitForSeconds(.5f);
		
		StartCoroutine(activePlayer.TakeDamage((int)damageToDeal, activeEnemy.monster.monster.monsterElement));
		
		CheckBleed(activePlayer);
		
		yield return new WaitForSeconds(.3f);
		
		if (activeEnemy.monster.monster.attackAnimationType == MonsterProto.AnimationType.MELEE)
		{
			yield return StartCoroutine(activeEnemy.AdvanceTo(enemyXPos, -background.direction, background.scrollSpeed * 4));
			activeEnemy.unit.direction = MSValues.Direction.WEST;
		}
		
		activeEnemy.unit.animat = MSUnit.AnimationType.IDLE;
		
	}

	public IEnumerator QueueUpPvp()
	{
		if (MSWhiteboard.loadedPvps != null)
		{
			foreach (var item in MSWhiteboard.loadedPvps.defenderInfoList) 
			{
				playersSeen.Add(item.defender.minUserProto.userId);
			}
		}

		QueueUpRequestProto request = new QueueUpRequestProto();
		request.attacker = MSWhiteboard.localMup;
		request.attackerElo = MSWhiteboard.localUser.pvpLeagueInfo.elo;
		request.clientTime = MSUtil.timeNowMillis;

		//We seenUserIds is read-only, so we can't just do seenUserIds = playersSeen. Laaame.
		foreach (var item in playersSeen) 
		{
			request.seenUserIds.Add(item);
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
		}

		nextPvpDefenderIndex = 0;
	}

	void ReviveWithGems()
	{
		int gemsToSpend = MSHospitalManager.instance.SimulateHealForRevive(playerGoonies, MSUtil.timeNowMillis);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, 
		                                     gemsToSpend,
		                                     ReviveWithGems))
		{
			ReviveInDungeonRequestProto request = new ReviveInDungeonRequestProto();
			request.sender = MSWhiteboard.localMup;
			request.userTaskId = MSWhiteboard.currUserTaskId;
			request.clientTime = MSUtil.timeNowMillis;

			foreach (var item in playerGoonies) 
			{
				item.currHP = item.maxHP;
				request.reviveMe.Add(item.GetCurrentHealthProto());
			}

			request.gemsSpent = gemsToSpend;

			UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REVIVE_IN_DUNGEON_EVENT, DealWithReviveResponse);
		}
	}

	void DealWithReviveResponse(int tagNum)
	{
		ReviveInDungeonResponseProto response = UMQNetworkManager.responseDict[tagNum] as ReviveInDungeonResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == ReviveInDungeonResponseProto.ReviveInDungeonStatus.SUCCESS)
		{
			OnDeploy(playerGoonies[0]);
		}
		else
		{
			Debug.LogError("Problem reviving in Dungeon: " + response.status.ToString());
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

	public void SaveAfterDamage(int enemyHealthToBe)
	{
		new PZCombatSave(activePlayer.monster, enemyHealthToBe, PZPuzzleManager.instance.board,
		                 battleStats, forfeitChance, 0, 0, PZPuzzleManager.instance.boardWidth,
		                 PZPuzzleManager.instance.boardHeight);
	}

	public void Save()
	{
		new PZCombatSave(activePlayer.monster, activeEnemy.monster.currHP, PZPuzzleManager.instance.board,
		                 battleStats, forfeitChance, currTurn, currPlayerDamage, 
		                 PZPuzzleManager.instance.boardWidth, PZPuzzleManager.instance.boardHeight);
	}

	void UpdateUserTaskStage(int taskStageId)
	{
		//Debug.Log("Update user task stage: " + taskStageId);

		UpdateMonsterHealthRequestProto request = new UpdateMonsterHealthRequestProto();
		request.isUpdateTaskStageForUser = true;
		request.userTaskId = MSWhiteboard.currUserTaskId;
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
