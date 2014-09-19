using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(MSPopupSwapper))]
public class MSSwapableErrorPopup : MonoBehaviour {

	[SerializeField]
	UILabel ErrorLabelA;

	[SerializeField]
	UILabel ErrorLabelB;

	[SerializeField]
	UISprite ErrorBgA;
	
	[SerializeField]
	UISprite ErrorBgB;
	
	[SerializeField]
	UISprite[] ErrorCapsA = new UISprite[2];
	
	[SerializeField]
	UISprite[] ErrorCapsB = new UISprite[2];

	MSPopupSwapper swapper;

	void Awake()
	{
		swapper = GetComponent<MSPopupSwapper>();
	}

	void OnEnable(){
		MSActionManager.Popup.DisplayRedError += initRedError;
		MSActionManager.Popup.DisplayGreenError += initGreenError;
	}

	void OnDisable(){
		MSActionManager.Popup.DisplayRedError -= initRedError;
		MSActionManager.Popup.DisplayGreenError -= initGreenError;
	}

	void initError(string text)
	{
		if(swapper.activePopup == MSPopupSwapper.Popup.A)
		{
			ErrorLabelB.text = text;
		}
		else
		{
			ErrorLabelA.text = text;
		}
		swapper.Swap();
	}

	void initRedError(string text)
	{
//		Color red = new Color(0.745f,0.192f,0.118f);
		if(swapper.activePopup == MSPopupSwapper.Popup.A)
		{
			//B
			foreach(UISprite sprite in ErrorCapsB)
			{
				sprite.spriteName = "notendcap";
			}
			ErrorBgB.spriteName = "notmiddle";
		}
		else
		{
			//A
			foreach(UISprite sprite in ErrorCapsA)
			{
				sprite.spriteName = "notendcap";
			}
			ErrorBgA.spriteName = "notmiddle";
		}

		initError(text);
	}

	void initGreenError(string text)
	{
//		Color green = new Color(0.533f,0.647f,0.149f);
		if(swapper.activePopup == MSPopupSwapper.Popup.A)
		{
			//B
			foreach(UISprite sprite in ErrorCapsB)
			{
				sprite.spriteName = "notendcapgreen";
			}
			ErrorBgB.spriteName = "notmiddlegreen";
		}
		else
		{
			//A
			foreach(UISprite sprite in ErrorCapsA)
			{
				sprite.spriteName = "notendcapgreen";
			}
			ErrorBgA.spriteName = "notmiddlegreen";
		}
		
		initError(text);
	}
}
