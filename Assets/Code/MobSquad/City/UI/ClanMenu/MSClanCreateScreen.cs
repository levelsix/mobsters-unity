using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSClanCreateScreen : MonoBehaviour {

	[SerializeField]
	UIInput clanNameBox;

	[SerializeField]
	UIInput clanTagBox;

	[SerializeField]
	UIInput descriptionBox;

	[SerializeField]
	MSActionButton changeClanTypeButton;

	public string symbolSpriteName = "shield";

	public bool openClan;

	const string OPEN_CLAN_BUTTON_LABEL = "OPEN";
	const string REQUEST_CLAN_BUTTON_LABEL = "REQUEST ONLY";

	FullClanProto clanEditting;

	public void InitCreate()
	{
		clanEditting = null;
		Init ();
	}

	public void InitEdit(FullClanProto clan)
	{
		clanEditting = clan;
		Init ();
	}

	void Init()
	{
		clanNameBox.label.text = " ";
		clanNameBox.characterLimit = MSWhiteboard.constants.clanConstants.maxCharLengthForClanName;
		
		clanTagBox.label.text = " ";
		clanTagBox.characterLimit = MSWhiteboard.constants.clanConstants.maxCharLengthForClanTag;
		
		descriptionBox.label.text = " ";
		descriptionBox.characterLimit = MSWhiteboard.constants.clanConstants.maxCharLengthForClanDescription;
		
		changeClanTypeButton.label.text = OPEN_CLAN_BUTTON_LABEL;
		openClan = true;
	}

	public void ChangeClanType()
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

	public void SubmitClan()
	{
		if (clanNameBox.label.text.Length > 0 && MSResourceManager.instance.Spend(ResourceType.CASH, MSWhiteboard.constants.clanConstants.coinPriceToCreateClan, SubmitClan))
		{
			if (clanEditting != null && clanEditting.clanId > 0)
			{
				MSClanManager.instance.EditClan(clanEditting, descriptionBox.label.text,
				                                openClan, 1);
			}
			else
			{
				MSClanManager.instance.StartCoroutine(MSClanManager.instance.CreateClan(
					clanNameBox.label.text,
					clanTagBox.label.text,
					openClan,
					descriptionBox.label.text));
			}
		}
		else
		{
			MSActionManager.Popup.CreatePopup("Invalid Name");
		}
	}
}
