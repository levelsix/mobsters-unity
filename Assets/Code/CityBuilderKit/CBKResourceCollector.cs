using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Component attached to a building to turn it into
/// a resource collector.
/// Gathers resources over time
/// </summary>
[RequireComponent (typeof (CBKBuilding))]
public class CBKResourceCollector : MonoBehaviour {
    
	bool hasMoney
	{
		get
		{
			return enabled && _building.userStructProto.isComplete && secondsUntilComplete <= 0;
		}
	}
	
	/// <summary>
	/// The capacity.
	/// </summary>
	public int income
	{
		get
		{
			return _building.structProto.income;
		}
	}
	
	/// <summary>
	/// The hourly rate at which this collector generates
	/// its resource.
	/// </summary>
	public int timeToGenerate
	{
		get
		{
			return _building.structProto.minutesToGain * 60 * 1000;
		}
	}
	
	public long secondsUntilComplete
	{
		get
		{
			//Debug.Log("Seconds:\nLast Retrieved: " + _building.userStructProto.lastRetrieved + "\nNow: " + CBKUtil.timeNow +
				//"\nTime to gen: " + timeToGenerate);
			if (_building.userStructProto.lastRetrieved + timeToGenerate < CBKUtil.timeNowMillis)
			{
				return 0;
			}
			return (_building.userStructProto.lastRetrieved + timeToGenerate) - CBKUtil.timeNowMillis;
		}
	}
	
	public long lastTime
	{
		get
		{
			return _building.userStructProto.lastRetrieved;
		}
		set
		{
			_building.userStructProto.lastRetrieved = value;
		}
	}
	
	public string timeLeftString
	{
		get
		{
			return CBKUtil.TimeStringShort(secondsUntilComplete);
		}
	}
	
	
	/// <summary>
	/// The last collection.
	/// </summary>
	protected long lastCollection;
    
    private CBKBuilding _building;
    
    private CBKBuildingUpgrade _upgrade;
	
	/// <summary>
	/// The UI Popup that signifies that this collector
	/// has enough resources to be harvested
	/// </summary>
	[SerializeField]
	GameObject hasResourcesPopup;
	
    void Awake()
    {
        _building = GetComponent<CBKBuilding>();
        _upgrade = GetComponent<CBKBuildingUpgrade>();
    }
    
	/*
    /// <summary>
    /// Initialize, using the specified protocol.
    /// </summary>
    /// <param name='proto'>
    /// Protocol instance.
    /// </param>
    public void Init(FullUserStructProto proto)
    {
        hourlyRate = proto.fullStruct.income;
        lastCollection = proto.lastCollectTime;
    }
    */
	
	/// <summary>
	/// Raises the enable event.
	/// Register delegates
	/// </summary>
	void OnEnable()
	{
		_building.OnSelect += Collect;
        _upgrade.OnFinishUpgrade += OnFinishUpgrade;
	}
	
	/// <summary>
	/// Raises the disable event.
	/// Release delegates
	/// </summary>
	void OnDisable()
	{
		_building.OnSelect -= Collect;
        _upgrade.OnFinishUpgrade -= OnFinishUpgrade;
	}
	
	public void Init(FullStructureProto proto)
	{
		StartCoroutine(CheckMoney());
		Init (true);
	}
	
	public void Init(bool operational)
	{
		enabled = operational;
		if (!enabled)
		{
			hasResourcesPopup.SetActive(false);
		}
	}
	
	/// <summary>
	/// Collect this instance.
	/// </summary>
	void Collect()
	{
		if (hasMoney)
		{
			CBKMoneyPickup money = CBKPoolManager.instance.Get(CBKPrefabList.instance.moneyPrefab, transform.position) as CBKMoneyPickup;
			money.Init(_building, income);
			
			_building.userStructProto.lastRetrieved = CBKUtil.timeNowMillis;
			hasResourcesPopup.SetActive(false);
			
			SendCollectRequest();
		}
	}
	
	void SendCollectRequest()
	{
		RetrieveCurrencyFromNormStructureRequestProto request = new RetrieveCurrencyFromNormStructureRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.structRetrievals.Add(new com.lvl6.proto.RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval());
		request.structRetrievals[0].userStructId = _building.userStructProto.userStructId;
		request.structRetrievals[0].timeOfRetrieval = CBKUtil.timeNowMillis;
		
		Debug.Log("Collecting from: " + _building.userStructProto.userStructId);
		
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT, LoadCollectResponse);
	}
	
	void LoadCollectResponse(int tagNum)
	{
		RetrieveCurrencyFromNormStructureResponseProto response = (RetrieveCurrencyFromNormStructureResponseProto)UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != RetrieveCurrencyFromNormStructureResponseProto.RetrieveCurrencyFromNormStructureStatus.SUCCESS)
		{
			Debug.LogError("Problem collecting money: " + response.status.ToString());
			if (response.status == RetrieveCurrencyFromNormStructureResponseProto.RetrieveCurrencyFromNormStructureStatus.CLIENT_TOO_APART_FROM_SERVER_TIME)
			{
				Debug.Log("Client time: " + CBKUtil.timeNowMillis);
			}
		}
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	IEnumerator CheckMoney()
	{
		while(enabled)
		{
			hasResourcesPopup.SetActive(hasMoney);
			if (_building.userStructProto.isComplete && _building.OnUpdateValues != null)
			{
				_building.OnUpdateValues();
			}
			yield return new WaitForSeconds(1f);
		}
	}
	
	public void OnFinishUpgrade ()
	{
		_building.userStructProto.lastRetrieved = _building.userStructProto.purchaseTime;//_building.userStructProto.lastUpgradeTime;
	}
	
	public int MoneyAtLevel(int level)
	{
		return _building.structProto.income * level;
	}
	
}
