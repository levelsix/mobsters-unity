using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[RequireComponent (typeof (MSSimplePoolable))]
[RequireComponent (typeof (MSUIHelper))]
public class MSChatBubbleOptions : MonoBehaviour {

	public static MSChatBubbleOptions instance;

	public MinimumUserProtoWithLevel messageSender;

	MSUIHelper helper;

	bool open = false;

	[SerializeField]
	UITweener[] tweens;

	[SerializeField] Vector3 offset;
	[ContextMenu ("Set Offset")] public void SetOffset(){offset = transform.localPosition;}

	void Awake()
	{
		instance = this;
		helper = GetComponent<MSUIHelper>();
	}

	public void Init(MinimumUserProtoWithLevel sender, Transform parent)
	{
		if (open)
		{
			Close();
		}
		else
		{
			open = true;
			transform.parent = parent;
			transform.localPosition = offset;
			messageSender = sender;
			foreach (var item in tweens) 
			{
				item.Sample(0, true);
				item.PlayForward();
			}
		}
	}

	public void Mute()
	{
		MSChatManager.instance.MutePlayer(messageSender);
		Close();
	}

	public void Message()
	{
		MSChatManager.instance.GoToPrivateChat(messageSender);
		Close(true);
	}

	public void Profile()
	{
		MSPopupManager.instance.popups.profilePopup.Popup(messageSender.minUserProto.userId);
		Close();
	}

	public void Close(bool instant = false)
	{
		open = false;
		Debug.LogWarning("Close");
		foreach (var item in tweens) 
		{
			if (instant)
			{
				item.Sample(0, true);
			}
			else
			{
				item.PlayReverse();
			}
		}
	}

}
