using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Tutorial
/// </summary>
[System.Serializable]
public class MSTutorial {

	[SerializeField] protected List<MSTutorialStep> steps;

	protected bool clicked = false;

	protected virtual IEnumerator RunStep(MSTutorialStep step)
	{
		clicked = false;
		while (!clicked)
		{
			yield return null;
		}

		yield return null;
	}

	public virtual IEnumerator Run()
	{
		foreach (var item in steps) 
		{
			yield return MSTutorialManager.instance.StartCoroutine(RunStep(item));
		}
	}

	[ContextMenu ("Click Dialogue")]
	public void OnDialogueClicked()
	{
		clicked = true;
	}
}
