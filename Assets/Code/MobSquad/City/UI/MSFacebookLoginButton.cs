using UnityEngine;
using System.Collections;

public class MSFacebookLoginButton : MonoBehaviour {

	[SerializeField]
	UISprite loginButton;

	[SerializeField]
	GameObject avatar;

	[SerializeField]
	UITexture faceAvatar;

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
		MSFacebookManager.instance.Init();
	}
}
