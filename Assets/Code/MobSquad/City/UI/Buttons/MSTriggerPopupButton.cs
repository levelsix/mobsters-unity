using UnityEngine;
using System.Collections;

public class MSTriggerPopupButton : MonoBehaviour {
	
	[SerializeField]
	protected GameObject popup;
	
	[SerializeField]
	bool closeTop = false;
	
	public virtual void OnClick()
	{
		if (MSTutorialManager.instance.currentTutorial != null && MSTutorialManager.instance.currentTutorial.currUI != null)
		{
			if (MSTutorialManager.instance.currentTutorial.currUI != gameObject)
			{
				return;
			}
			MSTutorialManager.instance.currentTutorial.OnClicked();
		}
		if (closeTop)
		{
			MSActionManager.Popup.CloseTopPopupLayer();
		}
		MSActionManager.Popup.OnPopup(popup.GetComponent<MSPopup>());
	}
	
}
