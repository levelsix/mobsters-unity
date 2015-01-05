﻿using UnityEngine;
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

	public bool finishedWithGems = false;

	public bool isRemoving;

	public bool finished = false;

	MSTimer removeTimer;

	public int gemsToFinish
	{
		get
		{
			return MSMath.GemsForTime(removeTimer.timeLeft, false);
		}
	}

	public long millisLeft
	{
		get
		{
			return removeTimer.timeLeft;
		}
	}

	public int secsLeft
	{
		get
		{
			if (removeTimer == null)
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

	public void Init(MinimumObstacleProto proto)
	{
		obstacle = MSDataManager.instance.Get<ObstacleProto>(proto.obstacleId);
		removeTimer = null;
	}

	public void Init(UserObstacleProto proto)
	{
		this.userObstacle = proto;
		obstacle = MSDataManager.instance.Get<ObstacleProto>(proto.obstacleId);
		removeTimer = null;
		if (proto.removalStartTime > 0)
		{
			removeTimer = new MSTimer(GameActionType.REMOVE_OBSTACLE, proto.userObstacleUuid, obstacle.obstacleId, proto.removalStartTime, obstacle.secondsToRemove * 1000);
			StartCoroutine(Check ());
		}
	}

	public void StartRemove(bool usingGems = false)
	{
		if (MSBuildingManager.instance.currentUnderConstruction != null)
		{
			MSPopupManager.instance.CreatePopup("Your builder is busy!",
    			"Speed him up for (G) " + 
	                MSMath.GemsForTime(MSBuildingManager.instance.currentUnderConstruction.timeLeft, true)
	                + " and remove this obstacle?",
                new string[]{"Cancel", "Speed Up"},
				new string[]{"greymenuoption", "purplemenuoption"},
				new Action[]{MSActionManager.Popup.CloseTopPopupLayer,
					delegate
					{
						MSActionManager.Popup.CloseTopPopupLayer();
						MSBuildingManager.instance.currentUnderConstruction.CompleteWithGems();
						StartRemove();
					}
				},
				"purple"
			);
			return;
		}
		
		BeginObstacleRemovalRequestProto request = new BeginObstacleRemovalRequestProto();

		if (usingGems)
		{
			int gemsNeeded = Mathf.CeilToInt((obstacle.cost - MSResourceManager.resources[obstacle.removalCostType]) * MSWhiteboard.constants.gemsPerResource);
			if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemsNeeded))
			{
				MSResourceManager.instance.SpendAll(obstacle.removalCostType);
				request.resourceChange = -MSResourceManager.instance.SpendAll(obstacle.removalCostType);
				request.gemsSpent = Mathf.CeilToInt((obstacle.cost + request.resourceChange) * MSWhiteboard.constants.gemsPerResource);
			}
			else
			{
				return;
			}
		}
		else if (MSResourceManager.instance.Spend(obstacle.removalCostType, obstacle.cost, delegate{StartRemove(true);}))
		{
			request.resourceChange = -obstacle.cost;
			request.gemsSpent = 0;
		}
		else
		{
			return;
		}

		removeTimer = new MSTimer(GameActionType.REMOVE_OBSTACLE, userObstacle.userObstacleUuid, obstacle.obstacleId, MSUtil.timeNowMillis, obstacle.secondsToRemove * 1000);
		StartCoroutine(Check ());
		
		request.sender = MSWhiteboard.localMup;
		request.curTime = MSUtil.timeNowMillis;
		request.resourceType = obstacle.removalCostType;
		request.userObstacleUuid = userObstacle.userObstacleUuid;
		
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
			if (removeTimer.done && !finishedWithGems)
			{
				FinishRemove();
				yield break;
			}
			yield return null;
		}
	}

	public void FinishWithGems()
	{
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemsToFinish))
		{
			finishedWithGems = true;
			FinishRemove(gemsToFinish, true);
		}
	}

	void FinishRemove(int gems = 0, bool spentGems = false)
	{
		if (MSActionManager.Town.OnObstacleRemoved != null)
		{
			MSActionManager.Town.OnObstacleRemoved(this);
		}

		ObstacleRemovalCompleteRequestProto request = new ObstacleRemovalCompleteRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.curTime = MSUtil.timeNowMillis;
		request.speedUp = spentGems;
		request.gemsSpent = gems;
		request.userObstacleUuid = userObstacle.userObstacleUuid;
		request.atMaxObstacles = (MSWhiteboard.constants.maxObstacles == MSBuildingManager.instance.obstacles.Count);

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_OBSTACLE_REMOVAL_COMPLETE_EVENT, DealWithFinishRemovalResponse);

		MSBuildingManager.instance.currentUnderConstruction = null;

		//turning finished to false true the progress bar to disapear
		finished = true;
	}
	
	public IEnumerator EndingAnimation()
	{
		MSBuildingManager.instance.FullDeselect();
		MSBuilding building = GetComponent<MSBuilding>();
		if(building.spinner != null)
		{
			StartCoroutine(ManualAlphaTween(1f, building.shadow));
			StartCoroutine(ManualAlphaTween(1f, building.floor));
			yield return StartCoroutine(ManualAlphaTween(1f, building.sprite));
//			yield return StartCoroutine(building.spinner.PlayAnimations()); //wups looks like the spinner isn't supposed to appear
		}
		else
		{
			yield return new WaitForSeconds(1f);
		}

		if (building.selected)
		{
			building.Deselect();
			MSBuildingManager.instance.FullDeselect();
		}
		building.sprite.color = new Color(building.sprite.color.r, building.sprite.color.g, building.sprite.color.b, 1f);
		isRemoving = false;
		building.Pool();
	}

	IEnumerator ManualAlphaTween(float duration, SpriteRenderer sprite)
	{
		float curTime = 0;
		while(curTime < duration)
		{
			float alpha = (duration - curTime) / duration;
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
			yield return null;
			curTime += Time.deltaTime;
		}
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
