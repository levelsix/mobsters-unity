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
		if(FB.IsLoggedIn)
		{
			SwitchToFaceBookAvatar();
		}
		else
		{
			avatar.SetActive(false);
			loginButton.gameObject.SetActive(true);
		}
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
		if(!FB.IsLoggedIn)
		{
			StartCoroutine(TryToLogin());
		}
		else
		{
			Debug.LogError("Login Button used while player is already logged in");
			SwitchToFaceBookAvatar();
		}
	}

	IEnumerator TryToLogin()
	{
		if (loadLock.locked) yield break;
		loadLock.Lock();
		MSFacebookManager.instance.Init();
		while(!MSFacebookManager.instance.hasTriedLogin)
		{
			yield return null;
		}
		if (FB.IsLoggedIn)
		{
			Debug.LogWarning("Yes");
			StartCoroutine(TryStartupWithFacebook());
		}
		else
		{
			Debug.LogWarning("No...");
			loadLock.Unlock();
		}
	}

	IEnumerator TryStartupWithFacebook()
	{
		Debug.LogWarning("Startup Test: Begin");

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

		loadLock.Unlock();
		
		StartupResponseProto startupResponse = UMQNetworkManager.responseDict[tagNum] as StartupResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		Debug.LogWarning("Startup Test: " + startupResponse.startupStatus);
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
					MSActionManager.Popup.CloseTopPopupLayer();
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
			SwitchToFaceBookAvatar();
			PlayerPrefs.SetInt(MSFacebookManager.FB_KEY, 1);
			PlayerPrefs.Save();
		}
	}
}
