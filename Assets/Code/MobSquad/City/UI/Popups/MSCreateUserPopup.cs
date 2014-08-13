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
		
		create.cash = MSResourceManager.resources[ResourceType.CASH];
		create.oil = MSResourceManager.resources[ResourceType.OIL];
		create.gems = MSResourceManager.resources[ResourceType.GEMS];

		foreach (var item in MSBuildingManager.instance.buildingsBuiltInTutorial) 
		{
			TutorialStructProto tutStruct = new TutorialStructProto();
			tutStruct.coordinate = new CoordinateProto();
			tutStruct.coordinate.x = (int)item.groundPos.x;
			tutStruct.coordinate.y = (int)item.groundPos.y;
			tutStruct.structId = item.combinedProto.structInfo.structId;
			create.structsJustBuilt.Add(tutStruct);
		}

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
			yield return StartCoroutine(QuickStartupRequest());
			MSTutorialManager.instance.OnUsernameEnter();
			MSActionManager.Popup.CloseAllPopups();
			break;
		default:
			MSActionManager.Popup.DisplayError(response.status.ToString());
			submitButton.enabled = true;
			break;
		}
	}

	IEnumerator QuickStartupRequest()
	{
		StartupRequestProto request = new StartupRequestProto();
		request.udid = UMQNetworkManager.udid;
		request.versionNum = MSValues.version;
		if (FB.IsLoggedIn)
		{
			request.fbId = FB.UserId;
		}
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_STARTUP_EVENT, null);
		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		StartupResponseProto response = UMQNetworkManager.responseDict[tagNum] as StartupResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		MSUtil.LoadLocalUser(response.sender);

		MSMonsterManager.instance.Init(response.usersMonsters, response.monstersHealing, response.enhancements);

		MSChatManager.instance.Init(response);
	}
}
