using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKChatManager : MonoBehaviour {
	
	public static CBKChatManager instance;
	
	[SerializeField]
	CBKChatGrid chatGrid;
	
	CBKValues.ChatMode currMode = CBKValues.ChatMode.GLOBAL;
	
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
		
		//SetChatMode(CBKValues.ChatMode.GLOBAL);
	}
	
	public void SetChatMode(CBKValues.ChatMode mode)
	{
		switch (mode) {
		case CBKValues.ChatMode.GLOBAL:
			chatGrid.SpawnBubbles(globalChat);
			break;
		case CBKValues.ChatMode.CLAN:
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
		groupMessage.timeOfChat = CBKUtil.timeNowMillis;
		groupMessage.isAdmin = proto.isAdmin;

		globalChat.Add(CBKUtil.timeNowMillis, groupMessage);

		if (CBKEventManager.UI.OnGroupChatReceived != null)
		{
			CBKEventManager.UI.OnGroupChatReceived(proto);
		}
	}
	
	public void ReceiveGroupChatMessage(GroupChatMessageProto message)
	{
		globalChat.Add(message.timeOfChat, message);
	}
}
