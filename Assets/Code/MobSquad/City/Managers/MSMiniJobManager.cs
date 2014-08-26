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

	public bool isBeginning = false;

	public bool isRedeeming = false;

	public bool isCompleting = false;

	MiniJobCenterProto currJobCenter;

	public List<UserMiniJobProto> userMiniJobs = new List<UserMiniJobProto>();

	public UserMiniJobProto currActiveJob = null;

	/// <summary>
	/// List of mobsters that have been damaged by the completed task
	/// </summary>
	public List<PZMonster> teamToDamage;

	/// <summary>
	/// List of damage dealt to each mobster
	/// </summary>
	public List<int> damageDelt = new List<int>();

	public long timeUntilRefresh
	{
		get
		{
			return MSWhiteboard.localUser.lastMiniJobSpawnedTime 
					+ (currJobCenter.hoursBetweenJobGeneration * 60 * 60 * 1000)
					- MSUtil.timeNowMillis;
		}
	}

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
				&& currActiveJob.userMiniJobId > 0
				&& currActiveJob.timeStarted > 0
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

	void OnEnable()
	{
		MSActionManager.Loading.OnStartup += OnStartup;
	}

	void OnDisable()
	{
		MSActionManager.Loading.OnStartup -= OnStartup;
	}

	void OnStartup(StartupResponseProto response)
	{
		userMiniJobs = response.userMiniJobProtos;

		foreach (var item in userMiniJobs) 
		{
			if (item.timeStarted > 0)
			{
				currActiveJob = item;
			}
		}
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

			yield return new WaitForSeconds(1);
		}
	}

	#endregion

	#region Job Spawning

	bool NeedsToSpawn()
	{
		return currJobCenter.structInfo.level > 0 && timeUntilRefresh <= 0;
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
				Debug.LogWarning("Adding job!");
				userMiniJobs.Add(job);
				if(MSActionManager.MiniJob.OnMiniJobRestock != null)
				{
					MSActionManager.MiniJob.OnMiniJobRestock();
				}
			}
		}
		else
		{
			Debug.LogError("Problem spawning minijobs: " + response.status.ToString());
		}
	}

	#endregion

	#region Beginning Job

	public void BeginJob(UserMiniJobProto job, List<PZMonster> monsters)
	{
		if (currActiveJob != null && currActiveJob.userMiniJobId > 0)
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

		isBeginning = true;



		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_MINI_JOB_EVENT, 
		                                       DealWithJobBegin);
		if(MSActionManager.MiniJob.OnMiniJobBegin != null)
		{
			MSActionManager.MiniJob.OnMiniJobBegin(job);
		}
	}

	void DealWithJobBegin(int tagNum)
	{
		isBeginning = false;

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
			StartCoroutine(CompleteCurrentJob(true, numGems));
			if(MSActionManager.MiniJob.OnMiniJobGemsComplete != null)
			{
				MSActionManager.MiniJob.OnMiniJobGemsComplete();
			}
		}
	}

	void CompleteCurrentJobWithWait()
	{
		if (!shouldComplete)
		{
			Debug.LogError("Don't try to complete a job that isn't actually complete. Dumbass.");
			return;
		}

		StartCoroutine(CompleteCurrentJob(false));
	}

	IEnumerator CompleteCurrentJob(bool speedUp, int gems = 0)
	{	
		if (isCompleting) //If the player decides to mash the button, don't send more requests
		{
			yield break;
		}

		isCompleting = true;

		while (isBeginning)
		{
			yield return null;
		}

		currActiveJob.timeCompleted = MSUtil.timeNowMillis;

		if(MSActionManager.MiniJob.OnMiniJobComplete != null)
		{
			MSActionManager.MiniJob.OnMiniJobComplete();
		}

		CompleteMiniJobRequestProto request = new CompleteMiniJobRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clientTime = MSUtil.timeNowMillis;
		request.userMiniJobId = currActiveJob.userMiniJobId;
		request.isSpeedUp = speedUp;
		request.gemCost = gems;

		teamToDamage = new List<PZMonster>();
		foreach (var item in currActiveJob.userMonsterIds) 
		{
			teamToDamage.Add(MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId == item));
		}
