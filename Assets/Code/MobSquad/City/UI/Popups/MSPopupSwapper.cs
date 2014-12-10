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

	IEnumerator animationEnder;

	/// <summary>
	/// The reletive 
	/// </summary>
	public Vector3 Tween = new Vector3(0f, 0f, 0f);

	[SerializeField]
	bool SetOriginAtAwake = false;

	void Awake()
	{
		if(SetOriginAtAwake)
		{
			originA = popupA.transform.localPosition;
			unsetA = false;

			originB = popupB.transform.localPosition;
			unsetB = false;
		}
	}

	void OnDisable()
	{
		popupA.GetComponent<TweenPosition>().enabled = false;
		popupB.GetComponent<TweenPosition>().enabled = false;
		popupA.transform.localPosition = originA;
		popupB.transform.localPosition = originB;
		animationEnder = null;
	}

	public void Swap(bool resetOrigin = false){
		switch(activePopup){
		case Popup.None:
			if(animating && animationEnder != null){
				StopCoroutine(animationEnder);
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
			if(animating && animationEnder != null){
				StopCoroutine(animationEnder);
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
			if(animating && animationEnder != null){
				StopCoroutine(animationEnder);
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
		animationEnder = EndAnimation(activePopup);
		StartCoroutine(animationEnder);
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
				Debug.LogError("Incorrect popup specified", this);
				break;
			}
		}
	}

	/// <summary>
	/// Convenience function for swapping in a new popup.
	/// </summary>
	/// <param name="newPopup">New popup.</param>
	/// <param name="NewAction">New action.</param>
//	public void SwapIn(GameObject newPopup, bool forceNewOrigin = false)
//	{
//
//	}

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
