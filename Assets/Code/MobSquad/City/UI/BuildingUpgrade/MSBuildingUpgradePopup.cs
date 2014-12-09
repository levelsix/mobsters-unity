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

	public static MSBuildingUpgradePopup instance;

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
	UIGrid grid;

	[SerializeField]
	Transform townHallUpgradeUI;

	[SerializeField]
	UILabel townHallLabel;

	[SerializeField]
	MSSimplePoolable unlockTile;

	[SerializeField]
	MSLoadLock loadLock;

	#region Upgrade Requirements

	[SerializeField] Color disabledCostColor;

	[SerializeField] UISprite bottom;
	const string GREY_BOTTOM = "upgradepopupbottom";
	const string RED_BOTTOM = "upgradepopupbottomred";

	[SerializeField]
	MSBuildingPrereqEntry[] prereqs;

	[SerializeField] UILabel bottomTopText;
	[SerializeField] UILabel bottomBotText;
	[SerializeField] Color topOkayColor;
	const string TOP_OKAY_TEXT = "Ready to Upgrade!";
	const string BOT_OKAY_TEXT = "You have all the requirements to upgrade.";
	const string TOP_BAD_TEXT = "Whoops!";

	[SerializeField] UISprite readySymbol;
	const string CHECK_SYMBOL = "readyforupgradebigcheck";
	const string WARNING_SYMBOL = "woopsalertsign";

	#endregion

	List<MSHireEntry> hireEntries = new List<MSHireEntry>();

	const string cashButtonName = "greenmenuoption";
	const string oilButtonName = "yellowmenuoption";

	static readonly Color cashTextColor = new Color(.353f, .491f, .027f);
	static readonly Color oilTextColor = new Color(.776f, .533f, 0);

	const int MAX_IMAGE_WIDTH = 220;
	const int MAX_IMAGE_HEIGHT = 220;
	
	MSBuilding currBuilding;
	
	ResourceType currResource;
	int currCost;

	bool firstFail = true;

	int tempSpend = 0;
	int tempSpendGems = 0;

	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		firstFail = true;
		foreach( MSSimplePoolable poolable in grid.GetAllComponentsInChildren<MSSimplePoolable>())
		{
			if(poolable.gameObject.activeSelf)
			{
				poolable.Pool();
			}
		}
	}

	void OnDisable()
	{
		bottomBar.transform.parent.gameObject.SetActive(true);
		townHallUpgradeUI.gameObject.SetActive(false);
	}

	IEnumerator WaitUntilFinish()
	{
		MSBuildingManager.instance.currentUnderConstruction.CompleteWithGems();
		while (MSBuildingManager.instance.currentUnderConstruction != null)
		{
			yield return null;
		}
		MSActionManager.Popup.CloseTopPopupLayer();
		TryToBuy();
	}
	
	void TryToBuy(bool useGems = false)
	{
		Debug.LogWarning("Trying to buy");

		if (MSBuildingManager.instance.currentUnderConstruction != null)
		{
			MSPopupManager.instance.CreatePopup("Your builder is busy!",
			                                    "Speed him up for (G) " + 
			                                        MSMath.GemsForTime(MSBuildingManager.instance.currentUnderConstruction.completeTime, true)
			                                        + " and upgrade this building?",
                new string[]{"Cancel", "Speed Up"},
				new string[]{"greymenuoption", "purplemenuoption"},
				new WaitFunction[]{MSUtil.QuickCloseTop, WaitUntilFinish},
				"purple"
			);
			return;
		}

		if (useGems)
		{
			int gemCost = Mathf.CeilToInt((currCost - MSResourceManager.resources[currResource]) * MSWhiteboard.constants.gemsPerResource);
			//int gemResources = gemCost / MSWhiteboard.constants.gemsPerResource;
			//int remainingCost = currCost - gemResources;
			if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost))
			{
				Buy (MSResourceManager.instance.SpendAll(currResource), gemCost);
			}
		}
		else if (MSResourceManager.instance.Spend(currResource, currCost, delegate{TryToBuy(true);}))
		{
			Buy (currCost);
		}
	}

	void Buy(int baseResource, int gems = 0)
	{
		currBuilding.upgrade.StartUpgrade(baseResource, gems);
		loadLock.Lock();
		tempSpend = baseResource;
		tempSpendGems = gems;
	}

	public void UnlockAndClose()
	{
		loadLock.Unlock();

		if (MSActionManager.Popup.CloseAllPopups != null)
		{
			MSActionManager.Popup.CloseAllPopups();
		}
		
		MSBuildingManager.instance.FullDeselect();
		if (!MSTutorialManager.instance.inTutorial)
		{
			currBuilding.Select();
		}
	}

	public void UnlockServerFail()
	{
		loadLock.Unlock();

		//Refund the money that they spent
		MSResourceManager.instance.Collect(currBuilding.combinedProto.structInfo.buildResourceType, tempSpend);
		MSResourceManager.instance.Collect(ResourceType.GEMS, tempSpendGems);

		if (firstFail)
		{
			firstFail = false;
			MSPopupManager.instance.CreatePopup("Whoops!", "Looks like there was a problem confirming your purchase. Try again?",
		            new string[] {"Okay"}, 
						new string[] {"greenmenuoption"}, 
						new Action[] {MSActionManager.Popup.CloseTopPopupLayer});
		}
		else
		{
			MSSceneManager.instance.ReconnectPopup();
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


		if (nextBuilding != null)
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
			buildingSprite.MakePixelPerfect();
			if (sprite != null)
			{
				buildingSprite.width = (int)sprite.textureRect.width;
				buildingSprite.height = (int)sprite.textureRect.height;

				if(buildingSprite.width > MAX_IMAGE_WIDTH)
				{
					float newHeight = ((float)buildingSprite.height/(float)buildingSprite.width) * (float)MAX_IMAGE_WIDTH;
					buildingSprite.width = MAX_IMAGE_WIDTH;
					buildingSprite.height = (int)newHeight;
				}
				if(buildingSprite.height > MAX_IMAGE_HEIGHT)
				{
					float newWidth = ((float)buildingSprite.width/(float)buildingSprite.height) * (float)MAX_IMAGE_HEIGHT;
					buildingSprite.height = MAX_IMAGE_HEIGHT;
					buildingSprite.width = (int)newWidth;
				}

				buildingSprite.MarkAsChanged();
			}

			buildingName.text = nextBuilding.structInfo.name;

			SetBuildingBarInfo (building, oldBuilding, nextBuilding);

			SetPrerequsites(nextBuilding);
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
				botBarText.text = oldBuilding.hospital.healthPerSecond + " + " + (nextBuilding.hospital.healthPerSecond - oldBuilding.hospital.healthPerSecond) + " Health Per Sec";
			}
			else {
				botBarText.text = oldBuilding.hospital.healthPerSecond + " Health Per Sec";
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
				botBarText.text = oldBuilding.lab.pointsPerSecond + " + " + (nextBuilding.lab.pointsPerSecond - oldBuilding.lab.pointsPerSecond) + " Points Per Sec";
			}
			else {
				botBarText.text = oldBuilding.lab.pointsPerSecond + " Points Per Sec";
			}
			break;
		case StructureInfoProto.StructType.RESIDENCE:
			topQuality.text = "Slots:";
			bottomBar.SetActive (false);
			SetBar (topBarCurrent, topBarFuture, oldBuilding.residence.numMonsterSlots, nextBuilding.residence.numMonsterSlots, max.residence.numMonsterSlots);
			topBarText.text = oldBuilding.residence.numMonsterSlots + " + " + (nextBuilding.residence.numMonsterSlots - oldBuilding.residence.numMonsterSlots);
			break;
		case StructureInfoProto.StructType.TOWN_HALL:
			InitTownHallGrid(oldBuilding, nextBuilding);
			townHallLabel.text = "Level " + nextBuilding.structInfo.level + " Command Center unlocks";
			bottomBar.transform.parent.gameObject.SetActive(false);
			break;
		case StructureInfoProto.StructType.MINI_JOB:
			bottomBar.SetActive(false);
			SetBar(topBarCurrent, topBarFuture, oldBuilding.miniJobCenter.generatedJobLimit, nextBuilding.miniJobCenter.generatedJobLimit, max.miniJobCenter.generatedJobLimit);
			topQuality.text = "MiniJobs:";
			topBarText.text = oldBuilding.miniJobCenter.generatedJobLimit + " + " + (nextBuilding.miniJobCenter.generatedJobLimit - oldBuilding.miniJobCenter.generatedJobLimit);
			break;
		}
	}

	void SetBar(MSFillBar currBar, MSFillBar nextBar, float curr, float next, float max)
	{
		currBar.fill = curr/max;
		nextBar.fill = next/max;
	}

	void InitTownHallGrid(MSFullBuildingProto oldBuilding, MSFullBuildingProto newBuilding)
	{
		TownHallProto newHall = newBuilding.townHall;
		TownHallProto oldHall = oldBuilding.townHall;
		townHallUpgradeUI.gameObject.SetActive(true);

		foreach(MSFullBuildingProto fullBuilding in MSDataManager.instance.GetAll<MSFullBuildingProto>().Values)
		{

			StructureInfoProto building = fullBuilding.structInfo;
			if(building.prerequisiteTownHallLvl == newBuilding.structInfo.level &&
			   (building.successorStructId == 0 || MSDataManager.instance.Get<MSFullBuildingProto>(building.successorStructId).structInfo.prerequisiteTownHallLvl != newBuilding.structInfo.level ))
				// there are no more evolutions for this building OR the next upgrade requires an even higher town hall level
				// there for I am processing the highest level building of this structure type available once town hall is upgraded
			{
				int oldNum = -1;
				int newNum = -1;
				switch(building.structType)
				{
				case StructureInfoProto.StructType.EVO:
					
					if(newHall.numEvoChambers > oldHall.numEvoChambers)
					{
						newNum = newHall.numEvoChambers;
						oldNum = oldHall.numEvoChambers;
					}
					break;
				case StructureInfoProto.StructType.HOSPITAL:
					if(newHall.numHospitals > oldHall.numHospitals)
					{
						newNum = newHall.numHospitals;
						oldNum = oldHall.numHospitals;
					}
					break;
				case StructureInfoProto.StructType.LAB:

					if(newHall.numLabs > oldHall.numLabs)
					{
						newNum = newHall.numLabs;
						oldNum = oldHall.numLabs;
					}
					break;
				case StructureInfoProto.StructType.RESIDENCE:
					if(newHall.numResidences > oldHall.numResidences)
					{
						newNum = newHall.numResidences;
						oldNum = oldHall.numResidences;
					}
					break;
				case StructureInfoProto.StructType.RESOURCE_GENERATOR:
					if(newHall.numResourceOneGenerators > oldHall.numResourceOneGenerators && building.buildResourceType == ResourceType.OIL)
						//assuming that cash printer is resource 'one'
					{
						newNum = newHall.numResourceOneGenerators;
						oldNum = oldHall.numResourceOneGenerators;
					}
					else if(newHall.numResourceTwoGenerators > oldHall.numResourceTwoGenerators)
					{
						newNum = newHall.numResourceTwoGenerators;
						oldNum = oldHall.numResourceTwoGenerators;
					}
					break;
				case StructureInfoProto.StructType.RESOURCE_STORAGE:
					if(newHall.numResourceOneStorages > oldHall.numResourceOneStorages)
						//assuming that oil drills are resource 'two'
					{
						newNum = newHall.numResourceOneStorages;
						oldNum = oldHall.numResourceOneStorages;
					}
					else if(newHall.numResourceTwoStorages > oldHall.numResourceTwoStorages)
					{
						newNum = newHall.numResourceTwoStorages;
						oldNum = oldHall.numResourceTwoStorages;
					}
					break;
				//empty cases can only show new levels
				case StructureInfoProto.StructType.TEAM_CENTER:
					break;
					//minijob special case cause there's only 1
				case StructureInfoProto.StructType.MINI_JOB:
					if(newHall.structInfo.level == MSBuildingManager.instance.LowestRequiredHall(StructureInfoProto.StructType.MINI_JOB))
					{
						newNum = 1;
						oldNum = 0;
					}
					break;
					//clan special case cause there's only 1
				case StructureInfoProto.StructType.CLAN:
					if(newHall.structInfo.level == MSBuildingManager.instance.LowestRequiredHall(StructureInfoProto.StructType.CLAN))
					{
						newNum = 1;
						oldNum = 0;
					}
					break;
				//using continue insteads of break disables townhall in the listings.
				case StructureInfoProto.StructType.TOWN_HALL:
					continue;
				}

				if(oldNum == 0)
				{
					//there were none of this building before so it is brand new.
					AddTileToGrid(building, true);
					//is continue is uncommented then a tile will display that there is a new
					//building and not display a tile for the max lvl
					//continue;
				}
				else if(oldNum > 0 && newNum > 0)
				{
					//add a tile that displays how many new buildings you can build
					AddTileToGrid(building, false, newNum - oldNum);
				}

				//if we get this far than we've found a new level of a building
				AddTileToGrid(building, false);
			}
		}

		grid.Reposition();

	}

	void AddTileToGrid(StructureInfoProto building, bool isNew, int quantity = 0)
	{
		int lvl = building.level;
		MSUnlockBuildingTile newTile = MSPoolManager.instance.Get<MSUnlockBuildingTile>(unlockTile, grid.transform);
		newTile.transform.localScale = Vector3.one;
		newTile.gameObject.name = building.name;
		if(quantity > 0)
		{
			newTile.Init(quantity + "x", building);
		}
		else if(lvl > 1 && !isNew)//new lvl of buildling
		{
			newTile.Init("LVL " + lvl, building);
		}
		else//first building
		{
			newTile.Init("NEW!", building);
		}

	}

	void SetPrerequsites(MSFullBuildingProto building)
	{
		int missing = 0;

		bool preq;
		int i;
		for (i = 0; i < building.prereqs.Count; i++) 
		{
			preq = MSBuildingManager.instance.HasPrereqBuilding(building.prereqs[i]);
			prereqs[i].Init(
				building.prereqs[i].prereqGameEntityId,
				building.prereqs[i].quantity,
				preq
				);
			if (!preq) missing++;
		}
		for (; i < prereqs.Length; i++)
		{
			prereqs[i].SetEmpty();
		}

		if (missing == 0)
		{
			bottom.spriteName = GREY_BOTTOM;

			bottomTopText.text = TOP_OKAY_TEXT;
			bottomTopText.color = topOkayColor;

			bottomBotText.text = BOT_OKAY_TEXT;
			bottomBotText.color = Color.black;

			readySymbol.spriteName = CHECK_SYMBOL;

			upgradeButton.onClick = delegate{TryToBuy(false);};
			upgradeButton.button.isEnabled = true;
		}
		else
		{
			bottom.spriteName = RED_BOTTOM;

			bottomTopText.text = TOP_BAD_TEXT;
			bottomTopText.color = Color.white;

			bottomBotText.text = "You are missing " + missing + " requirement " 
				+ ((missing>1)?"s":"") + " to upgrade";
			bottomBotText.color = Color.white;

			readySymbol.spriteName = WARNING_SYMBOL;

			upgradeButton.label.color = disabledCostColor;
			upgradeButton.onClick = null;
			upgradeButton.button.isEnabled = false;
		}
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
