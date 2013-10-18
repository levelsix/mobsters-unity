using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// The controller for the chat popup
/// </summary>
public class CBKChatPopup : MonoBehaviour {
	
	/// <summary>
	/// The number of pixels to space vertically between messages
	/// </summary>
	const float SPACE_BETWEEN_MESSAGES = 5f;
	
	#region UI Parts
	
	[SerializeField]
	UIInput inputField;
	
	[SerializeField]
	Transform grid;
	
	#endregion
	
	#region Prefabs
	
	[SerializeField]
	CBKChatBubble leftPrefab;
	
	[SerializeField]
	CBKChatBubble rightPrefab;
	
	#endregion
	
	public void SendChatMessage()
	{
		if (inputField.label.text.Length > 0)
		{
			SendGroupChatRequestProto request = new SendGroupChatRequestProto();
			
			request.sender = CBKWhiteboard.localMup;
			request.scope = GroupChatScope.GLOBAL;
			request.chatMessage = inputField.label.text;
			request.clientTime = CBKUtil.timeNow;
			
			UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolResponse.S_SEND_GROUP_CHAT_EVENT, CheckServerChatResponse);
			
			inputField.label.text = "";
		}
	}
	
	/// <summary>
	/// Makes sure that our chat went through and it's not innapropro or nuttin'
	/// </summary>
	void CheckServerChatResponse(int tagNum)
	{
		SendGroupChatResponseProto response = UMQNetworkManager.responseDict[tagNum] as SendGroupChatResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != SendGroupChatResponseProto.SendGroupChatStatus.SUCCESS)
		{
			Debug.LogError("Problem sending chat message: " + response.status.ToString());
		}
	}
	
	void Init(List<GroupChatMessageProto> messages)
	{
		
	}
	
}
