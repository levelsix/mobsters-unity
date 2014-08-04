using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// MSFacebookPopup
/// @author Rob Giusti
/// </summary>
public class MSFacebookPopup : MonoBehaviour 
{

	public void TryToConnectToFacebook()
	{
		StartCoroutine(ConnectToFacebook());
	}

	IEnumerator ConnectToFacebook()
	{
		MSFacebookManager.instance.Init();
		
		while (!FB.isInitCalled || (!FB.hasFailed && !MSFacebookManager.hasTriedLogin))
		{
			yield return null;
		}

		if (FB.IsLoggedIn)
		{
			MSActionManager.Popup.CloseAllPopups();
			
			MSActionManager.Tutorial.OnMakeFacebookDecision(true);
		}
	}

	public void RejectTheInvitation()
	{
		MSPopupManager.instance.CreatePopup("You Don't Like Free Stuff?", 
            "This is a once in a lifetime opportunity that you'll tell your grandchildren about. Please reconsider!",
            new string[] {"Skip", "Connect"},
			new string[] {"greymenuoption", "orangemenuoption"},
			new System.Action[] {RespondNo, TryToConnectToFacebook},
			"orange"
		);
	}

	void RespondNo()
	{
		MSActionManager.Popup.CloseAllPopups();

		MSActionManager.Tutorial.OnMakeFacebookDecision(false);
	}
	
}
