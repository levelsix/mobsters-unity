using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSClanHelpManager : MonoBehaviour {

	[SerializeField]
	MSClanHelpScreen helpScreen;

	[SerializeField]
	MSChatGrid chatHelp;

	Dictionary<int, List<MSClanHelpListing>> screenListings = new Dictionary<int, List<MSClanHelpListing>>();
	Dictionary<int, List<MSClanHelpListing>> chatListings = new Dictionary<int, List<MSClanHelpListing>>();

	public static MSClanHelpManager instance;

	void Awake()
	{
		instance = this;
	}
	
	void OnEnable()
	{
		MSActionManager.Clan.OnEndClanHelp += LiveEndClanHelp;
		MSActionManager.Clan.OnGiveClanHelp += LiveGiveClanHelp;
		MSActionManager.Clan.OnSolicitClanHelp += LiveSolicitClanHelp;
		MSActionManager.Loading.OnBuildingsLoaded += InitHelp;
	}
	
	void OnDisable()
	{
		MSActionManager.Clan.OnEndClanHelp -= LiveEndClanHelp;
		MSActionManager.Clan.OnGiveClanHelp -= LiveGiveClanHelp;
		MSActionManager.Clan.OnSolicitClanHelp -= LiveSolicitClanHelp;
		MSActionManager.Loading.OnBuildingsLoaded -= InitHelp;
	}
	
	public void InitHelp()
	{
		helpScreen.grid.animateSmoothly = false;
		foreach(KeyValuePair<int, List<MSClanHelpListing>> userList in screenListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				listing.GetComponent<MSSimplePoolable>().Pool();
			}
		}
		foreach(KeyValuePair<int, List<MSClanHelpListing>> userList in chatListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				listing.GetComponent<MSSimplePoolable>().Pool();
			}
		}


		screenListings.Clear();
		chatListings.Clear();
		
		foreach(ClanHelpProto help in MSClanManager.instance.clanHelpRequests)
		{
			if(help.mup.userId != MSWhiteboard.localMup.userId && !help.helperIds.Contains(MSWhiteboard.localMup.userId) && help.helperIds.Count < help.maxHelpers && help.open)
			{
				AddNewListing(help, screenListings, NewScreenListing);
//				AddNewListing(help, chatListings, NewChatListing);
			}
		}

		//these update the helpscreen
		helpScreen.grid.Reposition();
		UpdateButtonAndBackground();
		
		helpScreen.helpAllButton.onClick.Clear();
		EventDelegate.Add(helpScreen.helpAllButton.onClick, delegate {OnHelpAll();});

		helpScreen.grid.animateSmoothly = true;
	}

	public void ReinitChat()
	{
		foreach(KeyValuePair<int, List<MSClanHelpListing>> userList in chatListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				listing.GetComponent<MSSimplePoolable>().Pool();
			}
		}

		chatListings.Clear();

		foreach(ClanHelpProto help in MSClanManager.instance.clanHelpRequests)
		{
			if(help.mup.userId != MSWhiteboard.localMup.userId && !help.helperIds.Contains(MSWhiteboard.localMup.userId) && help.helperIds.Count < help.maxHelpers && help.open)
			{
				AddNewListing(help, chatListings, NewChatListing);
			}
		}
	}

	IEnumerator RepositionAfterFrame()
	{
		yield return null;
		helpScreen.scrollview.GetComponent<UIScrollView>().ResetPosition();
	}
	
	void UpdateButtonAndBackground()
	{
		int helpable = 0;
		foreach(KeyValuePair<int, List<MSClanHelpListing>> userList in screenListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				if(listing.stillHelpable)
				{
					Debug.Log(listing.ToString());
					helpable++;
				}
			}
		}
		
		helpScreen.noRequestsLabel.gameObject.SetActive( screenListings.Count == 0 );
		helpScreen.helpAllButton.gameObject.SetActive( helpable > 0 );
		helpScreen.badge.notifications = helpable;
		StartCoroutine(RepositionAfterFrame());
	}
	
	void AddNewListing(ClanHelpProto proto, Dictionary<int, List<MSClanHelpListing>> listings, Action<ClanHelpProto> AddNew)
	{
		if(proto.helpType == GameActionType.HEAL)
		{
			//If there's no list for this user, make one
			bool existingHealListing = false;
			if(!listings.ContainsKey(proto.mup.userId))
			{
				listings.Add(proto.mup.userId, new List<MSClanHelpListing>());
			}
			else
			{
				List<MSClanHelpListing> curUserListing = listings[proto.mup.userId];
				foreach(MSClanHelpListing listing in curUserListing)
				{
					if(listing.AddHealingProto(proto))
					{
						existingHealListing = true;
						break;
					}
				}
			}
			
			//if there isn't we make one
			if(!existingHealListing && proto.helperIds.Count < proto.maxHelpers && proto.open)
			{
				AddNew(proto);

//				MSClanHelpListing newListing = MSPoolManager.instance.Get<MSClanHelpListing>(helpScreen.listingPrefab, helpScreen.grid.transform);
//				newListing.transform.localScale = Vector3.one;
//				newListing.dragView.scrollView = helpScreen.scrollview;
//				newListing.Init(proto);
//				
//				listings[proto.mup.userId].Add(newListing);
			}
		}
		else if(!proto.helperIds.Contains(MSWhiteboard.localMup.userId) && proto.helperIds.Count < proto.maxHelpers && proto.open)
		{
			AddNew(proto);
		}
		
	}

	void NewScreenListing(ClanHelpProto proto)
	{
		MSClanHelpListing newListing = MSPoolManager.instance.Get<MSClanHelpListing>(helpScreen.listingPrefab, helpScreen.grid.transform);
		newListing.transform.localScale = Vector3.one;
		newListing.dragView.scrollView = helpScreen.scrollview;
		if(!screenListings.ContainsKey(proto.mup.userId))
		{
			screenListings.Add(proto.mup.userId,new List<MSClanHelpListing>());
		}
		newListing.Init(proto);
		screenListings[proto.mup.userId].Add(newListing);
		helpScreen.grid.Reposition();
	}

	void NewChatListing(ClanHelpProto proto)
	{
		MSClanHelpListing newListing = chatHelp.SpawnBubbleForHelp(proto);

		if(!chatListings.ContainsKey(proto.mup.userId))
		{
			chatListings.Add(proto.mup.userId,new List<MSClanHelpListing>());
		}
		newListing.Init(proto);
		chatListings[proto.mup.userId].Add(newListing);
	}
	
	MSClanHelpListing FindExistingHelpListing(ClanHelpProto findMe, Dictionary<int, List<MSClanHelpListing>> listings)
	{
		if(listings.ContainsKey(findMe.mup.userId))
		{
			List<MSClanHelpListing> curUserListing = listings[findMe.mup.userId];
			foreach(MSClanHelpListing listing in curUserListing)
			{
				if(listing.Contains(findMe))
				{
					return listing;
				}
			}
		}
		
		return null;
	}
	
	//'Live' functions are called while the list is already open
	//so it updates the list while the player is looking at it
	void LiveGiveClanHelp(GiveClanHelpResponseProto proto, bool self)
	{
		if(self || proto.sender.userId != MSWhiteboard.localMup.userId)
		{
			GiveHelp(proto, screenListings);
			GiveHelp(proto, chatListings);
			UpdateButtonAndBackground();
		}
	}

	void GiveHelp(GiveClanHelpResponseProto proto, Dictionary<int, List<MSClanHelpListing>> listings)
	{
		foreach(ClanHelpProto helpProto in proto.clanHelps)
		{
			MSClanHelpListing listing = FindExistingHelpListing(helpProto, listings);
			if(listing != null)
			{
				listing.UpdateListing(helpProto);
			}
		}
	}
	
	void LiveSolicitClanHelp(SolicitClanHelpResponseProto proto, bool self)
	{
		if(self || proto.sender.userId != MSWhiteboard.localMup.userId)
		{
			foreach(ClanHelpProto helpProto in proto.helpProto)
			{
				AddNewListing(helpProto, screenListings, NewScreenListing);
				AddNewListing(helpProto, chatListings, NewChatListing);
			}
			UpdateButtonAndBackground();
		}
	}
	
	void LiveEndClanHelp(EndClanHelpResponseProto proto, bool self)
	{
		if(self || proto.sender.userId != MSWhiteboard.localMup.userId)
		{
			EndHelp(proto, screenListings);
			EndHelp(proto, chatListings);
			UpdateButtonAndBackground();
		}
	}

	void EndHelp(EndClanHelpResponseProto proto, Dictionary<int, List<MSClanHelpListing>> listings)
	{
		if(listings.ContainsKey(proto.sender.userId))
		{
			foreach(MSClanHelpListing listing in listings[proto.sender.userId])
			{
				listing.RemoveClanHelp(proto.clanHelpIds);
				if(listing.helpLength < 1)
				{
					listing.GetComponent<MSSimplePoolable>().Pool();
				}
			}
			if(listings[proto.sender.userId].Count < 1)
			{
				listings.Remove(proto.sender.userId);
			}
		}
	}
	
	void OnHelpAll()
	{
		helpScreen.helpAllButton.GetComponent<MSLoadLock>().Lock();
		
		List<long> helpIds = new List<long>();
		foreach(KeyValuePair<int, List<MSClanHelpListing>> userList in screenListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				helpIds.AddRange( listing.GetIdsThatCanBeHelped());
			}
		}
		
		MSClanManager.instance.DoGiveClanHelp(helpIds, helpScreen.helpAllButton.GetComponent<MSLoadLock>().Unlock);
	}
}
