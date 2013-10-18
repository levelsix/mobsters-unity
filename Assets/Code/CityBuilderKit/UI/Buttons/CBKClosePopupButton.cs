using UnityEngine;
using System.Collections;

public class CBKClosePopupButton : MonoBehaviour {

	void OnClick()
	{
		CBKEventManager.Popup.CloseAllPopups();
	}
	
}
