using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKRequestPopup
/// </summary>
public class MSNewsPopup : MonoBehaviour {

	public static List<PvpHistoryProto> pvpHistory;

	[SerializeField]
	UIGrid requestGrid;

	[SerializeField]
	UIGrid attacksGrid;

	[SerializeField]
	MSTab requestTab;

	[SerializeField]
	MSTab attacksTab;

	[SerializeField]
	MSFacebookRequestEntry requestEntryPrefab;

	[SerializeField]
	MSAttackEntry attackEntryPrefab;

	[SerializeField]
	GameObject attacksParent;

	[SerializeField]
	GameObject requestsParent;

	List<MSFacebookRequestEntry> requestEntries = new List<MSFacebookRequestEntry>();

	List<MSAttackEntry> attackEntries = new List<MSAttackEntry>();

	void OnEnable()
	{
		InitAttacks();
	}

	public void InitAttacks()
	{
		requestsParent.SetActive(false);
		attacksParent.SetActive(true);

		requestTab.InitInactive();
		attacksTab.InitActive();

		RecycleAttackEntries();
		foreach (var item in pvpHistory) 
		{
			AddAttackEntry(item);
		}

		attacksGrid.Reposition();
	}

	void AddAttackEntry(PvpHistoryProto proto)
	{
		MSAttackEntry entry = MSPoolManager.instance.Get<MSAttackEntry>(attackEntryPrefab, attacksGrid.transform);
		entry.transform.localScale = Vector3.one;
		entry.Init (proto);

		attackEntries.Add(entry);
	}

	void RecycleAttackEntries()
	{
		foreach (var item in attackEntries) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}
		attackEntries.Clear();
	}

	public void InitRequests()
	{
		requestsParent.SetActive(true);
		attacksParent.SetActive(false);

		requestTab.InitActive();
		attacksTab.InitInactive();

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
