using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MSPopupSwapper : MonoBehaviour {

	[SerializeField]
	float tweenTime = 0.2f;

	[SerializeField]
	GameObject popupA;
	
	[SerializeField]
	GameObject popupB;

	Vector3 originA;

	Vector3 originB;

	TweenPosition curTween;

	bool unset = true;

	public enum Popup
	{
		A,
		B,
		None
	}

	[HideInInspector]
	public Popup activePopup = Popup.None;

	bool animating = false;

	public Action ActionA;

	public Action ActionB;

	public float waitThenReverse = -1f;

	/// <summary>
	/// The reletive 
	/// </summary>
	public Vector3 Tween = new Vector3(0f, 0f, 0f);

	public void Swap(){
		///This is something that would normally go in Awake but
		/// in some cases this need to wait for animations to finish
		if(unset){
			Debug.Log("beep boop");
			originA = popupA.transform.localPosition;
			originB = popupB.transform.localPosition;
			unset = false;
		}

		switch(activePopup){
		case Popup.None:
			if(animating){
				popupA.transform.localPosition = originA;
				StopCoroutine("EndAnimation");
			}
			TweenPosition.Begin(popupA.gameObject, tweenTime, new Vector3(originA.x + Tween.x, originA.y + Tween.y, originA.z + Tween.z));
			if(ActionA != null){
				ActionA();
			}
			activePopup = Popup.A;
			break;
		case Popup.A:
			if(animating){
				popupB.transform.localPosition = originB;
				StopCoroutine("EndAnimation");
			}
			TweenPosition.Begin(popupB.gameObject, tweenTime, new Vector3(originB.x + Tween.x, originB.y + Tween.y, originB.z + Tween.z));
			TweenPosition.Begin(popupA.gameObject, tweenTime, originA);
			if(ActionB != null){
				ActionB();
			}
			activePopup = Popup.B;
			break;
		case Popup.B:
			if(animating){
				popupA.transform.localPosition = originA;
				StopCoroutine("EndAnimation");
			}
			TweenPosition.Begin(popupA.gameObject, tweenTime, new Vector3(originA.x + Tween.x, originA.y + Tween.y, originA.z + Tween.z));
			TweenPosition.Begin(popupB.gameObject, tweenTime, originB);
			if(ActionA != null){
				ActionA();
			}
			activePopup = Popup.A;
			break;
		}
		animating = true;
		StartCoroutine("EndAnimation", activePopup);
	}

	public void Close(){
		curTween = TweenPosition.Begin(popupA.gameObject, tweenTime, originA);
		TweenPosition.Begin(popupB.gameObject, tweenTime, originB);
		activePopup = Popup.None;
	}

	IEnumerator EndAnimation(Popup pop){
		yield return new WaitForSeconds(tweenTime);
		if(waitThenReverse >= 0f){
			yield return new WaitForSeconds(waitThenReverse);
			switch(pop){
			case Popup.A:
				TweenPosition.Begin(popupA.gameObject, tweenTime, originA);
				break;
			case Popup.B:
				TweenPosition.Begin(popupB.gameObject, tweenTime, originB);
				break;
			default:
				break;
			}
		}
	}
}
