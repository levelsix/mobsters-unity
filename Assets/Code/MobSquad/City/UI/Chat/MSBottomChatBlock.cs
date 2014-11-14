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

	[SerializeField]
	UISprite bottomChatBox;

	UIWidget myWidget;

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

	void AddMessage(int avatarId, string senderName, string messageContent, Color color)
	{
		if (myWidget == null) myWidget = GetComponent<UIWidget>();
		myWidget.width = bottomChatBox.width;

		MSBottomChatMessage message = (MSPoolManager.instance.Get(bottomChatMessagePrefab.GetComponent<MSSimplePoolable>(),
		                                                         Vector3.zero,
		                                                          grid.transform) as MSSimplePoolable).GetComponent<MSBottomChatMessage>();
		message.transform.localScale = Vector3.one;
		message.Init(avatarId, senderName, messageContent, myWidget.width);
		message.GetComponent<MSUIHelper>().ResetAlpha(true);
		message.color = color;
		messages.Add(message);
		grid.Reposition();

		if (noChats != null)
		{
			noChats.SetActive(false);
		}

		CheckOverfull();

		MSBottomChat.instance.AlertDot(chatMode);
	}

	public void AddMessage(GroupChatMessageProto message)
	{
		AddMessage(message.sender.minUserProto.avatarMonsterId, message.sender.minUserProto.name,
		           message.content, Color.white);
	}

	void AddMessage(PrivateChatPostProto message)
	{
		AddMessage(message.poster.minUserProto.avatarMonsterId, message.poster.minUserProto.name,
		           message.content, Color.white);
	}

	void AddMessage(MSClanHelpListing listing)
	{
		AddMessage(listing.avatarId, listing.name, listing.description, Color.yellow);
	}

	public void AddMessage(MSChatBubble bubble)
	{
		if(bubble is MSHelpBubble)
		{
			AddMessage(bubble.avatarID, bubble.senderLabel.text, bubble.textLabel.text, Color.yellow);
		}
		else
		{
			AddMessage(bubble.avatarID, bubble.senderLabel.text, bubble.textLabel.text, Color.white);
		}
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
			           message.chatMessage, Color.white);
		}
	}

	void OnPrivateChatReceived(PrivateChatPostResponseProto message)
	{
		if (chatMode == MSValues.ChatMode.PRIVATE)
		{
			AddMessage(message.sender.avatarMonsterId, message.sender.name, message.post.content, Color.white);
		}
	}

	/// <summary>
	/// Adds new solicited help after it's been compressed in the clan help manager.
	/// </summary>
	/// <param name="listing">This is a reference to the help listing in the clan menu</param>
	public void AddSolicitedHelp(MSClanHelpListing listing)
	{
		AddMessage(listing);
	}
}
