using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TangoSDK;
using SimpleJSON;
using System.Text;

public class MSTangoManager : MonoBehaviour {

	public static MSTangoManager instance;

	private string authenticateText = "Authenticate";
	
	private string version;
	private string environmentName;
	
	private string userName;
	private string userId;
	private string userPhotoUrl;
	private List<string> friendNames = new List<string>();
	private List<string> friendIds = new List<string>();
	private List<string> leaderboardEntries = new List<string>();
	
	private string possessionVersion = "0";

	Sprite profilePic;

#if UNITY_ANDROID
	void HandleUrl(string url){
		Debug.Log ("Incoming URL is:" + url );  
		try{
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			HandleUrlResult result = SessionFactory.getSession().handleUrl(url, "UnityRock");
			
			switch(result.getHandleUrlResultType()){
			case HandleUrlResultType.HANDLE_URL_RESULT_NO_ACTION_NEEDED:
			case HandleUrlResultType.HANDLE_URL_RESULT_USER_URL:
				break;
			case  HandleUrlResultType.HANDLE_URL_RESULT_GIFT_MESSAGE_RECEIVED:
				Dictionary<string,string> dict = result.getSdkParameters();
				
				//Call UnityRockActivity.unityCallback(String id, String type);
				jo.Call("runOnUiThread", new AndroidJavaRunnable(() => {jo.Call("unityCallback", dict["gift_id"], dict["gift_type"]); }));
				
				break;
			case HandleUrlResultType.HANDLE_URL_RESULT_ERROR: 
			default:
				Debug.Log("Handle URL failed.");
				break;
				
			}
		}catch(System.Exception e){
			Debug.Log ("Handle URL failed" + e);
		}
	}
	#endif

	void Awake() {
		MSTangoManager.instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		bool login = TangoSDK.SessionFactory.getSession().init("ToonSquad", "toonsquad4t", gameObject.name, "SdkEventHandler");

		if(SessionFactory.getSession().isAuthenticated())
		{
			authenticateText = "Authenticated";
			SessionFactory.getSession().getMyProfile("GetMyProfileCallback");
		}
		else if(!SessionFactory.getSession().tangoIsInstalled())
		{
			authenticateText = "Install Tango";
		}
		else if (!SessionFactory.getSession().tangoHasSdkSupport())
		{
			authenticateText = "Update Tango";
		}
		else
		{
			uint requestId = SessionFactory.getSession().authenticate("AuthenticateCallback");
		}
		
		Debug.Log("Tango login success: " + login + "\nAuthentication: " + authenticateText);

		version = SessionFactory.getSession().getVersion();
		environmentName = SessionFactory.getSession().getEnvironmentName();
	}
	
	void SdkEventHandler(string message) {
		TangoSDK.Event ev = new TangoSDK.Event(message);
		Debug.Log ("eventCode: " + ev.getEventCode());
		Debug.Log ("content: " + ev.getContent());
	}
	
	void OnApplicationQuit () {
		SessionFactory.getSession().uninit();
	}
	
	// Must add for URL conversion support
	void ForwardUploadDetails(string message) {
		SessionFactory.getSession().fillActionMap(message);
	}
	
	void AuthenticateCallback(string message) {
		Response r = new Response(message);
		if (r.errorCode == ErrorCode.TANGO_SDK_SUCCESS) {
			Debug.Log (r.result);
			authenticateText = "Authenticated";
			Debug.Log("Tango Authenticated");
			//Load Profile
			SessionFactory.getSession().getMyProfile("GetMyProfileCallback");
		} else {
			Debug.Log (r.errorCode);
			Debug.Log (r.errorText);
			if (r.errorCode == ErrorCode.TANGO_SDK_TANGO_APP_NOT_INSTALLED || 
			    r.errorCode == ErrorCode.TANGO_SDK_TANGO_APP_NO_SDK_SUPPORT) {
				SessionFactory.getSession().installTango();
			}
		}
	}
	
	void ResetAuthenticationCallback(string message) {
		Response r = new Response(message);
		if (r.errorCode == ErrorCode.TANGO_SDK_SUCCESS) {
			Debug.Log (r.result);
			if(SessionFactory.getSession().isAuthenticated()){
				authenticateText = "Authenticated";
				Debug.Log("Tango Authenticated");
			}
			else {
				authenticateText = "Authenticate";
			}
		} else {
			Debug.Log (r.errorCode);
			Debug.Log (r.errorText);
		}
	}
	
