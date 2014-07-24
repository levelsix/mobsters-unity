using UnityEngine;
using System.Collections;

public class MSProfileTab : MSTab {

	[SerializeField]
	UISprite bar;
	
	public override void InitActive()
	{
		bar.alpha = 1;
		icon.spriteName = spriteRoot + "active";
		icon.MakePixelPerfect();
		label.color = MSColors.activeTabTextColor;
	}
	
	public override void InitInactive()
	{
		bar.alpha = 0;
		icon.spriteName = spriteRoot + "inactive";
		icon.MakePixelPerfect();
		label.color = MSColors.inactiveTabTextColor;
	}
}
