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
	GameObject bigChatPopup;
	
	CBKBuilding currBuilding;
	
	CBKUnit currUnit;
	
	const int BUTTON_WIDTH = 75;
	
	const int BUTTON_HEIGHT = 75;
	
	void Start()
	{
		SetChatMode();
	}
	
	void OnEnable()
	{
		CBKEventManager.Town.OnBuildingSelect += OnBuildingSelect;
		CBKEventManager.Town.OnUnitSelect += OnUnitSelect;
	}
	
	void OnDisable()
	{
		CBKEventManager.Town.OnBuildingSelect -= OnBuildingSelect;
		CBKEventManager.Town.OnUnitSelect -= OnUnitSelect;
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
			SetChatMode();
		}
	}
	
	void OnClickBar()
	{
		if (mode == TaskBarMode.CHAT)
		{
			CBKEventManager.Popup.OnPopup(bigChatPopup);
		}
	}
	
	void SetChatMode()
	{
		mode = TaskBarMode.CHAT;
		
		topText.text = "One message goes here";
		bottomText.text = "Another message goes here";
		
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
		CBKTaskButton button = CBKPoolManager.instance.Get(taskButtonPrefab, Vector3.zero) as CBKTaskButton;
		if (currBuilding != null)
		{
			button.Setup(mode, currBuilding);
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
			if (currBuilding.upgrade.timeRemaining <= 0)
			{
				AddButton(CBKTaskButton.Mode.FINISH);
			}
			else if (currBuilding.combinedProto.structInfo.predecessorStructId > 0)
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
		
		mode = TaskBarMode.BUILDING;
		
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
