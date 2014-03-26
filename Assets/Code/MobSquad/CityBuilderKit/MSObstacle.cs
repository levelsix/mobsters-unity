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

	public int secsLeft
	{
		get
		{
			if (endTime == 0)
			{
				return 0;
			}
			return (int)((endTime - MSUtil.timeNowMillis) / 1000);
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
		FinishRemove(CBKMath.GemsForTime(endTime - MSUtil.timeNowMillis));
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
