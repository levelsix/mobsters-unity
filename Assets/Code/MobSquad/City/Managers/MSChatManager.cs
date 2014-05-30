using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSChatManager : MonoBehaviour {
	
	public static MSChatManager instance;
	
	[SerializeField]
	MSChatGrid chatGrid;

	[SerializeField]
	UIGrid privateChatsGrid;

	public MSChatPopup chatPopup;

	public MSChatBubbleOptions chatOptionsPrefab;
	
	public MSValues.ChatMode currMode = MSValues.ChatMode.GLOBAL;
	
	SortedList<long, GroupChatMessageProto> globalChat = new SortedList<long, GroupChatMessageProto>();
	SortedList<long, GroupChatMessageProto> clanChat = new SortedList<long, GroupChatMessageProto>();
	Dictionary<int, SortedList<long, PrivateChatPostProto>> privateChats = new Dictionary<int, SortedList<long, PrivateChatPostProto>>();
	
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

		foreach (PrivateChatPostProto item in startup.pcpp) 
		{
			AddPrivateChat(item);
		}
	}

	void AddPrivateChat(PrivateChatPostProto item)
	{
		Debug.LogWarning("Private chat\nSender: " + item.poster + "\nMessage: " + item.content);
		MinimumUserProtoWithLevel otherPlayer = (item.poster.minUserProto.userId == MSWhiteboard.localUser.userId) ? item.recipient : item.poster;
		if (!privateChats.ContainsKey(otherPlayer.minUserProto.userId))
		{
			privateChats.Add(otherPlayer.minUserProto.userId, new SortedList<long, PrivateChatPostProto>());
		}
		
		privateChats[otherPlayer.minUserProto.userId].Add(item.timeOfPost, item);
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
		case MSValues.ChatMode.PRIVATE:
			chatPopup.SetupPrivateChatListing(privateChats);
			break;
		default:
			break;
		}
	}

	public void MutePlayer(MinimumUserProtoWithLevel otherUser)
	{

	}

	public void GoToPrivateChat(MinimumUserProtoWithLevel otherUser)
	{
		currMode = MSValues.ChatMode.PRIVATE;
		if (!chatPopup.gameObject.activeSelf)
		{
			MSActionManager.Popup.OnPopup(chatPopup.GetComponent<MSPopup>());
		}
		if (privateChats.ContainsKey(otherUser.minUserProto.userId))
		{
			chatPopup.GoToPrivateChat(otherUser, privateChats[otherUser.minUserProto.userId]);
			if (privateChats[otherUser.minUserProto.userId].Count == 1)
			{
				LoadAllMessagesForPrivateChat(otherUser, privateChats[otherUser.minUserProto.userId].Values[0]);
			}
		}
		else
		{
			chatPopup.GoToPrivateChat(otherUser, null);
		}
	}

	void LoadAllMessagesForPrivateChat(MinimumUserProtoWithLevel otherUser, PrivateChatPostProto onlyChat)
	{
		RetrievePrivateChatPostsRequestProto request = new RetrievePrivateChatPostsRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.otherUserId = otherUser.minUserProto.userId;
		request.beforePrivateChatId = onlyChat.privateChatPostId;
	}

	void DealWithLoadingPrivateChats(int tagNum)
	{
		RetrievePrivateChatPostsResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetrievePrivateChatPostsResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == RetrievePrivateChatPostsResponseProto.RetrievePrivateChatPostsStatus.SUCCESS)
		{

		}
		else
		{
			Debug.LogError("Problem loading private chats: " + response.status.ToString());
		}
	}

	public void ReceivePrivateChatMessage(PrivateChatPostResponseProto proto)
	{
		AddPrivateChat(proto.post);
		if (MSActionManager.UI.OnPrivateChatReceived != null)
		{
			MSActionManager.UI.OnPrivateChatReceived(proto);
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
