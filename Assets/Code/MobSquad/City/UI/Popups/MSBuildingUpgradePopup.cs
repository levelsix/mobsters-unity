using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;


/// <summary>
/// @author Rob Giusti
/// Popup that shows up when paying to start upgrading a building
/// or paying to rush construction on a building
/// </summary>
public class MSBuildingUpgradePopup : MonoBehaviour {
	
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
	/// The upgrade time information.
	/// </summary>
	[SerializeField]
	UILabel upgradeTime;

	[SerializeField]
	UI2DSprite buildingSprite;
	
	/// <summary>
	/// The upgrade button.
	/// </summary>
	[SerializeField]
	MSActionButton upgradeButton;

	[SerializeField]
	UILabel topQuality;

	[SerializeField]
	MSFillBar topBarCurrent;

	[SerializeField]
	MSFillBar topBarFuture;

	[SerializeField]
	UILabel topBarText;

	[SerializeField]
	GameObject bottomBar;

	[SerializeField]
	UILabel botQuality;

	[SerializeField]
	MSFillBar botBarCurrent;

	[SerializeField]
	MSFillBar botBarFuture;

	[SerializeField]
	UILabel botBarText;

	[SerializeField]
	MSHireEntry hireEntryPrefab;

	[SerializeField]
	Transform grid;

	List<MSHireEntry> hireEntries = new List<MSHireEntry>();

	const string cashButtonName = "greenmenuoption";
	const string oilButtonName = "yellowmenuoption";

	static readonly Color cashTextColor = new Color(.353f, .491f, .027f);
	static readonly Color oilTextColor = new Color(.776f, .533f, 0);
	
	MSBuilding currBuilding;
	
	ResourceType currResource;
	int currCost;
	
	void TryToBuy()
	{
		Debug.LogWarning("Trying to buy");

		if (MSBuildingManager.instance.currentUnderConstruction != null)
		{
			MSActionManager.Popup.CreateButtonPopup("Your builder is busy! Speed him up for " + 
			                                        MSMath.GemsForTime(MSBuildingManager.instance.currentUnderConstruction.completeTime)
			                                        + "gems and upgrade this building?",
			                                        new string[]{"Cancel", "Speed Up"},
			new Action[]{MSActionManager.Popup.CloseTopPopupLayer,
				delegate
				{
					MSActionManager.Popup.CloseTopPopupLayer();
					MSBuildingManager.instance.currentUnderConstruction.CompleteWithGems();
					TryToBuy();
				}
			}
			);
			return;
		}

		//Spend Money Here
		if (MSResourceManager.instance.Spend(currResource, currCost, TryToBuy))
		{
			if(currBuilding.userStructProto.isComplete)
			{
				currBuilding.upgrade.StartUpgrade();
			}
			else
			{
				currBuilding.upgrade.FinishWithPremium();
			}
			if (MSActionManager.Popup.CloseAllPopups != null)
			{
				MSActionManager.Popup.CloseAllPopups();
			}

			MSBuildingManager.instance.FullDeselect();
		}
	}
	
	public void Init(MSBuilding building)
	{
		currBuilding = building;
		
		gameObject.SetActive(true);

		MSFullBuildingProto nextBuilding = null;
		MSFullBuildingProto oldBuilding = null;
		if (!building.userStructProto.isComplete && building.combinedProto.structInfo.predecessorStructId > 0)
		{
			oldBuilding = building.combinedProto.predecessor;
			nextBuilding = building.combinedProto;
		}
		else if (building.combinedProto.structInfo.successorStructId > 0)
		{
			oldBuilding = building.combinedProto;
			nextBuilding = building.combinedProto.successor;
		}


		if (nextBuilding != null) //This should really be a precondition. Does the button even show up otherwise?
		{
				
			header.text = "Upgrade to level " + nextBuilding.structInfo.level + "?";
			
			upgradeTime.text = MSUtil.TimeStringLong(nextBuilding.structInfo.minutesToBuild * 60000);
			
			currResource = nextBuilding.structInfo.buildResourceType;
			
			currCost = (int) (nextBuilding.structInfo.buildCost);

			upgradeButton.button.normalSprite = ((nextBuilding.structInfo.buildResourceType == ResourceType.CASH) ? cashButtonName : oilButtonName);
			upgradeButton.label.text = ((nextBuilding.structInfo.buildResourceType == ResourceType.CASH) ? "$" : "(o) ") + currCost.ToString();
			upgradeButton.label.color = ((nextBuilding.structInfo.buildResourceType == ResourceType.CASH) ? cashTextColor : oilTextColor);

			Sprite sprite = MSSpriteUtil.instance.GetBuildingSprite(nextBuilding.structInfo.imgName);
			buildingSprite.sprite2D = sprite;
			if (sprite != null)
			{
				buildingSprite.width = (int)sprite.textureRect.width;
				buildingSprite.height = (int)sprite.textureRect.height;
			}

			buildingName.text = nextBuilding.structInfo.name;


			SetBuildingBarInfo (building, oldBuilding, nextBuilding);

			upgradeButton.onClick = TryToBuy;
			upgradeButton.button.enabled = true;
		}
	}

