using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKTaskable : MonoBehaviour {

	public FullTaskProto task;
	
	public MinimumUserTaskProto userTask;
	
	public void Init(FullTaskProto proto)
	{
		task = proto;
	}
	
	public void EngageTask()
	{
		//TODO: First, check if there's enough stamina/energy/whatever so we don't waste time loading all this scrote
		
		StartCoroutine(SendDungeonBeginRequest());
	}
	
	private IEnumerator SendDungeonBeginRequest()
	{
		BeginDungeonRequestProto request = new BeginDungeonRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.clientTime = CBKUtil.timeNow;
		request.taskId = task.taskId;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_DUNGEON_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		BeginDungeonResponseProto response = UMQNetworkManager.responseDict[tagNum] as BeginDungeonResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status == BeginDungeonResponseProto.BeginDungeonStatus.SUCCESS)
		{
			PZCombatManager.instance.enemies.Clear();
			
			foreach (TaskStageProto stage in response.tsp)
			{
				PZCombatManager.instance.enemies.Enqueue(stage.stageMonsters[0]);
				Debug.Log("Stage: " + stage.stageId + ": Adding monster " + stage.stageMonsters[0].monsterId);
			}
		}
		
		CBKEventManager.Scene.OnPuzzle();
	}
}
