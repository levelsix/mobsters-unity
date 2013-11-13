using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKUnit : MonoBehaviour, CBKIPoolable {
	
	public UISprite sprite;
	public UISpriteAnimation anim;
	
	string _spriteBaseName;
	
	public string spriteBaseName
	{
		get
		{
			return _spriteBaseName;
		}
		set
		{
			_spriteBaseName = value;
			sprite.spriteName = value;
			CBKAtlasUtil.instance.SetAtlasForSprite(sprite);
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
				//SetAnimation();
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
	public CBKIPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as CBKUnit;
		}
	}
	
	const int ANIMATION_FPS = 15;
	
	public float unitSize = 1.1f;
	
	static readonly Dictionary<CBKValues.Direction, string> dirNameDict = new Dictionary<CBKValues.Direction, string>()
	{
		{CBKValues.Direction.NORTH, "F"},
		{CBKValues.Direction.SOUTH, "N"},
		{CBKValues.Direction.WEST, "N"},
		{CBKValues.Direction.EAST, "F"},
		{CBKValues.Direction.NONE, "N"}
	};
	
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
	
	public CBKIPoolable Make (Vector3 origin)
	{
		CBKUnit unit = Instantiate(this, origin, Quaternion.identity) as CBKUnit;
		unit.prefab = this;
		return unit;
	}
	
	public void Pool ()
	{
		CBKPoolManager.instance.Pool(this);
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
	
	void SetAnimation(AnimationType animate)
	{
		string animationPrefix = spriteBaseName;
		
		switch(animate)
		{
			case AnimationType.ATTACK:
				animationPrefix += "Attack";
				anim.framesPerSecond = ANIMATION_FPS;
				break;
			case AnimationType.FLINCH:
				animationPrefix += "Flinch";
				anim.framesPerSecond = 3;
				break;
			case AnimationType.IDLE:
				animationPrefix += "Attack";
				anim.framesPerSecond = 0;
				break;
			case AnimationType.RUN:
				animationPrefix += "Run";
				anim.framesPerSecond = ANIMATION_FPS;
				break;
			default:
				break;
		}
		animationPrefix += dirNameDict[direction];
		
		if (direction == CBKValues.Direction.SOUTH || direction == CBKValues.Direction.EAST)
		{
			sprite.transform.localScale = new Vector3(-1,1,1) * unitSize;
		}
		else
		{
			sprite.transform.localScale = Vector3.one * unitSize;
		}
		
		anim.namePrefix = animationPrefix;
		anim.loop = (animate == AnimationType.RUN); //Loop run
		
		anim.Reset();
	}
	
	
}
