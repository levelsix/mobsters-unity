using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSClanListScreen : MonoBehaviour {

	[SerializeField]
	UIInput searchBox;

	[SerializeField]
	MSActionButton searchButton;

	[SerializeField]
	MSClanListEntry entryPrefab;

	[SerializeField]
	UIGrid entryGrid;

	[SerializeField]
	MSClanPopup popup;

	[SerializeField]
	GameObject loading;

	List<MSClanListEntry> currEntries = new List<MSClanListEntry>();

	int beforeId = 0;

	public bool clansSearched = false;

	void OnEnable()
	{
		MSActionManager.Clan.OnPlayerClanChange += ChangeClan;
	}

	void OnDisable()
	{
		clansSearched = false;
		MSActionManager.Clan.OnPlayerClanChange -= ChangeClan;
	}

	public void ChangeClan(string a, UserClanStatus b, int c)
	{
		clansSearched = false;
	}

	public void Init()
	{
		beforeId = 0;
		if(!clansSearched)
		{
			StartCoroutine(SearchClans());
		}
		else
		{
			entryGrid.Reposition();
		}
	}

	IEnumerator SearchClans()
	{
		RecycleEntries();

		string search = searchBox.label.text;
		if (searchBox.label.color != searchBox.activeTextColor)
		{
			search = "";
		}

		loading.SetActive(true);
		IEnumerator searcher = MSClanManager.instance.SearchClanListing(search, beforeId);
		while(searcher.MoveNext())
		{
			yield return searcher.Current;
		}
		List<FullClanProtoWithClanSize> clans = MSClanManager.instance.postedClans;
		foreach (var item in clans){
			AddEntry (item);
		}
		loading.SetActive(false);

//		Debug.Log("Clans: " + entryGrid.transform.childCount);
		entryGrid.Reposition();

		clansSearched = true;
	}

	void AddEntry(FullClanProtoWithClanSize clan)
	{
		MSClanListEntry entry = MSPoolManager.instance.Get(entryPrefab, Vector3.zero) as MSClanListEntry;
		entry.Init(clan, popup);
		entry.transf.parent = entryGrid.transform;
		entry.transf.localScale = Vector3.one;
		foreach (var item in entry.GetComponentsInChildren<UIWidget>()) 
		{
			item.ParentHasChanged();
		}

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
