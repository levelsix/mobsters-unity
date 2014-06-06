using UnityEngine;
using System.Collections;

public abstract class MSBuildingFrame : MonoBehaviour {

	public UISprite hoverIcon;

	void Awake(){
		hoverIcon = GetComponent<MSBuilding> ().hoverIcon;
	}

	public void CheckTag (){
		Debug.LogError ("CheckTag not implemented for some building");
	}
	
}
