using UnityEngine;
using System.Collections;

public class CBKClosePopupButton : MonoBehaviour {

	[SerializeField]
	bool all = true;

	void OnClick()
	{
		if (all)
		{
			CBKEventManager.Popup.CloseAllPopups();
		}
		else
		{
			CBKEventManager.Popup.CloseTopPopupLayer();
		}
	}
	
}
