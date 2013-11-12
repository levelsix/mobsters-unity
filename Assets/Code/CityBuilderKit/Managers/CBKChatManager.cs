using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKChatManager : MonoBehaviour {
	
	public static CBKChatManager instance;
	
	[SerializeField]
	CBKChatGrid chatGrid;
	
	CBKValues.ChatMode currMode = CBKValues.ChatMode.GLOBAL;
	
	SortedList<long, GroupChatMessageProto> globalChat = new SortedList<long, GroupChatMessageProto>();
	SortedList<long, GroupChatMessageProto> clanChat = new SortedList<long, GroupChatMessageProto>();
	
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
	
	public void ReceiveGroupChatMessage(GroupChatMessageProto message)
	{
		
	}
}
