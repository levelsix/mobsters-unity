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
	public static int[] resources = {0, 0};
	
	int _level = 0;
	
	int level{get{return _level;}}
	
	int _exp = 0;
	
	int exp{get{return _exp;}}
	
	int _expForNextLevel = 0;
	
	int expForNextLevel{get{return _expForNextLevel;}}
	
	public enum ResourceType { FREE, PREMIUM };
	
	void Awake()
	{
		instance = this;
	}
	
	public void Init(int lev, int xp, int xpNext, int free, int premium)
	{
		_level = lev;
		_exp = xp;
		_expForNextLevel = xpNext;
		
		resources[0] = free;
		resources[1] = premium;
		if (CBKEventManager.UI.OnChangeResource[(int)ResourceType.FREE] != null)
		{
			CBKEventManager.UI.OnChangeResource[(int)ResourceType.FREE](resources[(int)ResourceType.FREE]);
		}
		if (CBKEventManager.UI.OnChangeResource[(int)ResourceType.PREMIUM] != null)
		{
			CBKEventManager.UI.OnChangeResource[(int)ResourceType.PREMIUM](resources[(int)ResourceType.PREMIUM]);
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
		resources[(int)resource] += amount;
		
		if (CBKEventManager.UI.OnChangeResource[(int)resource] != null)
		{
			CBKEventManager.UI.OnChangeResource[(int)resource](resources[(int)resource]);
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
		if (resources[(int)resource] > amount)
		{
			resources[(int)resource] -= amount;
			if (CBKEventManager.UI.OnChangeResource[(int)resource] != null)
			{
				CBKEventManager.UI.OnChangeResource[(int)resource](resources[(int)resource]);
			}
			return true;
		}
		return false;
	}
	
	public void GainExp(int amount)
	{
		_exp += amount;
		if (_exp > _expForNextLevel)
		{
			
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
			//_expForNextLevel = response.experienceRequiredForNewNextLevel;
		}
		else
		{
			Debug.LogError("Problem leveling up: " + response.status.ToString());
		}
		
		
	}
	
}
