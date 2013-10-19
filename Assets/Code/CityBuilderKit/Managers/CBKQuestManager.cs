#define DEUBG3
#define DEBUG1
#define DEBUG2

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKQuestManager : MonoBehaviour {
	
	public static CBKQuestManager instance;
	
	Dictionary<int, FullQuestProto> tempQuests;
	
	public Dictionary<int, CBKFullQuest> questDict = new Dictionary<int, CBKFullQuest>();
	
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
	}
	
	public void Disable()
	{
		CBKEventManager.Quest.OnStructureBuilt -= OnStructureBuilt;
		CBKEventManager.Quest.OnStructureUpgraded -= OnStructureUpgraded;
		CBKEventManager.Quest.OnTaskCompleted -= OnTaskCompleted;
		CBKEventManager.Quest.OnMoneyCollected -= OnMoneyCollected;
	}
	
	public void Init(StartupResponseProto proto, RetrieveStaticDataRequestProto dataRequest)
	{
		tempQuests = new Dictionary<int, FullQuestProto>();
		foreach (FullQuestProto item in proto.availableQuests) 
		{
#if DEBUG2
			Debug.Log("Available Quest: " + item.questId);		
#endif
			StartCoroutine(AcceptQuest(item));
			CBKDataManager.instance.BuildQuestDataToStaticDataRequest(item, dataRequest);
		}
		foreach (var item in proto.inProgressCompleteQuests) 
		{
			//RedeemQuest(item);
			tempQuests[item.questId] = item;
			CBKDataManager.instance.BuildQuestDataToStaticDataRequest(item, dataRequest);
#if DEBUG2
			Debug.Log ("In Progress, Complete Quest: " + item.questId);
#endif
		}
		foreach (var item in proto.inProgressIncompleteQuests) 
		{
			tempQuests[item.questId] = item;
			CBKDataManager.instance.BuildQuestDataToStaticDataRequest(item, dataRequest);
#if DEBUG2
			Debug.Log("In Progress, Incomplete Quest: " + item.questId);	
#endif
		}
		
		if (tempQuests.Count > 0)
		{
			UserQuestDetailsRequestProto request = new UserQuestDetailsRequestProto();
			request.sender = CBKWhiteboard.localMup;
			
			UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_USER_QUEST_DETAILS_EVENT, LoadUserQuestDetails);
		}
		
#if DEBUG1
		string debug = "Sending Quest Load: ";
		foreach (KeyValuePair<int, FullQuestProto> item in tempQuests) {
			debug += "\nQuest: " + item.Key + ", " + item.Value.name;
		}
		Debug.Log(debug);
#endif
		
	}
	
	void LoadUserQuestDetails(int tagNum)
	{
		Debug.Log("Loading User Quest Details");
		
		UserQuestDetailsResponseProto response = UMQNetworkManager.responseDict[tagNum] as UserQuestDetailsResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != UserQuestDetailsResponseProto.UserQuestDetailsStatus.SUCCESS)
		{
			Debug.LogError("Problem loading user quest details: " + response.status.ToString());
			return;
		}
		
		CBKFullQuest fullQuest;
		foreach (FullUserQuestDataLargeProto item in response.inProgressUserQuestData) 
		{
			if (tempQuests.ContainsKey(item.questId))
			{
				fullQuest = new CBKFullQuest(tempQuests[item.questId], item);
				questDict.Add(item.questId, fullQuest);
				tempQuests.Remove(item.questId);
				CheckQuest(fullQuest);
			}
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
		
		UserQuestDetailsRequestProto detailRequest = new UserQuestDetailsRequestProto();
		detailRequest.sender = CBKWhiteboard.localMup;
		detailRequest.questId = fullQuest.questId;
		
		UMQNetworkManager.instance.SendRequest(detailRequest, (int)EventProtocolRequest.C_USER_QUEST_DETAILS_EVENT, LoadUserQuestDetails);
		
	}
	
	void CheckQuest(CBKFullQuest fullQuest)
	{
		
#if DEBUG3
		Debug.Log("Checking quest: " + fullQuest.quest.name);
#endif
		if (fullQuest.userQuest.isComplete && !fullQuest.userQuest.isRedeemed)
		{
			StartCoroutine(RedeemQuest(fullQuest.quest));
			return;
		}
		if (fullQuest.userQuest.numComponentsComplete == fullQuest.quest.numComponentsForGood)
		{
			StartCoroutine(RedeemQuest(fullQuest.quest));
			return;
		}
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
			//TODO: Reload
			Debug.LogError("Problem redeeming quest: " + response.status.ToString());
		}
		else
		{
			if (response.monsterId > 0)
			{
				//TODO: Get that equip bro
			}
			if (quest.coinsGained > 0)
			{
				CBKResourceManager.instance.Collect(CBKResourceManager.ResourceType.FREE, quest.coinsGained);
			}
			if (quest.diamondsGained > 0)
			{
				CBKResourceManager.instance.Collect(CBKResourceManager.ResourceType.PREMIUM, quest.diamondsGained);
			}
			if (quest.expGained > 0)
			{
				CBKResourceManager.instance.GainExp(quest.expGained);
			}
			foreach (FullQuestProto item in response.newlyAvailableQuests) 
			{
				AcceptQuest(item);
			}
		}
	}
	
	void OnStructureBuilt(int structID)
	{
		foreach (CBKFullQuest item in questDict.Values) 
		{
			foreach (MinimumUserBuildStructJobProto job in item.userQuest.requiredBuildStructJobProgress) 
			{
				BuildStructJobProto buildJob = CBKDataManager.instance.Get(typeof(BuildStructJobProto), job.buildStructJobId) as BuildStructJobProto;
				if (buildJob.structId == structID && job.numOfStructUserHas < buildJob.quantityRequired)
				{
					if (++job.numOfStructUserHas == buildJob.quantityRequired) 
					{
						item.userQuest.numComponentsComplete++;
						CheckQuest(item);
					}
				}
			}
		}
	}

	void OnStructureUpgraded(int structID, int level)
	{
		foreach (CBKFullQuest item in questDict.Values)
		{
			foreach (MinimumUserUpgradeStructJobProto job in item.userQuest.requiredUpgradeStructJobProgress)
			{
				UpgradeStructJobProto upJob = CBKDataManager.instance.Get(typeof(UpgradeStructJobProto), job.upgradeStructJobId) as UpgradeStructJobProto;
				if (upJob.structId == structID && job.currentLevel < upJob.levelReq && level > job.currentLevel)
				{
					job.currentLevel = level;
					if (job.currentLevel == upJob.levelReq)
					{
						item.userQuest.numComponentsComplete++;
						CheckQuest(item);
					}
				}
			}
		}
	}
	
	void OnTaskCompleted(int taskID)
	{
		foreach (CBKFullQuest item in questDict.Values)
		{
			foreach (MinimumUserQuestTaskProto job in item.userQuest.requiredTasksProgress)
			{
				FullTaskProto task = CBKDataManager.instance.Get(typeof(FullTaskProto), job.taskId) as FullTaskProto;
				item.userQuest.numComponentsComplete++;
				CheckQuest(item);
			}
		}
	}
	
	void OnMoneyCollected(int amount)
	{
		foreach (CBKFullQuest item in questDict.Values)
		{
			if (item.userQuest.coinsRetrievedForReq < item.quest.coinRetrievalReq)
			{
				item.userQuest.coinsRetrievedForReq += amount;
				if (item.userQuest.coinsRetrievedForReq >= item.quest.coinRetrievalReq)
				{
					item.userQuest.numComponentsComplete++;
					CheckQuest(item);
				}
			}
		}
	}
}
