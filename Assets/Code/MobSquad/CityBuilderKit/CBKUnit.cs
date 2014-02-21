using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKUnit : MonoBehaviour, CBKPoolable {
	
	public SpriteRenderer sprite;

	public Animator anim;
	
	string _spriteBaseName;

	public CBKTaskable taskable;

	public string spriteBaseName
	{
		get
		{
			return _spriteBaseName;
		}
		set
		{
			_spriteBaseName = value;
			anim.runtimeAnimatorController = CBKAtlasUtil.instance.GetAnimator(value);
			//_spriteBaseName = value;
			//sprite.spriteName = value;
			//CBKAtlasUtil.instance.SetAtlasForSprite(sprite);
			SetAnimation(AnimationType.IDLE);
		}
	}
	
	[SerializeField]
	UILabel nameLabel;
	
	[SerializeField]
	private CBKValues.Direction _direction = CBKValues.Direction.NORTH;
	
	private bool _selected;
	
	public CBKValues.Direction direction
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
	
	public AnimationType animat
	{
		set
		{
			SetAnimation(value);
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
			return trans;
		}
	}
	public CBKCityUnit cityUnit;
	
	CBKUnit _prefab;
	public CBKPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as CBKUnit;
		}
	}
	
	const int ANIMATION_FPS = 15;
	
	public float unitSize = 1.1f;
	
	/// <summary>
	/// Awake this instance.
	/// Set up internal component references
	/// </summary>
	void Awake()
	{
		trans = transform;
		gameObj = gameObject;
		cityUnit = GetComponent<CBKCityUnit>();
	}
	
	public CBKPoolable Make (Vector3 origin)
	{
		CBKUnit unit = Instantiate(this, origin, Quaternion.identity) as CBKUnit;
		unit.prefab = this;
		return unit;
	}
	
	public void Pool ()
	{
		CBKPoolManager.instance.Pool(this);
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
	
	public void Init(FullUserMonsterProto proto)
	{
		MonsterProto monster = CBKDataManager.instance.Get(typeof(MonsterProto), proto.monsterId) as MonsterProto;
		
		name = monster.displayName;
		
		spriteBaseName = CBKUtil.StripExtensions(monster.imagePrefix);
		
		Setup();
	}
	
	public void Init (CityElementProto proto)
	{
		
		name = proto.imgId;
		
		ncep = proto;
		
		spriteBaseName = CBKUtil.StripExtensions(ncep.imgId);
		
		Setup();
	}
	
	void Setup()
	{
		
		if (cityUnit != null)
		{
			cityUnit.Init ();
		}
		
		SetAnimation(AnimationType.IDLE);
		
		if (nameLabel != null)
		{
			nameLabel.text = name;
		}
	}

	void SetDirection ()
	{
		anim.SetBool("Far", (direction == CBKValues.Direction.NORTH || direction == CBKValues.Direction.EAST));

		if (direction == CBKValues.Direction.SOUTH || direction == CBKValues.Direction.EAST) {
			sprite.transform.localScale = new Vector3 (-1, 1, 1) * unitSize;
		}
		else {
			sprite.transform.localScale = Vector3.one * unitSize;
		}
	}
	
	void SetAnimation(AnimationType animate)
	{
		
		switch(animate)
		{
			case AnimationType.ATTACK:
				anim.SetBool("Attack", true);
				break;
			case AnimationType.FLINCH:
				anim.SetBool("Flinch", true);
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
	
	
}
