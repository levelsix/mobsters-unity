using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class UMQLoader : MonoBehaviour {
	
	[SerializeField]
	GameObject createUserPopup;

	[SerializeField]
	CBKFillBar fillBar;

	// Use this for initialization
	IEnumerator Start () {

		CBKFacebookManager.instance.Init();

		/*
		while (!CBKFacebookManager.hasTriedLogin)
		{
			yield return null;
		}
		*/

		//Hang here while we set up the connetion
		//TODO: Time out if we've been hanging here for too long
		while(!UMQNetworkManager.instance.ready)
		{
			yield return null;
		}

		fillBar.fill = .2f;
		
		UMQNetworkManager.instance.WriteDebug("Sending StartupRequest");
		
		StartupRequestProto startup = new StartupRequestProto();
		
		startup.udid = UMQNetworkManager.udid;
		startup.versionNum = 1.0f;
		startup.isForceTutorial = false;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(startup, (int) EventProtocolRequest.C_STARTUP_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		fillBar.fill = .75f;
		
		UMQNetworkManager.instance.WriteDebug(tagNum + ": Received StartupResponse");
		
		StartupResponseProto response = (StartupResponseProto) UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		UMQNetworkManager.instance.WriteDebug("Startup Status: " + response.startupStatus.ToString());

		if (response.startupStatus == StartupResponseProto.StartupStatus.USER_NOT_IN_DB)
		{
			CBKEventManager.Popup.OnPopup(createUserPopup);
			yield break;
		}

		fillBar.fill = .9f;
		
		UMQNetworkManager.instance.WriteDebug("Update Status: " + response.updateStatus.ToString());

		if (response.staticDataStuffProto != null)
		{
			CBKDataManager.instance.LoadStaticData(response.staticDataStuffProto);
		}

		CBKUtil.LoadLocalUser (response.sender);
		
		CBKChatManager.instance.Init(response);
		
		CBKQuestManager.instance.Init(response);

		CBKClanManager.instance.Init(response.userClanInfo);
		
		CBKMonsterManager.instance.Init(response.usersMonsters, response.monstersHealing, response.enhancements);
		
		if (response.startupStatus == StartupResponseProto.StartupStatus.USER_NOT_IN_DB)
		{
			CBKEventManager.Popup.OnPopup(createUserPopup);
		}
		else
		{
			CBKWhiteboard.constants = response.startupConstants;
			CBKResourceManager.instance.Init(response.sender.level, response.sender.experience,
				100/*response.experienceRequiredForNextLevel*/, response.sender.cash, response.sender.oil, response.sender.gems);
			
			CBKWhiteboard.currSceneType = CBKWhiteboard.SceneType.CITY;

			LoadPlayerCityRequestProto request = new LoadPlayerCityRequestProto();
			request.sender = CBKWhiteboard.localMup;
			request.cityOwnerId = CBKWhiteboard.cityID;
			
			tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_LOAD_PLAYER_CITY_EVENT, null);
			
			while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
			{
				//Debug.Log("Waiting on response: " + tagNum);
				yield return new WaitForSeconds(1);
			}
			
			Debug.Log("Got response");
			
			CBKWhiteboard.loadedPlayerCity = UMQNetworkManager.responseDict[tagNum] as LoadPlayerCityResponseProto;
			UMQNetworkManager.responseDict.Remove(tagNum);

			CBKValues.Scene.ChangeScene(CBKValues.Scene.Scenes.TOWN_SCENE);
		}
	}

}
