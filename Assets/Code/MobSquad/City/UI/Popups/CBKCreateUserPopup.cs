using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKCreateUserPopup : MonoBehaviour {
	
	[SerializeField]
	CBKActionButton submitButton;
	
	[SerializeField]
	UILabel inputLabel;
	
	[SerializeField]
	UILabel errorLabel;
	
	void OnEnable()
	{
		submitButton.onClick += OnSubmit;
	}
	
	void OnDisable()
	{
		submitButton.onClick -= OnSubmit;
	}
	
	void OnSubmit()
	{
		if (inputLabel.text.Length > 0)
		{
			UserCreateRequestProto create = new UserCreateRequestProto();
			create.udid = UMQNetworkManager.udid;
			create.name = inputLabel.text;

			if (FB.IsLoggedIn)
			{
				create.facebookId = FB.UserId;
			}

			UMQNetworkManager.instance.SendRequest(create, (int)EventProtocolRequest.C_USER_CREATE_EVENT, OnUserCreateResponse);
			
			submitButton.able = false;
		}
	}
	
	void OnUserCreateResponse(int tagNum)
	{
		UserCreateResponseProto response = (UserCreateResponseProto)UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status == UserCreateResponseProto.UserCreateStatus.SUCCESS)
		{
			//Doesn't work right now, is this getting fixed? If not, we don't know UserId to start up the networking queue...
			//MSUtil.LoadLocalUser(response.sender);
			
			MSWhiteboard.currSceneType = MSWhiteboard.SceneType.CITY;
			MSValues.Scene.ChangeScene(MSValues.Scene.Scenes.STARTING_SCENE);
		}
		else
		{
			errorLabel.text = response.status.ToString();
			submitButton.able = true;
		}
	}
}
