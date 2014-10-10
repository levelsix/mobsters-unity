using UnityEngine;
using System.Collections;

public class MSTriggerChatPopupButton : MSTriggerPopupButton {

	[SerializeField]
	MSValues.ChatMode chatMode;

	public override void OnClick()
	{
		base.OnClick();
		popup.GetComponent<MSChatPopup>().Init(chatMode);
	}
}