//		DamageMonsters(teamToDamage, currActiveJob.baseDmgReceived);
		DamageMonsters(currActiveJob.baseDmgReceived);

		foreach (var item in teamToDamage) 
		{
			//request.umchp.Add(item.GetCurrentHealthProto());
		}

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_COMPLETE_MINI_JOB_EVENT,
		                                       DealWithJobCompleteResponse);
	}

	/// <summary>
	/// Damages the team monsters.
	/// </summary>
	/// <param name="amount">Amount of damage to be split across all participating monsters.</param>
	/// <param name="damageOff">If set to <c>true</c> Monsters are not damaged, but damageDelt is updated.</param>
	void DamageMonsters(int amount, bool isDamageOff = false)//, ref List<PZMonster> team)
	{
		List<PZMonster> team = teamToDamage;

		List<PZMonster> aliveMonsters = team.FindAll(x=>x.currHP != 0);
		int amountPerMonster = Mathf.CeilToInt((float)amount / aliveMonsters.Count);
		int overflow = 0;

		for (int i = 0; i < team.Count; i++)
		{
			PZMonster item = team[i];
			if((!isDamageOff && item.currHP == 0) || (isDamageOff && item.tempHP == 0))
			{
				continue;
			}

			if ((!isDamageOff && amountPerMonster > item.currHP) || (isDamageOff && amountPerMonster > item.tempHP))
			{

				damageDelt[i] += isDamageOff?item.tempHP:item.currHP;
				if(!isDamageOff)
				{
					overflow = amountPerMonster - item.currHP;
					item.currHP = 0;
				}
				else
				{
					overflow = amountPerMonster - item.tempHP;
					item.tempHP = 0;
				}
			}
			else
			{
				damageDelt[i] += amountPerMonster;
				if(!isDamageOff)
				{
					item.currHP -= amountPerMonster;
				}
				else
				{
					item.tempHP -= amountPerMonster;
				}
			}

			if (overflow > 0)
			{
//				monsters = new List<PZMonster>(monsters); //We need to copy the list before removing things
//				monsters.RemoveAll(x=>x.currHP == 0);
				DamageMonsters(overflow, isDamageOff);
				overflow = 0;
			}
		}
//		Debug.Log("current damge taken: " + damageDelt[0] + "," + damageDelt[1] + "," + damageDelt[2]);

	}
	public void SetTeamAndDamage()
	{
		teamToDamage.Clear();
		damageDelt.Clear();
		foreach (var item in currActiveJob.userMonsterIds) 
		{
			PZMonster monster = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId == item);
			monster.tempHP = monster.currHP;
			teamToDamage.Add(monster);
		}
		for(int i = 0; i < teamToDamage.Count; i++)
		{
			damageDelt.Add(0);
		}
		DamageMonsters(currActiveJob.baseDmgReceived, true);
	}

	void DealWithJobCompleteResponse(int tagNum)
	{
		isCompleting = false;

		CompleteMiniJobResponseProto response = UMQNetworkManager.responseDict[tagNum] as CompleteMiniJobResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != CompleteMiniJobResponseProto.CompleteMiniJobStatus.SUCCESS)
		{
			Debug.Log("Problem completing job: " + response.status.ToString());
			//TODO: Change monster healths accordingly!
		}
	}

	#endregion

	#region Job Redeeming

	public bool RedeemCurrJob()
	{
		if (!isCompleted || isRedeeming) //Use the isRedeeming bool to make sure we don't stack requests
		{
			Debug.LogError("Job hasn't been completed, cannot redeem.");
			return false;
		}

		StartCoroutine(RunJobRedemption());
		return true;
	}

	IEnumerator RunJobRedemption()
	{
		//Gotta make sure that we've waited for the completion request to return before
		//we let the player actually redeem and shit
		while (currActiveJob.timeCompleted == 0)
		{
			yield return null;
		}

		UserMiniJobProto currJob = currActiveJob;
		currActiveJob = null;
		userMiniJobs.Remove(currJob);

		RedeemMiniJobRequestProto request = new RedeemMiniJobRequestProto();
		request.sender = MSWhiteboard.localMupWithResources;
		request.clientTime = MSUtil.timeNowMillis;
		request.userMiniJobId = currJob.userMiniJobId;
		
		//Damage Monsters
		int damage = currJob.baseDmgReceived;
		foreach (var item in currJob.userMonsterIds) 
		{
			PZMonster monster = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId == item);
			monster.currHP = Mathf.Max(monster.currHP - damage/currJob.userMonsterIds.Count, 0);
			request.umchp.Add(monster.GetCurrentHealthProto());
		}

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REDEEM_MINI_JOB_EVENT, null);

		isRedeeming = true;

		Debug.Log("Redeeming job: " + request.userMiniJobId);

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

			MSResourceManager.instance.Collect(ResourceType.CASH, currJob.miniJob.cashReward);
			MSResourceManager.instance.Collect(ResourceType.OIL, currJob.miniJob.oilReward);
			MSResourceManager.instance.Collect(ResourceType.GEMS, currJob.miniJob.gemReward);

			if(MSActionManager.MiniJob.OnMiniJobRedeem != null)
			{
				MSActionManager.MiniJob.OnMiniJobRedeem();
			}
		}
		else
		{
			Debug.LogError("Problem redeeming job: " + response.status.ToString());
		}

		isRedeeming = false;
	}

	#endregion

	#region Util

	bool IsJobActive(UserMiniJobProto userJob)
	{
		return userJob.timeStarted > 0;
	}

	public bool IsMonsterBusy(long userMonsterId)
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
