using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[RequireComponent (typeof (MSSimplePoolable))]
[RequireComponent (typeof (MSUIHelper))]
public class MSBottomChatMessage : MonoBehaviour {

	[SerializeField]
	MSChatAvatar avatar;

	[SerializeField]
	UILabel playerName;

	[SerializeField]
	UILabel dialogue;

	const int maxChars = 60;

	public void Init(int avatarId, string name, string content)
	{
		avatar.Init (avatarId);
		playerName.text = name + ":";
		dialogue.text = content;
		Chop();
	}

	[ContextMenu ("Chop")]
	public void Chop()
	{
		if (playerName.text.Length + dialogue.text.Length > maxChars)
		{
			//dialogue.text = dialogue.text.Substring(0, maxChars - playerName.text.Length - 3) + "...";
		}
	}
}
