using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class CBKMapButton : MonoBehaviour {

	public int cityID;

	[SerializeField]
	UILabel label;

	[SerializeField]
	UISprite sprite;

	const string OPEN_CITY = "opencity";
	const string CLOSED_CITY = "closedcity";

	void OnEnable()
	{
		FullCityProto city = CBKDataManager.instance.Get<FullCityProto>(cityID);
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
	}

	void OnClick()
	{
		if (cityID >= 1)
		{
			GoToTown();
			//CBKEventManager.Popup.CreateButtonPopup("Go to Town " + cityID + "?", new string[]{"Yes", "No"}, new Action[]{GoToTown, CBKEventManager.Popup.CloseAllPopups});
		}
		else
		{
			GoHome();
			//CBKEventManager.Popup.CreateButtonPopup("Go Home?", new string[]{"Yes", "No"}, new Action[]{GoHome, CBKEventManager.Popup.CloseAllPopups});
		}
	}
	
	void GoToTown()
	{
		CBKWhiteboard.currCityType = CBKWhiteboard.CityType.NEUTRAL;
		CBKWhiteboard.cityID = cityID;
		CBKEventManager.Loading.LoadBuildings();
		CBKEventManager.Popup.CloseAllPopups();
	}
	
	void GoHome()
	{	
		CBKWhiteboard.currCityType = CBKWhiteboard.CityType.PLAYER;
		CBKWhiteboard.cityID = CBKWhiteboard.localMup.userId;
		CBKEventManager.Loading.LoadBuildings();	
		CBKEventManager.Popup.CloseAllPopups();
	}
	
}
