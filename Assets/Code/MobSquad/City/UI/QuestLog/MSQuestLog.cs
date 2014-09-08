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
	MSQuestRewardBox rewardBoxPrefab;

	[SerializeField]
	MSAchievementEntry achievementEntryPrefab;

	[SerializeField]
	TweenPosition mover;
	
	#region List Mode Members

	[SerializeField]
	MSUIHelper questHelper;

	[SerializeField]
	MSTab listTab;

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

	#region Achievements

	[SerializeField]
	MSTab achievementTab;

	[SerializeField]
	MSUIHelper achievementHelper;
	
	public UIGrid achievementGrid;

	#endregion

	List<MSQuestRewardBox> rewards = new List<MSQuestRewardBox>();
	
	List<MSQuestEntry> quests = new List<MSQuestEntry>();

	List<MSQuestTaskEntry> tasks = new List<MSQuestTaskEntry>();

	public List<MSAchievementEntry> achievements = new List<MSAchievementEntry>();
	
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
		TabToAchievements();
		//SetupQuestList();
		InitAchievements();
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

	public void TabToList()
	{
		listTab.InitActive();
		questHelper.TurnOn();

		achievementTab.InitInactive();
		achievementHelper.TurnOff();
	}

	public void TabToAchievements()
	{
		//listTab.InitInactive();
		//questHelper.TurnOff();
		
		achievementTab.InitActive();
		achievementHelper.TurnOn();
		achievementGrid.Reposition();
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
		taskEntry.Init(quest, userJob, num+1);

		foreach (var item in taskEntry.GetComponentsInChildren<UIWidget>()) 
		{
			item.MarkAsChanged();
		}
	}

	void LoadQuestDetails(MSFullQuest fullQ)
	{
		if (fullQ.newQuest)
		{
			fullQ.newQuest = false;
			MSQuestManager.instance.questBadge.notifications--;
			MSQuestManager.instance.jobsBadge.notifications--;
		}

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
	
	MSQuestRewardBox AddReward()
	{
		MSQuestRewardBox reward = (MSPoolManager.instance.Get(rewardBoxPrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, rewardGrid.transform) 
		                  as MSSimplePoolable).GetComponent<MSQuestRewardBox>();
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

		if (fullQ.quest.oilReward > 0)
		{
			AddReward().InitOil(fullQ.quest.oilReward);
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

	#region Achievements

	void RecycleAchievements()
	{
		foreach (var item in achievements) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}
		achievements.Clear();
	}

	public MSAchievementEntry AddAchievement(MSFullAchievement fullA)
	{
		MSAchievementEntry entry = (MSPoolManager.instance.Get(MSPrefabList.instance.achievementEntry.GetComponent<MSSimplePoolable>(),
		                                                      Vector3.zero, achievementGrid.transform)
		                            as MSSimplePoolable).GetComponent<MSAchievementEntry>();
		entry.transform.localScale = Vector3.one;
		achievements.Add(entry);
		entry.Init(fullA);

		foreach (var item in entry.GetComponentsInChildren<UIWidget>()) 
		{
			item.ParentHasChanged();
		}

		return entry;
	}

	void InitAchievements()
	{
		RecycleAchievements();
		foreach (var item in MSAchievementManager.instance.currAchievements) 
		{
			if (MSAchievementManager.instance.currAchievements.Find(x=>x.achievement.achievementId == item.achievement.prerequisiteId) == null)
			{
				AddAchievement(item);
			}
		}
		achievementGrid.animateSmoothly = false;
		achievementGrid.Reposition();
	}

	#endregion

	public void OnQuestEntryClicked(MSFullQuest quest)
	{
		LoadQuestDetails(quest);

		mover.PlayForward();

		tabHelper.FadeOutAndOff();
		topDetailHelper.TurnOn();
		topDetailHelper.FadeIn();

		StartCoroutine(FixOnMoveFinish());
	}

	IEnumerator FixOnMoveFinish()
	{
		while (mover.tweenFactor < 1)
		{
			yield return null;
		}
		foreach (var item in tasks) 
		{
			item.DoneMoving();
		}
	}
	
}
