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
	UILabel timeLeft;

	[SerializeField]
	UILabel buttonLabel;

	[SerializeField]
	UIButton button;

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

	[SerializeField] Color greyTextColor;
	[SerializeField] Color startTextColor;
	[SerializeField] Color finishTextColor;
	[SerializeField] Color collectTextColor;

	const string GREY_BUTTON_NAME = "greymenuoption";
	const string START_BUTTON_NAME = "yellowmenuoption";
	const string FINISH_NOW_BUTTON_NAME = "purplemenuoption";
	const string COLLECT_BUTTON_NAME = "greenmenuoption";

	/// <summary>
	/// Getter for monster being enhanced to keep code clean.
	/// </summary>
	/// <value>The enhance monster.</value>
	PZMonster enhanceMonster
	{
		get
		{
			return MSEnhancementManager.instance.enhancementMonster;
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
			return MSEnhancementManager.instance.feeders;
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
			&& (MSEnhancementManager.instance.hasEnhancement || (MSEnhancementManager.instance.tempEnhancementMonster != null && MSEnhancementManager.instance.tempEnhancementMonster.monster.monsterId != 0));
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
		MSEnhancementManager.instance.ClearTemp();
	}

	void RefreshStats()
	{
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
			levelLabel.text = "Level " + ((int)futureLevel) + ":";
			if ((int)currLevel == (int)futureLevel)
			{
				topBar.fill = currLevel % 1;
			}
			else
			{
				topBar.fill = 0;
			}
			bottomBar.fill = futureLevel%1;
		}
		else
		{
			levelLabel.text = "Level " + enhanceMonster.level + ":";
			topBar.fill = enhanceMonster.LevelForMonster(enhanceMonster.userMonster.currentExp) % 1;
			bottomBar.fill = 0;
		}
	}

	//Updates the time/cost and other button info that changes moment to moment 
	void Update()
	{
		if (MSEnhancementManager.instance.hasEnhancement)
		{
			if (MSEnhancementManager.instance.finished)
			{
				timeLeft.text = "Done!";
				buttonLabel.text = "Collect";
				buttonLabel.color = collectTextColor;
				button.normalSprite = COLLECT_BUTTON_NAME;
			}
			else
			{
				timeLeft.text = MSUtil.TimeStringShort(MSEnhancementManager.instance.timeLeft);
				buttonLabel.text = "Finish\n(G) " + MSEnhancementManager.instance.gemsToFinish;
				buttonLabel.color = finishTextColor;
				button.normalSprite = FINISH_NOW_BUTTON_NAME;
				//RefreshBar();
			}
		}
		else
		{
			buttonLabel.text = "Enhance\n(O)" + totalCost;
			if (MSEnhancementManager.instance.feeders.Count == 0)
			{
				timeLeft.text = " ";
				button.normalSprite = GREY_BUTTON_NAME;
				buttonLabel.color = greyTextColor;
			}
			else
			{
				timeLeft.text = MSUtil.TimeStringShort(MSEnhancementManager.instance.potentialTime);
				button.normalSprite = START_BUTTON_NAME;
				buttonLabel.color = startTextColor;
			}
		}
	}

	public void AddMonster(MSGoonCard card)
	{
		RefreshStats();
	}

	public void RemoveMonster(MSGoonCard card)
	{
		RefreshStats();
	}

	public void ClickButton()
	{
		if (MSEnhancementManager.instance.hasEnhancement)
		{
			if (MSEnhancementManager.instance.finished) //Collect enhancement
			{
				MSEnhancementManager.instance.DoCollectEnhancement(loadLock);
			}
			else //Finish enhancement with gems
			{
				if (MSResourceManager.instance.Spend(ResourceType.GEMS, MSEnhancementManager.instance.gemsToFinish))
				{
					MSEnhancementManager.instance.FinishEnhanceWithGems(loadLock);
				}
			}
		}
		else if(MSEnhancementManager.instance.feeders.Count > 0 
		        && MSResourceManager.instance.Spend(ResourceType.OIL, totalCost, FinishBySpendingGemsForOil)) //Start enhancement
		{
			MSEnhancementManager.instance.DoSendStartEnhanceRequest(totalCost, 0, loadLock);
		}
	}

	void FinishBySpendingGemsForOil()
	{
		int gems = Mathf.CeilToInt((totalCost - MSResourceManager.resources[ResourceType.OIL]) * MSWhiteboard.constants.gemsPerResource);

		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gems))
		{
			int oilSpent = MSResourceManager.instance.SpendAll(ResourceType.OIL);
			MSEnhancementManager.instance.DoSendStartEnhanceRequest(oilSpent, gems, loadLock);
		}
	}

	public void Back()
	{
		MSPopupManager.instance.popups.goonScreen.DoShiftLeft(false);
	}

}
