using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using System.Collections.Generic;
using System;

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
			CBKEventManager.Popup.CreateButtonPopup("Uh oh, you have no mobsters on your team. Manage your team?",
                new string[]{"Later", "Manage"},
                new Action[]{delegate{CBKEventManager.Popup.CloseTopPopupLayer();},
					delegate{CBKEventManager.Popup.CloseAllPopups(); CBKEventManager.Popup.OnPopup(CBKPopupManager.instance.goonManagePopup);
						CBKPopupManager.instance.goonManagePopup.GetComponent<CBKGoonScreen>().InitHeal();}},
				true);
			return;
		}
		else if (CBKMonsterManager.userMonsters.Count > CBKMonsterManager.totalResidenceSlots)
		{
			CBKEventManager.Popup.CreateButtonPopup("Uh oh, you have recruited too many mobsters. Manage your team?",
			                                        new string[]{"Later", "Manage"},
			new Action[]{delegate{CBKEventManager.Popup.CloseTopPopupLayer();},
				delegate{CBKEventManager.Popup.CloseAllPopups(); CBKEventManager.Popup.OnPopup(CBKPopupManager.instance.goonManagePopup);
					CBKPopupManager.instance.goonManagePopup.GetComponent<CBKGoonScreen>().InitHeal();}}, true);
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

		StartCoroutine(BeginDungeonRequest());
	}

	IEnumerator BeginDungeonRequest()
	{
		BeginDungeonRequestProto request = new BeginDungeonRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.clientTime = CBKUtil.timeNowMillis;
		request.taskId = task.taskId;
		
		CBKWhiteboard.currSceneType = CBKWhiteboard.SceneType.PUZZLE;
		CBKWhiteboard.dungeonToLoad = request;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_DUNGEON_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		CBKWhiteboard.loadedDungeon = UMQNetworkManager.responseDict[tagNum] as BeginDungeonResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		PZCombatManager.instance.Init();
		PZPuzzleManager.instance.InitBoard();

		CBKEventManager.Scene.OnPuzzle();
	}
}
