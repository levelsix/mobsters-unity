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
	UILabel cost;

	[SerializeField]
	UILabel description;

	[SerializeField]
	UI2DSprite buildingSprite;

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

	bool isFlipping = false;

	bool on = true;

	UIButton button;

	MSFullBuildingProto building;

	const string FRONT_IMAGE = "buildinginfobg";
	const string BACK_IMAGE = "buildingbg";

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
		timeToBuild.text = "TIME\n(T) " + proto.structInfo.minutesToBuild + "m";

		description.text = proto.structInfo.description;

		buildingSprite.sprite2D = MSSpriteUtil.instance.GetBuildingSprite(proto.structInfo.imgName);
		if (buildingSprite.sprite2D != null)
		{
			buildingSprite.width = (int)buildingSprite.sprite2D.rect.width;
			buildingSprite.height = (int)buildingSprite.sprite2D.rect.height;
		}

		switch(proto.structInfo.buildResourceType)
		{
			case ResourceType.CASH:
				cost.text = "$" + proto.structInfo.buildCost;
				break;
			case ResourceType.OIL:
				cost.text = "(o) " + proto.structInfo.buildCost;
				break;
			case ResourceType.GEMS:
				cost.text = "(G)" + proto.structInfo.buildCost;
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
					max = MSBuildingManager.townHall.combinedProto.townHall.numResourceTwoGenerators;
					break;
				}
				break;
			case StructureInfoProto.StructType.RESIDENCE:
				max = MSBuildingManager.townHall.combinedProto.townHall.numResidences;
				break;
			}
		}
		builtCount.text = "Built " + count + "/" + max;

		if (count >= max)
		{
			on = false;
			button.defaultColor = button.disabledColor;
			button.hover = button.disabledColor;
			Tint(button.disabledColor);
		}
		else
		{
			on = true;
			button.defaultColor = Color.white;
			button.hover = Color.white;
			Tint(Color.white);
		}
	}

	void SetName()
	{
		switch (building.structInfo.structType) 
		{
		case StructureInfoProto.StructType.RESOURCE_GENERATOR:
			if (building.generator.resourceType == ResourceType.CASH)
			{
				name = "1 Cash Printer";
			}
			else
			{
				name = "3 Oil Drill";
			}
			break;
		case StructureInfoProto.StructType.RESOURCE_STORAGE:
			if (building.storage.resourceType == ResourceType.CASH)
			{
				name = "2 Cash Vault";
			}
			else
			{
				name = "4 Oil Storage";
			}
			break;
		case StructureInfoProto.StructType.HOSPITAL:
			name = "6 Hospital";
			break;
		case StructureInfoProto.StructType.RESIDENCE:
			name = "7 Residence";
			break;
		default:
			name = "8 Lab";
			break;
		}
	}

	void Tint(Color tint)
	{
		foreach (var item in tintSprites) 
		{
			item.color = tint;
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
		Debug.Log("Clicked!");
		if (flipped)
		{
			Flip();
		}
		else if (on)
		{
			BuyBuilding();
		}
	}

	public void BuyBuilding()
	{
		MSBuildingManager.instance.MakeHoverBuilding(building);
		MSActionManager.Popup.CloseAllPopups();
	}
}
