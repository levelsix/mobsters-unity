using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKBuildingMenu : MonoBehaviour {

	[SerializeField]
	CBKBuildingCard buildingCardProto;

	[SerializeField]
	UIGrid grid;

	[SerializeField]
	List<CBKBuildingCard> cards;

	CBKBuildingCard lastCard
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
		foreach (CBKCombinedBuildingProto proto in MSDataManager.instance.GetAll(typeof(CBKCombinedBuildingProto)).Values) 
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
		grid.Reposition();

		grid.collider.enabled = false;
		grid.collider.enabled = true;
	}

	void AddCard()
	{
		CBKBuildingCard card = Instantiate(buildingCardProto) as CBKBuildingCard;
		card.trans.parent = lastCard.trans.parent;
		card.trans.localScale = Vector3.one;
		card.actionButton.dragBehind = lastCard.actionButton.dragBehind;
		cards.Add (card);
	}
}
