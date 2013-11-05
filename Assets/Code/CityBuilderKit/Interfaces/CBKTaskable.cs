using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using System.Collections.Generic;

public class CBKTaskable : MonoBehaviour {

	public FullTaskProto task;
	
	public MinimumUserTaskProto userTask;
	
	public void Init(FullTaskProto proto)
	{
		task = proto;
	}
	
	public void EngageTask()
	{
		if (CBKMonsterManager.instance.monstersOnTeam == 0)
		{
			Debug.Log("No monsters on team!");
			return;
		}
		else
		{
			foreach (var item in CBKMonsterManager.instance.userTeam) 
			{
				
			}
		}
		
		StartCoroutine(SendDungeonBeginRequest());
	}
	
	private IEnumerator SendDungeonBeginRequest()
	{
		BeginDungeonRequestProto request = new BeginDungeonRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.clientTime = CBKUtil.timeNowMillis;
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
			CBKWhiteboard.currTaskID = response.userTaskId;
			
			PZCombatManager.instance.enemies.Clear();
			
			PZMonster monster;
			List<string> goonsToLoad = new List<string>();
			foreach (TaskStageProto stage in response.tsp)
			{
				monster = new PZMonster(stage.stageMonsters[0]);
				PZCombatManager.instance.enemies.Enqueue(monster);
				goonsToLoad.Add(monster.monster.imagePrefix);
				Debug.Log("Stage: " + stage.stageId + ": Adding monster " + stage.stageMonsters[0].monsterId);
			}
			
			foreach (var item in CBKMonsterManager.instance.userTeam) 
			{
				if (item != null && item.monster != null && item.monster.monsterId > 0)
				{
					goonsToLoad.Add(item.monster.imagePrefix);
				}
			}
			
			CBKAtlasUtil.instance.LoadAtlasesForSpriteNames(goonsToLoad);
		}
		
		CBKEventManager.Scene.OnPuzzle();
	}
}
