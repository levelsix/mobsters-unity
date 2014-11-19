using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[RequireComponent (typeof (BoxCollider))]
[RequireComponent (typeof (UIWidget))]
[RequireComponent (typeof (MSSimplePoolable))]
public class MSPrivateChatEntry : MonoBehaviour {

	[SerializeField]
	UILabel name;

	[SerializeField]
	UILabel firstLine;

	[SerializeField]
	UILabel time;

	[SerializeField]
	int maxPreviewLength;

	MinimumUserProtoWithLevel otherUser;

	public void Init(PrivateChatPostProto proto)
	{
		otherUser = proto.poster.minUserProto.userUuid.Equals(MSWhiteboard.localMup.userUuid) ? proto.recipient : proto.poster;
		name.text = otherUser.minUserProto.name;
		time.text = MSUtil.TimeStringLong(MSUtil.timeNowMillis - proto.timeOfPost) + " ago";
		if (proto.content.Length > maxPreviewLength)
		{
			firstLine.text = proto.content.Substring(0, maxPreviewLength) + "...";
		}
		else
		{
			firstLine.text = proto.content;
		}
	}

	void OnClick()
	{
		MSChatManager.instance.GoToPrivateChat(otherUser);
	}
}
