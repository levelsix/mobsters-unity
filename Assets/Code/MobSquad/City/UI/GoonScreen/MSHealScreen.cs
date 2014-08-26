using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class MSHealScreen : MSFunctionalScreen 
{
	public static MSHealScreen instance;

	public MSMobsterGrid grid;

	public UIGrid healQueue;

	[SerializeField]
	MSUIHelper emptyQueueRoot;

	[SerializeField]
	MSUIHelper queueRoot;

	[SerializeField]
	UILabel slotsLeftLabel;

	[SerializeField]
	MSUIHelper slotsLeftRoot;

	[SerializeField]
	UILabel timeLeftLabel;

	[SerializeField]
	UILabel finishNowLabel;

	[SerializeField]
	UISprite arrow;

	[SerializeField]
	MSLoadLock loadLock;

	public List<MSGoonCard> currHeals = new List<MSGoonCard>();

	const string GREEN_ARROW = "hospitalopenarrow";
	const string RED_ARROW = "fullhospitalarrow";

	void Awake()
	{
		instance = this;
	}

	public override void Init ()
	{
		currHeals.Clear();

		healQueue.animateSmoothly = false;
		grid.Init(GoonScreenMode.HEAL);
		healQueue.animateSmoothly = true;

		emptyQueueRoot.ResetAlpha(MSHospitalManager.instance.healingMonsters.Count == 0);
		queueRoot.ResetAlpha(MSHospitalManager.instance.healingMonsters.Count > 0);

		StartCoroutine(KeepRefreshing());
	}

	public void Add(MSGoonCard card)
	{
		if (currHeals.Count == 0)
		{
			emptyQueueRoot.FadeOut();
			queueRoot.FadeIn();
		}
		
		currHeals.Add(card);

		RefreshStats();
	}

	public void Remove(MSGoonCard card)
	{
		currHeals.Remove(card);
		
		if (currHeals.Count == 0)
		{
			emptyQueueRoot.FadeIn();
			queueRoot.FadeOut();
		}
		else
		{
			RefreshStats();
		}
	}

	IEnumerator KeepRefreshing()
	{
		while(true)
		{
			RefreshStats();
			yield return new WaitForSeconds(.5f);
		}
	}

	void RefreshStats()
	{
		long timeLeft = 0;
		foreach (var item in currHeals) 
		{
			timeLeft = Math.Max(item.monster.healTimeLeftMillis, timeLeft);
		}

		timeLeftLabel.text = MSUtil.TimeStringShort(timeLeft);
		finishNowLabel.text = "Finish\n(G)" + MSMath.GemsForTime(timeLeft); 
	}

	public void Finish()
	{
		MSHospitalManager.instance.TrySpeedUpHeal(loadLock);
	}

	public override bool IsAvailable ()
	{
		return MSHospitalManager.instance.hospitals.Count > 0;
	}

	void OnDisable()
	{
		MSHospitalManager.instance.DoSendHealRequest();
	}
}
