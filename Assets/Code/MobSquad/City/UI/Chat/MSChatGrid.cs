using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class MSChatGrid : MonoBehaviour {

	[SerializeField]
	MSValues.ChatMode gridType;
	
	public List<MSChatBubble> bubbles = new List<MSChatBubble>();
	
	const float SPACE_BETWEEN_MESSAGES = 15f;
	
	[SerializeField]
	MSChatBubble rightBubblePrefab;

	[SerializeField]
	MSChatBubble leftBubblePrefab;

	[SerializeField]
	MSHelpBubble helpBubble;

	[SerializeField]
	MSResetsPosition posResetter;

	[SerializeField]
	public MSChatTable table;

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

	//oldest message in index 0 newest in index count-1
	public void SortBubbles()
	{
		bubbles.Sort((x,y)=>string.Compare(y.gameObject.name, x.gameObject.name));
	}

	void RecycleBubbles ()
	{
		foreach (MSChatBubble item in bubbles) 
		{
			item.Pool ();
		}
		bubbles.Clear ();
	}

	public void RemoveAllHelpBubbles()
	{
		for(int i =0; i < bubbles.Count; i++)
		{
			if(bubbles[i] is MSHelpBubble)
			{
				bubbles.Remove(bubbles[i]);
				i--;
			}
		}
	}

	public bool RemoveHelpBubble(MSClanHelpListing listing)
	{
		bool removedListing = false;
		for(int i =0; i < bubbles.Count; i++)
		{
			if(bubbles[i] is MSHelpBubble)
			{
				if(bubbles[i].GetComponent<MSClanHelpListing>() == listing)
				{
					bubbles.Remove(bubbles[i]);
					i--;
					removedListing = true;
				}
			}
		}

		table.animateSmoothly = true;
		table.Reposition();

		return removedListing;
	}

	MSHelpBubble CreateHelpBubble(ClanHelpProto helpProto)
	{
		MSHelpBubble bub = MSPoolManager.instance.Get<MSHelpBubble>(helpBubble, transform);
		bub.transform.localPosition = Vector3.zero;
		bub.transform.localScale = Vector3.one;

		bub.name = (long.MaxValue - helpProto.timeRequested).ToString();
		bubbles.Add(bub);
		bubbles.Sort((a,b) => a.timeSent.CompareTo(b.timeSent));
		foreach (var widget in bub.GetComponentsInChildren<UIWidget>()) 
		{
			widget.ParentHasChanged();
		}
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
	
	public void SpawnBubbles(List<PrivateChatPostProto> messages, bool activateChat = true)
	{
		gameObject.SetActive(activateChat);
		
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
	
	public void SpawnBubbles(List<GroupChatMessageProto> messages, bool activateChat = true)
	{
		gameObject.SetActive(activateChat);

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
		if (gridType == MSValues.ChatMode.PRIVATE 
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
		if ((proto.scope == GroupChatScope.GLOBAL && gridType == MSValues.ChatMode.GLOBAL) 
		    || proto.scope == GroupChatScope.CLAN && gridType == MSValues.ChatMode.CLAN)
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

	public MSClanHelpListing SpawnBubbleForHelp(ClanHelpProto clanHelp, bool animate)
	{
		MSHelpBubble bub = CreateHelpBubble(clanHelp);
		bub.Init(clanHelp);
		table.animateSmoothly = animate;
		table.Reposition();

		return bub.GetComponent<MSClanHelpListing>();
	}
}
