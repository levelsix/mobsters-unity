using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSClanTab
/// </summary>
public class MSClanTab : MSTab {

	[SerializeField]
	MSClanTab other;

	[SerializeField]
	MSClanPopup pop;

	[SerializeField] bool inactive = true;

	ClanPopupMode clanMode;

	Dictionary<ClanPopupMode, string> iconDict = new Dictionary<ClanPopupMode, string>()
	{
		{ClanPopupMode.BROWSE, "clanbrowse"},
		{ClanPopupMode.DETAILS, "myclan"},
		{ClanPopupMode.CREATE, "clancreate"},
		{ClanPopupMode.RAIDS, "clansbrowse"}
	};

	Dictionary<ClanPopupMode, string> tabLabels = new Dictionary<ClanPopupMode, string>()
	{
		{ClanPopupMode.BROWSE, "BROWSE"},
		{ClanPopupMode.DETAILS, "MY SQUAD"},
		{ClanPopupMode.CREATE, "CREATE SQUAD"},
		{ClanPopupMode.RAIDS, "RAIDS"}
	};

	public void Init(ClanPopupMode mode, bool activate)
	{
		spriteRoot = iconDict[mode];
		clanMode = mode;
		inactive = !activate;
		label.text = tabLabels[mode];
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
		base.InitActive();
		inactive = false;
	}

	public void InitInactive()
	{
		base.InitInactive();
		inactive = true;
	}

	void OnClick()
	{
		if (inactive)
		{
			pop.GoToMode(clanMode, true);
			InitActive();
			other.InitInactive();
		}
	}
}
