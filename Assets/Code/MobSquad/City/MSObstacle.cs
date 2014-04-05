using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSObstacle
/// </summary>
[RequireComponent (typeof (MSBuilding))]
public class MSObstacle : MonoBehaviour {

	long endTime = 0;

	public bool isRemoving;

	public long millisLeft
	{
		get
		{
			if (endTime == 0)
			{
				return 0;
			}
			return endTime - MSUtil.timeNowMillis;
		}
	}

	public int secsLeft
	{
		get
		{
			if (endTime == 0)
			{
				return 0;
			}
			return (int)(millisLeft / 1000);
		}
	}

	public float progress
	{
		get
		{
			return 1f - ((float)millisLeft / (obstacle.secondsToRemove * 1000f));
		}
	}

	public UserObstacleProto userObstacle;

	public ObstacleProto obstacle;

	public void Init(UserObstacleProto proto)
	{
		this.userObstacle = proto;
		obstacle = MSDataManager.instance.Get<ObstacleProto>(proto.obstacleId);
		if (proto.removalStartTime > 0)
		{
			endTime = proto.removalStartTime + obstacle.secondsToRemove * 1000;
			StartCoroutine(Check ());
		}
	}

	public void StartRemove()
	{
		if (MSBuildingManager.instance.currentUnderConstruction != null)
		{
			MSActionManager.Popup.CreateButtonPopup("Your builder is busy! Speed him up for " + 
			                                        MSMath.GemsForTime(MSBuildingManager.instance.currentUnderConstruction.completeTime)
			                                        + "gems and remove this obstacle?",
			                                        new string[]{"Cancel", "Speed Up"},
			new Action[]{MSActionManager.Popup.CloseTopPopupLayer,
				delegate
				{
					MSActionManager.Popup.CloseTopPopupLayer();
					MSBuildingManager.instance.currentUnderConstruction.CompleteWithGems();
					StartRemove();
				}
			}
			);
			return;
		}

		endTime = MSUtil.timeNowMillis + obstacle.secondsToRemove * 1000;
		Debug.Log("Start remove");
		StartCoroutine(Check ());

		BeginObstacleRemovalRequestProto request = new BeginObstacleRemovalRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.curTime = MSUtil.timeNowMillis;
		request.resourceChange = -obstacle.cost;
		request.resourceType = obstacle.removalCostType;
		request.userObstacleId = userObstacle.userObstacleId;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_OBSTACLE_REMOVAL_EVENT, DealWithBeginRemovalResponse);
	}

	void DealWithBeginRemovalResponse(int tagNum)
	{
		BeginObstacleRemovalResponseProto response = UMQNetworkManager.responseDict[tagNum] as BeginObstacleRemovalResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != BeginObstacleRemovalResponseProto.BeginObstacleRemovalStatus.SUCCESS)
		{
			Debug.LogError("Problem begining obstacle removal: " + response.status.ToString());
		}
	}

	IEnumerator Check()
	{
		isRemoving = true;
		MSBuildingManager.instance.currentUnderConstruction = GetComponent<MSBuilding>();
		while (true)
		{
			if (MSUtil.timeNowMillis > endTime)
			{
				FinishRemove();
				yield break;
			}
			yield return null;
		}
	}

	public void FinishWithGems()
	{
		FinishRemove(MSMath.GemsForTime(endTime - MSUtil.timeNowMillis));
	}

	void FinishRemove(int gems = 0)
	{
		ObstacleRemovalCompleteRequestProto request = new ObstacleRemovalCompleteRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.curTime = MSUtil.timeNowMillis;
		request.speedUp = gems > 0;
		request.gemsSpent = gems;
		request.userObstacleId = userObstacle.userObstacleId;
		request.atMaxObstacles = (MSWhiteboard.constants.maxObstacles == MSBuildingManager.instance.obstacles.Count);

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_OBSTACLE_REMOVAL_COMPLETE_EVENT, DealWithFinishRemovalResponse);

		MSBuildingManager.instance.currentUnderConstruction = null;
		MSBuilding building = GetComponent<MSBuilding>();
		if (building.selected)
		{
			building.Deselect();
		}
		building.Pool();
	}

	void DealWithFinishRemovalResponse(int tagNum)
	{
		ObstacleRemovalCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as ObstacleRemovalCompleteResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != ObstacleRemovalCompleteResponseProto.ObstacleRemovalCompleteStatus.SUCCESS)
		{
			Debug.LogError("Problem finishing obstacle removal: " + response.status.ToString());
		}
	}
}
