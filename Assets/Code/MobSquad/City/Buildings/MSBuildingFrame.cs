using UnityEngine;
using System.Collections;

public class MSBuildingFrame : MonoBehaviour {

	public UISprite hoverIcon;

	void Awake(){
		hoverIcon = GetComponent<MSBuilding> ().hoverIcon;
	}
	
}
