using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// Bar along the bottom of the screen that displays pertinent information.
/// Has different modes depending on the current state of the game (CHAT, BUILDING, QUEST)
/// </summary>
public class CBKTaskBar : MonoBehaviour {

	enum TaskBarMode {CHAT, BUILDING, QUEST};
	
	TaskBarMode mode = TaskBarMode.CHAT;
	
	[SerializeField]
	List<CBKTaskButton> taskButtons;
	
	[SerializeField]
	CBKTaskButton taskButtonPrefab;
	
	[SerializeField]
	UILabel topText;
	
	[SerializeField]
	UILabel bottomText;
	
	[SerializeField]
	Transform taskButtonParent;
	
	[SerializeField]
	GameObject upgradePopup;
	
	CBKBuilding currBuilding;
	
	CBKUnit currUnit;
	
	const int BUTTON_WIDTH = 75;
	
	const int BUTTON_HEIGHT = 75;
	
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
	
	void OnUnitSelect(CBKUnit unit)
	{
		
	}
	
	void OnBuildingSelect(CBKBuilding building)
	{
		ClearButtons();


		
		if (currBuilding != null)
		{
			currBuilding.OnUpdateValues -= UpdateBuildingText;
		}
		
		if (building != null)
		{
			SetBuildingButtons(building);
		}
		else
		{
			//Close
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
	
	void AddButton(CBKTaskButton.Mode mode)
	{
		CBKTaskButton button = MSPoolManager.instance.Get(taskButtonPrefab, Vector3.zero) as CBKTaskButton;
		if (currBuilding != null)
		{
			if (mode == CBKTaskButton.Mode.UPGRADE || mode == CBKTaskButton.Mode.FINISH)
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
		foreach (CBKTaskButton item in taskButtons) 
		{
			item.trans.parent = taskButtonParent;
			item.trans.localScale = Vector3.one;
		}
		switch(taskButtons.Count)
		{
			case 3:
				taskButtons[0].trans.localPosition = new Vector3(-1.5f * BUTTON_WIDTH, BUTTON_HEIGHT);
				taskButtons[1].trans.localPosition = new Vector3(0, BUTTON_HEIGHT);
				taskButtons[2].trans.localPosition = new Vector3(1.5f * BUTTON_WIDTH, BUTTON_HEIGHT);
				break;
			case 2:
				taskButtons[0].trans.localPosition = new Vector3(-BUTTON_WIDTH, BUTTON_HEIGHT);
				taskButtons[1].trans.localPosition = new Vector3(BUTTON_WIDTH, BUTTON_HEIGHT);
				break;
			case 1:
				taskButtons[0].trans.localPosition = new Vector3(0, BUTTON_HEIGHT);
				break;
			default:
				break;
		}
	}
	
	void SetBuildingButtons(CBKBuilding building)
	{
		currBuilding = building;
		
		if (currBuilding.locallyOwned)
		{
			if (!currBuilding.userStructProto.isComplete)
			{
				AddButton(CBKTaskButton.Mode.FINISH);
			}
			else if (currBuilding.combinedProto.structInfo.successorStructId > 0)
			{
				AddButton(CBKTaskButton.Mode.UPGRADE);
			}
			AddButton(CBKTaskButton.Mode.SELL);
		}
		else
		{
			AddButton(CBKTaskButton.Mode.ENGAGE);
		}
		
		SortButtons();
		
		UpdateBuildingText();
		currBuilding.OnUpdateValues += UpdateBuildingText;
		
		topText.text = building.name;
	}
	
	void UpdateBuildingText()
	{
		if (currBuilding.userStructProto == null)
		{
			//TODO: Mission map building details
		}
		else if (!currBuilding.userStructProto.isComplete)
		{
			bottomText.text = "Upgrade completes in " + currBuilding.upgrade.timeLeftString;
		}
	}
}
