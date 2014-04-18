using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKRequestPopup
/// </summary>
public class MSRequestPopup : MonoBehaviour {

	[SerializeField]
	UIDragScrollView grid;

	[SerializeField]
	MSFacebookRequestEntry entryPrefab;

	List<MSFacebookRequestEntry> entries = new List<MSFacebookRequestEntry>();

	void OnEnable()
	{
		Init ();
	}

	void Init()
	{
		//Debug.Log("Initting request popup...");

		while (entries.Count < MSRequestManager.instance.invitesForMe.Count)
		{
			AddEntry();
		}

		int i = 0;
		for (; i < MSRequestManager.instance.invitesForMe.Count; i++) 
		{
			entries[i].gameObject.SetActive(true);
			entries[i].Init(MSRequestManager.instance.invitesForMe[i]);
		}
		for (; i < entries.Count; i++)  //Deactivate the rest of the entries that exist
		{
			entries[i].gameObject.SetActive(false);
		}

		grid.GetComponent<UIGrid>().Reposition();

		grid.collider.enabled = false;
		grid.collider.enabled = true;
	}

	public void AcceptButton()
	{
		foreach (var item in entries) 
		{
			item.TryAccept();
		}

		MSRequestManager.instance.SendAcceptRejectRequest();
	}

	void AddEntry()
	{
		//Debug.Log("Adding another entry");
		MSFacebookRequestEntry entry = Instantiate(entryPrefab) as MSFacebookRequestEntry;
		entry.transform.parent = grid.transform;
		entry.transform.localScale = Vector3.one;
		entry.GetComponent<MSUIHelper>().dragBehind = grid;
		entries.Add(entry);
	}
}
