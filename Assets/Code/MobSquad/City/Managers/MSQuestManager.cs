//#define DEUBG3
//#define DEBUG1
#define DEBUG2

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSQuestManager : MonoBehaviour {
	
	public static MSQuestManager instance;
	
	public List<MSFullQuest> currQuests = new List<MSFullQuest>();

	public Dictionary<int, bool> taskDict = new Dictionary<int, bool>();

	public void Awake()
	{
		instance = this;
	}
	
	public void OnEnable()
	{
		MSActionManager.Quest.OnStructureUpgraded += OnStructureUpgraded;
		MSActionManager.Quest.OnTaskCompleted += OnTaskCompleted;
		MSActionManager.Quest.OnMonsterDonated += OnMonsterDonated;
	}
	
	public void Disable()
	{
		MSActionManager.Quest.OnStructureUpgraded -= OnStructureUpgraded;
		MSActionManager.Quest.OnTaskCompleted -= OnTaskCompleted;
		MSActionManager.Quest.OnMonsterDonated -= OnMonsterDonated;
	}
	
	public void Init(StartupResponseProto proto)
	{
		foreach (FullQuestProto item in proto.staticDataStuffProto.availableQuests) 
		{
#if DEBUG2
			Debug.Log("Available Quest: " + item.questId);		
#endif
			StartCoroutine(AcceptQuest(item));
		}
		foreach (var item in proto.userQuests) 
		{
			currQuests.Add (new MSFullQuest(MSDataManager.instance.Get(typeof(FullQuestProto), item.questId) as FullQuestProto, item));
#if DEBUG2
			Debug.Log ("In Progress Quest: " + item.questId);
#endif
		}

		foreach (var item in proto.completedTaskIds) 
		{
			taskDict.Add(item, true);
		}
		
	}

	public bool HasFinishedAllTasksInCity(int cityID)
	{
		foreach (FullTaskProto item in MSDataManager.instance.GetAll(typeof(FullTaskProto)).Values)
		{
			if (item.cityId == cityID && !taskDict.ContainsKey(item.taskId))
			{
				return false;
			}
		}
		return true;
	}
	
	FullUserQuestProto FindFromUserQuests(List<FullUserQuestProto> questList, int questId)
	{
		foreach (FullUserQuestProto item in questList) {
			if (item.questId == questId)
			{
				questList.Remove(item);
				return item;
			}
		}
		return null;
	}
	
	IEnumerator LoadUserQuestProgress(KeyValuePair<int, FullQuestProto> questData)
	{
		QuestProgressRequestProto request = new QuestProgressRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.questId = questData.Key;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_QUEST_PROGRESS_EVENT, null);
		
#if DEBUG1
		Debug.Log("Loading User Quest Details");
#endif
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		QuestProgressResponseProto response = UMQNetworkManager.responseDict[tagNum] as QuestProgressResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != QuestProgressResponseProto.QuestProgressStatus.SUCCESS)
		{
			Debug.LogError("Problem loading user quest details: " + response.status.ToString());
			yield break;
		}
		
		
		
#if DEBUG1
		string deb = "Loaded Quest details: ";
		foreach (KeyValuePair<int, MSFullQuest> item in questDict) 
		{
			deb += "\n" + item.Key + ": " + item.Value.quest.name;
		}
		Debug.Log(deb);
		
		deb = "Quests not loaded: ";
		foreach (KeyValuePair<int, FullQuestProto> item in tempQuests) 
		{
			deb += "\n" + item.Key + ": " + item.Value.name;
		}
		Debug.Log(deb);
#endif
	}
	
	IEnumerator AcceptQuest(FullQuestProto fullQuest)
	{
		
#if DEBUG2
		Debug.Log("Accepting quest: " + fullQuest.questId);
#endif
		
		QuestAcceptRequestProto request = new QuestAcceptRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.questId = fullQuest.questId;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_QUEST_ACCEPT_EVENT, null);
		
		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
#if DEBUG2
		Debug.Log("Loading Acceptance of quest");
