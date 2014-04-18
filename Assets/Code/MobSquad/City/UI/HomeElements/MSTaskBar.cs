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
	UILabel topText;
	
	[SerializeField]
	UILabel bottomText;
	
	[SerializeField]
	Transform taskButtonParent;
	
	[SerializeField]
	GameObject upgradePopup;

	TweenPosition tweenPos;
	TweenAlpha tweenAlph;
	
	MSBuilding currBuilding;
	
	MSUnit currUnit;
	
	const int BUTTON_WIDTH = 75;
	
	const int BUTTON_HEIGHT = 100;

	void Awake()
	{
		tweenPos = GetComponent<TweenPosition>();
		tweenAlph = GetComponent<TweenAlpha>();
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
		ClearButtons();

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
			if (mode == MSTaskButton.Mode.UPGRADE || mode == MSTaskButton.Mode.FINISH)
			{
				button.Setup(mode, currBuilding, upgradePopup);
			}
			else
			{
				button.Setup(mode, currBuilding);
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
			item.trans.parent = taskButtonParent;
			item.trans.localScale = Vector3.one;
		}
		switch(taskButtons.Count)
		{
			case 3:
				taskButtons[0].trans.localPosition = new Vector3(-1.5f * BUTTON_WIDTH, 0);
				taskButtons[1].trans.localPosition = new Vector3(0, 0);
				taskButtons[2].trans.localPosition = new Vector3(1.5f * BUTTON_WIDTH, 0);
				break;
			case 2:
				taskButtons[0].trans.localPosition = new Vector3(-BUTTON_WIDTH, 0);
				taskButtons[1].trans.localPosition = new Vector3(BUTTON_WIDTH, 0);
				break;
			case 1:
				taskButtons[0].trans.localPosition = new Vector3(0, 0);
				break;
			default:
				break;
		}
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
			else if (currBuilding.combinedProto.structInfo.successorStructId > 0)
			{
				AddButton(MSTaskButton.Mode.UPGRADE);
			}

			if (currBuilding.hospital != null)
			{
				AddButton(MSTaskButton.Mode.HEAL);
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
		SetBuildingButtons(building);
		tweenPos.PlayForward();
		tweenAlph.PlayForward();
	}
}
