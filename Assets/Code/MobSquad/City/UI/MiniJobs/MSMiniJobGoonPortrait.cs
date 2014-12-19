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

	public MSUIHelper minusButton;

	public PZMonster monster;

	public void Init(PZMonster monster)
	{
		this.monster = monster;

		MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix, monster.monster.imagePrefix + "Card", goon, 1, delegate{ MSSpriteUtil.instance.FitIn(goon, bg); });

		bg.spriteName = MSMiniGoonBox.elementBackgrounds[monster.monster.monsterElement];
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

	public void Minus()
	{
		MSMiniJobPopup.instance.RemoveFromTeam(this);
		minusButton.FadeOutAndOff();
	}
}
