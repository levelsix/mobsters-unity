using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSUnit : MonoBehaviour, MSPoolable {
	
	public SpriteRenderer sprite;

	[SerializeField]
	public SpriteRenderer shadow;

	public Animator anim;
	
	string _spriteBaseName;

	public MSTaskable taskable;

	public string spriteBaseName
	{
		get
		{
			return _spriteBaseName;
		}
		set
		{
			_spriteBaseName = value;
			StartCoroutine(SetSprite(value));
		}
	}

	public float alpha
	{
		set
		{
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, value);
			if (shadow != null)
			{
				shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, value);
			}
		}
	}
	
	[SerializeField]
	UILabel nameLabel;
	
	[SerializeField]
	private MSValues.Direction _direction = MSValues.Direction.NORTH;
	
	private bool _selected;
	
	public MSValues.Direction direction
	{
		get
		{
			return _direction;
		}
		set
		{
			if (value != _direction)
			{
				_direction = value;
				SetDirection();
			}
		}
	}
	
	public CityElementProto ncep;
	
	public FullTaskProto task;
	public MinimumUserTaskProto userTask;
	
	public enum AnimationType {IDLE, RUN, ATTACK, FLINCH, STAY};
	
	private AnimationType _animat = AnimationType.IDLE;

	public AnimationType animat
	{
		set
		{
			_animat = value;
			SetAnimation(value);
		}
		get
		{
			return _animat;
		}
	}
	
	Transform trans;
	GameObject gameObj;
	
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	public Transform transf {
		get {
			return transform;
		}
	}
	public MSCityUnit cityUnit;
	
	MSUnit _prefab;
	public MSPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as MSUnit;
		}
	}
	
	public float unitSize = 1.1f;

	public bool tutorial = false;

	public MonsterProto monster;

	public bool hasSprite;
	
	/// <summary>
	/// Awake this instance.
	/// Set up internal component references
	/// </summary>
	void Awake()
	{
		trans = transform;
		gameObj = gameObject;
		cityUnit = GetComponent<MSCityUnit>();
	}

	void OnEnable(){
		Color newColor = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 1f);
		sprite.color = newColor;
		if (shadow != null)
		{
			shadow.color = newColor;
		}
		ResetAnimation ();
	}
	
	public MSPoolable Make (Vector3 origin)
	{
		MSUnit unit = Instantiate(this, origin, Quaternion.identity) as MSUnit;
		unit.prefab = this;
		return unit;
	}
	
	public void Pool ()
	{
		MSPoolManager.instance.Pool(this);
		if (taskable != null)
		{
			Destroy(taskable);
			taskable = null;
		}
	}
	
	public void Init ()
	{	
		name = "Rob";

		Setup();
	}

	public void Init(PZMonster mon)
	{
		monster = mon.monster;

		name = mon.monster.displayName;

		spriteBaseName = MSUtil.StripExtensions(mon.monster.imagePrefix);
		
		Setup();
	}
	
	public void Init(FullUserMonsterProto proto)
	{
		monster = MSDataManager.instance.Get(typeof(MonsterProto), proto.monsterId) as MonsterProto;
		
		name = monster.displayName;
		
		spriteBaseName = MSUtil.StripExtensions(monster.imagePrefix);
		
		Setup();
	}

	public void Init (int monsterId)
	{
		monster = MSDataManager.instance.Get<MonsterProto>(monsterId);

		name = monster.displayName;
		spriteBaseName = MSUtil.StripExtensions(monster.imagePrefix);

		Setup();
	}
	
	public void Init (CityElementProto proto)
	{
		name = proto.imgId;
		
		ncep = proto;
		
		spriteBaseName = MSUtil.StripExtensions(ncep.imgId);
		
		Setup();
	}
	
	void Setup()
	{
		if (cityUnit != null)
		{
			cityUnit.Init();
		}
		
		SetAnimation(AnimationType.IDLE);
		
		if (nameLabel != null)
		{
			nameLabel.text = name;
		}
	}

	void SetDirection ()
	{
		
		if (anim.runtimeAnimatorController == null) return;

		anim.SetBool("Far", (direction == MSValues.Direction.NORTH || direction == MSValues.Direction.EAST));

		if (direction == MSValues.Direction.SOUTH || direction == MSValues.Direction.EAST) {
			sprite.transform.localScale = new Vector3 (-1, 1, 1) * unitSize;
		}
		else {
			sprite.transform.localScale = Vector3.one * unitSize;
		}
	}
	
	void SetAnimation(AnimationType animate)
	{

		if (anim.runtimeAnimatorController == null) return;

		switch(animate)
		{
			case AnimationType.ATTACK:
				anim.SetBool("Attack", true);
				break;
			case AnimationType.FLINCH:
				anim.SetBool("Flinch", true);
				if(anim.GetBool("Far")){
					anim.Play("FlinchFarLeft", 0, 0f);
				}else{
					anim.Play("FlinchNearLeft", 0, 0f);
				}
				break;
			case AnimationType.IDLE:
				anim.SetBool("Running", false);
				anim.SetBool("Flinch", false);
				anim.SetBool("Attack", false);
				if (MSTutorialManager.instance.inTutorial) anim.SetBool("Stay", false);
				break;
			case AnimationType.RUN:
				anim.SetBool("Flinch", false);
				anim.SetBool("Running", true);
				break;
			case AnimationType.STAY:
				anim.SetBool("Stay", true);
				break;
			default:
				break;
		}

		
		SetDirection ();

	}

	public void ResetAnimation()
	{
		SetAnimation(animat);
	}
	
	IEnumerator SetSprite(string spriteName)
	{
		alpha = 0;
		hasSprite = false;
		yield return StartCoroutine(MSSpriteUtil.instance.SetUnitAnimator(this));
		alpha = anim.runtimeAnimatorController != null ? 1 : 0;
		if (shadow != null)
		{
			shadow.color = Color.white;
		}
		ResetAnimation();
		hasSprite = true;
		SetAnimation(AnimationType.IDLE);
	}

	[SerializeField] float testJumpHeight = 3;
	[SerializeField] float testJumpTime = .5f;
	[SerializeField] bool testJump = false;

	void Update()
	{
		if (testJump)
		{
			TestJump();
			testJump = false;
		}
	}

	public void TestJump()
	{
		DoJump(testJumpHeight, testJumpTime);
	}

	public Coroutine DoJump(float height, float speed)
	{
		return StartCoroutine(Jump(height, speed));
	}

	IEnumerator Jump(float jumpHeight, float time)
	{
		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.jump);
		float baseHeight = sprite.transform.localPosition.y;
		float currTime = 0, t = 0;
		do
		{
			currTime += Time.deltaTime;
			t = currTime/time;

			t = 1 - (2*t - 1) * (2*t - 1);

			sprite.transform.localPosition = 
				new Vector3( sprite.transform.localPosition.x,
		                     Mathf.Lerp(baseHeight,
										baseHeight + jumpHeight,
										t),
		                     sprite.transform.localPosition.z);
			yield return null;

		} while (currTime < time);
	}

	public Coroutine DoFade(bool fadeIn, float time)
	{
		return StartCoroutine(Fade (fadeIn, time));
	}

	IEnumerator Fade(bool fadeIn, float time)
	{
		float t = 0;
		if (time > 0)
		{
			while (t < 1)
			{
				t += Time.deltaTime / time;
				alpha = (fadeIn) ? t : 1-t;
				yield return null;
			}
		}
		alpha = (fadeIn) ? 1 : 0;
	}

}








