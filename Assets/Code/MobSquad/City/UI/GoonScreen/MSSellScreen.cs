using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSSellScreen : MSFunctionalScreen {

	public static MSSellScreen instance;

	public MSMobsterGrid grid;

	public UIGrid sellQueue;

	[SerializeField]
	MSUIHelper emptyQueueRoot;

	[SerializeField]
	MSUIHelper queueRoot;

	[SerializeField]
	UILabel currValue;

	[SerializeField]
	MSGoonScreen goonScreen;

	[SerializeField]
	MSLoadLock sellLock;

	public List<MSGoonCard> currSells = new List<MSGoonCard>();
	
	public override bool IsAvailable()
	{
		return true;
	}

	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		MSActionManager.Goon.OnFinishSelling += UnlockAfterSellResponse;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnFinishSelling -= UnlockAfterSellResponse;
	}

	public override void Init()
	{
		grid.Init(GoonScreenMode.SELL);
		currSells.Clear();

		queueRoot.ResetAlpha(false);
		emptyQueueRoot.ResetAlpha(true);

		SetHeaderText();
	}

	void SetHeaderText()
	{
		goonScreen.title = "SELL TOONS (" + MSMonsterManager.instance.userMonsters.Count + "/"
			+ MSMonsterManager.instance.totalResidenceSlots + ")";
	}

	public void Add(MSGoonCard card)
	{
		if (currSells.Count == 0)
		{
			emptyQueueRoot.FadeOut();
			queueRoot.FadeIn();
		}

		currSells.Add(card);
		RefreshValue();
	}

	public void Remove(MSGoonCard card)
	{
		currSells.Remove(card);

		if (currSells.Count == 0)
		{
			emptyQueueRoot.FadeIn();
			queueRoot.FadeOut();
		}
		else
		{
			RefreshValue();
		}
	}

	void RefreshValue()
	{
		int total = 0;
		foreach (var item in currSells) 
		{
			total += item.monster.sellValue;
		}
		currValue.text = "$" + total;
	}

	public void Sell()
	{
		List<PZMonster> monsters = new List<PZMonster>();
		foreach (var item in currSells) 
		{
			monsters.Add(item.monster);
		}
		sellLock.Lock();
		MSMonsterManager.instance.SellRequest(monsters);
	}

	void UnlockAfterSellResponse()
	{
		List<PZMonster> monsters = new List<PZMonster>();
		foreach (var item in currSells) 
		{
			monsters.Add(item.monster);
			grid.PhaseOutCard(item);
		}
		MSMonsterManager.instance.SellMonsters(monsters);
		currSells.Clear();
		sellLock.Unlock();
		SetHeaderText();

		emptyQueueRoot.FadeIn();
		queueRoot.FadeOut();
	}

}
