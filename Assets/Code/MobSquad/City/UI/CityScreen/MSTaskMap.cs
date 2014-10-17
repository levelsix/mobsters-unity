using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSTaskMap : MonoBehaviour {

	Vector3 worldPosition;

	[SerializeField]
	UIScrollView scrollView;

	[SerializeField]
	MSMapTaskPopupSwitcher switcher;
	
	[SerializeField]
	MSSimplePoolable mapTaskButton;

	[SerializeField]
	MSSegmentedMap maps;

	/// <summary>
	/// Parent that holds all of the task buttons
	/// </summary>
	[SerializeField]
	Transform TaskParent;

	[SerializeField]
	UIWidget pvpHud;

	[SerializeField]
	MSMapAvatar avatar;

	/// <summary>
	/// The right half of the cityMap UI screen
	/// </summary>
	[SerializeField]
	Transform right;

	/// <summary>
	/// The furthest task the player has unlocked
	/// </summary>
	MSMapTaskButton nextTask;

	MSDragDropLimited limitedDrag;

	public Dictionary<int, MSMapTaskButton> taskButtons = new Dictionary<int, MSMapTaskButton>();
	
	Transform trans;

	/// <summary>
	/// We have to scale the taskParent to get these coordinates to line up on the map
	/// </summary>
	const float SCALE_TO_FIT_X = 2f;
	const float SCALE_TO_FIT_Y = 2f;

	//This is called from MapManager before Awake
	/// <summary>
	/// Begins initializing the Map
	/// </summary>
	public void StartInit()
	{
		trans = transform;
		limitedDrag = GetComponent<MSDragDropLimited>();

		//		//We need to set the parent to be in just the right place so the map slides in just right
		//		//We're also making the assumption here that the parent is infact mapParent
		Transform parent = trans.parent;
		parent.position = new Vector3(right.position.x, right.position.y, 0f);
		parent.localPosition = new Vector3(parent.localPosition.x + 700, parent.localPosition.y, parent.localPosition.z);
		
		float width = MSMath.uiScreenWidth;
		float scale = (width - pvpHud.width) / maps.maps[0].width;
		trans.localScale = new Vector3(scale, scale, scale);
		trans.localPosition = new Vector3(-(maps.maps[0].width * scale) / 2f, trans.localPosition.y, 0f);
		
		
		maps.LoadAllMaps(FinishInit);
	}

	//This is called after the map is finished being loaded
	void FinishInit()
	{
		UI2DSprite map = maps.maps [0];
		TaskParent.localScale = new Vector3(SCALE_TO_FIT_X, SCALE_TO_FIT_Y, 1f);
		TaskParent.localPosition = new Vector3 (-map.width/2f, 0f, 0f);
		
		IDictionary tasks = MSDataManager.instance.GetAll<TaskMapElementProto> ();
		
		foreach (TaskMapElementProto task in tasks.Values)
		{
			mapTaskButton.GetComponent<UISprite>().MakePixelPerfect();
			
			MSSimplePoolable taskPool = MSPoolManager.instance.Get(mapTaskButton, Vector3.zero, TaskParent) as MSSimplePoolable;
			taskPool.transform.localScale = new Vector3(1/SCALE_TO_FIT_X, 1/SCALE_TO_FIT_Y, 1f);
			taskPool.GetComponent<UISprite>().depth = map.depth + 2;
			taskPool.GetComponent<MSMapTaskButton>().initTaskButton(task);
			
			taskButtons.Add(task.taskId, taskPool.GetComponent<MSMapTaskButton>());
		}
		
		float mapLength = maps.Height;
		
		BoxCollider box = GetComponent<BoxCollider> ();
		box.size = new Vector3 (map.width, mapLength, 0f);
		box.center = new Vector3(0f,  (mapLength / 2f), 0f);
	}

	void OnEnable(){
		StartCoroutine(SetupTasks());
	}

	IEnumerator SetupTasks(){
		nextTask = null;

		IDictionary tasks = MSDataManager.instance.GetAll<TaskMapElementProto> ();
		
		yield return null;
		FullTaskProto taskProto;
		foreach (TaskMapElementProto task in tasks.Values){
			taskProto = MSDataManager.instance.Get<FullTaskProto>(task.taskId);
			if (MSQuestManager.instance.taskDict.ContainsKey (taskProto.taskId))
			{
				taskButtons[task.taskId].Status = MSMapTaskButton.TaskStatusType.Completed;
			}
			else if(taskProto.prerequisiteTaskId == 0 ||
				MSQuestManager.instance.taskDict.ContainsKey (taskProto.prerequisiteTaskId))
			{
				taskButtons[task.taskId].Status = MSMapTaskButton.TaskStatusType.Undefeated;
				nextTask = taskButtons[task.taskId];

				avatar.MoveToNewTask(nextTask.mapTask.mapElementId);
				avatar.SetDepth(nextTask.GetComponent<UISprite>().depth + 2);
			}
			else
			{
				taskButtons[task.taskId].Status = MSMapTaskButton.TaskStatusType.Locked;
			}
		}

		if(nextTask != null)
		{
			Vector3 newLocation = trans.position;
			newLocation.y = trans.position.y - nextTask.transform.position.y;
			trans.position = newLocation;
			scrollView.Scroll(1f);
		}
	}

	public void SelectNextTask()
	{
		if(nextTask != null)
		{
			nextTask.OnClick();
		}
	}

	public void OnClick()
	{
		switcher.activateEventPopup();
	}

}
