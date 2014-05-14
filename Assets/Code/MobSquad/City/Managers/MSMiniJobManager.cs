using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobManager
/// </summary>
public class MSMiniJobManager : MonoBehaviour {

	public static MSMiniJobManager instance;

	public bool initialized = false;

	public bool isRedeeming = false;

	MiniJobCenterProto currJobCenter;

	public List<UserMiniJobProto> userMiniJobs = new List<UserMiniJobProto>();

	UserMiniJobProto currActiveJob = null;

	public int gemsToFinish
	{
		get
		{
			return MSMath.GemsForTime(timeLeft);
		}
	}

	public long timeLeft
	{
		get
		{
			if (currActiveJob == null)
			{
				return 0;
			}
			return currActiveJob.timeStarted + currActiveJob.durationMinutes * 60 * 1000 - MSUtil.timeNowMillis;
		}
	}

	bool shouldComplete
	{
		get
		{
			return currActiveJob != null
				&& timeLeft <= 0
				&& currActiveJob.timeCompleted == 0;
		}
	}

	public bool isCompleted
	{
		get
		{
			return currActiveJob != null
				&& currActiveJob.timeCompleted > 0;
		}
	}

	#region Startup

	void Awake()
	{
		instance = this;
	}

	void OnStartup(StartupResponseProto response)
	{
		userMiniJobs = response.userMiniJobProtos;
	}

	public void Init(MiniJobCenterProto pier)
	{
		currJobCenter = pier;

		if (pier != null)
		{
			StartCoroutine(CheckJobStuff());
		}
	}

	IEnumerator CheckJobStuff()
	{
		while (true)
		{
			if (NeedsToSpawn())
			{
				SendSpawnRequest();
			}
			if (shouldComplete)
			{
				CompleteCurrentJobWithWait();
			}

			yield return new WaitForSeconds(30);
		}
	}

	#endregion

	#region Job Spawning

	bool NeedsToSpawn()
	{
		return currJobCenter.structInfo.level > 0 &&
			(MSUtil.timeNowMillis - MSWhiteboard.localUser.lastMiniJobSpawnedTime 
			 	> currJobCenter.hoursBetweenJobGeneration * 60 * 60 * 1000);
	}

