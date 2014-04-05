using UnityEngine;
using System;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Component for a single building
/// </summary>
[RequireComponent (typeof(BoxCollider))]
public class MSBuilding : MonoBehaviour, CBKIPlaceable, MSPoolable, CBKITakesGridSpace, CBKISelectable
{
	#region Members
	
	#region Interface Fields
	
	private MSBuilding _prefab;
	
	public MSPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as MSBuilding;
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

	#endregion
	
    #region Public

	public MSUserBuildingData data;
	
	public MSFullBuildingProto combinedProto;
	
	public FullUserStructureProto userStructProto;

	public MSObstacle obstacle;

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
	
	public MSTaskable taskable;

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
    private MSGridNode _currPos;
	
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
	public MSBuildingUpgrade upgrade;
	
	/// <summary>
	/// The resource collector component.
	/// Added to the base building if this building
	/// is meant to be a resource collector.
	/// </summary>
	public MSResourceCollector collector;

	/// <summary>
	/// The resource storage component.
	/// </summary>
	public MSResourceStorage storage;

	/// <summary>
	/// The hospital component.
	/// </summary>
	public MSHospital hospital;

	[SerializeField]
	SpriteRenderer floor;

	[SerializeField]
	SpriteRenderer shadow;

	[SerializeField]
	public UISprite hoverIcon;

	[SerializeField]
	UITweener lockTween;

	[SerializeField]
	TweenPosition arrowPosTween;

	[SerializeField]
	TweenPosition arrowScaleTween;

	public bool locallyOwned = true;

	[SerializeField]
	GameObject confirmationButtons;

	public MSUnit overlayUnit;

	long constructionTimeLeft
	{
		get
		{
			if (obstacle != null)
			{
				return obstacle.secsLeft;
			}
			return upgrade.timeRemaining;
		}
	}
	
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
			return new Vector3(MSGridManager.instance.spaceSize * .5f, 0, 
				MSGridManager.instance.spaceSize * .5f);
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
		upgrade = GetComponent<MSBuildingUpgrade>();
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
		combinedProto = MSDataManager.instance.Get(typeof(MSFullBuildingProto), proto.structId) as MSFullBuildingProto;
		userStructProto = proto;

		id = proto.userStructId;

