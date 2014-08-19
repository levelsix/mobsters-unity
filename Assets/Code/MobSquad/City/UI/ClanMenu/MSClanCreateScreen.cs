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

	[SerializeField]
	UI2DSprite clanIcon;

	public bool openClan;

	int currClanIconId = 1;

	const string OPEN_CLAN_BUTTON_LABEL = "OPEN";
	const string REQUEST_CLAN_BUTTON_LABEL = "REQUEST ONLY";

	FullClanProto clanEditting;

	void OnEnable()
	{
		MSActionManager.UI.OnChangeClanIcon += OnChangeClanIcon;
	}

	void OnDisable()
	{
		MSActionManager.UI.OnChangeClanIcon -= OnChangeClanIcon;
	}

	public void InitCreate()
	{
		clanEditting = null;
		OnChangeClanIcon(1);
		Init ();
	}

	public void InitEdit(FullClanProto clan)
	{
		clanEditting = clan;
		OnChangeClanIcon(clan.clanIconId);
		Init ();
	}

	void Init()
	{
		clanNameBox.label.text = "Clan Name";
		clanNameBox.characterLimit = MSWhiteboard.constants.clanConstants.maxCharLengthForClanName;
		
		clanTagBox.label.text = "Tag";
		clanTagBox.characterLimit = MSWhiteboard.constants.clanConstants.maxCharLengthForClanTag;
		
		descriptionBox.label.text = "Description";
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
		if (clanNameBox.label.text.Length > 0)
		{
			if (clanEditting != null && clanEditting.clanId > 0)
			{
				MSClanManager.instance.EditClan(clanEditting, descriptionBox.label.text,
				                                openClan, 1);
			}
			else if (MSResourceManager.instance.Spend(ResourceType.CASH, MSWhiteboard.constants.clanConstants.coinPriceToCreateClan, SubmitClanWithGems))
			{
				MSClanManager.instance.StartCoroutine(MSClanManager.instance.CreateClan(
					clanNameBox.label.text,
					clanTagBox.label.text,
					openClan,
					descriptionBox.label.text,
					currClanIconId,
					MSWhiteboard.constants.clanConstants.coinPriceToCreateClan));
			}
		}
		else
		{
			MSPopupManager.instance.CreatePopup("Invalid Name");
		}
	}

	void SubmitClanWithGems()
	{
		int gemCost = Mathf.CeilToInt((MSWhiteboard.constants.clanConstants.coinPriceToCreateClan - MSResourceManager.resources[ResourceType.CASH]) * MSWhiteboard.constants.gemsPerResource);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost))
		{
			MSClanManager.instance.StartCoroutine(MSClanManager.instance.CreateClan(
				clanNameBox.label.text,
				clanTagBox.label.text,
				openClan,
				descriptionBox.label.text,
				currClanIconId,
				MSResourceManager.instance.SpendAll(ResourceType.CASH),
				gemCost));
		}
	}

	void OnChangeClanIcon(int iconId)
	{
		currClanIconId = iconId;
		MSSpriteUtil.instance.SetSprite("clanicon", "clanicon" + iconId, clanIcon);
	}
}
