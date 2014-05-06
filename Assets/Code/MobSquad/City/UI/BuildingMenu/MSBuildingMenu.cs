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

	MSBuildingCard lastCard
	{
		get
		{
			return cards[cards.Count - 1];
		}
	}

	void OnEnable()
	{
		Init();
	}

	void Init()
	{
		int index = 0;
		foreach (MSFullBuildingProto proto in MSDataManager.instance.GetAll(typeof(MSFullBuildingProto)).Values) 
		{
			if (proto.structInfo.level == 1 && proto.structInfo.structType != StructureInfoProto.StructType.TOWN_HALL)
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

		table.collider.enabled = false;
		table.collider.enabled = true;
	}

	void AddCard()
	{
		MSBuildingCard card = Instantiate(buildingCardPrefab) as MSBuildingCard;
		card.trans.parent = lastCard.trans.parent;
		card.trans.localScale = Vector3.one;
		card.GetComponent<MSUIHelper>().dragBehind = lastCard.GetComponent<MSUIHelper>().dragBehind;
		cards.Add (card);
	}
}
