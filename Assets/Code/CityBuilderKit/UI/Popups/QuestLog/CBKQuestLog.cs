﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBK quest log.
/// Only handles the visuals of the quest log popup
/// All Quest management handled by CBKQuestManager
/// </summary>
public class CBKQuestLog : MonoBehaviour {
	
	[SerializeField]
	CBKQuestEntry questEntryPrefab;
	
	[SerializeField]
	CBKQuestTaskEntry questTaskEntryPrefab;
	
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
	CBKVisitCityButton visitButton;

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
	
	List<CBKQuestEntry> quests = new List<CBKQuestEntry>();
	
	#endregion

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

	CBKFullQuest currQuest;
	
	void OnEnable()
	{
		CBKEventManager.UI.OnQuestEntryClicked += OnQuestEntryClicked;
		Init ();
	}
	
	void OnDisable()
	{
		CBKEventManager.UI.OnQuestEntryClicked -= OnQuestEntryClicked;

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

		while(quests.Count < CBKQuestManager.questDict.Count)
		{
			CBKQuestEntry entry = CBKPoolManager.instance.Get(questEntryPrefab, Vector3.zero) as CBKQuestEntry;
			entry.trans.parent = questGridParentTrans;
			entry.GetComponent<UIDragObject>().target = questGridParentTrans;
			quests.Add(entry);
		}

		int i = 0;
		foreach (CBKFullQuest item in CBKQuestManager.questDict.Values) 
		{
			quests[i].Init(item);
			i++;
		}

		for (; i < quests.Count; i++) 
		{
			quests[i].gameObj.SetActive(false);
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
		questGiver.GetComponent<CBKUIHelper>().FadeOut();
	}
	
	
	void LoadQuestDetails(CBKFullQuest fullQ)
	{
		header.text = fullQ.quest.name;

		questDescription.text = fullQ.quest.description;
		
		WriteTask(fullQ);

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
	
	void WriteTask(CBKFullQuest fullQ)
	{
		taskDescription.text = fullQ.quest.jobDescription;

		taskProgress.text = fullQ.progressString;

		if (fullQ.userQuest.progress < fullQ.quest.quantity)
		{
			visitButton.button.enabled = true;
			visitButton.cityID = fullQ.quest.cityId;
			completeLabel.text = " ";
		}
		else
		{
			visitButton.button.enabled = false;
			completeLabel.text = "Complete!";
		}

	}

	int prizeSize = 110;
	int buffer = 5;
	
	void BuildRewards(CBKFullQuest fullQ)
	{
		int numPrizes = 0;
		
		if (fullQ.quest.diamondReward > 0)
		{
			if (rewards.Count < numPrizes+1)
			{
				AddReward();
			}
			rewards[numPrizes].InitDiamond(fullQ.quest.diamondReward);
			numPrizes++;
		}

		if (fullQ.quest.coinReward > 0)
		{
			if (rewards.Count < numPrizes+1)
			{
				AddReward();
			}
			rewards[numPrizes].InitCash (fullQ.quest.coinReward);
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
		CBKQuestManager.instance.CompleteQuest(currQuest);
		if (shareCheck.value)
		{
			//TODO: Share quest
		}
		ReturnToList();
	}
	
	public void OnQuestEntryClicked(CBKFullQuest quest)
	{
		currQuest = quest;

		LoadQuestDetails(quest);
		
		questGiver.GetComponent<TweenPosition>().PlayForward();
		questGiver.GetComponent<CBKUIHelper>().FadeIn();
		questGiver.sprite2D = CBKAtlasUtil.instance.GetSprite("Quest/HD/" + CBKUtil.StripExtensions(quest.quest.questGiverImageSuffix) + "Big");

		GetComponent<TweenPosition>().PlayForward();

		TweenPosition.Begin(questGridParentGob, TWEEN_TIME, LEFT_POS + new Vector3(0, questGridParentTrans.localPosition.y));
		questGridParentTrans.parent.GetComponent<UIScrollView>().restrictWithinPanel = false;

		TweenPosition.Begin(detailsParent, TWEEN_TIME, Vector3.zero);
		
		backButton.enabled = true;
	}
	
}
