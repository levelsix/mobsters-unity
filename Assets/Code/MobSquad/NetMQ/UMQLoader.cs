using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class UMQLoader : MonoBehaviour {
	
	[SerializeField]
	GameObject createUserPopup;

	[SerializeField]
	MSFillBar fillBar;

	// Use this for initialization
	public IEnumerator Start () {

		Application.targetFrameRate = 60;

		/*
		MSFacebookManager.instance.Init();

		while (!FB.isInitCalled || (!FB.hasFailed && !MSFacebookManager.hasTriedLogin))
		{
			yield return null;
		}
		*/

		//Debug.Log("Loader hanging out");

		//Hang here while we set up the connetion
		//TODO: Time out if we've been hanging here for too long
		while(!UMQNetworkManager.instance.ready)
		{
			yield return new WaitForSeconds(1);
			//Debug.Log("Loader still waiting");
		}

		fillBar.fill = .2f;
		
		//Debug.Log("Sending StartupRequest");
		
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
		
		//Debug.Log(tagNum + ": Received StartupResponse");
		
		StartupResponseProto response = (StartupResponseProto) UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		Debug.Log("Startup Status: " + response.startupStatus.ToString());

		if (response.startupStatus == StartupResponseProto.StartupStatus.USER_NOT_IN_DB)
		{
			MSActionManager.Popup.OnPopup(createUserPopup.GetComponent<MSPopup>());
			yield break;
		}

		fillBar.fill = .9f;
		
		Debug.Log("Update Status: " + response.updateStatus.ToString());

		if (response.staticDataStuffProto != null)
		{
			MSDataManager.instance.LoadStaticData(response.staticDataStuffProto);
		}

		//IMPORTANT: Initialize the constants before ANYTHING with CBKUtil is called
		//Otherwise, the constructor on CBKUtil will fail and throw errors
		MSWhiteboard.constants = response.startupConstants;

		MSUtil.LoadLocalUser (response.sender);
		
		MSChatManager.instance.Init(response);
		
		MSQuestManager.instance.Init(response);

		MSClanManager.instance.Init(response.userClanInfo);

		MSRequestManager.instance.Init(response.invitesToMeForSlots);

		MSResidenceManager.instance.AddInvites(response.invitesFromMeForSlots);
		
		MSMonsterManager.instance.Init(response.usersMonsters, response.monstersHealing, response.enhancements);
		MSEvolutionManager.instance.Init(response.evolution);

		Debug.LogError("Global chats: " + response.globalChats.Count
		               + "\nClan chats: " + response.clanChats.Count
		               + "\nPrivate chats: " + response.pcpp.Count);

		if (MSActionManager.Loading.OnStartup != null)
		{
			MSActionManager.Loading.OnStartup(response);
		}

		if (response.startupStatus == StartupResponseProto.StartupStatus.USER_NOT_IN_DB)
		{
			MSActionManager.Popup.OnPopup(createUserPopup.GetComponent<MSPopup>());
		}
		else
		{
			MSResourceManager.instance.Init(response.sender.level, response.sender.experience,
				100/*response.experienceRequiredForNextLevel*/, response.sender.cash, response.sender.oil, response.sender.gems);
			
			MSWhiteboard.currSceneType = MSWhiteboard.SceneType.CITY;

			StartCoroutine(MSBuildingManager.instance.LoadPlayerCity());
		}

	}

}
