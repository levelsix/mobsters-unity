using UnityEngine;
using System.Collections;

public class CBKQuickMapButton : MonoBehaviour {
	
	void OnClick()
	{
		if (CBKWhiteboard.currCityType == CBKWhiteboard.CityType.PLAYER)
		{
			CBKWhiteboard.currCityType = CBKWhiteboard.CityType.NEUTRAL;
			CBKWhiteboard.cityID = 1;
		}
		else
		{
			CBKWhiteboard.currCityType = CBKWhiteboard.CityType.PLAYER;
			CBKWhiteboard.cityID = CBKWhiteboard.localMup.userId;
		}
			
		CBKEventManager.Loading.LoadBuildings();
	}
	
}
