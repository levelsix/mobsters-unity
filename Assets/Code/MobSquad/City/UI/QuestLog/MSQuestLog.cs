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

	[SerializeField]
	TweenPosition mover;
	
	#region List Mode Members

	[SerializeField]
	UIGrid questGrid;

	[SerializeField]
	MSUIHelper tabHelper;
	
	#endregion
	
	#region Details Mode Members

	[SerializeField]
	MSResetsPosition taskScrollResetter;

	[SerializeField]
	UIGrid taskGrid;

	[SerializeField]
	UIGrid rewardGrid;
	
	[SerializeField]
	UILabel questGiverName;
	
	[SerializeField]
	UILabel questDescription;
	
	[SerializeField]
	UIButton backButton;

	[SerializeField]
	UILabel questName;

	[SerializeField]
	UISprite questGiverBG;

	[SerializeField]
	UI2DSprite questGiverThumb;

	[SerializeField]
	MSUIHelper topDetailHelper;

	#endregion

	List<PZPrize> rewards = new List<PZPrize>();
	
	List<MSQuestEntry> quests = new List<MSQuestEntry>();

	List<MSQuestTaskEntry> tasks = new List<MSQuestTaskEntry>();
	
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

	void RecycleQuests()
	{
		foreach (var item in quests) 
		{
			item.Pool();
		}
		quests.Clear();
	}

	void AddQuest(MSFullQuest quest)
	{
		MSQuestEntry entry = (MSPoolManager.instance.Get(questEntryPrefab.GetComponent<MSSimplePoolable>(), 
		                                                 Vector3.zero, questGrid.transform) 
		                      	as MSSimplePoolable).GetComponent<MSQuestEntry>();
		entry.transform.localScale = Vector3.one;
		entry.Init (quest);
		quests.Add (entry);
	}
	
	public void SetupQuestList()
	{
		topDetailHelper.ResetAlpha(false);
		topDetailHelper.TurnOff();

		tabHelper.ResetAlpha (true);
		tabHelper.TurnOn();

		mover.Sample(0, true);

		RecycleQuests();
		foreach (MSFullQuest item in MSQuestManager.instance.currQuests) 
		{
			AddQuest(item);
		}

		questGrid.Reposition();
	}
	
	public void ReturnToList()
	{	
		mover.PlayReverse();
		topDetailHelper.FadeOutAndOff();
		tabHelper.TurnOn();
		tabHelper.FadeIn();
	}

	#region Quest Details

	void RecycleTasks()
	{
		foreach (var item in tasks) 
		{
			item.Pool();
		}
		tasks.Clear();
	}
	
	void AddTask(MSFullQuest quest, UserQuestJobProto userJob, int num)
	{
		MSQuestTaskEntry taskEntry = (MSPoolManager.instance.Get(questTaskEntryPrefab.GetComponent<MSSimplePoolable>(), 
		                                                         Vector3.zero, taskGrid.transform) 
		                              	as MSSimplePoolable).GetComponent<MSQuestTaskEntry>();
		taskEntry.transform.localScale = Vector3.one;
		tasks.Add (taskEntry);
		taskEntry.Init(quest, userJob, num);
	}

	void LoadQuestDetails(MSFullQuest fullQ)
	{
		questName.text = fullQ.quest.name;

		questDescription.text = fullQ.quest.description;

		RecycleTasks();
		for (int i = 0; i < fullQ.userQuest.userQuestJobs.Count; i++) 
		{
			AddTask(fullQ, fullQ.userQuest.userQuestJobs[i], i);
		}
		taskGrid.Reposition();
		taskScrollResetter.Reset();

		BuildRewards(fullQ);
	}

	#region Rewards
	
	PZPrize AddReward()
	{
		PZPrize reward = (MSPoolManager.instance.Get(rewardBoxPrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, rewardGrid.transform) 
		                  as MSSimplePoolable).GetComponent<PZPrize>();
		rewards.Add (reward);
		reward.transform.localScale = Vector3.one;
		return reward;
	}

	void RecycleRewards()
	{
		foreach (var item in rewards) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}
		rewards.Clear ();
	}

	void BuildRewards(MSFullQuest fullQ)
	{
		RecycleRewards();
		
		if (fullQ.quest.gemReward > 0)
		{
			AddReward().InitDiamond(fullQ.quest.gemReward);
		}

		if (fullQ.quest.cashReward > 0)
		{
			AddReward().InitCash (fullQ.quest.cashReward);
		}

		if (fullQ.quest.expReward > 0)
		{
			AddReward().InitXP (fullQ.quest.expReward);
		}

		if (fullQ.quest.monsterIdReward > 0)
		{
			AddReward().InitEnemy (fullQ.quest.monsterIdReward);
		}

		rewardGrid.Reposition();
	}

           	#endregion

	#endregion

	public void OnQuestEntryClicked(MSFullQuest quest)
	{
		currQuest = quest;

		LoadQuestDetails(quest);

		mover.PlayForward();

		tabHelper.FadeOutAndOff();
		topDetailHelper.TurnOn();
		topDetailHelper.FadeIn();
	}
	
}
