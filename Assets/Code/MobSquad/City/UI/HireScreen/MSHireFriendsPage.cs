using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSHireFriendsPage : MonoBehaviour {
	[SerializeField]
	TweenPosition mover;

	[SerializeField]
	MSHirePopup popup;

	[SerializeField]
	MSSimplePoolable hirableFriend;
	
	[SerializeField]
	MSSimplePoolable letterDivide;

	[SerializeField]
	Transform friendList;

	[SerializeField]
	Transform mobFriendList;

	[SerializeField]
	UIButton hireFriendButton;

	[SerializeField]
	MSCheckBox selectAll;

	[SerializeField]
	UIButton sendButton;

	[SerializeField]
	UIButton allFriendsTab;

	[SerializeField]
	UIButton mobFriendsTab;

	[SerializeField]
	UITexture[] bottomFriends;

	List<MSFacebookFriend> friends = new List<MSFacebookFriend>();

	List<MSHireFriendInvite> potentialInvites = new List<MSHireFriendInvite>();

	List<MSHireFriendInvite> potentialMobInvites = new List<MSHireFriendInvite>();

	UIScrollView scrollView;

	bool allPopulated = false;

	bool mobPopulated = false;

	bool allListActive
	{
		get
		{
			return allFriendsTab.GetComponent<MSTabAdvanced>().active;
		}
	}

	const float DIVIDER_HEIGHT = 24f;
	const float FRIEND_HEIGHT = 90f;

	void Awake()
	{
		scrollView = friendList.parent.GetComponent<UIScrollView>();
		EventDelegate.Add(hireFriendButton.onClick, delegate { OnClickAskFriend(); });
		EventDelegate.Add(sendButton.onClick, delegate { PostInvitesToFaceBook(); });
		EventDelegate.Add(allFriendsTab.onClick, delegate { selectAll.checkMarked = true; });
		EventDelegate.Add(mobFriendsTab.onClick, delegate { selectAll.checkMarked = true; });
		selectAll.OnToggle += ForceAllCheckBoxes;
	}

	void OnClickAskFriend()
	{
		if(FB.IsLoggedIn)
		{
			if(!allPopulated)
			{
				PopulateFriendList();
			}
			popup.AskFriends();
		}
		else
		{
			MSActionManager.Facebook.OnLoadFriends += DelayedLoading;
			MSFacebookManager.instance.Init();
		}
	}

	public void UpdateAcceptedFriends()
	{
		List<UserFacebookInviteForSlotProto> friends = MSResidenceManager.instance.GetAcceptedInvites();
		Debug.Log("frinds::" + friends.ToString());
		int index = 0;
		foreach(UITexture texture in bottomFriends)
		{
			if(index < friends.Count)
			{
				MSFacebookManager.instance.RunLoadPhotoForUser(friends[index].recipientFacebookId, texture);
			}
		}
	}

	/// <summary>
	/// this is for if the player clicks on aks friends without being logged into facebook
	/// they are prompter to log in then this cleans up and laods the friends
	/// </summary>
	void DelayedLoading()
	{
		MSActionManager.Facebook.OnLoadFriends -= DelayedLoading;
		popup.AskFriends();
		PopulateFriendList();
		UpdateAcceptedFriends();
	}

	void PopulateFriendList()
	{
		if(!allPopulated)
		{
			friends = MSFacebookManager.instance.friends;
			//Sorts the list of friends by name.
			friends.Sort((x, y) => string.Compare(x.name, y.name));
			PopulateWithList(friends, potentialInvites, this.friendList);
			allPopulated = true;
		}

		if(!mobPopulated)
		{
			//I'm assuming that allfriends alphabatizes the list for us
			List<MSFacebookFriend> mobOnly = new List<MSFacebookFriend>();
			foreach(MSFacebookFriend friend in friends)
			{
				if(friend.installed)
				{
					mobOnly.Add(friend);
				}
			}
			PopulateWithList(mobOnly, potentialMobInvites, this.mobFriendList);
			mobPopulated = true;
		}
			
			
	}

	/// <summary>
	/// Populates a list of friends into a panel.
	/// </summary>
	/// <param name="list">list of friends that need ot be populated.</param>
	/// <param name="storage">List that stores all these populated objects.</param>
	/// <param name="listTransform">panel .</param>
	void PopulateWithList(List<MSFacebookFriend> list,List<MSHireFriendInvite> storage, Transform listTransform)
	{
		bool spawnedDividerLast = true;
		float yOffset = 12;
		
		bool needsDivider = true;
		char letter = 'A';
		foreach(MSFacebookFriend friend in list)
		{
			while(!friend.name[0].Equals(letter) && letter != 'Z')
			{
				letter++;
				needsDivider = true;
			}
			
			if(needsDivider)
			{
				yOffset -= spawnedDividerLast? (DIVIDER_HEIGHT):(FRIEND_HEIGHT / 2f);
				//				yOffset -= DIVIDER_HEIGHT / 2f;
				spawnedDividerLast = true;
				
				MSSimplePoolable divider = MSPoolManager.instance.Get<MSSimplePoolable>(letterDivide, listTransform);
				divider.transf.GetChild(0).GetComponent<UILabel>().text = letter.ToString();
				divider.transf.localPosition = new Vector3(0f, yOffset, 0f);
				divider.transf.localScale = Vector3.one;
			}
			
			yOffset -= spawnedDividerLast? (DIVIDER_HEIGHT):(FRIEND_HEIGHT / 2f);
			yOffset -= FRIEND_HEIGHT / 2f;
			spawnedDividerLast = false;
			needsDivider = false;
			
			MSHireFriendInvite inviteListing = MSPoolManager.instance.Get<MSHireFriendInvite>(hirableFriend, listTransform);
			inviteListing.Init(friend, scrollView);
			inviteListing.transform.localPosition = new Vector3(0f, yOffset, 0f);
			inviteListing.transform.localScale = Vector3.one;
			storage.Add(inviteListing);
		}
	}

	public List<MSHireFriendInvite> GetCulledInviteList(List<MSHireFriendInvite> list)
	{
		List<MSHireFriendInvite> invites = new List<MSHireFriendInvite>();
		foreach(MSHireFriendInvite invite in list)
		{
			if(invite.checkMarked)
			{
				invites.Add(invite);
			}
		}

		return invites;
	}

	public void ForceAllCheckBoxes(bool checkMarked)
	{
		if(allListActive)
		{
			foreach(MSHireFriendInvite invite in potentialInvites)
			{
				invite.checkMarked = checkMarked;
			}
		}
		else
		{
			foreach(MSHireFriendInvite invite in potentialMobInvites)
			{
				invite.checkMarked = checkMarked;
			}
		}
	}

	void PostInvitesToFaceBook()
	{
		List<MSHireFriendInvite> inviteList = GetCulledInviteList(allListActive ? potentialInvites : potentialMobInvites);
		string[] ids = new string[inviteList.Count];
		int index = 0;
		foreach(MSHireFriendInvite invite in inviteList)
		{
			Debug.Log("inviting " + invite.friendInfo.name);
			ids[index] = invite.friendInfo.id;
			index++;
		}

		FB.AppRequest(
			"This is a test!",//title
			ids,//list of ids
			null,//something not implemented on mobile
			null,//something not implemented on mobile
			1,//Max number of requests null = no limit(set to for tests so I don't accidently send to all)
			"",
			"This is a test from " + MSWhiteboard.localMup.name,//message body
			RequestCallBack//function callback
			);
	}

	void RequestCallBack(FBResult result)
	{
		if(result.Error == null)
		{
			Debug.Log("invite result:"+result.Text);
			MSResidenceManager.instance.RequestCallback(result);
			MSActionManager.Popup.CloseTopPopupLayer();
		}
		else
		{
			Debug.Log("Error" + result.Error);
		}
	}
}
