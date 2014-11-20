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

	public MiniJobCenterProto currJobCenter;

	public List<UserMiniJobProto> userMiniJobs = new List<UserMiniJobProto>();

	public UserMiniJobProto currActiveJob = null;

	ClanHelpProto currActiveHelp = null;

	public int helpCount
	{
		get
		{
			if(currActiveJob != null)
			{
				if(currActiveHelp == null || currActiveHelp.helpType != GameActionType.MINI_JOB || !currActiveHelp.userDataUuid.Equals(currActiveJob.userMiniJobUuid))
				{
					currActiveHelp = MSClanManager.instance.GetClanHelp(GameActionType.MINI_JOB, currActiveJob.userMiniJobUuid);
				}

				if(currActiveHelp != null)
				{
					if(currActiveHelp.helperUuids.Count > MSBuildingManager.clanHouse.combinedProto.clanHouse.maxHelpersPerSolicitation)
					{
						return MSBuildingManager.clanHouse.combinedProto.clanHouse.maxHelpersPerSolicitation;
					}
					else
					{
						return currActiveHelp.helperUuids.Count;
					}
				}
			}

			return 0;
		}
	}

	StartupResponseProto.StartupConstants.ClanHelpConstants minijobHelpConstant;

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
			return MSMath.GemsForTime(timeLeft, true);
		}
	}

	public long helpTime
	{
		get
		{
			if(minijobHelpConstant == null)
			{
				minijobHelpConstant = MSWhiteboard.constants.clanHelpConstants.Find(x=>x.helpType == GameActionType.MINI_JOB);
			}
			int amountRemovedPerHelp = minijobHelpConstant.amountRemovedPerHelp;
			float percentRemovedPerHelp = minijobHelpConstant.percentRemovedPerHelp;

			long totalTime = currActiveJob.durationSeconds;
			if(amountRemovedPerHelp < percentRemovedPerHelp * totalTime)
			{
				return (long)(percentRemovedPerHelp * totalTime * helpCount);
			}
			else
			{
				return (long)(amountRemovedPerHelp * helpCount);
			}
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
			return (currActiveJob.timeStarted + currActiveJob.durationSeconds * 1000 - MSUtil.timeNowMillis) - (helpTime * 1000);
		}
	}

	bool shouldComplete
	{
		get
		{
			return currActiveJob != null
				&& !currActiveJob.userMiniJobUuid.Equals("")
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
		MSActionManager.Clan.OnGiveClanHelp += DealWithHelp;
		MSActionManager.Clan.OnEndClanHelp += DealWithEnd;
	}

	void OnDisable()
	{
		MSActionManager.Loading.OnStartup -= OnStartup;
		MSActionManager.Clan.OnGiveClanHelp -= DealWithHelp;
		MSActionManager.Clan.OnEndClanHelp -= DealWithEnd;
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

	void DealWithHelp(GiveClanHelpResponseProto help, bool self)
	{
		if(!self)
		{
			currActiveHelp = MSClanManager.instance.GetClanHelp(GameActionType.MINI_JOB, currActiveJob.userMiniJobUuid);
		}
	}

	void DealWithEnd(EndClanHelpResponseProto help, bool self)
	{
		if(self)
		{
			currActiveHelp = null;
		}
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

				if(MSActionManager.Popup.DisplayOrangeError != null)
				{
					MSActionManager.Popup.DisplayOrangeError("Mini jobs have been restocked!");
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
		if (currActiveJob != null && !currActiveJob.userMiniJobUuid.Equals(""))
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
			request.userMonsterUuids.Add(item.userMonster.userMonsterUuid);
			job.userMonsterUuids.Add (item.userMonster.userMonsterUuid);
		}

		request.userMiniJobUuid = job.userMiniJobUuid;

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
		if(MSActionManager.MiniJob.OnMiniJobBeginResponse != null)
		{
			MSActionManager.MiniJob.OnMiniJobBeginResponse();
		}
	}	                                   

	#endregion

	#region Completing Job

	public void DoCompleteCurrentJobWithGems(Action OnComplete = null)
	{
		StartCoroutine(CompleteCurrentJobWithGems(OnComplete));
	}

	public IEnumerator CompleteCurrentJobWithGems(Action OnComplete = null)
	{
		int numGems = MSMath.GemsForTime(timeLeft, false);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, numGems))
		{
			yield return StartCoroutine(CompleteCurrentJob(true, numGems));
			if(MSActionManager.MiniJob.OnMiniJobGemsComplete != null)
			{
				MSActionManager.MiniJob.OnMiniJobGemsComplete();
			}
		}

		if(OnComplete != null)
		{
			OnComplete();
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
		request.userMiniJobUuid = currActiveJob.userMiniJobUuid;
		request.isSpeedUp = speedUp;
		request.gemCost = gems;

		teamToDamage = new List<PZMonster>();
		foreach (var item in currActiveJob.userMonsterUuids) 
		{
			teamToDamage.Add(MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(item)));
		}
//		DamageMonsters(teamToDamage, currActiveJob.baseDmgReceived);
		damageDelt.Clear();
		for(int i = 0; i < teamToDamage.Count; i++)
		{
			damageDelt.Add(0);
		}
		DamageMonsters(currActiveJob.baseDmgReceived, true);

		foreach (var item in teamToDamage) 
		{
			//request.umchp.Add(item.GetCurrentHealthProto());
		}

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_COMPLETE_MINI_JOB_EVENT);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		isCompleting = false;
		
		CompleteMiniJobResponseProto response = UMQNetworkManager.responseDict[tagNum] as CompleteMiniJobResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != CompleteMiniJobResponseProto.CompleteMiniJobStatus.SUCCESS)
		{
			MSActionManager.Popup.DisplayRedError("Problem completing job: " + response.status.ToString());
		}
		else
		{
			ClanHelpProto miniJobHelp = MSClanManager.instance.GetClanHelp(GameActionType.MINI_JOB, (int)currActiveJob.miniJob.quality ,currActiveJob.userMiniJobUuid);
			if(miniJobHelp != null)
			{
				MSClanManager.instance.DoEndClanHelp(new List<string>{miniJobHelp.clanHelpUuid});
				if(MSActionManager.Popup.DisplayOrangeError != null)
				{
					MSActionManager.Popup.DisplayOrangeError("Your mobsters are back from their minijob!");
				}
			}
		}
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
		foreach (var item in currActiveJob.userMonsterUuids) 
		{
			PZMonster monster = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(item));
			monster.tempHP = monster.currHP;
			teamToDamage.Add(monster);
		}
		for(int i = 0; i < teamToDamage.Count; i++)
		{
			damageDelt.Add(0);
		}
		DamageMonsters(currActiveJob.baseDmgReceived, true);
	}

	#endregion

	#region Job Redeeming

	public Coroutine RedeemCurrJob()
	{
		if (!isCompleted || isRedeeming) //Use the isRedeeming bool to make sure we don't stack requests
		{
			MSActionManager.Popup.DisplayRedError("Job hasn't been completed, cannot redeem.");
			return null;
		}

		return StartCoroutine(RunJobRedemption());
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
		currActiveHelp = null;
		userMiniJobs.Remove(currJob);

		RedeemMiniJobRequestProto request = new RedeemMiniJobRequestProto();
		request.sender = MSWhiteboard.localMupWithResources;
		request.clientTime = MSUtil.timeNowMillis;
		request.userMiniJobUuid = currJob.userMiniJobUuid;
		
		//Damage Monsters
		int damage = currJob.baseDmgReceived;
		foreach (var item in currJob.userMonsterUuids) 
		{
			PZMonster monster = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(item));
			monster.currHP = Mathf.Max(monster.currHP - damage/currJob.userMonsterUuids.Count, 0);
			request.umchp.Add(monster.GetCurrentHealthProto());
		}

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REDEEM_MINI_JOB_EVENT, null);

		isRedeeming = true;

		Debug.Log("Redeeming job: " + request.userMiniJobUuid);

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

	public bool IsMonsterBusy(string userMonsterId)
	{
		foreach (var item in userMiniJobs) 
		{
			foreach (var monster in item.userMonsterUuids) 
			{
				if (monster.Equals(userMonsterId))
				{
					return true;
				}
			}
		}
		return false;
	}

	#endregion

}
