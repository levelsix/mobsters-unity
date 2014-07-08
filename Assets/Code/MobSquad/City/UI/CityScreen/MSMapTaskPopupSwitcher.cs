using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSMapTaskPopupSwitcher : MonoBehaviour {

	enum Popup
	{
		A,
		B,
		None
	}
	
	Popup activePopup = Popup.None;
	
	[SerializeField]
	MSMapTaskPopup popupA;
	
	[SerializeField]
	MSMapTaskPopup popupB;

	bool animating = false;

	static float ANIMATION_TIME = 0.2f;

	void OnEnable(){
		activePopup = Popup.None;
		MSActionManager.Map.OnMapTaskClicked += SwapPopup;
		MSActionManager.Popup.CloseAllPopups += Close;
	}

	void OnDisable(){
		MSActionManager.Map.OnMapTaskClicked -= SwapPopup;
		MSActionManager.Popup.CloseAllPopups -= Close;
		popupA.trans.localPosition = new Vector3 (popupA.trans.localPosition.x, 0f, popupA.trans.localPosition.z);
		popupB.trans.localPosition = new Vector3 (popupB.trans.localPosition.x, 0f, popupB.trans.localPosition.z);
	}
	
	void SwapPopup(TaskMapElementProto mapTask, MSMapTaskButton.TaskStatusType status){
		if(!animating){
			animating = true;

			StartCoroutine(endAnimation(ANIMATION_TIME));

			Vector3 startA = popupA.trans.localPosition;
			Vector3 startB = popupB.trans.localPosition;

			if (activePopup == Popup.None) {
				TweenPosition.Begin(popupA.gameObject, ANIMATION_TIME, new Vector3(startA.x, startA.y + 100, startA.z));

				popupA.init(mapTask, status);
				activePopup = Popup.A;
			} else if(activePopup == Popup.A) {
				TweenPosition.Begin(popupB.gameObject, ANIMATION_TIME, new Vector3(startB.x, startB.y + 100, startB.z));
				TweenPosition.Begin(popupA.gameObject, ANIMATION_TIME, new Vector3(startA.x, startA.y - 100, startA.z));
				
				popupB.init(mapTask, status);
				activePopup = Popup.B;
			} else {
				TweenPosition.Begin(popupA.gameObject, ANIMATION_TIME, new Vector3(startA.x, startA.y + 100, startA.z));
				TweenPosition.Begin(popupB.gameObject, ANIMATION_TIME, new Vector3(startB.x, startB.y - 100, startB.z));
				
				popupA.init(mapTask, status);
				activePopup = Popup.A;
			}

		}
	}

	void Close(){
		Vector3 startA = popupA.trans.localPosition;
		Vector3 startB = popupB.trans.localPosition;
		TweenPosition.Begin(popupA.gameObject, ANIMATION_TIME, new Vector3(startA.x, 0, startA.z));
		TweenPosition.Begin(popupB.gameObject, ANIMATION_TIME, new Vector3(startB.x, 0, startB.z));
	}

	IEnumerator endAnimation(float duration){
		yield return new WaitForSeconds (duration);
		animating = false;
	}
}
