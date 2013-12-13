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
	
	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
		button = GetComponent<UIButton>();
	}

	public void OnClick()
	{
		Debug.Log("Clicked: " + name);
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
