using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Manager for buildings
/// Keeps track of all buildings in the scene
/// As well as works as the intermediatary for controls interacting with the buildings
/// </summary>
public class CBKBuildingManager : MonoBehaviour
{
	#region Members
	
	#region Singleton Instance
	
	public static CBKBuildingManager instance;
	
	#endregion
	
    #region Public members

    /// <summary>
    /// Prefab for a building
    /// </summary>
    public CBKBuilding buildingPrefab;
	
	/// <summary>
	/// The scene's camera, which this needs to pass
	/// along drag events when the camera is the target
	/// </summary>
	public CBKBuildingCamera cam;
 
	public GameObject BuilderBar;
	
	public bool builderAvailable = true;
	
    private static int _nextId = 0;
    public static int getNextId{
        get
        {
            return _nextId++;
        }
    }
    
	[SerializeField]
	CBKUnit unitPrefab;
	
	[SerializeField]
	Transform unitParent;
	
	[SerializeField]
	Transform buildingParent;
	
    #endregion

    #region Private
	
	private Dictionary<int, CBKBuilding> buildings = new Dictionary<int, CBKBuilding>();
	
	private Dictionary<long, CBKUnit> units = new Dictionary<long, CBKUnit>();
	
	/// <summary>
	/// The current selected building.
	/// </summary>
	private CBKISelectable _selected;
	
	/// <summary>
	/// The current target, which will be moved as long
	/// as a drag continues
	/// </summary>
	private CBKIPlaceable _target;
    
    #endregion

    #region Constants

    /// <summary>
    /// Number of small buildings to make
    /// </summary>
    const int NUM_SMALL_BUILDINGS = 5;

    /// <summary>
    /// Number of medium buildings to make
    /// </summary>
    const int NUM_MED_BUILDINGS = 3;

    /// <summary>
    /// Number of large buildings to make
    /// </summary>
    const int NUM_LARGE_BUILDINGS = 2;

    #endregion
	
	#endregion

    #region Functions
	
	#region Initialization/Clean-up
	
	void Awake()
	{
		instance = this;
	}
	
	/// <summary>
	/// Raises the enable event.
	/// Sets up the event delegates
	/// </summary>
	void OnEnable ()
	{
		CBKEventManager.Loading.LoadBuildings += RequestCity;
		CBKEventManager.Controls.OnTap[0] += OnTap;
		CBKEventManager.Controls.OnKeepDrag[0] += OnDrag;
		CBKEventManager.Controls.OnStartHold[0] += OnStartHold;
		CBKEventManager.Controls.OnReleaseDrag[0] += OnReleaseDrag;
		CBKEventManager.Controls.OnStartDrag[0] += OnStartDrag;
		CBKEventManager.Town.PlaceBuilding += OnPlace;
	}
	
	/// <summary>
	/// Raises the disable event.
	/// Removes all event delegates
	/// </summary>
	void OnDisable ()
	{
		CBKEventManager.Loading.LoadBuildings -= RequestCity;
		CBKEventManager.Controls.OnTap[0] -= OnTap;
		CBKEventManager.Controls.OnKeepDrag[0] -= OnDrag;
		CBKEventManager.Controls.OnStartHold[0] -= OnStartHold;
		CBKEventManager.Controls.OnStartDrag[0] -= OnStartDrag;
		CBKEventManager.Controls.OnReleaseDrag[0] -= OnReleaseDrag;
		CBKEventManager.Town.PlaceBuilding -= OnPlace;
	}
	
	#endregion
	
	#region Building Generation
	
	public void RequestCity()
	{
		Debug.Log("Sending city request");
		if (CBKWhiteboard.currCityType == CBKWhiteboard.CityType.PLAYER)
		{
			LoadPlayerCityRequestProto load = new LoadPlayerCityRequestProto();
			load.sender = CBKWhiteboard.localMup;
			load.cityOwnerId = CBKWhiteboard.cityID;
			UMQNetworkManager.instance.SendRequest(load, (int)EventProtocolRequest.C_LOAD_PLAYER_CITY_EVENT, LoadPlayerCity);
		}
		else
		{
			StartCoroutine(LoadNeutralCity(CBKWhiteboard.cityID));
		}
	}
	
