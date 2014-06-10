using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSAchievementManager : MonoBehaviour {

	public static MSAchievementManager instance;

	public List<MSFullAchievement> currAchievements = new List<MSFullAchievement>();

	/// <summary>
	/// The progress request.
	/// We have this chill around so that if we have a bunch of achievements
	/// that get progressed on the same frame (e.g. startup, post-combat), we
	/// can collect all of the updates and send them in a single request
	/// </summary>
	AchievementProgressRequestProto progressRequest;

	void Awake()
	{
		progressRequest = new AchievementProgressRequestProto(); //Just to be sure...
		instance = this;
	}

	void OnEnable()
	{
		MSActionManager.Loading.OnStartup += OnStartup;
		MSActionManager.Quest.OnBattleFinish += OnCombatProgress;
		MSActionManager.Pvp.OnPvpVictory += OnPvpVictory;
		MSActionManager.Quest.OnMoneyCollected += OnCollectFromBuilding;
		MSActionManager.Quest.OnMonsterEnhanced += OnMonsterEnhanced;
		MSActionManager.Town.OnObstacleRemoved += OnObstacleRemoved;
		MSActionManager.Quest.OnMonstersSold += OnMobstersSold;
		MSActionManager.Quest.OnStructureUpgraded += OnStructureUpgraded;
	}

	void OnDisable()
	{
		MSActionManager.Loading.OnStartup -= OnStartup;
		MSActionManager.Quest.OnBattleFinish -= OnCombatProgress;
		MSActionManager.Pvp.OnPvpVictory -= OnPvpVictory;
		MSActionManager.Quest.OnMoneyCollected -= OnCollectFromBuilding;
		MSActionManager.Quest.OnMonsterEnhanced -= OnMonsterEnhanced;
		MSActionManager.Town.OnObstacleRemoved -= OnObstacleRemoved;
		MSActionManager.Quest.OnMonstersSold -= OnMobstersSold;
		MSActionManager.Quest.OnStructureUpgraded -= OnStructureUpgraded;
	}

	void OnStartup(StartupResponseProto startup)
	{
		progressRequest.sender = MSWhiteboard.localMup;
		foreach (var item in startup.userAchievements)
		{
			if (!item.isRedeemed)
			{
				currAchievements.Add(new MSFullAchievement(item));
			}
		}
		foreach (AchievementProto item in MSDataManager.instance.GetAll<AchievementProto>().Values)
		{
			//If we've never done this achievement, add it in as new
			if (startup.userAchievements.Find(x=>x.achievementId == item.achievementId) == null)
			{
				currAchievements.Add (new MSFullAchievement(item));
			}
		}
		currAchievements.Sort((x,y) => x.achievement.priority.CompareTo(y.achievement.priority));
	}

	public void AddProgressedAchievement(MSFullAchievement fullAchievement)
	{
		progressRequest.uapList.Add(fullAchievement.userAchievement);
	}

	void Update()
	{
		if (progressRequest.uapList.Count > 0)
		{
			progressRequest.clientTime = MSUtil.timeNowMillis;
			UMQNetworkManager.instance.SendRequest(progressRequest, (int)EventProtocolRequest.C_ACHIEVEMENT_PROGRESS_EVENT, DealWithProgressResponse);
			progressRequest.uapList.Clear();
		}
	}

	void DealWithProgressResponse(int tagNum)
	{
		AchievementProgressResponseProto response = UMQNetworkManager.responseDict[tagNum] as AchievementProgressResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != AchievementProgressResponseProto.AchievementProgressStatus.SUCCESS)
		{
			Debug.LogError("Problem progressing achievements: " + response.status.ToString());
		}
	}

	public MSFullAchievement RedeemAchievement(MSFullAchievement achievement)
	{
		AchievementRedeemRequestProto request = new AchievementRedeemRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.achievementId = achievement.userAchievement.achievementId;
		request.clientTime = MSUtil.timeNowMillis;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ACHIEVEMENT_REDEEM_EVENT, DealWithRedeemAchievementResponse);

		currAchievements.Remove(achievement);

		return currAchievements.Find(x=>x.achievement.achievementId == achievement.achievement.successorId);
	}

	void DealWithRedeemAchievementResponse(int tagNum)
	{
		AchievementRedeemResponseProto response = UMQNetworkManager.responseDict[tagNum] as AchievementRedeemResponseProto;
		UMQNetworkManager.responseDict.Remove (tagNum);

		if (response.status != AchievementRedeemResponseProto.AchievementRedeemStatus.SUCCESS)
		{
			Debug.LogError("Problem redeeming achievement: " + response.status.ToString());
		}
	}


	#region Progress Functions

	void OnCombatProgress(BattleStats battleStats)
	{
		foreach (var item in currAchievements) 
		{
			switch(item.achievement.achievementType)
			{
			case AchievementProto.AchievementType.DESTROY_ORBS:
				item.AddProgress(battleStats.orbs[(int)item.achievement.element-1]);
				break;
			case AchievementProto.AchievementType.CREATE_GRENADE:
				item.AddProgress(battleStats.grenades);
				break;
			case AchievementProto.AchievementType.CREATE_ROCKET:
				item.AddProgress(battleStats.rockets);
				break;
			case AchievementProto.AchievementType.CREATE_RAINBOW:
				item.AddProgress(battleStats.rainbows);
				break;
			case AchievementProto.AchievementType.MAKE_COMBO:
				item.AddProgress(battleStats.combos);
				break;
			case AchievementProto.AchievementType.TAKE_DAMAGE:
				item.AddProgress(battleStats.damageTaken);
				break;
			case AchievementProto.AchievementType.DEFEAT_MONSTERS:
				item.AddProgress(battleStats.monstersDefeated);
				break;
			default:
				break;
			}
		}
	}

	void OnPvpVictory(int cashStolen, int oilStolen)
	{
		foreach (var item in currAchievements) 
		{
			switch(item.achievement.achievementType)
			{
			case AchievementProto.AchievementType.STEAL_RESOURCE:
				item.AddProgress(item.achievement.resourceType == ResourceType.CASH ? cashStolen : oilStolen);
				break;
			case AchievementProto.AchievementType.WIN_PVP_BATTLE:
				item.AddProgress(1);
				break;
			default:
				break;
			}
		}
	}

	void OnCollectFromBuilding(ResourceType resourceType, int amount)
	{
		foreach (var item in currAchievements) 
		{
			switch(item.achievement.achievementType)
			{
			case AchievementProto.AchievementType.COLLECT_RESOURCE:
				if (item.achievement.resourceType == resourceType)
				{
					item.AddProgress(amount);
				}
				break;
			default:
				break;
			}
		}
	}

	void OnMonsterEnhanced(int enhancePoints)
	{
		foreach (var item in currAchievements)
		{
			switch(item.achievement.achievementType)
			{
			case AchievementProto.AchievementType.ENHANCE_POINTS:
				item.AddProgress(enhancePoints);
				break;
			default:
				break;
			}
		}
	}

	void OnLeagueJoined(int leagueId)
	{
		foreach (var item in currAchievements) 
		{
			switch(item.achievement.achievementType)
			{
			case AchievementProto.AchievementType.JOIN_LEAGUE:
				if (item.achievement.staticDataId == leagueId)
				{
					item.AddProgress(1);
				}
				break;
			default:
				break;
			}
		}
	}

	void OnObstacleRemoved(MSObstacle obstacle)
	{
		foreach (var item in currAchievements) 
		{
			switch (item.achievement.achievementType) 
			{
			case AchievementProto.AchievementType.REMOVE_OBSTACLE:
				item.AddProgress(1);
				break;
			default:
				break;
			}
		}
	}

	void OnMobstersSold(int numMonsters)
	{
		foreach (var item in currAchievements) 
		{
			switch (item.achievement.achievementType) 
			{
			case AchievementProto.AchievementType.SELL_MONSTER:
				item.AddProgress(numMonsters);
				break;
			default:
				break;
			}
		}
	}

	void OnStructureUpgraded(int structId)
	{
		foreach (var item in currAchievements) 
		{
			switch(item.achievement.achievementType)
			{
			case AchievementProto.AchievementType.UPGRADE_BUILDING:
				if (item.achievement.staticDataId == structId)
				{
					item.AddProgress(1);
				}
				break;
			default:
				break;
			}
		}
	}
	
	#endregion
}
