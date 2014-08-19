using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Component of a building that mananges upgrading,
/// including timing and cost.
/// </summary>
[RequireComponent (typeof (MSBuilding))]
public class MSBuildingUpgrade : MonoBehaviour {
	
	
	/// <summary>
	/// Quick getter and setter that wraps the FullUserStructureProto
	/// in the Building component and makes code here more readable.
	/// </summary>
	public int level
	{
		get
		{
			return building.combinedProto.structInfo.level;
		}
	}

	public bool isComplete
	{
		get
		{
			if (building.userStructProto == null)
			{
				return true;
			}
			return building.userStructProto.isComplete;
		}
	}
	
	/// <summary>
	/// Gets the time at which this building will be completed building or upgrading
	/// </summary>
	/// <value>
	/// The timestamp for when this building will be or was
	/// finished with it's last upgrade task
	/// </value>
	public long upgradeCompleteTime
	{
		get
		{
			if (building.userStructProto == null)
			{
				return 0;
			}
			return building.userStructProto.purchaseTime + building.combinedProto.structInfo.minutesToBuild * 60000;
		}
	}
	
	public long timeRemaining
	{
		get
		{
			return upgradeCompleteTime - MSUtil.timeNowMillis;
		}
	}
	
	public string timeLeftString
	{
		get
		{
			return MSUtil.TimeStringShort(timeRemaining);
		}
	}

	public float progress
	{
		get
		{
			float remaining = (float)timeRemaining;
			float total = building.combinedProto.structInfo.minutesToBuild * 60000f;
			float prog = remaining / total;
			//Debug.LogWarning("Remaining: " + remaining + ", Total: " + total + ", Progress: " + prog);
			return 1f - prog;//(((float)(timeRemaining)) / (building.combinedProto.structInfo.minutesToBuild * 60000));
		}
	}
	
    /// <summary>
    /// The building component.
    /// </summary>
	protected MSBuilding building;
	
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
	public int gemsToFinish
	{
		get
		{
			return Mathf.CeilToInt((timeRemaining / 60000f / MSWhiteboard.constants.minutesPerGem));
		}
	}

	[SerializeField]
	public MSBuildingProgressBar progressBar;
	
	/// <summary>
	/// Awake this instance.
	/// Set up internal component references
	/// </summary>
	public void Awake()
	{
		building = GetComponent<MSBuilding>();
	}

	void OnEnable()
	{
		OnFinishUpgrade += delegate {building.SetupSprite(building.combinedProto.structInfo.imgName);};
	}
	
	public void Init(StructureInfoProto sProto, FullUserStructureProto uProto)
    {
		if (!uProto.isComplete && uProto.purchaseTime > 0)
		{
			Debug.Log("Building " + uProto.userStructId + " isn't finished; checking upgrade");
			building.SetupConstructionSprite();
			StartCoroutine(CheckUpgrade());
		}
		
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
		StartBuild();
	}
	
	/// <summary>
	/// Spends the cash and starts the upgrade timer
	/// </summary>
	public virtual void StartUpgrade(int baseResource, int gems = 0)
	{
		building.combinedProto = building.combinedProto.successor;

		SendUpgradeRequest(baseResource, gems);

		MSBuildingManager.instance.RemoveFromFunctionalityLists(building);

		StartBuild();
	}

	public virtual void StartBuild()
	{
		building.userStructProto.isComplete = false;
		building.userStructProto.purchaseTime = MSUtil.timeNowMillis;
		building.SetupConstructionSprite();
		
		StartCoroutine(CheckUpgrade());
	}
	
	void SendUpgradeRequest(int baseResource, int gems = 0)
	{
		UpgradeNormStructureRequestProto request = new UpgradeNormStructureRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.userStructId = building.userStructProto.userStructId;
		request.timeOfUpgrade = MSUtil.timeNowMillis;
		request.resourceType = building.combinedProto.structInfo.buildResourceType;
		request.resourceChange = -baseResource;
		request.gemsSpent = gems;
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
			return building.combinedProto.structInfo.minutesToBuild * 60 * 1000;
		}
		else
		{
			return MSMath.TimeToBuildOrUpgradeStruct(building.combinedProto.structInfo.minutesToBuild, level);
		}
	}
	
	/// <summary>
	/// Checks if the building has past the upgrade timer
	/// </summary>
	public IEnumerator CheckUpgrade()
	{
		MSBuildingManager.instance.currentUnderConstruction = building;
		yield return null;
		while (!building.userStructProto.isComplete)
		{
			if (MSUtil.timeNowMillis > building.userStructProto.purchaseTime + building.combinedProto.structInfo.minutesToBuild * 60 * 1000
				&& MSUtil.timeNowMillis > upgradeCompleteTime)
			{
				FinishWithWait();
			}
			
			if (building.OnUpdateValues != null)
			{
				building.OnUpdateValues();
			}
			yield return new WaitForSeconds(1f);
		}
	}
	
	/// <summary>
	/// Finishes the upgrade with gems.
	/// </summary>
	public void FinishWithPremium()
	{
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemsToFinish))
		{
			SendPremiumFinishRequest();
			FinishUpgrade();
		}
	}
	
	void SendPremiumFinishRequest()
	{
		FinishNormStructWaittimeWithDiamondsRequestProto request = new FinishNormStructWaittimeWithDiamondsRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.userStructId = building.userStructProto.userStructId;
		request.timeOfSpeedup = MSUtil.timeNowMillis;
		request.gemCostToSpeedup = gemsToFinish;
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
		request.sender = MSWhiteboard.localMup;
		request.userStructId.Add(building.userStructProto.userStructId);
		request.curTime = MSUtil.timeNowMillis;
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
		MSBuildingManager.instance.currentUnderConstruction = null;

		building.userStructProto.isComplete = true;

		MSActionManager.Quest.OnStructureUpgraded(building.userStructProto.structId);

		MSBuildingManager.instance.AddToFunctionalityLists(building);

		if (building.selected)
		{
			MSBuildingManager.instance.SetSelectedBuilding(building);
		}

		if (building.upgrade.OnFinishUpgrade != null)
		{
			building.upgrade.OnFinishUpgrade();
		}

		if (building.GetComponent<MSBuildingFrame> () != null) {
			building.GetComponent<MSBuildingFrame>().CheckTag();
		}
	}
}
