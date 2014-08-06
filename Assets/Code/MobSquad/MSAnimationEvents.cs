using UnityEngine;
using System.Collections;

public class MSAnimationEvents : MonoBehaviour {

	int _totalAttacks;

	int _additionalAttacks;

	int _consecutiveAttacks = 0;

	PZCombatManager _combatManager;

	public int totalAttacks{
		set{
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

	public void shotFired(){
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

	public void endOfAttack(float startTime){
		if (_totalAttacks > 0) {
			if (_additionalAttacks > 0) {
				_additionalAttacks--;
				animate.Play ("AttackFarLeft", 0, startTime);
			} else {
				StartCoroutine (_combatManager.EnemyReturnToStartPosition ());
			}
		}
	}
}
