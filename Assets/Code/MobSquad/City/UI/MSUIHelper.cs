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
public class MSUIHelper : MonoBehaviour {

	public float fadeTime = .6f;
	
	public UIDragScrollView dragBehind;

	MSSimplePoolable poolable;

	void Awake()
	{
		poolable = GetComponent<MSSimplePoolable>();
	}

	public void ResetAlpha(bool on)
	{
		TweenAlpha.Begin(gameObject, 0, on ? 1 : 0); 
	}

	public TweenAlpha FadeIn()
	{
		return TweenAlpha.Begin(gameObject, fadeTime, 1);
	}

	public TweenAlpha FadeOut()
	{
		return TweenAlpha.Begin(gameObject, fadeTime, 0);
	}

	public TweenAlpha Fade(bool fadeIn)
	{
		return fadeIn ? FadeIn() : FadeOut();
	}

	public void FadeOutAndOff()
	{
		StartCoroutine(DoFadeOutThenDisable());
	}

	IEnumerator DoFadeOutThenDisable()
	{
		TweenAlpha alph = FadeOut();
		while (alph.tweenFactor < 1)
		{
			yield return null;
		}
		TurnOff();
	}

	public void FadeOutAndPool()
	{
		StartCoroutine(DoFadeOutThenPool());
	}

	IEnumerator DoFadeOutThenPool()
	{
		TweenAlpha alph = FadeOut();
		while (alph.tweenFactor < 1)
		{
			yield return null;
		}
		if (poolable != null)
		{
			poolable.Pool();
		}
		else
		{
			TurnOff();
		}
	}

	public void TurnOn()
	{
		gameObject.SetActive(true);
	}

	public void TurnOff()
	{
		gameObject.SetActive(false);
	}
	
	public void OnPress(bool pressed)
	{
		if (dragBehind != null)
		{
			dragBehind.OnPress(pressed);
		}
	}

	public void OnDrag(Vector2 delta)
	{
		if (dragBehind != null)
		{
			dragBehind.OnDrag(delta);
		}
	}
}