	public IEnumerator LoadNeutralCity(int cityId)
	{
		LoadCityRequestProto load = new LoadCityRequestProto();
		load.sender = CBKWhiteboard.localMup;
		load.cityId = cityId;
		int cityTag = UMQNetworkManager.instance.SendRequest(load, (int)EventProtocolRequest.C_LOAD_CITY_EVENT, null);
		
		RecycleCity();
		
		while(!UMQNetworkManager.responseDict.ContainsKey(cityTag))
		{
			yield return null;
		}
		
		LoadCityResponseProto response = UMQNetworkManager.responseDict[cityTag] as LoadCityResponseProto;
		UMQNetworkManager.responseDict.Remove(cityTag);
		
		Debug.Log("Loading neutral city: " + response.cityId);

		for (int i = 0; i < response.cityElements.Count; i++) 
		{
			//Debug.Log("Making neutral element " + i);
			switch (response.cityElements[i].type) {
				case CityElementProto.CityElemType.BUILDING:
					MakeBuilding(response.cityElements[i]);
					break;
				case CityElementProto.CityElemType.PERSON_NEUTRAL_ENEMY:
				case CityElementProto.CityElemType.BOSS:
					MakeNPC(response.cityElements[i]);
					break;
				default:
					break;
			}
		}
		
		foreach (FullTaskProto task in CBKDataManager.instance.GetAll(typeof(FullTaskProto)).Values)
		{
			if (task.cityId == cityId)
			{
				if(buildings.ContainsKey(task.assetNumWithinCity))
				{
					buildings[task.assetNumWithinCity].taskable.task = task;
				}
				else if(units.ContainsKey(task.assetNumWithinCity))
				{
					units[task.assetNumWithinCity].task = task;
				}
			}
		}
		
		if (CBKEventManager.Scene.OnCity != null)
		{
			CBKEventManager.Scene.OnCity();
		}
	}
	
	public void LoadPlayerCity(int tagNum)
	{
		RecycleCity();
		
		LoadPlayerCityResponseProto response = (LoadPlayerCityResponseProto) UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		Debug.Log("Loading city for player: " + response.cityOwner.name);
		
		for (int i = 0; i < response.ownerNormStructs.Count; i++) 
		{
			MakeBuilding(response.ownerNormStructs[i]);
			//CBKBuilding building = MakeBuildingAt((FullStructureProto) CBKDataManager.instance.Get(typeof(FullStructureProto), response.ownerNormStructs[i].structId), 
			//	(int)response.ownerNormStructs[i].coordinates.x, (int)response.ownerNormStructs[i].coordinates.y);
			//building.userStructProto = response.ownerNormStructs[i];
		}
		
		foreach (var item in CBKMonsterManager.instance.userMonsters) {
			if (item.Value.userMonster.isComplete)
			{
				CBKUnit dude = CBKPoolManager.instance.Get(unitPrefab, Vector3.zero) as CBKUnit;
				dude.transf.parent = unitParent;
				dude.Init(item.Value.userMonster);
				units.Add(item.Key, dude);
			}
		}
		
		if (CBKEventManager.Scene.OnCity != null)
		{
			CBKEventManager.Scene.OnCity();
		}
	}
	
	void MakeNPC(CityElementProto element)
	{
		CBKUnit unit = CBKPoolManager.instance.Get(unitPrefab, Vector3.zero) as CBKUnit;
		unit.transf.parent = unitParent;
		unit.Init(element);
		units.Add(element.assetId, unit);
	}
	
	CBKBuilding MakeBuildingAt (FullStructureProto proto, int id, int x, int y)
	{
		Vector3 position = new Vector3(CBKGridManager.instance.spaceSize * x, 0, 
    		CBKGridManager.instance.spaceSize * y);
    	
	    CBKBuilding building = Instantiate(buildingPrefab, position, buildingParent.rotation) as CBKBuilding;
    	
    	building.trans.parent = buildingParent;
    	building.Init(proto);
    	
	    CBKGridManager.instance.AddBuilding(building, x, y, proto.xLength, proto.yLength);
	
		buildings.Add(id, building);
		
    	return building;
	}
	
