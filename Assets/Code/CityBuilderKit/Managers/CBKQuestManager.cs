#define DEUBG3
#define DEBUG1
#define DEBUG2

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKQuestManager : MonoBehaviour {
	
	public static CBKQuestManager instance;
	
	static Dictionary<int, FullQuestProto> tempQuests;
	
	public static Dictionary<int, CBKFullQuest> questDict = new Dictionary<int, CBKFullQuest>();

	public static Dictionary<int, bool> taskDict = new Dictionary<int, bool>();
	
	public void Awake()
	{
		instance = this;
	}
	
	public void OnEnable()
	{
		CBKEventManager.Quest.OnStructureBuilt += OnStructureBuilt;
		CBKEventManager.Quest.OnStructureUpgraded += OnStructureUpgraded;
		CBKEventManager.Quest.OnTaskCompleted += OnTaskCompleted;
		CBKEventManager.Quest.OnMoneyCollected += OnMoneyCollected;
		CBKEventManager.Quest.OnMonsterDefeated += OnEnemyDefeated;
		CBKEventManager.Quest.OnMonsterDonated += OnMonsterDonated;
	}
	
	public void Disable()
	{
		CBKEventManager.Quest.OnStructureBuilt -= OnStructureBuilt;
		CBKEventManager.Quest.OnStructureUpgraded -= OnStructureUpgraded;
		CBKEventManager.Quest.OnTaskCompleted -= OnTaskCompleted;
		CBKEventManager.Quest.OnMoneyCollected -= OnMoneyCollected;
		CBKEventManager.Quest.OnMonsterDefeated -= OnEnemyDefeated;
		CBKEventManager.Quest.OnMonsterDonated -= OnMonsterDonated;
	}
	
	public void Init(StartupResponseProto proto)
	{
		tempQuests = new Dictionary<int, FullQuestProto>();
		foreach (FullQuestProto item in proto.staticDataStuffProto.availableQuests) 
		{
#if DEBUG2
			Debug.Log("Available Quest: " + item.questId);		
#endif
			StartCoroutine(AcceptQuest(item));
		}
		foreach (var item in proto.userQuests) 
		{
			questDict[item.questId] = new CBKFullQuest(CBKDataManager.instance.Get(typeof(FullQuestProto), item.questId) as FullQuestProto, item);
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
		foreach (FullTaskProto item in CBKDataManager.instance.GetAll(typeof(FullTaskProto)).Values)
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
		request.sender = CBKWhiteboard.localMup;
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
		foreach (KeyValuePair<int, CBKFullQuest> item in questDict) 
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
		
		tempQuests.Add(fullQuest.questId, fullQuest);
		
		QuestAcceptRequestProto request = new QuestAcceptRequestProto();
		request.sender = CBKWhiteboard.localMup;
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
			userQuest.userId = CBKWhiteboard.localMup.userId;
			userQuest.questId = fullQuest.questId;
			userQuest.isRedeemed = false;
			switch(fullQuest.questType)
			{
			case FullQuestProto.QuestType.BUILD_STRUCT:

				break;
			case FullQuestProto.QuestType.UPGRADE_STRUCT:

				break;
			default:

				break;
			}
		}
		
		/*
		UserQuestDetailsRequestProto detailRequest = new UserQuestDetailsRequestProto();
		detailRequest.sender = CBKWhiteboard.localMup;
		detailRequest.questId = fullQuest.questId;
		
		UMQNetworkManager.instance.SendRequest(detailRequest, (int)EventProtocolRequest.C_USER_QUEST_DETAILS_EVENT, LoadUserQuestDetails);
		*/
	}
	
	void CheckQuest(CBKFullQuest fullQuest)
	{
		
#if DEBUG3
		Debug.Log("Checking quest: " + fullQuest.quest.name);
#endif
		fullQuest.userQuest.isComplete = (fullQuest.userQuest.progress >= fullQuest.quest.quantity);
		
	}

	public void CompleteQuest(CBKFullQuest quest)
	{
		StartCoroutine(RedeemQuest(quest.quest));
	}
	
	//TODO
	IEnumerator RedeemQuest(FullQuestProto quest)
	{
#if DEBUG3
		Debug.Log("Redeeming quest: " + quest.name);
#endif
		yield return null;
		
		questDict.Remove(quest.questId);
		
		QuestRedeemRequestProto request = new QuestRedeemRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.questId = quest.questId;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_QUEST_REDEEM_EVENT, null);
		
		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		QuestRedeemResponseProto response = UMQNetworkManager.responseDict[tagNum] as QuestRedeemResponseProto;
		
		if (response.status != QuestRedeemResponseProto.QuestRedeemStatus.SUCCESS)
		{
			//TODO: Reload?
			Debug.LogError("Problem redeeming quest: " + response.status.ToString());
		}
		else
		{
			if (response.fump != null)
			{
				//TODO: Get that equip bro
			}
			if (quest.coinReward > 0)
			{
				CBKResourceManager.instance.Collect(ResourceType.CASH, quest.coinReward);
			}
			if (quest.diamondReward > 0)
			{
				CBKResourceManager.instance.Collect(ResourceType.GEMS, quest.diamondReward);
			}
			if (quest.expReward > 0)
			{
				CBKResourceManager.instance.GainExp(quest.expReward);
			}
			foreach (FullQuestProto item in response.newlyAvailableQuests) 
			{
				StartCoroutine(AcceptQuest(item));
			}
		}
	}
	
	void OnStructureBuilt(int structID)
	{
		foreach (CBKFullQuest item in questDict.Values) 
		{
			if (item.userQuest.isComplete)
			{
				continue;
			}
			if (item.quest.questType == FullQuestProto.QuestType.BUILD_STRUCT && item.quest.staticDataId == structID) 
			{
				item.userQuest.progress++;
				CheckQuest(item);
			}
		}
	}

	void OnStructureUpgraded(int structID, int level)
	{
		foreach (CBKFullQuest item in questDict.Values)
		{
			if (item.userQuest.isComplete)
			{
				continue;
			}
			if (item.quest.questType == FullQuestProto.QuestType.UPGRADE_STRUCT && item.quest.staticDataId == structID)
			{
				if (item.userQuest.progress < level)
				{
					item.userQuest.progress = level;
					CheckQuest(item);
				}
			}
		}
	}
	
	void OnTaskCompleted(int taskID)
	{
		foreach (CBKFullQuest item in questDict.Values)
		{
			if (item.userQuest.isComplete)
			{
				continue;
			}
			if (item.quest.questType == FullQuestProto.QuestType.COMPLETE_TASK && item.quest.staticDataId == taskID)
			{
				item.userQuest.progress++;
				CheckQuest(item);
			}
		}
	}
	
	void OnMoneyCollected(int amount)
	{
		foreach (CBKFullQuest item in questDict.Values)
		{
			if (item.userQuest.isComplete)
			{
				continue;
			}
			if (item.quest.questType == FullQuestProto.QuestType.COLLECT_COINS_FROM_HOME)
			{
				item.userQuest.progress += amount;
				CheckQuest(item);
			}
		}
	}

	void OnEnemyDefeated(int monsterId)
	{
		foreach (CBKFullQuest item in questDict.Values) {
			if (item.userQuest.isComplete)
			{
				continue;
			}
			if (item.quest.questType == FullQuestProto.QuestType.KILL_MONSTER && item.quest.staticDataId == monsterId)
			{
				item.userQuest.progress++;
				CheckQuest(item);
			}
		}
	}

	void OnMonsterDonated(int monsterId)
	{
		foreach (CBKFullQuest item in questDict.Values) {
			if (item.userQuest.isComplete)
			{
				continue;
			}
			if (item.quest.questType == FullQuestProto.QuestType.DONATE_MONSTER && item.quest.staticDataId == monsterId)
			{
				item.userQuest.progress++;
				CheckQuest(item);
			}
		}
	}
}
