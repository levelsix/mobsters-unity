using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Component of a building that mananges upgrading,
/// including timing and cost.
/// </summary>
[RequireComponent (typeof (CBKBuilding))]
public class CBKBuildingUpgrade : MonoBehaviour {
	
	/// <summary>
	/// Quick getter and setter that wraps the FullUserStructureProto
	/// in the Building component and makes code here more readable.
	/// </summary>
	public int level
	{
		get
		{
			return building.userStructProto.level;
		}
		set
		{
			building.userStructProto.level = value;
		}
	}
	
	/// <summary>
	/// Gets or sets the upgrade complete time in the FullUserStructureProto
	/// in the Building component
	/// </summary>
	/// <value>
	/// The timestamp for when this building will be or was
	/// finished with it's last upgrade task
	/// </value>
	public long upgradeCompleteTime
	{
		get
		{
			if (building.userStructProto.lastUpgradeTime > 0)
			{
				return building.userStructProto.lastUpgradeTime + CBKMath.TimeToBuildOrUpgradeStruct(building.structProto.minutesToBuild, building.userStructProto.level);
			}
			else
			{
				return building.userStructProto.purchaseTime + CBKMath.TimeToBuildOrUpgradeStruct(building.structProto.minutesToBuild, 0);
			}
		}
	}
	
	public long timeRemaining
	{
		get
		{
			return upgradeCompleteTime - CBKUtil.timeNow;
		}
	}
	
	public string timeLeftString
	{
		get
		{
			return CBKUtil.TimeStringShort(timeRemaining);
		}
	}
	
	public bool lookAtMeMom = false;
	
	/// <summary>
	/// The UI Popup
	/// </summary>
	[SerializeField]
	GameObject upgradePopup;
	
    /// <summary>
    /// The building component.
    /// </summary>
	protected CBKBuilding building;
	
    /// <summary>
    /// The on start upgrade event.
    /// </summary>
    public Action OnStartUpgrade;
    
    /// <summary>
    /// The on upgrade complete event.
    /// </summary>
	public Action OnFinishUpgrade;
	
	/// <summary>
	/// Gets the gems to finish this upgrade immediately
	/// </summary>
	/// <value>
	/// The gems to finish.
	/// </value>
	public int gemsToFinish{
		get
		{
			return 10;//AOC2Math.GemsForTime(finishUpgradeTime - AOC2Math.UnixTimeStamp(DateTime.UtcNow));
		}
	}
	
	/// <summary>
	/// Awake this instance.
	/// Set up internal component references
	/// </summary>
	public void Awake()
	{
		building = GetComponent<CBKBuilding>();
	}
	
    public void Init(FullStructureProto sProto, FullUserStructureProto uProto)
    {
		if (!uProto.isComplete)
		{
			Debug.Log("Building " + uProto.userStructId + " isn't finished; checking upgrade");
			building.SetupConstructionSprite();
			StartCoroutine(CheckUpgrade());
		}
		lookAtMeMom = uProto.isComplete;
		
		Init (true);
    }
	
	public void Init(bool operational)
	{
		enabled = operational;
	}
	
	void OnSelect()
	{
		//upgradePopup.SetActive(true);
	}
	
	void OnDeselect()
	{
		//upgradePopup.SetActive(false);
	}
	
	public virtual void StartConstruction()
	{
		level = 0;
		StartUpgrade();
	}
	
	/// <summary>
	/// Spends the cash and starts the upgrade timer
	/// </summary>
	public virtual void StartUpgrade()
	{
		building.userStructProto.lastUpgradeTime = CBKUtil.timeNow;
		
		building.userStructProto.isComplete = false;
		
		building.SetupConstructionSprite();
		
		SendUpgradeRequest();
		
		StartCoroutine(CheckUpgrade());
	}
	
