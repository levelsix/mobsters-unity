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
	
	CBKResourceManager.ResourceType currResource;
	int currCost;
	
	void OnEnable()
	{
		upgradeButton.onClick += TryToBuy;
	}
	
	void OnDisable()
	{
		upgradeButton.onClick -= TryToBuy;
	}
	
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
		
		int futureLevel = 1;//building.userStructProto.level + 1;

		StructureInfoProto nextBuilding = null;
		if (building.combinedProto.structInfo.successorStructId > 0)
		{
			nextBuilding = CBKDataManager.instance.Get(typeof(StructureInfoProto), building.combinedProto.structInfo.successorStructId) as StructureInfoProto;
		}

		if (nextBuilding != null)
		{
		//If it is complete, use Upgrade prompts, otherwise, Finish prompts
			if (building.userStructProto.isComplete)
			{
				
				header.text = "Upgrade to level " + futureLevel + "?";
				
				upgradeTime.text = CBKUtil.TimeStringLong(building.upgrade.TimeToUpgrade(1));//building.userStructProto.level+1));
				
				currResource = building.baseResource;
				
				currCost = (int) (building.basePrice);
				
			}
			else
			{
				header.text = "Finish upgrade?";
				
				currResource = CBKResourceManager.ResourceType.PREMIUM;
				currCost = building.upgrade.gemsToFinish;
				
			}
		
			upgradeCurrency.type = currResource;
			upgradeCostLabel.text = currCost.ToString();
			
			//buildingIcon.SetAtlasSprite(CBKAtlasUtil.instance.LookupBuildingSprite(building.structProto.name));
			buildingIcon.spriteName = CBKUtil.StripExtensions(building.combinedProto.structInfo.name);
			CBKAtlasUtil.instance.SetAtlasForSprite(buildingIcon);
			buildingName.text = building.combinedProto.structInfo.name;
			
			//Takes the color from the editor and turns it into hex values so that NGUI can interpret
			string moneyColorHexString = CBKMath.ColorToInt(moneyColor).ToString("X"); 
			//string timeString = CBKUtil.TimeStringMed(building.structProto.minutesToGain * 60);
			
			currIncome.text = "Current income:\n[" + moneyColorHexString + "]$" 
				+ building.collector._generator.productionRate + "[-] every hour";
			//TODO: Get this
			//futureIncome.text = "Upgraded income:\n[" + moneyColorHexString + "]$" 
					//+ building.collector.MoneyAtLevel(futureLevel) + "[-] every " + timeString;
		}
	}
	
	
	
}
