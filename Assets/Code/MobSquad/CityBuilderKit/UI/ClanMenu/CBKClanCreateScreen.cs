using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKClanCreateScreen : MonoBehaviour {

	[SerializeField]
	UIInput clanNameBox;

	[SerializeField]
	UIInput clanTagBox;

	[SerializeField]
	UIInput descriptionBox;

	[SerializeField]
	CBKActionButton changeClanTypeButton;

	[SerializeField]
	CBKActionButton createClanButton;

	public string symbolSpriteName = "shield";

	public bool openClan;

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
		clanNameBox.label.text = " ";
		//clanNameBox.maxChars = CBKWhiteboard.constants.clanConstants.maxCharLengthForClanName;

		clanTagBox.label.text = " ";
		//clanTagBox.maxChars = CBKWhiteboard.constants.clanConstants.maxCharLengthForClanTag;

		descriptionBox.label.text = " ";
		//descriptionBox.maxChars = CBKWhiteboard.constants.clanConstants.maxCharLengthForClanDescription;

		changeClanTypeButton.label.text = OPEN_CLAN_BUTTON_LABEL;
		openClan = true;
	}

	void ChangeClanType()
	{
		openClan = !openClan;
		if (openClan)
		{
			changeClanTypeButton.label.text = OPEN_CLAN_BUTTON_LABEL;
		}
		else
		{
			changeClanTypeButton.label.text = REQUEST_CLAN_BUTTON_LABEL;
		}
	}

	void SubmitClan()
	{
		if (clanNameBox.label.text.Length > 0 && MSResourceManager.instance.Spend(ResourceType.CASH, MSWhiteboard.constants.clanConstants.coinPriceToCreateClan, SubmitClan))
		{
			MSClanManager.instance.CreateClan(
				clanNameBox.label.text,
				clanTagBox.label.text,
				openClan,
				descriptionBox.label.text);
		}
		else
		{
			MSActionManager.Popup.CreatePopup("Invalid Name");
		}
	}
}