	CBKBuilding MakeBuilding(FullUserStructureProto proto)
	{
		Vector3 position = new Vector3(CBKGridManager.instance.spaceSize * proto.coordinates.x, 0, 
    		CBKGridManager.instance.spaceSize * proto.coordinates.y);
    	
	    CBKBuilding building = CBKPoolManager.instance.Get(buildingPrefab, position) as CBKBuilding;
		
		building.trans.rotation = buildingParent.rotation;
    	building.trans.parent = buildingParent;
    	building.Init(proto);
    	
		FullStructureProto fsp = CBKDataManager.instance.Get(typeof(FullStructureProto), proto.structId) as FullStructureProto;
	    CBKGridManager.instance.AddBuilding(building, (int)proto.coordinates.x, (int)proto.coordinates.y, fsp.xLength, fsp.yLength);
	
		buildings.Add(proto.userStructId, building);
		
    	return building;
	}
	
	CBKBuilding MakeBuilding(CityElementProto proto)
	{
		Debug.Log("Neutral building " + proto.imgId + " at " + proto.coords.x + ", " + proto.coords.y);
		
		Vector3 position = new Vector3(CBKGridManager.instance.spaceSize * proto.coords.x, 0, 
    		CBKGridManager.instance.spaceSize * proto.coords.y);
    	
	    CBKBuilding building = CBKPoolManager.instance.Get(buildingPrefab, position) as CBKBuilding;
		
		building.trans.rotation = buildingParent.rotation;
    	building.trans.parent = buildingParent;
    	building.Init(proto);
    	
	    CBKGridManager.instance.AddBuilding(building, (int)proto.coords.x, (int)proto.coords.y, proto.xLength, proto.yLength);
	
		buildings.Add(proto.assetId, building);
		
    	return building;
	}
	
