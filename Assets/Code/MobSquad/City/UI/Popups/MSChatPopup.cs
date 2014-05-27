using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// The controller for the chat popup
/// </summary>
public class MSChatPopup : MonoBehaviour {
	
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
	MSChatBubble leftPrefab;

	[SerializeField]
	MSActionButton globalChatButton;

	[SerializeField]
	MSActionButton clanChatButton;

	[SerializeField]
	MSActionButton privateChatButton;

	[SerializeField]
	UIScrollView scrollView;

	const string ACTIVE_BUTTON_SPRITE = "chatactivetab";
	const string INACTIVE_BUTTON_SPRITE = "chatinactivetab";
	
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

		globalChatButton.icon.spriteName = ACTIVE_BUTTON_SPRITE;
		clanChatButton.icon.spriteName = INACTIVE_BUTTON_SPRITE;
		privateChatButton.icon.spriteName = INACTIVE_BUTTON_SPRITE;
		
		globalChatButton.label.color = Color.white;
		globalChatButton.label.effectColor = Color.black;
		
		clanChatButton.label.color = Color.black;
		clanChatButton.label.effectColor = Color.white;

		privateChatButton.label.color = Color.black;
		privateChatButton.label.effectColor = Color.white;

		scrollView.ResetPosition();
	}

	public void SetClanChat()
	{
		MSChatManager.instance.SetChatMode(MSValues.ChatMode.CLAN);
		
		globalChatButton.icon.spriteName = INACTIVE_BUTTON_SPRITE;
		clanChatButton.icon.spriteName = ACTIVE_BUTTON_SPRITE;
		privateChatButton.icon.spriteName = INACTIVE_BUTTON_SPRITE;

		globalChatButton.label.color = Color.black;
		globalChatButton.label.effectColor = Color.white;

		clanChatButton.label.color = Color.white;
		clanChatButton.label.effectColor = Color.black;

		privateChatButton.label.color = Color.black;
		privateChatButton.label.effectColor = Color.white;
		
		scrollView.ResetPosition();
	}

	public void SetPrivateChat()
	{
		MSChatManager.instance.SetChatMode(MSValues.ChatMode.PRIVATE);
		
		globalChatButton.icon.spriteName = INACTIVE_BUTTON_SPRITE;
		clanChatButton.icon.spriteName = INACTIVE_BUTTON_SPRITE;
		privateChatButton.icon.spriteName = ACTIVE_BUTTON_SPRITE;
		
		globalChatButton.label.color = Color.black;
		globalChatButton.label.effectColor = Color.white;
		
		clanChatButton.label.color = Color.black;
		clanChatButton.label.effectColor = Color.white;
		
		privateChatButton.label.color = Color.white;
		privateChatButton.label.effectColor = Color.black;

		scrollView.ResetPosition();
	}
	
}
