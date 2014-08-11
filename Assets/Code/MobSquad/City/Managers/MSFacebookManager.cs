using UnityEngine;
using System.Collections;
using Facebook;
using Facebook.MiniJSON;
using System.Collections.Generic;

public class MSFacebookManager : MonoBehaviour {
	
	public static MSFacebookManager instance;

	public List<MSFacebookFriend> friends = new List<MSFacebookFriend>();

	public Dictionary<string, Texture2D> loadedProfilePictures = new Dictionary<string, Texture2D>();
	
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
			TryLoadFriends();
		}
		hasTriedLogin = true;
	}

	[ContextMenu ("Try Friends")]
	public void TryLoadFriends()
	{
		if (isLoggedIn)
		{
			FB.API("me/friends?fields=installed,name,picture,id&limit=100", Facebook.HttpMethod.GET, LoadFriendsCallback);
		}
	}

	void LoadFriendsCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError("Problem getting friends, trying again...");
			FB.API("me/friends?fields=installed,name,picture,id&limit=100", Facebook.HttpMethod.GET, LoadFriendsCallback);
			return;
		}

		Dictionary<string, object> friendsDict = Json.Deserialize(result.Text) as Dictionary<string, object>;

		List<object> friendsData = friendsDict["data"] as List<object>;

		string name = "";
		string id = "";
		string url = "";
		bool installed;
		foreach (var item in friendsData)
		{
			installed = false;
			foreach (var thing in (item as Dictionary<string, object>))
			{
				switch(thing.Key)
				{
				case "name":
					name = thing.Value as string;
					break;
				case "id":
					id = thing.Value as string;
					break;
				case "picture":
					url = (((thing.Value as Dictionary<string, object>)["data"]) as Dictionary<string, object>)["url"] as string;
					break;
				case "installed":
					installed = true;
					break;
				default:
					break;
				}

				//Debug.Log("Key: " + thing.Key + ", Value: " + thing.Value);
			}

			friends.Add(new MSFacebookFriend(name, id, url, installed));
		}

		foreach(var friend in friends)
		{
			//Debug.Log(friend);
		}

		//Debug.Log("Num friends: " + friendsData.Count);

		/*
		Dictionary<string, object> friendDict = friendsData[0] as Dictionary<string, object>;

		foreach (var item in friendDict) 
		{
			Debug.Log("Key: " + item.Key + ", Value: " + item.Value);
		}
		*/
	}

	public Coroutine RunLoadPhotoForUser(string userId, UITexture texture)
	{
		return StartCoroutine(LoadPhotoForUser(userId, texture));
	}

	IEnumerator LoadPhotoForUser(string userId, UITexture texture)
	{
		if (loadedProfilePictures.ContainsKey(userId))
		{
			texture.mainTexture = loadedProfilePictures[userId];
		}
		else
		{
			FBPicRequester requester = new FBPicRequester(userId, texture);
			while (!requester.finished)
			{
				yield return null;
			}
		}
	}
}

public class FBPicRequester
{
	public bool finished = false;
	public string facebookId;
	public UITexture texture;
	public FBPicRequester (string facebookId, UITexture texture)
	{
		finished = false;
		this.facebookId = facebookId;
		this.texture = texture;
		RequestPicture(facebookId, texture);
	}
	void RequestPicture(string facebookId, UITexture texture)
	{
		FB.API("/" + facebookId + "/picture?height=" + texture.height + "&width=" + texture.width, HttpMethod.GET, PictureCallback);
	}
	
	void PictureCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError("Failed to get picture");
			RequestPicture();
			return;
		}
		texture.mainTexture = result.Texture;
		MSFacebookManager.instance.loadedProfilePictures[facebookId] = result.Texture;
		finished = true;
	}
}

