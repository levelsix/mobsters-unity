using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MSTabButton : MonoBehaviour {

	[SerializeField]
	string iconWhite;

	[SerializeField]
	string iconBlue;

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UILabel label;

	[SerializeField]
	Color textBlue;

	[SerializeField]
	MSPopup popup;

	enum Tab{
		BUILDING,
		FUNDS,
		MOBSTERS
	}

	[SerializeField]
	Tab buttonType;

	UIButton button;
	
	UISprite tabSprite;

	int startingDepth;



	public static Action ClickTab;

	const string TAB_PRESSED = "menuinactivetabpressed";

	const string TAB_INACTIVE = "menuinactivetab";

	const string TAB_ACTIVE = "menuactivetab";

	void Awake(){
		button = GetComponent<UIButton>();
		tabSprite = GetComponent<UISprite>();
		startingDepth = tabSprite.depth;
		ClickTab += Deselect;
	}

	void OnEnable(){
		switch(buttonType){
		case Tab.BUILDING:
			if(MSBuildingManager.instance.CapacityForBuildings()){
				OnClick();
			}
			break;
		case Tab.MOBSTERS:
			if(!MSBuildingManager.instance.CapacityForBuildings()){
				OnClick();
			}
			break;
		default:
			break;
		}
	}

	void OnDisable(){
		Deselect();
	}

	void ChangeDepthTo(int depth){
		tabSprite.depth = depth;
		label.depth = depth + 1;
		icon.depth = depth + 1;
	}

	void Deselect(){
		button.normalSprite = TAB_INACTIVE;
		button.pressedSprite = TAB_PRESSED;
		icon.spriteName = iconWhite;
		label.color = Color.white;
		ChangeDepthTo(startingDepth);

		popup.gameObject.SetActive(false);
	}

	void OnClick(){
		ClickTab();
		popup.gameObject.SetActive(true);
		StartCoroutine(SetSprite(true));
		icon.spriteName = iconBlue;
		label.color = textBlue;
		ChangeDepthTo(startingDepth + 2);
	}

	IEnumerator SetSprite(bool active){
		if(active){
			while(button.normalSprite != TAB_ACTIVE){
				button.normalSprite = TAB_ACTIVE;
				button.pressedSprite = TAB_ACTIVE;
				yield return null;
			}
		}
		else
		{
			while(button.normalSprite != TAB_INACTIVE){
				button.normalSprite = TAB_INACTIVE;
				button.pressedSprite = TAB_PRESSED;
				yield return null;
			}
		}
	}
}
