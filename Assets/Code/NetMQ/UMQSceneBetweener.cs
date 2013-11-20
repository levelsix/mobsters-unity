using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class UMQSceneBetweener : MonoBehaviour {

	IEnumerator Start()
	{
		Debug.LogWarning("A");
		while(UMQNetworkManager.instance.numRequestsOut > 0)
		{
			yield return new WaitForSeconds(1);
			Debug.LogWarning("Waiting...");
		}

		Resources.UnloadUnusedAssets();
		switch (CBKWhiteboard.currSceneType) {
			case CBKWhiteboard.SceneType.PUZZLE:
				StartCoroutine(LoadTask());
				break;
			case CBKWhiteboard.SceneType.CITY:
			default:
				switch (CBKWhiteboard.currCityType)
				{
					default:
					case CBKWhiteboard.CityType.PLAYER:
						StartCoroutine(LoadPlayerCity());
						break;
					case CBKWhiteboard.CityType.NEUTRAL:
						StartCoroutine(LoadNeutralCity());
						break;
				}
				break;
		}
	}
	
	IEnumerator LoadNeutralCity()
	{
		LoadCityRequestProto request = new LoadCityRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.cityId = CBKWhiteboard.cityID;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_LOAD_CITY_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		CBKWhiteboard.loadedNeutralCity = UMQNetworkManager.responseDict[tagNum] as LoadCityResponseProto;
		UMQNetworkManager.responseDict.Remove (tagNum);
		
		CBKValues.Scene.ChangeScene(CBKValues.Scene.Scenes.TOWN_SCENE);
	}
	
	IEnumerator LoadPlayerCity()
	{
		LoadPlayerCityRequestProto request = new LoadPlayerCityRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.cityOwnerId = CBKWhiteboard.cityID;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_LOAD_PLAYER_CITY_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			Debug.Log("Waiting on response: " + tagNum);
			yield return new WaitForSeconds(1);
		}
		
		Debug.Log("Got response");
		
		CBKWhiteboard.loadedPlayerCity = UMQNetworkManager.responseDict[tagNum] as LoadPlayerCityResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		CBKValues.Scene.ChangeScene(CBKValues.Scene.Scenes.TOWN_SCENE);
	}
	
	IEnumerator LoadTask()
	{
		int tagNum = UMQNetworkManager.instance.SendRequest(CBKWhiteboard.dungeonToLoad, (int)EventProtocolRequest.C_BEGIN_DUNGEON_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		CBKWhiteboard.loadedDungeon = UMQNetworkManager.responseDict[tagNum] as BeginDungeonResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		CBKValues.Scene.ChangeScene(CBKValues.Scene.Scenes.PUZZLE_SCENE);
	}
}
