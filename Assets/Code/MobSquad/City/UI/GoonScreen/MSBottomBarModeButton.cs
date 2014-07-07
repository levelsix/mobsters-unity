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
	UISprite background;

	[SerializeField]
	UILabel label;

	[SerializeField]
	Color offColor;

	[SerializeField]
	Color onColor;

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


	Dictionary<GoonScreenMode, string> baseIconSprites = new Dictionary<GoonScreenMode, string>(){
		{GoonScreenMode.HEAL, "heal"},
		{GoonScreenMode.SELL, "sell"},
		{GoonScreenMode.DO_ENHANCE, "enhance"},
		{GoonScreenMode.EVOLVE, "evolove"}
	};

	Dictionary<GoonScreenMode, string> labels = new Dictionary<GoonScreenMode, string>(){
		{GoonScreenMode.HEAL, "Heal"},
		{GoonScreenMode.SELL, "Sell"},
		{GoonScreenMode.DO_ENHANCE, "Enhance"},
		{GoonScreenMode.EVOLVE, "Evolove"}
	};

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

		background.alpha = 0;
		collider.enabled = false;
	}

	[ContextMenu ("Set Inactive")]
	public void SetInactive()
	{
		icon.spriteName = baseIconSprites[_mode] + "grey";
		label.color = offColor;

		background.alpha = 1;
		collider.enabled = true;
	}

	void OnClick()
	{
		Debug.Log("Clicked");
		if (mode == GoonScreenMode.DO_ENHANCE)
		{
			MSPopupManager.instance.popups.goonScreen.InitEnhanceFromButton();
		}
		else
		{
			MSPopupManager.instance.popups.goonScreen.Init(_mode);
		}
	}

}
