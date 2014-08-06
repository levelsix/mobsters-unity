using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSCreateUserPopup : MonoBehaviour {
	
	[SerializeField]
	MSActionButton submitButton;
	
	[SerializeField]
	UIInput inputLabel;
	
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
		if (inputLabel.label.color == inputLabel.activeTextColor
		    && submitButton.enabled)
		{
			StartCoroutine(SendUsernameRequest(inputLabel.label.text));
		}
	}

	IEnumerator SendUsernameRequest(string username)
	{
		submitButton.enabled = false;

		//TODO: Register the user
		UserCreateRequestProto create = new UserCreateRequestProto();
		create.udid = UMQNetworkManager.udid;
		create.name = username;
		
		create.cash = MSWhiteboard.tutorialConstants.cashInit;
		create.oil = MSWhiteboard.tutorialConstants.oilInit;
		create.gems = MSWhiteboard.tutorialConstants.gemsInit;
		
		if (FB.IsLoggedIn)
		{
			create.facebookId = FB.UserId;
		}
		
		int tagNum = UMQNetworkManager.instance.SendRequest(create, (int)EventProtocolRequest.C_USER_CREATE_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		UserCreateResponseProto response = UMQNetworkManager.responseDict[tagNum] as UserCreateResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		switch (response.status)
		{
		case UserCreateResponseProto.UserCreateStatus.SUCCESS:
			MSTutorialManager.instance.OnUsernameEnter();
			MSActionManager.Popup.CloseAllPopups();
			break;
		default:
			MSActionManager.Popup.DisplayError(response.status.ToString());
			submitButton.enabled = true;
			break;
		}
	}
}
