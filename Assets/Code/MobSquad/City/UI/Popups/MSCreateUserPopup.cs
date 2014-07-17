using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSCreateUserPopup : MonoBehaviour {
	
	[SerializeField]
	MSActionButton submitButton;
	
	[SerializeField]
	UILabel inputLabel;
	
	[SerializeField]
	UILabel errorLabel;

	[SerializeField]
	UMQLoader loader;
	
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
			create.gems = 50;

			if (FB.IsLoggedIn)
			{
				create.facebookId = FB.UserId;
			}

			UMQNetworkManager.instance.SendRequest(create, (int)EventProtocolRequest.C_USER_CREATE_EVENT, OnUserCreateResponse);
			
			submitButton.able = false;
			MSActionManager.Popup.CloseAllPopups();
		}
	}
	
	void OnUserCreateResponse(int tagNum)
	{
		UserCreateResponseProto response = (UserCreateResponseProto)UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status == UserCreateResponseProto.UserCreateStatus.SUCCESS)
		{
			loader.StartCoroutine(loader.Start());
		}
		else
		{
			errorLabel.text = response.status.ToString();
			MSActionManager.Popup.OnPopup(GetComponent<MSPopup>());
			submitButton.able = true;
		}
	}
}
