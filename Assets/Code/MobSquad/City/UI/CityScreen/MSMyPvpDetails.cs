using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMyPvpDetails
/// </summary>
public class MSMyPvpDetails : MonoBehaviour {

	[SerializeField]
	UILabel leagueName;

	[SerializeField]
	UILabel leaguePlace;

	[SerializeField]
	UILabel placeEnding;

	[SerializeField]
	UISprite leagueIcon;

	[SerializeField]
	UI2DSprite ribbon;

	Dictionary<string, string> ribbonsForLeague = new Dictionary<string, string>()
	{
		{"bronze", "bronzegoldribbon"},
		{"gold", "bronzegoldribbon"},
		{"champion", "championribbon"},
		{"diamond", "diamondribbon"},
		{"silver", "silverribbon"},
		{"platinumribbon", "platinumribbon"}
	};

	void OnEnable()
	{
		SetLeagueInfo();
	}

	void SetLeagueInfo()
	{
		PvpLeagueProto pvpLeague = MSDataManager.instance.Get<PvpLeagueProto>(MSWhiteboard.localUser.pvpLeagueInfo.leagueId);

		leagueName.text = pvpLeague.leagueName;
		leaguePlace.text = MSWhiteboard.localUser.pvpLeagueInfo.rank.ToString();
		switch (MSWhiteboard.localUser.pvpLeagueInfo.rank) 
		{
			case 1:
				placeEnding.text = "st";
				break;
			case 2:
				placeEnding.text = "nd";
				break;
			case 3:
				placeEnding.text = "rd";
				break;
			default:
				placeEnding.text = "th";
				break;
		}

		leagueIcon.spriteName = MSUtil.StripExtensions(pvpLeague.imgPrefix) + "big";
		leagueIcon.MakePixelPerfect();

		//TODO: Get ribbon sprite from AWS
	}
}
