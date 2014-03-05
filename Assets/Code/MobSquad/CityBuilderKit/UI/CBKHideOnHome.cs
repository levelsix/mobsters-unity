using UnityEngine;
using System.Collections;

public class CBKHideOnHome : MonoBehaviour {

	GameObject gameObj;

	void Awake ()
	{
		gameObj = gameObject;
		MSActionManager.Scene.OnCity += OnCity;
	}

	void OnDestroy()
	{
		MSActionManager.Scene.OnCity -= OnCity;
	}

	void OnCity()
	{
		if (MSWhiteboard.currCityType == MSWhiteboard.CityType.NEUTRAL)
		{
			OnMission();
		}
		else
		{
			OnHome();
		}
	}

	void OnHome()
	{
		gameObj.SetActive(false);
	}

	void OnMission()
	{
		gameObj.SetActive(true);
	}
}
