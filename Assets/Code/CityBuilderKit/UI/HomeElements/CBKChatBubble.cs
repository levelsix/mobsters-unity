#define VERBOSE

using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKChatBubble : MonoBehaviour, CBKPoolable {
	
	#region Size Constants
	
	const float FONT_SIZE = 32;
	
	const float BASE_HEIGHT = 78;
	
	const float LINE_HEIGHT = 24;
	
	const float LINE_ADD_SCALE = .43f;
	
	const float ICON_LINE_Y_ADD = -35;
	
	const float BASE_ICON_Y = -5;
	
	const float BASE_ICON_X_POS = -387;
	
	#endregion
	
	CBKChatBubble _prefab;
	
	GameObject gameObj;
	
	Transform trans;
	
	public CBKPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as CBKChatBubble;
		}
	}
	
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	
	public Transform transf {
		get {
			return trans;
		}
	}
	
	public int height;
	
	public CBKPoolable Make (Vector3 origin)
	{
		CBKChatBubble bubble = Instantiate(this, origin, Quaternion.identity) as CBKChatBubble;
		bubble.prefab = this;
		return bubble;
	}
	
	public void Pool ()
	{
		CBKPoolManager.instance.Pool(this);
	}
	
	[SerializeField]
	UILabel textLabel;
	
	[SerializeField]
	UILabel senderLabel;
	
	[SerializeField]
	UISprite bubble;

	[SerializeField]
	UILabel timeLabel;
	
	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
	}
	
	/// <summary>
	/// Sets up the bubble for this message
	/// </summary>
	/// <param name='proto'>
	/// Proto.
	/// </param>
	/// <returns>
	/// The size of this bubble, for spacing purposes
	/// </returns>
	public void Init(GroupChatMessageProto proto)
	{
		height = Init(proto.timeOfChat, proto.sender.minUserProto.name, proto.content, proto.sender.level, proto.isAdmin);
	}
	
	/// <summary>
	/// Same as Init(GroupChatMessageProto), but with PrivateChatPostProto
	/// </summary>
	/// <param name='proto'>
	/// Proto.
	/// </param>
	public void Init(PrivateChatPostProto proto)
	{
		//Same shit, different proto
		height = Init(proto.timeOfPost, proto.poster.minUserProto.name, proto.content, proto.poster.level);
	}

	public void Init(ReceivedGroupChatResponseProto proto)
	{
		height = Init(CBKUtil.timeNowMillis, proto.sender.minUserProto.name, proto.chatMessage, proto.sender.level, proto.isAdmin);
	}
	
	/// <summary>
	/// Sets up a chat bubble
	/// </summary>
	/// <param name='type'>
	/// User Type (for the icon)
	/// </param>
	/// <param name='message'>
	/// Message.
	/// </param>
	int Init(long time, string sender, string message, int level, bool leader = false)
	{	
		//Fill text with message
		textLabel.text = message;
		senderLabel.text = sender;
		
		//Count the lines
		int lines = Mathf.RoundToInt(textLabel.localSize.y / FONT_SIZE);
		
		//Size the bubble
		bubble.height = (int)(BASE_HEIGHT + (lines - 1) * LINE_HEIGHT);

#if VERBOSE
		
		//Debug.Log("Message: " + textLabel.processedText + "\nLines: " + lines);
		
#endif

		StartCoroutine(CheckTime(time));

		return bubble.height;
	}

	IEnumerator CheckTime(long timeSent)
	{
		while(true)
		{
			long timePassed = CBKUtil.timeNowMillis - timeSent;
			if (timePassed < 60000)
			{
				timeLabel.text = "just recently";
			}
			else
			{
				timeLabel.text = (CBKUtil.TimeStringLong(timePassed, true)) + " ago";
			}
			yield return new WaitForSeconds(1);
		}
	}
}
