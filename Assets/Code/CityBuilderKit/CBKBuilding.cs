using UnityEngine;
using System;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Component for a single building
/// </summary>
[RequireComponent (typeof(BoxCollider))]
public class CBKBuilding : MonoBehaviour, CBKIPlaceable, CBKPoolable, CBKITakesGridSpace, CBKISelectable
{
	#region Members
	
	#region Interface Fields
	
	private CBKBuilding _prefab;
	
	public CBKPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as CBKBuilding;
		}
	}
	
	public Transform transf {
		get {
			return trans;
		}
	}

	[HideInInspector]
	public GameObject gameObj;
	
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	
	public bool walkable {
		get {
			return false;
		}
	}

	public GameObject hasMoneyPopup;

	#endregion
	
    #region Public

	public CBKUserBuildingData data;
	
	public CBKCombinedBuildingProto combinedProto;
	
	public FullUserStructureProto userStructProto;

    /// <summary>
    /// The position within ground that this building is located
    /// </summary>
    public Vector2 groundPos;

    /// <summary>
    /// The width, in grid spaces, of this building
    /// </summary>
    public int width = 1;
	
	/// <summary>
	/// The length, in grid spaces, of this building
	/// </summary>
	public int length = 1;

	public bool locked = false;

    /// <summary>
    /// This building's transform, 
    /// </summary>
    public Transform trans;
	
	/// <summary>
	/// DEBUG: The base tint. Used to reflect construction
	/// </summary>
	public Color baseColor = new Color(1f, 1f, 1f);
	
	/// <summary>
	/// The color of the building when selected.
	/// </summary>
	public Color selectColor = new Color(.5f, 1, .5f);
	
	/// <summary>
	/// The color of the building when placed in an improper place
	/// </summary>
	public Color badColor = new Color(1f, .5f, .5f);

	public Color lockColor = new Color(.66f, .66f, .66f);
	
	public Action OnSelect;
	
	public Action OnDeselect;
	
	public Action OnUpdateValues;
	
	/// <summary>
	/// The identifier.
	/// If player city, this is userStructId
	/// If neutral city, this is assetId
	/// </summary>
	public int id;
	
	public CBKTaskable taskable;

	const float HOME_BUILDING_SCALE = 0.66f;

	public long completeTime = 0;

    #endregion

    #region Private

    /// <summary>
    /// If the building is being moved, this holds its position prior
    /// to the move
    /// </summary>
    private Vector3 _originalPos;
	
	/// <summary>
	/// If the building is being moved, this keeps track of its
	/// steps along the way, so that a building can be moved in multiple
	/// steps before being placed
	/// </summary>
	private Vector3 _tempPos;

    /// <summary>
    /// If the building is being moved, the current position that it is being
    /// dragged over on the map
    /// </summary>
    private CBKGridNode _currPos;
	
	/// <summary>
	/// If the building is currently selected
	/// </summary>
	public bool selected;
	
	/// <summary>
	/// Current color.
	/// </summary>
	private Color _currColor;
	
	/// <summary>
	/// The direction of the tint ping-pong
	/// </summary>
	private int _ppDir = 1;
	
	/// <summary>
	/// The current tracker for the ping-pong
	/// </summary>
	private float _ppPow = 0;
	
	/// <summary>
	/// The box collider for this building
	/// </summary>
	private BoxCollider _box;
	
	/// <summary>
	/// The sprite for this building.
	/// </summary>
	public SpriteRenderer sprite;

	/// <summary>
	/// An sprite, used only by specific buildings (i.e. hospital) which
	/// need to put Units 'inside' of them
	/// </summary>
	public SpriteRenderer overlay;
	
	/// <summary>
	/// The upgrade component
	/// </summary>
	public CBKBuildingUpgrade upgrade;
	
	/// <summary>
	/// The resource collector component.
	/// Added to the base building if this building
	/// is meant to be a resource collector.
	/// </summary>
	public CBKResourceCollector collector;

	/// <summary>
	/// The resource storage component.
	/// </summary>
	public CBKResourceStorage storage;

	[SerializeField]
	GameObject shadow;

	[SerializeField]
	public UISprite hoverIcon;

	[SerializeField]
	UITweener lockTween;

	[SerializeField]
	TweenPosition arrowPosTween;

	[SerializeField]
	TweenPosition arrowScaleTween;

	public bool locallyOwned = true;
	
    #endregion

    #region Constants

	const float FLOAT_ICON_HOME_HEIGHT = 4f;
	const float FLOAT_ICON_MISSION_HEIGHT = 12f;
	
	/// <summary>
	/// The amount of time it takes the tint to ping-pong
	/// when this building is selected
	/// </summary>
	private const float COLOR_SPEED = 1.5f;

    /// <summary>
    /// How much to offset the center of the building by for each size increase
    /// </summary>
    private static Vector3 SIZE_OFFSET{
		get
		{
			return new Vector3(CBKGridManager.instance.spaceSize * .5f, 0, 
				CBKGridManager.instance.spaceSize * .5f);
		}
	}
	
	/// <summary>
	/// Constant coefficient for the drag amount to get it 
	/// to work just right
	/// </summary>
	public float X_DRAG_FUDGE = 1.1f;

	public static readonly Vector3 SHADOW_POS = new Vector3(-0.36f, 2.28f, 3.8f);
	public static readonly Vector3 FLIP_SHADOW_POS = new Vector3(0.36f, 2.28f, 3.8f);

	const string LOCK_SPRITE_NAME = "lockedup";
	const string ARROW_SPRITE_NAME = "arrow";

	const string CASH_READY_SPRITE_NAME = "cashready";
	const string OIL_READY_SPRITE_NAME = "oilready";

    #endregion
	
	#endregion
	
	#region Functions
	
	#region Instantiation/Clean-up
	
    /// <summary>
    /// On awake, get a pointer to all necessary components
    /// </summary>
    void Awake()
    {
		_box = GetComponent<BoxCollider>();
        trans = transform;
		gameObj = gameObject;
    }
	
	/// <summary>
	/// Create a building from an already existing FUSP
	/// </summary>
	/// <param name='proto'>
	/// FUSP from server
	/// </param>
	public void Init(FullUserStructureProto proto)
	{
		combinedProto = CBKDataManager.instance.Get(typeof(CBKCombinedBuildingProto), proto.structId) as CBKCombinedBuildingProto;
		
		userStructProto = proto;

		id = proto.userStructId;
		
		Setup();
	}
	
	/// <summary>
	/// Init a new building from specified FullUserStructProto.
	/// </summary>
	/// <param name='structProto'>
	/// Struct proto to initialize from.
	/// </param>
	public void Init(StructureInfoProto proto)
	{
		combinedProto = CBKDataManager.instance.Get(typeof(CBKCombinedBuildingProto), proto.structId) as CBKCombinedBuildingProto;
		
		userStructProto = new FullUserStructureProto();

		long now = CBKUtil.timeNowMillis;
		userStructProto.lastRetrieved = now;
		userStructProto.purchaseTime = now;
		userStructProto.isComplete = false;
		userStructProto.structId = combinedProto.structInfo.structId;
		userStructProto.userId = CBKWhiteboard.localMup.userId;
		
		Setup ();
	}
	
	public void Init(CityElementProto proto)
	{
		hoverIcon.transform.localPosition = new Vector3(0, FLOAT_ICON_MISSION_HEIGHT);

		groundPos = new Vector2(proto.coords.x, proto.coords.y);

		locked = false;
		hoverIcon.gameObject.SetActive(false);
		sprite.color = Color.white;

		trans.localScale = Vector3.one;

		id = proto.assetId;

		name = proto.imgId;

		//name = proto.name;
		userStructProto = null;
		
		width = (int)proto.xLength;
		length = (int)proto.yLength;
		
		SetupSprite(proto.imgId);

		if (proto.orientation == StructOrientation.POSITION_2)
		{
			sprite.transform.localScale = new Vector3(-1, 1, 1);
			shadow.transform.localPosition = FLIP_SHADOW_POS;
		}
		else
		{
			shadow.transform.localPosition = SHADOW_POS;
		}
		
		//trans.position += new Vector3(SIZE_OFFSET.x * width, 0, SIZE_OFFSET.z * length);
		//SetGridFromTrans();
		//sprite.depth = (int)(proto.coords.x + proto.coords.y + Mathf.Min(proto.xLength, proto.yLength)/2) * -1 - 10;

		locallyOwned = false;

		sprite.transform.localPosition = Vector3.zero;
		if (width > length)
		{
			//sprite.transform.localPosition = new Vector3(sprite.GetAtlasSprite().width/200f, 0, -sprite.GetAtlasSprite().width/200f);
		}
		else if (width < length)
		{
			//sprite.transform.localPosition = new Vector3(-sprite.GetAtlasSprite().width/200f, 0, sprite.GetAtlasSprite().width/200f);
		}

		if (proto.type == CityElementProto.CityElemType.BUILDING)
		{
			taskable = gameObj.AddComponent(typeof(CBKTaskable)) as CBKTaskable;
			_box.enabled = true;
			float hypot = Mathf.Max(width, length) * CBKGridManager.instance.gridSpaceHypotenuse / 2 *2/3;
			_box.center = new Vector3(hypot, 0, hypot);
			_box.size = new Vector3(width * CBKGridManager.instance.spaceSize, 1, length * CBKGridManager.instance.spaceSize);
			shadow.SetActive(true);
		}
		else
		{
			_box.enabled = false;
			shadow.SetActive(false);
		}
	}
	
	void Setup ()
	{

		hoverIcon.transform.localPosition = new Vector3(0, FLOAT_ICON_HOME_HEIGHT);

		locked = false;

		name = combinedProto.structInfo.name;

		shadow.transform.localPosition = SHADOW_POS;

		hoverIcon.gameObject.SetActive(false);
		sprite.color = Color.white;

		//trans.localScale = new Vector3(HOME_BUILDING_SCALE, HOME_BUILDING_SCALE, HOME_BUILDING_SCALE);

		//name = structProto.name;
		_box.enabled = true;
		shadow.SetActive(true);
		
		width = combinedProto.structInfo.width;
		length = combinedProto.structInfo.height;
		      
		SetupSprite(combinedProto.structInfo.imgName);
		
		_box.size = new Vector3(width * CBKGridManager.instance.spaceSize, 1, 
			length * CBKGridManager.instance.spaceSize);
		trans.position += new Vector3(SIZE_OFFSET.x * width, 0, SIZE_OFFSET.z * length);
		SetGridFromTrans();
		
		locallyOwned = (CBKWhiteboard.currCityType == CBKWhiteboard.CityType.PLAYER && CBKWhiteboard.cityID == CBKWhiteboard.localMup.userId);

		upgrade = gameObj.AddComponent<CBKBuildingUpgrade>();
		upgrade.Init(combinedProto.structInfo, userStructProto);

		//float hypot = Mathf.Max(width, length) * CBKGridManager.instance.gridSpaceHypotenuse / 2;

		//sprite.transform.localPosition = new Vector3(-hypot, 0, -hypot);
		sprite.transform.localPosition = new Vector3(-2.8f, 1.63f, -2.8f);

		_box.center = Vector3.zero;

		//Add and init the component for whatever struct type this building is. Unless it's a decoration. Then it does nothing.
		switch (combinedProto.structInfo.structType) {
		case StructureInfoProto.StructType.RESOURCE_GENERATOR:
			collector = gameObj.AddComponent<CBKResourceCollector>();
			collector.Init(combinedProto);
			break;
		default:
			storage = gameObj.AddComponent<CBKResourceStorage>();

			break;
		}
	}
	
	public void SetupConstructionSprite()
	{
		//TODO: Get construction sprite
		sprite.color = Color.yellow;
		baseColor = Color.yellow;
	}
	
	public void SetupSprite(string structName)
	{
		overlay.color = new Color(1,1,1,0);

		//sprite.spriteName = CBKUtil.StripExtensions(structName);
		//CBKAtlasUtil.instance.SetAtlasForSprite(sprite);
		sprite.sprite = CBKAtlasUtil.instance.GetBuildingSprite(CBKUtil.StripExtensions(structName));

		RuntimeAnimatorController animator = CBKAtlasUtil.instance.GetAnimator(CBKUtil.StripExtensions(structName));
		Animator spriteAnimator = sprite.GetComponent<Animator>();
		if (spriteAnimator != null)
		{
			if (animator != null)
			{
				spriteAnimator.runtimeAnimatorController = CBKAtlasUtil.instance.GetAnimator(combinedProto.structInfo.imgName);
			}
			else
			{
				spriteAnimator.runtimeAnimatorController = null;
			}
		}

		/*
		sprite.atlas = CBKAtlasUtil.instance.GetBuildingAtlas(structName);
		if (CBKAtlasUtil.instance.LookupBuildingSprite(structName) == null)
		{
			sprite.spriteName = "Church";
		}
		else
		{
			sprite.spriteName = CBKUtil.StripExtensions(CBKAtlasUtil.instance.StripSpaces(structName));
		}
		*/
		
		sprite.color = Color.white;
		baseColor = Color.white;
		
//		sprite.localSize = new Vector2(4, 4);
//		sprite.width = sprite.GetAtlasSprite().width;
//		sprite.height = sprite.GetAtlasSprite().height;

		
		Transform spriteTrans = sprite.transform;
		
		spriteTrans.localScale = Vector3.one;


		
		//spriteTrans.localPosition = new Vector3(0, sprite.height / 100f * (1/Mathf.Sin((90 - Camera.main.transform.parent.localRotation.x) * Mathf.Deg2Rad)), 0);
		
	}
	
	/// <summary>
	/// Raises the disable event.
	/// Makes sure that if this building is selected, it removes the event pointer for placement
	/// </summary>
	void OnDisable()
	{
		if (selected)
		{
			CBKEventManager.Town.PlaceBuilding -= Place;
		}
	}
	
	#endregion
	
	#region Building Selection & Movement

	void SetGridFromTrans ()
	{
		_currPos = new CBKGridNode(new Vector2(transform.position.x / CBKGridManager.instance.spaceSize - SIZE_OFFSET.x * width,
    	    transform.position.z / CBKGridManager.instance.spaceSize - SIZE_OFFSET.z * length));
		
		//sprite.depth = (int)(_currPos.pos.x + _currPos.pos.y + Mathf.Min(width, length)/2) * -1 - 10;
		//Debug.Log("Currpos: " + _currPos.pos);
	}
	
    /// <summary>
    /// Moves this building relative to its original position
    /// </summary>
    /// <param name="movement"></param> 
    public void MoveRelative(TCKTouchData touch)
    {
		Vector3 movement = touch.Movement;
		
        //Turn the mouse difference in screen coordinates to world coordinates
        movement.y *= 2 * (Camera.main.orthographicSize / Screen.height);
        movement.x *= 2 * (Camera.main.orthographicSize / Screen.width) * X_DRAG_FUDGE;

        //Turn the 2D coordinates into our tilted isometric coordinates
        movement.z = movement.y - movement.x;
        movement.x = movement.x + movement.y;
        movement.y = 0;

        //Add the difference to the original position, since we only hold original mouse pos
        trans.position = _tempPos + movement;

        trans.position = CBKGridManager.instance.SnapPointToGrid(transform.position, width, length);

		SetGridFromTrans ();
		
		
		if (CBKGridManager.instance.HasSpaceForBuilding(combinedProto.structInfo, _currPos))
		{
			_currColor = selectColor;
		}
		else
		{
			_currColor = badColor;
		}
		
    }
	
	/// <summary>
	/// Drop this instance in place, so that it can be moved more with another
	/// drag
	/// </summary>
	public void Drop()
	{
		_tempPos = trans.position;	
	}

    /// <summary>
    /// Move the building to the curent position
    /// If the current position is invalid, move it back to its original position
    /// </summary>
    public void Place()
    {
		if (userStructProto != null)
		{
	        if (_currPos.pos != groundPos && CBKGridManager.instance.HasSpaceForBuilding(combinedProto.structInfo, _currPos))
	        {
	            CBKGridManager.instance.AddBuilding(this, _currPos.x, _currPos.z, combinedProto.structInfo.width, combinedProto.structInfo.height);
				_originalPos = trans.position;
				
				SendBuildingMovedRequest();
	        }
	        else
	        {
	            CBKGridManager.instance.AddBuilding(this, (int)groundPos.x, (int)groundPos.y, combinedProto.structInfo.width, combinedProto.structInfo.height);
	            trans.position = _originalPos;
	        }
		}
		CBKEventManager.Town.PlaceBuilding -= Place;
		Deselect();
    }
	
	void SendBuildingMovedRequest()
	{
		MoveOrRotateNormStructureRequestProto request = new MoveOrRotateNormStructureRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.userStructId = userStructProto.userStructId;
		request.type = MoveOrRotateNormStructureRequestProto.MoveOrRotateNormStructType.MOVE;
		request.curStructCoordinates = new CoordinateProto();
		request.curStructCoordinates.x = _currPos.pos.x;
		request.curStructCoordinates.y = _currPos.pos.y;
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_MOVE_OR_ROTATE_NORM_STRUCTURE_EVENT, LoadBuildingMovedResponse);
	}
	
	void LoadBuildingMovedResponse(int tagNum)
	{
		//Grab the posted response & remove it
		MoveOrRotateNormStructureResponseProto response = (MoveOrRotateNormStructureResponseProto)UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status == MoveOrRotateNormStructureResponseProto.MoveOrRotateNormStructureStatus.OTHER_FAIL)
		{
			Debug.LogError("FAIURE: Could not confirm building movement.");
		}
	}
	
    /// <summary>
    /// When this building is selected
    /// </summary>
    public void Select()
    {
		if (locked)
		{
			lockTween.ResetToBeginning();
			lockTween.PlayForward();
			CBKBuildingManager.instance.FullDeselect();
		}
		else if (!selected)
		{
			_originalPos = trans.position;
			_tempPos = trans.position;
			if (userStructProto != null)
			{
	        	CBKGridManager.instance.RemoveBuilding(this);
			}
			CBKEventManager.Town.PlaceBuilding += Place;
			selected = true;
			_currColor = selectColor;
			
			if (OnSelect != null)
			{
				OnSelect();
			}
			
			StartCoroutine(ColorPingPong());
		}
    }
	
	/// <summary>
	/// Deselect this instance.
	/// </summary>
    public void Deselect()
    {
		selected = false;
		//Reset color to untinted

		sprite.color = (locked) ? lockColor : baseColor;
		if (OnDeselect != null)
		{
			OnDeselect();
		}
    }
	
	#endregion
	
	#region Utility

	public void SetLocked()
	{
		locked = true;
		sprite.color = lockColor;
		hoverIcon.gameObject.SetActive(true);
		hoverIcon.spriteName = LOCK_SPRITE_NAME;
	}

	public void SetUnlocked()
	{
		locked = false;
		sprite.color = Color.white;
		hoverIcon.gameObject.SetActive(false);
	}

	public void SetArrow()
	{
		hoverIcon.gameObject.SetActive(true);
		hoverIcon.spriteName = ARROW_SPRITE_NAME;
		arrowPosTween.PlayForward();
		arrowScaleTween.PlayForward();
	}
	
	/// <summary>
	/// Coroutine that lerps the color back and forth between
	/// white and the current color
	/// </summary>
	IEnumerator ColorPingPong()
	{
		_ppDir = 1;
		_ppPow = 0;
		Color curr;
		while(selected)
		{
			_ppPow += _ppDir * Time.deltaTime * COLOR_SPEED;
			
			//See if we need to change direction
			if (_ppPow >= 1)
			{
				_ppPow = 1;
				_ppDir = -1;
			}
			else if (_ppPow <= 0)
			{
				_ppPow = 0;
				_ppDir = 1;
			}

			curr = Color.Lerp(baseColor, _currColor, _ppPow);
			sprite.color = curr;
			if (overlay.color.a > 0)
			{
				overlay.color = curr;
			}
			
			yield return new WaitForEndOfFrame();
		}
	}
	
	public void Sell()
	{
		/*
		if (_selected)
		{
			Deselect();
		}
		
		CBKGridManager.instance.RemoveBuilding(this);

		SellNormStructureRequestProto request = new SellNormStructureRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.userStructId = userStructProto.userStructId;
		
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_SELL_NORM_STRUCTURE_EVENT, DealWithSell);
		
		CBKResourceManager.instance.Collect(baseResource, structProto.buildCost / 2);
		
		Pool ();
		*/
	}
	
	void DealWithSell(int tagNum)
	{
		/*
		SellNormStructureResponseProto response = UMQNetworkManager.responseDict[tagNum] as SellNormStructureResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != SellNormStructureResponseProto.SellNormStructureStatus.SUCCESS)
		{
			Debug.LogError("Problem selling building: " + response.status.ToString());
		}
		*/
	}
	
	#endregion
	
	#region Poolable Functions
	
	public CBKPoolable Make (Vector3 origin)
	{
		CBKBuilding building = Instantiate(this, origin, Quaternion.identity) as CBKBuilding;
		building.prefab = this;
		return building;
	}
	
	public void Pool ()
	{
		if (upgrade != null)
		{
			Destroy (upgrade);
			upgrade = null;
		}
		if (collector != null)
		{
			Destroy(collector);
			collector = null;
		}
		if (storage != null)
		{
			Destroy (storage);
			storage = null;
		}

		if (!selected)
		{
			CBKGridManager.instance.RemoveBuilding(this);
		}
		if (taskable != null)
		{
			Destroy(taskable);
			taskable = null;
		}
		CBKPoolManager.instance.Pool(this);
	}
	
	#endregion
	
	#endregion
}
