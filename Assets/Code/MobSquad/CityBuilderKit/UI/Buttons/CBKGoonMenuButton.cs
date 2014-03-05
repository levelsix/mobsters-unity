using UnityEngine;
using System.Collections;

public class CBKGoonMenuButton : MonoBehaviour {

	[SerializeField]
	CBKGoonScreen goonScreen;
	
	void OnClick()
	{
		MSActionManager.Popup.OnPopup(goonScreen.gameObject);
		goonScreen.InitHeal();
	}
}
