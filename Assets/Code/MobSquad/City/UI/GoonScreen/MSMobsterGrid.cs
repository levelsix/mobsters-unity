using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSMobsterGrid : MonoBehaviour {

	[SerializeField] MSGoonCard goonCardPrefab;

	[SerializeField] UIGrid grid;

	List<MSGoonCard> cards = new List<MSGoonCard>();

	public void Init(GoonScreenMode mode)
	{
		RecycleCards();
		foreach (var mobster in MSMonsterManager.instance.userMonsters) 
		{
			if (ShouldGoonBeAdded(mode, mobster))
			{
				AddCard(mobster, mode);
			}
		}
		grid.Reposition();
	}

	bool ShouldGoonBeAdded(GoonScreenMode mode, PZMonster mobster)
	{
		switch(mode)
		{
		case GoonScreenMode.HEAL:
			return mobster.monsterStatus == MonsterStatus.INJURED;
		case GoonScreenMode.SELL:
			return mobster.monsterStatus == MonsterStatus.HEALTHY || mobster.monsterStatus == MonsterStatus.INJURED
				|| mobster.monsterStatus == MonsterStatus.INCOMPLETE;
		case GoonScreenMode.TEAM:
			return mobster.monsterStatus == MonsterStatus.HEALTHY || mobster.monsterStatus == MonsterStatus.INJURED;
		default:
			return mobster.monsterStatus == MonsterStatus.HEALTHY || mobster.monsterStatus == MonsterStatus.INJURED;
		}
	}

	void RecycleCards()
	{
		foreach(var card in cards)
		{
			card.GetComponent<MSSimplePoolable>().Pool();
		}
		cards.Clear();
	}

	public void AddCard(PZMonster mobster, GoonScreenMode mode, bool reposition = false)
	{
		MSGoonCard card = (MSPoolManager.instance.Get(goonCardPrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, grid.transform) as MSSimplePoolable).GetComponent<MSGoonCard>();
		card.transform.localScale = Vector3.one;
		card.Init(mobster, mode);
		cards.Add (card);

		if (reposition)
		{
			grid.Reposition();
		}
	}

	public void PhaseOutCard(MSGoonCard card)
	{
		StartCoroutine(card.PhaseOut());
		cards.Remove(card);
		grid.Reposition();
	}

	public void PhaseOutCard(PZMonster monster)
	{
		MSGoonCard card = cards.Find(x=>x.monster == monster);
		if (card != null)
		{
			PhaseOutCard(card);
		}
	}

	public void Reposition()
	{
		grid.Reposition();
	}
}
