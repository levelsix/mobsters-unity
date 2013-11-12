using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Simple script to attach to an NGUI element to make it take the user
/// back to the city view
/// </summary>
public class PZCityModeButton : MonoBehaviour {

	void OnClick()
	{
		Debug.Log("To City");
		CBKWhiteboard.currSceneType = CBKWhiteboard.SceneType.CITY;
		CBKValues.Scene.ChangeScene(CBKValues.Scene.Scenes.LOADING_SCENE);
	}
}
