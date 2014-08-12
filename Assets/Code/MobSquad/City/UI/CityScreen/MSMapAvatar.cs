using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSMapAvatar : MonoBehaviour {



	[SerializeField]
	MSChatAvatar chatAvatar;

	[SerializeField]
	MSChatAvatar faceAvatar;

	int curTask = -1;

	void OnEnable()
	{
		//TODO: check faceBook for an image
		//else{
		faceAvatar.gameObject.SetActive(false);
		chatAvatar.gameObject.SetActive(true);
		//TODO: change the avatar to players avatar
	}

	public void SetDepth(int depth)
	{
		UISprite sprite = GetComponent<UISprite>();
		sprite.depth = depth+2;
		chatAvatar.SetDepth(depth);
		faceAvatar.SetDepth(depth);
	}

	public void InitPosition(int taskId)
	{
		curTask = taskId;
		TaskMapElementProto task = MSDataManager.instance.Get<TaskMapElementProto>(taskId);
		transform.localPosition = new Vector3(task.xPos, task.yPos, 0f);
		Debug.Log("Init avatar:"+taskId);
	}

	public void MoveToNewTask(int newTaskId)
	{
		if(curTask == -1)
		{
			InitPosition(newTaskId);
		}
		else if(curTask != newTaskId)
		{
			TaskMapElementProto nextTask = MSDataManager.instance.Get<TaskMapElementProto>(newTaskId);
			TweenPosition.Begin(gameObject, 2f, new Vector3(nextTask.xPos, nextTask.yPos, 0f));
		}
	}
}
