using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBK quest log.
/// Only handles the visuals of the quest log popup
/// All Quest management handled by CBKQuestManager
/// </summary>
public class MSQuestLog : MonoBehaviour {
	
	[SerializeField]
	MSQuestEntry questEntryPrefab;
	
	[SerializeField]
	MSQuestTaskEntry questTaskEntryPrefab;
	
	[SerializeField]
	PZPrize rewardBoxPrefab;
	
	#region List Mode Members
	
	[SerializeField]
	Transform questGridParentTrans;
	
	[SerializeField]
	GameObject questGridParentGob;
	
	#endregion
	
	#region Details Mode Members
	
	[SerializeField]
	GameObject detailsParent;

	[SerializeField]
	UILabel taskDescription;

	[SerializeField]
	UILabel taskProgress;

	[SerializeField]
	MSActionButton visitButton;

	[SerializeField]
	UILabel completeLabel;

	[SerializeField]
	Transform rewardHeader;
	
	[SerializeField]
	UILabel questGiverSaysLabel;
	
	[SerializeField]
	UILabel questDescription;
	
	[SerializeField]
	UIButton backButton;

	[SerializeField]
	UILabel header;

	[SerializeField]
	UIToggle shareCheck;

	[SerializeField]
	GameObject completeItems;

	[SerializeField]
	UI2DSprite questGiver;

	List<PZPrize> rewards = new List<PZPrize>();
	
	List<MSQuestEntry> quests = new List<MSQuestEntry>();
	
	#endregion
	
	int prizeSize = 110;
	int buffer = 5;

	const string QUEST_LIST_HEADER = "Quests";
	
	const float TWEEN_TIME = 0.4f;
	
	static readonly Vector3 LEFT_POS = new Vector3(-760, 0);
	static readonly Vector3 RIGHT_POS = new Vector3(760, 0);

	public Vector3 REWARD_HEADER_CENTER;
	public Vector3 REWARD_HEADER_COMPLETE_OFFSET;
	
	const float START_TASK_OFFSET = -75;
	
	const float OFFSET_PER_TASK = -100;
	
	const float BASE_REWARD_OFFSET = -22;
	
	const float REWARD_BOX_X_OFFSET = 120;
	
	float lastTaskOffset;

	MSFullQuest currQuest;
	
	void OnEnable()
	{
		MSActionManager.UI.OnQuestEntryClicked += OnQuestEntryClicked;
		Init ();
	}
	
	void OnDisable()
	{
		MSActionManager.UI.OnQuestEntryClicked -= OnQuestEntryClicked;

	}
	
	void Init()
	{
		SetupQuestList();
	}
	
	public void SetupQuestList()
	{
		header.text = QUEST_LIST_HEADER;

		GetComponent<TweenPosition>().value = Vector3.zero;
		questGridParentTrans.localPosition = new Vector3(0, questGridParentTrans.localPosition.y);
		questGridParentTrans.parent.GetComponent<UIScrollView>().restrictWithinPanel = true;
		detailsParent.transform.localPosition = RIGHT_POS;

		while(quests.Count < MSQuestManager.instance.currQuests.Count)
		{
			MSQuestEntry entry = MSPoolManager.instance.Get(questEntryPrefab, Vector3.zero) as MSQuestEntry;
			entry.trans.parent = questGridParentTrans;
			entry.GetComponent<UIDragObject>().target = questGridParentTrans;
			quests.Add(entry);
		}

		int i = 0;
		foreach (MSFullQuest item in MSQuestManager.instance.currQuests) 
		{
			quests[i].Init(item);
			i++;
		}

		for (; i < quests.Count; i++) 
		{
			quests[i].gameObj.SetActive(false);
		}
		foreach (var item in quests) 
		{
			item.GetComponent<UIDragObject>().restrictWithinPanel = true;
		}

		questGridParentTrans.GetComponentInChildren<UIGrid>().Reposition();
		
		backButton.enabled = false;
	}
	
	public void ReturnToList()
	{	
		GetComponent<TweenPosition>().PlayReverse();
		TweenPosition.Begin(questGridParentGob, TWEEN_TIME, new Vector3(0, questGridParentTrans.localPosition.y));
		TweenPosition.Begin(detailsParent, TWEEN_TIME, RIGHT_POS);
		questGridParentTrans.parent.GetComponent<UIScrollView>().restrictWithinPanel = true;
		questGiver.GetComponent<TweenPosition>().PlayReverse();
		questGiver.GetComponent<MSUIHelper>().FadeOut();
		foreach (var item in quests) 
		{
			item.GetComponent<UIDragObject>().restrictWithinPanel = true;
		}
	}
	
	
	void LoadQuestDetails(MSFullQuest fullQ)
	{
		header.text = fullQ.quest.name;

		questDescription.text = fullQ.quest.description;

		foreach (var job in fullQ.quest.jobs) 
		{
			Debug.Log("Job " + job.questJobId + ": " + job.description);
		}

		foreach (var userJob in fullQ.userQuest.userQuestJobs) 
		{
			Debug.Log ("Userjob: " + userJob.questJobId);
			WriteTask(fullQ.quest.jobs.Find(x=>x.questJobId==userJob.questJobId), userJob);
		}

		BuildRewards(fullQ);
	}
	
