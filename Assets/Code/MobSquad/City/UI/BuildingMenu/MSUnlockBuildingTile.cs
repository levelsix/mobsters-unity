﻿using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSUnlockBuildingTile : MonoBehaviour {

	[SerializeField]
	UILabel tag;

	[SerializeField]
	UI2DSprite buildingIcon;

	public void Init(string tag, StructureInfoProto building)
	{
		this.tag.text = tag;
		Sprite sprite = MSSpriteUtil.instance.GetBuildingSprite(building.imgName);
		buildingIcon.sprite2D = sprite;
		buildingIcon.MakePixelPerfect();
	}
}
