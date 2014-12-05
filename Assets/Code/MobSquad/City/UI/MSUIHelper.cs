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

	public float targetAlpha = 1;
	
	public UIDragScrollView dragBehind;

	MSSimplePoolable poolable;

	void Awake()
	{
		poolable = GetComponent<MSSimplePoolable>();
	}

	public void ResetAlpha(bool on)
	{
		if (on) gameObject.SetActive(true);

		TweenAlpha.Begin(gameObject, 0, on ? targetAlpha : 0); 
	}

	/// <summary>
	/// Fade in, to be called from the editor using UIButtons
	/// and other delegate systems
	/// </summary>
	public void EditorFadeIn()
	{
		FadeIn();
	}

	public TweenAlpha FadeIn()
	{
		gameObject.SetActive(true);
		return TweenAlpha.Begin(gameObject, fadeTime, targetAlpha);
	}
	
	/// <summary>
	/// Fade out, to be called from the editor using UIButtons
	/// and other delegate systems
	/// </summary>
	public void EditorFadeOut()
	{
		FadeOut();
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
