using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSClanTab
/// </summary>
public class MSClanTab : MonoBehaviour {

	[SerializeField]
	UISprite tab;

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UILabel label;

	[SerializeField]
	MSClanTab other;

	[SerializeField]
	MSClanPopup pop;

	[SerializeField]
	Color activeTextColor;

	[SerializeField]
	Color inactiveTextColor;

	bool inactive = true;

	ClanPopupMode clanMode;

	Dictionary<ClanPopupMode, string> iconDict = new Dictionary<ClanPopupMode, string>()
	{
		{ClanPopupMode.BROWSE, "clansbrowse"},
		{ClanPopupMode.DETAILS, "clansbrowse"},
		{ClanPopupMode.CREATE, "clanscreate"},
		{ClanPopupMode.RAIDS, "clansbrowse"}
	};

	const string inactiveTab = "popupinactivetab";
	const string activeTab = "popupactivetab";

	public void Init(ClanPopupMode mode, bool activate)
	{
		clanMode = mode;
		inactive = !activate;
		if (activate)
		{
			InitActive();
		}
		else
		{
			InitInactive();
		}
	}

	public void InitActive()
	{
		tab.spriteName = activeTab;
		icon.spriteName = iconDict[clanMode] + "active";
		label.color = activeTextColor;
		inactive = false;
	}

	public void InitInactive()
	{
		tab.spriteName = inactiveTab;
		icon.spriteName = iconDict[clanMode] + "inactive";
		label.color = inactiveTextColor;
		inactive = true;
	}

	void OnClick()
	{
		if (inactive)
		{
			pop.GoToMode(clanMode);
			InitActive();
			other.InitInactive();
		}
	}
}