	void SetBuildingBarInfo (MSBuilding building, MSFullBuildingProto oldBuilding, MSFullBuildingProto nextBuilding)
	{
		MSFullBuildingProto max = nextBuilding.maxLevel;

		switch (building.combinedProto.structInfo.structType) {
		case StructureInfoProto.StructType.RESOURCE_GENERATOR:
			topQuality.text = "Rate:";
			botQuality.text = "Capacity:";
			bottomBar.SetActive (true);
			SetBar (topBarCurrent, topBarFuture, oldBuilding.generator.productionRate, nextBuilding.generator.productionRate, max.generator.productionRate);
			SetBar (botBarCurrent, botBarFuture, oldBuilding.generator.capacity, nextBuilding.generator.capacity, max.generator.capacity);
			if (oldBuilding.generator.resourceType == ResourceType.CASH) {
				topBarText.text = "$" + oldBuilding.generator.productionRate + " + $" + (nextBuilding.generator.productionRate - oldBuilding.generator.productionRate) + " Per Hour";
				botBarText.text = "$" + oldBuilding.generator.capacity + " + $" + (nextBuilding.generator.capacity - oldBuilding.generator.capacity);
			}
			else {
				topBarText.text = oldBuilding.generator.productionRate + " + " + (nextBuilding.generator.productionRate - oldBuilding.generator.productionRate) + " Per Hour";
				botBarText.text = oldBuilding.generator.capacity + " + " + (nextBuilding.generator.capacity - oldBuilding.generator.capacity);
			}
			break;
		case StructureInfoProto.StructType.RESOURCE_STORAGE:
			topQuality.text = "Capacity:";
			bottomBar.SetActive (false);
			SetBar (topBarCurrent, topBarFuture, oldBuilding.storage.capacity, nextBuilding.storage.capacity, max.storage.capacity);
			if (oldBuilding.storage.resourceType == ResourceType.CASH) {
				topBarText.text = "$" + oldBuilding.storage.capacity + " + $" + (nextBuilding.storage.capacity - oldBuilding.storage.capacity);
			}
			else {
				topBarText.text = oldBuilding.storage.capacity + " + " + (nextBuilding.storage.capacity - oldBuilding.storage.capacity);
			}
			break;
		case StructureInfoProto.StructType.HOSPITAL:
			topQuality.text = "Queue Size:";
			botQuality.text = "Rate:";
			bottomBar.SetActive (true);
			SetBar (topBarCurrent, topBarFuture, oldBuilding.hospital.queueSize, nextBuilding.hospital.queueSize, max.hospital.queueSize);
			SetBar (botBarCurrent, botBarFuture, oldBuilding.hospital.healthPerSecond, nextBuilding.hospital.healthPerSecond, max.hospital.healthPerSecond);
			if (nextBuilding.hospital.queueSize > oldBuilding.hospital.queueSize) {
				topBarText.text = oldBuilding.hospital.queueSize + " + " + (nextBuilding.hospital.queueSize - oldBuilding.hospital.queueSize);
			}
			else {
				topBarText.text = nextBuilding.hospital.queueSize.ToString ();
			}
			if (nextBuilding.hospital.healthPerSecond > oldBuilding.hospital.healthPerSecond) {
				topBarText.text = oldBuilding.hospital.healthPerSecond + " + " + (nextBuilding.hospital.healthPerSecond - oldBuilding.hospital.healthPerSecond) + " Health Per Sec";
			}
			else {
				topBarText.text = oldBuilding.hospital.healthPerSecond + " Health Per Sec";
			}
			break;
		case StructureInfoProto.StructType.LAB:
			topQuality.text = "Queue Size:";
			botQuality.text = "Rate:";
			bottomBar.SetActive (true);
			SetBar (topBarCurrent, topBarFuture, oldBuilding.lab.queueSize, nextBuilding.lab.queueSize, max.lab.queueSize);
			SetBar (botBarCurrent, botBarFuture, oldBuilding.lab.pointsPerSecond, nextBuilding.lab.pointsPerSecond, max.lab.pointsPerSecond);
			if (nextBuilding.lab.queueSize > oldBuilding.lab.queueSize) {
				topBarText.text = oldBuilding.lab.queueSize + " + " + (nextBuilding.lab.queueSize - oldBuilding.lab.queueSize);
			}
			else {
				topBarText.text = nextBuilding.lab.queueSize.ToString ();
			}
			if (nextBuilding.lab.pointsPerSecond > oldBuilding.lab.pointsPerSecond) {
				topBarText.text = oldBuilding.lab.pointsPerSecond + " + " + (nextBuilding.lab.pointsPerSecond - oldBuilding.lab.pointsPerSecond) + " Points Per Sec";
			}
			else {
				topBarText.text = oldBuilding.lab.pointsPerSecond + " Points Per Sec";
			}
			break;
		case StructureInfoProto.StructType.RESIDENCE:
			topQuality.text = "Slots:";
			bottomBar.SetActive (false);
			SetBar (topBarCurrent, topBarFuture, oldBuilding.residence.numMonsterSlots, nextBuilding.residence.numMonsterSlots, max.residence.numMonsterSlots);
			topBarText.text = oldBuilding.residence.numMonsterSlots + " + " + nextBuilding.residence.numMonsterSlots;
			break;
		case StructureInfoProto.StructType.TOWN_HALL:
			topQuality.text = "City Level:";
			bottomBar.SetActive (false);
			SetBar (topBarCurrent, topBarFuture, oldBuilding.structInfo.level, nextBuilding.structInfo.level, max.structInfo.level);
			topBarText.text = oldBuilding.structInfo.level + " + " + nextBuilding.structInfo.level;
			break;
		}
	}

	void SetBar(MSFillBar currBar, MSFillBar nextBar, float curr, float next, float max)
	{
		currBar.fill = curr/max;
		nextBar.fill = next/max;
	}
	
	IEnumerator UpdateRemainingTime()
	{
		while(!currBuilding.userStructProto.isComplete)
		{
			upgradeTime.text = MSUtil.TimeStringMed(currBuilding.upgrade.timeRemaining);
			currCost = currBuilding.upgrade.gemsToFinish;
			upgradeButton.label.text = currCost.ToString();
			yield return new WaitForSeconds(1);
		}
		Init(currBuilding);
	}
	
}
