using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSEnhancementManager
/// </summary>
public class MSEnhancementManager : MonoBehaviour 
{
	public static MSEnhancementManager instance;

	/// <summary>
	/// The temporary spot for the monster whose enhancement
	/// is being considered. Cleared when the enhancement menu is
	/// closed or when an enhancement is sent to the server.
               	/// </summary>
	public PZMonster tempEnhancementMonster = null;

	/// <summary>
	/// The working list of feeders, until they're sent to the server for the enhancement.
	/// </summary>
	public List<PZMonster> feeders = new List<PZMonster>();

	/// <summary>
	/// The current, active enhancement.
	/// Set when after enhancement is started, or during startup if one is already active.
	/// If this is null, or contains no feeders, there is no ongoing enhancement.
	/// </summary>
	public UserEnhancementProto currEnhancement;

	#region clanHelp
	StartupResponseProto.StartupConstants.ClanHelpConstants enhancementHelpConstant;
	
	ClanHelpProto currActiveHelp = null;
	
	public int helpCount
	{
		get
		{
			if(currEnhancement != null && feeders.Count > 0)
			{
				if(currActiveHelp == null || currActiveHelp.helpType != GameActionType.ENHANCE_TIME || !currActiveHelp.userDataUuid.Equals(currEnhancement.baseMonster.userMonsterUuid))
				{
					currActiveHelp = MSClanManager.instance.GetClanHelp(GameActionType.ENHANCE_TIME, currEnhancement.baseMonster.userMonsterUuid);
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

	public long helpTime
	{
		get
		{
			if(enhancementHelpConstant == null)
			{
				enhancementHelpConstant = MSWhiteboard.constants.clanHelpConstants.Find(x=>x.helpType == GameActionType.ENHANCE_TIME);
			}
			int amountRemovedPerHelp = enhancementHelpConstant.amountRemovedPerHelp;
			float percentRemovedPerHelp = enhancementHelpConstant.percentRemovedPerHelp;
			
			long totalTime = totalDuration;
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
	
	#endregion clanHelp

	public LabProto currLab
	{
		get
		{
			if (MSBuildingManager.enhanceLabs.Count == 0)
			{
				return null;
			}
			if (!MSBuildingManager.enhanceLabs[0].upgrade.isComplete)
			{
				return MSBuildingManager.enhanceLabs[0].combinedProto.predecessor.lab;
			}
			return MSBuildingManager.enhanceLabs[0].combinedProto.lab;
		}
	}

	bool isCompleting = false;

	#region Properties

	public PZMonster enhancementMonster
	{
		get
		{
			if (hasEnhancement)
			{
				return MSMonsterManager.instance.userMonsters.Find (x=>x.userMonster.userMonsterUuid.Equals(currEnhancement.baseMonster.userMonsterUuid));
			}
			else
			{
				return tempEnhancementMonster;
			}
		}
	}

	public int startCost
	{
		get
		{
			if (tempEnhancementMonster == null || tempEnhancementMonster.monster == null || tempEnhancementMonster.monster.monsterId == 0) return 0;
			return tempEnhancementMonster.enhanceCost * feeders.Count;
		}
	}

	public long potentialTime
	{
		get
		{
			long potential = 0;
			foreach (var item in feeders) {
				potential += item.enhanceTime;
			}
			return potential;
		}
	}

	public bool hasEnhancement
	{
		get
		{
			return currEnhancement != null && currEnhancement.baseMonster != null && !currEnhancement.baseMonster.userMonsterUuid.Equals("");
		}
	}

	public long startTime
	{
		get
		{
			long startTime = 0;
			foreach (PZMonster item in feeders)
			{
				if(startTime == 0)
				{
					startTime = item.startEnhanceTime;
				}
				else
				{
					//min(startTime, item.startEnhanceTime)
					startTime = startTime < item.startEnhanceTime ? item.startEnhanceTime : startTime;
				}
			}

			return startTime;
		}
	}

	public long finishTime
	{
		get
		{
			if (!hasEnhancement) return -1;
			UserEnhancementItemProto last = currEnhancement.feeders[0];
			for (int i = 1; i < currEnhancement.feeders.Count; i++)
			{
				if (last.expectedStartTimeMillis < currEnhancement.feeders[i].expectedStartTimeMillis) 
				{
					last = currEnhancement.feeders[i];
				}
			}
			return MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(last.userMonsterUuid)).finishEnhanceTime;
		}
	}

	public long totalDuration
	{
		get
		{
			long now = MSUtil.timeNowMillis;
			long dur = finishTime - startTime;
			return finishTime - startTime;
		}
	}

	public long timeLeft
	{
		get
		{
			if (!hasEnhancement) return -1;
			return finishTime - MSUtil.timeNowMillis;
		}

	}

	public int gemsToFinish
	{
		get
		{
			if (!hasEnhancement) return -1;
			return MSMath.GemsForTime(timeLeft, true);
		}
	}

	public bool finished
	{
		get
		{
			return hasEnhancement && currEnhancement.baseMonster.enhancingComplete;
		}
	}

	#endregion

	void Awake()
	{
		instance = this;
	}

	public void Init(UserEnhancementProto enhancement)
	{
		if (enhancement != null)
		{
			this.currEnhancement = enhancement;
			PZMonster monster;
			foreach (var item in enhancement.feeders) 
			{
				monster = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(item.userMonsterUuid));
				feeders.Add(monster);
				monster.SetTimer(GameActionType.ENHANCE_TIME, item.expectedStartTimeMillis, monster.enhanceTime);
			}
		}
	}

	#region Adding and Removing Monsters
	/// <summary>
	/// Sets the temp enhancement monster and resets the feeder group.
	/// Called when a monster is picked on the PickEnhanceScreen.
	/// </summary>
	/// <param name="monster">Monster.</param>
	public void SetEnhancementMonster(PZMonster monster)
	{
		tempEnhancementMonster = monster;
		feeders.Clear();
	}

	/// <summary>
	/// Adds a monster to the feeder group, if there's room.
	/// TODO: We should really have more checks on this to make sure it's an appropriate submission,
	/// but for now we're trusting that the UI won't submit anything that shouldn't be used (healing, pieces, etc.)
	/// </summary>
	/// <param name="monster">Monster.</param>
	public bool AddMonster(PZMonster monster)
	{
		if (feeders.Count < currLab.queueSize)
		{
			feeders.Add(monster);
			return true;
		}
		else
		{
			MSActionManager.Popup.DisplayRedError("Queue full!");
			return false;
		}
	}

	/// <summary>
	/// Removes a monster from the feeder group.
	/// </summary>
	/// <param name="monster">Monster.</param>
	public void RemoveMonster(PZMonster monster)
	{
		feeders.Remove(monster);
	}

	public void ClearTemp()
	{
		tempEnhancementMonster = null;
		if (!hasEnhancement)
		{
			feeders.Clear();
		}
	}
	#endregion

	#region Starting Enhance
	public Coroutine DoSendStartEnhanceRequest(int oil, int gems, MSLoadLock loadLock)
	{
		return StartCoroutine(SendStartEnhanceRequest(oil, gems, loadLock));
	}

	IEnumerator SendStartEnhanceRequest(int oil, int gems, MSLoadLock loadLock)
	{
		if (tempEnhancementMonster == null
		    || tempEnhancementMonster.monster == null
		    || tempEnhancementMonster.monster.monsterId == 0)
		{
			Debug.LogError("FAIL: Tried to start enhancement without an enhancement monster");
			yield break;
		}

		if (feeders.Count == 0)
		{
			Debug.LogError("FAIL: Tried to start enhancement without any feeders");
			yield break;
		}

		yield return MSResourceManager.instance.RunCollectResources();

		SubmitMonsterEnhancementRequestProto request = new SubmitMonsterEnhancementRequestProto();
		request.sender = MSWhiteboard.localMupWithResources;
		request.gemsSpent = gems;
		request.oilChange = -oil;

		UserEnhancementItemProto item = new UserEnhancementItemProto();
		item.userMonsterUuid = tempEnhancementMonster.userMonster.userMonsterUuid;
		item.enhancingComplete = false;
		request.ueipNew.Add(item);

		UserEnhancementProto enhancement = new UserEnhancementProto();
		enhancement.baseMonster = item;

		long lastStartTime = MSUtil.timeNowMillis;
		foreach (var feeder in feeders) {
			item = new UserEnhancementItemProto();
			item.userMonsterUuid = feeder.userMonster.userMonsterUuid;
			item.enhancingCost = tempEnhancementMonster.enhanceCost;
			item.expectedStartTimeMillis = lastStartTime;
			long feedTime = (long)(feeder.enhanceXP * 1000f / currLab.pointsPerSecond);
			lastStartTime += feedTime;
			enhancement.feeders.Add (item);
			request.ueipNew.Add(item);
			feeder.SetTimer(GameActionType.ENHANCE_TIME, item.expectedStartTimeMillis, feedTime);
		}
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_SUBMIT_MONSTER_ENHANCEMENT_EVENT);
	
		if (loadLock != null) loadLock.Lock();

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		if (loadLock != null) loadLock.Unlock();

		SubmitMonsterEnhancementResponseProto response = UMQNetworkManager.responseDict[tagNum] as SubmitMonsterEnhancementResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status == SubmitMonsterEnhancementResponseProto.SubmitMonsterEnhancementStatus.SUCCESS)
		{
			currEnhancement = enhancement;
			ClearTemp();

			if(MSActionManager.Goon.OnStartEnhance != null)
			{
				MSActionManager.Goon.OnStartEnhance();
			}
		}
		else
		{
			Debug.LogError("Problem starting enhancement: " + response.status.ToString());
		}
	}
	#endregion

	#region Completing
	void CheckEnhancingMonsters()
	{
		if (hasEnhancement && !finished && !isCompleting && timeLeft <= 0)
		{
			StartCoroutine (SendCompleteEnhancementRequest());
		}
	}

	public Coroutine FinishEnhanceWithGems(MSLoadLock loadLock = null)
	{
		return StartCoroutine(SendCompleteEnhancementRequest(gemsToFinish, loadLock));
	}

	IEnumerator SendCompleteEnhancementRequest(int gems = 0, MSLoadLock loadLock = null)
	{
		isCompleting = true;
		EnhancementWaitTimeCompleteRequestProto request = new EnhancementWaitTimeCompleteRequestProto();
		request.sender = MSWhiteboard.localMup;

		request.isSpeedup = gems > 0;
		request.gemsForSpeedup = gems;
		request.userMonsterUuid = currEnhancement.baseMonster.userMonsterUuid;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT);

		if (loadLock != null) loadLock.Lock();
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum)) yield return null;
		if (loadLock != null) loadLock.Unlock();

