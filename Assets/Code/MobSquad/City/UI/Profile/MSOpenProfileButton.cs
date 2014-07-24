using UnityEngine;
using System.Collections;

public class MSOpenProfileButton : MonoBehaviour 
{
	public int userId;

	[SerializeField]
	bool self = false;

	void OnClick()
	{
		if (self)
		{
			MSPopupManager.instance.popups.profilePopup.Popup(MSWhiteboard.localMup.userId);
		}
		else
		{
			MSPopupManager.instance.popups.profilePopup.Popup(userId);
		}
	}
}
