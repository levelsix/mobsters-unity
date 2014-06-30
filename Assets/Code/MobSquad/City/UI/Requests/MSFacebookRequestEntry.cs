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
	GameObject checkMark;

	[SerializeField]
	UILabel topText;

	[SerializeField]
	UILabel bottomText;

	public MSActionButton button;

	bool accepted = false;

	UserFacebookInviteForSlotProto invite;

	public void Init(UserFacebookInviteForSlotProto invite)
	{
		this.invite = invite;

		accepted = false;
		checkMark.SetActive(accepted);

		topText.text = "Loading...";
		bottomText.text = " ";
		RequestName();

		RequestPicture();

		UIEventListener.Get(button.gameObj).onClick += Swap;
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
			topText.text = name + " needs your help!";
			bottomText.text = "Your friend " + name + " needs help unlocking more mobster slots.";
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

	public void Swap(GameObject go)
	{
		accepted = !accepted;
		checkMark.SetActive(accepted);
	}
}
