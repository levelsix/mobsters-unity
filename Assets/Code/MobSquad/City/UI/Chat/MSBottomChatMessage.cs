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

	[SerializeField]
	int chopWidth;

	UIWidget myWidget;

	public Color color
	{
		set
		{
			dialogue.color = value;
		}
		get
		{
			return dialogue.color;
		}
	}

	public void Init(int avatarId, string name, string content, int width)
	{
		if (myWidget == null) myWidget = GetComponent<UIWidget>();
		myWidget.width = width;

		avatar.Init (avatarId, 3);
		playerName.text = name + ":";
		dialogue.text = content;
		Chop();
	}

	[ContextMenu ("Chop")]
	public void Chop()
	{
		while (playerName.printedSize.x + dialogue.printedSize.x > myWidget.width - chopWidth)
		{
			if (dialogue.text.Length > 4)
			{
				dialogue.text = dialogue.text.Remove(dialogue.text.Length-4);
				if (dialogue.text[dialogue.text.Length-1] == ' ')
				{
					dialogue.text = dialogue.text.Remove(dialogue.text.Length-1);
				}
				dialogue.text += "...";
			}
			else
			{
				playerName.text = playerName.text.Remove(playerName.text.Length - 5) + "...:";
			}
		}
	}


}
