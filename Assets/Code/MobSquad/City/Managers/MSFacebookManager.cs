using UnityEngine;
using System.Collections;
using Facebook;
using Facebook.MiniJSON;
using System.Collections.Generic;

public class MSFacebookManager : MonoBehaviour {
	
	public static MSFacebookManager instance;

	public List<MSFacebookFriend> friends = new List<MSFacebookFriend>();

	public Dictionary<string, Texture2D> loadedProfilePictures = new Dictionary<string, Texture2D>();
	
	public bool hasTriedLogin = false;
	
	const string permissions = "email,read_friendlists,publish_actions,publish_stream";
	
	const string COLLECT_FROM_BUILDING_DESCRIPTION_FRONT = "I just collected money from my ";
	const string COLLECT_FROM_BUILDING_DESCRIPTION_BACK = "!";

	public const string FB_KEY = "SAVE_KEY_FACEBOOK";

	public bool hasFacebook = false;
	
	public void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);

		hasFacebook = PlayerPrefs.GetInt(FB_KEY, 0) == 1;
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
		hasTriedLogin = false;
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
			Debug.Log("Already logged in: " + FB.UserId);
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
		if (FB.IsLoggedIn)
		{
			Debug.Log("Logged in as: " + FB.UserId);
			MSFacebookManager.instance.LoadInvitableFriends();
			if(MSActionManager.Facebook.OnLoginSucces != null)
			{
				MSActionManager.Facebook.OnLoginSucces();
			}
		}
		else
		{
			if(MSActionManager.Facebook.OnLoginFail != null)
			{
				MSActionManager.Facebook.OnLoginFail();
			}
		}
		hasTriedLogin = true;
	}

	[ContextMenu ("Try Friends")]
	public void TryLoadFriends()
	{
		if (FB.IsLoggedIn)
		{
			FB.API("me/friends?fields=name,picture,id&limit=100", Facebook.HttpMethod.GET, LoadFriendsCallback);
		}
	}

	public void LoadInvitableFriends()
	{
		if (FB.IsLoggedIn)
		{
			FB.API("me/invitable_friends?fields=installed,name,picture,id&limit=100", Facebook.HttpMethod.GET, LoadFriendsCallback);
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
		friends.Clear();
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
			Debug.Log(friend);
		}

		Debug.Log("Num friends: " + friendsData.Count);

		/*
		Dictionary<string, object> friendDict = friendsData[0] as Dictionary<string, object>;

		foreach (var item in friendDict) 
		{
			Debug.Log("Key: " + item.Key + ", Value: " + item.Value);
		}
		*/

		if(MSActionManager.Facebook.OnLoadFriends != null)
		{
			MSActionManager.Facebook.OnLoadFriends();
		}
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
		RequestPicture();
	}
	void RequestPicture()
	{
		FB.API("/" + facebookId + "/picture?height=" + texture.height + "&width=" + texture.width, HttpMethod.GET, PictureCallback);
	}
	
	void PictureCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError("Failed to get picture, will try again");
			RequestPicture();
			return;
		}
		texture.mainTexture = result.Texture;
		MSFacebookManager.instance.loadedProfilePictures[facebookId] = result.Texture;
		finished = true;
	}
}

