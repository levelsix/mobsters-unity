using UnityEngine;
using System.Collections;

public class CBKTriggerPopupButton : MonoBehaviour {
	
	[SerializeField]
	protected GameObject popup;
	
	public virtual void OnClick()
	{
		CBKEventManager.Popup.OnPopup(popup);
	}
	
}
