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

	[SerializeField]
	CBKFillBar hpBar;

	[SerializeField]
	UILabel hpLabel;

	[SerializeField]
	UISprite shadow;

	[SerializeField]
	UILabel damageLabel;

	[SerializeField]
	UITweener[] damageLabelTweens;

	[SerializeField]
	public Vector3 startingPos;

	public bool alive = false;

	const float HP_LERP_FRAME = 1f;

	[ContextMenu("SetStartPos")]
	void SetStartingPos()
	{
		startingPos = transform.localPosition;
	}

	public void GoToStartPos()
	{
		transform.localPosition = startingPos;
	}

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
		alive = true;

		this.monster = monster;
		unit.spriteBaseName = monster.monster.imagePrefix;
		unit.sprite.color = new Color(unit.sprite.color.r, unit.sprite.color.g, unit.sprite.color.b, 1);
		shadow.alpha = 1;

		hpBar.fill = ((float)monster.currHP) / monster.maxHP;
		hpLabel.text = monster.currHP + "/" + monster.maxHP;
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
			damage += (int)(gems[i] * monster.attackDamages[(i>4 ? i-5 : i)]);
		}
		
		element = monster.monster.monsterElement;
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
	public IEnumerator TakeDamage(int damage, MonsterProto.MonsterElement element)
	{
		int fullDamage = (int)(damage * CBKUtil.GetTypeDamageMultiplier(monster.monster.monsterElement, element));
		
		//TODO: If fullDamage != damage, do some animation or something to reflect super/notvery effective
		
		RunDamageLabel(fullDamage);

		yield return StartCoroutine(LerpHealth(monster.currHP, Mathf.Max(monster.currHP - fullDamage, 0), monster.maxHP));
		
		monster.currHP -= fullDamage;


		if (monster.userMonster != null)
		{
			StartCoroutine(SendHPUpdateToServer());
		}
		
		if (monster.currHP <= 0)
		{
			yield return StartCoroutine(Die());
		}
	}

	IEnumerator LerpHealth(float hpBeforeDamage, float hpAfterDamage, int maxHP)
	{
		int frames = Mathf.Min((int)(hpBeforeDamage - hpAfterDamage), Application.targetFrameRate * 2);
		float currFrame = 0;
		while (currFrame < frames)
		{
			currFrame++;
			hpBar.fill = Mathf.Lerp(hpBeforeDamage/maxHP, hpAfterDamage/maxHP, currFrame/frames);
			hpLabel.text = ((int)Mathf.Lerp(hpBeforeDamage, hpAfterDamage, currFrame/frames)) + "/" + maxHP;
			yield return null;
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
		
		alive = false;
		
		unit.sprite.color = new Color(unit.sprite.color.r, unit.sprite.color.g, unit.sprite.color.b, 0);
		shadow.alpha = 0;

		CBKPoolManager.instance.Get(CBKPrefabList.instance.characterDieParticle, unit.transf.position);

		if (monster.taskMonster != null && monster.taskMonster.monsterId > 0 && monster.taskMonster.puzzlePieceDropped)
		{
			Transform crate = (CBKPoolManager.instance.Get(CBKPrefabList.instance.cratePrefab, unit.transf.position) as MonoBehaviour).transform;
			PZCombatManager.instance.crate = crate.GetComponent<PZCrate>();
			crate.parent = unit.transf.parent;
			crate.localScale = new Vector3(50,50,1);
		}

		yield return new WaitForSeconds(1);

		PZPuzzleManager.instance.swapLock -= 1;

		if (OnDeath != null)
		{
			OnDeath();
		}

	}

	void RunDamageLabel(int damage)
	{
		damageLabel.text = "-" + damage;
		foreach (var item in damageLabelTweens) 
		{
			item.ResetToBeginning();
			item.PlayForward();
		}
	}
}
