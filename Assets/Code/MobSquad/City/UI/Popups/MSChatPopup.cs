using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// The controller for the chat popup
/// </summary>
public class MSChatPopup : MonoBehaviour {

	#region UI Parts
	
	[SerializeField]
	UIInput inputField;

	[SerializeField]
	MSTab globalChatButton;

	[SerializeField]
	MSTab clanChatButton;

	[SerializeField]
	MSTab privateChatButton;

	#endregion
	
	public void SendChatMessage()
	{

		if (inputField.label.text.EndsWith("|"))
		{
			inputField.label.text = inputField.label.text.Substring(0, inputField.label.text.Length - 1);
		}

		if (inputField.label.text.Length > 0)
		{
			//TODO: Private chat send

			SendGroupChatRequestProto request = new SendGroupChatRequestProto();
			
			request.sender = MSWhiteboard.localMup;
			if(MSChatManager.instance.currMode == MSValues.ChatMode.CLAN)
			{
				Debug.Log("Sending Clan Message");
				request.scope = GroupChatScope.CLAN;
			}
			else
			{
				request.scope = GroupChatScope.GLOBAL;
			}
			request.chatMessage = inputField.label.text;
			request.clientTime = MSUtil.timeNowMillis;
			
			UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolResponse.S_SEND_GROUP_CHAT_EVENT, CheckServerChatResponse);
		}

		inputField.value = "";
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
	
	void OnEnable()
	{
		SetGlobalChat();
	}

	public void SetGlobalChat()
	{
		MSChatManager.instance.SetChatMode(MSValues.ChatMode.GLOBAL);

		globalChatButton.InitActive();
		clanChatButton.InitInactive();
		privateChatButton.InitInactive();
	}

	public void SetClanChat()
	{
		MSChatManager.instance.SetChatMode(MSValues.ChatMode.CLAN);
		
		globalChatButton.InitInactive();
		clanChatButton.InitActive();
		privateChatButton.InitInactive();
	}

	public void SetPrivateChat()
	{
		MSChatManager.instance.SetChatMode(MSValues.ChatMode.PRIVATE);
		
		globalChatButton.InitInactive();
		clanChatButton.InitInactive();
		privateChatButton.InitActive();
	}
	
}
