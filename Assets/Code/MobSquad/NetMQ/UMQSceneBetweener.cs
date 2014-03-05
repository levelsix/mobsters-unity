using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class UMQSceneBetweener : MonoBehaviour {

	IEnumerator Start()
	{
		while(UMQNetworkManager.instance.numRequestsOut > 0)
		{
			yield return new WaitForSeconds(1);
			Debug.LogWarning("Waiting...");
		}

		Resources.UnloadUnusedAssets();
		switch (MSWhiteboard.currSceneType) {
			case MSWhiteboard.SceneType.PUZZLE:
				StartCoroutine(LoadTask());
				break;
			case MSWhiteboard.SceneType.CITY:
			default:
				switch (MSWhiteboard.currCityType)
				{
					default:
					case MSWhiteboard.CityType.PLAYER:
						StartCoroutine(LoadPlayerCity());
						break;
					case MSWhiteboard.CityType.NEUTRAL:
						StartCoroutine(LoadNeutralCity());
						break;
				}
				break;
		}
	}
	
	IEnumerator LoadNeutralCity()
	{
		LoadCityRequestProto request = new LoadCityRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.cityId = MSWhiteboard.cityID;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_LOAD_CITY_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		MSWhiteboard.loadedNeutralCity = UMQNetworkManager.responseDict[tagNum] as LoadCityResponseProto;
		UMQNetworkManager.responseDict.Remove (tagNum);
		
		MSValues.Scene.ChangeScene(MSValues.Scene.Scenes.TOWN_SCENE);
	}
	
	IEnumerator LoadPlayerCity()
	{
		LoadPlayerCityRequestProto request = new LoadPlayerCityRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.cityOwnerId = MSWhiteboard.cityID;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_LOAD_PLAYER_CITY_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			Debug.Log("Waiting on response: " + tagNum);
			yield return new WaitForSeconds(1);
		}
		
		Debug.Log("Got response");
		
		MSWhiteboard.loadedPlayerCity = UMQNetworkManager.responseDict[tagNum] as LoadPlayerCityResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		MSValues.Scene.ChangeScene(MSValues.Scene.Scenes.TOWN_SCENE);
	}
	
	IEnumerator LoadTask()
	{
		int tagNum = UMQNetworkManager.instance.SendRequest(MSWhiteboard.dungeonToLoad, (int)EventProtocolRequest.C_BEGIN_DUNGEON_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		MSWhiteboard.loadedDungeon = UMQNetworkManager.responseDict[tagNum] as BeginDungeonResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		MSValues.Scene.ChangeScene(MSValues.Scene.Scenes.PUZZLE_SCENE);
	}
}
