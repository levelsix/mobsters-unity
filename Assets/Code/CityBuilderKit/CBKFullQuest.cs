using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKFullQuest {

	public FullUserQuestDataLargeProto userQuest;
	
	public FullQuestProto quest;
	
	public CBKFullQuest(FullQuestProto quest)
	{
		this.quest = quest;
	}
	
	public CBKFullQuest(FullQuestProto quest, FullUserQuestDataLargeProto userQuest)
	{
		this.quest = quest;
		this.userQuest = userQuest;
	}
	
	public string GetProgressString()
	{
		return userQuest.numComponentsComplete + "/" + quest.numComponentsForGood;
	}
}