		EnhancementWaitTimeCompleteResponseProto response = UMQNetworkManager.responseDict[tagNum] as EnhancementWaitTimeCompleteResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == EnhancementWaitTimeCompleteResponseProto.EnhancementWaitTimeCompleteStatus.SUCCESS)
		{
			currEnhancement.baseMonster.enhancingComplete = true;
			currActiveHelp = null;
		}
		else
		{
			Debug.LogError("Problem completing enhancement: "+ response.status.ToString());
		}
		isCompleting = false;
	}
	#endregion

	#region Collecting

	public Coroutine DoCollectEnhancement(MSLoadLock loadLock = null, Action after = null)
	{
		return StartCoroutine(CollectEnhancement(loadLock, after));
	}

	IEnumerator CollectEnhancement(MSLoadLock loadLock, Action after)
	{
		CollectMonsterEnhancementRequestProto request = new CollectMonsterEnhancementRequestProto();
		request.sender = MSWhiteboard.localMup;

		foreach (var item in currEnhancement.feeders) 
		{
			request.userMonsterUuids.Add(item.userMonsterUuid);
		}

		request.umcep = enhancementMonster.GetExpProtoWithMonsters(currEnhancement.feeders);

		int tagNum = UMQNetworkManager.instance.SendRequest (request, (int)EventProtocolRequest.C_COLLECT_MONSTER_ENHANCEMENT_EVENT);
		
		if (loadLock != null) loadLock.Lock();
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum)) yield return null;
		if (loadLock != null) loadLock.Unlock();

		CollectMonsterEnhancementResponseProto response = UMQNetworkManager.responseDict[tagNum] as CollectMonsterEnhancementResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == CollectMonsterEnhancementResponseProto.CollectMonsterEnhancementStatus.SUCCESS)
		{
			if (after != null) after();

			PZMonster monster;
			foreach (var item in currEnhancement.feeders) 
			{
				monster = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(item.userMonsterUuid));
				enhancementMonster.GainXP(monster.enhanceXP);
				MSMonsterManager.instance.RemoveMonster(item.userMonsterUuid);
			}
			tempEnhancementMonster = enhancementMonster;
			currEnhancement = null;
			feeders.Clear();
		}
	}

	#endregion

	void DealWithGiveClanHelp(GiveClanHelpResponseProto proto, bool self)
	{
		if(!self && !proto.sender.userUuid.Equals(MSWhiteboard.localMup.userUuid))
		{
			currActiveHelp = null;
		}
	}

	void DealWithEndClanHelp(GiveClanHelpResponseProto proto, bool self)
	{
		if(self)
		{
			currActiveHelp = null;
		}
	}

}
