using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKChatGrid : MonoBehaviour {
	
	public SortedList<long, CBKChatBubble> bubbles = new SortedList<long, CBKChatBubble>();
	
	const float SPACE_BETWEEN_MESSAGES = 15f;
	
	[SerializeField]
	CBKChatBubble bubblePrefab;

	UITable table;
	
	void Awake()
	{
		table = GetComponent<UITable>();
	}

	void OnEnable()
	{
		CBKEventManager.UI.OnGroupChatReceived += SpawnBubbleFromNewMessage;
	}

	void OnDisable()
	{
		CBKEventManager.UI.OnGroupChatReceived -= SpawnBubbleFromNewMessage;
	}
	
	public void SpawnBubbles(SortedList<long, GroupChatMessageProto> messages)
	{
		foreach (CBKChatBubble item in bubbles.Values) 
		{
			item.Pool();
		}

		foreach (KeyValuePair<long, GroupChatMessageProto> item in messages) 
		{
			CBKChatBubble bub = CBKPoolManager.instance.Get(bubblePrefab, Vector3.zero) as CBKChatBubble;
			bub.transf.parent = transform;
			bub.transf.localScale = Vector3.one;
			bub.Init(item.Value);
			bubbles.Add(-item.Key, bub);
		}

		table.Reposition();
	}

	public void SpawnBubbleFromNewMessage(ReceivedGroupChatResponseProto proto)
	{
		CBKChatBubble bub = CBKPoolManager.instance.Get(bubblePrefab, Vector3.zero) as CBKChatBubble;
		bub.transf.parent = transform;
		bub.transf.localScale = Vector3.one;
		bub.Init (proto);
		bubbles.Add(-CBKUtil.timeNowMillis, bub);

		table.Reposition();
	}
	
}
