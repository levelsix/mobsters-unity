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
	}

	void OnDisable()
	{
		MSActionManager.UI.OnGroupChatReceived -= SpawnBubbleFromNewMessage;
	}
	
	public void SpawnBubbles(SortedList<long, GroupChatMessageProto> messages)
	{
		foreach (MSChatBubble item in bubbles.Values) 
		{
			item.Pool();
		}

		bubbles.Clear();

		foreach (KeyValuePair<long, GroupChatMessageProto> item in messages) 
		{
			MSChatBubble bub = MSPoolManager.instance.Get(item.Value.sender.minUserProto.userId == MSWhiteboard.localMup.userId ? rightBubblePrefab : leftBubblePrefab,
			                                              Vector3.zero) as MSChatBubble;
			bub.transf.parent = transform;
			bub.transf.localScale = Vector3.one;
			bub.Init(item.Value);
			bub.name = (long.MaxValue - item.Key).ToString();
			bubbles.Add(item.Key, bub);
			foreach (var widget in bub.GetComponentsInChildren<UIWidget>()) {
				widget.ParentHasChanged();
			}
		}

		table.animateSmoothly = false;
		table.Reposition();

		posResetter.Reset();
	}

	public void SpawnBubbleFromNewMessage(ReceivedGroupChatResponseProto proto)
	{
		MSChatBubble bub = MSPoolManager.instance.Get(
			proto.sender.minUserProto.userId == MSWhiteboard.localMup.userId ? rightBubblePrefab : leftBubblePrefab,
			Vector3.zero) as MSChatBubble;
		bub.transf.parent = transform;
		bub.transf.localScale = Vector3.one;
		bub.Init (proto);
		bub.name = (long.MaxValue - MSUtil.timeNowMillis).ToString();
		bubbles.Add(MSUtil.timeNowMillis, bub);
		foreach (var widget in bub.GetComponentsInChildren<UIWidget>()) {
			widget.ParentHasChanged();
		}
		
		table.animateSmoothly = true;
		table.Reposition();
	}
	
}
