using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

	public bool pvpMode = false;

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
	[SerializeField]
	PZCombatUnit activeEnemy;

	[SerializeField]
	PZCombatUnit[] backupPvPEnemies;

	/// <summary>
	/// The unit prefab which will be used as the template to generate
	/// enemies.
	/// </summary>
	[SerializeField]
	CBKUnit unitPrefab;
	
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
	
	/// <summary>
	/// The Y value at which the enemy needs to be for the scrolling to stop happening
	/// </summary>
	public float enemyYThreshold = 30;
	
	/// <summary>
	/// Gets the enemy spawn position.
	/// Offset from the player using the same values as the background
	/// </summary>
	/// <value>
	/// The enemy spawn position.
	/// </value>
	public static readonly Vector3 enemySpawnPosition = new Vector3(464, 511);

	const float playerXFromSideThreshold = 78;

	float playerXPos
	{
		get
		{
			return -(Screen.width * Mathf.Max(1, 640f / Screen.height) / 2) + playerXFromSideThreshold;
		}
	}

	//const float enemyXFromRightThreshold = -380;

	float enemyXPos
	{
		get
		{
			return -playerXFromSideThreshold - 100;
		}
	}
	
	[SerializeField]
	PZDeployPopup deployPopup;
	
	/// <summary>
	/// The popup that displays when the player fails a mission
	/// </summary>
	[SerializeField]
	PZWinLosePopup winLosePopup;

	[SerializeField]
	UISprite attackWords;

	[SerializeField]
	UITweener attackWordsTweenPos;

	[SerializeField]
	TweenAlpha boardTint;

	[SerializeField]
	TweenPosition boardMove;

	[SerializeField]
	UITweener screenTint;

	[SerializeField]
	TweenAlpha bloodSplatter;

	[SerializeField]
	MSPvpUI pvpUI;

	bool wordsMoving = false;

	int currPlayerDamage = 0;

	public int currTurn = 0;

	public int playerTurns = 3;

	public int nextPvpDefenderIndex = 0;

	PvpProto defender;

	public float recoilDistance = 20;

	public float recoilTime = .2f;

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

	public const int MATCH_MONEY = 1500;

	/// <summary>
	/// Awake this instance. Set up instance reference.
	/// </summary>
	void Awake()
	{
		instance = this;
	}
	
	void OnEnable()
	{
		CBKEventManager.Puzzle.OnDeploy += OnDeploy;
		activePlayer.OnDeath += OnPlayerDeath;
		activeEnemy.OnDeath += OnEnemyDeath;
	}
	
	void OnDisable()
	{
		CBKEventManager.Puzzle.OnDeploy -= OnDeploy;
		activePlayer.OnDeath -= OnPlayerDeath;
		activeEnemy.OnDeath -= OnEnemyDeath;
	}

	void Init()
	{
		boardTint.PlayForward();
		
		StopBleeding();
		
		activeEnemy.GoToStartPos();
		activeEnemy.alive = false;
		activePlayer.GoToStartPos();
		activePlayer.alive = false;
		activePlayer.monster = null;

		//TODO: Make sure to reset the PvP stand-ins!
		
		enemies.Clear();
		defeatedEnemies.Clear();
		currTurn = 0;
		currPlayerDamage = 0;

		winLosePopup.gameObject.SetActive(false);

		if (activeEnemy != null && activeEnemy.unit != null)
		{
			Color temp = activeEnemy.unit.sprite.color;
			activeEnemy.unit.sprite.color = new Color(temp.r, temp.g, temp.b, 0);
		}
		
		foreach (PZMonster monster in CBKMonsterManager.userTeam)
		{
			if (monster != null && monster.monster != null && monster.monster.monsterId > 0 && monster.currHP > 0)
			{
				playerGoonies.Add(monster);
			}
		}
	}
	
	/// <summary>
	/// Start this instance. Gets the combat going.
	/// </summary>
	public void InitTask()
	{
		Init ();
		
		boardMove.Sample(0,false);
		boardMove.PlayForward();

		pvpUI.Reset();
		pvpMode = false;

		MSWhiteboard.currUserTaskId = MSWhiteboard.loadedDungeon.userTaskId;

		PZMonster mon;
		foreach (TaskStageProto stage in MSWhiteboard.loadedDungeon.tsp)
		{
			Debug.Log("Stage " + stage.stageId + ", Monster: " + stage.stageMonsters[0].monsterId);
			mon = new PZMonster(stage.stageMonsters[0]);
			enemies.Enqueue(mon);
		}
		//CBKEventManager.Popup.OnPopup(deployPopup.gameObject);
		deployPopup.Init(CBKMonsterManager.userTeam);
		
		//Lock swap until deploy
		PZPuzzleManager.instance.swapLock += 1;
	}

	public void InitPvp()
	{
		Init ();

		playersSeen.Clear();

		MSWhiteboard.loadedPvps = null;

		pvpUI.Reset();
		pvpMode = true;

		activePlayer.Init(playerGoonies[0]);
		activePlayer.GoToStartPos();

		nextPvpDefenderIndex = 0;

		StartCoroutine(SpawnPvps());

		PZPuzzleManager.instance.swapLock += 1;
	}

	public void SpawnNextPvp()
	{
		if (CBKResourceManager.instance.Spend(ResourceType.CASH, MATCH_MONEY, SpawnNextPvp))
	    {
			StartCoroutine(SpawnPvps());
		}
	}

	IEnumerator SpawnPvps()
	{
		pvpUI.Retract();

		StartCoroutine(SpendPvpMatchMoney());

		yield return null;
		yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed));
		Debug.Log("Finished player run");
		activePlayer.unit.animat = CBKUnit.AnimationType.RUN;
		activePlayer.unit.direction = CBKValues.Direction.EAST;
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
		activePlayer.unit.animat = CBKUnit.AnimationType.IDLE;

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
		boardMove.PlayForward();

		boardTint.Sample(1, false);
		boardTint.PlayReverse();
		PZPuzzleManager.instance.swapLock = 0;
	}

	IEnumerator SendBeginPvpRequest()
	{
		BeginPvpBattleRequestProto request = new BeginPvpBattleRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.senderElo = MSWhiteboard.localUser.elo;
		request.attackStartTime = CBKUtil.timeNowMillis;
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

	public IEnumerator SpendPvpMatchMoney()
	{
		UpdateUserCurrencyRequestProto request = new UpdateUserCurrencyRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.cashSpent = MATCH_MONEY;
		request.reason = "pvp";

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UPDATE_USER_CURRENCY_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

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
		Debug.Log("Deploying " + monster.userMonster.userId);

		if (monster != activePlayer.monster)
		{
			Debug.Log ("Actually deploying");

			if (activePlayer.alive)
			{
				StartCoroutine(SwapCharacters(monster));
			}
			else
			{
				activePlayer.Init(monster);
				activePlayer.GoToStartPos();
				CheckBleed(activePlayer);
				//if (!activeEnemy.alive)
				//{
					StartCoroutine(ScrollToNextEnemy());
				//}
			}
		}
		else
		{
			if (CBKEventManager.Puzzle.ForceShowSwap != null)
			{
				CBKEventManager.Puzzle.ForceShowSwap();
			}
		}
	}
	
	void OnEnemyDeath()
	{
		if (CBKEventManager.Quest.OnMonsterDefeated != null)
		{
			CBKEventManager.Quest.OnMonsterDefeated(activeEnemy.monster.monster.monsterId);
		}

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
				deployPopup.Init(CBKMonsterManager.userTeam);
				return;
			}
		}
		
		winLosePopup.gameObject.SetActive(true);
		winLosePopup.tweener.ResetToBeginning();
		winLosePopup.tweener.GetComponent<UITweener>().PlayForward();
		winLosePopup.InitLose();
		
		StartCoroutine(SendEndResult(false));
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

		yield return StartCoroutine(activePlayer.AdvanceTo(playerXPos, -background.direction, background.scrollSpeed*3.5f));

		PZPuzzleManager.instance.swapLock = 0;
		
		if (CBKEventManager.Puzzle.ForceShowSwap != null)
		{
			CBKEventManager.Puzzle.ForceShowSwap();
		}
	}

	/// <summary>
	/// Spawns a new enemy and scrolls the background until
	/// that enemy is in its place.
	/// </summary>
	IEnumerator ScrollToNextEnemy()
	{
		if (boardTint.value <= .01f)
		{
			boardTint.PlayForward();
		}

		//Debug.Log("Lock: Scrolling");
		PZPuzzleManager.instance.swapLock += 1;
		
		activePlayer.unit.animat = CBKUnit.AnimationType.RUN;

		if (enemies.Count > 0)
		{
			activeEnemy.GoToStartPos();
			activeEnemy.Init(enemies.Dequeue());
			activeEnemy.unit.direction = CBKValues.Direction.WEST;
			activeEnemy.unit.animat = CBKUnit.AnimationType.IDLE;
		}
		else if (!activeEnemy.alive)
		{
			activeEnemy.GoToStartPos();
			StartCoroutine(SendEndResult(true));

			boardMove.PlayReverse();

			if (!pvpMode)
			{
				if (CBKEventManager.Quest.OnTaskCompleted != null)
				{
					CBKEventManager.Quest.OnTaskCompleted(MSWhiteboard.loadedDungeon.taskId);
				}
			}

			winLosePopup.gameObject.SetActive(true);

			GetRewards();
			winLosePopup.tweener.ResetToBeginning();
			winLosePopup.tweener.PlayForward();
		}

		CBKSoundManager.instance.Loop(CBKSoundManager.instance.walking);

		while(activePlayer.unit.transf.localPosition.x < playerXPos)
		{
			activePlayer.unit.transf.localPosition += Time.deltaTime * -background.direction * background.scrollSpeed;
			yield return null;
		}

		while(activeEnemy.unit.transf.localPosition.x > enemyXPos)
		{
			background.Scroll(activeEnemy.unit);
			yield return null;
		}


		activePlayer.unit.animat = CBKUnit.AnimationType.IDLE;

		CBKSoundManager.instance.StopLoop();

		if (activeEnemy.alive && activePlayer.alive)
		{
			if (CBKEventManager.Puzzle.ForceShowSwap != null)
			{
				CBKEventManager.Puzzle.ForceShowSwap();
			}
			boardTint.PlayReverse();
		}

		PZPuzzleManager.instance.swapLock = activeEnemy.alive ? 0 : 1;
	}

	void GetRewards()
	{
		int cash = 0;
		int xp = 0;
		List<MonsterProto> pieces = new List<MonsterProto>();
		if (pvpMode)
		{
			cash = defender.prospectiveCashWinnings;
		}
		else
		{
			foreach (var item in defeatedEnemies) 
			{
				cash += item.taskMonster.cashReward;
				xp += item.taskMonster.expReward;
				if (item.taskMonster.puzzlePieceDropped)
				{
					pieces.Add(item.monster);
				}
			}
		}
		winLosePopup.InitWin(xp, cash, pieces);
		CBKResourceManager.instance.Collect(ResourceType.CASH, cash);
		CBKResourceManager.instance.GainExp(xp);
	}
	
	IEnumerator SendEndResult(bool userWon)
	{
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

				CBKResourceManager.instance.Collect(ResourceType.CASH, defender.prospectiveCashWinnings);
				CBKResourceManager.instance.Collect (ResourceType.OIL, defender.prospectiveOilWinnings);
			}
			request.clientTime = CBKUtil.timeNowMillis;

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
		else
		{
			EndDungeonRequestProto request = new EndDungeonRequestProto();
			request.sender = MSWhiteboard.localMupWithResources;
			request.userTaskId = MSWhiteboard.currUserTaskId;
			request.userWon = userWon;
			request.clientTime = CBKUtil.timeNowMillis;

			if (!CBKQuestManager.taskDict.ContainsKey(MSWhiteboard.loadedDungeon.taskId))
			{
				int task = MSWhiteboard.loadedDungeon.taskId;
				CBKQuestManager.taskDict[task] = true;
				request.firstTimeUserWonTask = true;
				request.userBeatAllCityTasks = CBKQuestManager.instance.HasFinishedAllTasksInCity(MSWhiteboard.cityID);
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
				CBKMonsterManager.instance.UpdateOrAddAll(response.updatedOrNew);
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
		int damage;
		MonsterProto.MonsterElement element;
		activePlayer.DealDamage(gemsBroken, out damage, out element);
		
		//Debug.Log("Damage: " + damage + ", Combo: " + combo);
		
		if (combo > 1)
		{
			damage = (int)(damage * (1 + (combo-1) / 4f));
			//Debug.Log("Combo damage: " + damage);
		}

		currPlayerDamage += damage;

		if (++currTurn == playerTurns)
		{
			Attack();
			currPlayerDamage = 0;
		}
		
		if (CBKEventManager.Puzzle.OnTurnChange != null)
		{
			CBKEventManager.Puzzle.OnTurnChange(playerTurns - currTurn);
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
			attackWords.spriteName = MAKE_IT_RAIN_PREFIX + "2";
			attackWords.GetComponent<PZRainbow>().Play();
		}
		else
		{
			if (score > HAMMERTIME_SCORE)
			{
				attackWords.spriteName = HAMMERTIME_SPRITE_NAME;
			}
			else if (score > CANT_TOUCH_THIS_SCORE)
			{
				attackWords.spriteName = CANT_TOUCH_THIS_SPRITE_NAME;
			}
			else if (score > BALLIN_SCORE)
			{
				attackWords.spriteName = BALLIN_SPRITE_NAME;
			}
		}

		UISpriteData data = attackWords.GetAtlasSprite();
		attackWords.width = data.width;
		attackWords.height = data.height;
		
		attackWordsTweenPos.ResetToBeginning();
		attackWordsTweenPos.PlayForward();

		wordsMoving = true;
		while(wordsMoving)
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
		Transform plane = (CBKPoolManager.instance.Get(CBKPrefabList.instance.planePrefab, Vector3.zero) as MonoBehaviour).transform;
		plane.parent = combatParent;
		plane.localPosition = activePlayer.startingPos;

		for (int i = 0; i < NUM_BOMBS; i++) 
		{
			BombAt(activeEnemy.unit.transf.localPosition.x - BOMB_SPACING * NUM_BOMBS / 2f + BOMB_SPACING * i);
		}
	}

	void BombAt(float x)
	{
		Transform bomb = (CBKPoolManager.instance.Get(CBKPrefabList.instance.bombPrefab, Vector3.zero) as MonoBehaviour).transform;
		bomb.parent = combatParent;
		bomb.localPosition = new Vector3(x, Screen.height/2 + BOMB_SPACING);
		bomb.GetComponent<PZBomb>().targetHeight = activeEnemy.unit.transf.localPosition.y + 
			(background.direction.y / background.direction.x) * (x - activeEnemy.unit.transf.localPosition.x);
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
		float strength = Mathf.Min(1, score/MAKE_IT_RAIN_SCORE);

		int shots = Mathf.RoundToInt(strength * MAX_SHOTS);

		Vector3 enemyPos = activeEnemy.unit.transf.localPosition;

		for (int i = 0; i < shots; i++) {

			activePlayer.unit.animat = CBKUnit.AnimationType.ATTACK;

			yield return new WaitForSeconds(0.1f);

			activeEnemy.unit.animat = CBKUnit.AnimationType.FLINCH;

			CBKPoolManager.instance.Get(CBKPrefabList.instance.flinchParticle, activeEnemy.unit.transf.position);

			CBKSoundManager.instance.PlayOneShot(CBKSoundManager.instance.pistol);

			while (activeEnemy.unit.transf.localPosition.x < enemyPos.x + recoilDistance * (i+1))
			{
				activeEnemy.unit.transf.localPosition += Time.deltaTime * recoilDistance / recoilTime * -background.direction;
				yield return null;
			}

			yield return new WaitForSeconds(recoilTime-.2f);

		}

		activePlayer.unit.animat = CBKUnit.AnimationType.IDLE;
		activeEnemy.unit.animat = CBKUnit.AnimationType.IDLE;

		while(activeEnemy.unit.transf.localPosition.x > enemyPos.x)
		{
			activeEnemy.unit.transf.localPosition -= Time.deltaTime * recoilDistance / recoilTime * -background.direction * 3;
			yield return null;
		}

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
	IEnumerator DamageAnimations(int damage, MonsterProto.MonsterElement element)
	{
		boardTint.PlayForward();

		//Debug.Log("Lock: Animating");
		PZPuzzleManager.instance.swapLock += 1;

		//TODO: Words?

		float score = damage/activePlayer.monster.totalDamage;

		yield return StartCoroutine(ShowAttackWords(score));

		yield return StartCoroutine(PlayerShoot(score));

		
		yield return StartCoroutine(activeEnemy.TakeDamage(damage, element));
		
		//Enemy attack back if not dead
		if (activeEnemy.monster.currHP > 0 && activeEnemy.monster.currHP < activeEnemy.monster.maxHP)
		{
			
			activeEnemy.unit.animat = CBKUnit.AnimationType.ATTACK;
			yield return new WaitForSeconds(.5f);

			Debug.Log("Dealing Player Damage");
			StartCoroutine(activePlayer.TakeDamage(Mathf.RoundToInt(activeEnemy.monster.totalDamage * Random.Range(1.0f, 4.0f)), element));

			CheckBleed(activePlayer);

			yield return new WaitForSeconds(.3f);
			
			activeEnemy.unit.animat = CBKUnit.AnimationType.IDLE;
		
		}
		
		//Debug.Log("Unlock: Done Animating");
		PZPuzzleManager.instance.swapLock -= 1;
		
		currTurn = 0;

		if (CBKEventManager.Puzzle.OnTurnChange != null)
		{
			CBKEventManager.Puzzle.OnTurnChange(playerTurns);
		}

		if (activeEnemy.alive)
		{
			if (CBKEventManager.Puzzle.OnNewPlayerTurn != null)
			{
				CBKEventManager.Puzzle.OnNewPlayerTurn();
			}
		}

		boardTint.PlayReverse();

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
		request.attackerElo = MSWhiteboard.localUser.elo;
		request.clientTime = CBKUtil.timeNowMillis;

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

		nextPvpDefenderIndex = 0;
	}
	
	int[] PickEnemyGems()
	{
		int num = Random.Range(1,4);
		int type = Random.Range (0,5);
		
		int[] gems = new int[5];
		gems[type] = num;
		
		return gems;
	}
}
