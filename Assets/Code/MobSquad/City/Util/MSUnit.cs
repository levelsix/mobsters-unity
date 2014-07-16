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
			anim.runtimeAnimatorController = MSSpriteUtil.instance.GetUnitAnimator(MSUtil.StripExtensions(value));
			//StartCoroutine(MSSpriteUtil.instance.SetUnitAnimator(this));


			if (MSSpriteUtil.instance.HasBundle(value))
			{
				sprite.color = Color.white;
			}
			else
			{
				sprite.color = new Color(1,1,1,1);
			}

			SetAnimation(AnimationType.IDLE);
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
	
	public enum AnimationType {IDLE, RUN, ATTACK, FLINCH};
	
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
				break;
			case AnimationType.RUN:
				anim.SetBool("Running", true);
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
	
}
