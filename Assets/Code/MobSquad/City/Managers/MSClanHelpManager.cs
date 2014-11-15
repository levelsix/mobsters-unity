using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSClanHelpManager : MonoBehaviour {

	[SerializeField]
	MSClanHelpScreen helpScreen;

	[SerializeField]
	MSChatGrid clanChatGrid;

	[SerializeField]
	public MSBottomChatBlock clanChatblock;

	[SerializeField]
	MSChatPopup chatPopup;

	Dictionary<string, List<MSClanHelpListing>> screenListings = new Dictionary<string, List<MSClanHelpListing>>();
	Dictionary<string, List<MSClanHelpListing>> chatListings = new Dictionary<string, List<MSClanHelpListing>>();

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
		foreach(KeyValuePair<string, List<MSClanHelpListing>> userList in screenListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				listing.GetComponent<MSSimplePoolable>().Pool();
			}
		}
		foreach(KeyValuePair<string, List<MSClanHelpListing>> userList in chatListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				listing.GetComponent<MSSimplePoolable>().Pool();
			}
		}


		screenListings.Clear();
		chatListings.Clear();
		clanChatGrid.RemoveAllHelpBubbles();
		
		foreach(ClanHelpProto help in MSClanManager.instance.clanHelpRequests)
		{
			if(!help.mup.userUuid.Equals(MSWhiteboard.localMup.userUuid) && !help.helperUuids.Contains(MSWhiteboard.localMup.userUuid) && help.helperUuids.Count < help.maxHelpers && help.open)
			{
				AddNewListing(help, screenListings, NewScreenListing, false);
				//this is commented out because the chat menu gets pooled and reset every time it opens
				//commented back in to add these helps to the bottom chat block
				AddNewListing(help, chatListings, NewChatListing, false);
			}
		}

		//these update the helpscreen
		helpScreen.grid.Reposition();
		UpdateButtonAndBackground();
		
		helpScreen.helpAllButton.onClick.Clear();
		EventDelegate.Add(helpScreen.helpAllButton.onClick, delegate {OnHelpAll();});

	}

	public void ReinitChat()
	{
		foreach(KeyValuePair<string, List<MSClanHelpListing>> userList in chatListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				listing.GetComponent<MSSimplePoolable>().Pool();
			}
		}

		chatListings.Clear();
		clanChatGrid.RemoveAllHelpBubbles();

		foreach(ClanHelpProto help in MSClanManager.instance.clanHelpRequests)
		{
			if(!help.mup.userUuid.Equals(MSWhiteboard.localMup.userUuid) && !help.helperUuids.Contains(MSWhiteboard.localMup.userUuid) && help.helperUuids.Count < help.maxHelpers && help.open)
			{
				AddNewListing(help, chatListings, NewChatListing, false);
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
		foreach(KeyValuePair<string, List<MSClanHelpListing>> userList in screenListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				if(listing.stillHelpable)
				{
					Debug.Log("playerID: " + userList.Key + " " + listing.ToString());
					helpable++;
				}
			}
		}
		
		helpScreen.noRequestsLabel.gameObject.SetActive( screenListings.Count == 0 );
		helpScreen.helpAllButton.gameObject.SetActive( helpable > 0 );
		helpScreen.badge.notifications = helpable;
		chatPopup.helpNotification.notifications = helpable;
		StartCoroutine(RepositionAfterFrame());

		clanChatGrid.SortBubbles();
		if(clanChatGrid.bubbles.Count > 1)
		{
			clanChatblock.AddMessage(clanChatGrid.bubbles[clanChatGrid.bubbles.Count - 2]);
			clanChatblock.AddMessage(clanChatGrid.bubbles[clanChatGrid.bubbles.Count - 1]);
		}
		else if(clanChatGrid.bubbles.Count > 0)
		{
			clanChatblock.AddMessage(clanChatGrid.bubbles[0]);
		}

		if(MSActionManager.Clan.OnUpdateNumberOfAvailableHelpRequests != null)
		{
			MSActionManager.Clan.OnUpdateNumberOfAvailableHelpRequests(helpable);
		}
	}
	
	MSClanHelpListing AddNewListing(ClanHelpProto proto, Dictionary<string, List<MSClanHelpListing>> listings, Func<ClanHelpProto, bool, MSClanHelpListing> AddNew, bool animate)
	{
		if(proto.helpType == GameActionType.HEAL)
		{
			//If there's no list for this user, make one
			bool existingHealListing = false;
			if(!listings.ContainsKey(proto.mup.userUuid))
			{
				listings.Add(proto.mup.userUuid, new List<MSClanHelpListing>());
			}
			else
			{
				List<MSClanHelpListing> curUserListing = listings[proto.mup.userUuid];
				foreach(MSClanHelpListing listing in curUserListing)
				{
					if(listing.AddHealingProto(proto))
					{
						existingHealListing = true;
						return listing;
					}
				}
			}

			//if there isn't we make one
			if(!existingHealListing && proto.helperUuids.Count < proto.maxHelpers && proto.open)
			{
				return AddNew(proto, animate);
			}
		}
		else if(!proto.helperUuids.Contains(MSWhiteboard.localMup.userUuid) && proto.helperUuids.Count < proto.maxHelpers && proto.open)
		{
			return AddNew(proto, animate);
		}
		return null;
	}

	MSClanHelpListing NewScreenListing(ClanHelpProto proto, bool animate)
	{
		MSClanHelpListing newListing = MSPoolManager.instance.Get<MSClanHelpListing>(helpScreen.listingPrefab, helpScreen.grid.transform);
		newListing.transform.localScale = Vector3.one;
		newListing.dragView.scrollView = helpScreen.scrollview;
		if(!screenListings.ContainsKey(proto.mup.userUuid))
		{
			screenListings.Add(proto.mup.userUuid,new List<MSClanHelpListing>());
		}
		newListing.Init(proto);
		Debug.LogWarning(proto.mup.userUuid + " adding new listing");
		screenListings[proto.mup.userUuid].Add(newListing);
		helpScreen.grid.animateSmoothly = animate;
		helpScreen.grid.Reposition();

		return newListing;
	}

	MSClanHelpListing NewChatListing(ClanHelpProto proto, bool animate)
	{
		MSClanHelpListing newListing = clanChatGrid.SpawnBubbleForHelp(proto, animate);

		if(!chatListings.ContainsKey(proto.mup.userUuid))
		{
			chatListings.Add(proto.mup.userUuid,new List<MSClanHelpListing>());
		}
		newListing.Init(proto);
		chatListings[proto.mup.userUuid].Add(newListing);

		return newListing;
	}
	
	MSClanHelpListing FindListingContainingProto(ClanHelpProto findMe, Dictionary<string, List<MSClanHelpListing>> listings)
	{
		if(listings.ContainsKey(findMe.mup.userUuid))
		{
			List<MSClanHelpListing> curUserListing = listings[findMe.mup.userUuid];
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
		if(self || !proto.sender.userUuid.Equals(MSWhiteboard.localMup.userUuid))
		{
			GiveHelp(proto, screenListings);
			GiveHelp(proto, chatListings);
			UpdateButtonAndBackground();
		}
	}

	void GiveHelp(GiveClanHelpResponseProto proto, Dictionary<string, List<MSClanHelpListing>> listings)
	{
		foreach(ClanHelpProto helpProto in proto.clanHelps)
		{
			MSClanHelpListing listing = FindListingContainingProto(helpProto, listings);
			if(listing != null)
			{
				listing.UpdateListing(helpProto);
				//this line removes the listing from the chat IF it's even a chat listing
				if(clanChatGrid.RemoveHelpBubble(listing))
				{
					listing.GetComponent<MSSimplePoolable>().Pool();

					listings[helpProto.mup.userUuid].Remove(listing);

					clanChatGrid.table.animateSmoothly = true;
					clanChatGrid.table.Reposition();

					listing = null;
				}
			}
		}


	}
	
	void LiveSolicitClanHelp(SolicitClanHelpResponseProto proto, bool self)
	{
		if(self || !proto.sender.userUuid.Equals( MSWhiteboard.localMup.userUuid))
		{
			foreach(ClanHelpProto helpProto in proto.helpProto)
			{
				AddNewListing(helpProto, screenListings, NewScreenListing, false);
				AddNewListing(helpProto, chatListings, NewChatListing, true);
			}
			UpdateButtonAndBackground();
		}
	}
	
	void LiveEndClanHelp(EndClanHelpResponseProto proto, bool self)
	{
		if(self || !proto.sender.userUuid.Equals( MSWhiteboard.localMup.userUuid))
		{
			EndHelp(proto, screenListings);
			EndHelp(proto, chatListings);
			UpdateButtonAndBackground();
		}
	}

	void EndHelp(EndClanHelpResponseProto proto, Dictionary<string, List<MSClanHelpListing>> dictionary)
	{
		if(dictionary.ContainsKey(proto.sender.userUuid))
		{
			for(int i = 0; i < dictionary[proto.sender.userUuid].Count; i++)//foreach(MSClanHelpListing listing in dictionary[proto.sender.userId])
			{
				MSClanHelpListing listing = dictionary[proto.sender.userUuid][i];
				listing.RemoveClanHelp(proto.clanHelpUuids);
				if(listing.helpLength < 1)
				{
					//this line removes the listing from the chat IF it's even a chat listing
					clanChatGrid.RemoveHelpBubble(listing);
					if(!dictionary[proto.sender.userUuid].Remove(listing))
					{
						Debug.LogError("could not find listing that required removal");
					}
					listing.GetComponent<MSSimplePoolable>().Pool();
					i--;
					helpScreen.grid.Reposition();
				}
			}
			if(dictionary[proto.sender.userUuid].Count < 1)
			{
				dictionary.Remove(proto.sender.userUuid);
			}

		}
	}

	void OnHelpAll()
	{
		helpScreen.helpAllButton.GetComponent<MSLoadLock>().Lock();
		
		List<string> helpIds = new List<string>();
		foreach(KeyValuePair<string, List<MSClanHelpListing>> userList in screenListings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				helpIds.AddRange( listing.GetIdsThatCanBeHelped());
			}
		}
		
		MSClanManager.instance.DoGiveClanHelp(helpIds, helpScreen.helpAllButton.GetComponent<MSLoadLock>().Unlock);
	}
}
