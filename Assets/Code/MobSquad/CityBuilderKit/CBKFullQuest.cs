using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKFullQuest {

	public FullUserQuestProto userQuest;
	
	public FullQuestProto quest;

	public string progressString
	{
		get
		{
			return userQuest.progress + "/" + quest.quantity;
		}
	}
	
	public CBKFullQuest(FullQuestProto quest)
	{
		this.quest = quest;
	}
	
	public CBKFullQuest(FullQuestProto quest, FullUserQuestProto userQuest)
	{
		this.quest = quest;
		this.userQuest = userQuest;
	}
	
	public string GetProgressString()
	{
		return userQuest.progress + "/" + quest.quantity;
	}
}
