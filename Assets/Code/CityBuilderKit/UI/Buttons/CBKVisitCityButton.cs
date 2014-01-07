using UnityEngine;
using System.Collections;

public class CBKVisitCityButton : MonoBehaviour {

	public int cityID;

	[HideInInspector]
	public UIButton button;

	void Awake()
	{
		button = GetComponent<UIButton>();
	}

	void OnClick()
	{
		if (cityID >= 1)
		{
			GoToTown();
		}
		else
		{
			GoHome();
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
