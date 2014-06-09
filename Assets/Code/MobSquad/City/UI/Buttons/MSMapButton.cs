using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class MSMapButton : MonoBehaviour {

	public int cityID;

	[SerializeField]
	UILabel label;

	[SerializeField]
	UISprite sprite;

	const string OPEN_CITY = "opencitypin";
	const string CLOSED_CITY = "closedcitypin";

	void OnEnable()
	{
		if (cityID > 0)
		{
			FullCityProto city = MSDataManager.instance.Get<FullCityProto>(cityID);
			if (city != null)
			{
				sprite.spriteName = OPEN_CITY;
				label.text = city.name;
			}
			else
			{
				sprite.spriteName = CLOSED_CITY;
				label.text = " ";
			}
			sprite.MakePixelPerfect();
		}
	}

	void OnClick()
	{
		if (cityID >= 1)
		{
			StartCoroutine(GoToTown());
		}
		else
		{
			StartCoroutine(GoHome());
		}
	}
	
	IEnumerator GoToTown()
	{
		MSWhiteboard.currCityType = MSWhiteboard.CityType.NEUTRAL;
		MSWhiteboard.cityID = cityID;

		yield return StartCoroutine(MSBuildingManager.instance.LoadNeutralCity(cityID));
	}
	
	IEnumerator GoHome()
	{	
		MSWhiteboard.currCityType = MSWhiteboard.CityType.PLAYER;
		MSWhiteboard.cityID = MSWhiteboard.localMup.userId;

		yield return StartCoroutine(MSBuildingManager.instance.LoadPlayerCity());

		MSActionManager.Popup.CloseAllPopups();
	}
	
}
