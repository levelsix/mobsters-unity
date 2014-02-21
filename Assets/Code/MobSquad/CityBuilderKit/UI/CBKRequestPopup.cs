using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKRequestPopup
/// </summary>
public class CBKRequestPopup : MonoBehaviour {

	[SerializeField]
	UIDragScrollView grid;

	[SerializeField]
	CBKFacebookRequestEntry entryPrefab;

	List<CBKFacebookRequestEntry> entries = new List<CBKFacebookRequestEntry>();

	void OnEnable()
	{
		Init ();
	}

	void Init()
	{
		//Debug.Log("Initting request popup...");

		while (entries.Count < CBKRequestManager.invitesForMe.Count)
		{
			AddEntry();
		}

		int i = 0;
		for (; i < CBKRequestManager.invitesForMe.Count; i++) 
		{
			entries[i].gameObject.SetActive(true);
			entries[i].Init(CBKRequestManager.invitesForMe[i]);
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

		CBKRequestManager.instance.SendAcceptRejectRequest();
	}

	void AddEntry()
	{
		//Debug.Log("Adding another entry");
		CBKFacebookRequestEntry entry = Instantiate(entryPrefab) as CBKFacebookRequestEntry;
		entry.transform.parent = grid.transform;
		entry.transform.localScale = Vector3.one;
		entry.button.dragBehind = grid;
		entries.Add(entry);
	}
}
