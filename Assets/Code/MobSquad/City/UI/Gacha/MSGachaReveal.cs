using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using System.Collections.Generic;

public class MSGachaReveal : MonoBehaviour {
	
	[SerializeField]
	MSGachaFeaturedMobster mobsterPrefab;
	
	[SerializeField]
	UIGrid mobsterGrid;
	
	List<MSGachaFeaturedMobster> mobsters = new List<MSGachaFeaturedMobster>();
	
	List<BoosterItemProto> allPrizes = new List<BoosterItemProto>();
	
	int nextLeftIndex;
	int nextRightIndex;
	
	public void Init(BoosterItemProto prize)
	{
		RecycleMobsters();
		allPrizes.Clear();
		AddMobster(prize);
		allPrizes.Add(prize);
		mobsterGrid.Reposition();
	}
	
	public void Init(List<BoosterItemProto> prizes)
	{
		RecycleMobsters();
		allPrizes.Clear();
		foreach (var item in prizes) 
		{
			allPrizes.Add (item);
		}
		
		int i;
		for (i = 0; i < prizes.Count && i < 4; i++) 
		{
			AddMobster(prizes[i]);
		}
		nextLeftIndex = LoopDisplayItemIndex(-1);
		nextRightIndex = LoopDisplayItemIndex(i);
		mobsterGrid.Reposition();
	}
	
	void RecycleMobsters()
	{
		foreach (var item in mobsters) {
			item.GetComponent<MSSimplePoolable>().Pool();
		}
		mobsters.Clear();
	}
	
	void AddMobster(BoosterItemProto prize)
	{
		MSGachaFeaturedMobster mobster = (MSPoolManager.instance.Get(mobsterPrefab.GetComponent<MSSimplePoolable>(),
		                                                             Vector3.zero,
		                                                             mobsterGrid.transform) as MSSimplePoolable)
			.GetComponent<MSGachaFeaturedMobster>();
		mobsters.Add (mobster);
		mobster.transform.localScale = Vector3.one;
		mobster.Init(prize);
		mobster.gachaReveal = this;
	}
	
	public BoosterItemProto PickGoonLeft()
	{
		int index = nextLeftIndex;
		nextLeftIndex = LoopDisplayItemIndex(--nextLeftIndex);
		nextRightIndex = LoopDisplayItemIndex(--nextRightIndex);
		return allPrizes[index];
	}
	
	public BoosterItemProto PickGoonRight()
	{
		int index = nextRightIndex;
		nextLeftIndex = LoopDisplayItemIndex(++nextLeftIndex);
		nextRightIndex = LoopDisplayItemIndex(++nextRightIndex);
		return allPrizes[index];
	}
	
	public int LoopDisplayItemIndex(int index)
	{
		while (index >= allPrizes.Count)
		{
			index -= allPrizes.Count;
		}
		while (index < 0)
		{
			index += allPrizes.Count;	
		}
		return index;
	}
}
