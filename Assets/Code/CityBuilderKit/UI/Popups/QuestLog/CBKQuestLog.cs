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
	Transform taskHeader;
	
	[SerializeField]
	Transform rewardHeader;
	
	[SerializeField]
	UILabel questGiverSaysLabel;
	
	[SerializeField]
	UILabel questDescription;
	
	[SerializeField]
	UIButton backButton;
	
	
	List<CBKQuestTaskEntry> tasks = new List<CBKQuestTaskEntry>();
	
	List<CBKQuestRewardBox> rewards = new List<CBKQuestRewardBox>();
	
	List<CBKQuestEntry> quests = new List<CBKQuestEntry>();
	
	#endregion
	
	const float TWEEN_TIME = 0.8f;
	
	static readonly Vector3 LEFT_POS = new Vector3(-720, 0);
	static readonly Vector3 RIGHT_POS = new Vector3(720, 0);
	
	const float START_TASK_OFFSET = -75;
	
	const float OFFSET_PER_TASK = -100;
	
	const float BASE_REWARD_OFFSET = -22;
	
	const float REWARD_BOX_Y_OFFSET = -100;
	
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
		foreach (CBKQuestEntry item in quests) {
			item.Pool();
		}
	}
	
	void Init()
	{
		SetupQuestList();
	}
	
	public void SetupQuestList()
	{
		ReturnToList();
		
		foreach (CBKFullQuest item in CBKQuestManager.questDict.Values) 
		{
			CBKQuestEntry entry = CBKPoolManager.instance.Get(questEntryPrefab, Vector3.zero) as CBKQuestEntry;
			entry.trans.parent = questGridParentTrans;
			entry.Init(item);
			quests.Add(entry);
		}
		
		backButton.enabled = false;
	}
	
	public void ReturnToList()
	{
		questGridParentGob.SetActive(true);
		
		TweenPosition.Begin(questGridParentGob, TWEEN_TIME, Vector3.zero);
		TweenPosition.Begin(detailsParent, TWEEN_TIME, RIGHT_POS);
	}
	
	
	void LoadQuestDetails(CBKFullQuest fullQ)
	{
		questDescription.text = fullQ.quest.description;
		
		BuildTasks(fullQ);
		
		rewardHeader.transform.localPosition = new Vector3(0, BASE_REWARD_OFFSET + lastTaskOffset);
		
		BuildRewards(fullQ);
	}
	
	CBKQuestTaskEntry GetTask()
	{
		CBKQuestTaskEntry entry = CBKPoolManager.instance.Get(questTaskEntryPrefab, Vector3.zero) as CBKQuestTaskEntry;
		tasks.Add (entry);
		entry.transf.parent = taskHeader;
		entry.transf.localScale = Vector3.one;
		lastTaskOffset += OFFSET_PER_TASK;
		entry.transf.localPosition = new Vector3(0, lastTaskOffset, 0);
		
		return entry;
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
	
	void BuildTasks(CBKFullQuest fullQ)
	{
		foreach (CBKQuestTaskEntry item in tasks) {
			item.Pool();
		}
		
		tasks.Clear();
		
		lastTaskOffset = START_TASK_OFFSET - OFFSET_PER_TASK; //Hacky hacky hack hack
		
		CBKQuestTaskEntry task;
		/*
		if (fullQ.quest.coinRetrievalReq > 0)
		{
			task = GetTask();
			task.InitMoneyCollect(fullQ.userQuest.coinsRetrievedForReq, fullQ.quest.coinRetrievalReq);
		}
		foreach (var item in fullQ.userQuest.requiredTasksProgress) 
		{
			task = GetTask ();
			task.Init(item);
		}
		foreach (var item in fullQ.userQuest.requiredBuildStructJobProgress) 
		{
			task = GetTask ();
			task.Init(item);
		}
		foreach (var item in fullQ.userQuest.requiredUpgradeStructJobProgress) 
		{
			task = GetTask ();
			task.Init(item);
		}
		*/
		
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
			rewards[0].transf.localPosition = new Vector3(-REWARD_BOX_X_OFFSET, REWARD_BOX_Y_OFFSET, 0);
			rewards[1].transf.localPosition = new Vector3( REWARD_BOX_X_OFFSET, REWARD_BOX_Y_OFFSET, 0);
		}
		else if (rewards.Count == 3)
		{
			rewards[0].transf.localPosition = new Vector3(-REWARD_BOX_X_OFFSET*2, REWARD_BOX_Y_OFFSET, 0);
			rewards[2].transf.localPosition = new Vector3( REWARD_BOX_X_OFFSET*2, REWARD_BOX_Y_OFFSET, 0);
		}
	}
	
	public void OnQuestEntryClicked(CBKFullQuest quest)
	{
		LoadQuestDetails(quest);
		TweenPosition.Begin(questGridParentGob, TWEEN_TIME, LEFT_POS);
		
		TweenPosition.Begin(detailsParent, TWEEN_TIME, Vector3.zero);
		
		backButton.enabled = true;
		
		//LoadQuestDetails(quest);
	}
	
}
