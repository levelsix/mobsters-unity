using UnityEngine;
using System.Collections;

public class MSTriggerShopPopupButton : MSTriggerPopupButton {

	[SerializeField]
	MSTabButton funds;

	[SerializeField]
	MSTabButton monsters;

	[SerializeField]
	MSTabButton Building;

	enum Button
	{
		FUNDS,
		MONSTERS,
		BUILDING
	}

	[SerializeField]
	Button activeButton;

	public virtual void OnClick(){
		base.OnClick();

		switch(activeButton)
		{
		case Button.BUILDING:
			Building.OnClick();
			break;
		case Button.FUNDS:
			funds.OnClick();
			break;
		case Button.MONSTERS:
			monsters.OnClick();
			break;
		default:
			break;
		}
	}
}