	private void BuyBuilding(FullStructureProto proto)
	{
		bool hasEnoughMoney;
		if (proto.cashPrice > proto.gemPrice)
		{
			hasEnoughMoney = CBKResourceManager.instance.Spend(CBKResourceManager.ResourceType.FREE, proto.cashPrice);
		}
		else
		{
			hasEnoughMoney = CBKResourceManager.instance.Spend(CBKResourceManager.ResourceType.PREMIUM, proto.gemPrice);
		}
		
		if (hasEnoughMoney)
		{
			PurchaseNormStructureRequestProto request = new PurchaseNormStructureRequestProto();
			request.sender = CBKWhiteboard.localMup;
			
			CBKGridNode coords = CBKGridManager.instance.ScreenToPoint(new Vector3(Screen.width/2, Screen.height/2));
			coords = FindSpaceInRange(proto, coords, 0);
			
			request.structCoordinates = new CoordinateProto();
			request.structCoordinates.x = coords.pos.x;
			request.structCoordinates.y = coords.pos.y;
			
			request.structId = proto.structId;
			request.timeOfPurchase = CBKUtil.timeNowMillis;
			
			CBKWhiteboard.tempStructureProto = proto;
			CBKWhiteboard.tempStructurePos = coords;
			
			UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_PURCHASE_NORM_STRUCTURE_EVENT, PurchaseBuildingResponse);
			
			//TODO: Make Game hang on response
		}
	}
	
	private void PurchaseBuildingResponse(int tagNum)
	{
		PurchaseNormStructureResponseProto response = UMQNetworkManager.responseDict[tagNum] as PurchaseNormStructureResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		FullStructureProto proto = CBKWhiteboard.tempStructureProto;
		
		if (response.status == PurchaseNormStructureResponseProto.PurchaseNormStructureStatus.SUCCESS)
		{
	        CBKBuilding building = MakeBuildingAt(proto, response.userStructId,
				(int)CBKWhiteboard.tempStructurePos.pos.x, (int)CBKWhiteboard.tempStructurePos.pos.y);
			
			building.userStructProto.userStructId = response.userStructId;
		}
		else
		{
			Debug.LogError("Problem building building: " + response.status.ToString());
		}
	}
	
	/// <summary>
	/// Makes a building, adding it to the closest place near the center of the current screen possible
	/// </summary>
	/// <returns>
	/// The created building
	/// </returns>
	/// <param name='proto'>
	/// The prefab for the building to be made
	/// </param>
	private CBKBuilding MakeBuildingCenter(FullStructureProto proto)
	{
		CBKGridNode coords = CBKGridManager.instance.ScreenToPoint(new Vector3(Screen.width/2, Screen.height/2));
		coords = FindSpaceInRange(proto, coords, 0);
		
		Vector3 position = new Vector3(CBKGridManager.instance.spaceSize * coords.x, 0, 
			CBKGridManager.instance.spaceSize * coords.z);
		
		CBKBuilding building = Instantiate(buildingPrefab, position, buildingParent.rotation) as CBKBuilding;
		
		building.trans.parent = buildingParent;
		building.Init(proto);
		building.upgrade.StartConstruction();
        CBKGridManager.instance.AddBuilding(building, (int)coords.x, (int)coords.z, proto.xLength, proto.yLength);

        return building;
	}
	
	#endregion
	
	#region Building Placement
	
	/// <summary>
	/// Checks all of the spaces in the given range.
    /// If no appropriate space, recursively inreases the range
	/// </summary>
	/// <returns>
	/// The space where this building can fit
	/// </returns>
	/// <param name='proto'>
	/// Building prefab.
	/// </param>
	/// <param name='startPos'>
	/// Start position.
	/// </param>
	/// <param name='range'>
	/// Range.
	/// </param>
	public CBKGridNode FindSpaceInRange(FullStructureProto proto, CBKGridNode startPos, int range)
	{
		if (range > 36)
		{
			throw new System.Exception("Not enough room to place the building. Throw a popup and refund.");
		}
		for (int i = 0; i <= range; i++) 
		{
			CBKGridNode space;
			space = CheckSpaces(proto, startPos, range, i);
			if (space != null && space.x >= 0)
			{
				return space;
			}
			space = CheckSpaces(proto, startPos, i, range);
			if (space != null && space.x >= 0)
			{
				return space;
			}
		}
		return FindSpaceInRange(proto, startPos, range+1);
	}
	
	/// <summary>
	/// Checks the spaces using the given derivations from the base position
	/// </summary>
	/// <returns>
	/// The spaces.
	/// </returns>
	/// <param name='proto'>
	/// Building prefab.
	/// </param>
	/// <param name='basePos'>
	/// Base position.
	/// </param>
	/// <param name='x'>
	/// X derivation
	/// </param>
	/// <param name='y'>
	/// Y derivation
	/// </param>
	public CBKGridNode CheckSpaces(FullStructureProto proto, CBKGridNode basePos, int x, int y)
	{
		if (CBKGridManager.instance.HasSpaceForBuilding(proto, basePos + new CBKGridNode(x,y)))
		{
			return basePos+new CBKGridNode(x,y);	
		}
		if (x==0 || y==0) return new CBKGridNode(-1,-1);
		if (CBKGridManager.instance.HasSpaceForBuilding(proto, basePos + new CBKGridNode(-x,y)))
		{
			return basePos+new CBKGridNode(-x,y);	
		}
		if (CBKGridManager.instance.HasSpaceForBuilding(proto, basePos + new CBKGridNode(x,-y)))
		{
			return basePos+new CBKGridNode(x,-y);	
		}
		if (CBKGridManager.instance.HasSpaceForBuilding(proto, basePos + new CBKGridNode(-x,-y)))
		{
			return basePos+new CBKGridNode(-x,-y);	
		}
		
		return new CBKGridNode(-1,-1);
	}
	
	#endregion
	
	#region Grid/Building Control (Adding/Removing)
	
	public Collider SelectSomethingFromScreen(Vector2 point)
	{
		//Cast a ray using the mouse position
        Ray ray = Camera.main.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //If our ray hits, select that building
            return hit.collider;
        }
        else
        {
            return null;
        }
	}
	
	/// <summary>
	/// Selects the building from a touch or click
	/// </summary>
	/// <param name='point'>
	/// Point on screen
	/// </param>
	public CBKBuilding SelectBuildingFromScreen(Vector2 point)
	{
		Collider coll = SelectSomethingFromScreen(point);
		if (coll != null)
		{
			return coll.GetComponent<CBKBuilding>();
		}
		return null;
	}
	
	/// <summary>
	/// Deselect the current building, if one is selected, and place it
	/// </summary>
	private void Deselect()
	{
		if (_selected != null)
		{
			_selected.Deselect();
			if (_selected is CBKBuilding)
			{
				CBKEventManager.Town.PlaceBuilding();	
			}
			_selected = null;
			_target = null;
		}
	}
	
	private void SetSelectedBuilding(CBKBuilding building)
	{
		if (_selected != null)
		{
			Deselect();
		}
		_selected = building;
		building.Select();
		if (CBKEventManager.Town.OnBuildingSelect != null)
		{
			CBKEventManager.Town.OnBuildingSelect(building);
		}
	}
	
	private void SetSelectedUnit(CBKCityUnit unit)
	{
		if (_selected != null)
		{
			Deselect();
		}
		_selected = unit;
		unit.Select();
	}
	
	/// <summary>
	/// Deselect the current building and change to having no building selected
	/// </summary>
	private void FullDeselect()
	{
		Deselect();
		if (CBKEventManager.Town.OnBuildingSelect != null)
		{
			CBKEventManager.Town.OnBuildingSelect(null);
		}
	}
	
	private void RecycleCity()
	{
		foreach (CBKBuilding item in buildings.Values) 
		{
			item.Pool();
		}
		foreach (CBKUnit item in units.Values) 
		{
			item.Pool();
		}
		buildings.Clear();
		units.Clear();
	}
	
	#endregion
	
	#region Debug
	
