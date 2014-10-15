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
	UILabel timeLeftLabel;

	[SerializeField]
	UILabel finishNowLabel;

	[SerializeField]
	UISprite arrow;

	[SerializeField]
	MSLoadLock loadLock;

	public long timeLeft;

	public List<MSGoonCard> currHeals = new List<MSGoonCard>();

	const string GREEN_ARROW = "hospitalopenarrow";
	const string RED_ARROW = "fullhospitalarrow";

	[SerializeField]
	Color slotsOpenLabelColor;

	[SerializeField]
	Color slotsFullLabelColor;

	void Awake()
	{
		instance = this;
	}

	public override void Init ()
	{
		currHeals.Clear();

		healQueue.animateSmoothly = false;
		grid.Init(GoonScreenMode.HEAL);
		healQueue.Reposition();
		healQueue.animateSmoothly = true;

		emptyQueueRoot.ResetAlpha(MSHospitalManager.instance.healingMonsters.Count == 0);
		queueRoot.ResetAlpha(MSHospitalManager.instance.healingMonsters.Count > 0);
		
		RefreshSlots();
	}

	public void Add(MSGoonCard card)
	{
		if (currHeals.Count == 0)
		{
			emptyQueueRoot.FadeOut();
			queueRoot.FadeIn();
		}
		
		currHeals.Add(card);
		RefreshSlots();
	}

	public void Remove(MSGoonCard card)
	{
		currHeals.Remove(card);
		
		if (currHeals.Count == 0)
		{
			emptyQueueRoot.FadeIn();
			queueRoot.FadeOut();
		}
		RefreshSlots();
	}

	void RefreshSlots()
	{
		int slotsRemaining = MSHospitalManager.instance.queueSize - currHeals.Count;
		if (slotsRemaining <= 0)
		{
			slotsLeftLabel.text = "HOSPITAL\nFULL";
			slotsLeftLabel.color = slotsFullLabelColor;
			arrow.spriteName = RED_ARROW;
		}
		else
		{
			slotsLeftLabel.text = slotsRemaining + " SLOT" + (slotsRemaining>1?"S":"") + "\nOPEN";
			slotsLeftLabel.color = slotsOpenLabelColor;
			arrow.spriteName = GREEN_ARROW;
		}
	}

	void Update()
	{
		timeLeft = 0;
		foreach (var item in currHeals) 
		{
			timeLeft = Math.Max(item.monster.healTimeLeftMillis, timeLeft);
		}

		timeLeftLabel.text = MSUtil.TimeStringShort(timeLeft);
		int finishAmount = MSMath.GemsForTime(timeLeft, true);
		if(finishAmount != 0)
		{
			finishNowLabel.text = "Finish\n(g) " + finishAmount;
		}
		else
		{
			finishNowLabel.text = "Finish\n FREE";
		}
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
