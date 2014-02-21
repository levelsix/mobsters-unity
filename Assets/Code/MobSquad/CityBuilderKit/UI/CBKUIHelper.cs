using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKUIHelper
/// Component to be attached to UI elements to allow NGUI callbacks to be
/// easily assigned from the inspector
/// </summary>
public class CBKUIHelper : MonoBehaviour {

	public float fadeTime = .6f;

	public void FadeIn()
	{
		TweenAlpha.Begin(gameObject, fadeTime, 1);
	}

	public void FadeOut()
	{
		TweenAlpha.Begin(gameObject, fadeTime, 0);
	}

	public void TurnOn()
	{
		gameObject.SetActive(true);
	}

	public void TurnOff()
	{
		gameObject.SetActive(false);
	}
}
