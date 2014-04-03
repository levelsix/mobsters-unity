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
		menuSlide.TweenIn();
		base.Popup ();
	}

	protected override IEnumerator RunOutTweens ()
	{
		foreach (var item in inTweens) 
		{
			item.tweenFactor = 1;
		}
		if (MSPopupManager.instance.top != null)
		{
			menuSlide.TweenClosed();
			while (menuSlide.tweenFactor < 1)
			{
				yield return null;
			}
		}
		else
		{
			foreach (var item in outTweens) 
			{
				item.ResetToBeginning();
				item.Play();
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
