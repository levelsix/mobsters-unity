using UnityEngine;
using System.Collections;
using System;

public class PZRetreatButton : MonoBehaviour {

	[SerializeField]
	MSPopup retreatPopup;

	UIButton button;
//
	UISprite sprite;

	void Awake()
	{
		button = GetComponent<UIButton>();
		sprite = GetComponent<UISprite>();
	}

	void OnEnable()
	{
		MSActionManager.Puzzle.OnTurnChange += TurnChange;
	}

	void OnDisable()
	{
		MSActionManager.Puzzle.OnTurnChange -= TurnChange;
		button.enabled = true;
		sprite.alpha = 1;
	}

	void TurnChange(int turnsLeft)
	{
		button.enabled = turnsLeft > 0;
		sprite.alpha = (button.enabled) ? 1 : 0;
	}

	void OnClick()
	{
		MSActionManager.Popup.OnPopup(retreatPopup);
	}

	public void Hide()
	{
		button.enabled = false;
		sprite.alpha = 0f;
	}

	//Kenny removed this cause UI change. So this button should always show.
//	void Update()
//	{
//		button.enabled = PZPuzzleManager.instance.swapLock <= 0 || PZDeployPopup.acting;
//		sprite.alpha = (button.enabled) ? 1 : 0;
//	}
}
