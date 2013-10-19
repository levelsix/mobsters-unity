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
	
	/// <summary>
	/// The player goonies that they brought into combat with them.
	/// </summary>
	List<MonsterProto> playerGoonies = new List<MonsterProto>();
	
	/// <summary>
	/// The remaining enemies. This is populated during level loading,
	/// and dequeued whenever we need another enemy. When this is empty
	/// and an enemy is defeated, the dungeon is complete.
	/// </summary>
	Queue<MonsterProto> enemies = new Queue<MonsterProto>();
	
	/// <summary>
	/// The player's active goonie, who deals and takes damage.
	/// </summary>
	[SerializeField]
	PZCombatUnit activePlayer;
	
	/// <summary>
	/// The active enemy being fought and dealt damage by the player
	/// </summary>
	PZCombatUnit activeEnemy;
	
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
	PZScrollingBackground background;
	
	/// <summary>
	/// The combat parent. Generated units will use this as their parent
	/// to keep the heriarchy organized, as well as ensure that coordiantes
	/// won't get messed up
	/// </summary>
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
	
	/// <summary>
	/// Awake this instance. Set up instance reference.
	/// </summary>
	void Awake()
	{
		instance = this;
	}
	
	/// <summary>
	/// Start this instance. Gets the combat going.
	/// TODO: Change this to an init that gets called when we switch to the combat view
	/// </summary>
	void Start()
	{
		StartCoroutine(ScrollToNextEnemy());
	}
	
	void OnEnemyDeath()
	{
		activeEnemy.OnDeath -= OnEnemyDeath;
		StartCoroutine(ScrollToNextEnemy());
	}
	
	/// <summary>
	/// Spawns a new enemy and scrolls the background until
	/// that enemy is in its place.
	/// </summary>
	IEnumerator ScrollToNextEnemy()
	{
		PZPuzzleManager.instance.swapLock += 1;
		
		activePlayer.unit.animat = CBKUnit.AnimationType.RUN;
		
		activeEnemy = (CBKPoolManager.instance.Get(unitPrefab, Vector3.zero) as CBKUnit).GetComponent<PZCombatUnit>();
		activeEnemy.unit.transf.parent = combatParent;
		activeEnemy.unit.transf.localScale = Vector3.one;
		activeEnemy.unit.transf.localPosition = enemySpawnPosition;
		activeEnemy.unit.direction = CBKValues.Direction.WEST;
		activeEnemy.unit.animat = CBKUnit.AnimationType.IDLE;
		activeEnemy.hp = 100;
		activeEnemy.unit.spriteBaseName = "Cheerleader1SMG";
		
		activeEnemy.OnDeath += OnEnemyDeath;
		
		while(activeEnemy.unit.transf.localPosition.y > enemyYThreshold)
		{
			background.Scroll(activeEnemy.unit);
			yield return null;
		}
		
		activePlayer.unit.animat = CBKUnit.AnimationType.IDLE;
		
		PZPuzzleManager.instance.swapLock -= 1;
	}
	
	/// <summary>
	/// Attached to the OnBreakGems event.
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
		
		Debug.Log("Damage: " + damage + ", Combo: " + combo);
		
		if (combo > 1)
		{
			damage = (int)(damage * (1 + (combo-1) / 4f));
			Debug.Log("Combo damage: " + damage);
		}
		
		StartCoroutine(DamageAnimations(damage, element));
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
		
		activePlayer.unit.animat = CBKUnit.AnimationType.ATTACK;
		
		yield return new WaitForSeconds(0.4f);
		
		activeEnemy.TakeDamage(damage, element);
		activeEnemy.unit.animat = CBKUnit.AnimationType.FLINCH;
		
		yield return new WaitForSeconds(0.7f);
		
		activeEnemy.unit.animat = CBKUnit.AnimationType.IDLE;
	}
}
