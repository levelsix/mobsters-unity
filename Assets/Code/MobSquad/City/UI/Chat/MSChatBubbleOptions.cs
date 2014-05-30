using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[RequireComponent (typeof (MSSimplePoolable))]
[RequireComponent (typeof (MSUIHelper))]
public class MSChatBubbleOptions : MonoBehaviour {

	public MinimumUserProtoWithLevel messageSender;

	MSUIHelper helper;

	[SerializeField] Vector3 offset;
	[ContextMenu ("Set Offset")] public void SetOffset(){offset = transform.localPosition;}

	void Awake()
	{
		helper = GetComponent<MSUIHelper>();
	}

	public void Init(MinimumUserProtoWithLevel sender, Transform parent)
	{
		transform.parent = parent;
		transform.localPosition = offset;
		helper.FadeIn();
		messageSender = sender;
	}

	public void Mute()
	{
		MSChatManager.instance.MutePlayer(messageSender);
		helper.FadeOutAndPool();
	}

	public void Message()
	{
		MSChatManager.instance.GoToPrivateChat(messageSender);
		GetComponent<MSSimplePoolable>().Pool();
	}

	public void Profile()
	{
		//TODO: Put this in when we have a profile page...
		helper.FadeOutAndPool();
	}

}
