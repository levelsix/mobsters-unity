using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// Bar along the bottom of the screen that displays pertinent information.
/// Has different modes depending on the current state of the game (CHAT, BUILDING, QUEST)
/// </summary>
public class MSTaskBar : MonoBehaviour {

	public static MSTaskBar instance;

	public List<MSTaskButton> taskButtons;
	
	[SerializeField]
	MSTaskButton taskButtonPrefab;
	
	[SerializeField]
	UILabel topText;

	[SerializeField]
	GameObject layoutSmall;

	[SerializeField]
	UIGrid taskButtonGrid;
	
	[SerializeField]
	GameObject upgradePopup;

	[SerializeField]
	GameObject hirePopup;

	[SerializeField]
	GameObject minijobPopup;

	[SerializeField]
	MSBottomChat bottomChat;

	[SerializeField]
	TweenAlpha tweenAlph;
	
	MSBuilding currBuilding;
	
	MSUnit currUnit;

	public bool manageTeamNeedsArrow = false;
	
	const int BUTTON_WIDTH = 75;
	
	const int BUTTON_HEIGHT = 100;

	bool wide
	{
		get
		{
			float ratio = ((float)Screen.width)/Screen.height;
			return ratio > 1.59f;
		}
	}

	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		MSActionManager.Town.OnBuildingSelect += OnBuildingSelect;
	}
	
	void OnDisable()
	{
		MSActionManager.Town.OnBuildingSelect -= OnBuildingSelect;
	}
	
	void OnBuildingSelect(MSBuilding building)
	{
		currBuilding = building;

		tweenAlph.PlayReverse();
		MoveButtons();
		
		if (building != null && building != MSBuildingManager.instance.hoveringToBuild)
		{
			bottomChat.Hide ();
			//StartCoroutine(TweenWhenOffScreen(building));
		}
		else
		{
			bottomChat.Unhide();
			taskButtons.Clear();
		}
	}
	
	void MoveButtons()
	{
		foreach (var item in taskButtons) 
		{
			item.Pool();
		}
	}
	
	MSTaskButton AddButton(MSTaskButton.Mode mode)
	{
		MSTaskButton button = MSPoolManager.instance.Get(taskButtonPrefab, Vector3.zero) as MSTaskButton;
		if (currBuilding != null)
		{
			switch (mode)
			{
			case MSTaskButton.Mode.FIX:
			case MSTaskButton.Mode.UPGRADE:
			case MSTaskButton.Mode.FINISH:
				button.Setup(mode, currBuilding, upgradePopup);
				break;
			case MSTaskButton.Mode.HIRE:
				button.Setup(mode, currBuilding, hirePopup);
				break;
			case MSTaskButton.Mode.MINIJOB:
				button.Setup(mode, currBuilding, minijobPopup);
				break;
			default:
				button.Setup(mode, currBuilding);
				break;
			}
		}
		else
		{
			button.Setup(mode, currUnit);
		}
		taskButtons.Add(button);

		return button;
	}
	
	void SortButtons()
	{
		foreach (MSTaskButton item in taskButtons) 
		{
			item.trans.parent = taskButtonGrid.transform;
			item.trans.localScale = Vector3.one;
		}

		taskButtonGrid.Reposition();

		for (int i = 0; i < taskButtons.Count; i++) 
		{
			taskButtons[i].Enter(i);
		}
	}
	
	void SetBuildingButtons(MSBuilding building)
	{

		if (building == MSBuildingManager.instance.hoveringToBuild)
		{
			return;
		}

		if (building.locallyOwned)
		{
			if (!building.userStructProto.isComplete)
			{
				AddButton(MSTaskButton.Mode.FINISH);
			}
			else
			{
				if (building.combinedProto.structInfo.level == 0)
				{
					AddButton(MSTaskButton.Mode.FIX);
				}
				else if (building.combinedProto.structInfo.successorStructId > 0)
				{
					AddButton(MSTaskButton.Mode.UPGRADE);
				} 
				if (building.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.MINI_JOB
				         && building.combinedProto.structInfo.level > 0)
				{
					AddButton(MSTaskButton.Mode.MINIJOB);
				}
				else if (building.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.LAB
				         && building.combinedProto.structInfo.level > 0)
				{
					AddButton(MSTaskButton.Mode.ENHANCE);
				}
				else if (building.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.EVO)
				{
					AddButton(MSTaskButton.Mode.EVOLVE);
				}
				if (building.hospital != null)
				{
					AddButton(MSTaskButton.Mode.HEAL);
				}
			}
			if (building.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.RESIDENCE)
			{
				AddButton(MSTaskButton.Mode.HIRE);
				AddButton(MSTaskButton.Mode.SELL_MOBSTERS);
			}
			else if (building.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.TEAM_CENTER)
			{
				MSTaskButton button = AddButton(MSTaskButton.Mode.TEAM);
				if(manageTeamNeedsArrow)
				{
					MSTutorialArrow.instance.Init(button.trans, 150, MSValues.Direction.EAST);
					button.hintArrow = MSTutorialArrow.instance;
					manageTeamNeedsArrow = false;
				}
			}
			else if (building.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.CLAN
			         && building.combinedProto.structInfo.level > 0)
			{
				AddButton(MSTaskButton.Mode.SQUAD);
			}

		}
		else if (building.obstacle != null)
		{
			if (building.obstacle.secsLeft > 0)
			{
				AddButton(MSTaskButton.Mode.FINISH);
			}
			else
			{
				AddButton(MSTaskButton.Mode.REMOVE_OBSTACLE);
			}
		}
		
		SortButtons();

		if (building.combinedProto != null)
		{
			topText.text = building.name;
		}
		else if (building.obstacle != null)
		{
			topText.text = building.obstacle.obstacle.name;
		}
		else if (building.taskable != null)
		{
			topText.text = building.taskable.task.name;
		}
	}

	void Update()
	{
		CheckTweenIn ();
	}

	void CheckTweenIn ()
	{
		if (currBuilding != null) 
		{
			foreach (var item in taskButtons) 
			{
				if (item.gameObj.activeSelf) return;
			}

			taskButtons.Clear();
			SetBuildingButtons(currBuilding);
			tweenAlph.PlayForward();

			currBuilding = null;
		}
	}

	IEnumerator TweenWhenOffScreen(MSBuilding building)
	{
		//Debug.Log("Da");
		foreach (var item in taskButtons) 
		{
			while (item.tweeningOut)
			{
				yield return null;
			}
		}
		while (tweenAlph.tweenFactor > 0)
		{
			yield return null;
		}
		taskButtons.Clear();
		SetBuildingButtons(building);
		tweenAlph.PlayForward();
	}
}
