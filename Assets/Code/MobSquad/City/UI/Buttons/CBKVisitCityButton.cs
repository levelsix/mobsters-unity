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
		MSActionManager.Loading.LoadBuildings();
		MSActionManager.Popup.CloseAllPopups();
	}
	
	void GoHome()
	{	
		MSWhiteboard.currCityType = MSWhiteboard.CityType.PLAYER;
		MSWhiteboard.cityID = MSWhiteboard.localMup.userId;
		MSActionManager.Loading.LoadBuildings();	
		MSActionManager.Popup.CloseAllPopups();
	}

}
