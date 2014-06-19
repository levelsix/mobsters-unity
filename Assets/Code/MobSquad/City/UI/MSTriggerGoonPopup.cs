using UnityEngine;
using System.Collections;

public class MSTriggerGoonPopup : MSTriggerPopupButton {

	[SerializeField]
	GoonScreenMode mode;
	
	public override void OnClick ()
	{
		base.OnClick ();
		MSPopupManager.instance.popups.goonScreen.Init (mode);
	}
	
}
