using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSChatGrid : MonoBehaviour {
	
	public SortedList<long, MSChatBubble> bubbles = new SortedList<long, MSChatBubble>();
	
	const float SPACE_BETWEEN_MESSAGES = 15f;
	
	[SerializeField]
	MSChatBubble rightBubblePrefab;

	[SerializeField]
	MSChatBubble leftBubblePrefab;

	[SerializeField]
	MSResetsPosition posResetter;

	MSChatTable table;

	void Awake()
	{
		table = GetComponent<MSChatTable>();
	}

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
		foreach (MSChatBubble item in bubbles.Values) 
		{
			item.Pool ();
		}
		bubbles.Clear ();
	}

	MSChatBubble CreateBubble(int senderId, long timeSent)
	{
		MSChatBubble bub = MSPoolManager.instance.Get(leftBubblePrefab,
		                           Vector3.zero, transform) as MSChatBubble;
		bub.transf.localScale = Vector3.one;
		bub.name = (long.MaxValue - timeSent).ToString();
		bubbles.Add(timeSent, bub);
		foreach (var widget in bub.GetComponentsInChildren<UIWidget>()) 
		{
			widget.ParentHasChanged();
		}
		return bub;
	}
	
	public void SpawnBubbles(SortedList<long, PrivateChatPostProto> messages)
	{
		gameObject.SetActive(true);
		
		RecycleBubbles ();

		if (messages != null)
		{
			foreach (KeyValuePair<long, PrivateChatPostProto> item in messages) 
			{
				MSChatBubble bub = CreateBubble(item.Value.poster.minUserProto.userId, item.Key);
				bub.Init(item.Value);
			}
		}
		
		table.animateSmoothly = false;
		table.Reposition();
		
		posResetter.Reset();
	}
	
	public void SpawnBubbles(SortedList<long, GroupChatMessageProto> messages)
	{
		gameObject.SetActive(true);

		RecycleBubbles ();

		foreach (KeyValuePair<long, GroupChatMessageProto> item in messages) 
		{
			MSChatBubble bub = CreateBubble(item.Value.sender.minUserProto.userId, item.Key);
			bub.Init(item.Value);
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
	
}
