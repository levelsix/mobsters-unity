using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSClanHelpScreen : MonoBehaviour {
	
	public MSClanHelpListing listingPrefab;

	public UILabel noRequestsLabel;

	public UIGrid grid;

	public UIButton helpAllButton;
	
	public UIScrollView scrollview;

	public MSBadge badge;

//	void OnEnable()
//	{
//		MSActionManager.Clan.OnEndClanHelp += LiveEndClanHelp;
//		MSActionManager.Clan.OnGiveClanHelp += LiveGiveClanHelp;
//		MSActionManager.Clan.OnSolicitClanHelp += LiveSolicitClanHelp;
//	}
//
//	void OnDisable()
//	{
//		MSActionManager.Clan.OnEndClanHelp -= LiveEndClanHelp;
//		MSActionManager.Clan.OnGiveClanHelp -= LiveGiveClanHelp;
//		MSActionManager.Clan.OnSolicitClanHelp -= LiveSolicitClanHelp;
//	}

	public void Init()
	{
		MSClanHelpManager.instance.InitHelpScreen();
	}

//	public void Init()
//	{
//		foreach(KeyValuePair<int, List<MSClanHelpListing>> userList in listings)
//		{
//			foreach(MSClanHelpListing listing in userList.Value)
//			{
//				listing.GetComponent<MSSimplePoolable>().Pool();
//			}
//		}
//		listings.Clear();
//
//		foreach(ClanHelpProto help in MSClanManager.instance.clanHelpRequests)
//		{
//			if(help.mup.userId != MSWhiteboard.localMup.userId && !help.helperIds.Contains(MSWhiteboard.localMup.userId) && help.helperIds.Count < help.maxHelpers && help.open)
//			{
//				AddNewListing(help);
//			}
//		}
//
//		this.grid.Reposition();
//
//		UpdateButtonAndBackground();
//
//		helpAllButton.onClick.Clear();
//		EventDelegate.Add(helpAllButton.onClick, delegate {OnHelpAll();});
//	}

//	IEnumerator RepositionAfterFrame()
//	{
//		yield return null;
//		scrollview.GetComponent<UIScrollView>().ResetPosition();
//	}
//
//	void UpdateButtonAndBackground()
//	{
//		int helpable = 0;
//		foreach(KeyValuePair<int, List<MSClanHelpListing>> userList in listings)
//		{
//			foreach(MSClanHelpListing listing in userList.Value)
//			{
//				if(listing.stillHelpable)
//				{
//					helpable++;
//				}
//			}
//		}
//
//		noRequestsLabel.gameObject.SetActive( listings.Count == 0 );
//		helpAllButton.gameObject.SetActive( helpable > 0 );
//		badge.notifications = helpable;
//		StartCoroutine(RepositionAfterFrame());
//	}
//
//	void AddNewListing(ClanHelpProto proto)
//	{
//		if(proto.helpType == ClanHelpType.HEAL)
//		{
////			Debug.Log("adding a heal");
//			//If there's no list for this user, make one
//			bool existingHealListing = false;
//			if(!listings.ContainsKey(proto.mup.userId))
//			{
////				Debug.Log("Making a new dictionary entry");
//				listings.Add(proto.mup.userId, new List<MSClanHelpListing>());
//			}
//			else
//			{
////				Debug.Log("check for existing listing");
//				List<MSClanHelpListing> curUserListing = listings[proto.mup.userId];
//				foreach(MSClanHelpListing listing in curUserListing)
//				{
//					if(listing.AddHealingProto(proto))
//					{
////						Debug.Log("found listing and added healing proto");
//						existingHealListing = true;
//						break;
//					}
//				}
//
//				if(!existingHealListing)
//				{
////					Debug.Log("couldn't find a healing listing");
//				}
//			}
//			
//			//if there isn't we make one
//			if(!existingHealListing && proto.helperIds.Count < proto.maxHelpers && proto.open)
//			{
//
////				Debug.Log("create new listing");
//				MSClanHelpListing newListing = MSPoolManager.instance.Get<MSClanHelpListing>(listingPrefab, grid.transform);
//				newListing.transform.localScale = Vector3.one;
//				newListing.dragView.scrollView = scrollview;
//				newListing.Init(proto);
//				
//				listings[proto.mup.userId].Add(newListing);
//
//				grid.Reposition();
//			}
//			else
//			{
////				Debug.Log("nothing I guess");
//			}
//		}
//		else if(!proto.helperIds.Contains(MSWhiteboard.localMup.userId) && proto.helperIds.Count < proto.maxHelpers && proto.open)
//		{
////			listings.ContainsKey
//			MSClanHelpListing newListing = MSPoolManager.instance.Get<MSClanHelpListing>(listingPrefab, grid.transform);
//			newListing.transform.localScale = Vector3.one;
//			newListing.dragView.scrollView = scrollview;
//			if(!listings.ContainsKey(proto.mup.userId))
//			{
//				listings.Add(proto.mup.userId,new List<MSClanHelpListing>());
//			}
//			newListing.Init(proto);
//			listings[proto.mup.userId].Add(newListing);
//		}
//
//		grid.Reposition();
//	}
//
//	MSClanHelpListing FindExistingHelpListing(ClanHelpProto findMe)
//	{
//		List<MSClanHelpListing> curUserListing = listings[findMe.mup.userId];
//		foreach(MSClanHelpListing listing in curUserListing)
//		{
//			if(listing.Contains(findMe))
//			{
//				return listing;
//			}
//		}
//
//		return null;
//	}
//
//	//'Live' functions are called while the list is already open
//	//so it updates the list while the player is looking at it
//	void LiveGiveClanHelp(GiveClanHelpResponseProto proto, bool self)
//	{
//		if(self || proto.sender.userId != MSWhiteboard.localMup.userId)
//		{
//			foreach(ClanHelpProto helpProto in proto.clanHelps)
//			{
//				MSClanHelpListing listing = FindExistingHelpListing(helpProto);
//				if(listing != null)
//				{
//					listing.UpdateListing(helpProto);
//				}
//			}
//			UpdateButtonAndBackground();
//		}
//	}
//
//	void LiveSolicitClanHelp(SolicitClanHelpResponseProto proto, bool self)
//	{
//		if(self || proto.sender.userId != MSWhiteboard.localMup.userId)
//		{
//			foreach(ClanHelpProto helpProto in proto.helpProto)
//			{
//				AddNewListing(helpProto);
//			}
//			UpdateButtonAndBackground();
//		}
//	}
//
//	void LiveEndClanHelp(EndClanHelpResponseProto proto, bool self)
//	{
//		if(self || proto.sender.userId != MSWhiteboard.localMup.userId)
//		{
//			if(listings.ContainsKey(proto.sender.userId))
//			{
//				foreach(MSClanHelpListing listing in listings[proto.sender.userId])
//				{
//					listing.RemoveClanHelp(proto.clanHelpIds);
//					if(listing.helpLength < 1)
//					{
//						listing.GetComponent<MSSimplePoolable>().Pool();
//					}
//				}
//				if(listings[proto.sender.userId].Count < 1)
//				{
//					listings.Remove(proto.sender.userId);
//				}
//				UpdateButtonAndBackground();
//			}
//
//		}
//	}
//
//	void OnHelpAll()
//	{
//		helpAllButton.GetComponent<MSLoadLock>().Lock();
//
//		List<long> helpIds = new List<long>();
//		foreach(KeyValuePair<int, List<MSClanHelpListing>> userList in listings)
//		{
//			foreach(MSClanHelpListing listing in userList.Value)
//			{
//				helpIds.AddRange( listing.GetIdsThatCanBeHelped());
//			}
//		}
//
//		MSClanManager.instance.DoGiveClanHelp(helpIds, helpAllButton.GetComponent<MSLoadLock>().Unlock);
//	}
}
