using UnityEngine;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Popup that shows up when paying to start upgrading a building
/// or paying to rush construction on a building
/// </summary>
public class CBKBuildingUpgradePopup : MonoBehaviour {
	
	/// <summary>
	/// The header.
	/// </summary>
	[SerializeField]
	UILabel header;
	
	/// <summary>
	/// The name.
	/// </summary>
	[SerializeField]
	UILabel buildingName;
	
	/// <summary>
	/// The current income information.
	/// </summary>
	[SerializeField]
	UILabel currIncome;
	
	/// <summary>
	/// The future income information.
	/// </summary>
	[SerializeField]
	UILabel futureIncome;
	
	/// <summary>
	/// The upgrade time information.
	/// </summary>
	[SerializeField]
	UILabel upgradeTime;
	
	/// <summary>
	/// The upgrade cost label.
	/// </summary>
	[SerializeField]
	UILabel upgradeCostLabel;
	
	/// <summary>
	/// The building icon.
	/// </summary>
	[SerializeField]
	UISprite buildingIcon;
	
	/// <summary>
	/// The upgrade currency.
	/// </summary>
	[SerializeField]
	CBKCurrencySprite upgradeCurrency;
	
	/// <summary>
	/// The upgrade button.
	/// </summary>
	[SerializeField]
	CBKActionButton upgradeButton;
	
	[SerializeField]
	Color moneyColor = new Color(.2f, 1f, .2f);
	
	CBKBuilding currBuilding;
	
	ResourceType currResource;
	int currCost;
	
	void TryToBuy()
	{
		//Spend Money Here
		if(CBKResourceManager.instance.Spend(currResource, currCost))
		{
			if(currBuilding.userStructProto.isComplete)
			{
				currBuilding.upgrade.StartUpgrade();
			}
			else
			{
				currBuilding.upgrade.FinishWithPremium();
			}
			if (CBKEventManager.Popup.CloseAllPopups != null)
			{
				CBKEventManager.Popup.CloseAllPopups();
			}
		}
	}
	
	public void Init(CBKBuilding building)
	{
		currBuilding = building;
		
		gameObject.SetActive(true);




		//Takes the color from the editor and turns it into hex values so that NGUI can interpret
		string moneyColorHexString = CBKMath.ColorToInt(moneyColor).ToString("X"); 

		CBKCombinedBuildingProto nextBuilding = null;
		CBKCombinedBuildingProto oldBuilding = null;
		if (!building.userStructProto.isComplete && building.combinedProto.structInfo.predecessorStructId > 0)
		{
			oldBuilding = CBKDataManager.instance.Get(typeof(CBKCombinedBuildingProto), building.combinedProto.structInfo.predecessorStructId) as CBKCombinedBuildingProto;
			nextBuilding = building.combinedProto;
		}
		else if (building.combinedProto.structInfo.successorStructId > 0)
		{
			oldBuilding = building.combinedProto;
			nextBuilding = CBKDataManager.instance.Get(typeof(CBKCombinedBuildingProto), building.combinedProto.structInfo.successorStructId) as CBKCombinedBuildingProto;
		}


		if (nextBuilding != null)
		{
		//If it is complete, use Upgrade prompts, otherwise, Finish prompts
			if (building.userStructProto.isComplete)
			{
				
				header.text = "Upgrade to level " + nextBuilding.structInfo.level + "?";
				
				upgradeTime.text = CBKUtil.TimeStringLong(nextBuilding.structInfo.minutesToBuild * 60000);
				
				currResource = nextBuilding.structInfo.buildResourceType;
				
				currCost = (int) (nextBuilding.structInfo.buildCost);
				
			}
			else
			{
				header.text = "Finish upgrade?";

				upgradeTime.text = CBKUtil.TimeStringMed(building.upgrade.timeRemaining);
				StartCoroutine(UpdateRemainingTime());

				currResource = ResourceType.GEMS;
				currCost = building.upgrade.gemsToFinish;
			}
		
			upgradeCurrency.type = currResource;
			upgradeCostLabel.text = currCost.ToString();

			buildingIcon.spriteName = CBKUtil.StripExtensions(nextBuilding.structInfo.imgName);
			UISpriteData buildingSprite = buildingIcon.GetAtlasSprite();
			buildingIcon.width = buildingSprite.width;
			buildingIcon.height = buildingSprite.height;

			CBKAtlasUtil.instance.SetAtlasForSprite(buildingIcon);
			buildingName.text = nextBuilding.structInfo.name;
			
			//TODO: Change this to be specific to the building type!
			currIncome.text = "Current income:\n[" + moneyColorHexString + "]$" 
				+ oldBuilding.generator.productionRate + "[-] every hour";
			futureIncome.text = "Upgraded income:\n[" + moneyColorHexString + "]$" 
				+ nextBuilding.generator.productionRate + "[-] every hour";

			upgradeButton.onClick = TryToBuy;
			upgradeButton.button.enabled = true;
		}
		else //Assume building is fully upgraded
		{
			header.text = "Building at Max Level";

			futureIncome.text = " ";
			upgradeCurrency.type = ResourceType.GEMS;
			upgradeCostLabel.text = " ";

			buildingIcon.spriteName = CBKUtil.StripExtensions(building.combinedProto.structInfo.imgName);
			UISpriteData buildingSprite = buildingIcon.GetAtlasSprite();
			buildingIcon.width = buildingSprite.width;
			buildingIcon.height = buildingSprite.height;

			upgradeButton.onClick = null;
			upgradeButton.button.enabled = false;

			upgradeTime.text = " ";

			currIncome.text = "Current income:\n[" + moneyColorHexString + "]$" 
				+ building.collector._generator.productionRate + "[-] every hour";
		}
	}
	
	IEnumerator UpdateRemainingTime()
	{
		while(!currBuilding.userStructProto.isComplete)
		{
			upgradeTime.text = CBKUtil.TimeStringMed(currBuilding.upgrade.timeRemaining);
			currCost = currBuilding.upgrade.gemsToFinish;
			upgradeCostLabel.text = currCost.ToString();
			yield return new WaitForSeconds(1);
		}
		Init(currBuilding);
	}
	
}
