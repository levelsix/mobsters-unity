using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSMobsterGrid : MonoBehaviour {

	[SerializeField] MSGoonCard goonCardPrefab;

	[SerializeField] UIGrid grid;

	[SerializeField] MSUIHelper noMobstersLabel;

	[HideInInspector]
	public List<MSGoonCard> cards = new List<MSGoonCard>();

	[SerializeField] bool evoReady = false;

	GoonScreenMode mode;

	public int Count
	{
		get
		{
			return cards.Count;
		}
	}

	public void Init(GoonScreenMode mode)
	{
		this.mode = mode;
		RecycleCards();
		foreach (var mobster in MSMonsterManager.instance.userMonsters) 
		{
			if (ShouldGoonBeAdded(mode, mobster))
			{
				AddCard(mobster, mode);
			}
		}
		grid.animateSmoothly = false;
		Reposition();
		grid.animateSmoothly = true;
	}

	bool ShouldGoonBeAdded(GoonScreenMode mode, PZMonster mobster)
	{
		switch(mode)
		{
		case GoonScreenMode.HEAL:
			return mobster.monsterStatus == MonsterStatus.INJURED || mobster.monsterStatus == MonsterStatus.HEALING;
		case GoonScreenMode.SELL:
			return mobster.monsterStatus == MonsterStatus.HEALTHY || mobster.monsterStatus == MonsterStatus.INJURED
				|| mobster.monsterStatus == MonsterStatus.INCOMPLETE;
		case GoonScreenMode.DO_ENHANCE:	
			return mobster != MSMonsterManager.instance.currentEnhancementMonster &&
				(mobster.monsterStatus == MonsterStatus.HEALTHY 
				 || mobster.monsterStatus == MonsterStatus.INJURED
				 || mobster.monsterStatus == MonsterStatus.ENHANCING);
		case GoonScreenMode.TEAM:
			return true;
		case GoonScreenMode.PICK_ENHANCE:
			return mobster.monsterStatus == MonsterStatus.HEALTHY || mobster.monsterStatus == MonsterStatus.INJURED;
		case GoonScreenMode.PICK_EVOLVE:
			if (IsMonsterEvoBuddy(mobster))
			{
				return false;
			}
			return mobster.monster.evolutionMonsterId > 0 
				&& (mobster.monsterStatus == MonsterStatus.HEALTHY || mobster.monsterStatus == MonsterStatus.INJURED);
		default:
			return mobster.monsterStatus == MonsterStatus.HEALTHY || mobster.monsterStatus == MonsterStatus.INJURED;
		}
	}

	void RecycleCards()
	{
		foreach(var card in cards)
		{
			card.Pool();
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
			Reposition();
		}
	}

	public void PhaseOutCard(MSGoonCard card)
	{
		StartCoroutine(card.PhaseOut());
		cards.Remove(card);
		Reposition();
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
		SetSort();
		grid.Reposition();
		if (noMobstersLabel != null)
		{
			if (grid.animateSmoothly)
			{
				noMobstersLabel.Fade(grid.transform.childCount == 0);
			}
			else
			{
				noMobstersLabel.ResetAlpha(grid.transform.childCount == 0);
			}
		}
	}

	void SetSort()
	{
		switch (mode) {
		case GoonScreenMode.SELL:
			grid.sorting = UIGrid.Sorting.Custom;
			grid.onCustomSort = ReverseCompareSellPrices;
			break;
		case GoonScreenMode.DO_ENHANCE:
			grid.sorting = UIGrid.Sorting.Custom;
			grid.onCustomSort = ReverseCompareExp;
			break;
		default:
			grid.sorting = UIGrid.Sorting.Alphabetic;
			break;
		}
	}

	bool IsMonsterEvoBuddy(PZMonster mon)
	{
		foreach (var item in cards) 
		{
			if (item.buddy != null && item.buddy.monster == mon)
			{
				return true;
			}
		}
		return false;
	}

	int ReverseCompareSellPrices(Transform a, Transform b){return a.GetComponent<MSGoonCard>().monster.sellValue.CompareTo(b.GetComponent<MSGoonCard>().monster.sellValue) * -1;}
	int ReverseCompareExp(Transform a, Transform b){return a.GetComponent<MSGoonCard>().monster.enhanceXP.CompareTo(b.GetComponent<MSGoonCard>().monster.enhanceXP) * -1;}
}
