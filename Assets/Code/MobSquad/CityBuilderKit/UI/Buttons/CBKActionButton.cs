using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// @author Rob Giusti
/// Action button, which allows other scripts to hook into its actions.
/// Allows scripts that involve multiple UI elements to more easily contain
/// all logic in one place.
/// </summary>
public class CBKActionButton : MonoBehaviour {
	
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

	public UIDragScrollView dragBehind;

	CBKUIHelper helper;
	
	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
		button = GetComponent<UIButton>();
		helper = GetComponent<CBKUIHelper>();
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

	public virtual void OnClick()
	{
		if (able && onClick != null)
		{
			onClick();
		}
	}

	public void OnPress(bool pressed)
	{
		if (dragBehind != null)
		{
			dragBehind.OnPress(pressed);
		}
	}

	public void OnDrag(Vector2 delta)
	{
		if (dragBehind != null)
		{
			dragBehind.OnDrag(delta);
		}
	}
}
