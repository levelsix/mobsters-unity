using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSBackSlideButton
/// </summary>
[RequireComponent (typeof(UIButton))]
[RequireComponent (typeof(CBKMenuSlideButton))]
public class MSBackSlideButton : MonoBehaviour 
{
	[SerializeField]
	UILabel label;

	[HideInInspector]
	UIButton button;

	[HideInInspector]
	CBKMenuSlideButton slide;

	void Awake()
	{
		button = GetComponent<UIButton>();
		slide = GetComponent<CBKMenuSlideButton>();
	}

	void OnEnable()
	{
		GameObject backPop = MSPopupManager.instance.backPop;
		if (backPop != null)
		{
			button.isEnabled = true;
			label.text = backPop.name;
			slide.slidingIn = backPop.GetComponent<TweenPosition>();
			slide.slidingOut = MSPopupManager.instance.top.GetComponent<TweenPosition>();
		}
		else
		{
			button.isEnabled = false;
		}
	}

	void OnClick()
	{
		if (button.isEnabled)
		{
			slide.Slide();
		}
	}
}
