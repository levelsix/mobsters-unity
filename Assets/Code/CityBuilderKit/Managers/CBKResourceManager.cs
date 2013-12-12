using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Keeps track of the local player's resources and experience level
/// </summary>
public class CBKResourceManager : MonoBehaviour {
	
	public static CBKResourceManager instance;
	
	/// <summary>
	/// The resources.
	/// Indexed using the AOC2Values.Buildings.Resources enum
	/// </summary>
	public static int[] resources = {0, 0, 0};
	
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
			if (CBKEventManager.UI.OnChangeResource[i] != null)
			{
				CBKEventManager.UI.OnChangeResource[i](resources[i]);
			}
		}
	}

	public void CollectFromBuilding(ResourceType resource, int amount, int userStructId)
	{
		Collect(resource, amount);

		if (retrieveRequest == null)
		{
			retrieveRequest = new RetrieveCurrencyFromNormStructureRequestProto();
			retrieveRequest.sender = CBKWhiteboard.localMup;
		}

		RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval structRetrieval = new RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval();
		structRetrieval.amountCollected = amount;
		structRetrieval.userStructId = userStructId;
		structRetrieval.timeOfRetrieval = CBKUtil.timeNowMillis;

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
		resources[(int)resource-1] += amount;
		
		if (CBKEventManager.UI.OnChangeResource[(int)resource-1] != null)
		{
			CBKEventManager.UI.OnChangeResource[(int)resource-1](resources[(int)resource-1]);
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
	public bool Spend(ResourceType resource, int amount)
	{
		if (resources[(int)resource-1] > amount)
		{
			resources[(int)resource-1] -= amount;
			if (CBKEventManager.UI.OnChangeResource[(int)resource-1] != null)
			{
				CBKEventManager.UI.OnChangeResource[(int)resource-1](resources[(int)resource-1]);
			}
			return true;
		}
		Debug.LogWarning("Tried to spend " + amount + " " + resource.ToString() + ", only have " + resources[(int)resource-1].ToString());
		return false;
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
		request.sender = CBKWhiteboard.localMup;
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
			StaticUserLevelInfoProto nextLevelData = CBKDataManager.instance.Get(typeof(StaticUserLevelInfoProto), _level + 1) as StaticUserLevelInfoProto;
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