	void SendSpawnRequest()
	{
		MSWhiteboard.localUser.lastMiniJobSpawnedTime = MSUtil.timeNowMillis;

		SpawnMiniJobRequestProto request = new SpawnMiniJobRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clientTime = MSUtil.timeNowMillis;
		request.numToSpawn = currJobCenter.generatedJobLimit - userMiniJobs.Count;
		request.structId = currJobCenter.structInfo.structId;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_SPAWN_MINI_JOB_EVENT, DealWithSpawnResponse);
	}

	void DealWithSpawnResponse(int tagNum)
	{
		SpawnMiniJobResponseProto response = UMQNetworkManager.responseDict[tagNum] as SpawnMiniJobResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == SpawnMiniJobResponseProto.SpawnMiniJobStatus.SUCCESS)
		{
			foreach (var job in response.miniJobs)
			{
				userMiniJobs.Add(job);
			}
		}
		else
		{
			Debug.LogError("Problem spawning minijobs: " + response.status.ToString());
		}
	}

	#endregion

	#region Beginning Job

	void BeginJob(UserMiniJobProto job, List<PZMonster> monsters)
	{
		if (currActiveJob != null)
		{
			Debug.LogError("Should not be trying to start a job when there is an active one");
			return;
		}

		currActiveJob = job;

		BeginMiniJobRequestProto request = new BeginMiniJobRequestProto();
		request.sender = MSWhiteboard.localMup;

		request.clientTime = MSUtil.timeNowMillis;
		job.timeStarted = MSUtil.timeNowMillis;

		foreach (var item in monsters) 
		{
			request.userMonsterIds.Add(item.userMonster.userMonsterId);
			job.userMonsterIds.Add (item.userMonster.userMonsterId);
		}

		request.userMiniJobId = job.userMiniJobId;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_MINI_JOB_EVENT, 
		                                       DealWithJobBegin);

	}

	void DealWithJobBegin(int tagNum)
	{
		BeginMiniJobResponseProto response = UMQNetworkManager.responseDict[tagNum] as BeginMiniJobResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != BeginMiniJobResponseProto.BeginMiniJobStatus.SUCCESS)
		{
			Debug.LogError("Problem beginning minijob: " + response.status.ToString());
		}
	}	                                   

	#endregion

	#region Completing Job

	public void CompleteCurrentJobWithGems()
	{
		int numGems = MSMath.GemsForTime(timeLeft);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, numGems))
		{
			CompleteCurrentJob(true, numGems);
		}
	}

	void CompleteCurrentJobWithWait()
	{
		if (!shouldComplete)
		{
			Debug.LogError("Don't try to complete a job that isn't actually complete. Dumbass.");
			return;
		}

		CompleteCurrentJob(false);
	}

	void CompleteCurrentJob(bool speedUp, int gems = 0)
	{	
		currActiveJob.timeCompleted = MSUtil.timeNowMillis;
		
		CompleteMiniJobRequestProto request = new CompleteMiniJobRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clientTime = MSUtil.timeNowMillis;
		request.userMiniJobId = currActiveJob.miniJob.miniJobId;
		request.isSpeedUp = speedUp;
		request.gemCost = gems;

		//TODO: Damage monsters!

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_COMPLETE_MINI_JOB_EVENT,
		                                       DealWithJobCompleteResponse);
	}

	void DealWithJobCompleteResponse(int tagNum)
	{
		CompleteMiniJobResponseProto response = UMQNetworkManager.responseDict[tagNum] as CompleteMiniJobResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != CompleteMiniJobResponseProto.CompleteMiniJobStatus.SUCCESS)
		{
			Debug.Log("Problem completing job: " + response.status.ToString());
		}
	}

	#endregion

	#region Job Redeeming

	public void RedeemCurrJob()
	{
		if (!isCompleted)
		{
			Debug.LogError("Job hasn't been completed, cannot redeem.");
			return;
		}

		StartCoroutine(RunJobRedemption());
	}

	IEnumerator RunJobRedemption()
	{
		RedeemMiniJobRequestProto request = new RedeemMiniJobRequestProto();
		request.sender = MSWhiteboard.localMupWithResources;
		request.clientTime = MSUtil.timeNowMillis;
		request.userMiniJobId = currActiveJob.userMiniJobId;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REDEEM_MINI_JOB_EVENT, null);

		isRedeeming = true;

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		RedeemMiniJobResponseProto response = UMQNetworkManager.responseDict[tagNum] as RedeemMiniJobResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == RedeemMiniJobResponseProto.RedeemMiniJobStatus.SUCCESS)
		{
			if (response.fump != null)
			{
				MSMonsterManager.instance.UpdateOrAdd(response.fump);
			}

			MSResourceManager.instance.Collect(ResourceType.CASH, currActiveJob.miniJob.cashReward);
			MSResourceManager.instance.Collect(ResourceType.OIL, currActiveJob.miniJob.oilReward);
			MSResourceManager.instance.Collect(ResourceType.GEMS, currActiveJob.miniJob.gemReward);

		}
		else
		{
			Debug.LogError("Problem redeeming job: " + response.status.ToString());
		}

		currActiveJob = null;
		isRedeeming = false;
	}

	#endregion

	#region Util

	bool IsJobActive(UserMiniJobProto userJob)
	{
		return userJob.timeStarted > 0;
	}

	public bool IsMonsterBusy(int userMonsterId)
	{
		foreach (var item in userMiniJobs) 
		{
			foreach (var monster in item.userMonsterIds) 
			{
				if (monster == userMonsterId)
				{
					return true;
				}
			}
		}
		return false;
	}

	#endregion

}
