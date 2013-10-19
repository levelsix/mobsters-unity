using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

/// <summary>
/// @author Rob Giusti
/// Component for handling all damage dealing and taking done by units.
/// </summary>
[RequireComponent (typeof(CBKUnit))]
public class PZCombatUnit : MonoBehaviour {
	
	/// <summary>
	/// The unit component
	/// </summary>
	public CBKUnit unit;
	
	/// <summary>
	/// The monster proto
	/// </summary>
	public MonsterProto monster;
	
	/// <summary>
	/// The current running hp of this unit.
	/// </summary>
	public int hp = 100;
	
	/// <summary>
	/// The on death event.
	/// Mainly used to hook up to the CombatManager to communicate
	/// unit death.
	/// </summary>
	public Action OnDeath;
	
	/// <summary>
	/// Awake this instance and set up component references
	/// </summary>
	void Awake()
	{
		unit = GetComponent<CBKUnit>();
	}
	
	/// <summary>
	/// Init this combat unit that represents an unowned goon
	/// </summary>
	/// <param name='proto'>
	/// The Monster Proto to build the unit from
	/// </param>
	public void Init(MonsterProto proto)
	{
		this.monster = proto;
		
		hp = proto.maxHp;
	}
	
	/// <summary>
	/// Init this combat unit that represents the Player's goon or another player's goon
	/// </summary>
	/// <param name='fump'>
	/// The Full User Monster Proto to build the unit from
	/// </param>
	public void Init(FullUserMonsterProto fump)
	{
		monster = CBKDataManager.instance.Get(typeof(MonsterProto), fump.monsterId) as MonsterProto;
		
		hp = fump.currentHealth;
	}
	
	/// <summary>
	/// Calculates the damage that this unit deals, given a set of gems
	/// </summary>
	/// <param name='gems'>
	/// Gems that were broken
	/// </param>
	/// <param name='damage'>
	/// Out: Damage to be dealt
	/// </param>
	/// <param name='element'>
	/// Out: Element of the damage
	/// </param>
	public void DealDamage(int[] gems, out int damage, out MonsterProto.MonsterElement element)
	{
		damage = 0;
		for (int i = 0; i < gems.Length; i++) 
		{
			//TODO: Use monster's damage table to look up damage per gem
			damage += gems[i];
		}
		element = monster.element;
	}
	
	/// <summary>
	/// Takes the given amount of damage, applying 
	/// </summary>
	/// <param name='damage'>
	/// Damage.
	/// </param>
	/// <param name='element'>
	/// Damage element.
	/// </param>
	public void TakeDamage(int damage, MonsterProto.MonsterElement element)
	{
		hp -= damage;
		
		if (hp <= 0)
		{
			StartCoroutine(Die());
		}
	}
	
	public IEnumerator Die()
	{
		//TODO: Animation?
		yield return null;
		Debug.Log("Death!");
		
		CBKPoolManager.instance.Pool(unit);
		
		if (OnDeath != null)
		{
			OnDeath();
		}
	}
}
