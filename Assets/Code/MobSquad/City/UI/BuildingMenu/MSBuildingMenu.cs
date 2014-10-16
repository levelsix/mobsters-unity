using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSBuildingMenu : MonoBehaviour {

	public static MSBuildingMenu instance;

//	[SerializeField]
//	GameObject midDivider;

	[SerializeField]
	GameObject endDivider;

	[SerializeField]
	MSBuildingCard buildingCardPrefab;

	[SerializeField]
	UITable table;
	
	public List<MSBuildingCard> cards;

	const string DIVIDER_NAME = "space blocker";

	void Awake()
	{
		instance = this;
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
			if (proto.structInfo.level == 1 &&
			    proto.structInfo.structType != StructureInfoProto.StructType.TOWN_HALL && 
			    proto.structInfo.structType != StructureInfoProto.StructType.TEAM_CENTER &&
			    proto.structInfo.structType != StructureInfoProto.StructType.LAB)
			{
				if (cards.Count <= index)
				{
					AddCard();
				}
				cards[index].Init(proto);
				index++;
			}
		}
		cards = SortCards();
		table.Reposition();
	}

	void AddCard()
	{
		MSBuildingCard card = Instantiate(buildingCardPrefab) as MSBuildingCard;
		card.trans.parent = table.transform;
		card.trans.localScale = Vector3.one;
		cards.Add (card);
	}

	/// <summary>
	/// Returns a list of the building cards sorted
	/// </summary>
	List<MSBuildingCard> SortCards()
	{
		///Goal of this sort:
		/// Available cards infront of unavailable cards
		/// Available cards are sorted by struct ID with labs and evo chamber in front
		/// unavailable cards are sorted by lowest command level and struct level
		/// 
		/// cards are sorted in the table alphabetically so the game object
		/// names will start with the index number from the list

		List<MSBuildingCard> onCards = new List<MSBuildingCard>();
		List<MSBuildingCard> offCards = new List<MSBuildingCard>();

		MSBuildingCard lab = null;
		MSBuildingCard evo = null;

		//this increments to put a different number in front of the name of each card
		//we start at 1 because 0 is reserved for the first divider
		int nameIndex = 1;

		foreach(MSBuildingCard card in cards)
		{
			//this restores the name of the card before it the number in front
			card.SetName();
			if(card.on)
			{
				//if the card is available and for a lab or evo chamber save it so we can put it in the front.
				if(card.building.lab != null)
				{
					lab = card;
				}
				else if(card.building.evoChamber != null)
				{
					evo = card;
				}
				else
				{
					onCards.Add(card);
				}
			}
			else
			{
				offCards.Add(card);
			}
		}

		//sort both lists by struct ID
		onCards.Sort((x,y)=>x.building.structInfo.structId.CompareTo(y.building.structInfo.structId));
		offCards.Sort((x,y)=>x.building.structInfo.structId.CompareTo(y.building.structInfo.structId));

		//bucket sort offCards so it is sorted into blocks of the same command center levels
		//make a bucket for each possible townhall level
		List<MSBuildingCard>[] buckets = new List<MSBuildingCard>[MSBuildingManager.townHall.combinedProto.maxLevel.structInfo.level];
		for(int i = 0; i < buckets.Length; i++)
		{
			buckets[i] = new List<MSBuildingCard>();
		}

		//put each card into a bucket that matches the level of town hall required
		for(int i = 0; i < onCards.Count; i++)
		{
			//I'm not 100% on the accuracy of prerequisiteTownHallLvl.
			buckets[onCards[i].building.structInfo.prerequisiteTownHallLvl].Add( onCards[i]);
		}

		//put the bucket sorted cards back into onCards
		onCards.Clear();
		foreach(List<MSBuildingCard> bucket in buckets)
		{
			if(bucket.Count > 0)
			{
				onCards.AddRange(bucket);
			}
		}

		//insert lab and evo so onCards starts [lab, evo, <others>] and name them to be in order
		if(evo != null)
		{
			evo.name = nameIndex + evo.name;
			nameIndex++;
			onCards.Insert(0, evo);
		}
		if(lab != null)
		{
			lab.name = nameIndex + lab.name;
			nameIndex++;
			onCards.Insert(0, lab);
		}

		//at this point only the 'On' cards are in the list still, so let's number
		//them and name the divider
		foreach(MSBuildingCard card in onCards)
		{
			card.name = nameIndex + card.name;
			nameIndex++;
		}

//		midDivider.name = nameIndex + DIVIDER_NAME;
//		nameIndex++;

		//now name the offcards
		foreach(MSBuildingCard card in offCards)
		{
			card.name = nameIndex + card.name;
			nameIndex++;
		}

		endDivider.name = nameIndex + DIVIDER_NAME;

		onCards.AddRange(offCards);
		return onCards;
	}
}
