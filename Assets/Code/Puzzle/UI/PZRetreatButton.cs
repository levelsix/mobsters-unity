using UnityEngine;
using System.Collections;
using System;

public class PZRetreatButton : MonoBehaviour {

	UIButton button;

	UISprite sprite;

	void Awake()
	{
		button = GetComponent<UIButton>();
		sprite = GetComponent<UISprite>();
	}

	void OnClick()
	{
		if (button.enabled)
		{
			CBKEventManager.Popup.CreateButtonPopup("You will lose everything - are you sure you want to forfeit?", new string[]{"Cancel", "Forfeit"},
			new Action[]{CBKEventManager.Popup.CloseAllPopups, delegate {CBKEventManager.Puzzle.ForceHideSwap(); CBKEventManager.Popup.CloseAllPopups(); CBKEventManager.Scene.OnCity(); }  }, false);
		}
	}

	void Update()
	{
		button.enabled = PZPuzzleManager.instance.swapLock <= 0;
		sprite.alpha = (button.enabled) ? 1 : 0;
	}
}
