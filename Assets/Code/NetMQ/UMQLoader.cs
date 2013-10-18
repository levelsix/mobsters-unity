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
		
		yield return new WaitForSeconds(2);
		
		UMQNetworkManager.instance.WriteDebug("Sending StartupRequest");
		
		StartupRequestProto startup = new StartupRequestProto();
		
		startup.udid = UMQNetworkManager.udid;
		startup.versionNum = 1.0f;
		startup.isForceTutorial = false;
		
		UMQNetworkManager.instance.SendRequest(startup, (int) EventProtocolRequest.C_STARTUP_EVENT, LoadStartupResponse);

	}
	
	/// <summary>
	/// Loads the startup response once it has been received
	/// </summary>
	/// <param name='tagNum'>
	/// Tag number.
	/// </param>
	void LoadStartupResponse(int tagNum)
	{
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
			CBKUtil.LoadLocalUser (response.sender);
			CBKEventManager.Loading.LoadBuildings();
			CBKResourceManager.instance.Init(response.sender.level, response.sender.experience,
				response.experienceRequiredForNextLevel, response.sender.coins, response.sender.diamonds);
		}
		
		CBKQuestManager.instance.Init(response, staticDataRequest);
		
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
	}
}
