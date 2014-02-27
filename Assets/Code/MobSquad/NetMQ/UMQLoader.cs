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

		Application.targetFrameRate = 30;

		CBKFacebookManager.instance.Init();

		while (!FB.isInitCalled || (!FB.hasFailed && !CBKFacebookManager.hasTriedLogin))
		{
			yield return null;
		}

		Debug.Log("Loader hanging out");

		//Hang here while we set up the connetion
		//TODO: Time out if we've been hanging here for too long
		while(!UMQNetworkManager.instance.ready)
		{
			yield return new WaitForSeconds(1);
			Debug.Log("Loader still waiting");
		}

		fillBar.fill = .2f;
		
		Debug.Log("Sending StartupRequest");
		
		StartupRequestProto startup = new StartupRequestProto();
		
		startup.udid = UMQNetworkManager.udid;
		startup.versionNum = 1.0f;
		startup.isForceTutorial = false;

		if (FB.IsLoggedIn)
		{
			startup.fbId = FB.UserId;
		}
		
		int tagNum = UMQNetworkManager.instance.SendRequest(startup, (int) EventProtocolRequest.C_STARTUP_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		fillBar.fill = .75f;
		
		Debug.Log(tagNum + ": Received StartupResponse");
		
		StartupResponseProto response = (StartupResponseProto) UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		Debug.Log("Startup Status: " + response.startupStatus.ToString());

		if (response.startupStatus == StartupResponseProto.StartupStatus.USER_NOT_IN_DB)
		{
			CBKEventManager.Popup.OnPopup(createUserPopup);
			yield break;
		}

		fillBar.fill = .9f;
		
		Debug.Log("Update Status: " + response.updateStatus.ToString());

		if (response.staticDataStuffProto != null)
		{
			CBKDataManager.instance.LoadStaticData(response.staticDataStuffProto);
		}

		//IMPORTANT: Initialize the constants before ANYTHING with CBKUtil is called
		//Otherwise, the constructor on CBKUtil will fail and throw errors
		CBKWhiteboard.constants = response.startupConstants;

		CBKUtil.LoadLocalUser (response.sender);
		
		CBKChatManager.instance.Init(response);
		
		CBKQuestManager.instance.Init(response);

		CBKClanManager.instance.Init(response.userClanInfo);

		CBKRequestManager.instance.Init(response.invitesToMeForSlots);


		Debug.Log("Invites to me: " + response.invitesToMeForSlots.Count
		          + "\nInvites from me: " + response.invitesFromMeForSlots.Count);

		CBKResidenceManager.instance.AddInvites(response.invitesFromMeForSlots);
		
		if (response.startupStatus == StartupResponseProto.StartupStatus.USER_NOT_IN_DB)
		{
			CBKEventManager.Popup.OnPopup(createUserPopup);
		}
		else
		{
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

			CBKBuildingManager.instance.LoadPlayerCity();

			CBKEventManager.Scene.OnCity();
		}

		CBKMonsterManager.instance.Init(response.usersMonsters, response.monstersHealing, response.enhancements);
		CBKEvolutionManager.instance.Init(response.evolution);
	}

}
