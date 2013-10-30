using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class UMQLoader : MonoBehaviour {
	
	[SerializeField]
	GameObject createUserPopup;
	
	/// <summary>
	/// The player unit.
	/// DEBUG until we get a UnitManager
	/// </summary>
	[SerializeField]
	CBKUnit playerUnit;
	
	// Use this for initialization
	IEnumerator Start () {
		
		//Hang here while we set up the connetion
		//TODO: Time out if we've been hanging here for too long
		while(!UMQNetworkManager.instance.ready)
		{
			yield return null;
		}
		
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
		
		UMQNetworkManager.instance.WriteDebug(tagNum + ": Received StartupResponse");
		
		StartupResponseProto response = (StartupResponseProto) UMQNetworkManager.responseDict[tagNum];
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		UMQNetworkManager.instance.WriteDebug(response.startupStatus.ToString());
		
		UMQNetworkManager.instance.WriteDebug(response.updateStatus.ToString());
		
		//Debug.Log("Last Login: " + response.sender.lastLoginTime + ", Current: " + CBKUtil.timeNow);
		
		RetrieveStaticDataRequestProto staticDataRequest = new RetrieveStaticDataRequestProto();
		
		LoadStaticData(response);
		
		CBKChatManager.instance.Init(response);
		
		if (response.startupStatus == StartupResponseProto.StartupStatus.USER_NOT_IN_DB)
		{
			CBKEventManager.Popup.OnPopup(createUserPopup);
		}
		else
		{
			CBKWhiteboard.constants = response.startupConstants;
			CBKUtil.LoadLocalUser (response.sender);
			CBKEventManager.Loading.LoadBuildings();
			CBKResourceManager.instance.Init(response.sender.level, response.sender.experience,
				100/*response.experienceRequiredForNextLevel*/, response.sender.cash, response.sender.gems);
			
			
			if (CBKEventManager.Scene.OnCity != null)
			{
				CBKEventManager.Scene.OnCity();
			}
			
		}
		
		CBKQuestManager.instance.Init(response, staticDataRequest);
		
		CBKMonsterManager.instance.Init(response.usersMonsters, response.monstersHealing);
		
		foreach (FullCityProto city in response.allCities) 
		{
			foreach (int item in city.taskIds) 
			{
				staticDataRequest.taskIds.Add(item);
			}
		}
		
		CBKDataManager.instance.RequestStaticData(staticDataRequest);
	}
	
	void LoadStaticData(StartupResponseProto response)
	{
		foreach (var item in response.staticStructs) 
		{
			CBKDataManager.instance.Load(item, item.structId);
		}
		foreach (var item in response.staticMonsters) 
		{
			CBKDataManager.instance.Load(item, item.monsterId);
		}
	}
}
