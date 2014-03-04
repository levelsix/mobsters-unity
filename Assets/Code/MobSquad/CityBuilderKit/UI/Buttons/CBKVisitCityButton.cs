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
		MSWhiteboard.currCityType = MSWhiteboard.CityType.NEUTRAL;
		MSWhiteboard.cityID = cityID;
		CBKEventManager.Loading.LoadBuildings();
		CBKEventManager.Popup.CloseAllPopups();
	}
	
	void GoHome()
	{	
		MSWhiteboard.currCityType = MSWhiteboard.CityType.PLAYER;
		MSWhiteboard.cityID = MSWhiteboard.localMup.userId;
		CBKEventManager.Loading.LoadBuildings();	
		CBKEventManager.Popup.CloseAllPopups();
	}

}
