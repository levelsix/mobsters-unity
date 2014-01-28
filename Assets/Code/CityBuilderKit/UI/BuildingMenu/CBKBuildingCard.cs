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

	[SerializeField]
	Color tintColor;

	[SerializeField]
	public CBKActionButton buyButton;

	CBKCombinedBuildingProto building;

	[HideInInspector]
	public Transform trans;

	void Awake()
	{
		trans = transform;
	}

	public void Init(CBKCombinedBuildingProto proto)
	{
		building = proto;

		nameLabel.text = proto.structInfo.name;
		timeToBuild.text = "TIME " + proto.structInfo.minutesToBuild + "m";

		buildingSprite.sprite2D = CBKAtlasUtil.instance.GetBuildingSprite(proto.structInfo.imgName);
		buildingSprite.width = (int)buildingSprite.sprite2D.rect.width;
		buildingSprite.height = (int)buildingSprite.sprite2D.rect.height;

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

		buyButton.onClick += BuyBuilding;
	}

	void DetermineCount()
	{
		int count = CBKBuildingManager.instance.GetBuildingTypeCount(building.structInfo.structType, building.structInfo.buildResourceType);
		int max = 0;
		if (CBKBuildingManager.townHall != null)
		{
			switch(building.structInfo.structType)
			{
			case StructureInfoProto.StructType.HOSPITAL:
				max = CBKBuildingManager.townHall.combinedProto.townHall.numHospitals;
				break;
			case StructureInfoProto.StructType.RESOURCE_GENERATOR:
				switch(building.generator.resourceType)
				{
				case ResourceType.CASH:
					max = CBKBuildingManager.townHall.combinedProto.townHall.numResourceOneGenerators;
					break;
				case ResourceType.OIL:
					max = CBKBuildingManager.townHall.combinedProto.townHall.numResourceTwoGenerators;
					break;
				}
				break;
			case StructureInfoProto.StructType.RESOURCE_STORAGE:
				switch(building.storage.resourceType)
				{
				case ResourceType.CASH:
					max = CBKBuildingManager.townHall.combinedProto.townHall.numResourceOneStorages;
					break;
				case ResourceType.OIL:
					max = CBKBuildingManager.townHall.combinedProto.townHall.numResourceTwoGenerators;
					break;
				}
				break;
			case StructureInfoProto.StructType.RESIDENCE:
				max = CBKBuildingManager.townHall.combinedProto.townHall.numResidences;
				break;
			}
		}
		builtCount.text = "Built " + count + "/" + max;

		if (count >= max)
		{
			Tint(tintColor);
			buyButton.able = false;
		}
		else
		{
			Tint(Color.white);
			buyButton.able = true;
		}
	}

	void Tint(Color tint)
	{
		foreach (var item in tintSprites) 
		{
			item.color = tint;
		}
	}

	void BuyBuilding()
	{
		if (CBKBuildingManager.instance.BuyBuilding(building.structInfo))
		{
			CBKEventManager.Popup.CloseAllPopups();
		}
	}
}
