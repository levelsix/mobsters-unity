using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSBuildingCard : MonoBehaviour {

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel timeToBuild;

	[SerializeField]
	UILabel builtCount;

	[SerializeField]
	UISprite currency;

	[SerializeField]
	UILabel cost;

	[SerializeField]
	UILabel description;

	[SerializeField]
	UILabel backDescription;

	[SerializeField]
	UISprite buildingSprite;

	[SerializeField]
	UIWidget[] tintSprites;

	[SerializeField]
	UITweener[] flipTweens;

	[SerializeField]
	UISprite background;

	[SerializeField]
	GameObject front;

	[SerializeField]
	GameObject back;

	bool flipped = false;

	/// <summary>
	/// whether or not the card is available for purchase.
	/// </summary>
	public bool on{
		set{
			if(value){
				state = State.ACTIVE;
			} else{
				state = State.INACTIVE;
			}
		}

		get{
			return state == State.ACTIVE;
		}
	}

	UIButton button;

	public MSFullBuildingProto building;

	int[] townHalls;

	const string FRONT_IMAGE = "menusquareactive";
	const string BACK_IMAGE = "menusquareinactive";

	public enum State{
		ACTIVE,
		INACTIVE
	}
	
	State _state;
	
	public State state{
		get{
			return _state;
		}
		set{
			_state = value;
			
			if(value == State.ACTIVE){
				button.normalSprite = FRONT_IMAGE;
				//button.pressed = button.disabledColor;

				buildingSprite.color = Color.white;
				
				front.SetActive(true);
				back.SetActive(false);
			}else{
				button.normalSprite = BACK_IMAGE;
				//button.pressed = Color.white;

				buildingSprite.color = Color.black;
				
				front.SetActive(false);
				back.SetActive(true);
			}
		}
	}

	[HideInInspector]
	public Transform trans;

	void Awake()
	{
		trans = transform;
		button = GetComponent<UIButton>();
	}

	public void Init(MSFullBuildingProto proto)
	{
		//Reset all the flip stuff
		flipped = false;
		front.SetActive(true);
		back.SetActive(false);
		background.spriteName = FRONT_IMAGE;
		foreach (var item in flipTweens) 
		{
			item.Sample(0, true);
		}

		building = proto;

		nameLabel.text = proto.structInfo.name;
		timeToBuild.text = proto.structInfo.minutesToBuild + " m";

		description.text = proto.structInfo.shortDescription;

		buildingSprite.spriteName = proto.structInfo.imgName.Substring(0,proto.structInfo.imgName.Length - ".png".Length);
		buildingSprite.MakePixelPerfect();

		cost.text = proto.structInfo.buildCost.ToString();

		switch(proto.structInfo.buildResourceType)
		{
		case ResourceType.CASH:
			currency.spriteName = "moneysmall";
			break;
		case ResourceType.OIL:
			currency.spriteName = "oilsmall";
			break;
		case ResourceType.GEMS:
			currency.spriteName = "oilsmall";
			break;
		}

		DetermineCount();

		SetName();
	}

	void DetermineCount()
	{
		int count = MSBuildingManager.instance.GetBuildingTypeCount(building.structInfo.structType, building.structInfo.buildResourceType);
		int max = 0;
		if (MSBuildingManager.townHall != null)
		{
			switch(building.structInfo.structType)
			{
			case StructureInfoProto.StructType.HOSPITAL:
				max = MSBuildingManager.townHall.combinedProto.townHall.numHospitals;
				break;
			case StructureInfoProto.StructType.RESOURCE_GENERATOR:
				switch(building.generator.resourceType)
				{
				case ResourceType.CASH:
					max = MSBuildingManager.townHall.combinedProto.townHall.numResourceOneGenerators;
					break;
				case ResourceType.OIL:
					max = MSBuildingManager.townHall.combinedProto.townHall.numResourceTwoGenerators;
					break;
				}
				break;
			case StructureInfoProto.StructType.RESOURCE_STORAGE:
				switch(building.storage.resourceType)
				{
				case ResourceType.CASH:
					max = MSBuildingManager.townHall.combinedProto.townHall.numResourceOneStorages;
					break;
				case ResourceType.OIL:
					max = MSBuildingManager.townHall.combinedProto.townHall.numResourceTwoStorages;
					break;
				}
				break;
			case StructureInfoProto.StructType.RESIDENCE:
				max = MSBuildingManager.townHall.combinedProto.townHall.numResidences;
				break;
			case StructureInfoProto.StructType.EVO:
				max = MSBuildingManager.townHall.combinedProto.townHall.numEvoChambers;
				break;
			case StructureInfoProto.StructType.LAB:
				max = MSBuildingManager.townHall.combinedProto.townHall.numLabs;
				break;
			}
		}
		builtCount.text = count + "/" + max;

		if (count >= max)
		{
			int lowestLevelRequired = LowestRequiredHall(building.structInfo.structType, count);
			if(lowestLevelRequired != -1)
			{
				on = false;
				backDescription.text = "Requires Level " + lowestLevelRequired + " Command Center";
			}else{
				on = false;
			backDescription.text = "You Have the Max Number of This Building";
			}
		}
		else
		{
			on = true;
		}



	}

	int LowestRequiredHall(StructureInfoProto.StructType type, int currentCount){
		int lowestLevel = 999;
		MSFullBuildingProto curBuilding = MSBuildingManager.townHall.combinedProto.successor;
		while(curBuilding != null){
			TownHallProto townHall = curBuilding.townHall;
			//TODO: we may have to differentiate between Cash and Oil
			//Can't do that now cause I have no idea if cash/oil is numResourceOneGenerators or numResourceTwoGenerators
			switch(type){
			case StructureInfoProto.StructType.RESOURCE_GENERATOR:
				if(townHall.numResourceOneGenerators > currentCount && curBuilding.structInfo.level < lowestLevel){
					lowestLevel = curBuilding.structInfo.level;
				}
				break;
			case StructureInfoProto.StructType.RESOURCE_STORAGE:
				if(townHall.numResourceOneStorages > currentCount && curBuilding.structInfo.level < lowestLevel){
					lowestLevel = curBuilding.structInfo.level;
				}
				break;
			case StructureInfoProto.StructType.HOSPITAL:
				if(townHall.numHospitals > currentCount && curBuilding.structInfo.level < lowestLevel){
					lowestLevel = curBuilding.structInfo.level;
				}
				break;
			case StructureInfoProto.StructType.RESIDENCE:
				if(townHall.numResidences > currentCount && curBuilding.structInfo.level < lowestLevel){
					lowestLevel = curBuilding.structInfo.level;
				}
				break;
			case StructureInfoProto.StructType.LAB:
				if(townHall.numLabs > currentCount && curBuilding.structInfo.level < lowestLevel){
					lowestLevel = curBuilding.structInfo.level;
				}
				break;
			case StructureInfoProto.StructType.EVO:
				if(townHall.numEvoChambers > currentCount && curBuilding.structInfo.level < lowestLevel){
					lowestLevel = curBuilding.structInfo.level;
				}
				break;
			case StructureInfoProto.StructType.CLAN:
				if(currentCount > 0) lowestLevel = 1;
				break;
			default:
				Debug.LogWarning("Could not find required TownHall level for " + type.ToString());
				break;
			}
			curBuilding = curBuilding.successor;
		}
		return lowestLevel == 999 ? -1 : lowestLevel;
	}

	/// <summary>
	/// Sets the name of this gameobject to reflect which building is on the card
	/// </summary>
	public void SetName()
	{
		switch (building.structInfo.structType) 
		{
		case StructureInfoProto.StructType.RESOURCE_GENERATOR:
			if (building.generator.resourceType == ResourceType.CASH)
			{
				name = "Cash Printer";
			}
			else
			{
				name = "Oil Drill";
			}
			break;
		case StructureInfoProto.StructType.RESOURCE_STORAGE:
			if (building.storage.resourceType == ResourceType.CASH)
			{
				name = "Cash Vault";
			}
			else
			{
				name = "Oil Storage";
			}
			break;
		case StructureInfoProto.StructType.HOSPITAL:
			name = "Hospital";
			break;
		case StructureInfoProto.StructType.RESIDENCE:
			name = "Residence";
			break;
		default:
			name = "Lab";
			break;
		}
	}

	public void Flip()
	{
		StartCoroutine(DoFlip());
	}

	IEnumerator DoFlip()
	{
		foreach (var item in flipTweens) 
		{
			item.PlayForward();
		}
		foreach (var item in flipTweens) 
		{
			while (item.tweenFactor < 1)
			{
				yield return null;
			}
		}

		flipped = !flipped;
		front.SetActive(!flipped);
		back.SetActive(flipped);
		background.spriteName = flipped ? BACK_IMAGE : FRONT_IMAGE;
		
		foreach (var item in flipTweens) 
		{
			item.PlayReverse();
		}
		foreach (var item in flipTweens) 
		{
			while (item.tweenFactor > 0)
			{
				yield return null;
			}
		}
	}

	void OnClick()
	{
		if (flipped)
		{
			Flip();
		}
		else if (on)
		{
			BuyBuilding();
		}
		else if(!on){
			MSActionManager.Popup.DisplayRedError(backDescription.text);
		}
	}

	public void BuyBuilding()
	{
		BuildingPurchase();
	}

	void BuildingPurchase(){
		MSBuildingManager.instance.MakeHoverBuilding(building);
		MSActionManager.Popup.CloseAllPopups();
	}


}
