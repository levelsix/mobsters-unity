using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// CBKGachaBanner
/// @author Rob Giusti
/// </summary>
public class CBKGachaBanner : MonoBehaviour {

	[SerializeField]
	UI2DSprite background;

	[SerializeField]
	UILabel details;

	BoosterPackProto pack;

	public CBKPickGachaScreen screen;

	public void Init(BoosterPackProto pack)
	{
		gameObject.SetActive(true);
		this.pack = pack;

		background.sprite2D = MSAtlasUtil.instance.GetSprite( "Gacha/" + MSUtil.StripExtensions(pack.listBackgroundImgName) );

		details.text = pack.listDescription;
	}

	void OnClick()
	{
		screen.ChooseBanner(pack);
	}
}
