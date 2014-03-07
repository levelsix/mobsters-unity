using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using System.Collections.Generic;
using System;

public class CBKTaskable : MonoBehaviour {

	public FullTaskProto task;
	
	public MinimumUserTaskProto userTask;

	[SerializeField]
	bool isEvent = false;

	int persistentEventId = 0;
	
	public void Init(FullTaskProto proto, int eventId = 0)
	{
		task = proto;

		persistentEventId = eventId;

		DetermineHoverIcon ();
	}

	void OnEnable()
	{
		DetermineHoverIcon();
	}

	/// <summary>
	/// Determines whether Hover Icon should be set up as a lock or arrow, if at all.
	/// </summary>
	void DetermineHoverIcon ()
	{
		if (task != null && task.prerequisiteTaskId > 0) 
		{
			CBKBuilding building = GetComponent<CBKBuilding> ();
			CBKCityUnit unit = GetComponent<CBKCityUnit>();
			if (!MSQuestManager.taskDict.ContainsKey (task.prerequisiteTaskId)) 
			{
				if (building != null) 
				{
					building.SetLocked();
				}
				if (unit != null)
				{
					unit.SetLocked();
				}
			}
			else
			{
				if (building != null)
				{
					building.SetUnlocked();
				}
				if (unit != null)
				{
					unit.SetUnlocked();
				}
				if (!MSQuestManager.taskDict.ContainsKey (task.taskId)) 
				{
					if (building != null) 
					{
						building.SetArrow();
					}
					if (unit != null)
					{
						unit.SetArrow();
					}
				}
			}
		}
	}
	
	public void EngageTask()
	{
		if (MSMonsterManager.monstersOnTeam == 0)
		{
			MSActionManager.Popup.CreateButtonPopup("Uh oh, you have no mobsters on your team. Manage your team?",
                new string[]{"Later", "Manage"},
                new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
					delegate{MSActionManager.Popup.CloseAllPopups(); MSActionManager.Popup.OnPopup(CBKGoonScreen.instance.gameObject);
						CBKGoonScreen.instance.InitHeal();}}
				);
			return;
		}
		else if (MSMonsterManager.instance.userMonsters.Count > MSMonsterManager.totalResidenceSlots)
		{
			MSActionManager.Popup.CreateButtonPopup("Uh oh, you have recruited too many mobsters. Manage your team?",
			                                        new string[]{"Later", "Manage"},
			new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
				delegate{MSActionManager.Popup.CloseAllPopups(); MSActionManager.Popup.OnPopup(CBKGoonScreen.instance.gameObject);
					CBKGoonScreen.instance.InitHeal();}});
			return;
		}
		else
		{
			int i;
			for (i = 0; i < MSMonsterManager.userTeam.Length; i++) 
			{
				if (MSMonsterManager.userTeam[i] != null && MSMonsterManager.userTeam[i].currHP > 0)
				{
					break;
				}
			}
			if (i == MSMonsterManager.userTeam.Length)
			{
				MSActionManager.Popup.CreateButtonPopup("No monsters on team have health! Manage your team?",
				                                        new string[]{"Later", "Manage"},
				new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
					delegate{MSActionManager.Popup.CloseAllPopups(); MSActionManager.Popup.OnPopup(CBKGoonScreen.instance.gameObject);
						CBKGoonScreen.instance.InitHeal();}});
				return;
			}
		}

		StartCoroutine(BeginDungeonRequest());
	}

	IEnumerator BeginDungeonRequest()
	{
		BeginDungeonRequestProto request = new BeginDungeonRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clientTime = MSUtil.timeNowMillis;
		request.taskId = task.taskId;

		request.isEvent = isEvent;
		request.persistentEventId = persistentEventId;
		
		MSWhiteboard.currSceneType = MSWhiteboard.SceneType.PUZZLE;
		MSWhiteboard.dungeonToLoad = request;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_DUNGEON_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		MSWhiteboard.loadedDungeon = UMQNetworkManager.responseDict[tagNum] as BeginDungeonResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		PZCombatManager.instance.InitTask();
		PZPuzzleManager.instance.InitBoard();

		MSActionManager.Scene.OnPuzzle();
	}
}
