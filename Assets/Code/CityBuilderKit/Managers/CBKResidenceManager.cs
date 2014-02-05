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
public class CBKResidenceManager : MonoBehaviour {

	public static CBKResidenceManager instance;

	Dictionary<int, List<UserFacebookInviteForSlotProto>> fbInviteAccepted = 
		new Dictionary<int, List<UserFacebookInviteForSlotProto>>();

	Dictionary<int, CBKBuilding> residences = new Dictionary<int, CBKBuilding>();

	void Awake()
	{
		instance = this;
	}

	[ContextMenu ("Open Request")]
	void OpenRequestDialogue()
	{
		if (FB.IsLoggedIn)
		{
			FB.AppRequest(
				message: "Please respond to my request!",
				title: "A request from " + CBKWhiteboard.localMup.name
				);
		}
	}

	void RequestCallback(FBResult result)
	{
		Debug.Log("Request callback");
		if (result != null)
		{
			Dictionary<string, object> responseObject = 
				Json.Deserialize(result.Text) as Dictionary<string, object>;
			object obj = 0;
			if (responseObject.TryGetValue("request", out obj))
		    {
				Debug.Log("Request made");
			}

			foreach (var item in responseObject) 
			{
				Debug.Log("Response part: " + item.Key + ": " + item.Value);
			}
		}
	}

	void CheckBuilding(int userBuildingId)
	{
		CBKBuilding building = residences[userBuildingId];
		if (fbInviteAccepted[userBuildingId].Count >= GetResidenceLevelBelowCurrent(building.userStructProto.fbInviteStructLvl+1, building.combinedProto).residence.numAcceptedFbInvites)
		{
			UpgradeResidenceFacebookLevelFromInvites(userBuildingId);
		}
	}

	CBKCombinedBuildingProto GetResidenceLevelBelowCurrent(int level, CBKCombinedBuildingProto residence)
	{
		if (residence.structInfo.level > level)
		{
			return (GetResidenceLevelBelowCurrent(level, residence.predecessor));
		}
		return residence;
	}

	void AddInvite(UserFacebookInviteForSlotProto addInvite)
	{
		if (!fbInviteAccepted.ContainsKey(addInvite.userStructId) && addInvite.structFbLvl == residences[addInvite.userStructId].userStructProto.fbInviteStructLvl + 1)
		{
			fbInviteAccepted.Add(addInvite.userStructId, new List<UserFacebookInviteForSlotProto>());
		}
		fbInviteAccepted[addInvite.userStructId].Add(addInvite);
	}

	void AddInvites(List<UserFacebookInviteForSlotProto> addInvites)
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

	IEnumerator SendFBRequests()
	{
		yield return null;
	}

	IEnumerator UpgradeResidenceFacebookLevelFromInvites(int userStructureId)
	{
		IncreaseMonsterInventorySlotRequestProto request = new IncreaseMonsterInventorySlotRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.increaseSlotType = IncreaseMonsterInventorySlotRequestProto.IncreaseSlotType.REDEEM_FACEBOOK_INVITES;
		request.userStructId = userStructureId;
		foreach (var item in fbInviteAccepted[userStructureId]) 
		{
			request.userFbInviteForSlotIds.Add(item.inviteId);
		}

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_INCREASE_MONSTER_INVENTORY_SLOT_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		IncreaseMonsterInventorySlotResponseProto response = UMQNetworkManager.responseDict[tagNum] as IncreaseMonsterInventorySlotResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
	}
}
