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
			MSActionManager.Popup.CreateButtonPopup("You will lose everything - are you sure you want to forfeit?", new string[]{"Cancel", "Forfeit"},
			new Action[]{MSActionManager.Popup.CloseAllPopups, delegate {MSActionManager.Puzzle.ForceHideSwap(); MSActionManager.Popup.CloseAllPopups(); PZCombatManager.instance.ActivateLoseMenu(); }  });
		}
	}

	void Update()
	{
		button.enabled = PZPuzzleManager.instance.swapLock <= 0 || PZDeployPopup.acting;
		sprite.alpha = (button.enabled) ? 1 : 0;
	}
}
