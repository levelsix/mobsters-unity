using UnityEngine;
using System.Collections;

public class MSClosePopupButton : MonoBehaviour {

	[SerializeField]
	bool all = true;

	void OnClick()
	{
		if (all)
		{
			MSActionManager.Popup.CloseAllPopups();
		}
		else
		{
			MSActionManager.Popup.CloseTopPopupLayer();
		}
	}
	
}
