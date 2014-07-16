using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSTaskMap : MonoBehaviour {
	
	[SerializeField]
	MSSimplePoolable mapTaskButton;

	[SerializeField]
	MSSegmentedMap maps;

	[SerializeField]
	Transform TaskParent;

	[SerializeField]
	UI2DSprite pvpHud;

	public Dictionary<int, MSMapTaskButton> taskButtons = new Dictionary<int, MSMapTaskButton>();
	
	Transform trans;
	
	/// <summary>
	/// We have to scale the taskParent to get these coordinates to line up on the map
	/// </summary>
	const float SCALE_TO_FIT_X = 2f;
	const float SCALE_TO_FIT_Y = 2f;
		
	void Awake(){
		trans = transform;

		float width = MSMath.uiScreenWidth;
		float scale = (width - pvpHud.width) / maps.maps[0].width;
		trans.localScale = new Vector3(scale, scale, scale);

		UI2DSprite map = maps.maps [0];
		TaskParent.localScale = new Vector3(SCALE_TO_FIT_X, SCALE_TO_FIT_Y, 1f);
		TaskParent.localPosition = new Vector3 (-map.width, -map.height/2f, 0f);
		
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
		GetComponent<MSDragDropLimited> ().min = new Vector2 (0f, -(mapLength - map.height) + (mapLength * (1f - trans.localScale.y)));
		GetComponent<MSDragDropLimited> ().max = new Vector2 (0f, -(map.height / 2f) * (1f - trans.localScale.y));
		
		BoxCollider box = GetComponent<BoxCollider> ();
		box.size = new Vector3 (map.width, mapLength, 0f);
		box.center = new Vector3(-map.width/2f,  (mapLength / 2f) - map.height/2f, 0f);
	}

	void Start(){

	}

	void OnEnable(){
		IDictionary tasks = MSDataManager.instance.GetAll<TaskMapElementProto> ();
				
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
			}
			else
			{
				taskButtons[task.taskId].Status = MSMapTaskButton.TaskStatusType.Locked;
			}
		}
	}
}
