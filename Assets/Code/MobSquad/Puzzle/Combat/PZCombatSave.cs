using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

[System.Serializable]
public class PZCombatSave 
{
	public int activePlayerSlot;

	public int activeEnemyTaskStageId;
	public int activeEnemyHealth;

	public int[,] gemColors;
	public int[,] gemTypes;

	public BeginDungeonResponseProto ho;

	const string key = "CombatSave";

	public PZCombatSave(int activePlayerSlot, PZMonster activeEnemy,
	                 PZGem[,] board)
	{
		this.activePlayerSlot = activePlayerSlot;

		ho = MSWhiteboard.loadedDungeon;

		MSUtil.Save(key, this);
	}

	/// <summary>
	/// Load up the combat save
	/// </summary>
	public static PZCombatSave Load()
	{
		if (PlayerPrefs.HasKey(key))
		{
			return MSUtil.Load<PZCombatSave>(key);
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Call this on the win/lose screen appear to wipe all of the
	/// current save data
	/// </summary>
	public static void Wipe()
	{
		MSUtil.Save(key, null);
	}
}
