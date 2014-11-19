using UnityEngine;
using System.Collections;

public class MSOpenProfileButton : MonoBehaviour 
{
	public string userUuid;

	[SerializeField]
	bool self = false;

	void OnClick()
	{
		if (self)
		{
			MSPopupManager.instance.popups.profilePopup.Popup(MSWhiteboard.localMup.userUuid);
		}
		else
		{
			MSPopupManager.instance.popups.profilePopup.Popup(userUuid);
		}
	}
}
