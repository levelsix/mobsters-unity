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
			MSPopupManager.instance.CreatePopup("Forfeit?",
				"You will lose everything - are you sure you want to forfeit?", 
                new string[]{"Cancel", "Forfeit"},
				new string[]{"greymenuoption", "redmenuoption"},
				new Action[]{MSActionManager.Popup.CloseAllPopups, 
					delegate {
						MSActionManager.Puzzle.ForceHideSwap(); 
						MSActionManager.Popup.CloseAllPopups(); 
						StartCoroutine(PZCombatManager.instance.OnPlayerForfeit());
					}  
				});
		}
	}

	void Update()
	{
		button.enabled = PZPuzzleManager.instance.swapLock <= 0 || PZDeployPopup.acting;
		sprite.alpha = (button.enabled) ? 1 : 0;
	}
}
