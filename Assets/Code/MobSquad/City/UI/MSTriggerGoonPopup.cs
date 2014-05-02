using UnityEngine;
using System.Collections;

public class MSTriggerGoonPopup : MSTriggerPopupButton {

	[SerializeField]
	GoonScreenMode mode;
	
	public override void OnClick ()
	{
		if (MSTutorialManager.instance.currentTutorial != null && MSTutorialManager.instance.currentTutorial.currUI != null)
		{
			if (MSTutorialManager.instance.currentTutorial.currUI != gameObject)
			{
				return;
			}
			MSTutorialManager.instance.currentTutorial.OnClicked();
		}
		base.OnClick ();
		MSPopupManager.instance.popups.goonScreen.Init (mode);
	}
	
}
