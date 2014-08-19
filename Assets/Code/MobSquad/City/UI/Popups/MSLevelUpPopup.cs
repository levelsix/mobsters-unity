using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MSLevelUpPopup : MonoBehaviour {

	[SerializeField]
	UILabel level;

	[SerializeField]
	TweenPosition tween;

	void OnEnable()
	{
		tween.gameObject.transform.position = tween.from;
	}

	public void ActivateLevelUpScreen(int level)
	{
		gameObject.SetActive(true);
		this.level.text = level.ToString();
		tween.ResetToBeginning();
		tween.PlayForward();
	}

	public void TurnOff()
	{
		gameObject.SetActive(false);
	}

}
