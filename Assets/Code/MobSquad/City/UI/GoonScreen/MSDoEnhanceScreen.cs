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

	public List<MSGoonCard> feederCards = new List<MSGoonCard>();

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

	[SerializeField]
	MSLevelUpAnimation levelUpAnim;

	[SerializeField]
	TweenAlpha glowTween;

	[SerializeField]
	UISprite glow;

	float futureLevel;

	[SerializeField] Color greyTextColor;
	[SerializeField] Color startTextColor;
	[SerializeField] Color finishTextColor;
	[SerializeField] Color collectTextColor;
	[SerializeField] Color HelpTextColor;

	const string GREY_BUTTON_NAME = "greymenuoption";
	const string START_BUTTON_NAME = "yellowmenuoption";
	const string FINISH_NOW_BUTTON_NAME = "purplemenuoption";
	const string COLLECT_BUTTON_NAME = "greenmenuoption";
	const string HELP_BUTTON_NAME = "orangemenuoption";

	IEnumerator currCollectAnim;
	IEnumerator currCollectStep;

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
		feederCards.Clear();

		grid.Init(GoonScreenMode.DO_ENHANCE);

		MSSpriteUtil.instance.SetSprite(enhanceMonster.monster.imagePrefix, 
		                                enhanceMonster.monster.imagePrefix + "Character", 
		                                mobsterPose);
		if (enhanceMonster.monster.monsterElement == Element.DARK)
		{
			glow.spriteName = "nightenhancebg";
		}
		else
		{
			glow.spriteName = enhanceMonster.monster.monsterElement.ToString().ToLower() + "enhancebg";
		}

		RefreshStats();
	}

	void OnDisable()
	{
		MSEnhancementManager.instance.ClearTemp();
	}

	/// <summary>
	/// Updates the stats and bar.
	/// Called on init, and when monsters are added and removed from feeders.
	/// </summary>
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
		float currLevel = enhanceMonster.LevelForMonster(enhanceMonster.userMonster.currentExp);
		SetBar(currLevel, futureLevel);
	}

	void SetBar(float currLevel, float futureLevel)
	{
		levelLabel.text = "Level " + ((int)currLevel) + ":";
		topBar.fill = currLevel % 1;
		if ((int)futureLevel > (int)currLevel)
		{
			bottomBar.fill = 1;
		}
		else
		{
			bottomBar.fill = futureLevel % 1;
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

				//ifShouldshow helpbutton
				if(!MSClanManager.instance.HelpAlreadyRequested(GameActionType.ENHANCE_TIME,
				                                                (int)MSEnhancementManager.instance.enhancementMonster.monster.monsterId,
				                                                MSEnhancementManager.instance.currEnhancement.baseMonster.userMonsterUuid) &&
				   MSClanManager.instance.isInClan)
				{
					buttonLabel.text = "Get Help!";
					buttonLabel.color = HelpTextColor;
					button.normalSprite = HELP_BUTTON_NAME;
				}
				else
				{
					buttonLabel.text = "Finish\n(g) " + MSEnhancementManager.instance.gemsToFinish;
					buttonLabel.color = finishTextColor;
					button.normalSprite = FINISH_NOW_BUTTON_NAME;
					//RefreshBar();
				}
			}
		}
		else
		{
			buttonLabel.text = "Enhance\n(o) " + totalCost;
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
		feederCards.Add (card);
		RefreshStats();
	}

	public void RemoveMonster(MSGoonCard card)
	{
		feederCards.Remove (card);
		RefreshStats();
	}

	public void ClickButton()
	{
		if (MSEnhancementManager.instance.hasEnhancement)
		{
			if (MSEnhancementManager.instance.finished) //Collect enhancement
			{
				MSEnhancementManager.instance.DoCollectEnhancement(loadLock, DoCollectAnimation);
			}
			else //Finish enhancement with gems or Solicite help
			{
				if(!MSClanManager.instance.HelpAlreadyRequested(GameActionType.ENHANCE_TIME,
				                                                (int)MSEnhancementManager.instance.enhancementMonster.monster.monsterId,
				                                                MSEnhancementManager.instance.currEnhancement.baseMonster.userMonsterUuid) &&
				   MSClanManager.instance.isInClan)
				{
					loadLock.Lock();
					MSClanManager.instance.DoSolicitClanHelp(GameActionType.ENHANCE_TIME,
					                                         (int)MSEnhancementManager.instance.enhancementMonster.monster.monsterId,
					                                         MSEnhancementManager.instance.currEnhancement.baseMonster.userMonsterUuid,
					                                         MSBuildingManager.clanHouse.combinedProto.clanHouse.maxHelpersPerSolicitation,
					                                         loadLock.Unlock);

				}
				//finish with gems
				else if (MSResourceManager.instance.Spend(ResourceType.GEMS, MSEnhancementManager.instance.gemsToFinish))
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

	[SerializeField] float spinTime = 1f;
	[SerializeField] float spinFinalAngle = 720;
	[SerializeField] Transform spinParent;
	[SerializeField] AnimationCurve spinCurve;

	void DoCollectAnimation()
	{
		currCollectAnim = CollectAnimation();
		StartCoroutine(currCollectAnim);
	}

	void StopCollectAnimation()
	{
		if (currCollectAnim != null) StopCoroutine(currCollectAnim);
		if (currCollectStep != null) StopCoroutine(currCollectStep);
	}

	IEnumerator CollectAnimation()
	{
		int currXp = enhanceMonster.userMonster.currentExp;
		while (feederCards.Count > 0)
		{
			currCollectStep = SpinInMobster(feederCards[0], spinTime);
			yield return StartCoroutine(currCollectStep);
			glowTween.Sample (0, true);
			glowTween.PlayForward();
			currCollectStep = FillUpBar(enhanceMonster.LevelForMonster(currXp), enhanceMonster.LevelForMonster(currXp + feederCards[0].monster.enhanceXP));
			feederCards.RemoveAt(0);
			yield return StartCoroutine(currCollectStep);

		}
		currCollectAnim = null;
		currCollectStep = null;
	}

	IEnumerator SpinInMobster(MSGoonCard card, float time)
	{
		float currTime = 0;
		card.transform.parent = spinParent;
		yield return null;
		enhanceQueue.Reposition();
		float startDistance = card.transform.localPosition.magnitude;
		float startAngle = -Vector3.Angle(new Vector3(0, -1, 0), card.transform.localPosition);
		Vector3 startScale = card.transform.localScale;

		Vector3 originalPos = card.transform.localPosition;

		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.enhanceFeed);

		float angle = startAngle;
		while (currTime < time)
		{
			currTime += Time.deltaTime;
			angle = Mathf.Lerp(startAngle, spinFinalAngle, spinCurve.Evaluate (currTime/time));
//			Debug.Log("Angle: " + angle);
			card.transform.localPosition = MSMath.DirectionFromEuler(angle) * Mathf.Lerp(startDistance, 0, spinCurve.Evaluate(currTime/time));
			card.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, spinCurve.Evaluate(currTime/time));
			yield return null;
		}

		card.Pool();
	}

	IEnumerator FillUpBar(float start, float end)
	{
		int currLevel = (int) start;
		float curr = start;

		while (curr < end)
		{
			curr = Mathf.Min (curr+Time.deltaTime*1f, end); //Don't let (curr > end)
			topBar.fill = curr%1;

			if ((int)curr > currLevel)
			{
				currLevel = (int) curr;
				curr = currLevel;
				SetBar (curr, end);
				levelUpAnim.Play();
				yield return new WaitForSeconds(1f);
			}
			else
			{	
				SetBar(curr, end);
			}

			yield return null;
		}
	}

	public void Back()
	{
		MSPopupManager.instance.popups.goonScreen.DoShiftLeft(false);
	}

}
