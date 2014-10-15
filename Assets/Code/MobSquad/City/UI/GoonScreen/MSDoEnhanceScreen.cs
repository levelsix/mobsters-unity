using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSDoEnhanceScreen : MSFunctionalScreen {

	public static MSDoEnhanceScreen instance;

	[SerializeField]
	UI2DSprite mobsterPose;
	
	public MSMobsterGrid grid;

	[SerializeField]
	UILabel currExpNeeded;

	[SerializeField]
	UILabel neededForLevel;

	[SerializeField]
	UILabel finishButtonLabel;

	public UIGrid enhanceQueue;

	[SerializeField]
	UILabel levelLabel;

	[SerializeField]
	UILabel resultLevel;

	[SerializeField]
	MSFillBar bottomBar;

	[SerializeField]
	MSFillBar topBar;

	[SerializeField]
	UILabel hpLabel;

	[SerializeField]
	UILabel attackLabel;

	[SerializeField]
	UILabel costLabel;

	[SerializeField]
	MSLoadLock loadLock;

	float futureLevel;

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

	int totalCost
	{
		get
		{
			return feeders.Count * enhanceMonster.enhanceCost;
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
		RefreshStats();
	}

	void OnDisable()
	{
		if (feeders.Count == 0 && enhanceMonster != null && enhanceMonster.monster.monsterId > 0)
		{
			MSMonsterManager.instance.RemoveFromEnhanceQueue(enhanceMonster);
		}
		//no longer uses the timer way
//		MSMonsterManager.instance.DoSendStartEnhanceRequest();
		MSMonsterManager.instance.ClearEnhanceQueue();
	}

	void RefreshStats()
	{
		finishButtonLabel.text = "Enhance\n(o) " + totalCost;
		

		//Get future level
		futureLevel = enhanceMonster.LevelWithFeeders(feeders);

		if (futureLevel == enhanceMonster.level)
		{
			hpLabel.text = enhanceMonster.maxHP.ToString();
			attackLabel.text = ((int)enhanceMonster.totalDamage).ToString();
		}
		else
		{
			//Get HP difference
			int hpDiff = enhanceMonster.MaxHPAtLevel((int)futureLevel) - enhanceMonster.maxHP;
			hpLabel.text = enhanceMonster.maxHP + " + " + hpDiff;

			//Get Attack difference
			int attDiff = (enhanceMonster.TotalAttackAtLevel((int)futureLevel) - (int)enhanceMonster.totalDamage);
			attackLabel.text = ((int)enhanceMonster.totalDamage) + " + " + attDiff;
		}

		//Get Next level
		int next = Mathf.FloorToInt(futureLevel + 1);
		int xpNext = enhanceMonster.XpForLevel(next) - enhanceMonster.ExpWithFeeders(feeders);

		resultLevel.text = "Level " + Mathf.FloorToInt(futureLevel);
		currExpNeeded.text = xpNext + "xp";
		neededForLevel.text = "Needed for level " + next;

		costLabel.text = "(o) " + enhanceMonster.enhanceCost;

		RefreshBar();
	}

	void RefreshBar()
	{
		if (feeders.Count > 0)
		{
			float currLevel = enhanceMonster.LevelForMonster(enhanceMonster.userMonster.currentExp);
			levelLabel.text = "Level " + ((int)currLevel) + ":";
			topBar.fill = currLevel % 1;


			if ((int)currLevel < (int)futureLevel)
			{
				bottomBar.fill = 1;
			}
			else
			{
				bottomBar.fill = futureLevel%1;
			}
		}
		else
		{
			levelLabel.text = "Level " + enhanceMonster.level + ":";
			topBar.fill = enhanceMonster.LevelForMonster(enhanceMonster.userMonster.currentExp) % 1;
			bottomBar.fill = 0;
		}
	}

	//no longer uses a timer
//	void Update()
//	{
//		if (feeders.Count == 0)
//		{
//			timeHelper.ResetAlpha(false);
//		}
//		else
//		{
//			timeHelper.ResetAlpha(true);
//			timeLeft.text = MSUtil.TimeStringShort(feeders[feeders.Count-1].enhanceTimeLeft);
//			finishButtonLabel.text = "Finish\n(G) " + MSMath.GemsForTime(feeders[feeders.Count-1].enhanceTimeLeft);
//			RefreshBar();
//		}
//	}


	public void AddMonster(MSGoonCard card)
	{
		RefreshStats();
	}

	public void RemoveMonster(MSGoonCard card)
	{
		RefreshStats();
	}

	public void Finish()
	{
		if(MSResourceManager.instance.Spend(ResourceType.OIL, totalCost, FinishBySpendingGemsForOil))
		{
			MSMonsterManager.instance.DoEnhanceRequest(totalCost, 0, loadLock);
		}
	}

	void FinishBySpendingGemsForOil()
	{
		int oilSpent = MSResourceManager.instance.SpendAll(ResourceType.OIL);
		int gems = Mathf.CeilToInt((totalCost - oilSpent) * MSWhiteboard.constants.gemsPerResource);
		MSMonsterManager.instance.DoEnhanceRequest(oilSpent, gems, loadLock);
	}

	public void Back()
	{
		MSPopupManager.instance.popups.goonScreen.DoShiftLeft(false);
	}

}
