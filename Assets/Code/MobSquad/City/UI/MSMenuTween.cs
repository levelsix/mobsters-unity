using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMenuTween
/// Custom tween that tweens 
/// </summary>
public class MSMenuTween : TweenPosition {
	
	float screenWidth
	{
		get
		{
			return (Screen.width * 640f) / Screen.height ;
		}
	}

	Vector3 leftPos
	{
		get
		{
			return new Vector3(-screenWidth, 0, 0);
		}
	}

	Vector3 rightPos
	{
		get
		{
			return new Vector3(screenWidth, 0, 0);
		}
	}


	public void TweenIn()
	{
		from = rightPos;
		to = Vector3.zero;
		ResetToBeginning();
		PlayForward();
	}

	public void TweenClosed()
	{
		from = Vector3.zero;
		to = rightPos;
		ResetToBeginning();
		PlayForward ();
	}

	public void TweenBackIn()
	{
		from = leftPos;
		to = Vector3.zero;
		ResetToBeginning();
		PlayForward();
	}

	public void TweenOut()
	{
		from = Vector3.zero;
		to = leftPos;
		ResetToBeginning();
		PlayForward();
	}
}
