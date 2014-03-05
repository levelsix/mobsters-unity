using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSEvoScientistNumbers
/// </summary>
public class MSEvoScientistNumbers : MonoBehaviour {

	[SerializeField]
	UILabel[] numbers;

	public void Init(int[] scientists)
	{
		for (int i = 0; i < numbers.Length; i++) 
		{
			if (scientists.Length <= i)
			{
				numbers[i].text = "0";
			}
			else
			{
				numbers[i].text = MSMonsterManager.instance.GetMonstersByMonsterId(scientists[i]).Count.ToString();
			}
		}
	}
}
