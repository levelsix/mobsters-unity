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
			return currMoney >= MONEY_THRESHOLD;
		}
	}

	bool isGenerating
	{
		get
		{
			return enabled && _building.userStructProto.isComplete;
		}
	}
	
	/// <summary>
	/// The capacity.
	/// </summary>
	public int currMoney
	{
		get
		{
			if (!isGenerating)
			{ 
				return 0;
			}
			return (int)Mathf.Min(_generator.productionRate * (secsSinceCollect/3600f), _generator.capacity);
		}
	}

	int secsSinceCollect
	{
		get
		{
			if (!isGenerating)
			{
				return 0;
			}
			return (int)((CBKUtil.timeNowMillis - lastTime)/1000f);
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
	
	
	/// <summary>
	/// The last collection.
	/// </summary>
	protected long lastCollection;
    
	public ResourceGeneratorProto _generator;

    private CBKBuilding _building;
    
    private CBKBuildingUpgrade _upgrade;

	/// <summary>
	/// A building needs to have at least this much money to be collected from
	/// </summary>
	const int MONEY_THRESHOLD = 1;
	
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

	public void Init(CBKCombinedBuildingProto proto)
	{
		_generator = proto.generator;
		StartCoroutine(CheckMoney());
	}
	
	/// <summary>
	/// Collect this instance.
	/// </summary>
	void Collect()
	{
		if (hasMoney && CBKResourceManager.resources[(int)_generator.resourceType - 1] < CBKResourceManager.maxes[(int)_generator.resourceType - 1])
		{
			CBKResourceManager.instance.CollectFromBuilding(_generator.resourceType, currMoney, _building.userStructProto.userStructId);
			if (CBKEventManager.Quest.OnMoneyCollected != null)
			{
				CBKEventManager.Quest.OnMoneyCollected(currMoney);
			}

			_building.userStructProto.lastRetrieved = CBKUtil.timeNowMillis;
			_building.hasMoneyPopup.SetActive(false);
			
			if (CBKEventManager.Town.OnCollectFromBuilding != null)
			{
				CBKEventManager.Town.OnCollectFromBuilding(_building);
			}

		}
		else
		{
			Debug.LogWarning("Current money: " + currMoney + "\nTime since last collected: " + secsSinceCollect 
			                 + (isGenerating ? "\nIs" : "\nIsn't") + "Generating");
		}
	}
	
	void SendCollectRequest(int amount)
	{
		RetrieveCurrencyFromNormStructureRequestProto request = new RetrieveCurrencyFromNormStructureRequestProto();
		request.sender = CBKWhiteboard.localMupWithResources;
		request.structRetrievals.Add(new com.lvl6.proto.RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval());
		request.structRetrievals[0].userStructId = _building.userStructProto.userStructId;
		request.structRetrievals[0].timeOfRetrieval = CBKUtil.timeNowMillis;
		request.structRetrievals[0].amountCollected = amount;
		
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
		}
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	IEnumerator CheckMoney()
	{
		while(enabled)
		{
			_building.hasMoneyPopup.SetActive(hasMoney);
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
	
}
