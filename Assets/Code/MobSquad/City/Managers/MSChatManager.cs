using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSChatManager : MonoBehaviour {
	
	public static MSChatManager instance;
	
	[SerializeField]
	MSChatGrid chatGrid;
	
	public MSValues.ChatMode currMode = MSValues.ChatMode.GLOBAL;
	
	static SortedList<long, GroupChatMessageProto> globalChat = new SortedList<long, GroupChatMessageProto>();
	static SortedList<long, GroupChatMessageProto> clanChat = new SortedList<long, GroupChatMessageProto>();
	
	void Awake()
	{
		instance = this;
	}
	
	public void Init(StartupResponseProto startup)
	{
		foreach (GroupChatMessageProto item in startup.globalChats) 
		{
			//Debug.Log("Global Chat message: From: " + item.sender.minUserProto.name + "\n" + item.content);
			globalChat.Add(item.timeOfChat, item);
		}

		foreach (GroupChatMessageProto item in startup.clanChats)
		{
			clanChat.Add(item.timeOfChat, item);
		}
		
		//SetChatMode(CBKValues.ChatMode.GLOBAL);
	}
	
	public void SetChatMode(MSValues.ChatMode mode)
	{
		currMode = mode;
		switch (mode) {
		case MSValues.ChatMode.GLOBAL:
			chatGrid.SpawnBubbles(globalChat);
			break;
		case MSValues.ChatMode.CLAN:
			chatGrid.SpawnBubbles(clanChat);
			break;
		default:
			break;
		}
	}

	public void ReceiveGroupChatMessage(ReceivedGroupChatResponseProto proto)
	{
		GroupChatMessageProto groupMessage = new GroupChatMessageProto();
		groupMessage.sender = proto.sender;
		groupMessage.content = proto.chatMessage;
		groupMessage.timeOfChat = MSUtil.timeNowMillis;
		groupMessage.isAdmin = proto.isAdmin;

		globalChat.Add(MSUtil.timeNowMillis, groupMessage);

		if (MSActionManager.UI.OnGroupChatReceived != null)
		{
			MSActionManager.UI.OnGroupChatReceived(proto);
		}
	}
	
	public void ReceiveGroupChatMessage(GroupChatMessageProto message)
	{
		globalChat.Add(message.timeOfChat, message);
	}
}
