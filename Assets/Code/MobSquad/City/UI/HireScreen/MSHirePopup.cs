using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSHirePopup
/// </summary>
public class MSHirePopup : MonoBehaviour {

	[SerializeField]
	MSHireEntry hireEntryPrefab;

	[SerializeField]
	UIGrid grid;

	List<MSHireEntry> hireEntries = new List<MSHireEntry>();

	public void Init(MSBuilding residence)
	{
		MSFullBuildingProto thisLevel = residence.combinedProto.baseLevel;
		int i = 0;
		while (thisLevel != null)
		{
			while (hireEntries.Count <= i)
			{
				AddHireEntry();
			}
			
			if (thisLevel.structInfo.level > residence.combinedProto.structInfo.level)
			{
				hireEntries[i].Init(thisLevel.residence, "Requires a level " + thisLevel.structInfo.level + " residence");
			}
			else if (thisLevel.structInfo.level > residence.userStructProto.fbInviteStructLvl + 1)
			{
				hireEntries[i].Init(thisLevel.residence, "Requires a " + thisLevel.predecessor.residence.occupationName);
			}
			else
			{
				hireEntries[i].Init(thisLevel.residence, thisLevel.structInfo.level <= residence.userStructProto.fbInviteStructLvl, residence.userStructProto.userStructId);
			}
			
			thisLevel = thisLevel.successor;
			i++;
		}
		grid.Reposition();
	}

	void AddHireEntry()
	{
		MSHireEntry entry = Instantiate(hireEntryPrefab) as MSHireEntry;
		entry.transform.parent = grid.transform;
		entry.transform.localScale = Vector3.one;
		entry.transform.localPosition = Vector3.zero;
		hireEntries.Add(entry);
	}
}
