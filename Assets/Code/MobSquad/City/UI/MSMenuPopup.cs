using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMenuPopup
/// </summary>
[RequireComponent (typeof (MSMenuTween))]
public class MSMenuPopup : MSPopup {

	MSMenuTween menuSlide;

	void Awake()
	{
		menuSlide = GetComponent<MSMenuTween>();
	}

	public override void Popup ()
	{
		gameObject.SetActive(true);
		if (MSPopupManager.instance.backPop != null)
		{
			menuSlide.TweenIn();
			foreach (var item in inTweens) 
			{
				item.Sample(1, true);
			}
		}
		else
		{
			transform.localPosition = Vector3.zero;
			StartCoroutine(RunInTweens());
			MSMenuTopper.instance.Popup();
		}
	}

	IEnumerator RunInTweens()
	{
		gameObject.SetActive(true);
		foreach (var item in outTweens) 
		{
			item.tweenFactor = 1;
		}
		foreach (var item in inTweens) 
		{
			item.ResetToBeginning();
			item.PlayForward();
		}
		foreach (var item in inTweens) 
		{
			while (item.tweenFactor < 1)
			{
				yield return null;
			}
		}
		MSTownCamera.instance.gameObject.SetActive(false);
	}

	protected override IEnumerator RunOutTweens (bool all)
	{
		foreach (var item in inTweens) 
		{
			item.tweenFactor = 1;
		}
		if (!all && MSPopupManager.instance.top != null)
		{
			menuSlide.TweenClosed();
			while (menuSlide.tweenFactor < 1)
			{
				yield return null;
			}
		}
		else
		{
			MSTownCamera.instance.gameObject.SetActive(true);
			MSMenuTopper.instance.Close(all);
			foreach (var item in outTweens) 
			{
				item.ResetToBeginning();
				item.PlayForward();
			}
			foreach (var item in outTweens) 
			{
				while(item.tweenFactor < 1)
				{
					yield return null;
				}
			}
		}
		gameObject.SetActive(false);
	}

	public void SlideOut()
	{
		menuSlide.TweenOut();
	}

	public void SlideBackIn()
	{
		menuSlide.TweenBackIn();
	}
}
