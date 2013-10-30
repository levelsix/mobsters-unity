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
	
	public UIButton button;
	
	public bool able = true;
	
	public UILabel label;
	
	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
		button = GetComponent<UIButton>();
	}
	
	void OnClick()
	{
		if (able && onClick != null)
		{
			onClick();
		}
	}
}
