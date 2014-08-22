using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using Facebook.MiniJSON;

/// <summary>
/// CBKResidenceManager
/// Handles facebook requests and job levels for residences
/// @author Rob Giusti
/// </summary>
public class MSResidenceManager : MonoBehaviour {

	public static MSResidenceManager instance;

	static Dictionary<int, List<UserFacebookInviteForSlotProto>> fbInviteAccepted = 
		new Dictionary<int, List<UserFacebookInviteForSlotProto>>();

	public static Dictionary<int, MSBuilding> residences = new Dictionary<int, MSBuilding>();

	int currBuildingId;

	void Awake()
	{
		instance = this;
	}

	public void OpenRequestDialogue(int forBuilding)
	{
		if (FB.IsLoggedIn)
		{
			currBuildingId = forBuilding;

			FB.AppRequest(
				"Please respond to my request!",
				null,
				"",
				null,
				100,
				"",
				"A request from " + MSWhiteboard.localMup.name,
				RequestCallback
			);
				
		}
	}

	/// <summary>
	/// Callback from sending the FB request.
	/// Determines if the result is a successful request
	/// If so, reports the list of friends to our own server for bookkeeping
	/// </summary>
	/// <param name="result">Result.</param>
	public void RequestCallback(FBResult result)
	{
		//Debug.Log("Request callback");
		if (result != null)
		{
			Dictionary<string, object> responseObject = 
				Json.Deserialize(result.Text) as Dictionary<string, object>;
			object obj = 0;
			if (responseObject.TryGetValue("request", out obj)) //If the user cancelled the request, this will return false
		    {
				//Debug.Log("Request made");
				foreach (var item in (IList)(responseObject["to"])) 
				{
					  Debug.Log("Response part: " + item.GetType().ToString() + ", " + item);
				}

				StartCoroutine(ReportFBRequests((IList)responseObject["to"]));
			}

		}
	}

	/// <summary>
	/// If we've sent FB requests, we need to report them to our own server so that we can keep
	/// track of who's sent requests to whom, and for what building.
	/// </summary>
	/// <returns>The FB requests.</returns>
	/// <param name="fbIds">Fb identifiers.</param>
	IEnumerator ReportFBRequests(IList fbIds)
	{
		InviteFbFriendsForSlotsRequestProto request = new InviteFbFriendsForSlotsRequestProto();
		request.sender = MSWhiteboard.localMupWithFacebook;
		InviteFbFriendsForSlotsRequestProto.FacebookInviteStructure fish;
		foreach (var item in fbIds) 
		{
			fish = new InviteFbFriendsForSlotsRequestProto.FacebookInviteStructure();
			fish.fbFriendId = item.ToString();
			fish.userStructId = currBuildingId;
			fish.userStructFbLvl = residences[currBuildingId].userStructProto.fbInviteStructLvl + 1;
			request.invites.Add(fish);
		}

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_INVITE_FB_FRIENDS_FOR_SLOTS_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		InviteFbFriendsForSlotsResponseProto response = UMQNetworkManager.responseDict[tagNum] as InviteFbFriendsForSlotsResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

	}

	public void CheckBuilding(int userBuildingId)
	{
		if (!residences.ContainsKey(userBuildingId))
		{
			return;
		}

		//We know if there's no list, there's no requests
		if (!fbInviteAccepted.ContainsKey(userBuildingId)) 
		{
			return;
		}

		for (int i = 0; i < fbInviteAccepted[userBuildingId].Count; ) 
		{
			if (fbInviteAccepted[userBuildingId][i].structFbLvl <= residences[userBuildingId].userStructProto.fbInviteStructLvl)
			{
				fbInviteAccepted[userBuildingId].RemoveAt(i);
			}
			else
			{
				i++;
			}
		}

		int invites = fbInviteAccepted[userBuildingId].Count;
		int invitesNeeded = GetResidenceLevelBelowCurrent(residences[userBuildingId].userStructProto.fbInviteStructLvl+1, residences[userBuildingId].combinedProto).residence.numAcceptedFbInvites;

		Debug.LogWarning("Checking invites for building " + userBuildingId + "--Invites: " + invites + " / " + invitesNeeded);

		if (invites >= invitesNeeded)
		{
			UpgradeResidenceFacebookLevelFromInvites(userBuildingId);
		}
	}

	public MSFullBuildingProto GetResidenceLevelBelowCurrent(int level, MSFullBuildingProto residence)
	{
		if (residence.structInfo.level > level)
		{
			return (GetResidenceLevelBelowCurrent(level, residence.predecessor));
		}
		return residence;
	}

	public void AddInvite(UserFacebookInviteForSlotProto addInvite)
	{
		if (!fbInviteAccepted.ContainsKey(addInvite.userStructId))
		{
			fbInviteAccepted.Add(addInvite.userStructId, new List<UserFacebookInviteForSlotProto>());
		}
		//if (addInvite.structFbLvl == residences[addInvite.userStructId].userStructProto.fbInviteStructLvl + 1)
		//{
			fbInviteAccepted[addInvite.userStructId].Add(addInvite);
		//}
	}

	public void AddInvites(List<UserFacebookInviteForSlotProto> addInvites)
	{
		foreach (var item in addInvites) 
		{
			AddInvite(item);
		}
		foreach (var item in fbInviteAccepted) 
		{
			CheckBuilding(item.Key);
		}
	}

	public void UpgradeResidenceFacebookLevelWithGems(int userStructureId, int gems)
	{
		IncreaseMonsterInventorySlotRequestProto request = new IncreaseMonsterInventorySlotRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.increaseSlotType = IncreaseMonsterInventorySlotRequestProto.IncreaseSlotType.PURCHASE;
		request.userStructId = userStructureId;
		//TODO: Need a field in the proto for how many gems are being spent!

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_INCREASE_MONSTER_INVENTORY_SLOT_EVENT, DealWithUpgradeResidenceResponse);
	}

	void UpgradeResidenceFacebookLevelFromInvites(int userStructureId)
	{
		IncreaseMonsterInventorySlotRequestProto request = new IncreaseMonsterInventorySlotRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.increaseSlotType = IncreaseMonsterInventorySlotRequestProto.IncreaseSlotType.REDEEM_FACEBOOK_INVITES;
		request.userStructId = userStructureId;
		foreach (var item in fbInviteAccepted[userStructureId]) 
		{
			request.userFbInviteForSlotIds.Add(item.inviteId);
		}

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_INCREASE_MONSTER_INVENTORY_SLOT_EVENT, DealWithUpgradeResidenceResponse);
	}

	void DealWithUpgradeResidenceResponse(int tagNum)
	{
		IncreaseMonsterInventorySlotResponseProto response = UMQNetworkManager.responseDict[tagNum] as IncreaseMonsterInventorySlotResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != IncreaseMonsterInventorySlotResponseProto.IncreaseMonsterInventorySlotStatus.SUCCESS)
		{
			Debug.LogError("Problem increasing residence slots: " + response.status.ToString());
		}
	}

	public void JustReceivedFriendAccept(AcceptAndRejectFbInviteForSlotsResponseProto response)
	{
		AddInvites(response.acceptedInvites);
	}
}
