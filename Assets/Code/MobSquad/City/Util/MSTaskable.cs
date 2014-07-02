using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using System.Collections.Generic;
using System;

public class MSTaskable : MonoBehaviour {

	public FullTaskProto task;
	
	public MinimumUserTaskProto userTask;

	[SerializeField]
	bool isEvent = false;

	int persistentEventId = 0;
	
	public void Init(FullTaskProto proto, int eventId = 0)
	{
		task = proto;

		persistentEventId = eventId;

		//DetermineHoverIcon (); No longer in Cities
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
			MSBuilding building = GetComponent<MSBuilding> ();
			MSCityUnit unit = GetComponent<MSCityUnit>();
			if (!MSQuestManager.instance.taskDict.ContainsKey (task.prerequisiteTaskId)) 
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
				if (!MSQuestManager.instance.taskDict.ContainsKey (task.taskId)) 
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
		if (MSMonsterManager.monstersOwned == 0)
		{
			MSPopupManager.instance.CreatePopup("No Mobsters!",
				"Uh oh, you have no mobsters on your team. Manage your team?",
                new string[]{"Later", "Manage"},
				new string[]{"greymenuoption", "greenmenuoption"},
				new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
					delegate{MSActionManager.Popup.CloseAllPopups();
						MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonScreen.gameObject.GetComponent<MSPopup>());
						MSPopupManager.instance.popups.goonScreen.Init(GoonScreenMode.HEAL);}}
			);
			return;
		}
		else if (MSMonsterManager.instance.userMonsters.Count > MSMonsterManager.instance.totalResidenceSlots)
		{
			MSPopupManager.instance.CreatePopup("Residences Full!",
				"Uh oh, you have recruited too many mobsters. Manage your team?",
                new string[]{"Later", "Manage"},
				new string[]{"greymenuoption", "greenmenuoption"},
				new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
					delegate{MSActionManager.Popup.CloseAllPopups();
						MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonScreen.gameObject.GetComponent<MSPopup>());
						MSPopupManager.instance.popups.goonScreen.Init(GoonScreenMode.HEAL);}}
			);
			return;
		}
		else
		{
			int i;
			for (i = 0; i < MSMonsterManager.instance.userTeam.Length; i++) 
			{
				if (MSMonsterManager.instance.userTeam[i] != null && MSMonsterManager.instance.userTeam[i].currHP > 0)
				{
					break;
				}
			}
			if (i == MSMonsterManager.instance.userTeam.Length)
			{
				MSPopupManager.instance.CreatePopup("No Mobsters!",
					"No monsters on team have health! Manage your team?",
                	new string[]{"Later", "Manage"},
					new string[]{"greymenuoption", "greenmenuoption"},
					new Action[]{delegate{MSActionManager.Popup.CloseTopPopupLayer();},
						delegate{MSActionManager.Popup.CloseAllPopups();
							MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonScreen.gameObject.GetComponent<MSPopup>());
							MSPopupManager.instance.popups.goonScreen.Init(GoonScreenMode.HEAL);}}
				);
				return;
			}
		}

		PZCombatManager.instance.StartCoroutine(BeginDungeonRequest());
	}

	private IEnumerator EnterDungeon (){
		foreach (KeyValuePair<long, MSUnit> monster in MSBuildingManager.instance.playerUnits) {
			if(GetComponent<MSBuilding>() != null){
				MSBuilding building = GetComponent<MSBuilding>();

				//move the monster to be infront of the door
				Vector2 doorPosition = new Vector2(building.groundPos.x + 1, building.groundPos.y - 4);
				monster.Value.transf.position = MSGridManager.instance.GridToWorld(doorPosition);

				//set the monster to move through the building
				Color newColor = new Color(monster.Value.sprite.color.r ,monster.Value.sprite.color.g, monster.Value.sprite.color.b, 0f);
				monster.Value.sprite.color = newColor;
				MSGridNode node = new MSGridNode(new Vector2(doorPosition.x, doorPosition.y + 4));
				node.direction = MSValues.Direction.NORTH;
				monster.Value.GetComponent<MSCityUnit>().SetTarget(node);

				//manually tween the alpha as they walk into the building
				StartCoroutine(TweenAlpha(0f,1f,0.2f,monster.Value));
				yield return new WaitForSeconds(0.2f);
				StartCoroutine(TweenAlpha(1f,0f,0.8f,monster.Value));
			}else{
				//else enter a person (oh baby)
			}
			yield return new WaitForSeconds(1f);
		}

	}

	//This system assumes that the user is trying to fade all the way from 0 to 1 or vise versa
	private IEnumerator TweenAlpha(float from, float to, float fadeTime, MSUnit unit){

		Color newColor = new Color(unit.sprite.color.r ,unit.sprite.color.g, unit.sprite.color.b, from);
		unit.sprite.color = newColor;
		unit.shadow.color = newColor;

		float currTime = 0;
		float alpha;
		while (currTime <= fadeTime)
		{
			currTime += Time.deltaTime;
			if(to > from){
				alpha = (currTime / fadeTime);
			}else{
				alpha = 1 - (currTime / fadeTime);
			}
			newColor = new Color(unit.sprite.color.r ,unit.sprite.color.g, unit.sprite.color.b, alpha);
			unit.sprite.color = newColor;
			unit.shadow.color = newColor;
			yield return new WaitForEndOfFrame();
		}

		if (to > from) {
			alpha = 1f;
		} else {
			alpha = 0f;
		}

		newColor = new Color(unit.sprite.color.r ,unit.sprite.color.g, unit.sprite.color.b, alpha);
		unit.sprite.color = newColor;
		unit.shadow.color = newColor;
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
		
		yield return StartCoroutine (EnterDungeon ());

		MSActionManager.Scene.OnPuzzle();
		
		PZCombatManager.instance.PreInitTask();

		PZPuzzleManager.instance.InitBoard();

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		MSWhiteboard.loadedDungeon = UMQNetworkManager.responseDict[tagNum] as BeginDungeonResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		PZCombatManager.instance.InitTask();

	}
}
