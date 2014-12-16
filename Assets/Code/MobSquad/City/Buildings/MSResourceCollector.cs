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
    
	public ResourceType resource
	{
		get
		{
			return _generator.resourceType;
		}
	}

	public bool hasMoney
	{
		get
		{
			return currMoney > moneyThreshold;
		}
	}

	public bool canCollect
	{
		get
		{
			return hasMoney && MSResourceManager.resources[_generator.resourceType] < MSResourceManager.maxes[(int)_generator.resourceType - 1];
		}
	}

	bool isGenerating
	{
		get
		{
			return enabled && !_building.userStructProto.userStructUuid.Equals("") && _building.userStructProto.isComplete && !_building.upgrade.progressBar.upgrading;
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
			return (int)Mathf.Min(_generator.productionRate * (secsSinceCollect/3600f) + overflow, _generator.capacity);
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

	public long timeUntilFull
	{
		get
		{
			float resourceLeft = _generator.capacity - currMoney;
			return (long)(resourceLeft / _generator.productionRate * 36000);
		}
	}
	
	
	/// <summary>
	/// The last collection.
	/// </summary>
	protected long lastCollection;
    
	public ResourceGeneratorProto _generator;

    private MSBuilding _building;
    
    private MSBuildingUpgrade _upgrade;

	public bool unblockClick = false;

	int overflow = 0;

	[SerializeField]
	float unblockClickTime = 10f;

	[SerializeField]
	float iconFadeTime = 0.3f;

	/// <summary>
	/// A building needs to have at least this much money to be collected from
	/// </summary>
	int moneyThreshold
	{
		get
		{
			return (int)_generator.productionRate/10;
		}
	}

	const float FLOAT_ICON_MISSION_HEIGHT = 1.75f;
	
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
		if (hasMoney && MSResourceManager.resources[_generator.resourceType] < MSResourceManager.maxes[(int)_generator.resourceType - 1])
		{
			if (_generator.resourceType == ResourceType.CASH)
			{
				MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.collectCash);
			}
			else
			{
				MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.collectOil);
			}

			MSResourceManager.instance.CollectFromBuilding(_generator.resourceType, currMoney, _building.userStructProto.userStructUuid, _generator.productionRate);
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

			overflow = MSResourceManager.instance.GetOverFlow(_generator.resourceType, currMoney);
			_building.userStructProto.lastRetrieved = MSUtil.timeNowMillis;
			if(canCollect)
			{
				StartCoroutine(HideHoverIcon());
			}
			else
			{
				_building.hoverIcon.gameObject.SetActive(false); 
			}
			
			if (MSActionManager.Town.OnCollectFromBuilding != null)
			{
				MSActionManager.Town.OnCollectFromBuilding(_building);
			}

		}
		else
		{
			if(!unblockClick)
			{
				StartCoroutine(HideHoverIcon());

				if (_generator.resourceType == ResourceType.CASH)
				{
					MSActionManager.Popup.DisplayRedError("Your  storage is full. Upgrade or build more Cash Vaults to store more.");
				}
				else
				{
					MSActionManager.Popup.DisplayRedError("Your storage is full. Upgrade or build more Oil Silos to store more");
				}

			}
		}
	}
	
	void SendCollectRequest(int amount)
	{
		RetrieveCurrencyFromNormStructureRequestProto request = new RetrieveCurrencyFromNormStructureRequestProto();
		request.sender = MSWhiteboard.localMupWithResources;
		request.structRetrievals.Add(new com.lvl6.proto.RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval());
		request.structRetrievals[0].userStructUuid = _building.userStructProto.userStructUuid;
		request.structRetrievals[0].timeOfRetrieval = MSUtil.timeNowMillis;
		request.structRetrievals[0].amountCollected = amount;
		
		//Debug.Log("Collecting from: " + _building.userStructProto.userStructUuid);
		
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
				_building.hoverIcon.transform.localPosition = new Vector3(0, FLOAT_ICON_MISSION_HEIGHT);
				if(canCollect && !MSTutorialManager.instance.inTutorial){
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

	IEnumerator HideHoverIcon()
	{
		unblockClick = true;
		TweenAlpha.Begin(_building.hoverIcon.gameObject, iconFadeTime, 0f);
		yield return new WaitForSeconds(unblockClickTime);
		TweenAlpha.Begin(_building.hoverIcon.gameObject, iconFadeTime, 1f);
		yield return new WaitForSeconds(iconFadeTime);
		unblockClick = false;
	}
	
}
