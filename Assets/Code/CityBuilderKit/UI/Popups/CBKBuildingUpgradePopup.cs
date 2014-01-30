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
	UILabel qualities;
	
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
	CBKActionButton upgradeButton;

	[SerializeField]
	CBKFillBar topBarCurrent;

	[SerializeField]
	CBKFillBar topBarFuture;

	[SerializeField]
	UILabel topBarText;

	[SerializeField]
	GameObject bottomBar;

	[SerializeField]
	CBKFillBar botBarCurrent;

	[SerializeField]
	CBKFillBar botBarFuture;

	[SerializeField]
	UILabel botBarText;

	[SerializeField]
	Color moneyColor = new Color(.2f, 1f, .2f);

	const string cashButtonName = "confirm";
	const string oilButtonName = "oilupgradebutton";
	
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
			oldBuilding = building.combinedProto.predecessor;
			nextBuilding = building.combinedProto;
		}
		else if (building.combinedProto.structInfo.successorStructId > 0)
		{
			oldBuilding = building.combinedProto;
			nextBuilding = building.combinedProto.successor;
		}

		CBKCombinedBuildingProto max = nextBuilding.maxLevel;

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
		

			upgradeButton.icon.spriteName = ((nextBuilding.structInfo.buildResourceType == ResourceType.CASH) ? cashButtonName : oilButtonName);
			upgradeButton.label.text = ((nextBuilding.structInfo.buildResourceType == ResourceType.CASH) ? "$" : "(O) ") + currCost.ToString();

			Sprite sprite = CBKAtlasUtil.instance.GetBuildingSprite(nextBuilding.structInfo.imgName);
			buildingSprite.sprite2D = sprite;
			if (sprite != null)
			{
				buildingSprite.width = (int)sprite.textureRect.width;
				buildingSprite.height = (int)sprite.textureRect.height;
			}

			buildingName.text = nextBuilding.structInfo.name;


			switch(building.combinedProto.structInfo.structType)
			{
			case StructureInfoProto.StructType.RESOURCE_GENERATOR:
				qualities.text = "Rate:\n\nCapacity:";
				bottomBar.SetActive(true);
				SetBar(topBarCurrent, topBarFuture, oldBuilding.generator.productionRate, nextBuilding.generator.productionRate, max.generator.productionRate);
				SetBar(botBarCurrent, botBarFuture, oldBuilding.generator.capacity, nextBuilding.generator.capacity, max.generator.capacity);
				if (oldBuilding.generator.resourceType == ResourceType.CASH)
				{
					topBarText.text = "$" + oldBuilding.generator.productionRate + 
						" + $" + (nextBuilding.generator.productionRate - oldBuilding.generator.productionRate) + " Per Hour";
					botBarText.text = "$" + oldBuilding.generator.capacity + 
						" + $" + (nextBuilding.generator.capacity - oldBuilding.generator.capacity);
				}
				else
				{
					topBarText.text = oldBuilding.generator.productionRate + 
						" + " + (nextBuilding.generator.productionRate - oldBuilding.generator.productionRate) + " Per Hour"; 
					botBarText.text = oldBuilding.generator.capacity + 
						" + " + (nextBuilding.generator.capacity - oldBuilding.generator.capacity);
				}
				break;
			case StructureInfoProto.StructType.RESOURCE_STORAGE:
				qualities.text = "Capacity:";
				bottomBar.SetActive(false);
				SetBar (topBarCurrent, topBarFuture, oldBuilding.storage.capacity, nextBuilding.storage.capacity, max.storage.capacity);
				if (oldBuilding.storage.resourceType == ResourceType.CASH)
				{
					topBarText.text = "$" + oldBuilding.storage.capacity + 
						" + $" + (nextBuilding.storage.capacity - oldBuilding.storage.capacity);
				}
				else
				{
					topBarText.text = oldBuilding.storage.capacity + 
						" + " + (nextBuilding.storage.capacity - oldBuilding.storage.capacity);
				}
				break;
			case StructureInfoProto.StructType.HOSPITAL:
				qualities.text = "Queue Size:\n\nRate:";
				bottomBar.SetActive(true);
				SetBar(topBarCurrent, topBarFuture, oldBuilding.hospital.queueSize, nextBuilding.hospital.queueSize, max.hospital.queueSize);
				SetBar(botBarCurrent, botBarFuture, oldBuilding.hospital.healthPerSecond, nextBuilding.hospital.healthPerSecond, max.hospital.healthPerSecond);
				if (nextBuilding.hospital.queueSize > oldBuilding.hospital.queueSize)
				{
					topBarText.text = oldBuilding.hospital.queueSize + " + " 
						+ (nextBuilding.hospital.queueSize - oldBuilding.hospital.queueSize);
				}
				else
				{
					topBarText.text = nextBuilding.hospital.queueSize.ToString();
				}
				if (nextBuilding.hospital.healthPerSecond > oldBuilding.hospital.healthPerSecond)
				{
					topBarText.text = oldBuilding.hospital.healthPerSecond + " + " 
						+ (nextBuilding.hospital.healthPerSecond - oldBuilding.hospital.healthPerSecond) + " Health Per Sec";
				}
				else
				{
					topBarText.text = oldBuilding.hospital.healthPerSecond + " Health Per Sec";
				}
				break;
			case StructureInfoProto.StructType.LAB:
				qualities.text = "Queue Size:\n\nRate:";
				bottomBar.SetActive(true);SetBar(topBarCurrent, topBarFuture, oldBuilding.lab.queueSize, nextBuilding.lab.queueSize, max.lab.queueSize);
				SetBar(botBarCurrent, botBarFuture, oldBuilding.lab.pointsPerSecond, nextBuilding.lab.pointsPerSecond, max.lab.pointsPerSecond);
				if (nextBuilding.lab.queueSize > oldBuilding.lab.queueSize)
				{
					topBarText.text = oldBuilding.lab.queueSize + " + " 
						+ (nextBuilding.lab.queueSize - oldBuilding.lab.queueSize);
				}
				else
				{
					topBarText.text = nextBuilding.lab.queueSize.ToString ();
				}
				if (nextBuilding.lab.pointsPerSecond > oldBuilding.lab.pointsPerSecond)
				{
					topBarText.text = oldBuilding.lab.pointsPerSecond + " + " 
						+ (nextBuilding.lab.pointsPerSecond - oldBuilding.lab.pointsPerSecond) + " Points Per Sec";
				}
				else
				{
					topBarText.text = oldBuilding.lab.pointsPerSecond + " Points Per Sec";
				}
				break;
			case StructureInfoProto.StructType.RESIDENCE:
				qualities.text = "Slots:";
				bottomBar.SetActive(false);
				SetBar(topBarCurrent, topBarFuture, oldBuilding.residence.numMonsterSlots, nextBuilding.residence.numMonsterSlots, max.residence.numMonsterSlots);
				topBarText.text = oldBuilding.residence.numMonsterSlots + " + " + nextBuilding.residence.numMonsterSlots;
				break;
			case StructureInfoProto.StructType.TOWN_HALL:
				qualities.text = "City Level:";
				bottomBar.SetActive(false);
				SetBar (topBarCurrent, topBarFuture, oldBuilding.structInfo.level, nextBuilding.structInfo.level, max.structInfo.level);
				topBarText.text = oldBuilding.structInfo.level + " + " + nextBuilding.structInfo.level;
				break;
			}

			upgradeButton.onClick = TryToBuy;
			upgradeButton.button.enabled = true;
		}
	}

	void SetBar(CBKFillBar currBar, CBKFillBar nextBar, float curr, float next, float max)
	{
		currBar.fill = curr/max;
		nextBar.fill = next/max;
	}
	
	IEnumerator UpdateRemainingTime()
	{
		while(!currBuilding.userStructProto.isComplete)
		{
			upgradeTime.text = CBKUtil.TimeStringMed(currBuilding.upgrade.timeRemaining);
			currCost = currBuilding.upgrade.gemsToFinish;
			upgradeButton.label.text = currCost.ToString();
			yield return new WaitForSeconds(1);
		}
		Init(currBuilding);
	}
	
}
