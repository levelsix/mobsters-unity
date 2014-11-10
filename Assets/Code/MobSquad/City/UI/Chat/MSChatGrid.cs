using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class MSChatGrid : MonoBehaviour {
	
	public List<MSChatBubble> bubbles = new List<MSChatBubble>();
	
	const float SPACE_BETWEEN_MESSAGES = 15f;
	
	[SerializeField]
	MSChatBubble rightBubblePrefab;

	[SerializeField]
	MSChatBubble leftBubblePrefab;

	[SerializeField]
	MSChatBubble MSHelpBubble;

	[SerializeField]
	MSResetsPosition posResetter;

	[SerializeField]
	MSChatTable table;

	void OnEnable()
	{
		MSActionManager.UI.OnGroupChatReceived += SpawnBubbleFromNewMessage;
		MSActionManager.UI.OnPrivateChatReceived += SpawnBubbleFromPrivateMessage;
	}

	void OnDisable()
	{
		MSActionManager.UI.OnGroupChatReceived -= SpawnBubbleFromNewMessage;
		MSActionManager.UI.OnPrivateChatReceived -= SpawnBubbleFromPrivateMessage;
	}

	void RecycleBubbles ()
	{
		foreach (MSChatBubble item in bubbles) 
		{
			item.Pool ();
		}
		bubbles.Clear ();
	}

	MSChatBubble CreateHelpBubble(ClanHelpProto helpProto)
	{
		MSChatBubble bub = MSPoolManager.instance.Get<MSChatBubble>(MSHelpBubble, transform);
		bub.transform.localPosition = Vector3.zero;
		bub.transform.localScale = Vector3.one;

		bub.name = (long.MaxValue - helpProto.timeRequested).ToString();
		bubbles.Add(bub);
		bubbles.Sort((a,b) => a.timeSent.CompareTo(b.timeSent));

		return bub;
	}

	MSChatBubble CreateBubble(int senderId, long timeSent)
	{
		MSChatBubble bub = MSPoolManager.instance.Get(senderId == MSWhiteboard.localUser.userId ? rightBubblePrefab : leftBubblePrefab,
		                           Vector3.zero, transform) as MSChatBubble;
		bub.transf.localScale = Vector3.one;
		bub.name = (long.MaxValue - timeSent).ToString();
		bubbles.Add(bub);
		bubbles.Sort((a,b) => a.timeSent.CompareTo(b.timeSent));
		foreach (var widget in bub.GetComponentsInChildren<UIWidget>()) 
		{
			widget.ParentHasChanged();
		}
		return bub;
	}
	
	public void SpawnBubbles(List<PrivateChatPostProto> messages)
	{
		gameObject.SetActive(true);
		
		RecycleBubbles ();

		if (messages != null)
		{
			foreach (PrivateChatPostProto item in messages) 
			{
				MSChatBubble bub = CreateBubble(item.poster.minUserProto.userId, item.timeOfPost);
				bub.Init(item);
			}
		}
		
		table.animateSmoothly = false;
		table.Reposition();
		
		posResetter.Reset();
	}
	
	public void SpawnBubbles(List<GroupChatMessageProto> messages)
	{
		gameObject.SetActive(true);

		RecycleBubbles ();

		foreach (GroupChatMessageProto item in messages) 
		{
			MSChatBubble bub = CreateBubble(item.sender.minUserProto.userId, item.timeOfChat);
			bub.Init(item);
		}

		table.animateSmoothly = false;
		table.Reposition();

		posResetter.Reset();
	}

	public void SpawnBubbleFromPrivateMessage(PrivateChatPostResponseProto proto)
	{
		if (MSChatManager.instance.currMode == MSValues.ChatMode.PRIVATE 
		    && (proto.sender.userId == MSChatManager.instance.chatPopup.privateChatter.minUserProto.userId
		    || proto.post.recipient.minUserProto.userId == MSChatManager.instance.chatPopup.privateChatter.minUserProto.userId))
		{
			MSChatBubble bub = CreateBubble(proto.sender.userId, MSUtil.timeNowMillis);
			bub.Init (proto.post);
			
			table.animateSmoothly = true;
			table.Reposition();
		}
	}

	public void SpawnBubbleFromNewMessage(ReceivedGroupChatResponseProto proto)
	{
		if ((proto.scope == GroupChatScope.GLOBAL && MSChatManager.instance.currMode == MSValues.ChatMode.GLOBAL) 
		    || proto.scope == GroupChatScope.CLAN && MSChatManager.instance.currMode == MSValues.ChatMode.CLAN)
		{
			MSChatBubble bub = CreateBubble(proto.sender.minUserProto.userId, MSUtil.timeNowMillis);
			bub.Init (proto);
			
			table.animateSmoothly = true;
			table.Reposition();
		}
	}

	public void SpawnBubbleForCheat(string cheatMessage)
	{

	}
	
}
