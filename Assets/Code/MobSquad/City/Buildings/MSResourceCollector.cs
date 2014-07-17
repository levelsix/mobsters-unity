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
[RequireComponent (typeof (MSBuilding))]
public class MSResourceCollector : MonoBehaviour {
    
	bool hasMoney
	{
		get
		{
			return currMoney >= MONEY_THRESHOLD;
		}
	}

	public bool canCollect
	{
		get
		{
			return hasMoney && MSResourceManager.resources[(int)_generator.resourceType - 1] < MSResourceManager.maxes[(int)_generator.resourceType - 1];
		}
	}

	bool isGenerating
	{
		get
		{
			return enabled && _building.userStructProto.isComplete && !_building.upgrade.progressBar.upgrading;
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
			return (int)((MSUtil.timeNowMillis - lastTime)/1000f);
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

    private MSBuilding _building;
    
    private MSBuildingUpgrade _upgrade;

	/// <summary>
	/// A building needs to have at least this much money to be collected from
	/// </summary>
	const int MONEY_THRESHOLD = 1;
	
    void Awake()
    {
        _building = GetComponent<MSBuilding>();
        _upgrade = GetComponent<MSBuildingUpgrade>();
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
        _upgrade.OnFinishUpgrade += OnFinishUpgrade;
	}
	
	/// <summary>
	/// Raises the disable event.
	/// Release delegates
	/// </summary>
	void OnDisable()
	{
        _upgrade.OnFinishUpgrade -= OnFinishUpgrade;
	}

	public void Init(MSFullBuildingProto proto)
	{
		_generator = proto.generator;
		StartCoroutine(CheckMoney());
	}
	
	/// <summary>
	/// Collect this instance.
	/// </summary>
	public void Collect()
	{
		if (hasMoney && MSResourceManager.resources[(int)_generator.resourceType - 1] < MSResourceManager.maxes[(int)_generator.resourceType - 1])
		{
			MSResourceManager.instance.CollectFromBuilding(_generator.resourceType, currMoney, _building.userStructProto.userStructId);
			if (MSActionManager.Quest.OnMoneyCollected != null)
			{
				MSActionManager.Quest.OnMoneyCollected(_generator.resourceType, currMoney);
			}

			if(_building != null){
				MSResourceCollectLabel label = (MSPoolManager.instance.Get(_building.textLabel, Vector3.zero, _building.trans) as MSSimplePoolable).GetComponent<MSResourceCollectLabel>();
				label.label.text = currMoney.ToString();
				if(_generator.resourceType == ResourceType.CASH){
					label.setFontCash();
				}else{
					label.setFontOil();
				}

			}

			_building.userStructProto.lastRetrieved = MSUtil.timeNowMillis;
			_building.hoverIcon.gameObject.SetActive(false); 
			
			if (MSActionManager.Town.OnCollectFromBuilding != null)
			{
				MSActionManager.Town.OnCollectFromBuilding(_building);
			}

		}
	}
	
	void SendCollectRequest(int amount)
	{
		RetrieveCurrencyFromNormStructureRequestProto request = new RetrieveCurrencyFromNormStructureRequestProto();
		request.sender = MSWhiteboard.localMupWithResources;
		request.structRetrievals.Add(new com.lvl6.proto.RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval());
		request.structRetrievals[0].userStructId = _building.userStructProto.userStructId;
		request.structRetrievals[0].timeOfRetrieval = MSUtil.timeNowMillis;
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
			if (hasMoney)
			{
				_building.hoverIcon.gameObject.SetActive(true);
				if(canCollect){
					_building.hoverIcon.spriteName = (_generator.resourceType == ResourceType.CASH) ? "cashready" : "oilready";
				}else{
					_building.hoverIcon.spriteName = (_generator.resourceType == ResourceType.CASH) ? "cashoverflow" : "oiloverflow";
				}
			}
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
