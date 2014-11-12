using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class UMQLoader : MonoBehaviour {

	public static UMQLoader instance;
	
	[SerializeField]
	GameObject createUserPopup;

	[SerializeField]
	MSFillBar fillBar;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	public IEnumerator Start () {

		Kamcord.WhitelistAll();
		Kamcord.DoneChangingWhitelist();

		TouchScreenKeyboard.hideInput = true;

		Application.targetFrameRate = 60;

		foreach (var item in MSSpriteUtil.instance.immediateBundles) 
		{
			item.Download();
		}

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
		
		startup.udid = UMQNetworkManager.instance.udid;
		startup.versionNum = MSValues.version;
		
		if (MSFacebookManager.instance.hasFacebook)
		{
			MSFacebookManager.instance.Init();
			while (!MSFacebookManager.instance.hasTriedLogin)
			{
				yield return null;
			}
			startup.fbId = FB.UserId;
		}

		if (PlayerPrefs.HasKey("CleanStart"))
		{
			startup.isFreshRestart = true;
			PlayerPrefs.DeleteKey("CleanStart");
			PlayerPrefs.Save();
		}

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
		
		MSResourceManager.instance.Init(response.sender.level, response.sender.experience, 
		                                response.sender.cash, response.sender.oil, response.sender.gems);

 		StartUp(response);
			
		MSWhiteboard.currSceneType = MSWhiteboard.SceneType.CITY;

		foreach (var item in MSMonsterManager.instance.userMonsters) 
		{
			MSSpriteUtil.instance.RunDownloadAndCache(item.monster.imagePrefix);
		}

		yield return MSBuildingManager.instance.RunLoadPlayerCity();

		foreach (var item in MSMonsterManager.instance.userMonsters) 
		{
			//while (!MSSpriteUtil.instance.HasBundle(item.monster.imagePrefix))
			//{
			//	yield return null;
			//}
		}

		foreach (var item in MSSpriteUtil.instance.immediateBundles) 
		{
			while (!item.loaded)
			{
				yield return null;
			}
		}

		if (response.curTask != null && response.curTask.taskId > 0)
		{
			MSActionManager.Scene.OnPuzzle();
			PZCombatManager.instance.RunInitLoadedTask(response.curTask, response.curTaskStages);
		}
		else
		{
			MSActionManager.Scene.OnCity();
		}

	}

	public static void StartUp(StartupResponseProto response)
	{
		MSUtil.LoadLocalUser (response.sender);
		
		MSChatManager.instance.Init(response);
		
		MSQuestManager.instance.Init(response);
		
		MSClanManager.instance.Init(response.userClanInfo);
		
		MSRequestManager.instance.Init(response.invitesToMeForSlots);
		
		MSResidenceManager.instance.AddInvites(response.invitesFromMeForSlots);
		
		MSNewsPopup.pvpHistory = response.recentNBattles;
		Debug.LogWarning("Recent battles: " + response.recentNBattles.Count);
		
		MSMonsterManager.instance.Init(response.usersMonsters, response.monstersHealing, response.enhancements);
		MSEvolutionManager.instance.Init(response.evolution);

		MSEnhancementManager.instance.Init(response.enhancements);

		if (MSActionManager.Loading.OnStartup != null)
		{
			MSActionManager.Loading.OnStartup(response);
		}
	}

}
