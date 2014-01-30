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

	char[] delimiters = {'-'};

	BoosterPackProto pack;

	public CBKPickGachaScreen screen;

	public void Init(BoosterPackProto pack)
	{
		//Debug.Log("Init booster pack " + pack.boosterPackId + " banner");
		gameObject.SetActive(true);
		this.pack = pack;

		background.sprite2D = CBKAtlasUtil.instance.GetSprite( "Gacha/" + CBKUtil.StripExtensions(pack.listBackgroundImgName) );

		details.text = pack.listDescription;

		/*
		string[] detailList = pack.listDescription.Split(delimiters);

		details.text = detailList[0];
		for (int i = 1; i < detailList.Length; i++)
		{
			details.text += "\n" + detailList[i];
		}
		*/
	}

	void OnClick()
	{
		screen.ChooseBanner(pack);
	}
}
