using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// @author Rob Giusti
/// Action button, which allows other scripts to hook into its actions.
/// Allows scripts that involve multiple UI elements to more easily contain
/// all logic in one place.
/// </summary>
public class MSActionButton : MonoBehaviour {
	
	public Action onClick;
	
	[HideInInspector]
	public GameObject gameObj;
	
	[HideInInspector]
	public Transform trans;
		
	public UISprite icon;

	[HideInInspector]
	public UIButton button;
	
	public bool able = true;
	
	public UILabel label;

	MSUIHelper helper;
	
	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
		button = GetComponent<UIButton>();
		helper = GetComponent<MSUIHelper>();
	}

	public void Enable()
	{
		if (button != null)
		{
			button.enabled = true;
		}
		if (helper != null)
		{
			helper.FadeIn();
		}
	}

	public void Disable()
	{
		if (button != null)
		{
			button.enabled = false;
		}
		if (helper != null)
		{
			helper.FadeOut();
		}
	}

	protected virtual void OnClick()
	{
		if (able && onClick != null)
		{
			onClick();
		}
	}
}
