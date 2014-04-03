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
public class MSBackSlideButton : MonoBehaviour 
{
	[SerializeField]
	UILabel label;

	[HideInInspector]
	UIButton button;

	void Awake()
	{
		button = GetComponent<UIButton>();
	}

	void OnEnable()
	{
		MSPopup backPop = MSPopupManager.instance.backPop;
		if (backPop != null)
		{
			button.isEnabled = true;
			label.text = backPop.name;
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
			MSActionManager.Popup.CloseTopPopupLayer();
		}
	}
}
