using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobGoonGrid
/// </summary>
public class MSMiniJobGoonGrid : UIGrid {

	public enum SortingMode {HP, ATK};

	public SortingMode sortingMode = SortingMode.HP;

	public void SetATKMode() { sortingMode = SortingMode.ATK; }
	public void SetHPMode() { sortingMode = SortingMode.HP; }
	
	static public int SortByHP (Transform a, Transform b) 
	{ 
		return a.GetComponent<MSMiniJobGoonie>().goonie.currHP.CompareTo
		                     (b.GetComponent<MSMiniJobGoonie>().goonie.currHP); 
	}
	static public int SortByATK (Transform a, Transform b) 
	{ 
		return a.GetComponent<MSMiniJobGoonie>().goonie.totalDamage.CompareTo
		                   (b.GetComponent<MSMiniJobGoonie>().goonie.totalDamage); 
	}

	protected override void Sort (BetterList<Transform> list)
	{
		switch (sortingMode) 
		{
		case SortingMode.ATK:
			list.Sort(SortByATK);
			break;
		case SortingMode.HP:
			list.Sort(SortByHP);
			break;
		default:
			break;
		}
	}
}
