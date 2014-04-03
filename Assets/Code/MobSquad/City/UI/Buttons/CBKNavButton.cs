using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Buttons for the top of the chat popup that change what type of chat to currently display.
/// </summary>
public class CBKNavButton : CBKActionButton {
	
	[SerializeField]
	MSValues.ChatMode chatMode = MSValues.ChatMode.GLOBAL;
	
	public override void OnClick()
	{
		MSChatManager.instance.SetChatMode(chatMode);
	}
	
}
