using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBKRequestManager
/// </summary>
public class MSRequestManager : MonoBehaviour {

	public static MSRequestManager instance;

	public List<UserFacebookInviteForSlotProto> invitesForMe = new List<UserFacebookInviteForSlotProto>();

	AcceptAndRejectFbInviteForSlotsRequestProto _inviteResponseRequest;

	AcceptAndRejectFbInviteForSlotsRequestProto inviteResponseRequest
	{
		get
		{
			if (_inviteResponseRequest == null)
			{
				_inviteResponseRequest = new AcceptAndRejectFbInviteForSlotsRequestProto();
				_inviteResponseRequest.sender = MSWhiteboard.localMupWithFacebook;
			}
			return _inviteResponseRequest;
		}
	}

	void Awake()
	{
		instance = this;
	}

	public void Init(List<UserFacebookInviteForSlotProto> invitesToMe)
	{
		invitesForMe = invitesToMe;

		if (MSActionManager.UI.OnRequestsAcceptOrReject != null)
		{
			MSActionManager.UI.OnRequestsAcceptOrReject();
		}
	}

	public void AcceptOrRejectInvite(UserFacebookInviteForSlotProto invite, bool accepted)
	{
		if (accepted)
		{
			AcceptInvite(invite);
		}
		else
		{
			RejectInvite(invite);
		}

		if (MSActionManager.UI.OnRequestsAcceptOrReject != null)
		{
			MSActionManager.UI.OnRequestsAcceptOrReject();
		}
	}

	public void AcceptInvite(UserFacebookInviteForSlotProto invite)
	{
		inviteResponseRequest.acceptedInviteIds.Add(invite.inviteId);
		invitesForMe.Remove(invite);
	}

	public void RejectInvite(UserFacebookInviteForSlotProto invite)
	{
		inviteResponseRequest.rejectedInviteIds.Add(invite.inviteId);
		invitesForMe.Remove(invite);
	}

	public void SendAcceptRejectRequest()
	{
		if (_inviteResponseRequest != null)
		{
			StartCoroutine(SendAcceptRejectRequest(_inviteResponseRequest));
			_inviteResponseRequest = null;
		}
	}

	IEnumerator SendAcceptRejectRequest(AcceptAndRejectFbInviteForSlotsRequestProto request)
	{
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ACCEPT_AND_REJECT_FB_INVITE_FOR_SLOTS_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		AcceptAndRejectFbInviteForSlotsResponseProto response = UMQNetworkManager.responseDict[tagNum] as AcceptAndRejectFbInviteForSlotsResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != AcceptAndRejectFbInviteForSlotsResponseProto.AcceptAndRejectFbInviteForSlotsStatus.SUCCESS)
		{
			Debug.LogError("Problem logging request responses: " + response.status.ToString()); 
		}

	}

	public void JustReceivedFriendInvite(InviteFbFriendsForSlotsResponseProto response)
	{
		if (response.status == InviteFbFriendsForSlotsResponseProto.InviteFbFriendsForSlotsStatus.SUCCESS)
		{
			foreach (UserFacebookInviteForSlotProto item in response.invitesNew) 
			{
				if (item.recipientFacebookId == FB.UserId)
				{
					invitesForMe.Add(item);
				}
			}
		}
	}
}
