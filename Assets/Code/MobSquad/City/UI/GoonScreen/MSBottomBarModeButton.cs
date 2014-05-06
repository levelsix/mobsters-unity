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
	GoonScreenMode _mode;

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

	public GoonScreenMode mode
	{
		set
		{
			_mode = value;

		}
		get
		{
			return _mode;
		}
	}

	UISprite backgroundSprite;

	Dictionary<GoonScreenMode, string> baseIconSprites = new Dictionary<GoonScreenMode, string>(){
		{GoonScreenMode.HEAL, "heal"},
		{GoonScreenMode.SELL, "sell"},
		{GoonScreenMode.ENHANCE, "enhance"},
		{GoonScreenMode.EVOLVE, "evolove"}
	};

	Dictionary<GoonScreenMode, string> labels = new Dictionary<GoonScreenMode, string>(){
		{GoonScreenMode.HEAL, "Heal"},
		{GoonScreenMode.SELL, "Sell"},
		{GoonScreenMode.ENHANCE, "Enhance"},
		{GoonScreenMode.EVOLVE, "Evolove"}
	};

	void Awake()
	{
		backgroundSprite = GetComponent<UISprite>();
	}

	public void Set(bool active)
	{
		if (active)
		{
			SetActive();
		}
		else
		{
			SetInactive();
		}
		label.text = labels[mode];
	}

	[ContextMenu ("Set Active")]
	public void SetActive()
	{
		icon.spriteName = baseIconSprites[_mode] + "blue";
		label.color = onColor;

		if (backgroundSprite == null)
		{
			Awake ();
		}
		backgroundSprite.enabled = false;
		collider.enabled = false;
	}

	[ContextMenu ("Set Inactive")]
	public void SetInactive()
	{
		icon.spriteName = baseIconSprites[_mode] + "grey";
		label.color = offColor;

		if (backgroundSprite == null)
		{
			Awake ();
		}
		backgroundSprite.enabled = true;
		collider.enabled = true;
	}

	void OnClick()
	{
		if (mode == GoonScreenMode.ENHANCE)
		{
			MSPopupManager.instance.popups.goonScreen.InitEnhanceFromButton();
		}
		else
		{
			MSPopupManager.instance.popups.goonScreen.Init(_mode);
		}
	}

}
