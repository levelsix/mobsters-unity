using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKChatGrid : UIGrid {
	
	public SortedList<long, CBKChatBubble> bubbles = new SortedList<long, CBKChatBubble>();
	
	const float SPACE_BETWEEN_MESSAGES = 15f;
	
	[SerializeField]
	CBKChatBubble bubblePrefab;
	
	void Awake()
	{
		myTrans = transform;
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
	}
	
	protected override void Position (ref int x, ref int y)
	{
		float runningHeight = startingHeight;
		foreach (CBKChatBubble bub in bubbles.Values) 
		{
			if (!NGUITools.GetActive(bub.gObj) && hideInactive) continue;
			
			bub.transf.localPosition = new Vector3(0, runningHeight + bub.height);
			
			runningHeight += bub.height + SPACE_BETWEEN_MESSAGES;
		}
	}
	
}
