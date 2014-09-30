using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSFacebookLoginButton : MonoBehaviour {

	[SerializeField]
	UISprite loginButton;

	[SerializeField]
	GameObject avatar;

	[SerializeField]
	UITexture faceAvatar;

	[SerializeField]
	MSLoadLock loadLock;
	
	void Awake()
	{
		UIButton button = loginButton.GetComponent<UIButton>();
		EventDelegate.Add(button.onClick, delegate {
			this.OnClick();
		});
	}

	void OnEnable()
	{
		MSActionManager.Facebook.OnLoginSucces += SwitchToFaceBookAvatar;

		if(MSFacebookManager.isLoggedIn)
		{
			SwitchToFaceBookAvatar();
		}
		else
		{
			avatar.SetActive(false);
			loginButton.gameObject.SetActive(true);
		}
	}

	void OnDisbale()
	{
		MSActionManager.Facebook.OnLoginSucces -= SwitchToFaceBookAvatar;
	}

	//Changes the button to the circle avatar with the players face in it.
	void SwitchToFaceBookAvatar()
	{
		MSFacebookManager.instance.RunLoadPhotoForUser(FB.UserId.ToString(),faceAvatar);
		loginButton.gameObject.SetActive(false);
		avatar.SetActive(true);
	}

	void OnClick()
	{
		if(!MSFacebookManager.isLoggedIn)
		{
			MSFacebookManager.instance.Init();
		}
		else
		{
			Debug.LogError("Login Button used while player is already logged in");
		}
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
		
		StartupResponseProto startupResponse = UMQNetworkManager.responseDict[tagNum] as StartupResponseProto;
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
}
