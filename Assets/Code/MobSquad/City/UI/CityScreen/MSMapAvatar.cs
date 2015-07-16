using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSMapAvatar : MonoBehaviour {

	[SerializeField]
	MSChatAvatar chatAvatar;

	[SerializeField]
	UI2DSprite faceAvatar;

	int curTask = -1;

	void OnEnable()
	{
		if (MSTangoManager.instance.HasProfilePhoto ()) {
			Debug.Log("setting tango profile photo for map");
			faceAvatar.gameObject.SetActive(true);
			chatAvatar.thumbnail.gameObject.SetActive(false);

			faceAvatar.sprite2D = MSTangoManager.instance.GetProfilePhoto();
			faceAvatar.width = faceAvatar.height = 60;
		}
//		else if(FB.IsLoggedIn)
//		{
//			faceAvatar.gameObject.SetActive(true);
//			chatAvatar.gameObject.SetActive(false);
//
//			MSFacebookManager.instance.RunLoadPhotoForUser(FB.UserId.ToString(),faceAvatar);
//		}
		else
		{
			faceAvatar.gameObject.SetActive(false);
			chatAvatar.thumbnail.gameObject.SetActive(true);
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

	public IEnumerator InitPosition(MSMapTaskButton task)
	{
		yield return null;
		curTask = task.mapTask.taskId;
		transform.position = task.transform.position;
	}

	public void MoveToNewTask(MSMapTaskButton newTask)
	{
		if(curTask == -1)
		{
			StartCoroutine(InitPosition(newTask));
		}
		else if(curTask != newTask.mapTask.taskId)
		{
			TweenPosition.Begin(gameObject, 2f, transform.parent.InverseTransformPoint(newTask.transform.position));
		}
	}
}
