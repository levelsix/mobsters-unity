using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;
using Facebook;
using Facebook.MiniJSON;

/// <summary>
/// @author Rob Giusti
/// CBKFacebookRequestEntry
/// </summary>
[RequireComponent (typeof(MSSimplePoolable))]
public class MSFacebookRequestEntry : MonoBehaviour {

	[SerializeField]
	UITexture fbPhoto;

	[SerializeField]
	UILabel topText;

	[SerializeField]
	UILabel bottomText;

	bool accepted = false;

	UserFacebookInviteForSlotProto invite;

	string hireRoleName
	{
		get
		{
			foreach (MSFullBuildingProto item in MSDataManager.instance.GetAll<MSFullBuildingProto>().Values)
			{
				if (item.structInfo.structType == StructureInfoProto.StructType.RESIDENCE
				    && item.structInfo.level == invite.structFbLvl)
				{
					return item.residence.occupationName;
				}
			}
			return "friend!";
		}
	}

	public void Init(UserFacebookInviteForSlotProto invite)
	{
		this.invite = invite;

		accepted = false;

		topText.text = "Loading...";
		bottomText.text = " ";
		RequestName();

		RequestPicture();
	}

	public void TryAccept()
	{
		MSRequestManager.instance.AcceptOrRejectInvite(invite, accepted);
	}

	#region FB Requests and Callbacks

	void RequestName()
	{
		FB.API ("/" + invite.inviter.facebookId, HttpMethod.GET, NameCallback);
	}

	void NameCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError("Failed to get username.");
			RequestName();
		}
		else
		{
			var profile = (Dictionary<string,object>) Json.Deserialize(result.Text);
			string name = (string)profile["first_name"];
			topText.text = name + " needs help hiring a " + hireRoleName + "!";
			bottomText.text = MSUtil.TimeStringShort(MSUtil.timeNowMillis - invite.timeOfInvite);
		}
	}

	void RequestPicture()
	{
		FB.API("/" + invite.inviter.facebookId + "/picture?height=" + fbPhoto.height + "&width=" + fbPhoto.width, HttpMethod.GET, PictureCallback);
	}

	void PictureCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError("Failed to get picture");
			RequestPicture();
			return;
		}
		fbPhoto.mainTexture = result.Texture;
	}

	#endregion

	public void Accept()
	{
		MSRequestManager.instance.AcceptInvite(invite);
		GetComponent<MSSimplePoolable>().Pool();
	}

	public void Reject()
	{
		MSRequestManager.instance.RejectInvite(invite);
		GetComponent<MSSimplePoolable>().Pool();
	}
}
