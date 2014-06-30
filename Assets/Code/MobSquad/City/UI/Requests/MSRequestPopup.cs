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
	UIGrid requestGrid;

	[SerializeField]
	MSFacebookRequestEntry requestEntryPrefab;

	[SerializeField]
	MSAttackEntry attackEntryPrefab;

	List<MSFacebookRequestEntry> requestEntries = new List<MSFacebookRequestEntry>();

	List<MSAttackEntry> attackEntries = new List<MSAttackEntry>();

	void OnEnable()
	{
		InitRequests();
	}

	public void InitRequests()
	{
		RecycleRequestEntries();
		foreach (var item in MSRequestManager.instance.invitesForMe) 
		{
			AddRequestEntry(item);
		}

		requestGrid.Reposition();
	}

	void AddRequestEntry(UserFacebookInviteForSlotProto invite)
	{
		//Debug.Log("Adding another entry");
		MSFacebookRequestEntry entry = (MSPoolManager.instance.Get(requestEntryPrefab.GetComponent<MSSimplePoolable>(),
		                                                          Vector3.zero,
		                                                           requestGrid.transform) as MSSimplePoolable).GetComponent<MSFacebookRequestEntry>();
		entry.transform.localScale = Vector3.one;
		entry.Init(invite);
		requestEntries.Add(entry);
	}

	void RecycleRequestEntries()
	{
		foreach (var item in requestEntries) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}
		requestEntries.Clear();
	}

	void OnRequestAcceptedOrDenied()
	{
		requestGrid.Reposition();
	}
}
