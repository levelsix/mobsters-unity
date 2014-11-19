using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;
using Soomla.Store;

/// <summary>
/// @author Rob Giusti
/// Keeps track of the local player's resources and experience level
/// </summary>


public class MSResourceManager : MonoBehaviour {
	
	public static MSResourceManager instance;
	
	[SerializeField]
	MSLevelUpPopup levelUpPopup;

	[SerializeField]
	MSPopup shopMenuPopup;

	[SerializeField]
	MSTabButton fundsTab;

	/// <summary>
	/// The resources.
	/// Indexed by subtracting one from the ResourceType enum
	/// </summary>
	public static Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>() {
		{ResourceType.CASH, 0}, 
		{ResourceType.OIL, 0},
		{ResourceType.GEMS, 0}
	};

	public static int[] maxes = {0, 0, int.MaxValue};
	
	int _level = 0;
	
	int level{get{return _level;}}
	
	int _exp = 0;
	
	int exp{get{return _exp;}}
	
	int expForNextLevel{
		get
		{
			return MSWhiteboard.nextLevelInfo.requiredExperience;
		}
	}

	bool checkingEXP = false;

	RetrieveCurrencyFromNormStructureRequestProto retrieveRequest = null;

	/// <summary>
	/// If this much time has passed without a building having money collected from it, the request will be sent
	/// </summary>
	const float COLLECT_TIME_OUT = 15f;

	const int BASE_RESOURCE_AMOUNT = 1000;

