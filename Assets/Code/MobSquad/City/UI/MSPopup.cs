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

	[SerializeField] protected UITweener[] inTweens;

	[SerializeField] protected UITweener[] outTweens;

	public virtual void Popup()
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
	}

	[ContextMenu ("Close")]
	public virtual void Close(bool all = false)
	{
		StartCoroutine(RunOutTweens(all));
	}

	/// <summary>
	/// Convieniece function to allow closing all popups from button OnClick
	/// </summary>
	public void CloseAll(){
		Close (true);
	}

	protected virtual IEnumerator RunOutTweens(bool all)
	{
		foreach (var item in inTweens) 
		{
			item.tweenFactor = 1;
		}
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
		gameObject.SetActive(false);
	}
}
