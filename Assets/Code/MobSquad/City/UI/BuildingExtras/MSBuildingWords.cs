using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSBuildingWords
/// </summary>
public class MSBuildingWords : MonoBehaviour {

	[SerializeField]
	float time = 1;

	[SerializeField]
	UISprite sprite;

	[SerializeField]
	MSBuilding building;

	[SerializeField]
	string completeSpriteName;

	[SerializeField]
	string upgradeSpriteName;

	TweenPosition pos;
	TweenAlpha alph;

	void Awake()
	{
		pos = GetComponent<TweenPosition>();
		alph = GetComponent<TweenAlpha>();
	}

	void OnEnable()
	{
		if (building.upgrade != null)
		{
			building.upgrade.OnFinishUpgrade += OnUpgradeFinish;
		}
	}

	void OnDisable()
	{
		if (building.upgrade != null)
		{
			building.upgrade.OnFinishUpgrade -= OnUpgradeFinish;
		}
	}

	void OnUpgradeFinish()
	{
		pos.duration = time;
		pos.ResetToBeginning();
		pos.PlayForward();

		alph.duration = time;
		alph.ResetToBeginning();
		alph.PlayForward();

		sprite.spriteName = building.upgrade.level == 1 ? completeSpriteName : upgradeSpriteName;
	}
}
