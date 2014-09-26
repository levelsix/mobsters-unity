using UnityEngine;
using System.Collections;

public class MSTriggerChatPopupButton : MSTriggerPopupButton {

	[SerializeField]
	MSValues.ChatMode chatMode;

	void OnClick()
	{
		base.OnClick();
		popup.GetComponent<MSChatPopup>().Init(chatMode);
	}
}