	void AddReward()
	{
		PZPrize reward = Instantiate(rewardBoxPrefab) as PZPrize;
		rewards.Add (reward);
		reward.transform.parent = rewardHeader;
		reward.transform.localScale = Vector3.one;
		reward.transform.localPosition = new Vector3(0, 0);
	}

	void WriteTask(QuestJobProto job, UserQuestJobProto userJob)
	{
		Debug.Log(job);
		Debug.Log(userJob);
		taskDescription.text = job.description;

		taskProgress.text = userJob.progress + "/" + job.quantity;

		if (userJob.progress < job.quantity)
		{
			visitButton.button.enabled = true;
			if (job.questJobType == QuestJobProto.QuestJobType.DONATE_MONSTER)
			{
				completeLabel.text = "Donate";
			}
			else
			{
				completeLabel.text = "Visit";
			}
			visitButton.icon.spriteName = "confirm";
		}
		else
		{
			visitButton.button.enabled = false;
			completeLabel.text = "[90E200]Complete![-]";
			visitButton.icon.spriteName = "complete";
		}

	}

	public void OnClickTaskButton()
	{
		/*
		if (currQuest.quest.questType == FullQuestProto.QuestType.DONATE_MONSTER)
		{
			if (MSQuestManager.instance.AttemptDonation(currQuest))
			{
				//WriteTask(currQuest);
				BuildRewards(currQuest);
				quests.Find(x=>x.fullQuest == currQuest).Init(currQuest);
			}

		}
		else
		{
			if (currQuest.quest.cityId >= 1)
			{
				GoToTown();
			}
			else
			{
				GoHome();
			}
		}
		*/
	}

	void GoToTown(int cityId)
	{
		MSWhiteboard.currCityType = MSWhiteboard.CityType.NEUTRAL;
		MSWhiteboard.cityID = cityId;
		MSActionManager.Loading.LoadBuildings();
		MSActionManager.Popup.CloseAllPopups();
	}
	
	void GoHome()
	{	
		MSWhiteboard.currCityType = MSWhiteboard.CityType.PLAYER;
		MSWhiteboard.cityID = MSWhiteboard.localMup.userId;
		MSActionManager.Loading.LoadBuildings();	
		MSActionManager.Popup.CloseAllPopups();
	}



	
	void BuildRewards(MSFullQuest fullQ)
	{
		int numPrizes = 0;
		
		if (fullQ.quest.gemReward > 0)
		{
			if (rewards.Count < numPrizes+1)
			{
				AddReward();
			}
			rewards[numPrizes].InitDiamond(fullQ.quest.gemReward);
			numPrizes++;
		}

		if (fullQ.quest.cashReward > 0)
		{
			if (rewards.Count < numPrizes+1)
			{
				AddReward();
			}
			rewards[numPrizes].InitCash (fullQ.quest.cashReward);
			numPrizes++;
		}

		if (fullQ.quest.expReward > 0)
		{
			if (rewards.Count < numPrizes+1)
			{
				AddReward();
			}
			rewards[numPrizes].InitXP (fullQ.quest.expReward);
			numPrizes++;
		}

		if (fullQ.quest.monsterIdReward > 0)
		{
			if (rewards.Count < numPrizes+1)
			{
				AddReward();
			}
			rewards[numPrizes].InitEnemy (fullQ.quest.monsterIdReward);
			numPrizes++;
		}

		float spaceNeeded = numPrizes * prizeSize + (numPrizes-1) * buffer;
		for (int i = 0; i < numPrizes; i++) 
		{
			rewards[i].gameObject.SetActive(true);
			rewards[i].transform.localPosition = new Vector3(i * (prizeSize + buffer) - spaceNeeded/2, 0, 0);
		}

		for (int i = numPrizes; i < rewards.Count; i++) 
		{
			rewards[i].gameObject.SetActive(false);
		}

		if (fullQ.userQuest.isComplete)
		{
			completeItems.SetActive(true);
			shareCheck.value = true;
			rewardHeader.transform.localPosition = REWARD_HEADER_COMPLETE_OFFSET;
		}
		else
		{
			completeItems.SetActive(false);
			rewardHeader.transform.localPosition = REWARD_HEADER_CENTER;
		}
	}

	public void CollectQuest()
	{
		MSQuestManager.instance.CompleteQuest(currQuest);
		if (shareCheck.value)
		{
			//TODO: Share quest
		}
		SetupQuestList();
		ReturnToList();
	}
	
	public void OnQuestEntryClicked(MSFullQuest quest)
	{
		currQuest = quest;

		LoadQuestDetails(quest);
		
		questGiver.GetComponent<TweenPosition>().PlayForward();
		questGiver.GetComponent<MSUIHelper>().FadeIn();
		questGiver.sprite2D = MSSpriteUtil.instance.GetSprite("Quest/HD/" + MSUtil.StripExtensions(quest.quest.questGiverImagePrefix) + "Big");

		GetComponent<TweenPosition>().PlayForward();

		TweenPosition.Begin(questGridParentGob, TWEEN_TIME, LEFT_POS + new Vector3(0, questGridParentTrans.localPosition.y));

		questGridParentTrans.parent.GetComponent<UIScrollView>().restrictWithinPanel = false;
		foreach (var item in quests) 
		{
			item.GetComponent<UIDragObject>().restrictWithinPanel = false;
		}

		TweenPosition.Begin(detailsParent, TWEEN_TIME, Vector3.zero);
		
		backButton.enabled = true;
	}
	
}
