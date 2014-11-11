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
	
	public MSValues.ChatMode currMode = MSValues.ChatMode.GLOBAL;
	
	List<GroupChatMessageProto> globalChat = new List<GroupChatMessageProto>();
	List<GroupChatMessageProto> clanChat = new List<GroupChatMessageProto>();
	Dictionary<int, List<PrivateChatPostProto>> privateChats = new Dictionary<int, List<PrivateChatPostProto>>();

	public bool hasPrivateChats
	{
		get
		{
			return privateChats.Count > 0;
		}
	}

	void Awake()
	{
		instance = this;
	}
	
	public void Init(StartupResponseProto startup)
	{
		foreach (GroupChatMessageProto item in startup.globalChats) 
		{
			//Debug.Log("Global Chat message: From: " + item.sender.minUserProto.name + "\n" + item.content);
			globalChat.Add(item);
		}
		globalChat.Sort ((a,b) => a.timeOfChat.CompareTo(b.timeOfChat));

		foreach (GroupChatMessageProto item in startup.clanChats)
		{
			clanChat.Add(item);
		}
		clanChat.Sort ((a,b) => a.timeOfChat.CompareTo(b.timeOfChat));

		foreach (PrivateChatPostProto item in startup.pcpp) 
		{
			AddPrivateChat(item);
		}
	}

	void AddPrivateChat(PrivateChatPostProto item)
	{
		//Debug.LogWarning("Private chat\nSender: " + item.poster + "\nMessage: " + item.content);
		MinimumUserProtoWithLevel otherPlayer = (item.poster.minUserProto.userId == MSWhiteboard.localUser.userId) ? item.recipient : item.poster;
		if (!privateChats.ContainsKey(otherPlayer.minUserProto.userId))
		{
			privateChats.Add(otherPlayer.minUserProto.userId, new List<PrivateChatPostProto>());
		}
		
		privateChats[otherPlayer.minUserProto.userId].Add(item);
		privateChats[otherPlayer.minUserProto.userId].Sort((a,b) => a.timeOfPost.CompareTo(b.timeOfPost));
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
			MSClanHelpManager.instance.ReinitChat();
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
				LoadAllMessagesForPrivateChat(otherUser, privateChats[otherUser.minUserProto.userId][0]);
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

		switch (proto.scope)
		{
		case GroupChatScope.GLOBAL:
			globalChat.Add(groupMessage);
			globalChat.Sort((a,b) => a.timeOfChat.CompareTo(b.timeOfChat));
			break;
		case GroupChatScope.CLAN:
			clanChat.Add(groupMessage);
			clanChat.Sort ((a,b) => a.timeOfChat.CompareTo(b.timeOfChat));
			break;
		}

		if (MSActionManager.UI.OnGroupChatReceived != null)
		{
			MSActionManager.UI.OnGroupChatReceived(proto);
		}
	}
}
