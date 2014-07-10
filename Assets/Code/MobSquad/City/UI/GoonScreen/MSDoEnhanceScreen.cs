﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSDoEnhanceScreen : MSFunctionalScreen {

	public static MSDoEnhanceScreen instance;

	[SerializeField]
	UI2DSprite mobsterPose;
	
	public MSMobsterGrid grid;

	[SerializeField]
	UILabel timeLeft;

	[SerializeField]
	MSUIHelper timeHelper;

	[SerializeField]
	UILabel currPerc;

	[SerializeField]
	UILabel finishButtonLabel;

	public UIGrid enhanceQueue;

	[SerializeField]
	UILabel hpLabel;

	[SerializeField]
	UILabel attackLabel;

	[SerializeField]
	UILabel costLabel;

	/// <summary>
	/// Getter for monster being enhanced to keep code clean.
	/// </summary>
	/// <value>The enhance monster.</value>
	PZMonster enhanceMonster
	{
		get
		{
			return MSMonsterManager.instance.currentEnhancementMonster;
		}
	}

	/// <summary>
	/// Getter for Feeders to keep code looking clean.
	/// </summary>
	/// <value>The feeders.</value>
	List<PZMonster> feeders
	{
		get
		{
			return MSMonsterManager.instance.enhancementFeeders;
		}
	}

	void Awake()
	{
		instance = this;
	}

	public override bool IsAvailable ()
	{
		return MSBuildingManager.enhanceLabs.Count > 0
			&& MSMonsterManager.instance.isEnhancing;
	}

	public override void Init ()
	{
		grid.Init(GoonScreenMode.DO_ENHANCE);

		MSSpriteUtil.instance.SetSprite(enhanceMonster.monster.imagePrefix, 
		                                enhanceMonster.monster.imagePrefix + "Character", 
		                                mobsterPose);
	}

	void OnDisable()
	{
		if (feeders.Count == 0)
		{
			MSMonsterManager.instance.currentEnhancementMonster = null;
		}
	}

	void RefreshStats()
	{
		//Get future level
		float level = enhanceMonster.LevelWithFeeders(feeders);

		if (level == enhanceMonster.level)
		{
			hpLabel.text = enhanceMonster.maxHP.ToString();
			attackLabel.text = ((int)enhanceMonster.totalDamage).ToString();
		}
		else
		{
			//Get HP difference
			int hpDiff = enhanceMonster.MaxHPAtLevel((int)level) - enhanceMonster.maxHP;
			hpLabel.text = enhanceMonster.maxHP + " + " + hpDiff;

			//Get Attack difference
			int attDiff = (enhanceMonster.TotalAttackAtLevel((int)level) - (int)enhanceMonster.totalDamage);
			attackLabel.text = ((int)enhanceMonster.totalDamage) + " + " + attDiff;
		}
	}

	void RefreshTime()
	{
		if (feeders.Count == 0)
		{
			timeHelper.ResetAlpha(false);
		}
		else
		{
			timeHelper.ResetAlpha(true);
			timeLeft.text = MSUtil.TimeStringShort(feeders[feeders.Count-1].enhanceTimeLeft);
		}
	}

	public void AddMonster(MSGoonCard card)
	{
		RefreshTime();
	}

	public void RemoveMonster(MSGoonCard card)
	{
		RefreshTime();
	}

	public void Finish()
	{
		MSMonsterManager.instance.SpeedUpEnhance(MSMath.GemsForTime(feeders[feeders.Count-1].enhanceTimeLeft));
	}

	public void Back()
	{
		MSMonsterManager.instance.ClearEnhanceQueue();
		MSPopupManager.instance.popups.goonScreen.DoShiftLeft(false);
	}

}
