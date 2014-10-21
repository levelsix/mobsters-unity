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

	bool reverse = true;

	public bool SetMode(SortingMode mode)
	{
		if (mode == sortingMode)
		{
			reverse = !reverse;
		}
		else
		{
			reverse = true;
		}
		sortingMode = mode;
		Reposition();
		return reverse;
	}

	public void SetATKMode() { sortingMode = SortingMode.ATK; }
	public void SetHPMode() { sortingMode = SortingMode.HP; }
	
	public int SortByHP (Transform a, Transform b) 
	{ 
		if (reverse)
		{
			return b.GetComponent<MSMiniJobGoonie>().goonie.currHP.CompareTo
				(a.GetComponent<MSMiniJobGoonie>().goonie.currHP); 
		}
		return a.GetComponent<MSMiniJobGoonie>().goonie.currHP.CompareTo
		                     (b.GetComponent<MSMiniJobGoonie>().goonie.currHP); 
	}

	public int SortByATK (Transform a, Transform b) 
	{ 
		if (reverse)
		{
			return b.GetComponent<MSMiniJobGoonie>().goonie.totalDamage.CompareTo
				(a.GetComponent<MSMiniJobGoonie>().goonie.totalDamage);
		}
		return a.GetComponent<MSMiniJobGoonie>().goonie.totalDamage.CompareTo
		                   (b.GetComponent<MSMiniJobGoonie>().goonie.totalDamage); 
	}

	protected override void Sort (List<Transform> list)
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
