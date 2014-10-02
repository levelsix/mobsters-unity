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
		//Debug.Log("Hurr");

		tweenAlph.PlayReverse();
		MoveButtons();
		
		if (building != null && building != MSBuildingManager.instance.hoveringToBuild)
		{
			bottomChat.Hide ();
			StartCoroutine(TweenWhenOffScreen(building));
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
	
	void AddButton(MSTaskButton.Mode mode)
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
		//Debug.Log("Durr?");

		currBuilding = building;

		if (currBuilding == MSBuildingManager.instance.hoveringToBuild)
		{
			return;
		}

		if (currBuilding.locallyOwned)
		{
			if (!currBuilding.userStructProto.isComplete)
			{
				AddButton(MSTaskButton.Mode.FINISH);
			}
			else
			{
				if (currBuilding.combinedProto.structInfo.level == 0)
				{
					AddButton(MSTaskButton.Mode.FIX);
				}
				else if (currBuilding.combinedProto.structInfo.successorStructId > 0)
				{
					AddButton(MSTaskButton.Mode.UPGRADE);
				}
				if (currBuilding.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.RESIDENCE)
				{
					AddButton(MSTaskButton.Mode.HIRE);
					AddButton(MSTaskButton.Mode.SELL_MOBSTERS);
				}
				else if (currBuilding.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.MINI_JOB
				         && currBuilding.combinedProto.structInfo.level > 0)
				{
					AddButton(MSTaskButton.Mode.MINIJOB);
				}
				else if (currBuilding.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.TEAM_CENTER)
				{
					AddButton(MSTaskButton.Mode.TEAM);
				}
				else if (currBuilding.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.LAB
				         && currBuilding.combinedProto.structInfo.level > 0)
				{
					AddButton(MSTaskButton.Mode.ENHANCE);
				}
				else if (currBuilding.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.EVO)
				{
					AddButton(MSTaskButton.Mode.EVOLVE);
				}
				if (currBuilding.hospital != null)
				{
					AddButton(MSTaskButton.Mode.HEAL);
				}
			}


		}
		else if (currBuilding.obstacle != null)
		{
			if (currBuilding.obstacle.secsLeft > 0)
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
