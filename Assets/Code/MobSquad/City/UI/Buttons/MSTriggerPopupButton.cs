using UnityEngine;
using System.Collections;

public class MSTriggerPopupButton : MonoBehaviour {
	
	[SerializeField]
	public GameObject popup;
	
	[SerializeField]
	bool closeTop = false;
	
	public virtual void OnClick()
	{
		if (closeTop)
		{
			MSActionManager.Popup.CloseTopPopupLayer();
		}
		MSActionManager.Popup.OnPopup(popup.GetComponent<MSPopup>());
	}
	
}
