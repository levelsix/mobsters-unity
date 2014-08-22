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

		Kamcord.WhitelistAll();
		Kamcord.DoneChangingWhitelist();

		TouchScreenKeyboard.hideInput = true;

		Application.targetFrameRate = 60;

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
		startup.versionNum = MSValues.version;

		if (PlayerPrefs.HasKey("CleanStart"))
		{
			startup.isFreshRestart = true;
			PlayerPrefs.DeleteKey("CleanStart");
			PlayerPrefs.Save();
		}

		/*
		MSFacebookManager.instance.Init();

		while (!MSFacebookManager.hasTriedLogin)
		{
			yield return null;
		}
		*/

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
		
		StartupResponseProto response = (StartupResponseProto) UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		Debug.Log("Startup Status: " + response.startupStatus.ToString());
		
		//IMPORTANT: Initialize the constants before ANYTHING with CBKUtil is called
		//Otherwise, the constructor on CBKUtil will fail and throw errors
		MSWhiteboard.constants = response.startupConstants;
		
		if (response.staticDataStuffProto != null)
		{
			MSDataManager.instance.LoadStaticData(response.staticDataStuffProto);
		}

		if (response.startupStatus == StartupResponseProto.StartupStatus.USER_NOT_IN_DB)
		{
			fillBar.fill = 1;
			MSWhiteboard.tutorialConstants = response.tutorialConstants;
			MSTutorialManager.instance.StartBeginningTutorial();
			yield break;
		}
		
		fillBar.fill = .9f;
		
		Debug.Log("Update Status: " + response.updateStatus.ToString());

		MSUtil.LoadLocalUser (response.sender);
		
		MSChatManager.instance.Init(response);
		
		MSQuestManager.instance.Init(response);

		MSClanManager.instance.Init(response.userClanInfo);

		MSRequestManager.instance.Init(response.invitesToMeForSlots);

		MSResidenceManager.instance.AddInvites(response.invitesFromMeForSlots);
		
		MSMonsterManager.instance.Init(response.usersMonsters, response.monstersHealing, response.enhancements);
		MSEvolutionManager.instance.Init(response.evolution);

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
				MSWhiteboard.nextLevelInfo.requiredExperience, response.sender.cash, response.sender.oil, response.sender.gems);
			
			MSWhiteboard.currSceneType = MSWhiteboard.SceneType.CITY;

			yield return MSBuildingManager.instance.RunLoadPlayerCity();

			PZCombatSave save = PZCombatSave.Load();

			if (response.curTask != null && response.curTask.taskId > 0)
			{
				//PZCombatManager.instance.RunInitLoadedTask(response.curTask, response.curTaskStages);
				//MSActionManager.Scene.OnPuzzle();
				MSActionManager.Scene.OnCity();
			}
			else
			{
				MSActionManager.Scene.OnCity();
			}
		}

	}

}
