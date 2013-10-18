using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Bar along the bottom of the screen that displays pertinent information.
/// Has different modes depending on the current state of the game (CHAT, BUILDING, QUEST)
/// </summary>
public class CBKTaskBar : MonoBehaviour {

	enum TaskBarMode {CHAT, BUILDING, QUEST};
	
	TaskBarMode mode = TaskBarMode.CHAT;
	
	[SerializeField]
	CBKActionButton taskBar;
	
	[SerializeField]
	UILabel topText;
	
	[SerializeField]
	UILabel bottomText;
	
	[SerializeField]
	CBKTaskButton button;
	
	[SerializeField]
	GameObject bigChatPopup;
	
	CBKBuilding currBuilding;
	
	void Start()
	{
		SetChatMode();
	}
	
	void OnEnable()
	{
		CBKEventManager.Town.OnBuildingSelect += OnBuildingSelect;
		taskBar.onClick += OnClickBar;
	}
	
	void OnDisable()
	{
		CBKEventManager.Town.OnBuildingSelect -= OnBuildingSelect;
		taskBar.onClick -= OnClickBar;
	}
	
	void OnBuildingSelect(CBKBuilding building)
	{
		if (currBuilding != null)
		{
			currBuilding.OnUpdateValues -= UpdateBuildingText;
		}
		
		if (building != null)
		{
			SetBuildingMode (building);
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
		button.gameObject.SetActive(false);
	}
	
	void SetBuildingMode(CBKBuilding building)
	{
		mode = TaskBarMode.BUILDING;
		
		currBuilding = building;
		
		UpdateBuildingText();
		currBuilding.OnUpdateValues += UpdateBuildingText;
		
		button.gameObject.SetActive(true);
		button.SetupBuilding(building);
		topText.text = building.name;
	}
	
	void UpdateBuildingText()
	{
		if (currBuilding.userStructProto.isComplete)
		{
			if (currBuilding.collector.secondsUntilComplete > 0)
			{
				bottomText.text = "Collects in " + currBuilding.collector.timeLeftString;
			}
			else
			{
				bottomText.text = "Resources Available!";
			}
		}
		else
		{
			bottomText.text = "Upgrade completes in " + currBuilding.upgrade.timeLeftString;
		}
	}
}
