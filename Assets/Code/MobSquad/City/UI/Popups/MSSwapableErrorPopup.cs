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
		MSActionManager.Popup.DisplayOrangeError += initOrangeError;
		MSActionManager.Popup.DisplayBlueError += initBlueError;
		MSActionManager.Popup.DisplayPurpleError += initPurpleError;
	}

	void OnDisable(){
		MSActionManager.Popup.DisplayRedError -= initRedError;
		MSActionManager.Popup.DisplayGreenError -= initGreenError;
		MSActionManager.Popup.DisplayOrangeError -= initOrangeError;
		MSActionManager.Popup.DisplayBlueError -= initBlueError;
		MSActionManager.Popup.DisplayPurpleError -= initPurpleError;
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
		SetColor("notendcap", "notmiddle");
		initError(text);
	}

	void initGreenError(string text)
	{
		SetColor("notendcapgreen", "notmiddlegreen");
		initError(text);
	}

	void initOrangeError(string text)
	{
		SetColor("notendcaporange", "notmiddleorange");
		initError(text);
	}

	void initBlueError(string text)
	{
		SetColor("notendcapblue", "notmiddleblue");
		initError(text);
	}

	void initPurpleError(string text)
	{
		SetColor("notendcappurple", "notmiddlepurple");
		initError(text);
	}

	void SetColor(string capName, string middleName)
	{
		if(swapper.activePopup == MSPopupSwapper.Popup.A)
		{
			//B
			foreach(UISprite sprite in ErrorCapsB)
			{
				sprite.spriteName = capName;
			}
			ErrorBgB.spriteName = middleName;
		}
		else
		{
			//A
			foreach(UISprite sprite in ErrorCapsA)
			{
				sprite.spriteName = capName;
			}
			ErrorBgA.spriteName = middleName;
		}
	}
}
