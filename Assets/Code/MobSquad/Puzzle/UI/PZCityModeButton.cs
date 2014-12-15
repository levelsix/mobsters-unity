using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Simple script to attach to an NGUI element to make it take the user
/// back to the city view
/// </summary>
[RequireComponent (typeof (UIButton))]
public class PZCityModeButton : MonoBehaviour {

	/// <summary>
	/// Keep a pointer to this in case anything else needs to access it through this
	/// </summary>
	public UIButton button;

	void Awake()
	{
		button = GetComponent<UIButton>();
	}

	void OnClick()
	{
//		Debug.Log("To City");
		MSWhiteboard.currSceneType = MSWhiteboard.SceneType.CITY;
		if(MSActionManager.Scene.OnCity != null)
		{
			MSActionManager.Scene.OnCity();
		}
	}
}
