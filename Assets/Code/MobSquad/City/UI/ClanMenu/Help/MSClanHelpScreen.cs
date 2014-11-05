using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSClanHelpScreen : MonoBehaviour {

	[SerializeField]
	MSClanHelpListing listingPrefab;

	[SerializeField]
	UIGrid grid;

	[SerializeField]
	UIButton helpAllButton;
	
	Dictionary<int, List<MSClanHelpListing>> listings = new Dictionary<int, List<MSClanHelpListing>>();

	[SerializeField]
	UIScrollView scrollview;

	void OnEnable()
	{
		MSActionManager.Clan.OnEndClanHelp += LiveEndClanHelp;
		MSActionManager.Clan.OnGiveClanHelp += LiveGiveClanHelp;
		MSActionManager.Clan.OnSolicitClanHelp += LiveSolicitClanHelp;
	}

	void OnDisable()
	{
		MSActionManager.Clan.OnEndClanHelp -= LiveEndClanHelp;
		MSActionManager.Clan.OnGiveClanHelp -= LiveGiveClanHelp;
		MSActionManager.Clan.OnSolicitClanHelp -= LiveSolicitClanHelp;
	}

	public void Init()
	{
		foreach(KeyValuePair<int, List<MSClanHelpListing>> userList in listings)
		{
			foreach(MSClanHelpListing listing in userList.Value)
			{
				listing.GetComponent<MSSimplePoolable>().Pool();
			}
		}
		listings.Clear();

		foreach(ClanHelpProto help in MSClanManager.instance.clanHelpRequests)
		{
			if(!help.helperIds.Contains(MSWhiteboard.localMup.userId) && help.helperIds.Count < help.maxHelpers && help.open)
			{
				AddNewListing(help);
			}
		}

		this.grid.Reposition();

		helpAllButton.gameObject.SetActive( listings.Count > 0 );
	}

	void AddNewListing(ClanHelpProto proto)
	{
		if(proto.helpType == ClanHelpType.HEAL)
		{
			//If there's no list for this user, make one
			if(!listings.ContainsKey(proto.mup.userId))
			{
				listings.Add(proto.mup.userId, new List<MSClanHelpListing>());
			}

			List<MSClanHelpListing> curUserListing = listings[proto.mup.userId];
			
			//Is there a healing listing already?
			bool existingHealListing = false;
			foreach(MSClanHelpListing listing in curUserListing)
			{
				if(listing.AddHealingProto(proto))
				{
					existingHealListing = true;
					break;
				}
			}
			
			//if there isn't we make one
			if(!existingHealListing && proto.helperIds.Count < proto.maxHelpers && proto.open)
			{
				MSClanHelpListing newListing = MSPoolManager.instance.Get<MSClanHelpListing>(listingPrefab, grid.transform);
				newListing.transform.localScale = Vector3.one;
				newListing.dragView.scrollView = scrollview;
				newListing.Init(proto);
				
				listings[proto.mup.userId].Add(newListing);
			}
		}
		else if(!proto.helperIds.Contains(MSWhiteboard.localMup.userId) && proto.helperIds.Count < proto.maxHelpers && proto.open)
		{
//			listings.ContainsKey
			MSClanHelpListing newListing = MSPoolManager.instance.Get<MSClanHelpListing>(listingPrefab, grid.transform);
			newListing.transform.localScale = Vector3.one;
			newListing.dragView.scrollView = scrollview;
			if(!listings.ContainsKey(proto.mup.userId))
			{
				listings.Add(proto.mup.userId,new List<MSClanHelpListing>());
			}
			newListing.Init(proto);
			listings[proto.mup.userId].Add(newListing);
		}

		grid.Reposition();
	}

	MSClanHelpListing FindExistingHelpListing(ClanHelpProto findMe)
	{
		List<MSClanHelpListing> curUserListing = listings[findMe.mup.userId];
		foreach(MSClanHelpListing listing in curUserListing)
		{
			if(listing.Contains(findMe))
			{
				return listing;
			}
		}

		return null;
	}

	//'Live' functions are called while the list is already open
	//so it updates the list while the player is looking at it
	void LiveGiveClanHelp(GiveClanHelpResponseProto proto, bool self)
	{
		Debug.Log("Recieving GiveClanHelp and updating board");
		if(self || proto.sender.userId != MSWhiteboard.localMup.userId)
		{
			foreach(ClanHelpProto helpProto in proto.clanHelps)
			{
				MSClanHelpListing listing = FindExistingHelpListing(helpProto);
				if(listing != null)
				{
					listing.UpdateListing(helpProto);
				}
			}
		}
	}

	void LiveSolicitClanHelp(SolicitClanHelpResponseProto proto, bool self)
	{
		Debug.Log("Recieving solicitClanHelp and updating board");
		if(self || proto.sender.userId != MSWhiteboard.localMup.userId)
		{
			foreach(ClanHelpProto helpProto in proto.helpProto)
			{
				AddNewListing(helpProto);
			}
		}
	}

	void LiveEndClanHelp(EndClanHelpResponseProto proto, bool self)
	{
		Debug.Log("Recieving EndClanHelp and updating board");
		if(self || proto.sender.userId != MSWhiteboard.localMup.userId)
		{
			foreach(MSClanHelpListing listing in listings[proto.sender.userId])
			{
				listing.RemoveClanHelp(proto.clanHelpIds);
			}
		}
	}
}
