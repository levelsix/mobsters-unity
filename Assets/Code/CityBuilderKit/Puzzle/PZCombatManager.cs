using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;


public class PZCombatManager : MonoBehaviour {
	
	public static PZCombatManager instance;
	
	List<MonsterProto> playerGoonies = new List<MonsterProto>();
	
	Queue<MonsterProto> enemies = new Queue<MonsterProto>();
	
	[SerializeField]
	CBKUnit activePlayer;
	
	CBKUnit activeEnemy;
	
	[SerializeField]
	CBKUnit unitPrefab;
	
	[SerializeField]
	PZScrollingBackground background;
	
	[SerializeField]
	Transform combatParent;
	
	/// <summary>
	/// The Y value at which the enemy needs to be for the scrolling to stop happening
	/// </summary>
	const float enemyYThreshold = 250;
	
	/// <summary>
	/// Gets the enemy spawn position.
	/// Offset from the player using the same values as the background
	/// </summary>
	/// <value>
	/// The enemy spawn position.
	/// </value>
	public static readonly Vector3 enemySpawnPosition = new Vector3(464, 511);
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		StartCoroutine(ScrollToNextEnemy());
	}
	
	
	
	IEnumerator ScrollToNextEnemy()
	{
		PZPuzzleManager.instance.swapLock += 1;
		
		activePlayer.animat = CBKUnit.AnimationType.RUN;
		
		activeEnemy = CBKPoolManager.instance.Get(unitPrefab, Vector3.zero) as CBKUnit;
		activeEnemy.transf.parent = combatParent;
		activeEnemy.transf.localScale = Vector3.one;
		activeEnemy.transf.localPosition = enemySpawnPosition;
		activeEnemy.direction = CBKValues.Direction.WEST;
		activeEnemy.animat = CBKUnit.AnimationType.IDLE;
		
		activeEnemy.spriteBaseName = "ClownBoss";
		
		while(activeEnemy.transf.localPosition.y > enemyYThreshold)
		{
			background.Scroll(activeEnemy);
			yield return null;
		}
		
		activePlayer.animat = CBKUnit.AnimationType.IDLE;
		
		PZPuzzleManager.instance.swapLock -= 1;
	}
	
	public void OnBreakGems(int[] gemsBroken, int combo)
	{
		int damage = CalculateDamage(gemsBroken);
		
		Debug.Log("Damage: " + damage + ", Combo: " + combo);
		
		if (damage > 0)
		{
			activePlayer.animat = CBKUnit.AnimationType.ATTACK;
			activeEnemy.animat = CBKUnit.AnimationType.FLINCH;
		}
	}
	
	int CalculateDamage(int[] gems)
	{
		int total = 0;
		foreach (int item in gems) 
		{
			total += item;
		}
		return total;
	}
	
	void PlayerDealDamage()
	{
		
	}
}
