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
			MSActionManager.Popup.CloseTopPopupLayer();
		}
		MSActionManager.Popup.OnPopup(popup);
	}
	
}