		Setup();

	}

	public void Init(UserObstacleProto proto)
	{
		obstacle = gameObj.AddComponent<MSObstacle>();

		obstacle.Init(proto);

		combinedProto = null;
		userStructProto = null;

		id = proto.userObstacleId;

		Setup();

		locallyOwned = false;
	}
	
	/// <summary>
	/// Init a new building from specified FullUserStructProto.
	/// </summary>
	/// <param name='structProto'>
	/// Struct proto to initialize from.
	/// </param>
	public void Init(MSFullBuildingProto proto)
	{
		combinedProto = proto;
		
		userStructProto = new FullUserStructureProto();

		Setup ();

		confirmationButtons.SetActive(true);
	}
	
	public void Init(CityElementProto proto)
	{
		confirmationButtons.SetActive(false);

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

		floor.gameObject.SetActive(false);
		SetupSprite(proto.imgId);

		if (proto.orientation == StructOrientation.POSITION_2)
		{
			//sprite.transform.localScale = new Vector3(-1, 1, 1);
			//shadow.transform.localPosition = FLIP_SHADOW_POS;
		}
		else
		{
			//shadow.transform.localPosition = SHADOW_POS;
		}
		
		//trans.position += new Vector3(SIZE_OFFSET.x * width, 0, SIZE_OFFSET.z * length);
		//SetGridFromTrans();
		//sprite.depth = (int)(proto.coords.x + proto.coords.y + Mathf.Min(proto.xLength, proto.yLength)/2) * -1 - 10;

		locallyOwned = false;

		sprite.transform.localPosition = Vector3.zero;

		if (proto.type == CityElementProto.CityElemType.BUILDING)
		{
			taskable = gameObj.AddComponent(typeof(MSTaskable)) as MSTaskable;
			_box.enabled = true;
			float hypot = Mathf.Max(width, length) * MSGridManager.instance.gridSpaceHypotenuse / 2 *2/3;
			_box.center = new Vector3(hypot, 0, hypot);
			_box.size = new Vector3(width * MSGridManager.instance.spaceSize, 1, length * MSGridManager.instance.spaceSize);
			//shadow.gameObject.SetActive(true);
		}
		else
		{
			_box.enabled = false;
			//shadow.gameObject.SetActive(false);
		}
	}
	
	void Setup ()
	{
		confirmationButtons.SetActive(false);
		locked = false;
		
		hoverIcon.gameObject.SetActive(false);
		sprite.color = Color.white;

		hoverIcon.transform.localPosition = new Vector3(0, FLOAT_ICON_HOME_HEIGHT);

		_box.enabled = true;
		floor.gameObject.SetActive(true);

		sprite.transform.localPosition = Vector3.zero;

		if (combinedProto != null)
		{
			width = combinedProto.structInfo.width;
			length = combinedProto.structInfo.height;
			      
			SetupSprite(combinedProto.structInfo.imgName);

			sprite.transform.localPosition = new Vector3(combinedProto.structInfo.imgHorizontalPixelOffset/25, combinedProto.structInfo.imgVerticalPixelOffset/25);

			name = combinedProto.structInfo.name;
		}
		else if (obstacle != null)
		{
			width = obstacle.obstacle.width;
			length = obstacle.obstacle.height;

			SetupSprite(obstacle.obstacle.imgName);

			sprite.transform.Translate(0, obstacle.obstacle.imgVerticalPixelOffset/25, 0, Space.Self);

			name = obstacle.obstacle.name;
		}
		_box.size = new Vector3(width * MSGridManager.instance.spaceSize, 1, 
	                        length * MSGridManager.instance.spaceSize);
		trans.position += new Vector3(SIZE_OFFSET.x * width, 0, SIZE_OFFSET.z * length);
		SetGridFromTrans();
		
		
		locallyOwned = (MSWhiteboard.currCityType == MSWhiteboard.CityType.PLAYER && MSWhiteboard.cityID == MSWhiteboard.localMup.userId);

		if (locallyOwned && combinedProto != null)
		{
			upgrade.Init(combinedProto.structInfo, userStructProto);

			switch (combinedProto.structInfo.structType) 
			{
			case StructureInfoProto.StructType.RESOURCE_GENERATOR:
				collector = gameObj.AddComponent<MSResourceCollector>();
				collector.Init(combinedProto);
				break;
			case StructureInfoProto.StructType.RESOURCE_STORAGE:
				storage = gameObj.AddComponent<MSResourceStorage>();
				break;
			case StructureInfoProto.StructType.HOSPITAL:
				hospital = gameObj.AddComponent<MSHospital>();
				break;
			default:
				break;
			}
		}

		sprite.transform.localPosition += new Vector3(-2.8f, 1.63f, -2.8f);
		floor.transform.localPosition = new Vector3(-2.8f, 1.63f, -2.8f);

		_box.center = Vector3.zero;

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
		sprite.sprite = MSAtlasUtil.instance.GetBuildingSprite(MSUtil.StripExtensions(structName));

		RuntimeAnimatorController animator = MSAtlasUtil.instance.GetAnimator(MSUtil.StripExtensions(structName));
		Animator spriteAnimator = sprite.GetComponent<Animator>();
		if (spriteAnimator != null)
		{
			if (animator != null)
			{
				spriteAnimator.runtimeAnimatorController = MSAtlasUtil.instance.GetAnimator(combinedProto.structInfo.imgName);
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

		//Set up shadow stuff
		shadow.sprite = MSAtlasUtil.instance.GetBuildingSprite(width + "x" + length + "shadow");
		floor.sprite = MSAtlasUtil.instance.GetBuildingSprite(width + "x" + length + "dark");
		
		
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
			MSActionManager.Town.PlaceBuilding -= Place;
		}
	}
	
	#endregion
	
	#region Building Selection & Movement

	void SetGridFromTrans ()
	{
		_currPos = new MSGridNode(new Vector2(transform.position.x / MSGridManager.instance.spaceSize - SIZE_OFFSET.x * width,
    	    transform.position.z / MSGridManager.instance.spaceSize - SIZE_OFFSET.z * length));
		
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

        trans.position = MSGridManager.instance.SnapPointToGrid(transform.position, width, length);

		SetGridFromTrans ();
		
		
		if (MSGridManager.instance.HasSpaceForBuilding(combinedProto.structInfo, _currPos))
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
	        if (_currPos.pos != groundPos && MSGridManager.instance.HasSpaceForBuilding(combinedProto.structInfo, _currPos))
	        {
	            MSGridManager.instance.AddBuilding(this, _currPos.x, _currPos.z, combinedProto.structInfo.width, combinedProto.structInfo.height);
				_originalPos = trans.position;
				
				SendBuildingMovedRequest();
	        }
	        else
	        {
	            MSGridManager.instance.AddBuilding(this, (int)groundPos.x, (int)groundPos.y, combinedProto.structInfo.width, combinedProto.structInfo.height);
	            trans.position = _originalPos;
	        }
		}
		MSActionManager.Town.PlaceBuilding -= Place;
		Deselect();
    }

	public void Confirm()
	{
		if(MSBuildingManager.instance.currentUnderConstruction != null)
		{
			MSActionManager.Popup.CreateButtonPopup("Your builder is busy! Speed him up for " + 
			                                        MSMath.GemsForTime(MSBuildingManager.instance.currentUnderConstruction.completeTime)
			                                        + "gems and build this structure?",
			                                        new string[]{"Cancel", "Speed Up"},
			new Action[]{MSActionManager.Popup.CloseTopPopupLayer,
				delegate
				{
					MSActionManager.Popup.CloseTopPopupLayer();
					MSBuildingManager.instance.currentUnderConstruction.CompleteWithGems();
					Confirm();
				}
			}
			);
			return;
		}

		if (MSBuildingManager.instance.BuyBuilding(this, Confirm))
		{
			long now = MSUtil.timeNowMillis;
			userStructProto.lastRetrieved = now;
			userStructProto.purchaseTime = now;
			userStructProto.isComplete = false;
			userStructProto.structId = combinedProto.structInfo.structId;
			userStructProto.userId = MSWhiteboard.localMup.userId;

			upgrade.StartConstruction();

			MSBuildingManager.instance.FullDeselect();
		}
	}

	public void Cancel()
	{
		Pool();
		MSBuildingManager.instance.hoveringToBuild = null;
	}
	
	void SendBuildingMovedRequest()
	{
		MoveOrRotateNormStructureRequestProto request = new MoveOrRotateNormStructureRequestProto();
		request.sender = MSWhiteboard.localMup;
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
			MSBuildingManager.instance.FullDeselect();
		}
		else if (!selected)
		{
			_originalPos = trans.position;
			_tempPos = trans.position;
			if (userStructProto != null)
			{
	        	MSGridManager.instance.RemoveBuilding(this);
			}
			MSActionManager.Town.PlaceBuilding += Place;
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

	public void CompleteWithGems()
	{
		if (upgrade != null && upgrade.timeRemaining > 0)
		{
			upgrade.FinishWithPremium();
		}
		else if (obstacle != null && obstacle.millisLeft > 0)
		{
			obstacle.FinishWithGems();
		}
	}

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
	
	public MSPoolable Make (Vector3 origin)
	{
		MSBuilding building = Instantiate(this, origin, Quaternion.identity) as MSBuilding;
		building.prefab = this;
		return building;
	}
	
	public void Pool ()
	{
		hoverIcon.gameObject.SetActive(false);

		//if (upgrade != null)
		//{
		//	Destroy (upgrade);
		//	upgrade = null;
		//}
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
		if (hospital != null)
		{
			Destroy (hospital);
			hospital = null;
		}
		if (!selected)
		{
			MSGridManager.instance.RemoveBuilding(this);
		}
		else
		{
			MSBuildingManager.instance.FullDeselect();
		}

		if (obstacle != null)
		{
			Destroy (obstacle);
			obstacle = null;
		}

		if (taskable != null)
		{
			Destroy(taskable);
			taskable = null;
		}
		MSPoolManager.instance.Pool(this);
	}
	
	#endregion
	
	#endregion
}
