using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKClanListScreen : MonoBehaviour {

	[SerializeField]
	UIInput searchBox;

	[SerializeField]
	CBKActionButton searchButton;

	[SerializeField]
	Transform clanListParent;

	[SerializeField]
	CBKClanListEntry entryPrefab;

	[SerializeField]
	UIGrid entryGrid;

	List<CBKClanListEntry> currEntries = new List<CBKClanListEntry>();

	int beforeId = 0;

	public void Init()
	{
		beforeId = 0;
		StartCoroutine(SearchClans());
	}

	IEnumerator SearchClans()
	{
		RecycleEntries();

		string search = searchBox.label.text;
		if (searchBox.label.color != searchBox.activeTextColor)
		{
			search = " ";
		}
		IEnumerator searcher = MSClanManager.instance.SearchClanListing(search, beforeId);
		while(searcher.MoveNext())
		{

			yield return searcher.Current;
		}
		List<FullClanProtoWithClanSize> clans = MSClanManager.instance.postedClans;
		foreach (var item in clans){
			AddEntry (item);
		}
		//entryGrid.Reposition();
	}

	void AddEntry(FullClanProtoWithClanSize clan)
	{
		CBKClanListEntry entry = MSPoolManager.instance.Get(entryPrefab, Vector3.zero) as CBKClanListEntry;
		entry.Init(clan);
		entry.transf.parent = clanListParent;
		entry.transf.localScale = Vector3.one;
		currEntries.Add(entry);
	}

	void RecycleEntries()
	{
		foreach (var item in currEntries) 
		{
			item.Pool();
		}
		currEntries.Clear();
	}
}
