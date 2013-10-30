using UnityEngine;
using System.Collections;
using System;

public class CBKQuickMapButton : MonoBehaviour {
	
	void OnClick()
	{
		if (CBKWhiteboard.currCityType == CBKWhiteboard.CityType.PLAYER)
		{
			CBKEventManager.Popup.CreateButtonPopup("Go to Town 1?", new string[]{"Yes", "No"}, new Action[]{GoToTown, CBKEventManager.Popup.CloseAllPopups});
		}
		else
		{
			CBKEventManager.Popup.CreateButtonPopup("Go Home?", new string[]{"Yes", "No"}, new Action[]{GoHome, CBKEventManager.Popup.CloseAllPopups});
		}
	}
	
	void GoToTown()
	{
		CBKWhiteboard.currCityType = CBKWhiteboard.CityType.NEUTRAL;
		CBKWhiteboard.cityID = 1;
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
