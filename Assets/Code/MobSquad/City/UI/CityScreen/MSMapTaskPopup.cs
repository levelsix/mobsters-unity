using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSMapTaskPopup : MonoBehaviour {
	[SerializeField]
	UILabel level;

	[SerializeField]
	UILabel status;

	[SerializeField]
	UILabel levelTitle;

	[SerializeField]
	UIButton button;

	[SerializeField]
	UISprite background;

	FullTaskProto task;

	const int LAST_TUTORIAL_LEVEL = 7;

	const string CANCEL_BUTTON = "cancel";

	const string ACCEPT_BUTTON = "acceptrequest";

	const string LOCKED_BACKGROUND = "lockeddailylab";
	
	const string FIRE_BACKGROUND = "firedailylab";
	
	const string EARTH_BACKGROUND = "earthdailylab";
	
	const string LIGHT_BACKGROUND = "lightdailylab";
	
	const string NIGHT_BACKGROUND = "nightdailylab";
	
	const string WATER_BACKGROUND = "waterdailylab";

	const string RAINBOW_BACKGROUND = "rainbowbar";

	[HideInInspector]
	public Transform trans;

	void Awake()
	{
		trans = transform;
	}

	public void init(TaskMapElementProto mapTask, MSMapTaskButton.TaskStatusType statusType)
	{
		task = MSDataManager.instance.Get<FullTaskProto> (mapTask.taskId);
		button.GetComponent<MSTaskable> ().task = task;

		switch (mapTask.element) {
		case Element.FIRE:
			background.spriteName = FIRE_BACKGROUND;
			break;
		case Element.EARTH:
			background.spriteName = EARTH_BACKGROUND;
			break;
		case Element.LIGHT:
			background.spriteName = LIGHT_BACKGROUND;
			break;
		case Element.DARK:
			background.spriteName = NIGHT_BACKGROUND;
			break;
		case Element.WATER:
			background.spriteName = WATER_BACKGROUND;
			break;
		default:
			background.spriteName = "";
			break;
		}

		if(mapTask.mapElementId <= LAST_TUTORIAL_LEVEL){
			background.spriteName = RAINBOW_BACKGROUND;
		}

		levelTitle.text = task.name;
		level.text = "Level " + mapTask.mapElementId;

		if (statusType == MSMapTaskButton.TaskStatusType.Completed) {
			button.SetState(UIButtonColor.State.Normal, true);
			status.text = "Completed";
		} else if(statusType == MSMapTaskButton.TaskStatusType.Undefeated){
			button.SetState(UIButtonColor.State.Normal, true);
			status.text = "Undefeated";
		} else {
			status.text = "Locked";
			background.spriteName = LOCKED_BACKGROUND;
			button.SetState(UIButtonColor.State.Disabled, true);
		}
	}
	
}
