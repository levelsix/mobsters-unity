using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSEvoScientistButton
/// </summary>
public class MSEvoScientistButton : MonoBehaviour {

	[SerializeField]
	int orderId;

	[SerializeField]
	int[] scientistIds;

	[SerializeField]
	UILabel numLabel;

	[SerializeField]
	UITable table;

	[SerializeField]
	MSEvoScientistNumbers numbers;

	void OnEnable()
	{
		int sciCount = 0;
		foreach (var item in scientistIds)
		{
			sciCount += MSMonsterManager.instance.GetMonstersByMonsterId(item).Count;
		}
		numLabel.text = "x" + sciCount;

		if (orderId == 1)
		{
			OnClick();
		}
	}

	void OnClick()
	{
//		Debug.Log("Clicked on a scientist");
		numbers.Init(scientistIds);
		numbers.name = orderId + " 2 Numbers";
		table.Reposition();
	}
}
