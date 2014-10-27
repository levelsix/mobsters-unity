using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSAnimationEvents : MonoBehaviour {

	int _totalAttacks;

	int _additionalAttacks;

	int _consecutiveAttacks = 0;

	public static int curDamage;
	public static Element curElement;

	PZCombatManager _combatManager;

	bool _waitOnEnemyAttack = false;

	public int totalAttacks
	{
		set
		{
			_totalAttacks = value;
			_additionalAttacks = value - 1;
			_consecutiveAttacks = 0;
			_combatManager = PZCombatManager.instance;
		}
	}

	Animator animate;

	void Awake(){
		_totalAttacks = 0;
		animate = GetComponent<Animator> ();
		_combatManager = PZCombatManager.instance;
	}

	public void shotFired()
	{
		//if _totalAttack is == 0 then this is an NPC and not the player
		if (_totalAttacks > 0) {
			//this stop coroutine line probably doesn't do anything.  90% sure.
			StopCoroutine(_combatManager.EnemyFlinch(_consecutiveAttacks));
			StartCoroutine (_combatManager.EnemyFlinch (++_consecutiveAttacks));
		}
		else
		{
			StartCoroutine(_combatManager.PlayerFlinch());
		}
	}

	public void endOfAttack(float startTime)
	{
		Debug.Log("End?");
		//if _totalAttack is == 0 then this is an NPC and not the player
		if (_totalAttacks > 0) {
			if (_additionalAttacks > 0) {
				_additionalAttacks--;
				animate.Play ("AttackFarLeft", 0, startTime);
			} else {
				Debug.Log("Badabum");
				StartCoroutine (_combatManager.EnemyReturnToStartPosition ());
				StartCoroutine (_combatManager.ReturnPlayerAfterAttack());
			}
		}
		else
		{
			_waitOnEnemyAttack = false;
		}
	}

	public IEnumerator WaitForEndOfEnemyAttack()
	{
		if (animate.runtimeAnimatorController != null)
		{
			_waitOnEnemyAttack = true;
			while(_waitOnEnemyAttack)
			{
				yield return null;
			}
		}
	}
}
