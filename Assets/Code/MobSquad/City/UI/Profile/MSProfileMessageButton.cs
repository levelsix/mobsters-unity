using UnityEngine;
using System.Collections;

public class MSProfileMessageButton : MonoBehaviour {

	[SerializeField]
	MSProfilePopup profilePopup;

	void OnClick()
	{
		MSActionManager.Popup.CloseAllPopups();
		MSChatManager.instance.GoToPrivateChat(profilePopup.minUserWithLevel);
	}
}