#if UNITY_EDITOR
	/// <summary>
	/// Cheats.
	/// Update this instance.
	/// </summary>
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (CBKEventManager.Town.PlaceBuilding != null)
			{
				CBKEventManager.Town.PlaceBuilding();
			}
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			CBKGridManager.instance.DebugPrintGrid();	
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			//MakeBuildingCenter(CBKBuildingList.instance.sevenEleven);	
			BuyBuilding(CBKDataManager.instance.Get(typeof(FullStructureProto), 2) as FullStructureProto);
		}
	}
#endif
	
	#endregion
	
	#region Control Delegates
	
	/// <summary>
	/// Casts a ray from the tap point
 	/// Selects or deselects a building
	/// </summary>
	/// <param name='touch'>
	/// Touch.
	/// </param>
	public void OnTap(TCKTouchData touch)
	{        
		Collider hit = SelectSomethingFromScreen(touch.pos);
		if (hit != null){
			CBKBuilding building = hit.GetComponent<CBKBuilding>();
			if (building != null)
			{
				if (building != _selected)
				{
					SetSelectedBuilding(building);
				}
				else if (building.OnSelect != null)
				{
					building.OnSelect();
				}
			}
			else
			{
				CBKCityUnit unit = hit.GetComponent<CBKCityUnit>();
				if (unit != null)
				{
					SetSelectedUnit (unit);
				}
			}
		}
		else //if (hit.GetComponent<CBKGround>() != null)
		{
			FullDeselect();
		}
		
	}
	
	/// <summary>
	/// Raises the start hold event.
	/// If we start holding on a building, select it and get it ready to start moving
	/// If we start holding on empty space, get the camera ready to be moved
	/// </summary>
	/// <param name='touch'>
	/// Touch.
	/// </param>
	public void OnStartHold(TCKTouchData touch)
	{
		//If we're on a building, select it so that if we start dragging, we can move it
		CBKBuilding chosen = SelectBuildingFromScreen(touch.pos);
		if (chosen != null)
		{
			if (chosen != _selected){
				SetSelectedBuilding(chosen);
			}
			_target = chosen;
		}
		if (_selected == null || _target == null)
		{
			_target = cam;
		}
	}
	
	/// <summary>
	/// Raises the start drag event.
	/// If the drag started on the selected building, target that building
	/// Otherwise, target the camera
	/// </summary>
	/// <param name='touch'>
	/// Touch.
	/// </param>
	public void OnStartDrag(TCKTouchData touch)
	{
		CBKBuilding touched = SelectBuildingFromScreen(touch._initialPos);
		if (_selected != null && touched == _selected && touched.locallyOwned)
		{
			_target = touched;
		}
		else
		{
			_target = cam;
		}
	}
	
	/// <summary>
	/// Raises the release drag event.
	/// When a drag is released, if we have a building selected, drop it.
	/// </summary>
	/// <param name='touch'>
	/// Touch.
	/// </param>
	public void OnReleaseDrag(TCKTouchData touch)
	{
		if (_target != null && _target is CBKBuilding)
		{
			(_target as CBKBuilding).Drop();
		}
	}
	
	/// <summary>
	/// Raises the drag event.
	/// As long as we're dragging, move the target.
	/// </summary>
	/// <param name='touch'>
	/// Touch.
	/// </param>
	public void OnDrag(TCKTouchData touch)
	{
		//Move building or camera
		if (_target != null){
			_target.MoveRelative(touch);
		}
	}
	
	/// <summary>
	/// Raises the place event.
	/// If we place a building, null out the selected building pointer
	/// </summary>
	public void OnPlace()
	{
		_selected = null;	
	}
	
	#endregion
	
    #endregion
}
