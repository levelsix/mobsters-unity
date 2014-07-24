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

	bool unsetA = true;
	bool unsetB = true;

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

	public void Swap(bool resetOrigin = false){
		switch(activePopup){
		case Popup.None:
			if(animating){
				StopCoroutine("EndAnimation");
			}

			if(ActionA != null){
				ActionA();
			}

			if(unsetA || resetOrigin)
			{
				originA = popupA.transform.localPosition;
				unsetA = false;
			}


			TweenPosition.Begin(popupA.gameObject, tweenTime, new Vector3(originA.x + Tween.x, originA.y + Tween.y, originA.z + Tween.z));

			activePopup = Popup.A;
			break;
		case Popup.A:
			if(animating){
				StopCoroutine("EndAnimation");
			}
			if(ActionB != null){
				ActionB();
			}
			if(unsetB || resetOrigin)
			{
				originB = popupB.transform.localPosition;
				unsetB = false;
			}

			TweenPosition.Begin(popupB.gameObject, tweenTime, new Vector3(originB.x + Tween.x, originB.y + Tween.y, originB.z + Tween.z));
			TweenPosition.Begin(popupA.gameObject, tweenTime, originA);

			activePopup = Popup.B;
			break;
		case Popup.B:
			if(animating){
				StopCoroutine("EndAnimation");
			}
			if(ActionA != null){
				ActionA();
			}

			if(unsetA || resetOrigin)
			{
				originA = popupA.transform.localPosition;
				unsetA = false;
			}


			TweenPosition.Begin(popupA.gameObject, tweenTime, new Vector3(originA.x + Tween.x, originA.y + Tween.y, originA.z + Tween.z));
			TweenPosition.Begin(popupB.gameObject, tweenTime, originB);

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

	/// <summary>
	/// Convenience function for swapping in a new popup.
	/// </summary>
	/// <param name="newPopup">New popup.</param>
	/// <param name="NewAction">New action.</param>
	public void SwapIn(GameObject newPopup, Action NewAction = null, bool forceNewOrigin = false)
	{
		if(activePopup == Popup.A)
		{
			popupB = newPopup;
			ActionB = NewAction;
		}
		else
		{
			popupA = newPopup;
			ActionA = NewAction;
		}
		Swap(forceNewOrigin);
	}
}
