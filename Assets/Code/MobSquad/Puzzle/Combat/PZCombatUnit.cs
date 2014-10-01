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
	SpriteRenderer shadow;

	[SerializeField]
	UILabel damageLabel;

	[SerializeField]
	UITweener[] damageLabelTweens;

	[SerializeField]
	UIWidget alphaParent;

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

	public float alpha
	{
		set
		{
			alphaParent.alpha = value;
			unit.alpha = value;
		}
	}

	public int health
	{
		get
		{
			return monster.currHP;
		}
		set
		{
			hpBar.fill = ((float)value) / monster.maxHP;
			hpLabel.text = value + "/" + monster.maxHP;
			monster.currHP = value;
		}
	}

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
		if (pvpMonster != null && pvpMonster.monsterId > 0)
		{
			alive = true;
			this.monster = new PZMonster(pvpMonster);
			Init ();
		}
		else
		{
			DeInit();
		}
	}

	public void DeInit()
	{
		this.monster = null;
		alive = false;
		unit.sprite.color = new Color (unit.sprite.color.r, unit.sprite.color.g, unit.sprite.color.b, 0);
		alpha = 0;
	}

	void Init()
	{
		unit.spriteBaseName = monster.monster.imagePrefix;
		alpha = 1;
		hpBar.fill = ((float)monster.currHP) / monster.maxHP;
		hpLabel.text = monster.currHP + "/" + monster.maxHP;

		if(monster.monster.shorterName != null && monster.monster.shorterName.Length != 0)
		{
			unitName.text = monster.monster.shorterName;
			Debug.Log("name set to [" + monster.monster.shorterName +"]");
		}
		else
		{
			//if there isn't a short version on hand we display the longer name
			unitName.text = monster.monster.displayName;
		}

		unitLevelInfo.text = "L" + monster.level;
		unitRarity.spriteName = "";

		unitName.MarkAsChanged();
		unitLevelInfo.MarkAsChanged();
		unitRarity.MarkAsChanged();

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

	int CalculateDamage(int damage, Element element)
	{
		return (int)(damage * MSUtil.GetTypeDamageMultiplier(monster.monster.monsterElement, element));
	}

	public int HealthAfterDamage(int damage, Element element)
	{
		return Mathf.Max(0, monster.currHP - CalculateDamage(damage, element));
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
		int fullDamage = CalculateDamage (damage, element);

		//Debug.Log(name + " taking " + fullDamage + " damage");
		if(!MSTutorialManager.instance.inTutorial)
		{
			if(fullDamage < damage)
			{
				PZCombatManager.instance.EffectiveAttack(true);
			}
			else if(fullDamage > damage)
			{
				PZCombatManager.instance.EffectiveAttack(false);
			}
		}
		
		yield return StartCoroutine(TakeDamage(fullDamage));
	}

	public IEnumerator TakeDamage(int damage)
	{
		RunDamageLabel(damage);

		//alive = damage >= monster.currHP;
		
		float startHP = monster.currHP;

		monster.currHP = Mathf.Max(monster.currHP - damage, 0);
		alive = monster.currHP > 0;
		
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
	
	public IEnumerator Die(bool callOnDeath = true)
	{
		//Debug.Log("Lock");
		PZPuzzleManager.instance.swapLock += 1;
		
		alive = false;

		alpha = 0;

		MSPoolManager.instance.Get(MSPrefabList.instance.characterDieParticle, unit.transf.position);

		if (monster.taskMonster != null && monster.taskMonster.puzzlePieceMonsterId > 0 && (monster.taskMonster.puzzlePieceDropped || monster.taskMonster.itemId > 0))
		{
			Transform crate = (MSPoolManager.instance.Get(MSPrefabList.instance.cratePrefab, unit.transf.position) as MonoBehaviour).transform;
			PZCombatManager.instance.crate = crate.GetComponent<PZCrate>();
			MonsterProto newMonster = MSDataManager.instance.Get<MonsterProto>(monster.taskMonster.puzzlePieceMonsterId);
			PZCombatManager.instance.crate.InitCrate(monster.taskMonster);
			crate.parent = unit.transf.parent;
			crate.transform.position = new Vector3(unit.transf.position.x, unit.transf.position.y, unit.transf.position.z);
			crate.transform.localScale = Vector3.one;
		}

		yield return new WaitForSeconds(1);

		PZPuzzleManager.instance.swapLock -= 1;

		if (callOnDeath)
		{
			if (OnDeath != null)
			{
				OnDeath();
			}
		}

	}

	public IEnumerator MoveTo(Vector3 position, float speed, bool idleAfter = true)
	{
		moving = true;
		unit.animat = MSUnit.AnimationType.RUN;

		Vector3 originalPosition = transform.localPosition;
		float time = (position - transform.localPosition).magnitude / speed;
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime / time;
			transform.localPosition = Vector3.Lerp(originalPosition, position, t);
			yield return null;
		}

		if (idleAfter)
		{
			unit.animat = MSUnit.AnimationType.IDLE;
		}
		transform.localPosition = position;
		moving = false;
	}

	public Coroutine RunAdvanceTo(float x, Vector3 direction, float speed, bool idleAfter = true)
	{
		return StartCoroutine(AdvanceTo(x, direction, speed, idleAfter));
	}

	public IEnumerator AdvanceTo(float x, Vector3 direction, float speed, bool idleAfter = true)
	{
		Transform trans = transform;
		moving = true;
		unit.animat = MSUnit.AnimationType.RUN;
		if (trans.localPosition.x <= x)
		{
			unit.direction = MSValues.Direction.EAST;
			while (trans.localPosition.x < x)
			{
				transform.localPosition += direction * speed * Time.deltaTime;
				if(trans.localPosition.x > x)//this is to prevent a single frame where the characters is in the wrong place
				{
					trans.localPosition += (x - trans.localPosition.x) * direction;
				}
				yield return null;
			}
		}
		else
		{
			unit.direction = MSValues.Direction.WEST;
			while (trans.localPosition.x > x)
			{
				trans.localPosition -= direction * speed * Time.deltaTime;
				if(trans.localPosition.x < x)
				{
					trans.localPosition += (x - trans.localPosition.x) * direction;
				}
				yield return null;
			}
		}

		if (idleAfter)
		{
			unit.animat = MSUnit.AnimationType.IDLE;
		}
		trans.localPosition = new Vector3 (x, trans.localPosition.y, trans.localPosition.z);
		moving = false;
	}

	public IEnumerator Retreat(Vector3 direction, float speed)
	{
		yield return StartCoroutine(AdvanceTo(startingPos.x, direction, speed));
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
