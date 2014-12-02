using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSPopup
/// </summary>
public class MSPopup : MonoBehaviour {

	[SerializeField] bool defaultIn = false;

	[SerializeField] bool defaultOut = false;

	[SerializeField] GameObject defaultTarget;

	[SerializeField] protected UITweener[] inTweens;

	[SerializeField] protected UITweener[] outTweens;

	[SerializeField] bool defaultInSound = true;

	[SerializeField] bool defaultOutSound = true;

	/// <summary>
	/// The poolable component, if any.
	/// If there is a poolable component, this popup will
	/// pool when it finishes its outro.
	/// Otherwise, it'll just turn itself off.
	/// </summary>
	MSSimplePoolable poolable;

	void Awake()
	{
		poolable = GetComponent<MSSimplePoolable>();
	}

	public virtual void Popup()
	{
		if (defaultInSound) MSSoundManager.instance.PlayOneShot (MSSoundManager.instance.defaultPopupIn);
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
		if (defaultIn)
		{
			MSPopupManager.instance.DefaultTweenIn(defaultTarget);
		}
	}

	[ContextMenu ("Close")]
	public virtual void Close(bool all = false)
	{
		StartCoroutine(RunOutTweens(all));
		if (defaultOutSound) MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.defaultPopupOut);
	}

	/// <summary>
	/// Convieniece function to allow closing all popups from button OnClick
	/// </summary>
	public void CloseAll(){
		Close (true);
	}

	protected virtual IEnumerator RunOutTweens(bool all)
	{
		float maxDuration = 0;
		foreach (var item in inTweens) 
		{
			item.tweenFactor = 1;
		}
		foreach (var item in outTweens) 
		{
			item.ResetToBeginning();
			item.PlayForward();
			maxDuration = Mathf.Max(maxDuration, item.duration);
		}
		if (defaultOut)
		{
			maxDuration = Mathf.Max(maxDuration, MSPopupManager.instance.DefaultTweenOut(defaultTarget));
		}

		yield return new WaitForSeconds(maxDuration);

		if (poolable != null)
		{
			poolable.Pool();
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}
