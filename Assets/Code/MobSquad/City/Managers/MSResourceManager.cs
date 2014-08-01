using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Keeps track of the local player's resources and experience level
/// </summary>
using com.soomla.unity;
using System;


public class MSResourceManager : MonoBehaviour {
	
	public static MSResourceManager instance;
	
	/// <summary>
	/// The resources.
	/// Indexed using the AOC2Values.Buildings.Resources enum
	/// </summary>
	public static int[] resources = {0, 0, 0};

	public static int[] maxes = {0, 0, int.MaxValue};
	
	int _level = 0;
	
	int level{get{return _level;}}
	
	int _exp = 0;
	
	int exp{get{return _exp;}}
	
	int _expForNextLevel = 0;
	
	int expForNextLevel{get{return _expForNextLevel;}}

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
	
	public void Init(int lev, int xp, int xpNext, int cash, int oil, int premium)
	{
		_level = lev;
		_exp = xp;
		_expForNextLevel = xpNext;
		
		resources[0] = cash;
		resources[1] = oil;
		resources[2] = premium;

		for (int i = 0; i < resources.Length; i++) 
		{
			if (MSActionManager.UI.OnChangeResource[i] != null)
			{
				MSActionManager.UI.OnChangeResource[i](resources[i]);
			}
		}
	}

	public void DetermineResourceMaxima()
	{
		maxes[0] = maxes[1] = BASE_RESOURCE_AMOUNT;
		foreach (ResourceStorageProto item in MSBuildingManager.instance.GetAllStorages()) 
		{
			maxes[(int)item.resourceType-1] += item.capacity;
		}

		if (MSActionManager.UI.OnSetResourceMaxima != null)
		{
			MSActionManager.UI.OnSetResourceMaxima(maxes);
		}
	}

	public void CollectFromBuilding(ResourceType resource, int amount, int userStructId)
	{
		Collect(resource, amount);

		if (retrieveRequest == null)
		{
			retrieveRequest = new RetrieveCurrencyFromNormStructureRequestProto();
			retrieveRequest.sender = MSWhiteboard.localMupWithResources;
		}

		RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval structRetrieval = new RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval();
		structRetrieval.amountCollected = amount;
		structRetrieval.userStructId = userStructId;
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
			UMQNetworkManager.instance.SendRequest(retrieveRequest, (int)EventProtocolRequest.C_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT, HandleRetrieveResponse);
			retrieveRequest = null;
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
		resources[(int)resource-1] = Mathf.Min(resources[(int)resource-1] + amount, maxes[(int)resource-1]);
		
		if (MSActionManager.UI.OnChangeResource[(int)resource-1] != null)
		{
			MSActionManager.UI.OnChangeResource[(int)resource-1](resources[(int)resource-1]);
		}
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
		if (resources[(int)resource-1] >= amount)
		{
			resources[(int)resource-1] -= amount;
			if (MSActionManager.UI.OnChangeResource[(int)resource-1] != null)
			{
				MSActionManager.UI.OnChangeResource[(int)resource-1](resources[(int)resource-1]);
			}
			return true;
		}
		Debug.LogWarning("Tried to spend " + amount + " " + resource.ToString() + ", only have " + resources[(int)resource-1].ToString());

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
				}
			},
			"purple"
			);
		}
		else if (amount <= maxes[(int)resource-1]) //We don't want to let users use gem buys to overcome resource limits
		{
			//Prompt to convert gems to currency
			int resourceNeeded = amount - resources[(int)resource - 1];
			int gemsNeeded = Mathf.CeilToInt(resourceNeeded * MSWhiteboard.constants.gemsPerResource);

			MSPopupManager.instance.CreatePopup("Not enough resources!",
				"Spend (G)" + gemsNeeded + " to buy the remaining " + ((resource== ResourceType.CASH)?"$":"(O)") + resourceNeeded,
                new string[] {"No", "Yes"},
				new string[] {"greymenuoption", "purplemenuoption"},
				new Action[] { 
					MSActionManager.Popup.CloseTopPopupLayer,
					delegate{ MSActionManager.Popup.CloseTopPopupLayer(); SpendGemsForOtherResource(resource, resourceNeeded, action);}
				},
				"purple"
			);
		}
		else
		{
			//Popup that says so
			MSPopupManager.instance.CreatePopup("Not enough " + resource.ToString().ToLower());
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
		if (_exp > _expForNextLevel)
		{
			//StartCoroutine(LevelUp());
		}
	}

	IEnumerator LevelUp()
	{
		LevelUpRequestProto request = new LevelUpRequestProto();
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
			StaticUserLevelInfoProto nextLevelData = MSDataManager.instance.Get(typeof(StaticUserLevelInfoProto), _level + 1) as StaticUserLevelInfoProto;
			if (nextLevelData != null)
			{
				_expForNextLevel = nextLevelData.requiredExperience;
			}
		}
		else
		{
			Debug.LogError("Problem leveling up: " + response.status.ToString());
		}
		
		
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
		UpdateUserCurrencyRequestProto request = new UpdateUserCurrencyRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.reason = MSChatPopup.RESET_CHEAT;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UPDATE_USER_CURRENCY_EVENT, DealWithCheatResponse);
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
		if (retrieveRequest != null)
		{
			UMQNetworkManager.instance.SendRequest(retrieveRequest, (int)EventProtocolRequest.C_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT, HandleRetrieveResponse);
			retrieveRequest = null;
		}
	}

	void OnDestroy()
	{
		if (retrieveRequest != null)
		{
			UMQNetworkManager.instance.SendRequest(retrieveRequest, (int)EventProtocolRequest.C_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT, HandleRetrieveResponse);
			retrieveRequest = null;
		}
	}
	
}
