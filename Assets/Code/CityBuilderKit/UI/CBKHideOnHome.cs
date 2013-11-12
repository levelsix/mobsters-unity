using UnityEngine;
using System.Collections;

public class CBKHideOnHome : MonoBehaviour {

	GameObject gameObj;

	void Awake ()
	{
		gameObj = gameObject;
		CBKEventManager.Scene.OnCity += OnCity;
	}

	void OnDestroy()
	{
		CBKEventManager.Scene.OnCity -= OnCity;
	}

	void OnCity()
	{
		if (CBKWhiteboard.currCityType == CBKWhiteboard.CityType.NEUTRAL)
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
