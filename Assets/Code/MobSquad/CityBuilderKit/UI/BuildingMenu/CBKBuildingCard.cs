using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKBuildingCard : MonoBehaviour {

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel timeToBuild;

	[SerializeField]
	UILabel builtCount;

	[SerializeField]
	UILabel cost;

	[SerializeField]
	UI2DSprite buildingSprite;

	[SerializeField]
	UIWidget[] tintSprites;

	public CBKActionButton actionButton;

	UIButton button;

	CBKCombinedBuildingProto building;

	[HideInInspector]
	public Transform trans;

	void Awake()
	{
		trans = transform;
		button = GetComponent<UIButton>();
	}

	public void Init(CBKCombinedBuildingProto proto)
	{
		building = proto;

		nameLabel.text = proto.structInfo.name;
		timeToBuild.text = "TIME " + proto.structInfo.minutesToBuild + "m";

		buildingSprite.sprite2D = MSAtlasUtil.instance.GetBuildingSprite(proto.structInfo.imgName);
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
				cost.text = "(O)" + proto.structInfo.buildCost;
				break;
			case ResourceType.GEMS:
				cost.text = "(G)" + proto.structInfo.buildCost;
				break;
		}

		DetermineCount();
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
			button.isEnabled = false;
			Tint(button.disabledColor);
		}
		else
		{
			button.isEnabled = true;
			Tint(Color.white);
		}
	}

	void Tint(Color tint)
	{
		foreach (var item in tintSprites) 
		{
			item.color = tint;
		}
	}

	public void BuyBuilding()
	{
		MSBuildingManager.instance.MakeHoverBuilding(building);
		MSActionManager.Popup.CloseAllPopups();
	}
}
