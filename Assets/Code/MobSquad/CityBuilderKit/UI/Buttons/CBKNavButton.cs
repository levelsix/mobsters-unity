using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Buttons for the top of the chat popup that change what type of chat to currently display.
/// </summary>
public class CBKNavButton : CBKActionButton {
	
	[SerializeField]
	CBKValues.ChatMode chatMode = CBKValues.ChatMode.GLOBAL;
	
	public override void OnClick()
	{
		CBKChatManager.instance.SetChatMode(chatMode);
	}
	
}
