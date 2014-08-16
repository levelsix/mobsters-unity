using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MSLevelUpPopup : MonoBehaviour {

	[SerializeField]
	UILabel level;

	[SerializeField]
	TweenPosition tween;

	public static Action<int> Init;

	void Awake()
	{
		Init += InitLevelUpScreen;
	}

	void OnEnable()
	{
		transform.localPosition = tween.from;
	}

	public void InitLevelUpScreen(int level)
	{
		enabled = true;
		this.level.text = level.ToString();
		tween.ResetToBeginning();
		tween.PlayForward();
		tween.AddOnFinished( delegate { enabled = false; });
	}

}