	int currCollectRequests = 0;

	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		MSActionManager.Scene.OnCity += CheckEXP;
		MSActionManager.Scene.OnCity += DetermineResourceMaxima;
	}

	void OnDisable()
	{
		MSActionManager.Scene.OnCity -= CheckEXP;
		MSActionManager.Scene.OnCity -= DetermineResourceMaxima;
	}
	
	public void Init(int lev, int xp, int cash, int oil, int premium)
	{
		_level = lev;
		_exp = xp;
		
		resources[ResourceType.CASH] = cash;
		resources[ResourceType.OIL] = oil;
		resources[ResourceType.GEMS] = premium;

		foreach (var item in resources) 
		{
			if (MSActionManager.UI.OnChangeResource[item.Key] != null)
			{
				Debug.Log("Resource Setup: " + item.ToString());
				MSActionManager.UI.OnChangeResource[item.Key](item.Value);
			}
		}
	}

	public void DetermineResourceMaxima()
	{
		if (MSBuildingManager.townHall != null)
		{
			maxes[0] = maxes[1] = MSBuildingManager.townHall.combinedProto.townHall.resourceCapacity;
		}
		else
		{
			maxes[0] = maxes[1] = 1000;
		}
		foreach (ResourceStorageProto item in MSBuildingManager.instance.GetAllStorages()) 
		{
			maxes[(int)item.resourceType-1] += item.capacity;
		}

		if (MSActionManager.UI.OnSetResourceMaxima != null)
		{
			MSActionManager.UI.OnSetResourceMaxima(maxes);
		}
	}

	public void CollectFromBuilding(ResourceType resource, int amount, string userStructUuid)
	{
		Collect(resource, amount);

		if (retrieveRequest == null)
		{
			retrieveRequest = new RetrieveCurrencyFromNormStructureRequestProto();
			retrieveRequest.sender = MSWhiteboard.localMupWithResources;
		}

		RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval structRetrieval = new RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval();
		structRetrieval.amountCollected = amount;
		structRetrieval.userStructUuid = userStructUuid;
		structRetrieval.timeOfRetrieval = MSUtil.timeNowMillis;

		retrieveRequest.structRetrievals.Add(structRetrieval);

		StartCoroutine(WaitForMoreCollects());
	}

	IEnumerator WaitForMoreCollects()
	{
		currCollectRequests++;
		yield return new WaitForSeconds(COLLECT_TIME_OUT);
		if (--currCollectRequests == 0)
		{
			RunCollectResources();
		}
	}

	/// <summary>
	/// Collect the specified resource and amount.
	/// On success, returns 0
	/// If there is not enough capacity, returns the amount
	/// that was unable to be stored
	/// </summary>
	/// <param name='resource'>
	/// Resource type.
	/// </param>
	/// <param name='amount'>
	/// Amount.
	/// </param>
	public void Collect(ResourceType resource, int amount)
	{
		resources[resource] = Mathf.Min(resources[resource] + amount, maxes[(int)resource-1]);
		
		if (MSActionManager.UI.OnChangeResource[resource] != null)
		{
			MSActionManager.UI.OnChangeResource[resource](resources[resource]);
		}
	}

	/// <summary>
	/// When the player spends gems and 
	/// </summary>
	/// <returns><c>true</c>, if all was spent, <c>false</c> otherwise.</returns>
	/// <param name="resource">Resource.</param>
	public int SpendAll(ResourceType resource)
	{
		int amount = resources[resource];
		Spend (resource, amount);
		return amount;
	}
	
	/// <summary>
	/// Spend the specified resource and amount.
	/// Returns true if the amount was spent.
	/// Returns false if there was not enough of the
	/// specified resource to afford payment.
	/// </summary>
	/// <param name='resource'>
	/// If set to <c>true</c> resource.
	/// </param>
	/// <param name='amount'>
	/// If set to <c>true</c> amount.
	/// </param>
	public bool Spend(ResourceType resource, int amount, Action action = null)
	{
		if (resources[resource] >= amount)
		{
			resources[resource] -= amount;
			if (MSActionManager.UI.OnChangeResource[resource] != null)
			{
				MSActionManager.UI.OnChangeResource[resource](resources[resource]);
			}
			return true;
		}
		Debug.LogWarning("Tried to spend " + amount + " " + resource.ToString() + ", only have " + resources[resource].ToString());

		if (resource == ResourceType.GEMS)
		{
			//Prompt to buy more gems
			MSPopupManager.instance.CreatePopup("Not Enough Gems",
				"You don't have enough gems. Want more?",
	            new string[] {"Enter Shop"},
				new string[] {"purplemenuoption"},
				new Action[] {
					delegate 
					{
					MSActionManager.Popup.CloseAllPopups(); 
					MSBuildingManager.instance.FullDeselect();
					MSActionManager.Popup.OnPopup(shopMenuPopup);
					fundsTab.OnClick();
					}
				},
				"purple"
				);
		}
		else //Or do we? if (amount <= maxes[(int)resource-1]) //We don't want to let users use gem buys to overcome resource limits
		{
			//Prompt to convert gems to currency
			int resourceNeeded = amount - resources[resource];
			int gemsNeeded = Mathf.CeilToInt(resourceNeeded * MSWhiteboard.constants.gemsPerResource);

			MSPopupManager.instance.CreatePopup("Not enough resources!",
				"Spend (G)" + gemsNeeded + " to buy the remaining " + ((resource== ResourceType.CASH)?"$":"(O)") + resourceNeeded + "?",
                new string[] {"No", "Yes"},
				new string[] {"greymenuoption", "purplemenuoption"},
				new Action[] { 
					MSActionManager.Popup.CloseTopPopupLayer,
					delegate{ MSActionManager.Popup.CloseTopPopupLayer(); if (action!=null) action();}
				},
				"purple"
			);
		}

		return false;
	}

	public void FillByPercentage(ResourceType resource, float percent)
	{
		int resources = Mathf.CeilToInt(maxes[(int)resource-1] * percent);
		int gems = Mathf.CeilToInt(resources * MSWhiteboard.constants.gemsPerResource);

		if (Spend(ResourceType.GEMS, gems))
		{
			Collect(resource, resources);
		}
	}

	/// <summary>
	/// Spends the gems for other resource.
	/// </summary>
	/// <returns>The gems for other resource.</returns>
	/// <param name="otherResource">Other resource.</param>
	/// <param name="amountToSpend">Amount to spend.</param>
	public int SpendGemsForOtherResource(ResourceType otherResource, int amountNeeded, Action action = null)
	{
		int gems = Mathf.CeilToInt(MSWhiteboard.constants.gemsPerResource * amountNeeded);

		int resources = Mathf.CeilToInt(gems / MSWhiteboard.constants.gemsPerResource);

		if (Spend(ResourceType.GEMS, gems))
		{
			Collect(otherResource, resources);

			ExchangeGemsForResourcesRequestProto request = new ExchangeGemsForResourcesRequestProto();
			request.sender = MSWhiteboard.localMupWithResources;
			request.numGems = gems;
			request.numResources = resources;
			request.resourceType = otherResource;
			request.clientTime = MSUtil.timeNowMillis;

			UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_EXCHANGE_GEMS_FOR_RESOURCES_EVENT, DealWithSpendGemsForOtherResourceResponse);

			if (action != null)
			{
				Debug.Log("Triggered action");
				action();
			}
		}
		else
		{
			return -1;
		}

		return gems;
	}

	void DealWithSpendGemsForOtherResourceResponse(int tagNum)
	{
		ExchangeGemsForResourcesResponseProto response = UMQNetworkManager.responseDict[tagNum] as ExchangeGemsForResourcesResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != ExchangeGemsForResourcesResponseProto.ExchangeGemsForResourcesStatus.SUCCESS)
		{
			Debug.LogError("Problem exchanging gems for other resource: " + response.status.ToString());
		}
	}
	
	public void GainExp(int amount)
	{
		_exp += amount;
		if (MSWhiteboard.nextLevelInfo != null && _exp > expForNextLevel)
		{
			StartCoroutine(LevelUp());
		}
	}

	[ContextMenu("check exp")]
	public void CheckEXP()
	{
		if (MSWhiteboard.nextLevelInfo != null && _exp > expForNextLevel && !checkingEXP)
		{
			Debug.LogWarning("leveling up: " + _exp + " > " + expForNextLevel);
			checkingEXP = true;
			StartCoroutine(LevelUp());
		}
	}

	IEnumerator LevelUp()
	{
		LevelUpRequestProto request = new LevelUpRequestProto();
		request.nextLevel = _level + 1;
		request.sender = MSWhiteboard.localMup;
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_LEVEL_UP_EVENT, null);
		
		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		LevelUpResponseProto response = UMQNetworkManager.responseDict[tagNum] as LevelUpResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status == LevelUpResponseProto.LevelUpStatus.SUCCESS)
		{
			_level++;
			levelUpPopup.ActivateLevelUpScreen(_level);

			StaticUserLevelInfoProto nextLevelData = MSDataManager.instance.Get<StaticUserLevelInfoProto>(_level + 1);
			MSWhiteboard.localUser.level = _level;

			if (MSActionManager.UI.OnLevelUp != null)
			{
				MSActionManager.UI.OnLevelUp();
			}
		}
		else
		{
			Debug.LogError("Problem leveling up: " + response.status.ToString());
		}
		checkingEXP = false;
	}

	void HandleRetrieveResponse(int tagNum)
	{

	}

	public void CheatMoney(int cash, int oil, int gems, string reason)
	{
		UpdateUserCurrencyRequestProto request = new UpdateUserCurrencyRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.reason = reason;
		request.cashSpent = cash;
		request.oilSpent = oil;
		request.gemsSpent = gems;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UPDATE_USER_CURRENCY_EVENT, DealWithCheatResponse);

		Collect(ResourceType.CASH, cash);
		Collect(ResourceType.OIL, oil);
		Collect(ResourceType.GEMS, gems);
	}

	public void CheatReset()
	{
		DevRequestProto request = new DevRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.devRequest = DevRequest.RESET_ACCOUNT;

		PlayerPrefs.SetString("CleanStart", "Yeah");
		PlayerPrefs.Save();

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_DEV_EVENT, DealWithDevResponse);
	}	

	void DealWithDevResponse(int tagNum)
	{
		DevResponseProto response = UMQNetworkManager.responseDict[tagNum] as DevResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != DevResponseProto.DevStatus.SUCCESS)
		{
			Debug.LogError("Problem dev cheating: " + response.status.ToString());
		}
	}

	void DealWithCheatResponse(int tagNum)
	{
		UpdateUserCurrencyResponseProto response = UMQNetworkManager.responseDict[tagNum] as UpdateUserCurrencyResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != UpdateUserCurrencyResponseProto.UpdateUserCurrencyStatus.SUCCESS)
		{
			Debug.LogError("Problem cheating: " + response.status.ToString());
		}
	}


	void OnApplicationPause(bool pauseStatus)
	{
		RunCollectResources();
	}

	void OnApplicationQuit()
	{
		RunCollectResources();
	}

	public Coroutine RunCollectResources()
	{
		return StartCoroutine(CollectResources());
	}

	IEnumerator CollectResources()
	{
		if (retrieveRequest == null || retrieveRequest.structRetrievals.Count == 0)
		{
			yield break;
		}

		int tagNum = UMQNetworkManager.instance.SendRequest(retrieveRequest, (int)EventProtocolRequest.C_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT);
		retrieveRequest = null;

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		RetrieveCurrencyFromNormStructureResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetrieveCurrencyFromNormStructureResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != RetrieveCurrencyFromNormStructureResponseProto.RetrieveCurrencyFromNormStructureStatus.SUCCESS)
		{
			MSSceneManager.instance.ReconnectPopup();
		}
	}

}
