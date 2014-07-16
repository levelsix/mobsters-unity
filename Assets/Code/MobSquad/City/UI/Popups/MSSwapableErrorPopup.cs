using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MSPopupSwapper))]
public class MSSwapableErrorPopup : MonoBehaviour {

	[SerializeField]
	UILabel ErrorLabelA;

	[SerializeField]
	UILabel ErrorLabelB;

	MSPopupSwapper swapper;

	void Awake(){
		swapper = GetComponent<MSPopupSwapper>();
	}

	void OnEnable(){
		MSActionManager.Popup.DisplayError += initError;
	}

	void OnDisable(){
		MSActionManager.Popup.DisplayError -= initError;
	}

	void initError(string text){
		if(swapper.activePopup == MSPopupSwapper.Popup.A){
			ErrorLabelB.text = text;
		}else{
			ErrorLabelA.text = text;
		}
		swapper.Swap();
	}
}
