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

	StartupResponseProto startupResponse;

	public void TryToConnectToFacebook()
	{
		StartCoroutine(ConnectToFacebook());
	}

	IEnumerator ConnectToFacebook()
	{
		MSFacebookManager.instance.Init();
		
		while (!FB.isInitCalled || (!FB.hasFailed && !MSFacebookManager.instance.hasTriedLogin))
		{
			yield return null;
		}

		if (FB.IsLoggedIn)
		{
			yield return StartCoroutine(TryStartupWithFacebook());
			
			MSTutorialManager.instance.OnMakeFacebookDecision(true);
		}
	}

	public void RejectTheInvitation()
	{
		MSPopupManager.instance.CreatePopup("You Don't Like Free Stuff?", 
            "This is a once in a lifetime opportunity that you'll tell your grandchildren about. Please reconsider!",
            new string[] {"Skip", "Connect"},
			new string[] {"greymenuoption", "orangemenuoption"},
			new System.Action[] {RespondNo, delegate{ MSActionManager.Popup.CloseTopPopupLayer(); TryToConnectToFacebook();} },
			"orange"
		);
	}

	IEnumerator TryStartupWithFacebook()
	{
		StartupRequestProto request = new StartupRequestProto();
		request.fbId = FB.UserId;
		request.versionNum = MSValues.version;
		request.isFreshRestart = true;
		request.udid = UMQNetworkManager.instance.udid;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_STARTUP_EVENT);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		startupResponse = UMQNetworkManager.responseDict[tagNum] as StartupResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		Debug.Log("Startup Test: " + startupResponse.startupStatus);
		if (startupResponse.startupStatus == StartupResponseProto.StartupStatus.USER_IN_DB)
		{
			MSPopupManager.instance.CreatePopup("Account Already Used",
        		"Oops! This Facebook account is already linked to another player (" + startupResponse.sender.name + "). " +
			                                    "Would you like to load that account now?",
	            new string[] {"No", "Yes"},
				new string[] {"greymenuoption", "orangemenuoption"},
				new System.Action[] { 
					delegate {
						MSTutorialManager.instance.OnMakeFacebookDecision(false);
						FB.Logout();
						MSActionManager.Popup.CloseAllPopups();
						PlayerPrefs.SetInt(MSFacebookManager.FB_KEY, 0);
						PlayerPrefs.Save();
					}, 
					delegate {
						MSSceneManager.instance.Reload();
						PlayerPrefs.SetInt(MSFacebookManager.FB_KEY, 1);
						PlayerPrefs.Save();
					}
				},
				"orange"
			);

			while (MSPopupManager.instance.top != null)
			{
				yield return null;
			}
		}
		else
		{
			PlayerPrefs.SetInt(MSFacebookManager.FB_KEY, 1);
			PlayerPrefs.Save();
		}
	}

	void RespondNo()
	{
		MSActionManager.Popup.CloseAllPopups();

		MSTutorialManager.instance.OnMakeFacebookDecision(false);
	}
	
}
