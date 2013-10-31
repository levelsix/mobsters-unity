using UnityEngine;
using System.Collections;

public class CBKTriggerPopupButton : MonoBehaviour {
	
	[SerializeField]
	protected GameObject popup;
	
	[SerializeField]
	bool closeTop = false;
	
	public virtual void OnClick()
	{
		if (closeTop)
		{
			CBKEventManager.Popup.CloseTopPopupLayer();
		}
		CBKEventManager.Popup.OnPopup(popup);
	}
	
}
