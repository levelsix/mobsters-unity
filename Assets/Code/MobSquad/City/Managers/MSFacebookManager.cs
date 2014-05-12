using UnityEngine;
using System.Collections;
using Facebook;
using Facebook.MiniJSON;
using System.Collections.Generic;

public class MSFacebookManager : MonoBehaviour {
	
	public static MSFacebookManager instance;
	
	public static bool hasTriedLogin = false;
	
	public static bool isLoggedIn = false;
	
	const string permissions = "email,read_friendlists,publish_actions,publish_stream";
	
	const string COLLECT_FROM_BUILDING_DESCRIPTION_FRONT = "I just collected money from my ";
	const string COLLECT_FROM_BUILDING_DESCRIPTION_BACK = "!";
	
	public void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
	
	public void OnEnable()
	{
		//CBKEventManager.Town.OnCollectFromBuilding += ShareCollectFromBuildingToFeed;
	}
	
	public void OnDisable()
	{
		//CBKEventManager.Town.OnCollectFromBuilding -= ShareCollectFromBuildingToFeed;
	}
	
	void ShareCollectFromBuildingToFeed(MSBuilding building)
	{
		if (FB.IsLoggedIn)
		{
			Debug.Log("Sharing?");
			FB.Feed(
				linkDescription: COLLECT_FROM_BUILDING_DESCRIPTION_FRONT + building.name + COLLECT_FROM_BUILDING_DESCRIPTION_BACK
			);
		}
	}
	
	public void Init()
	{
		FB.Init(TryFBLogin, OnHideUnity);
	}
	
	private void TryFBLogin()
	{
		if (!FB.IsLoggedIn)
		{
			FB.Login(permissions, OnLogin);
		}
		else
		{
			hasTriedLogin = true;
		}
	}
	
	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown) Time.timeScale = 0;
		else Time.timeScale = 1;
	}
			
	private void OnLogin(FBResult result)
	{
		isLoggedIn = FB.IsLoggedIn;
		if (isLoggedIn)
		{
			Debug.Log("Logged in as: " + FB.UserId);
		}
		hasTriedLogin = true;
	}

	[ContextMenu ("Try Friends")]
	public void TryLoadFriends()
	{
		if (isLoggedIn)
		{
			FB.API("/me/friends", Facebook.HttpMethod.GET, LoadFriendsCallback);
		}
	}

	void LoadFriendsCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError("Problem getting friends, trying again...");
			FB.API("/me/friends", Facebook.HttpMethod.GET, LoadFriendsCallback);
			return;
		}

		Dictionary<string, object> friends = Json.Deserialize(result.Text) as Dictionary<string, object>;

		foreach (var item in (friends["data"] as List<object>)) 
		{
			Debug.Log("Item: " + item);
		}
	}

			    
	
	
}
