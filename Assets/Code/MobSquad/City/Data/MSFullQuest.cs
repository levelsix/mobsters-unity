using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[System.Serializable]
public class MSFullQuest {

	public bool newQuest;

	public FullUserQuestProto userQuest;
	
	public FullQuestProto quest;

	public string progressString
	{
		get
		{
			return userQuest.userQuestJobs.FindAll(x=>x.isComplete).Count + "/" + quest.jobs.Count;
		}
	}
	
	public MSFullQuest(FullQuestProto quest)
	{
		this.quest = quest;
	}
	
	public MSFullQuest(FullQuestProto quest, FullUserQuestProto userQuest, bool newQuest)
	{
		this.quest = quest;
		this.userQuest = userQuest;
		this.newQuest = newQuest;

		if (!newQuest)
		{
			UserQuestJobProto userjob;
			foreach (var job in quest.jobs) 
			{
				if (userQuest.userQuestJobs.Find(x=>x.questJobId == job.questJobId) == null)
				{
					userjob = new UserQuestJobProto();
					userjob.questId = quest.questId;
					userjob.questJobId = job.questJobId;
					userjob.isComplete = false;
					userjob.progress = 0;
					
					userQuest.userQuestJobs.Add(userjob);
				}
			}
		}
	}
	
	public string GetProgressString()
	{
		return "?/?";
		//return userQuest.progress + "/" + quest.quantity;
	}
}
