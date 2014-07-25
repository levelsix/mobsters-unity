using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

/// <summary>
/// @author Rob Giusti
/// Component for handling all damage dealing and taking done by units.
/// </summary>
[RequireComponent (typeof(MSUnit))]
public class PZCombatUnit : MonoBehaviour {

	/// <summary>
	/// The unit component
	/// </summary>
	public MSUnit unit;
	
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
	MSFillBar hpBar;

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

	public bool moving = false;

	const float HP_LERP_FRAME = 1f;

	/// <summary>
	/// When this unit is clicked, info appears above it's head
	/// </summary>
	[SerializeField]
	UIWidget unitInfo;

	[SerializeField]
	UILabel unitName;

	[SerializeField]
	UILabel unitLevelInfo;

	[SerializeField]
	UISprite unitRarity;
	
	const float INFO_DISPLAY_TIME = 3f;

	float currTotalTime = 0f;

	[SerializeField]
	UISprite forfeitSprite;

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
		unit = GetComponent<MSUnit>();
	}
	
	/// <summary>
	/// Init this combat unit that represents an goon
	/// </summary>
	/// <param name='proto'>
	/// The Monster Proto to build the unit from
	/// </param>
	public void Init(PZMonster monster)
	{
		alive = true;

		this.monster = monster;
		Init();

	}

	public void Init(MinimumUserMonsterProto pvpMonster)
	{
		if (pvpMonster != null)
		{
			alive = true;
			this.monster = new PZMonster(pvpMonster);
			Init ();
		}
		else
		{
			this.monster = null;
			alive = false;
			unit.sprite.color = new Color (unit.sprite.color.r, unit.sprite.color.g, unit.sprite.color.b, 0);
		}
	}

	public void DeInit()
	{
		this.monster = null;
		alive = false;
		unit.sprite.color = new Color (unit.sprite.color.r, unit.sprite.color.g, unit.sprite.color.b, 0);
	}

	void Init()
	{
		unit.spriteBaseName = monster.monster.imagePrefix;
		unit.sprite.color = new Color (unit.sprite.color.r, unit.sprite.color.g, unit.sprite.color.b, 1);
		shadow.alpha = 1;
		hpBar.fill = ((float)monster.currHP) / monster.maxHP;
		hpLabel.text = monster.currHP + "/" + monster.maxHP;

		unitName.text = monster.monster.shorterName;
		unitLevelInfo.text = "L" + monster.level;
		unitRarity.spriteName = "";

		if (monster.monster.quality != Quality.COMMON)
		{
			unitRarity.spriteName = "battle" + monster.monster.quality.ToString().ToLower() + "tag";
			unitRarity.MakePixelPerfect();
		}

		Color healthColor;
		switch (monster.monster.monsterElement) {
		case Element.DARK:
			healthColor = new Color(172f/255f, 90f/255f, 217f/255f);
			break;
		case Element.EARTH:
			healthColor = new Color(90f/255f,162f/255f,15f/255f);
			break;
		case Element.FIRE:
			healthColor = new Color(215f/255f, 46f/255f, 0f/255f);
			break;
		case Element.LIGHT:
			healthColor = new Color(1f, 1f, 0f);
			break;
		case Element.ROCK:
			healthColor = new Color(171f/255f, 171f/255f, 171f/255f);
			break;
		case Element.WATER:
			healthColor = new Color(79f/255f,199f/255f,234f/255f);
			break;
		default:
			healthColor = new Color(0f,0f,0f);
			break;
		}

		hpBar.GetComponent<UISprite> ().color = healthColor;
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
	public void DealDamage(int[] gems, out int damage, out Element element)
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
	public IEnumerator TakeDamage(int damage, Element element)
	{
		int fullDamage = (int)(damage * MSUtil.GetTypeDamageMultiplier(monster.monster.monsterElement, element));
		
		//Debug.Log(name + " taking " + fullDamage + " damage");

		//TODO: If fullDamage != damage, do some animation or something to reflect super/not very effective
		
		yield return StartCoroutine(TakeDamage(fullDamage));
	}

	public IEnumerator TakeDamage(int damage)
	{
		RunDamageLabel(damage);

		alive = damage >= monster.currHP;
		
		float startHP = monster.currHP;

		monster.currHP = Mathf.Max(monster.currHP - damage, 0);
		
		yield return StartCoroutine(LerpHealth(Math.Min(monster.currHP + damage, startHP), Mathf.Max(monster.currHP, 0), monster.maxHP));

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

	public void SendDamageUpdateToServer(int damage)
	{
		UpdateMonsterHealthRequestProto request = new UpdateMonsterHealthRequestProto();
		request.sender = MSWhiteboard.localMup;
		
		UserMonsterCurrentHealthProto hpProto = new UserMonsterCurrentHealthProto();
		hpProto.userMonsterId = monster.userMonster.userMonsterId;
		hpProto.currentHealth = Mathf.Max(monster.currHP - damage, 0);
		
		request.umchp.Add(hpProto);

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UPDATE_MONSTER_HEALTH_EVENT, DealWithHPUpdateResponse);
	}

	void DealWithHPUpdateResponse(int tagNum)
	{
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

		MSPoolManager.instance.Get(MSPrefabList.instance.characterDieParticle, unit.transf.position);

		if (monster.taskMonster != null && monster.taskMonster.monsterId > 0 && (monster.taskMonster.puzzlePieceDropped || monster.taskMonster.itemId > 0))
		{
			Transform crate = (MSPoolManager.instance.Get(MSPrefabList.instance.cratePrefab, unit.transf.position) as MonoBehaviour).transform;
			PZCombatManager.instance.crate = crate.GetComponent<PZCrate>();
			PZCombatManager.instance.crate.initCrate(monster);
			crate.transform.position = unit.transf.position;
			crate.parent = unit.transf.parent;
			crate.transform.localScale = Vector3.one;
		}

		yield return new WaitForSeconds(1);

		PZPuzzleManager.instance.swapLock -= 1;

		if (OnDeath != null)
		{
			OnDeath();
		}

	}

	public IEnumerator AdvanceTo(float x, Vector3 direction, float speed, bool idleAfter = true)
	{
		moving = true;
		unit.animat = MSUnit.AnimationType.RUN;
		if (transform.localPosition.x <= x)
		{
			unit.direction = MSValues.Direction.EAST;
			while (transform.localPosition.x < x)
			{
				transform.localPosition += direction * speed * Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			unit.direction = MSValues.Direction.WEST;
			while (transform.localPosition.x > x)
			{
				transform.localPosition -= direction * speed * Time.deltaTime;
				yield return null;
			}
		}
		transform.localPosition += (x - transform.localPosition.x) * direction;
		if (idleAfter)
		{
			unit.animat = MSUnit.AnimationType.IDLE;
		}
		transform.localPosition = new Vector3 (x, transform.localPosition.y, transform.localPosition.z);
		moving = false;
	}

	public IEnumerator Retreat(Vector3 direction, float speed)
	{
		yield return StartCoroutine(AdvanceTo(startingPos.x, direction, speed));
	}

	public IEnumerator Forfeit(bool successfulForfeit)
	{
		forfeitSprite.transform.localPosition = Vector3.zero;

		Transform oldParent = forfeitSprite.transform.parent;
		forfeitSprite.transform.parent = transform.parent;
		if (successfulForfeit) {
			forfeitSprite.spriteName = "runawaysuccess";
			forfeitSprite.MakePixelPerfect();
		} else {
			forfeitSprite.spriteName = "runawayfailed";
			forfeitSprite.MakePixelPerfect();
		}

		TweenAlpha alpha = forfeitSprite.GetComponent<TweenAlpha> ();
		TweenScale scale = forfeitSprite.GetComponent<TweenScale> ();

		alpha.ResetToBeginning ();
		alpha.PlayForward ();
		scale.ResetToBeginning ();
		scale.PlayForward ();

		Vector3 start = forfeitSprite.transform.localPosition;

		float currTime = 0f;
		while (currTime < alpha.duration) {
			forfeitSprite.transform.localPosition = new Vector3(start.x, start.y + (200f * (currTime/alpha.duration)), start.z);
			currTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		//TweenPosition.Begin (forfeitSprite.gameObject, alpha.duration, new Vector3 (start.x + 0f, start.y + 200.0f, start.z + 0f));

		yield return new WaitForSeconds (alpha.duration);

		forfeitSprite.transform.parent = oldParent;
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

	public void OnClick(){
		if (currTotalTime == 0f && !unitInfo.GetComponent<TweenAlpha> ().enabled) {
			StartCoroutine (showInfo ());	
		} else if (currTotalTime > 0) {
			currTotalTime -= INFO_DISPLAY_TIME;
		} else {
			unitInfo.GetComponent<TweenAlpha> ().PlayForward();
			currTotalTime = -0.5f;
			StartCoroutine(showInfo());
		}
	}

	IEnumerator showInfo(){
		float totalDisplayTime = INFO_DISPLAY_TIME;
		
		TweenAlpha tween = unitInfo.GetComponent<TweenAlpha> ();
		if(currTotalTime >= 0){
			tween.Sample(0f, false);
			tween.PlayForward();
		}

		while (totalDisplayTime > currTotalTime) {
			currTotalTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		tween.Sample (1f, false);
		tween.PlayReverse ();
		currTotalTime = 0f;
	}
}
