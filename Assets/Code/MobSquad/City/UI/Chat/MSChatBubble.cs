#define VERBOSE

using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSChatBubble : MonoBehaviour, MSPoolable {
	
	#region Size Constants

	[SerializeField] int LINE_LENGTH = 40;

	[SerializeField] int LINE_WIDTH = 100;
	
	#endregion
	
	MSChatBubble _prefab;
	
	GameObject gameObj;
	
	Transform trans;

	bool rightSide = true;
	
	public MSPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as MSChatBubble;
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
	
	public MSPoolable Make (Vector3 origin)
	{
		MSChatBubble bubble = Instantiate(this, origin, Quaternion.identity) as MSChatBubble;
		bubble.prefab = this;
		return bubble;
	}
	
	public void Pool ()
	{
		MSPoolManager.instance.Pool(this);
	}
	
	[SerializeField]
	UILabel textLabel;
	
	[SerializeField]
	UILabel senderLabel;
	
	[SerializeField]
	UISprite bubble;
	
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
		Init(proto.timeOfChat, proto.sender.minUserProto.name, proto.content, proto.sender.level, proto.isAdmin);
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
		Init(proto.timeOfPost, proto.poster.minUserProto.name, proto.content, proto.poster.level);
	}

	public void Init(ReceivedGroupChatResponseProto proto)
	{
		Init(MSUtil.timeNowMillis, proto.sender.minUserProto.name, proto.chatMessage, proto.sender.level, proto.isAdmin);
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
	void Init(long time, string sender, string message, int level, bool leader = false)
	{	
		//Fill text with message
		textLabel.text = message;
		senderLabel.text = sender;

		if (message.Length < LINE_LENGTH)
		{
			textLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
			textLabel.alignment = rightSide ? NGUIText.Alignment.Right : NGUIText.Alignment.Left;
		}
		else
		{
			textLabel.overflowMethod = UILabel.Overflow.ResizeHeight;
			textLabel.width = LINE_WIDTH;
			textLabel.alignment = NGUIText.Alignment.Left;
		}

#if VERBOSE
		
		//Debug.Log("Message: " + textLabel.processedText + "\nLines: " + lines);
		
#endif

	}
}