	void SendUpgradeRequest()
	{
		UpgradeNormStructureRequestProto request = new UpgradeNormStructureRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.userStructId = building.userStructProto.userStructId;
		request.timeOfUpgrade = CBKUtil.timeNow;
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UPGRADE_NORM_STRUCTURE_EVENT, CheckUpgradeResponse);
	}
			
	void CheckUpgradeResponse(int tagNum)
	{
		UpgradeNormStructureResponseProto response = (UpgradeNormStructureResponseProto)UMQNetworkManager.responseDict[tagNum];	
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != UpgradeNormStructureResponseProto.UpgradeNormStructureStatus.SUCCESS)
		{
			Debug.LogError("Problem certifying upgrade: " + response.status);
			//TODO: Rectify discrepencies
		}
	}
	
	/// <summary>
	/// Calculates how much time it will take for this building to upgrade
	/// when at the given level
	/// </summary>
	/// <returns>
	/// The time it will take to upgrade
	/// </returns>
	/// <param name='level'>
	/// Current level of the building
	/// </param>
	public long TimeToUpgrade(int level)
	{
		if (level == 1)
		{
			return building.structProto.minutesToBuild * 60 * 1000;
		}
		else
		{
			return CBKMath.TimeToBuildOrUpgradeStruct(building.structProto.minutesToBuild, level);
		}
	}
	
	/// <summary>
	/// Checks if the building has past the upgrade timer
	/// </summary>
	public IEnumerator CheckUpgrade()
	{
		Debug.Log("Checking...");
		while (!building.userStructProto.isComplete)
		{
			if (CBKUtil.timeNow > building.userStructProto.purchaseTime + building.structProto.minutesToBuild * 60 * 1000
				&& CBKUtil.timeNow > upgradeCompleteTime)
			{
				FinishWithWait();
			}
			
			if (building.OnUpdateValues != null)
			{
				building.OnUpdateValues();
			}
			//TODO: Send update to UI if selected
			yield return new WaitForSeconds(1f);
		}
	}
	
	/// <summary>
	/// Finishes the upgrade with gems.
	/// </summary>
	public void FinishWithPremium()
	{
		building.userStructProto.lastUpgradeTime = CBKUtil.timeNow;
		SendPremiumFinishRequest();
		FinishUpgrade();
	}
	
	void SendPremiumFinishRequest()
	{
		FinishNormStructWaittimeWithDiamondsRequestProto request = new FinishNormStructWaittimeWithDiamondsRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.userStructId = building.userStructProto.userStructId;
		request.timeOfSpeedup = CBKUtil.timeNow;
		UMQNetworkManager.instance.SendRequest(request, 
			(int)EventProtocolRequest.C_FINISH_NORM_STRUCT_WAITTIME_WITH_DIAMONDS_EVENT, LoadPremiumFinishResponse);
	}
	
	void LoadPremiumFinishResponse(int tagNum)
	{
		FinishNormStructWaittimeWithDiamondsResponseProto response = UMQNetworkManager.responseDict[tagNum] as FinishNormStructWaittimeWithDiamondsResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != FinishNormStructWaittimeWithDiamondsResponseProto.FinishNormStructWaittimeStatus.SUCCESS)
		{
			Debug.LogError("Problem finishing construction with diamonds: " + response.status.ToString());
		}
	}
	
	void SendWaitFinishRequest()
	{
		NormStructWaitCompleteRequestProto request = new NormStructWaitCompleteRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.userStructId.Add(building.userStructProto.userStructId);
		request.curTime = CBKUtil.timeNow;
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_NORM_STRUCT_WAIT_COMPLETE_EVENT, LoadWaitFinishResponse);
	}
	
	void LoadWaitFinishResponse(int tagNum)
	{
		NormStructWaitCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as NormStructWaitCompleteResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status == NormStructWaitCompleteResponseProto.NormStructWaitCompleteStatus.SUCCESS)
		{
			building.userStructProto = response.userStruct[0];
		}
		else
		{
			Debug.LogError("Problem completing building from finish: " + response.status.ToString());
		}
	}
	
	public void FinishWithWait()
	{
		SendWaitFinishRequest();
		FinishUpgrade();
	}
	
	/// <summary>
	/// Finishes the upgrade.
	/// </summary>
	public virtual void FinishUpgrade()
	{
        
		building.userStructProto.isComplete = true;
		lookAtMeMom = true;
		
		building.SetupSprite(building.structProto.name);
		
		if (building.userStructProto.lastUpgradeTime > 0)
		{
			level++;
			CBKEventManager.Quest.OnStructureUpgraded(building.userStructProto.structId, level);
		}
		else
		{
			CBKEventManager.Quest.OnStructureBuilt(building.userStructProto.structId);
		}
			
        if (OnFinishUpgrade != null)
        {
            OnFinishUpgrade();  
        }
	}
}
