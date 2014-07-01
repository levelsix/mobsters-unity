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

	const int maxChars = 80;

	public void Init(int avatarId, string name, string content)
	{
		avatar.Init (avatarId);
		playerName.text = name + ":";
		if (name.Length + content.Length > maxChars)
		{
			dialogue.text = content.Substring(0, maxChars - name.Length) + "...";
		}
		else
		{
			dialogue.text = content;
		}
	}
}
