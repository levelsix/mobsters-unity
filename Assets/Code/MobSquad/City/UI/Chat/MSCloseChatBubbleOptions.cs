using UnityEngine;
using System.Collections;

public class MSCloseChatBubbleOptions : MonoBehaviour {

	void OnClick()
	{
		MSChatBubbleOptions.instance.Close();
	}
}
