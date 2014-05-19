using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobGoonSlot
/// </summary>
public class MSMiniJobGoonSlot : MonoBehaviour {

	[SerializeField]
	MSUIHelper minus;

	[SerializeField]
	MSMiniJobPopup popup;

	MSMiniJobGoonPortrait portrait;

	PZMonster monster;

	public bool isOpen
	{
		get
		{
			return portrait == null;
		}
	}

	public void Reset()
	{
		if (portrait != null)
		{
			portrait.Pool();
		}
		monster = null;
		portrait = null;
		minus.TurnOff();
	}

	public void InsertMonster(PZMonster monster, MSMiniJobGoonPortrait portrait)
	{
		this.portrait = portrait;
		this.monster = monster;

		portrait.transform.parent = transform;

		portrait.ResetPanel();

		TweenPosition tween = TweenPosition.Begin(portrait.gameObject, .2f, Vector3.zero);
		StartCoroutine(WaitForTween(tween));
	}

	IEnumerator WaitForTween(TweenPosition tween)
	{
		while (tween.tweenFactor < 1)
		{
			yield return null;
		}
		minus.TurnOn();
	}

	public void Minus()
	{
		minus.TurnOff();
		portrait.GetComponent<MSUIHelper>().FadeOutAndPool();

		popup.InsertGoonEntry(monster, true);

		popup.currTeam.Remove(monster);
		popup.CalculateBars();

		portrait = null;
		monster = null;
	}

}
