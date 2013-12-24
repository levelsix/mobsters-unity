using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UIInput))]
public class CBKSetCharacterLimit : MonoBehaviour {

	public enum InputType {PLAYER_NAME, CLAN_NAME, CHAT};

	[SerializeField]
	InputType inputType;

	UIInput input;

	void Awake()
	{
		input = GetComponent<UIInput>();
	}

	void Start()
	{
		switch(inputType)
		{
		case InputType.PLAYER_NAME:
			input.characterLimit = CBKWhiteboard.constants.maxNameLength;
			break;
		case InputType.CHAT:
			input.characterLimit = CBKWhiteboard.constants.maxLengthOfChatString;
			break;
		}
	}
}
