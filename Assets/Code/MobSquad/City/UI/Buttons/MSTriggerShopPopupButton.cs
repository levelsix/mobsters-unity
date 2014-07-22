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

	void OnEnable()
	{
		if(!shortCutButton)
		{
			MSActionManager.Loading.OnBuildingsLoaded += UpdateBadge;
			MSActionManager.Town.PlaceBuilding += UpdateBadge;
		}
	}

	void OnDisable()
	{
		if(!shortCutButton)
		{
			MSActionManager.Loading.OnBuildingsLoaded -= UpdateBadge;
			MSActionManager.Town.PlaceBuilding -= UpdateBadge;
		}
	}

	public virtual void OnClick(){
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
		//TODO: add free gatcha stuff
		int availableBuildings = MSBuildingManager.instance.CapacityForBuildings();//add free gacha count
		badge.notifications = availableBuildings;

		building.badge.notifications = availableBuildings;
	}
}
