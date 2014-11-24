using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TangoSDK;
using SimpleJSON;
using System.Text;

public class UnityRockTest : MonoBehaviour {

	private string authenticateText = "Authenticate";

	private string version;
	private string environmentName;

	private string userName;
	private string userId;
	private string userPhotoUrl;
	private List<string> friendNames = new List<string>();
	private List<string> friendIds = new List<string>();
	private List<string> leaderboardEntries = new List<string>();


	private enum View {
		Main,
		MyProfile,
		FriendsProfiles,
		SelectedFriend,
		MyProfilePhoto,
    Leaderboard
	};

	private View selectedScreen = View.Main;

	private int buttonWidth = (int)(Screen.width * 0.8);
	private int buttonHeight = (int)(Screen.height * 0.07);
	private int horizontalPadding = (int)(Screen.width * 0.1);

	public Vector2 scrollPosition = Vector2.zero;
	public int selectedFriendIndex;

	private Texture2D thumbnail;
	private Texture2D content;
	private Texture2D profilePhoto;

	private string possessionVersion = "0";


#if UNITY_ANDROID
  void HandleUrl(String url){
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



	// Use this for initialization
	void Start () {
		bool result = SessionFactory.getSession().init("UnityRock", "unityrock", this.gameObject.name, "SdkEventHandler");
		Debug.Log ( result );

    		if(SessionFactory.getSession().isAuthenticated()){
      			authenticateText = "Authenticated";
    		}else if(!SessionFactory.getSession().tangoIsInstalled()){
			authenticateText = "Install Tango";
		}else if (!SessionFactory.getSession().tangoHasSdkSupport()) {
			authenticateText = "Update Tango";
		} 

		version = SessionFactory.getSession().getVersion();
		environmentName = SessionFactory.getSession().getEnvironmentName();

		thumbnail = (Texture2D)Resources.Load("UnityRockThumbnail");
		content = (Texture2D)Resources.Load("UnityRockContent");
	}
	
	void SdkEventHandler(string message) {
		TangoSDK.Event ev = new TangoSDK.Event(message);
		Debug.Log ("eventCode: " + ev.getEventCode());
		Debug.Log ("content: " + ev.getContent());
	}
	// Update is called once per frame
	void Update () {
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
		} else {
			Debug.Log (r.errorCode);
			Debug.Log (r.errorText);
		}
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

	void DisplayMain (GUIStyle buttonStyle) {

		int verticalPadding = (int)(buttonHeight * 0.5);

		int y = verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), authenticateText, buttonStyle)) {
			Debug.Log("Authenticate Called");
			uint requestId = SessionFactory.getSession().authenticate("AuthenticateCallback");
			Debug.Log (requestId);
		}
		
		y += buttonHeight + verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Reset Authentication", buttonStyle)) {
			Debug.Log("Reset Authentication Called");
			uint requestId = SessionFactory.getSession().resetAuthentication("ResetAuthenticationCallback");
			Debug.Log (requestId);
		}
		
		y += buttonHeight + verticalPadding * 2;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Get Friends Profiles", buttonStyle)) {
			Debug.Log("GetFriendsProfiles Called");
			uint requestId = SessionFactory.getSession().getCachedFriends("GetFriendsCallback");
			Debug.Log (requestId);
			selectedScreen = View.FriendsProfiles;
		}

		y += buttonHeight + verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Get My Profile", buttonStyle)) {
			Debug.Log("GetMyProfile Called");
			uint requestId = SessionFactory.getSession().getMyProfile("GetMyProfileCallback");
			Debug.Log (requestId);
			selectedScreen = View.MyProfile;
		}

		y += buttonHeight + verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Get Access Token", buttonStyle)) {
		        Debug.Log("GetAccessToken Called");
			uint requestId = SessionFactory.getSession().getAccessToken("PrintCallback");
			Debug.Log (requestId);
		}

		y += buttonHeight + verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Get Advertisement", buttonStyle)) {
		        Debug.Log("GetAdvertisement Called");
			uint requestId = SessionFactory.getSession().getAdvertisement("PrintCallback");
			Debug.Log (requestId);
		}

		y += buttonHeight + verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Get Leaderboard", buttonStyle)){
			Debug.Log("GetLeaderboard Called");
			List<LeaderboardMetric> metrics = new List<LeaderboardMetric>();
			LeaderboardMetric m = new LeaderboardMetric("score", RollUpCode.MAX_THIS_WEEK, false);
			metrics.Add(m);
			Debug.Log(metrics);
			uint requestId = SessionFactory.getSession().getLeaderboard(metrics, "GetLeaderboardCallback");
			Debug.Log(requestId);
			selectedScreen = View.Leaderboard;
		}

		GUI.Label(new Rect(horizontalPadding, Screen.height-buttonHeight-50, buttonWidth, buttonHeight), version + " (" + environmentName + ")");
	}

	void DisplayFriendsProfiles (GUIStyle buttonStyle) {
		int friendCount = friendNames.Count;
		Rect scrollViewRect = new Rect(0, 200, Screen.width, Screen.height-200-10);
		scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, new Rect(scrollViewRect.xMin, scrollViewRect.yMin, scrollViewRect.width - 50, friendCount * buttonHeight));
		for (int i = 0; i < friendCount; i++) {
			if (GUI.Button(new Rect(horizontalPadding, scrollViewRect.yMin + i * buttonHeight, buttonWidth, buttonHeight), friendNames[i]+"\n"+friendIds[i], buttonStyle)) {
				selectedFriendIndex = i;
				selectedScreen = View.SelectedFriend;
			}
		}
		GUI.EndScrollView();
	}

	void DisplayLeaderboard(GUIStyle buttonStyle) {
		GUI.Label(new Rect(horizontalPadding, 20, buttonWidth, buttonHeight), "Leaderboard of Max this week");
		int start = 210;
		foreach(string s in leaderboardEntries){
			GUI.Button(new Rect(horizontalPadding, start, buttonWidth, buttonHeight), s, buttonStyle);
			start += 130;
		}
	}

	void DisplaySelectedFriend (GUIStyle buttonStyle) {
		if (GUI.Button(new Rect(horizontalPadding, 200, buttonWidth, buttonHeight), "Send Message", buttonStyle)) {
	            	Debug.Log("SendMsg Called");
	            	List<string> recipients = new List<string>();
	           	recipients.Add(friendIds[selectedFriendIndex]);
	       		string description = "I sent you an SDK text!";
	      		string messageText = "Main body text here.";
			uint requestId = SessionFactory.getSession().sendMessageToRecipients (recipients, description, null, 
						    messageText, null, "PrintCallback");
			Debug.Log (requestId);
		}
		
		if (GUI.Button(new Rect(horizontalPadding, 330, buttonWidth, buttonHeight), "Send Gift Message", buttonStyle)) {
            		Debug.Log("SendMsg Called");
            		List<string> recipients = new List<string>();
            		recipients.Add(friendIds[selectedFriendIndex]);
            	  string notificationText = "This is a simple gift";
                string linkText = "Collect gift";
                string giftType = "200-Golden-Nuggets";	
			uint requestId = SessionFactory.getSession().sendGiftMessageToRecipients (recipients,notificationText, linkText, giftType,"PrintCallback");
			Debug.Log (requestId);
		}

		if (GUI.Button(new Rect(horizontalPadding, 460, buttonWidth, buttonHeight), "Send Image Message", buttonStyle)) {
            		Debug.Log("SendContentMsg Called");
            		List<string> recipients = new List<string>();
            		recipients.Add(friendIds[selectedFriendIndex]);
            		string description = "Check out this cool picture from UnityRock!";
            		string messageText = "This was uploaded to the Tango content servers from an external application!";
            		string actionPrompt = "Tap to see the image!";
            		byte[] contentData = content.EncodeToPNG();
			uint requestId = SessionFactory.getSession().sendMessageToRecipients (recipients, description, null, null, null, 
						    "image/png",  contentData, actionPrompt, null, messageText, "UploadCallback");
			Debug.Log (requestId);
		}

		if (GUI.Button(new Rect(horizontalPadding, 590, buttonWidth, buttonHeight), "Send Image with URL Thumb", buttonStyle)) {
            		Debug.Log("SendContentMsg Called");
            		List<string> recipients = new List<string>();
            		recipients.Add(friendIds[selectedFriendIndex]);
            		string description = "Check out this cool picture from UnityRock!";
            		string thumbnailUrl = "http://www.tango.me/images/logo_lg.png";
            		string messageText = "This was uploaded to the Tango content servers from an external application!";
            		string actionPrompt = "Tap to see the image!";
            		byte[] contentData = content.EncodeToPNG();
			uint requestId = SessionFactory.getSession().sendMessageToRecipients (recipients, description, thumbnailUrl, null, null, 
						    "image/png",  contentData, actionPrompt, null, messageText, "UploadCallback");
			Debug.Log (requestId);
		}

		if (GUI.Button(new Rect(horizontalPadding, 720, buttonWidth, buttonHeight), "Send Image with Uploaded Thumb", buttonStyle)) {
            		Debug.Log("SendContentMsg Called");
            		List<string> recipients = new List<string>();
            		recipients.Add(friendIds[selectedFriendIndex]);
            		string description = "Check out this cool picture from UnityRock!";
            		string messageText = "This was uploaded to the Tango content servers from an external application!";
            		string actionPrompt = "Tap to see the image!";
            		byte[] thumbnailData = thumbnail.EncodeToPNG();
            		byte[] contentData = content.EncodeToPNG();
			          uint requestId = SessionFactory.getSession().sendMessageToRecipients (recipients, description, null, "image/png", thumbnailData, 
						        "image/png", contentData, actionPrompt, null, messageText, "UploadCallback");
			Debug.Log (requestId);
		}

		if (GUI.Button(new Rect(horizontalPadding, 850, buttonWidth, buttonHeight), "Send Image with URL Conversion", buttonStyle)) {
           		Debug.Log("SendContentMsg Called");
           		List<string> recipients = new List<string>();
           		recipients.Add(friendIds[selectedFriendIndex]);
           		string description = "Check out this cool picture from UnityRock!";
           		string messageText = "This was uploaded to the Tango content servers from an external application!";
           		string actionPrompt = "Tap to see the image!";
           		ContentConverterCallback converter = (uploadDetails) => {
			Dictionary<Platform, MessagingAction> actionMap = new Dictionary<Platform, MessagingAction>();
			actionMap.Add (Platform.PlatformAny, new MessagingAction("Tap to open converted URL", uploadDetails[ContentConverter.CONTENT_URL], "text/url"));
			return actionMap;
            };
            byte[] thumbnailData = thumbnail.EncodeToPNG();
            byte[] contentData = content.EncodeToPNG();
			uint requestId = SessionFactory.getSession().sendMessageToRecipients (recipients, description, null, "image/png", thumbnailData, 
						    "image/png",  contentData, actionPrompt, converter, messageText, "UploadCallback");
			Debug.Log (requestId);
		}
	}
	void DisplayMyProfile(GUIStyle buttonStyle) {
		string defaultPossessionId = "foo";
    long defaultPossessionValue = 4;
    string defaultMetricId = "score";
    long defaultMetricValue = 2;
    
    int verticalPadding = (int)(buttonHeight * 0.5);
    int y = buttonHeight * 2;

		GUI.Label(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), userName+"\n"+userId);
    y += buttonHeight + verticalPadding;


		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Get All Possessions", buttonStyle)) {
            		Debug.Log("GetAllPossessions Called");
			uint requestId = SessionFactory.getSession().getAllPossessions("PossessionsCallback");
			Debug.Log (requestId);
		}
    y += buttonHeight + verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Set Possessions", buttonStyle)) {
            		Debug.Log("SetPossessions Called");
            		List<Possession> possessions = new List<Possession>();
            		possessions.Add(new Possession(defaultPossessionId, defaultPossessionValue, possessionVersion, "0")); 
			uint requestId = SessionFactory.getSession().setPossessions(possessions, "PossessionsCallback");
			Debug.Log (requestId);
		}
    y += buttonHeight + verticalPadding;


  if(GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Share a post", buttonStyle))  {
        SharingData data = new SharingData();
        data.SetCaption("Captain UnityRock.");
				//TODO: Add support for Android UnityRock to post SDK image feed.
        #if UNITY_IPHONE
        Uri image = new Uri(Application.streamingAssetsPath + "/Resources/UnityRockThumbnail.jpg");
        String mime = "image/jpg";
        data.SetMedia(image, mime);
        #endif

        data.SetNotificationText("Check it out!");
        data.SetLinkText("Tap Now !");
        uint requestId = SessionFactory.getSession().share(data, null, "PrintCallback");
        Debug.Log(requestId);
    }

    y += buttonHeight + verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Get Computed Metrics", buttonStyle)) {
            		Debug.Log("GetComputedMetrics Called");
            		List<string> accountIds = new List<string>();
            		accountIds.Add(userId);
           		List<Metric> metrics = new List<Metric>();
           		Metric score = new Metric(defaultMetricId);
           		score.addRollUp(RollUpCode.MAX_THIS_WEEK);
            		score.addRollUp(RollUpCode.MAX);
            		metrics.Add(score);
			uint requestId = SessionFactory.getSession().getComputedMetrics(accountIds, metrics, "PrintCallback");
			Debug.Log (requestId);
		}
    y += buttonHeight + verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Set Raw Metrics", buttonStyle)) {
			List<Metric> metrics = new List<Metric>();
            Metric score = new Metric(defaultMetricId, defaultMetricValue);
            score.addRollUp(RollUpCode.MAX_THIS_WEEK);
            metrics.Add(score);
			uint requestId = SessionFactory.getSession().setRawMetrics(metrics, "PrintCallback");
			Debug.Log (requestId);
		}
    y += buttonHeight + verticalPadding;

		if (GUI.Button(new Rect(horizontalPadding, y, buttonWidth, buttonHeight), "Get Profile Photo", buttonStyle)) {
			selectedScreen = View.MyProfilePhoto;
		}
  	}

	void DisplayMyProfilePhoto(GUIStyle buttonStyle) {
        if (profilePhoto != null) {
		GUI.DrawTexture(new Rect(10, 200, 176, 176), profilePhoto, ScaleMode.ScaleToFit, false);
	} else {
		StartCoroutine(LoadImage());
	}
        
	}

	IEnumerator LoadImage() {
		WWW www = new WWW(userPhotoUrl);
		yield return www;
		Texture2D texTmp = new Texture2D(176, 176);
		www.LoadImageIntoTexture(texTmp);
		profilePhoto = texTmp;
	}
	
	void OnGUI () {
		
		GUIStyle buttonStyle = new GUIStyle( GUI.skin.button );
		buttonStyle.fontSize = 30;

		GUI.skin.label.fontSize = 30;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;

		if (selectedScreen != View.Main && GUI.Button(new Rect(horizontalPadding, 70, buttonWidth, buttonHeight), "Back", buttonStyle)) {
			if (selectedScreen == View.SelectedFriend) {
				selectedScreen = View.FriendsProfiles;
			} else if (selectedScreen == View.MyProfilePhoto) {
				selectedScreen = View.MyProfile;
				profilePhoto = null;
			} else{
				selectedScreen = View.Main;
			}
		}

		switch (selectedScreen)
		{
			case View.Main: 
				DisplayMain(buttonStyle);
				break;
			case View.FriendsProfiles:
				DisplayFriendsProfiles(buttonStyle);
				break;
			case View.SelectedFriend:
				DisplaySelectedFriend(buttonStyle);
				break;
			case View.MyProfile:
				DisplayMyProfile(buttonStyle);
				break;
			case View.MyProfilePhoto:
				DisplayMyProfilePhoto(buttonStyle);
				break;
      case View.Leaderboard:
        DisplayLeaderboard(buttonStyle);
				break;
		}
	}
	
	void OnApplicationPause( bool pauseStatus){
		if(SessionFactory.getSession().tangoIsInstalled() && SessionFactory.getSession().tangoHasSdkSupport()
                     && !SessionFactory.getSession().isAuthenticated()){
			authenticateText = "Authenticate";
		}
	}
	
}
