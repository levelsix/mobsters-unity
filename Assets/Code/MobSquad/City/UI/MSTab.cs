using UnityEngine;
using System.Collections;

public class MSTab : MonoBehaviour {

	[SerializeField]
	protected UIButton button;

	public UILabel label;

	[SerializeField]
	protected UISprite icon;

	[SerializeField]
	protected string spriteRoot;

	[SerializeField]
	string inactiveTab = "popupinactivetab";

	[SerializeField]
	string activeTab = "popupactivetab";

	public bool active;

	public virtual void InitActive()
	{
		button.normalSprite = activeTab;
		button.GetComponent<UISprite>().spriteName = activeTab;
		icon.spriteName = spriteRoot + "active";
		icon.MakePixelPerfect();
		label.color = MSColors.activeTabTextColor;
		active = true;
	}

	public virtual void InitInactive()
	{
		button.normalSprite = inactiveTab;
		button.GetComponent<UISprite>().spriteName = inactiveTab;
		icon.spriteName = spriteRoot + "inactive";
		icon.MakePixelPerfect();
		label.color = MSColors.inactiveTabTextColor;
		active = false;
	}
}
