using UnityEngine;
using System.Collections;

public class CBKGoonMenuButton : MonoBehaviour {

	[SerializeField]
	CBKGoonScreen goonScreen;
	
	void OnClick()
	{
		CBKEventManager.Popup.OnPopup(goonScreen.gameObject);
		goonScreen.InitHeal();
	}
}
