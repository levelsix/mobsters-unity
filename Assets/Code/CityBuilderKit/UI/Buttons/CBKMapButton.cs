using UnityEngine;
using System.Collections;
using System;

public class CBKMapButton : MonoBehaviour {

	public int cityID;

	void OnClick()
	{
		if (cityID >= 1)
		{
			CBKEventManager.Popup.CreateButtonPopup("Go to Town " + cityID + "?", new string[]{"Yes", "No"}, new Action[]{GoToTown, CBKEventManager.Popup.CloseAllPopups}, true);
		}
		else
		{
			CBKEventManager.Popup.CreateButtonPopup("Go Home?", new string[]{"Yes", "No"}, new Action[]{GoHome, CBKEventManager.Popup.CloseAllPopups}, true);
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
