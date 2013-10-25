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
	public List<PZMonster> playerGoonies = new List<PZMonster>();
	
	/// <summary>
	/// The remaining enemies. This is populated during level loading,
	/// and dequeued whenever we need another enemy. When this is empty
	/// and an enemy is defeated, the dungeon is complete.
	/// </summary>
	public Queue<PZMonster> enemies = new Queue<PZMonster>();
	
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
	
	[SerializeField]
	PZDeployPopup deployPopup;
	
	/// <summary>
	/// The popup that displays when the player fails a mission
	/// </summary>
	[SerializeField]
	UITweener losePopup;
	
	[SerializeField]
	UITweener winPopup;
	
	/// <summary>
	/// Awake this instance. Set up instance reference.
	/// </summary>
	void Awake()
	{
		instance = this;
	}
	
	void OnEnable()
	{
		CBKEventManager.Scene.OnPuzzle += OnPuzzle;
		CBKEventManager.Puzzle.OnDeploy += OnDeploy;
		activePlayer.OnDeath += OnPlayerDeath;
	}
	
	void OnDisable()
	{
		CBKEventManager.Scene.OnPuzzle -= OnPuzzle;
		CBKEventManager.Puzzle.OnDeploy -= OnDeploy;
		activePlayer.OnDeath -= OnPlayerDeath;
	}
	
	/// <summary>
	/// Start this instance. Gets the combat going.
	/// </summary>
	void OnPuzzle()
	{
		winPopup.gameObject.SetActive(false);
		losePopup.gameObject.SetActive(false);
		
		if (activeEnemy != null)
		{
			activeEnemy.unit.sprite.alpha = 0;
		}
		
		foreach (PZMonster monster in CBKMonsterManager.instance.userTeam)
		{
			playerGoonies.Add(monster);
		}
		
		CBKEventManager.Popup.OnPopup(deployPopup.gameObject);
		deployPopup.Init(playerGoonies);
		
		//Lock swap until deploy
		PZPuzzleManager.instance.swapLock += 1;
	}
	
	void OnDeploy(PZMonster monster)
	{
		PZPuzzleManager.instance.swapLock -= 1;
		CBKEventManager.Popup.CloseAllPopups();
		activePlayer.Init(monster);
		StartCoroutine(ScrollToNextEnemy());
	}
	
	void OnEnemyDeath()
	{
		StartCoroutine(ScrollToNextEnemy());
	}
	
	void OnPlayerDeath()
	{
		//Debug.Log("Lock: Player death");
		PZPuzzleManager.instance.swapLock += 1;
		
		foreach (var goon in playerGoonies)
		{
			if (goon.currHP > 0)
			{
				CBKEventManager.Popup.OnPopup(deployPopup.gameObject);
				deployPopup.Init(playerGoonies);
				return;
			}
		}
		
		losePopup.gameObject.SetActive(true);
		losePopup.Play();
		
		StartCoroutine(SendEndResult(false));
	}
	
	/// <summary>
	/// Spawns a new enemy and scrolls the background until
	/// that enemy is in its place.
	/// </summary>
	IEnumerator ScrollToNextEnemy()
	{
		//Debug.Log("Lock: Scrolling");
		PZPuzzleManager.instance.swapLock += 1;
		
		
		if (enemies.Count > 0)
		{
			activePlayer.unit.animat = CBKUnit.AnimationType.RUN;
			
			if (activeEnemy == null)
			{
				activeEnemy = (CBKPoolManager.instance.Get(unitPrefab, Vector3.zero) as CBKUnit).GetComponent<PZCombatUnit>();
				activeEnemy.OnDeath += OnEnemyDeath;
			}
			activeEnemy.unit.transf.parent = combatParent;
			activeEnemy.unit.transf.localScale = Vector3.one;
			activeEnemy.unit.transf.localPosition = enemySpawnPosition;
			activeEnemy.unit.direction = CBKValues.Direction.WEST;
			activeEnemy.unit.animat = CBKUnit.AnimationType.IDLE;
			
			activeEnemy.Init(enemies.Dequeue());
			
			
			while(activeEnemy.unit.transf.localPosition.y > enemyYThreshold)
			{
				background.Scroll(activeEnemy.unit);
				yield return null;
			}
			
			activePlayer.unit.animat = CBKUnit.AnimationType.IDLE;
		}
		else
		{
			activePlayer.unit.animat = CBKUnit.AnimationType.IDLE;
			
			winPopup.gameObject.SetActive(true);
			winPopup.Play();
			StartCoroutine(SendEndResult(true));
		}
		
		//Debug.Log("Unlock: Done Scrolling");
		PZPuzzleManager.instance.swapLock -= 1;
	}
	
	IEnumerator SendEndResult(bool userWon)
	{
		EndDungeonRequestProto request = new EndDungeonRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.userTaskId = CBKWhiteboard.currTaskID;
		request.userWon = userWon;
		request.clientTime = CBKUtil.timeNowMillis;
		
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
		//Debug.Log("Lock: Animating");
		PZPuzzleManager.instance.swapLock += 1;
		
		activePlayer.unit.animat = CBKUnit.AnimationType.ATTACK;
		
		yield return new WaitForSeconds(0.4f);
		
		activeEnemy.TakeDamage(damage, element);
		activeEnemy.unit.animat = CBKUnit.AnimationType.FLINCH;
		
		yield return new WaitForSeconds(0.7f);
		
		//Enemy attack back if not dead
		if (activeEnemy.monster.currHP > 0)
		{
		
			activeEnemy.unit.animat = CBKUnit.AnimationType.IDLE;
			
			yield return new WaitForSeconds(0.5f);
			
			activeEnemy.unit.animat = CBKUnit.AnimationType.ATTACK;
			activeEnemy.DealDamage(PickEnemyGems(), out damage, out element);
			activePlayer.TakeDamage(damage, element);
			
			yield return new WaitForSeconds(0.4f);
			
			activePlayer.unit.animat = CBKUnit.AnimationType.FLINCH;
			
			yield return new WaitForSeconds(0.7f);
			
			activePlayer.unit.animat = CBKUnit.AnimationType.IDLE;
		
		}
		
		//Debug.Log("Unlock: Done Animating");
		PZPuzzleManager.instance.swapLock -= 1;
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
