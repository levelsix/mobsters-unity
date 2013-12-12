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

		if (proto.prerequisiteTaskId > 0 && !CBKQuestManager.taskDict.ContainsKey(proto.prerequisiteTaskId))
		{
			CBKBuilding building = GetComponent<CBKBuilding>();
			if (building != null)
			{
				building.SetLocked();
			}
		}
	}
	
	public void EngageTask()
	{
		if (CBKMonsterManager.monstersOnTeam == 0)
		{
			Debug.Log("No monsters on team!");
			return;
		}
		else
		{
			int i;
			for (i = 0; i < CBKMonsterManager.userTeam.Length; i++) 
			{
				if (CBKMonsterManager.userTeam[i] != null && CBKMonsterManager.userTeam[i].currHP > 0)
				{
					break;
				}
			}
			if (i == CBKMonsterManager.userTeam.Length)
			{
				Debug.Log("No monsters on team have health!");
			}
		}
		
		BeginDungeonRequestProto request = new BeginDungeonRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.clientTime = CBKUtil.timeNowMillis;
		request.taskId = task.taskId;
		
		CBKWhiteboard.currSceneType = CBKWhiteboard.SceneType.PUZZLE;
		CBKWhiteboard.dungeonToLoad = request;
		CBKValues.Scene.ChangeScene(CBKValues.Scene.Scenes.LOADING_SCENE);
		//StartCoroutine(SendDungeonBeginRequest());
	}
}
