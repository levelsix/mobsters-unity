using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSHelpBubble : MSChatBubble {

	[SerializeField]
	float minWidth = 0f;

	[SerializeField]
	float minHeight = 0f;

	public void Init(ClanHelpProto proto)
	{
		sender = new MinimumUserProtoWithLevel();
		sender.minUserProto = proto.mup;
		sender.level = 0;
		Init(proto.timeRequested, proto.mup.name, "", proto.mup.avatarMonsterId, false);
	}

	void Init(long time, string sender, string message, int avatarId, bool leader = false)
	{
		base.Init(time, sender, message, avatarId, leader);
		bubble.width = (int)Mathf.Max(minWidth, bubble.width);
		bubble.height = (int)Mathf.Max(minHeight, bubble.height);
	}
}
