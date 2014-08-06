using UnityEngine;
using System.Collections;
using System;

public class PZRetreatButton : MonoBehaviour {

	[SerializeField]
	MSPopup retreatPopup;

	UIButton button;

	UISprite sprite;

	void Awake()
	{
		button = GetComponent<UIButton>();
		sprite = GetComponent<UISprite>();
	}

	void OnClick()
	{
		MSActionManager.Popup.OnPopup(retreatPopup);
	}

	void Update()
	{
		button.enabled = PZPuzzleManager.instance.swapLock <= 0 || PZDeployPopup.acting;
		sprite.alpha = (button.enabled) ? 1 : 0;
	}
}
