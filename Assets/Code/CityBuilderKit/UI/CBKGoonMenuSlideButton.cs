using UnityEngine;
using System.Collections;

public class CBKGoonMenuSlideButton : CBKMenuSlideButton {
	
	[SerializeField]
	CBKGoonScreen goonScreen;
	
	[SerializeField]
	bool healMode;
	
	public override void OnClick ()
	{
		if (healMode)
		{
			goonScreen.InitHeal();
		}
		else
		{
			goonScreen.InitEnhance();
		}
		base.OnClick ();
	}
	
}
