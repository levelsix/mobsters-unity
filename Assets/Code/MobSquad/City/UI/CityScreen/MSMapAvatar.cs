using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSMapAvatar : MonoBehaviour {
	[SerializeField]
	MSChatAvatar chatAvatar;

	[SerializeField]
	UITexture faceAvatar;

	int curTask = -1;

	void OnEnable()
	{
		if(FB.IsLoggedIn)
		{
			faceAvatar.gameObject.SetActive(true);
			chatAvatar.gameObject.SetActive(false);

			MSFacebookManager.instance.RunLoadPhotoForUser(FB.UserId.ToString(),faceAvatar);
		}
		else
		{
			faceAvatar.gameObject.SetActive(false);
			chatAvatar.gameObject.SetActive(true);
			chatAvatar.Init(MSWhiteboard.localUser.avatarMonsterId);
		}

	}

	public void SetDepth(int depth)
	{
		UISprite sprite = GetComponent<UISprite>();
		sprite.depth = depth + 2;
		chatAvatar.SetDepth(depth);
		faceAvatar.depth = depth + 1;
	}

	public void InitPosition(int taskId)
	{
		curTask = taskId;
		TaskMapElementProto task = MSDataManager.instance.Get<TaskMapElementProto>(taskId);
		transform.localPosition = new Vector3(task.xPos, task.yPos, 0f);
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
