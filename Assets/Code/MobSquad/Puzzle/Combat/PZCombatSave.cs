﻿using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[System.Serializable]
public class PZCombatSave 
{
	public long activePlayerUserMonsterId;

	public int activeEnemyHealth = 1;

	public int[,] gemColors;
	public int[,] gemTypes;

	public BattleStats battleStats = new BattleStats();

	public float forfeitChance = .5f;

	public int currTurn = 0;
	public int currPlayerDamage = 0;

	const string key = "CombatSave";

	public PZCombatSave(){}

	public PZCombatSave(PZMonster activePlayer, int activeEnemyHealth,
	                 PZGem[,] board, BattleStats battleStats,
	                    float forfeitChance, int currTurn, int currPlayerDamage,
	                    int boardWidth, int boardHeight)
	{
		this.activePlayerUserMonsterId = activePlayer.userMonster.userMonsterId;

		this.activeEnemyHealth = activeEnemyHealth;

		gemColors = new int[boardWidth, boardHeight];
		gemTypes = new int[boardWidth, boardHeight];

		for (int x = 0; x < boardWidth; x++) {
			for (int y = 0; y < boardHeight; y++) {
				gemColors[x,y] = board[x,y].colorIndex;
				gemTypes[x,y] = (int)board[x,y].gemType;
			}
		}

		this.battleStats = battleStats;

		this.currTurn = currTurn;
		this.currPlayerDamage = currPlayerDamage;

		Debug.LogWarning("Saving!" +
		                 "\nActive player: " + activePlayerUserMonsterId +
		                 "\nEnemy HP: " + activeEnemyHealth +
		                 "\nCurr Turn: " + currTurn +
		                 "\nCurr Player Damage: " + currPlayerDamage);

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
