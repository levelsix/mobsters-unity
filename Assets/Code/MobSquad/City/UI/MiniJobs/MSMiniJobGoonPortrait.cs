using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobGoonPortrait
/// </summary>
[RequireComponent (typeof (MSSimplePoolable))]
[RequireComponent (typeof (MSUIHelper))]
public class MSMiniJobGoonPortrait : MonoBehaviour {

	[SerializeField]
	UI2DSprite goon;

	[SerializeField]
	UISprite bg;

	public void Init(PZMonster monster)
	{
		StartCoroutine(MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix, monster.monster.imagePrefix + "Thumbnail", goon));

		//TODO: Set background
	}

	public void ResetPanel()
	{
		goon.ParentHasChanged();
		bg.ParentHasChanged();
		
		goon.alpha = 1;
		bg.alpha = 1;
	}

	public void Pool()
	{
		GetComponent<MSSimplePoolable>().Pool();
	}
}
