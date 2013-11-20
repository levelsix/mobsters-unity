using UnityEngine;
using System.Collections;

public class CBKClanCreateScreen : MonoBehaviour {

	[SerializeField]
	UIInput clanNameBox;

	[SerializeField]
	UIInput clanTagBox;

	[SerializeField]
	UIInput descriptionBox;

	[SerializeField]
	CBKActionButton editSymbolButton;

	[SerializeField]
	CBKActionButton changeClanTypeButton;

	[SerializeField]
	CBKActionButton createClanButton;

	public string symbolSpriteName = "shield";

	public bool typeOfClan;

	const string OPEN_CLAN_BUTTON_LABEL = "OPEN";
	const string REQUEST_CLAN_BUTTON_LABEL = "REQUEST ONLY";

	public void OnEnable()
	{

	}

	public void OnDisable()
	{

	}

	public void Init()
	{
		clanNameBox.label.text = "";
		clanNameBox.maxChars = CBKWhiteboard.constants.clanConstants.maxCharLengthForClanName;

		clanTagBox.label.text = "";
		clanTagBox.maxChars = CBKWhiteboard.constants.clanConstants.maxCharLengthForClanTag;

		descriptionBox.label.text = "";
		descriptionBox.maxChars = CBKWhiteboard.constants.clanConstants.maxCharLengthForClanDescription;
	}
}
