using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSRaidListScreen
/// </summary>
public class MSRaidListScreen : MonoBehaviour {

	[SerializeField] UITable table;

	[SerializeField] MSRaidListEntry raidListEntryPrefab;

	List<MSRaidListEntry> raidEntries = new List<MSRaidListEntry>();

	void OnEnable()
	{
		Init ();
	}

	void Init()
	{
		IDictionary events = MSDataManager.instance.GetAll<PersistentClanEventProto>();

		int i = 0;
		foreach (PersistentClanEventProto item in events.Values) 
		{
			if (raidEntries.Count <= i)
			{
				AddEntry();
			}
			raidEntries[i].gameObject.SetActive(true);
			raidEntries[i++].Init(item);
		}

		while (i < raidEntries.Count)
		{
			raidEntries[i++].gameObject.SetActive(false);
		}

		table.Reposition();
		table.collider.enabled = false;
		table.collider.enabled = true;
	}

	void AddEntry()
	{
		MSRaidListEntry entry = Instantiate(raidListEntryPrefab) as MSRaidListEntry;
		raidEntries.Add(entry);
		entry.transform.parent = table.transform;
		entry.transform.localScale = Vector3.one;

		entry.GetComponent<MSUIHelper>().dragBehind = table.GetComponent<UIDragScrollView>();
	}
}
