using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public class MSTaskMap : MonoBehaviour {
	
	[SerializeField]
	MSSimplePoolable mapTaskButton;

	[SerializeField]
	Transform TaskParent;
	
	Transform trans;

	/// <summary>
	/// We have to scale the taskParent to get these coordinates to line up on the map
	/// </summary>
	const float SCALE_TO_FIT = 2f;
		
	void Awake(){
		trans = transform;

		UI2DSprite map = GetComponent<UI2DSprite> ();
		TaskParent.localScale = Vector3.one * SCALE_TO_FIT;
		TaskParent.localPosition = new Vector3 (-map.width, -map.height/2f, 0f);

		IDictionary tasks = MSDataManager.instance.GetAll<TaskMapElementProto> ();
		foreach (TaskMapElementProto task in tasks.Values)
		{
			mapTaskButton.GetComponent<UISprite>().MakePixelPerfect();
			MSSimplePoolable taskPool = MSPoolManager.instance.Get(mapTaskButton, Vector3.zero, TaskParent) as MSSimplePoolable;
			taskPool.transform.localScale = Vector3.one / SCALE_TO_FIT;
			taskPool.GetComponent<UISprite>().depth = map.depth + 1;
			taskPool.GetComponent<MSMapTaskButton>().initTaskButton(task);
		}
	}
	
}
