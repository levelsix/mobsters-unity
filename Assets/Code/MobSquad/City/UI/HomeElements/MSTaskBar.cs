using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// Bar along the bottom of the screen that displays pertinent information.
/// Has different modes depending on the current state of the game (CHAT, BUILDING, QUEST)
/// </summary>
public class MSTaskBar : MonoBehaviour {
	
	[SerializeField]
	List<MSTaskButton> taskButtons;
	
	[SerializeField]
	MSTaskButton taskButtonPrefab;
	
	[SerializeField]
	UILabel topTextSmall;

	[SerializeField]
	UILabel topTextWide;

	UILabel topText
	{
		get
		{
			return wide ? topTextWide : topTextSmall;
		}
	}
	
	[SerializeField]
	UILabel bottomTextSmall;

	[SerializeField]
	UILabel bottomTextWide;

	UILabel bottomText
	{
		get
		{
			return wide ? bottomTextWide : bottomTextSmall;
		}
	}

	[SerializeField]
	GameObject layoutSmall;

	[SerializeField]
	GameObject layoutWide;

	[SerializeField]
	UIGrid taskButtonGridSmall;

	[SerializeField]
	UIGrid taskButtonGridWide;

	UIGrid taskButtonGrid
	{
		get
		{
			return wide ? taskButtonGridWide : taskButtonGridSmall;
		}
	}
	
	[SerializeField]
	GameObject upgradePopup;

	[SerializeField]
	GameObject hirePopup;

	[SerializeField]
	GameObject minijobPopup;

	TweenPosition tweenPos;
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
		tweenPos = GetComponent<TweenPosition>();
		tweenAlph = GetComponent<TweenAlpha>();
	}

	void Start()
	{
		ResetLayoutAssignment();
	}

	[ContextMenu ("Reset Layout")]
	void ResetLayoutAssignment()
	{
		layoutSmall.SetActive(!wide);
		layoutWide.SetActive(wide);
	}

	void OnEnable()
	{
		MSActionManager.Town.OnBuildingSelect += OnBuildingSelect;
		MSActionManager.Town.OnUnitSelect += OnUnitSelect;
	}
	
	void OnDisable()
	{
		MSActionManager.Town.OnBuildingSelect -= OnBuildingSelect;
		MSActionManager.Town.OnUnitSelect -= OnUnitSelect;
	}
	
	void OnUnitSelect(MSUnit unit)
	{
		if (tweenPos.tweenFactor > 0)
		{
			tweenPos.PlayReverse();
			tweenAlph.PlayReverse();
		}
	}
	
	void OnBuildingSelect(MSBuilding building)
	{

		if (currBuilding != null)
		{
			tweenPos.PlayReverse();
			tweenAlph.PlayReverse();
		}
		
		if (building != null)
		{
			StartCoroutine(TweenWhenOffScreen(building));
		}
		else
		{
			tweenPos.PlayReverse();
			tweenAlph.PlayReverse();
		}
	}
	
	void ClearButtons()
	{
		while(taskButtons.Count > 0)
		{
			taskButtons[0].Pool();
			taskButtons.RemoveAt(0);
		}
	}
	
	void AddButton(MSTaskButton.Mode mode)
	{
		MSTaskButton button = MSPoolManager.instance.Get(taskButtonPrefab, Vector3.zero) as MSTaskButton;
		if (currBuilding != null)
		{
			switch (mode)
			{
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
	}
	
	void SetBuildingButtons(MSBuilding building)
	{
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
				if (currBuilding.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.RESIDENCE)
				{
					AddButton(MSTaskButton.Mode.HIRE);
				}
				else if (currBuilding.combinedProto.structInfo.structType == com.lvl6.proto.StructureInfoProto.StructType.MINI_JOB
				         && currBuilding.combinedProto.structInfo.level > 0)
				{
					AddButton(MSTaskButton.Mode.MINIJOB);
				}
				if (currBuilding.hospital != null)
				{
					AddButton(MSTaskButton.Mode.HEAL);
				}
				if (currBuilding.combinedProto.structInfo.successorStructId > 0)
				{
					AddButton(MSTaskButton.Mode.UPGRADE);
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
				AddButton(MSTaskButton.Mode.REMOVE);
			}
		}
		else
		{
			AddButton(MSTaskButton.Mode.ENGAGE);
		}
		
		SortButtons();

		if (building.combinedProto != null)
		{
			topText.text = building.name;
			bottomText.text = building.combinedProto.structInfo.shortDescription;
		}
		else if (building.obstacle != null)
		{
			topText.text = building.obstacle.obstacle.name;
			bottomText.text = building.obstacle.obstacle.description;
		}
		else if (building.taskable != null)
		{
			topText.text = building.taskable.task.name;
			bottomText.text = building.taskable.task.description;
		}
	}

	IEnumerator TweenWhenOffScreen(MSBuilding building)
	{
		while (tweenPos.tweenFactor > 0 || tweenAlph.tweenFactor > 0)
		{
			yield return null;
		}
		ClearButtons();
		SetBuildingButtons(building);
		tweenPos.PlayForward();
		tweenAlph.PlayForward();
	}
}
