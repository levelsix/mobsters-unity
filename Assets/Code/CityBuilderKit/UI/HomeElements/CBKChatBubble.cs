#define VERBOSE

using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKChatBubble : MonoBehaviour, CBKPoolable {
	
	#region Size Constants
	
	const float FONT_SIZE = 63;
	
	const float BASE_HEIGHT = 86;
	
	const float LINE_HEIGHT = 37;
	
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
	UISprite icon;
	
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
		height = Init(proto.timeOfChat, proto.sender.minUserProto.name, proto.content);
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
		height = Init(proto.timeOfPost, proto.poster.minUserProto.name, proto.content);
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
	int Init(long time, string sender, string message)
	{	
		//Fill text with message
		textLabel.text = message;
		senderLabel.text = sender;
		
		//Count the lines
		int lines = Mathf.RoundToInt(textLabel.localSize.y / FONT_SIZE);
		
		//Size the bubble
		bubble.height = (int)(BASE_HEIGHT + (lines - 1) * LINE_HEIGHT);
		
		//Set up the icon
		//icon.spriteName = CBKValues.CharacterNames[(int)type];
		icon.transform.localPosition = new Vector3(BASE_ICON_X_POS, BASE_ICON_Y + (lines-1) * ICON_LINE_Y_ADD);
#if VERBOSE
		
		Debug.Log("Message: " + textLabel.processedText + "\nLines: " + lines);
		
#endif
		
		return bubble.height;
	}
}
