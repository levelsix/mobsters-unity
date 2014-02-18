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
public class CBKQuestLog : MonoBehaviour {
	
	[SerializeField]
	CBKQuestEntry questEntryPrefab;
	
	[SerializeField]
	CBKQuestTaskEntry questTaskEntryPrefab;
	
	[SerializeField]
	CBKQuestRewardBox rewardBoxPrefab;
	
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

	List<CBKQuestRewardBox> rewards = new List<CBKQuestRewardBox>();
	
	List<CBKQuestEntry> quests = new List<CBKQuestEntry>();
	
	#endregion

	const string QUEST_LIST_HEADER = "Quests";
	
	const float TWEEN_TIME = 0.4f;
	
	static readonly Vector3 LEFT_POS = new Vector3(-760, 0);
	static readonly Vector3 RIGHT_POS = new Vector3(760, 0);
	
	const float START_TASK_OFFSET = -75;
	
	const float OFFSET_PER_TASK = -100;
	
	const float BASE_REWARD_OFFSET = -22;
	
	const float REWARD_BOX_Y_OFFSET = -90;
	
	const float REWARD_BOX_X_OFFSET = 120;
	
	float lastTaskOffset;
	
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
		TweenPosition.Begin(questGridParentGob, TWEEN_TIME, new Vector3(0, questGridParentTrans.localPosition.y));
		TweenPosition.Begin(detailsParent, TWEEN_TIME, RIGHT_POS);
		questGridParentTrans.parent.GetComponent<UIScrollView>().restrictWithinPanel = true;
	}
	
	
	void LoadQuestDetails(CBKFullQuest fullQ)
	{
		header.text = fullQ.quest.name;

		questDescription.text = fullQ.quest.description;
		
		WriteTask(fullQ);

		BuildRewards(fullQ);
	}
	
	CBKQuestRewardBox GetReward()
	{
		CBKQuestRewardBox reward = CBKPoolManager.instance.Get(rewardBoxPrefab, Vector3.zero) as CBKQuestRewardBox;
		rewards.Add (reward);
		reward.transf.parent = rewardHeader;
		reward.transf.localScale = Vector3.one;
		reward.transf.localPosition = new Vector3(0, REWARD_BOX_Y_OFFSET);
		
		return reward;
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
	
	void BuildRewards(CBKFullQuest fullQ)
	{
		foreach (CBKQuestRewardBox item in rewards) 
		{
			item.Pool();
		}
		
		rewards.Clear();
		
		CBKQuestRewardBox box;
		if (fullQ.quest.coinReward > 0)
		{
			box = GetReward();
			box.Init (CBKQuestRewardBox.RewardType.MONEY, fullQ.quest.coinReward);
		}
		if (fullQ.quest.expReward > 0)
		{
			box = GetReward();
			box.Init (CBKQuestRewardBox.RewardType.EXP, fullQ.quest.expReward);
		}
		
		if(rewards.Count == 2)
		{
			rewards[0].transf.localPosition = new Vector3(-REWARD_BOX_X_OFFSET/2, REWARD_BOX_Y_OFFSET, 0);
			rewards[1].transf.localPosition = new Vector3( REWARD_BOX_X_OFFSET/2, REWARD_BOX_Y_OFFSET, 0);
		}
		else if (rewards.Count == 3)
		{
			rewards[0].transf.localPosition = new Vector3(-REWARD_BOX_X_OFFSET, REWARD_BOX_Y_OFFSET, 0);
			rewards[2].transf.localPosition = new Vector3( REWARD_BOX_X_OFFSET, REWARD_BOX_Y_OFFSET, 0);
		}
	}
	
	public void OnQuestEntryClicked(CBKFullQuest quest)
	{
		LoadQuestDetails(quest);
		TweenPosition.Begin(questGridParentGob, TWEEN_TIME, LEFT_POS + new Vector3(0, questGridParentTrans.localPosition.y));
		questGridParentTrans.parent.GetComponent<UIScrollView>().restrictWithinPanel = false;

		TweenPosition.Begin(detailsParent, TWEEN_TIME, Vector3.zero);
		
		backButton.enabled = true;
	}
	
}
