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

	MSTimer buildTimer;

	StartupResponseProto.StartupConstants.ClanHelpConstants upgradeConstants;
	ClanHelpProto currActiveHelp = null;
	
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
			return buildTimer.finishTime;
			//return building.userStructProto.purchaseTime + building.combinedProto.structInfo.minutesToBuild * 60000; //Deprecated
		}
	}
	
	public long timeRemaining
	{
		get
		{
			if (buildTimer == null) {
				return 0;
			}
			return buildTimer.timeLeft;
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
			return buildTimer.progress;
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
			return MSMath.GemsForTime(timeRemaining, building.obstacle == null);
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
		OnFinishUpgrade += delegate {
			StartCoroutine( building.SetupSprite(building.combinedProto.successor.structInfo.imgName));
			building.sprite.GetComponent<Animator>().enabled = true;
			FinishUpgrade();
		};

		MSActionManager.Clan.OnGiveClanHelp += DealWithGiveClanHelp;
	}

	public void DealWithGiveClanHelp(GiveClanHelpResponseProto response, bool self)
	{
		if(currActiveHelp != null && !self)
		{
			foreach(ClanHelpProto help in response.clanHelps)
			{
				if(help.clanHelpUuid.Equals(currActiveHelp.clanHelpUuid))
				{
					currActiveHelp = null;
				}
			}
		}
	}
	
	public void Init(StructureInfoProto sProto, FullUserStructureProto uProto)
    {
		if (!uProto.isComplete && uProto.purchaseTime > 0)
		{
			buildTimer = new MSTimer(GameActionType.UPGRADE_STRUCT, uProto.userStructUuid, sProto.structId, uProto.purchaseTime, sProto.minutesToBuild * 1000L * 60);
			building.SetupConstructionSprite();
			StartCoroutine(CheckUpgrade());
		}
		
		Init (true);
    }
	
	public void Init(bool operational)
	{
		enabled = operational;
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

		StartCoroutine(SendUpgradeRequest(baseResource, gems));
	}

	public virtual void StartBuild()
	{
		building.userStructProto.isComplete = false;
		building.userStructProto.purchaseTime = MSUtil.timeNowMillis;
		building.SetupConstructionSprite();

		buildTimer = new MSTimer(GameActionType.UPGRADE_STRUCT,  building.userStructProto.userStructUuid, building.combinedProto.structInfo.structId, MSUtil.timeNowMillis, building.combinedProto.structInfo.minutesToBuild * 1000L * 60); 
		
		StartCoroutine(CheckUpgrade());
	}
	
	IEnumerator SendUpgradeRequest(int baseResource, int gems = 0)
	{
		yield return MSResourceManager.instance.RunCollectResources();
		UpgradeNormStructureRequestProto request = new UpgradeNormStructureRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.userStructUuid = building.userStructProto.userStructUuid;
		request.timeOfUpgrade = MSUtil.timeNowMillis;
		request.resourceType = building.combinedProto.structInfo.buildResourceType;
		request.resourceChange = -baseResource;
		request.gemsSpent = gems;
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UPGRADE_NORM_STRUCTURE_EVENT, ReceiveUpgradeResponse);
	}
			
	void ReceiveUpgradeResponse(int tagNum)
	{
		UpgradeNormStructureResponseProto response = (UpgradeNormStructureResponseProto)UMQNetworkManager.responseDict[tagNum];	
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status == UpgradeNormStructureResponseProto.UpgradeNormStructureStatus.SUCCESS)
		{
			MSBuildingManager.instance.RemoveFromFunctionalityLists(building);
			StartBuild();
			MSBuildingUpgradePopup.instance.UnlockAndClose();
		}
		else
		{
			MSBuildingUpgradePopup.instance.UnlockServerFail();
			Debug.LogError("Problem upgrading building: " + response.status.ToString());
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
			if (buildTimer.done)
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
			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.buildingFinishNow);
			SendPremiumFinishRequest();
			MSBuildingManager.instance.currentUnderConstruction = null;
			building.userStructProto.isComplete = true;
		}
	}
	
	void SendPremiumFinishRequest()
	{
		if (!MSTutorialManager.instance.inTutorial)
		{
			FinishNormStructWaittimeWithDiamondsRequestProto request = new FinishNormStructWaittimeWithDiamondsRequestProto();
			request.sender = MSWhiteboard.localMup;
			request.userStructUuid = building.id;
			request.timeOfSpeedup = MSUtil.timeNowMillis;
			request.gemCostToSpeedup = gemsToFinish;
			UMQNetworkManager.instance.SendRequest(request, 
				(int)EventProtocolRequest.C_FINISH_NORM_STRUCT_WAITTIME_WITH_DIAMONDS_EVENT, LoadPremiumFinishResponse);
		}
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
		request.userStructUuid.Add(building.userStructProto.userStructUuid);
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
		if (building.upgrade.OnFinishUpgrade != null)
		{
			building.upgrade.OnFinishUpgrade();
		}
	}
	
	/// <summary>
	/// Finishes the upgrade.
	/// </summary>
	public virtual void FinishUpgrade()
	{
		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.buildingComplete);

		MSBuildingManager.instance.currentUnderConstruction = null;

		building.userStructProto.isComplete = true;

		MSActionManager.Quest.OnStructureUpgraded(building);

		MSBuildingManager.instance.AddToFunctionalityLists(building);

		if (building.selected)
		{
			if (MSTutorialManager.instance.inTutorial)
			{
				MSBuildingManager.instance.FullDeselect();
			}
			else 
			{
				MSBuildingManager.instance.SetSelectedBuilding(building);
			}
		}

		if (building.GetComponent<MSBuildingFrame> () != null) {
			building.GetComponent<MSBuildingFrame>().CheckTag();
		}


	}
}
