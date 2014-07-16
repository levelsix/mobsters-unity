using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSBuildingMenu : MonoBehaviour {

	[SerializeField]
	MSBuildingCard buildingCardPrefab;

	[SerializeField]
	UITable table;

	[SerializeField]
	List<MSBuildingCard> cards;

	void OnEnable()
	{
		Init();
	}

	void Init()
	{
		int index = 0;
		foreach (MSFullBuildingProto proto in MSDataManager.instance.GetAll(typeof(MSFullBuildingProto)).Values) 
		{
			if (proto.structInfo.level == 1 &&
			    proto.structInfo.structType != StructureInfoProto.StructType.TOWN_HALL && 
			    proto.structInfo.structType != StructureInfoProto.StructType.TEAM_CENTER &&
			    proto.structInfo.structType != StructureInfoProto.StructType.MINI_JOB)
			{
				if (cards.Count <= index)
				{
					AddCard();
				}
				cards[index].Init(proto);
				index++;
			}
		}

		table.Reposition();
	}

	void AddCard()
	{
		MSBuildingCard card = Instantiate(buildingCardPrefab) as MSBuildingCard;
		card.trans.parent = table.transform;
		card.trans.localScale = Vector3.one;
		cards.Add (card);
	}
}
