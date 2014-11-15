
using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSChatBubble : MonoBehaviour, MSPoolable {

	[SerializeField] int lineMaxWidth = 100;
	
	protected MSChatBubble _prefab;
	
	protected GameObject gameObj;
	
	protected Transform trans;

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

	static int nextId = 0;
	int id = nextId++;
	
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
	public UILabel textLabel;
	
	[SerializeField]
	public UILabel senderLabel;

	[SerializeField]
	protected UILabel timeLabel;

	[SerializeField]
	protected UISprite bubble;

	[SerializeField]
	protected MSChatAvatar avatar;
	public int avatarID = 0;

	MSChatBubbleOptions options
	{
		get
		{
			return MSChatBubbleOptions.instance;
		}
	}

	protected MinimumUserProtoWithLevel sender;

	public long timeSent;
	
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
		sender = proto.sender;
		Init(proto.timeOfChat, proto.sender.minUserProto.name, proto.content, proto.sender.minUserProto.avatarMonsterId, proto.isAdmin);
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
		sender = proto.poster;
		Init(proto.timeOfPost, proto.poster.minUserProto.name, proto.content, proto.poster.minUserProto.avatarMonsterId);
	}

	public void Init(ReceivedGroupChatResponseProto proto)
	{
		sender = proto.sender;
		Init(MSUtil.timeNowMillis, proto.sender.minUserProto.name, proto.chatMessage, proto.sender.minUserProto.avatarMonsterId, proto.isAdmin);
	}

	public void InitCheat(string message)
	{
		//sender = MSWhiteboard.localMup;
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
	protected void Init(long time, string sender, string message, int avatarId, bool leader = false)
	{
		avatar.Init(avatarId);
		this.avatarID = avatarId;
		timeSent = time;

		//Fill text with message
		textLabel.text = message;

		senderLabel.text = sender;
		timeLabel.text = MSUtil.TimeStringLong(MSUtil.timeNowMillis - time) + " ago";

		int topLength = (int)(senderLabel.printedSize.x + timeLabel.printedSize.x + 50);
		bubble.width = Mathf.Max(topLength, (int)textLabel.printedSize.x) + 75;
		bubble.height = (int)textLabel.printedSize.y + 75;
	}

	void OnClick()
	{
		if (!sender.minUserProto.userUuid.Equals(MSWhiteboard.localMup.userUuid))
		{
			options.Init(sender, trans);
			options.transform.localScale = Vector3.one;
		}
	}
}
