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
	public PZMonster monster;
	
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
	public void Init(PZMonster monster)
	{
		this.monster = monster;
		unit.spriteBaseName = monster.monster.imagePrefix;
		unit.sprite.alpha = 1;
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
		for (int i = 0; i < monster.attackDamages.Length; i++) 
		{
			damage += (int)(gems[i] * monster.attackDamages[i]);
		}
		
		element = monster.monster.element;
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
		int fullDamage = (int)(damage * CBKUtil.GetTypeDamageMultiplier(monster.monster.element, element));
		
		//TODO: If fullDamage != damage, do some animation or something to reflect super/notvery effective
		
		monster.currHP -= fullDamage;
		if (monster.userMonster != null)
		{
			StartCoroutine(SendHPUpdateToServer());
		}
		
		if (monster.currHP <= 0)
		{
			StartCoroutine(Die());
		}
	}
	
	IEnumerator SendHPUpdateToServer ()
	{
		UpdateMonsterHealthRequestProto request = new UpdateMonsterHealthRequestProto();
		request.sender = CBKWhiteboard.localMup;
		
		UserMonsterCurrentHealthProto hpProto = new UserMonsterCurrentHealthProto();
		hpProto.userMonsterId = monster.userMonster.userMonsterId;
		hpProto.currentHealth = monster.currHP;
		
		request.umchp.Add(hpProto);
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UPDATE_MONSTER_HEALTH_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		UpdateMonsterHealthResponseProto response = UMQNetworkManager.responseDict[tagNum] as UpdateMonsterHealthResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != UpdateMonsterHealthResponseProto.UpdateMonsterHealthStatus.SUCCESS)
		{
			Debug.LogError(response.status.ToString());
		}
	}
	
	public IEnumerator Die()
	{
		//Debug.Log("Lock");
		PZPuzzleManager.instance.swapLock += 1;
		
		unit.animat = CBKUnit.AnimationType.FLINCH;
		
		float time = 0;
		while (time < 3f)
		{
			time += Time.deltaTime;
			unit.sprite.alpha = Mathf.Lerp(1, 0, time/3f);
			yield return null;
		}
		//TODO: Animation?
		yield return null;
		Debug.Log("Death!");
		
		//CBKPoolManager.instance.Pool(unit);
		PZPuzzleManager.instance.swapLock -= 1;
		
		if (OnDeath != null)
		{
			OnDeath();
		}
	}
}
