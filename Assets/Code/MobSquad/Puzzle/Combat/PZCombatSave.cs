using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

[System.Serializable]
public class PZCombatSave 
{
	public string activePlayerUserMonsterUuid;

	public int activeEnemyHealth = 1;

	public int[,] gemColors;
	public int[,] gemTypes;
	public int[,] bombTicks;
	public int[,] bombDamages;
	public bool[,] jelly;

	public BattleStats battleStats = new BattleStats();

	public float forfeitChance = .5f;

	public int currTurn = 0;
	public int currPlayerDamage = 0;

	public List<CombatTurn> turns;
	public int currTurnIndex;

	public SkillSaveData playerSkillSave = new SkillSaveData();
	public SkillSaveData enemySkillSave = new SkillSaveData();

	public float enemySpeed; //In case it's been modified by Cake

	public int playerSkillPoints;
	public int enemySkillPoints;
	
	const string key = "CombatSave";

	public string userTaskUuid;

	public int totalEnemies;

	public PZCombatSave(){}

	public PZCombatSave(PZMonster activePlayer, int activeEnemyHealth,
	                 PZGem[,] board, BattleStats battleStats,
	                    float forfeitChance, int currTurn, int currPlayerDamage,
	                    int boardWidth, int boardHeight, int playerSkillPoints, int enemySkillPoints,
	                    PZCombatUnit player, PZCombatUnit enemy)
	{
		this.activePlayerUserMonsterUuid = activePlayer.userMonster.userMonsterUuid;

		this.activeEnemyHealth = activeEnemyHealth;

		gemColors = new int[boardWidth, boardHeight];
		gemTypes = new int[boardWidth, boardHeight];
		bombTicks = new int[boardWidth, boardHeight];
		bombDamages = new int[boardWidth, boardHeight];
		jelly = new bool[boardWidth, boardHeight];

		for (int x = 0; x < boardWidth; x++) {
			for (int y = 0; y < boardHeight; y++) {
				gemColors[x,y] = board[x,y].colorIndex;
				if (board[x,y].gemType == PZGem.GemType.ROCKET)
				{
					gemTypes[x,y] = (int)(board[x,y].horizontal ? PZGem.GemType.HORIZONTAL_ROCKET : PZGem.GemType.VERTICAL_ROCKET);
				}
				else
				{
					gemTypes[x,y] = (int)board[x,y].gemType;
					if (board[x,y].gemType == PZGem.GemType.BOMB)
					{
						bombTicks[x,y] = board[x,y].bombTicks;
						bombDamages[x,y] = board[x,y].bombDamage;
					}
				}
				jelly[x,y] = PZPuzzleManager.instance.jellyBoard[x,y] != null;
			}
		}

		this.battleStats = battleStats;

		this.currTurn = currTurn;
		this.currPlayerDamage = currPlayerDamage;

		this.forfeitChance = forfeitChance;

		Debug.LogWarning("Saving!"
		                 //+ "\nActive player: " + activePlayerUserMonsterId +
		                 //+ "\nEnemy HP: " + activeEnemyHealth +
		                 + "\nCurr Turn: " + currTurn
		                 //+ "\nCurr Player Damage: " + currPlayerDamage
		                 );


		turns = PZCombatScheduler.instance.turns;
		currTurnIndex = PZCombatScheduler.instance.currInd;

		this.playerSkillPoints = playerSkillPoints;
		this.enemySkillPoints = enemySkillPoints;

		playerSkillSave = new SkillSaveData(player);
		enemySkillSave = new SkillSaveData(enemy);

		enemySpeed = enemy.monster.speed;

		this.userTaskUuid = MSWhiteboard.currUserTaskUuid;

		totalEnemies = PZCombatManager.instance.totalEnemies;

		MSUtil.Save(key, this);
	}

	public void SaveBoard(PZGem[,] board, int boardHeight, int boardWidth)
	{
		gemColors = new int[boardWidth, boardHeight];
		gemTypes = new int[boardWidth, boardHeight];
		jelly = new bool[boardWidth, boardHeight];

		for (int x = 0; x < boardWidth; x++) {
			for (int y = 0; y < boardHeight; y++) {
				gemColors[x,y] = board[x,y].colorIndex;
				gemTypes[x,y] = (int)board[x,y].gemType;
				jelly[x,y] = PZPuzzleManager.instance.jellyBoard[x,y] != null;
			}
		}
	}

	public static void Delete()
	{
		MSUtil.Save(key, null);
	}

	public void Save()
	{
		MSUtil.Save(key, this);
	}

	/// <summary>
	/// Load up the combat save
	/// </summary>
	public static PZCombatSave Load()
	{
		Debug.Log("Trying to load...");
		if (PlayerPrefs.HasKey(key))
		{
			Debug.Log("Success!");
			return MSUtil.Load<PZCombatSave>(key);
		}
		else
		{
			Debug.Log("Fail!");
			return null;
		}
	}
}

[System.Serializable]
public class SkillSaveData
{
	public float damageMultiplier;
	public bool skillActive;
	public int shieldHealth;

	public SkillSaveData (){}

	public SkillSaveData(PZCombatUnit unit)
	{
		shieldHealth = unit.shieldHealth;
		skillActive = unit.skillActive;
		damageMultiplier = unit.damageMultiplier;
	}
}
