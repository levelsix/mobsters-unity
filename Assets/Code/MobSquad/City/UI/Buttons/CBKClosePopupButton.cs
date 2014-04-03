using UnityEngine;
using System.Collections;

public class CBKClosePopupButton : MonoBehaviour {

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