	void GetMyProfileCallback(string message) {
		Response r = new Response(message);
		if (r.errorCode == ErrorCode.TANGO_SDK_SUCCESS) {
			Debug.Log (r.result);
			var result = JSON.Parse(r.result);
			userId = result["Profile"][0]["AccountId"];
			userName = result["Profile"][0]["FirstName"] + " " + result["Profile"][0]["LastName"];
			userPhotoUrl = result["Profile"][0]["ProfilePhotoUrl"];
			StartCoroutine(GetPhoto(userPhotoUrl));
		} else {
			Debug.Log (r.errorCode);
			Debug.Log (r.errorText);
		}
	}

	IEnumerator GetPhoto(string photoUrl) {
		Debug.Log ("Tango: Starting photo download: " + photoUrl);
		WWW www = new WWW (photoUrl);
		yield return www;
		profilePic = Sprite.Create (www.texture, new Rect (0, 0, 200, 200), Vector2.zero);
		Debug.Log ("Tango: Downloaded photo! " + profilePic);
	}

	public bool HasProfilePhoto() {
		return profilePic != null;
	}

	public Sprite GetProfilePhoto() {
		return profilePic;
	}
	
	void GetFriendsCallback(string message) {
		Response r = new Response(message);
		if (r.errorCode == ErrorCode.TANGO_SDK_SUCCESS) {
			var result = JSON.Parse(r.result);
			friendNames.Clear();
			friendIds.Clear();
			for (int i = 0; i < result["Friends"].Count; i++) {
				Debug.Log(result["Friends"][i]);
				friendNames.Add(result["Friends"][i]["FirstName"] + " " + result["Friends"][i]["LastName"]);
				friendIds.Add(result["Friends"][i]["AccountId"]);
			}
		} else {
			Debug.Log (r.errorCode);
			Debug.Log (r.errorText);
		}
	}
	
	void GetLeaderboardCallback(string message){
		Debug.Log("This is a message from server " + message);
		string kLeaderboard = "Leaderboard";
		Response r = new Response(message);
		if(r.errorCode == ErrorCode.TANGO_SDK_SUCCESS) {
			var result = JSON.Parse(r.result);
			leaderboardEntries.Clear();
			for(int i=0; i < result["Leaderboard"].Count; i++){
				Debug.Log(result["Leaderboard"][i]);
				leaderboardEntries.Add(result[kLeaderboard][i]["FirstName"] + " " + result[kLeaderboard][i]["LastName"] + " "
				                       + result[kLeaderboard][i]["ComputedMetrics"][0]["MetricId"] + ":" + result[kLeaderboard][i]["ComputedMetrics"][0]["Value"] );
				
			}
		}
	}
	
	void PossessionsCallback(string message) {
		Response r = new Response(message);
		if (r.errorCode == ErrorCode.TANGO_SDK_SUCCESS) {
			Debug.Log (r.result);
			var result = JSON.Parse(r.result);
			if (result["Possessions"][0]["Version"] != null) {
				possessionVersion = result["Possessions"][0]["Version"];
			}
		} else {
			Debug.Log (r.errorCode);
			Debug.Log (r.errorText);
		}
	}
	
	void UploadCallback(string message) {
		Response r = new Response(message);
		if (r.errorCode == ErrorCode.TANGO_SDK_SUCCESS || r.errorCode == ErrorCode.TANGO_SDK_MESSAGE_SEND_PROGRESS) {
			Debug.Log(r.result);
		} else {
			Debug.Log (r.errorCode);
			Debug.Log (r.errorText);
		}
	}
	
	void PrintCallback(string message) {
		Response r = new Response(message);
		if (r.errorCode == ErrorCode.TANGO_SDK_SUCCESS) {
			Debug.Log (r.result);
		} else {
			Debug.Log (r.errorCode);
			Debug.Log (r.errorText);
		}
	}
	
	void OnApplicationPause( bool pauseStatus){
		if(SessionFactory.getSession().tangoIsInstalled() && SessionFactory.getSession().tangoHasSdkSupport()
		   && !SessionFactory.getSession().isAuthenticated()){
			authenticateText = "Authenticate";
		}
	}
}
