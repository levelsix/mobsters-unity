using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class MSGoonCard : MonoBehaviour {
	
	#region Editor Pointers
	
	[SerializeField]
	UISprite cardBackground;
	
	[SerializeField]
	UI2DSprite goonPose;
	
	[SerializeField]
	UILabel healCostLabel;
	
	[SerializeField]
	UISprite rarityRibbon;
	
	[SerializeField]
	UILabel rarityLabel;
	
	[SerializeField]
	MSFillBar healthBar;

	[SerializeField]
	UILabel healthBarText;

	[SerializeField]
	UISprite healthBarBackground;
	
	[SerializeField]
	UILabel nameLabel;
	
	[SerializeField]
	UILabel bottomCardLabel;
	
	[SerializeField]
	Color darkenColor;

	[SerializeField]
	GameObject bottomHolder;

	[HideInInspector]
	public MSGoonCard buddy = null;

	bool isBuddyParent = false;

	Vector3 buddyOffset = new Vector3(15,-15);
	
	[SerializeField]
	MSUIHelper bigHelper;
	
	[SerializeField]
	GameObject infoButton;

	public MSUIHelper dots;

	#region SmallMobster

	[SerializeField]
	UISprite smallBG;

	[SerializeField]
	UI2DSprite smallMobster;

	[SerializeField]
	MSUIHelper smallHelper;

	[SerializeField]
	GameObject smallBarRoot;

	[SerializeField]
	MSFillBar smallBar;

	[SerializeField]
	UILabel smallBarLabel;

	[SerializeField]
	UILabel smallBottomLabel;

	#endregion

	#region Medium Mobster

	[SerializeField]
	UISprite mediumBG;

	[SerializeField]
	UI2DSprite mediumMobster;

	[SerializeField]
	MSUIHelper mediumHelper;

	[SerializeField]
	GameObject mediumRemove;

	[SerializeField]
	GameObject mediumRibbonHelper;

	[SerializeField]
	UISprite mediumRibbon;

	[SerializeField]
	UILabel mediumRibbonLabel;

	[SerializeField]
	GameObject mediumBottomBG;

	[SerializeField]
	UILabel mediumBottomlabel;

	[SerializeField]
	UILabel mediumScientistCount;

	[SerializeField]
	GameObject lockIcon;

	[SerializeField]
	Color cashTextColor;

	#endregion
	
	#endregion
	
	#region Image Constants
	
	public static readonly Dictionary<Element, string> backgroundsForElements = new Dictionary<Element, string>()
	{
		{Element.DARK, "darksquare"},
		{Element.FIRE, "firesquare"},
		{Element.EARTH, "earthsquare"},
		{Element.LIGHT, "lightsquare"},
		{Element.WATER, "watersquare"},
		{Element.ROCK, "earthsquare"},
		{Element.NO_ELEMENT, "nightsquare"}
	};

	public static readonly Dictionary<Element, string> mediumBackgrounds = new Dictionary<Element, string>()
	{
		{Element.DARK, "darkmediumsquare"},
		{Element.FIRE, "firemediumsquare"},
		{Element.EARTH, "earthmediumsquare"},
		{Element.LIGHT, "lightmediumsquare"},
		{Element.WATER, "watermediumsquare"},
		{Element.ROCK, "earthmediumsquare"},
		{Element.NO_ELEMENT, "nightmediumsquare"}
	};

	public static readonly Dictionary<Element, string> smallBackgrounds = new Dictionary<Element, string>()
	{
		{Element.DARK, "darksmallsquare"},
		{Element.FIRE, "firesmallsquare"},
		{Element.EARTH, "earthsmallsquare"},
		{Element.LIGHT, "lightsmallsquare"},
		{Element.WATER, "watersmallsquare"},
		{Element.ROCK, "earthsmallsquare"},
		{Element.NO_ELEMENT, "nightsmallsquare"}
	};
	
	public static readonly Dictionary<Quality, string> ribbonsForRarity = new Dictionary<Quality, string>()
	{
		{Quality.COMMON, "commonband"},
		{Quality.EPIC, "epicband"},
		{Quality.LEGENDARY, "legendaryband"},
		{Quality.RARE, "rareband"},
		{Quality.ULTRA, "ultraband"},
		{Quality.SUPER, "superband"}
	};
	
	const string emptyBackground = "emptyslot";
	
	const string healIcon = "healbutton";
	const string gemIcon = "diamond";

	const int HEALTH_BAR_WIDTH = 80;
	const int ENHANCE_BAR_WIDTH = 160;
	const int BAR_BG_WIDTH = 4;

	#endregion
	
	public PZMonster monster;

	const string teamMemberToHealWarning = "Are you sure you want to heal this goon? You won't be able to use them in your team until " +
		"they are fully healed.";

	GoonScreenMode goonScreenMode;

	bool readyToEvolve = false;

	bool _dark = false;

	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory += CheckRemovedMonster;
		MSActionManager.Goon.OnEnhanceQueueChanged += OnEnhancementQueueChanged;
		MSActionManager.Goon.OnMonsterFinishHeal += OnMonsterFinishHeal;
		MSActionManager.Goon.OnFinishFeeding += OnMonsterFinishFeed;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory -= CheckRemovedMonster;
		MSActionManager.Goon.OnEnhanceQueueChanged -= OnEnhancementQueueChanged;
		MSActionManager.Goon.OnMonsterFinishHeal -= OnMonsterFinishHeal;
		MSActionManager.Goon.OnFinishFeeding -= OnMonsterFinishFeed;
	}

	public void Init(PZMonster goon, GoonScreenMode mode)
	{
		goonScreenMode = mode;
		switch (mode) {
		case GoonScreenMode.HEAL:
			InitHeal(goon);
			break;
		case GoonScreenMode.SELL:
			InitSell(goon);
			break;
		case GoonScreenMode.PICK_ENHANCE:
			InitPickEnhance(goon);
			break;
		case GoonScreenMode.DO_ENHANCE:
			InitDoEnhance(goon);
			break;
		case GoonScreenMode.TEAM:
			InitTeam(goon);
			break;
		case GoonScreenMode.PICK_EVOLVE:
			InitEvolve(goon);
			break;
		default:
			break;
		}
		//StartCoroutine(MakeDamnSure());
	}

	IEnumerator MakeDamnSure()
	{
		yield return null;
		transform.localScale = Vector3.one;
	}

	public void InitHeal(PZMonster goon)
	{
		Setup (goon);
		SetHealButton(goon);

		bottomCardLabel.text = " ";

		healCostLabel.text = "$" + goon.healCost;
		healCostLabel.color = Color.black;



		infoButton.SetActive(false);

		if (goon.monsterStatus == MonsterStatus.HEALING)
		{
			AddToHealQueue();
		}
		else
		{
			SetName();
		}
	}

	public void InitTeam(PZMonster goon)
	{
		Setup(goon);
		healCostLabel.text = " ";
		bottomCardLabel.text = " ";

		mediumRibbonHelper.SetActive(false);

		SetName();

		TintElements(goon.monsterStatus != MonsterStatus.HEALTHY && goon.monsterStatus != MonsterStatus.INJURED);

		if (goon.userMonster.teamSlotNum > 0)
		{
			transform.parent = MSTeamScreen.instance.playerTeam[goon.userMonster.teamSlotNum-1].transform;
			transform.localPosition = Vector3.zero;
			bigHelper.gameObject.SetActive(false);
			mediumHelper.gameObject.SetActive(true);
			foreach (var widget in GetComponentsInChildren<UIWidget>()) 
			{
				widget.ParentHasChanged();
			}
		}
	}

	void SetName()
	{
		switch (monster.monsterStatus)
		{
		case MonsterStatus.INCOMPLETE:
			name = "4 Unavailable 6 Piece";
			bottomCardLabel.text = "Pieces: " + monster.userMonster.numPieces + "/" + monster.monster.numPuzzlePieces;
			break;
		case MonsterStatus.ON_MINI_JOB:
			name = "4 Unavailable 5 On Mini Job";
			bottomCardLabel.text = "MiniJob";
			break;
		case MonsterStatus.COMBINING:
			name = "4 Unavailable 4 Combining";
			bottomCardLabel.text = "Combining...";
			break;
		case MonsterStatus.ENHANCING:
			name = "4 Unavailable 3 Enhancing";
			bottomCardLabel.text = "Enhancing";
			break;
		case MonsterStatus.HEALING:
			name = "4 Unavailable 2 Healing";
			bottomCardLabel.text = "Healing";
			break;
		case MonsterStatus.INJURED:
			name = "1 Injured 2 Card";
			break;
		case MonsterStatus.HEALTHY:
			name = "3 Healthy 2 Card";
			break;
		}
		name += " " + monster.monster.monsterId + " " + monster.userMonster.currentLvl + " " + monster.userMonster.userMonsterId;
	}

	public void InitPickEnhance(PZMonster goon)
	{
		Setup (goon);
		healthBarBackground.alpha = 0;
		float currLevel = goon.LevelForMonster(goon.userMonster.currentExp);
		Debug.Log("Curr level: " + currLevel);
		bottomCardLabel.text = Mathf.CeilToInt((currLevel%1)*100) + "%";
	}

	public void InitDoEnhance(PZMonster goon)
	{
		Setup (goon);
		
		SetName ();

		bigHelper.gameObject.SetActive(false);
		mediumHelper.gameObject.SetActive(true);

		mediumRemove.SetActive(false);
		mediumBottomBG.SetActive(true);
		mediumRibbonHelper.SetActive(true);

		if (goon.restricted)
		{
			mediumBottomlabel.text = "Locked";
			lockIcon.SetActive(true);
			TintElements(true);
		}
		else
		{
			TintElements(false);
			mediumBottomlabel.text = goon.enhanceXP + "xp";
		}

		//Small gets fiddled with on Update

		if (MSMonsterManager.instance.enhancementFeeders.Contains(goon))
		{
			MoveToEnhanceQueue();
		}

	}

	/// <summary>
	/// We're assuming that whenever we init something for evolve, we're starting on the
	/// enhancement screen, so we've already InitLab'd
	/// If we can get a buddy
	/// </summary>
	/// <param name="buddy">Buddy.</param>
	public void InitEvolve(PZMonster monster)
	{
		Setup(monster);

		healthBarBackground.alpha = 0;
		bottomCardLabel.text = "Tap for Info";
		dots.ResetAlpha(true);

	}

	public bool FindBuddy(List<MSGoonCard> cards)
	{
		if (buddy != null
		    || monster.userMonster.currentLvl < monster.monster.maxLevel)
		{
			return false;
		}

		foreach (var item in cards) 
		{
			if (item != this 
			    && item.buddy == null
			    && item.monster.monster.monsterId == monster.monster.monsterId
			    && item.monster.userMonster.currentLvl == monster.monster.maxLevel)
			{
				BuddyUp(item);
				return true;
			}
		}
		return false;
	}

	public void BuddyUp(MSGoonCard buddy)
	{
		foreach (var item in GetComponentsInChildren<UIWidget>()) 
		{
			item.depth += 10;
		}
		
		isBuddyParent = true;
		this.buddy = buddy;
		buddy.buddy = this;
		buddy.transform.parent = transform;
		buddy.transform.localPosition = new Vector3(15, -15);
		buddy.bottomHolder.SetActive(false);
		dots.ResetAlpha(false);
		buddy.dots.ResetAlpha(false);
		
		nameLabel.text = " ";
	}

	public void InitScientist(long userMonsterId)
	{
		if (userMonsterId > 0)
		{
			monster = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId==userMonsterId);
			Setup (monster);
			bottomHolder.SetActive(false);
			goonPose.alpha = 1;
		}
		else
		{
			nameLabel.text = "Missing Scientist";
			cardBackground.spriteName = emptyBackground;
			goonPose.alpha = 0;
		}
	}

	/// <summary>
	/// Inits the card to sell mode.
	/// </summary>
	public void InitSell(PZMonster goon)
	{
		this.monster = goon;
		Setup(goon);

		infoButton.SetActive(false);

		if (goon.restricted)
		{
			TintElements(true);
			lockIcon.SetActive(true);
			bottomCardLabel.text = "Locked";
			healCostLabel.text = " ";
			name = "0";
		}
		else
		{
			TintElements(false);

			name = goon.sellValue.ToString();

			healCostLabel.text = "$" + goon.sellValue;
			healCostLabel.color = cashTextColor;
		}

	}

	void SetEnhancementBonusText()
	{
		if (monster == MSMonsterManager.instance.currentEnhancementMonster)
		{
			bottomCardLabel.text = " ";
		}
		else
		{
			bottomCardLabel.text = (MSMonsterManager.instance.currentEnhancementMonster.PercentageOfAddedLevelup(monster.enhanceXP) * 100).ToString("N0") + "% for $" + monster.enhanceCost;
		}
	}

	void Setup(PZMonster goon)
	{
		lockIcon.SetActive(false);
		bigHelper.gameObject.SetActive(true);
		bigHelper.ResetAlpha(true);
		mediumHelper.gameObject.SetActive(false);
		smallHelper.gameObject.SetActive(false);
		dots.ResetAlpha(false);

		bottomHolder.SetActive(true);

		healthBarBackground.alpha = 1;

		this.monster = goon;

		string goonImageBase = MSUtil.StripExtensions(goon.monster.imagePrefix);
		MSSpriteUtil.instance.SetSprite(goonImageBase, goonImageBase + "Card", goonPose);
		MSSpriteUtil.instance.SetSprite(goonImageBase, goonImageBase + "Thumbnail", mediumMobster);
		MSSpriteUtil.instance.SetSprite(goonImageBase, goonImageBase + "Thumbnail", smallMobster);

		cardBackground.spriteName = backgroundsForElements[goon.monster.monsterElement];
		mediumBG.spriteName = mediumBackgrounds[goon.monster.monsterElement];
		smallBG.spriteName = smallBackgrounds[goon.monster.monsterElement];

		SetTextOverCard (goon);
		
		rarityRibbon.spriteName = mediumRibbon.spriteName = ribbonsForRarity[goon.monster.quality];
		switch(goon.monster.quality)
		{
		case Quality.LEGENDARY:
			rarityLabel.text = mediumRibbonLabel.text = "LEG.";
			break;
		case Quality.COMMON:
			rarityLabel.text = mediumRibbonLabel.text = "COM.";
			break;
		default:
			rarityLabel.text = mediumRibbonLabel.text = goon.monster.quality.ToString();
			break;
		}

		smallBarRoot.SetActive(false);
		smallBottomLabel.text = " ";

		mediumRemove.SetActive(true);
		mediumRibbonHelper.SetActive(false);
		mediumBottomBG.SetActive(false);
		mediumBottomlabel.text = " ";
		mediumScientistCount.text = " ";

		healthBarBackground.alpha = 1;
		healthBar.fill = ((float)goon.currHP) / goon.maxHP;
		healthBarText.text = goon.currHP + "/" + goon.maxHP;
		healCostLabel.text = " ";
		infoButton.SetActive(true);
		
		nameLabel.text = goon.monster.displayName + " [4a7eae]L" + goon.userMonster.currentLvl + "[-]";
	}

	void SetTextOverCard (PZMonster goon)
	{
		if (!goon.userMonster.isComplete)
		{
			healthBarBackground.alpha = 0;
			if (goon.userMonster.numPieces < goon.monster.numPuzzlePieces)
			{
				bottomCardLabel.text = "Pieces: " + goon.userMonster.numPieces + "/" + goon.monster.numPuzzlePieces;  
			}
			else
			{
				bottomCardLabel.text = "Completing in " + MSUtil.TimeStringShort(goon.combineTimeLeft);
			}
			TintElements (true);
		}
		else
		{
			healthBarBackground.alpha = 1;
			bottomCardLabel.text = " ";
			TintElements(false);
		}
	}
	
	void SetHealButton(PZMonster goon)
	{
		if (goon.currHP < goon.maxHP)
		{
			healCostLabel.text = "$" + goon.healCost;
		}
		else
		{
			healCostLabel.text = "Healthy";
		}
	}
	
	void TintElements(bool darken)
	{
		_dark = darken;
		smallMobster.color = mediumMobster.color = goonPose.color = darken ? Color.black : Color.white;
		if (darken)
		{
			healthBarBackground.alpha = 0;
			cardBackground.spriteName = "greysquare";
			mediumBG.spriteName = "greymediumsquare";
			smallBG.spriteName = "greysmallsquare";
		}
	}
	
	void AddToTeam()
	{
		if (monster.userMonster.teamSlotNum == 0)
		{
			if (MSMonsterManager.instance.AddToTeam(monster) == 0)
			{
				MSActionManager.Popup.DisplayRedError("Team is already full!");
			}
			else
			{
				transform.parent = MSTeamScreen.instance.playerTeam[monster.userMonster.teamSlotNum-1].transform;
				SpringPosition.Begin(gameObject, Vector3.zero, 15);
				bigHelper.FadeOutAndOff();
				mediumHelper.ResetAlpha(false);
				mediumHelper.FadeIn();
				MSTeamScreen.instance.mobsterGrid.Reposition();
				foreach (var widget in GetComponentsInChildren<UIWidget>()) 
				{
					widget.ParentHasChanged();
				}
			}
		}
	}

	void RemoveFromTeam()
	{
		transform.parent = MSTeamScreen.instance.mobsterGrid.transform;
		MSTeamScreen.instance.mobsterGrid.Reposition();

		foreach (var widget in GetComponentsInChildren<UIWidget>()) 
		{
			widget.ParentHasChanged();
		}

		bigHelper.ResetAlpha(false);
		bigHelper.FadeIn();
		mediumHelper.FadeOutAndOff();

		MSMonsterManager.instance.RemoveFromTeam(monster);
	}

	void ClearEnhanceQueue()
	{
		MSMonsterManager.instance.ClearEnhanceQueue();
	}

	void PickEnhanceMonster()
	{
		MSPickEnhanceScreen.instance.PickMonster(monster);
	}

	void TryAddToEnhanceQueue()
	{
		if (monster.userMonster.teamSlotNum > 0)
		{
			PopupTeamMemberToEnhanceQueue(monster);
		}
		else
		{
			AddToEnhanceQueue();
		}
	}

	void AddToEnhanceQueue()
	{
		if (MSMonsterManager.instance.AddToEnhanceQueue(monster))
		{
			MoveToEnhanceQueue ();
		}
	}

	void MoveToEnhanceQueue ()
	{
		MSDoEnhanceScreen.instance.AddMonster (this);
		name = (long.MaxValue - MSUtil.timeNowMillis).ToString ();
		transform.parent = MSDoEnhanceScreen.instance.enhanceQueue.transform;
		MSDoEnhanceScreen.instance.grid.Reposition ();
		MSDoEnhanceScreen.instance.enhanceQueue.Reposition ();
		foreach (var widget in GetComponentsInChildren<UIWidget> ()) {
			widget.ParentHasChanged ();
		}
		mediumHelper.FadeOutAndOff();
		smallHelper.ResetAlpha(false);
		smallHelper.FadeIn();
	}

	void RemoveFromEnhanceQueue()
	{
		MSMonsterManager.instance.RemoveFromEnhanceQueue(monster);
		MSDoEnhanceScreen.instance.RemoveMonster(this);

		transform.parent = MSDoEnhanceScreen.instance.grid.transform;
		MSDoEnhanceScreen.instance.grid.Reposition();
		MSDoEnhanceScreen.instance.enhanceQueue.Reposition();
		
		foreach (var widget in GetComponentsInChildren<UIWidget>()) 
		{
			widget.ParentHasChanged();
		}

		mediumHelper.ResetAlpha(false);
		mediumHelper.FadeIn();
		smallHelper.FadeOutAndOff();
	}
	
	void AddToHealQueue()
	{
		if (!MSHospitalManager.instance.healingMonsters.Contains(monster) && !MSHospitalManager.instance.AddToHealQueue(monster))
		{
			return;
		}
		MSHealScreen.instance.Add(this);
		name = (100 - monster.healingMonster.priority).ToString();

		transform.parent = MSHealScreen.instance.healQueue.transform;
		MSHealScreen.instance.grid.Reposition();
		MSHealScreen.instance.healQueue.Reposition();

		foreach (var widget in GetComponentsInChildren<UIWidget>()) 
		{
			widget.ParentHasChanged();
		}
		
		bigHelper.FadeOutAndOff();
		smallHelper.ResetAlpha(true);
		smallHelper.FadeIn();
	}

	void RemoveFromHealQueue()
	{
		MSHospitalManager.instance.RemoveFromHealQueue(monster);

		MSHealScreen.instance.Remove(this);
		
		transform.parent = MSHealScreen.instance.grid.transform;
		MSHealScreen.instance.grid.Reposition();
		MSHealScreen.instance.healQueue.Reposition();
		
		foreach (var widget in GetComponentsInChildren<UIWidget>()) 
		{
			widget.ParentHasChanged();
		}

		bigHelper.ResetAlpha(false);
		bigHelper.FadeIn();
		smallHelper.FadeOutAndOff();
	}

	void AddToSellQueue()
	{
		if (monster.restricted) return;

		//Look for another monster that's not this monster, is complete, and isn't in the sell queue.
		//If we can't find that, we can't sell this monster because it's our last.
		if (MSMonsterManager.instance.userMonsters.Find(x => (x != monster && x.userMonster.isComplete)
		                                               && (MSSellScreen.instance.currSells.Find(y=>y.monster == x) == null)) == null)
	    {
			return;
		}

		MSSellScreen.instance.Add(this);

		transform.parent = MSSellScreen.instance.sellQueue.transform;
		MSSellScreen.instance.grid.Reposition();
		MSSellScreen.instance.sellQueue.Reposition();
		
		foreach (var widget in GetComponentsInChildren<UIWidget>()) 
		{
			widget.ParentHasChanged();
		}
		
		bigHelper.FadeOutAndOff();
		smallHelper.ResetAlpha(false);
		smallHelper.FadeIn();
	}

	void RemoveFromSellQueue()
	{
		MSSellScreen.instance.Remove(this);

		transform.parent = MSSellScreen.instance.grid.transform;
		MSSellScreen.instance.grid.Reposition();
		MSSellScreen.instance.sellQueue.Reposition();
		
		foreach (var widget in GetComponentsInChildren<UIWidget>()) 
		{
			widget.ParentHasChanged();
		}

		bigHelper.ResetAlpha(false);
		bigHelper.FadeIn();
		smallHelper.FadeOutAndOff();
	}

	void PickForEvolve()
	{
		MSEvolutionManager.instance.TryEvolveMonster(monster, (buddy!=null) ? buddy.monster : null);
		MSPopupManager.instance.popups.goonScreen.DoShiftRight(false);
	}
	
	void SpeedUpCombine()
	{
		MSMonsterManager.instance.SpeedUpCombine(monster);
	}

	void OnClick()
	{
		if (!_dark)
		{
			switch(goonScreenMode)
			{
			case GoonScreenMode.HEAL:
				AddToHealQueue();
				break;
			case GoonScreenMode.PICK_ENHANCE:
				PickEnhanceMonster();
				break;
			case GoonScreenMode.DO_ENHANCE:
				TryAddToEnhanceQueue();
				break;
			case GoonScreenMode.SELL:
				AddToSellQueue();
				break;
			case GoonScreenMode.PICK_EVOLVE:
				PickForEvolve();
				break;
			case GoonScreenMode.TEAM:
				if (monster.userMonster.teamSlotNum > 0)
				{
					RemoveFromTeam();
				}
				else
				{
					AddToTeam();
				}
				break;
			}
		}
	}

	public void ClickInfoButton()
	{
		MSPopupManager.instance.popups.goonInfoPopup.Init(monster);
		MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.goonInfoPopup.GetComponent<MSPopup>());
	}

	public void ClickRemoveButton()
	{
		Debug.Log("Clicked remove");
		switch (goonScreenMode)
		{
		case GoonScreenMode.TEAM:
			RemoveFromTeam();
			break;
		case GoonScreenMode.SELL:
			RemoveFromSellQueue();
			break;
		case GoonScreenMode.HEAL:
			RemoveFromHealQueue();
			break;
		case GoonScreenMode.DO_ENHANCE:
			RemoveFromEnhanceQueue();
			break;
		}
	}

	public IEnumerator PhaseOut()
	{
		transform.parent = transform.parent.parent;
		TweenAlpha tween;
		if(cardBackground.alpha != 0)
		{
			tween = TweenAlpha.Begin(cardBackground.gameObject, .3f, 0);
		}
		else
		{
			tween = TweenAlpha.Begin(smallBG.gameObject, .3f, 0);
		}
		while (tween.tweenFactor < 1)
		{
			yield return null;
		}
		MSPoolManager.instance.Pool(GetComponent<MSSimplePoolable>());
	}

	void PopupTeamMemberToEnhanceQueue(PZMonster monster)
	{
		MSPopupManager.instance.CreatePopup("Mobster On Team",
			"Mobster will be removed from team, dawg!", new string[]{"Yes", "No"},
			new string[]{"greenmenuoption", "greymenuoption"},
			new Action[]{delegate{ MSMonsterManager.instance.RemoveFromTeam(monster);
				AddToEnhanceQueue();
				MSActionManager.Popup.CloseTopPopupLayer();}, 
				MSActionManager.Popup.CloseTopPopupLayer});
	}
	
	void PopupTeamMemberToHealingQueue(PZMonster monster)
	{
		MSPopupManager.instance.CreatePopup("Mobster On Team",
            teamMemberToHealWarning, new string[]{"Yes", "No"},
			new string[]{"greenmenuoption", "greymenuoption"},
			new Action[]{delegate{MSHospitalManager.instance.AddToHealQueue(monster); 
				MSActionManager.Popup.CloseTopPopupLayer();}, 
				MSActionManager.Popup.CloseTopPopupLayer});
	}

	void Update()
	{
		switch(monster.monsterStatus)
		{
		case MonsterStatus.HEALING:
			smallBarRoot.SetActive(MSUtil.timeNowMillis >= monster.healStartTime);
			smallBar.fill = monster.healProgressPercentage;
			smallBarLabel.text = MSUtil.TimeStringShort(monster.healTimeLeftMillis);
			break;
		case MonsterStatus.ENHANCING:
			//enhancing no longer uses a timer
//			if (monster.enhancement.expectedStartTimeMillis <= MSUtil.timeNowMillis)
//			{
//				smallBarRoot.SetActive(false);
//				smallBottomLabel.text = " ";
//				smallBar.fill = 1;// - ((float)monster.enhanceTimeLeft) / ((float)monster.timeToUseEnhance); enhance no longer uses a timer
//				smallBarLabel.text = "no more timer";//MSUtil.TimeStringShort(monster.enhanceTimeLeft);
//			}
//			else
//			{
			smallBarRoot.SetActive(false);
			smallBottomLabel.text = monster.enhanceXP + "xp";
//			}
			break;
		}
	}

	void CheckRemovedMonster(long userMonsterId)
	{
		if (monster != null && monster.userMonster != null && monster.userMonster.userMonsterId == userMonsterId)
		{
			//GetComponent<MSSimplePoolable>().Pool();
		}
	}

	void OnEnhancementQueueChanged()
	{
		if (MSMonsterManager.instance.currentEnhancementMonster != null 
		    && monster != null && monster.monster != null && monster.monster.monsterId > 0)
		{
			SetEnhancementBonusText();
		}
	}

	void OnMonsterFinishFeed(PZMonster monster)
	{
		if (this.monster == monster)
		{
			StartCoroutine(PhaseOut());
			if (goonScreenMode == GoonScreenMode.DO_ENHANCE)
			{
				MSDoEnhanceScreen.instance.RemoveMonster(this);
				MSDoEnhanceScreen.instance.enhanceQueue.Reposition();
			}
		}
	}

	void OnMonsterFinishHeal(PZMonster monster)
	{
		if (this.monster == monster && goonScreenMode == GoonScreenMode.HEAL)
		{
			StartCoroutine(PhaseOut());
			MSHealScreen.instance.Remove(this);
			MSHealScreen.instance.healQueue.Reposition();
		}
	}

	public void Pool()
	{
		if (isBuddyParent)
		{
			buddy.transform.parent = transform.parent;
			isBuddyParent = false;
			
			foreach (var item in GetComponentsInChildren<UIWidget>()) 
			{
				item.depth -= 10;
			}
			
			buddy.buddy = null;
			buddy = null;
		}

		GetComponent<MSSimplePoolable>().Pool();
	}

}
