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
	
	public bool able = true;
	
	void OnClick()
	{
		if (able && onClick != null)
		{
			onClick();
		}
	}
}
