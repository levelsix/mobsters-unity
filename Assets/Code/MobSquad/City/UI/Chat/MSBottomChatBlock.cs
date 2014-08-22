using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSBottomChatBlock : MonoBehaviour {

	public MSValues.ChatMode chatMode;

	[SerializeField]
	MSBottomChatMessage bottomChatMessagePrefab;

	[SerializeField]
	UIGrid grid;

	[SerializeField]
	int maxMessages = 2;

	[SerializeField]
	GameObject noChats;

	List<MSBottomChatMessage> messages = new List<MSBottomChatMessage>();

	void Awake()
	{
		MSActionManager.UI.OnGroupChatReceived += OnGroupChatReceived;
		MSActionManager.UI.OnPrivateChatReceived += OnPrivateChatReceived;
		MSActionManager.Loading.OnStartup += OnStartup;
	}

	void OnDestroy()
	{
		MSActionManager.UI.OnGroupChatReceived -= OnGroupChatReceived;
		MSActionManager.UI.OnPrivateChatReceived -= OnPrivateChatReceived;
		MSActionManager.Loading.OnStartup -= OnStartup;
	}

	void AddMessage(int avatarId, string senderName, string messageContent)
	{
		MSBottomChatMessage message = (MSPoolManager.instance.Get(bottomChatMessagePrefab.GetComponent<MSSimplePoolable>(),
		                                                         Vector3.zero,
		                                                          grid.transform) as MSSimplePoolable).GetComponent<MSBottomChatMessage>();
		message.transform.localScale = Vector3.one;
		message.Init(avatarId, senderName, messageContent);
		message.GetComponent<MSUIHelper>().ResetAlpha(true);
		messages.Add(message);
		grid.Reposition();

		if (noChats != null)
		{
			noChats.SetActive(false);
		}

		CheckOverfull();

		MSBottomChat.instance.AlertDot(chatMode);
	}

	void AddMessage(GroupChatMessageProto message)
	{
		AddMessage(message.sender.minUserProto.avatarMonsterId, message.sender.minUserProto.name,
		           message.content);
	}

	void AddMessage(PrivateChatPostProto message)
	{
		AddMessage(message.poster.minUserProto.avatarMonsterId, message.poster.minUserProto.name,
		           message.content);
	}

	/// <summary>
	/// If we have more than the max messages, fade out the oldest one
	/// </summary>
	void CheckOverfull()
	{
		while (messages.Count > maxMessages)
		{
			messages[0].GetComponent<MSUIHelper>().FadeOutAndPool();
			messages.RemoveAt(0);
		}
	}

	void OnStartup(StartupResponseProto response)
	{
		switch(chatMode)
		{
		case MSValues.ChatMode.GLOBAL:
			if (response.globalChats.Count > 1)
			{
				AddMessage(response.globalChats[response.globalChats.Count-2]);
				AddMessage(response.globalChats[response.globalChats.Count-1]);
			}
			else if (response.globalChats.Count > 0)
			{
				AddMessage(response.globalChats[response.globalChats.Count-1]);
			}
			break;
		case MSValues.ChatMode.CLAN:
			if (response.clanChats.Count > 1)
			{
				AddMessage(response.clanChats[response.clanChats.Count-2]);
				AddMessage(response.clanChats[response.clanChats.Count-1]);
			}
			else if (response.clanChats.Count > 0)
			{
				AddMessage(response.clanChats[response.clanChats.Count-1]);
			}
			break;
		case MSValues.ChatMode.PRIVATE:
			if (response.pcpp.Count > 1)
			{
				AddMessage(response.pcpp[response.pcpp.Count-2]);
				AddMessage(response.pcpp[response.pcpp.Count-1]);
			}
			else if (response.pcpp.Count > 0)
			{
				AddMessage(response.pcpp[response.pcpp.Count-1]);
			}
			break;
		}

		if (noChats != null)
		{
			noChats.SetActive(messages.Count == 0);
		}
	}

	void OnGroupChatReceived(ReceivedGroupChatResponseProto message)
	{
		if ((message.scope == GroupChatScope.CLAN && chatMode == MSValues.ChatMode.CLAN)
		    || (message.scope == GroupChatScope.GLOBAL && chatMode == MSValues.ChatMode.GLOBAL))
		{
			AddMessage(message.sender.minUserProto.avatarMonsterId, message.sender.minUserProto.name,
			           message.chatMessage);
		}
	}

	void OnPrivateChatReceived(PrivateChatPostResponseProto message)
	{
		if (chatMode == MSValues.ChatMode.PRIVATE)
		{
			AddMessage(message.sender.avatarMonsterId, message.sender.name, message.post.content);
		}
	}
}
