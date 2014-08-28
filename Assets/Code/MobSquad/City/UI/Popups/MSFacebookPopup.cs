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
		
		while (!FB.isInitCalled || (!FB.hasFailed && !MSFacebookManager.hasTriedLogin))
		{
			yield return null;
		}

		if (FB.IsLoggedIn)
		{
			yield return StartCoroutine(TryStartupWithFacebook());

			MSActionManager.Popup.CloseAllPopups();
			
			MSTutorialManager.instance.OnMakeFacebookDecision(true);
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

	IEnumerator TryStartupWithFacebook()
	{
		StartupRequestProto request = new StartupRequestProto();
		request.fbId = FB.UserId;
		request.versionNum = MSValues.version;
		request.isFreshRestart = true;
		request.udid = UMQNetworkManager.udid;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_STARTUP_EVENT);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		startupResponse = UMQNetworkManager.responseDict[tagNum] as StartupResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (startupResponse.startupStatus == StartupResponseProto.StartupStatus.USER_IN_DB)
		{
			MSPopupManager.instance.CreatePopup("Account Already Used",
        		"Oops! This Facebook account is already linked to another player (" + startupResponse.sender.name + "). " +
			                                    "Would you like to load that account now?",
	            new string[] {"No", "Yes"},
				new string[] {"greymenuoption", "orangemenuoption"},
				new System.Action[] { 
					delegate {
						MSTutorialManager.instance.OnMakeFacebookDecision(true, false);
						MSActionManager.Popup.CloseTopPopupLayer();
					}, 
					delegate {
						MSTutorialManager.instance.OnMakeFacebookDecision(true, true);
						UMQLoader.StartUp(startupResponse);
						MSActionManager.Popup.CloseTopPopupLayer();
					}
				},
				"orange"
			);

		}
	}

	void RespondNo()
	{
		MSActionManager.Popup.CloseAllPopups();

		MSTutorialManager.instance.OnMakeFacebookDecision(false);
	}
	
}
