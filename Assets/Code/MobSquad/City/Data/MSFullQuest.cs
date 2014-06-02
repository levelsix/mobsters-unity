using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[System.Serializable]
public class MSFullQuest {

	public FullUserQuestProto userQuest;
	
	public FullQuestProto quest;

	public string progressString
	{
		get
		{
			return "?/?";
			//return userQuest.progress + "/" + quest.quantity;
		}
	}
	
	public MSFullQuest(FullQuestProto quest)
	{
		this.quest = quest;
	}
	
	public MSFullQuest(FullQuestProto quest, FullUserQuestProto userQuest)
	{
		this.quest = quest;
		this.userQuest = userQuest;
	}
	
	public string GetProgressString()
	{
		return "?/?";
		//return userQuest.progress + "/" + quest.quantity;
	}
}