#endif
		
		QuestAcceptResponseProto response = UMQNetworkManager.responseDict[tagNum] as QuestAcceptResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != QuestAcceptResponseProto.QuestAcceptStatus.SUCCESS)
		{
			Debug.Log("Problem accepting quest: " + response.status.ToString());
		}
		else
		{
			FullUserQuestProto userQuest = new FullUserQuestProto();
			userQuest.userId = MSWhiteboard.localMup.userId;
			userQuest.questId = fullQuest.questId;
			userQuest.isRedeemed = false;

			UserQuestJobProto userjob;
			foreach (var job in fullQuest.jobs) 
			{
				userjob = new UserQuestJobProto();
				userjob.questId = fullQuest.questId;
				userjob.questJobId = job.questJobId;
				userjob.isComplete = false;
				userjob.progress = 0;

				userQuest.userQuestJobs.Add(userjob);

			}

			currQuests.Add(new MSFullQuest(fullQuest, userQuest));
		}
	}
	
	void UpdateQuestProgress(MSFullQuest fullQuest, List<UserQuestJobProto> jobsUpdated, List<PZMonster> donateMonsters = null)
	{
		
#if DEBUG3
		Debug.Log("Checking quest: " + fullQuest.quest.name);
#endif
		QuestProgressRequestProto request = new QuestProgressRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.questId = fullQuest.quest.questId;
		foreach (var item in fullQuest.userQuest.userQuestJobs) 
		{
			request.userQuestJobs.Add(item);
		}
		request.isComplete = fullQuest.userQuest.isComplete = fullQuest.userQuest.userQuestJobs.Find(x=>!x.isComplete) == null;

		if (donateMonsters != null)
		{
			foreach (var item in donateMonsters) 
			{
				request.deleteUserMonsterIds.Add(item.userMonster.userMonsterId);
			}
		}

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_QUEST_PROGRESS_EVENT, null);
		
	}

	public void CompleteQuest(MSFullQuest quest)
	{
		currQuests.RemoveAll(x=>x.quest.questId == quest.quest.questId);
		StartCoroutine(RedeemQuest(quest.quest));
	}
	
	//TODO
	IEnumerator RedeemQuest(FullQuestProto quest)
	{
#if DEBUG3
		Debug.Log("Redeeming quest: " + quest.name);
#endif
		yield return null;
		
		currQuests.RemoveAll(x=>x.quest.questId == quest.questId);
		
		QuestRedeemRequestProto request = new QuestRedeemRequestProto();
		request.sender = MSWhiteboard.localMupWithResources;
		request.questId = quest.questId;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_QUEST_REDEEM_EVENT, null);
		
		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		QuestRedeemResponseProto response = UMQNetworkManager.responseDict[tagNum] as QuestRedeemResponseProto;
		
		if (response.status != QuestRedeemResponseProto.QuestRedeemStatus.SUCCESS)
		{
			Debug.LogError("Problem redeeming quest: " + response.status.ToString());
		}
		else
		{
			if (response.fump != null)
			{
				//TODO: Get that equip bro
			}
			if (quest.cashReward > 0)
			{
				MSResourceManager.instance.Collect(ResourceType.CASH, quest.cashReward);
			}
			if (quest.gemReward > 0)
			{
				MSResourceManager.instance.Collect(ResourceType.GEMS, quest.gemReward);
			}
			if (quest.expReward > 0)
			{
				MSResourceManager.instance.GainExp(quest.expReward);
			}
			foreach (FullQuestProto item in response.newlyAvailableQuests) 
			{
				StartCoroutine(AcceptQuest(item));
			}
		}
	}

	void OnStructureUpgraded(int structID)
	{
		foreach (MSFullQuest item in currQuests)
		{
			if (item.userQuest.isComplete)
			{
				continue;
			}
			foreach (var job in item.quest.jobs)
			{
				if (job.questJobType == QuestJobProto.QuestJobType.UPGRADE_STRUCT
				    && job.staticDataId == structID)
				{
					UserQuestJobProto userJob = item.userQuest.userQuestJobs.Find(x=>x.questJobId == job.questJobId);
					userJob.isComplete = ++userJob.progress >= job.quantity;
				}
			}
		}
	}
	
	void OnTaskCompleted(BeginDungeonResponseProto dungeon)
	{
		List<UserQuestJobProto> updatedJobs = new List<UserQuestJobProto>();
		UserQuestJobProto userJob;
		int numMonsters;
		foreach (MSFullQuest item in currQuests)
		{
			if (item.userQuest.isComplete)
			{
				continue;
			}
			updatedJobs.Clear();
			foreach (var job in item.quest.jobs) 
			{
				switch (job.questJobType) {
				case QuestJobProto.QuestJobType.COMPLETE_TASK:
					if (dungeon.taskId == job.staticDataId)
					{
						userJob = item.userQuest.userQuestJobs.Find(x=>x.questJobId==job.questJobId);
						userJob.isComplete = ++userJob.progress >= job.quantity;
						updatedJobs.Add(userJob);
					}
					break;
				case QuestJobProto.QuestJobType.KILL_SPECIFIC_MONSTER:
					numMonsters = 0;
					foreach (var taskStage in dungeon.tsp) {
						foreach (var taskStageMonster in taskStage.stageMonsters) 
						{
							if (taskStageMonster.monsterId == job.staticDataId)
							{
								numMonsters++;
							}
						}
					}
					if (numMonsters > 0)
					{
						userJob = item.userQuest.userQuestJobs.Find(x=>x.questJobId==job.questJobId);
						userJob.progress = Mathf.Max(userJob.progress + numMonsters, job.quantity);
						userJob.isComplete = userJob.progress >= job.quantity;
						updatedJobs.Add(userJob);
					}
					break;
				case QuestJobProto.QuestJobType.KILL_MONSTER_IN_CITY:
					if (MSWhiteboard.cityID == job.staticDataId)
					{
						numMonsters = 0;
						foreach (var taskStage in dungeon.tsp) 
						{
							numMonsters += taskStage.stageMonsters.Count;
						}
						
						userJob = item.userQuest.userQuestJobs.Find(x=>x.questJobId==job.questJobId);
						userJob.progress = Mathf.Max(userJob.progress + numMonsters, job.quantity);
						userJob.isComplete = userJob.progress >= job.quantity;
						updatedJobs.Add(userJob);
					}
					break;
				case QuestJobProto.QuestJobType.COLLECT_SPECIAL_ITEM:
					numMonsters = 0;
					foreach (var taskStage in dungeon.tsp) {
						foreach (var taskStageMonster in taskStage.stageMonsters) 
						{
							if (taskStageMonster.itemId == job.staticDataId)
							{
								numMonsters++;
							}
						}
					}
					if (numMonsters > 0)
					{
						userJob = item.userQuest.userQuestJobs.Find(x=>x.questJobId==job.questJobId);
						userJob.progress = Mathf.Max(userJob.progress + numMonsters, job.quantity);
						userJob.isComplete = userJob.progress >= job.quantity;
						updatedJobs.Add(userJob);
					}
					break;
				default:
					break;
				}
				if (updatedJobs.Count > 0)
				{
					UpdateQuestProgress(item, updatedJobs);
				}
			}
		}
	}

	public bool AttemptDonation(MSFullQuest quest)
	{
		/* TODO: Fix for jobs
		List<PZMonster> matchingMonsters = MSMonsterManager.instance.GetMonstersByMonsterId(quest.quest.staticDataId);
		if (matchingMonsters.Count >= quest.quest.quantity)
		{
			List<PZMonster> donateMonsters = new List<PZMonster>();
			PZMonster curr;
			while (donateMonsters.Count < quest.quest.quantity)
			{
				curr = null;
				foreach (var item in matchingMonsters) 
				{
					if (!donateMonsters.Contains(item) && (curr == null || item.userMonster.currentLvl < curr.userMonster.currentLvl))
					{
						curr = item;
					}
				}
				donateMonsters.Add(curr);
			}

			quest.userQuest.progress = quest.quest.quantity;
			quest.userQuest.isComplete = true;

			UpdateQuestProgress(quest, donateMonsters);

			foreach (var item in donateMonsters) 
			{
				MSMonsterManager.instance.RemoveMonster(item.userMonster.userMonsterId);
			}
			return true;
		}
		*/
		return false;
	}

	void OnMonsterDonated(int monsterId)
	{
		foreach (MSFullQuest item in currQuests) {
			if (item.userQuest.isComplete)
			{
				continue;
			}
			/* TODO: Fix for jobs
			if (item.quest.questType == FullQuestProto.QuestType.DONATE_MONSTER && item.quest.staticDataId == monsterId)
			{
				item.userQuest.progress++;
				UpdateQuestProgress(item);
			}
			*/
		}
	}
}
