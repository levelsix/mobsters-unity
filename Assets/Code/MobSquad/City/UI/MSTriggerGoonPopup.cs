using UnityEngine;
using System.Collections;

public class MSTriggerGoonPopup : MSTriggerPopupButton {
	
	[SerializeField]
	CBKGoonScreen goonScreen;
	
	[SerializeField]
	bool healMode;
	
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
		if (healMode)
		{
			goonScreen.InitHeal();
		}
		else
		{
			goonScreen.InitEnhance();
		}
	}
	
}
