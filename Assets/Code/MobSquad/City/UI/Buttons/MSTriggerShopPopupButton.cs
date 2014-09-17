using UnityEngine;
using System.Collections;

public class MSTriggerShopPopupButton : MSTriggerPopupButton {

	[SerializeField]
	MSTabButton funds;

	[SerializeField]
	MSTabButton monsters;

	[SerializeField]
	MSTabButton building;

	/// <summary>
	/// This badge displays the total of buildings available for purchase and number of free gatchas
	/// </summary>
	[SerializeField]
	MSBadge badge;

	/// <summary>
	/// Short cut button allows for forcing a specific tab to start open.
	/// Short cut buttons can't have a badge
	/// </summary>
	[SerializeField]
	bool shortCutButton = false;

	enum Button
	{
		FUNDS,
		MONSTERS,
		BUILDING
	}

	[SerializeField]
	Button shortCutTo;

	const string redBadge = "badgeicon";
	const string blueBadge = "basic1spin";

	void OnEnable()
	{
		if(!shortCutButton)
		{
			MSActionManager.Loading.OnBuildingsLoaded += UpdateBadge;
			MSActionManager.Town.PlaceBuilding += UpdateBadge;
			MSActionManager.Gacha.OnPurchaseBoosterSucces += UpdateBadge;
			if (MSTutorialManager.instance != null && MSTutorialManager.instance.inTutorial && badge != null)
			{
				UpdateBadge();
			}
		}

	}

	void OnDisable()
	{
		if(!shortCutButton)
		{
			MSActionManager.Loading.OnBuildingsLoaded -= UpdateBadge;
			MSActionManager.Town.PlaceBuilding -= UpdateBadge;
			MSActionManager.Gacha.OnPurchaseBoosterSucces -= UpdateBadge;
		}
	}

	public virtual void OnClick(){
		MSBuildingManager.instance.FullDeselect();
		base.OnClick();
		if(shortCutButton)
		{
			switch(shortCutTo)
			{
			case Button.BUILDING:
				building.OnClick();
				break;
			case Button.FUNDS:
				funds.OnClick();
				break;
			case Button.MONSTERS:
				monsters.OnClick();
				break;
			default:
				break;
			}
		}
	}

	void UpdateBadge(){
		int availableBuildings = MSBuildingManager.instance.CapacityForBuildings();
		if(!MSTutorialManager.instance.inTutorial && MSUtil.timeSince(MSWhiteboard.localUser.lastFreeBoosterPackTime) > 24 * 60 * 60 * 1000)
		{
			badge.sprite.spriteName = blueBadge;
			badge.notifications = 1;
			monsters.badge.notifications = 1;
			monsters.secondaryBadge.notifications = 1;
		}
		else
		{
			monsters.badge.notifications = 0;
			monsters.secondaryBadge.notifications = 0;
			
			badge.sprite.spriteName = redBadge;
			badge.notifications = availableBuildings;
		}

		building.badge.notifications = availableBuildings;

	}
}
