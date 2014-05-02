using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSBottomBarModeButton
/// </summary>
public class MSBottomBarModeButton : MonoBehaviour {

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UILabel label;

	[SerializeField]
	Color offColor;

	[SerializeField]
	Color onColor;

	[SerializeField]
	bool active = false;

	[SerializeField]
	float onOffset = 2;

	[SerializeField]
	string baseIconSpriteName = "heal";

	UISprite backgroundSprite;

	void Awake()
	{
		backgroundSprite = GetComponent<UISprite>();
	}

	[ContextMenu ("Set Active")]
	public void SetActive()
	{
		icon.spriteName = baseIconSpriteName + "blue";
		label.color = onColor;

		if (backgroundSprite == null)
		{
			Awake ();
		}
		backgroundSprite.enabled = false;

		icon.transform.localPosition = new Vector3(onOffset, icon.transform.localPosition.y, icon.transform.localPosition.z);
		label.transform.localPosition = new Vector3(onOffset, label.transform.localPosition.y, label.transform.localPosition.z);
	}

	[ContextMenu ("Set Inactive")]
	public void SetInactive()
	{
		icon.spriteName = baseIconSpriteName + "grey";
		label.color = offColor;

		if (backgroundSprite == null)
		{
			Awake ();
		}
		backgroundSprite.enabled = true;
		
		icon.transform.localPosition = new Vector3(0, icon.transform.localPosition.y, icon.transform.localPosition.z);
		label.transform.localPosition = new Vector3(0, label.transform.localPosition.y, label.transform.localPosition.z);
	}

}
