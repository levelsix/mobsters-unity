using UnityEngine;
using System;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Component for a single building
/// </summary>
[RequireComponent (typeof(BoxCollider))]
public class CBKBuilding : MonoBehaviour, CBKIPlaceable, CBKIPoolable, CBKITakesGridSpace, CBKISelectable
{
	#region Members
	
	#region Interface Fields
	
	private CBKBuilding _prefab;
	
	public CBKIPoolable prefab {
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
	
	#endregion
	
    #region Public
	
	public FullStructureProto structProto;
	
	public FullUserStructureProto userStructProto;
	
	public FullTaskProto task;
	
	public MinimumUserTaskProto userTask;
	
	//public UserStructProto userStructProto;

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
	private bool _selected;
	
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
	public UISprite sprite;
	
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
	
	public bool locallyOwned = true;
	
    #endregion

    #region Constants
	
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

    #endregion
	
	#region Properties
	
	public bool underConstruction {
		get
		{
			return !userStructProto.isComplete;
		}
		set
		{
			userStructProto.isComplete = !value;
		}
	}
	
	public CBKResourceManager.ResourceType baseResource {
		get
		{
			if (structProto.coinPrice > structProto.diamondPrice)
			{
				return CBKResourceManager.ResourceType.FREE;
			}
			else
			{
				return CBKResourceManager.ResourceType.PREMIUM;
			}
		}
	}
	
	public int basePrice {
		get
		{
			return Mathf.Max(structProto.coinPrice, structProto.diamondPrice);
		}
	}
	
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
		collector = GetComponent<CBKResourceCollector>();
		upgrade = GetComponent<CBKBuildingUpgrade>();
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
		structProto = CBKDataManager.instance.Get(typeof(FullStructureProto), proto.structId) as FullStructureProto;
		
		userStructProto = proto;
		
		Setup();
	}
	
	/// <summary>
	/// Init a new building from specified FullUserStructProto.
	/// </summary>
	/// <param name='structProto'>
	/// Struct proto to initialize from.
	/// </param>
	public void Init(FullStructureProto proto)
	{
		structProto = proto;
		
		userStructProto = new FullUserStructureProto();
		
		long now = CBKUtil.timeNow;
		userStructProto.lastRetrieved = now;
		userStructProto.purchaseTime = now;
		userStructProto.isComplete = false;
		userStructProto.structId = proto.structId;
		userStructProto.userId = CBKWhiteboard.localMup.userId;
		userStructProto.level = 1;
		
		Setup ();
	}
	
	public void Init(CityElementProto proto)
	{
		name = proto.name;
		
		width = proto.xLength;
		length = proto.yLength;
		
		SetupSprite(proto.name);
		
		trans.position += new Vector3(SIZE_OFFSET.x * width, 0, SIZE_OFFSET.z * length);
		SetGridFromTrans();
		
		locallyOwned = false;
		
		upgrade.Init(false);
		collector.Init(false);
		
		taskable = gameObj.AddComponent(typeof(CBKTaskable)) as CBKTaskable;
	}
	
	void Setup ()
	{
		name = structProto.name;
		
		width = structProto.xLength;
		length = structProto.yLength;
		      
		SetupSprite(structProto.name);
		
		_box.size = new Vector3(width * CBKGridManager.instance.spaceSize, 1, 
			length * CBKGridManager.instance.spaceSize);
		trans.position += new Vector3(SIZE_OFFSET.x * width, 0, SIZE_OFFSET.z * length);
		SetGridFromTrans();
		
		locallyOwned = (CBKWhiteboard.currCityType == CBKWhiteboard.CityType.PLAYER && CBKWhiteboard.cityID == CBKWhiteboard.localMup.userId);
		
		upgrade.Init(structProto, userStructProto);
		collector.Init(structProto);
	}
	
	public void SetupConstructionSprite()
	{
		//TODO: Get construction sprite
		sprite.color = Color.yellow;
		baseColor = Color.yellow;
	}
	
	public void SetupSprite(string structName)
	{
		sprite.atlas = CBKAtlasUtil.instance.GetBuildingAtlas(structName);
		if (CBKAtlasUtil.instance.LookupBuildingSprite(structName) == null)
		{
			sprite.spriteName = "Church";
		}
		else
		{
			sprite.spriteName = CBKAtlasUtil.instance.StripSpaces(structName);
		}
		
		sprite.color = Color.white;
		baseColor = Color.white;
		
//		sprite.localSize = new Vector2(4, 4);
		
		Transform spriteTrans = sprite.transform;
		
		
		//spriteTrans.localScale = new Vector3(structProto.xLength * CBKGridManager.instance.gridSpaceHypotenuse,
			//structProto.xLength * CBKGridManager.instance.gridSpaceHypotenuse * sprite.GetAtlasSprite().height / sprite.GetAtlasSprite().width, 0)
			//* .8f;
		//spriteTrans.localScale = new Vector3(4, 4, 1);
		
		spriteTrans.localPosition = new Vector3(0, spriteTrans.localScale.y * Mathf.Sin((90-48) * Mathf.Deg2Rad) / 2.2f, 0);
		
	}
	
	/// <summary>
	/// Raises the disable event.
	/// Makes sure that if this building is selected, it removes the event pointer for placement
	/// </summary>
	void OnDisable()
	{
		if (_selected)
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
		
		sprite.depth = (int)(_currPos.pos.x + _currPos.pos.y) * -1;
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
		
		
		if (CBKGridManager.instance.HasSpaceForBuilding(structProto, _currPos))
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
        if (CBKGridManager.instance.HasSpaceForBuilding(structProto, _currPos))
        {
            CBKGridManager.instance.AddBuilding(this, _currPos.x, _currPos.z, structProto.xLength, structProto.yLength);
			_originalPos = trans.position;
			
			SendBuildingMovedRequest();
        }
        else
        {
            CBKGridManager.instance.AddBuilding(this, (int)groundPos.x, (int)groundPos.y, structProto.xLength, structProto.yLength);
            trans.position = _originalPos;
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
		if (!_selected)
		{
			_originalPos = trans.position;
			_tempPos = trans.position;
	        CBKGridManager.instance.RemoveBuilding(this);
			CBKEventManager.Town.PlaceBuilding += Place;
			_selected = true;
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
		_selected = false;
		//Reset color to untinted
		sprite.color = baseColor;
		if (OnDeselect != null)
		{
			OnDeselect();
		}
    }
	
	#endregion
	
	#region Utility
	
	/// <summary>
	/// Coroutine that lerps the color back and forth between
	/// white and the current color
	/// </summary>
	IEnumerator ColorPingPong()
	{
		_ppDir = 1;
		_ppPow = 0;
		while(_selected)
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
			
			sprite.color = Color.Lerp(baseColor, _currColor, _ppPow);
			
			yield return new WaitForEndOfFrame();
		}
	}
	
	#endregion
	
	#region Poolable Functions
	
	public CBKIPoolable Make (Vector3 origin)
	{
		CBKBuilding building = Instantiate(this, origin, Quaternion.identity) as CBKBuilding;
		building.prefab = this;
		return building;
	}
	
	public void Pool ()
	{
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
