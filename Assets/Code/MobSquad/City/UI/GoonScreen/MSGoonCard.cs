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

	#endregion
	
	#endregion
	
	#region Image Constants
	
	static readonly Dictionary<Element, string> backgroundsForElements = new Dictionary<Element, string>()
	{
		{Element.DARK, "darksquare"},
		{Element.FIRE, "firesquare"},
		{Element.EARTH, "earthsquare"},
		{Element.LIGHT, "lightsquare"},
		{Element.WATER, "watersquare"},
		{Element.ROCK, "earthsquare"},
		{Element.NO_ELEMENT, "nightsquare"}
	};

	static readonly Dictionary<Element, string> mediumBackgrounds = new Dictionary<Element, string>()
	{
		{Element.DARK, "darkmediumsquare"},
		{Element.FIRE, "firemediumsquare"},
		{Element.EARTH, "earthmediumsquare"},
		{Element.LIGHT, "lightmediumsquare"},
		{Element.WATER, "watermediumsquare"},
		{Element.ROCK, "earthmediumsquare"},
		{Element.NO_ELEMENT, "nightmediumsquare"}
	};

	static readonly Dictionary<Element, string> smallBackgrounds = new Dictionary<Element, string>()
	{
		{Element.DARK, "darksmallsquare"},
		{Element.FIRE, "firesmallsquare"},
		{Element.EARTH, "earthsmallsquare"},
		{Element.LIGHT, "lightsmallsquare"},
		{Element.WATER, "watersmallsquare"},
		{Element.ROCK, "earthsmallsquare"},
		{Element.NO_ELEMENT, "nightsmallsquare"}
	};
	
	static readonly Dictionary<Quality, string> ribbonsForRarity = new Dictionary<Quality, string>()
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

	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory += CheckRemovedMonster;
		MSActionManager.Goon.OnEnhanceQueueChanged += OnEnhancementQueueChanged;
		MSActionManager.Goon.OnMonsterFinishHeal += OnMonsterFinishHeal;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory -= CheckRemovedMonster;
		MSActionManager.Goon.OnEnhanceQueueChanged -= OnEnhancementQueueChanged;
		MSActionManager.Goon.OnMonsterFinishHeal -= OnMonsterFinishHeal;
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
		default:
			break;
		}
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

		if (goon.userMonster.teamSlotNum > 0)
		{
			transform.parent = MSTeamScreen.instance.playerTeam[goon.userMonster.teamSlotNum-1].transform;
			transform.localPosition = Vector3.zero;
			bigHelper.ResetAlpha(false);
			mediumHelper.ResetAlpha(true);
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
			break;
		case MonsterStatus.ON_MINI_JOB:
			name = "4 Unavailable 5 On Mini Job";
			break;
		case MonsterStatus.COMBINING:
			name = "4 Unavailable 4 Combining";
			break;
		case MonsterStatus.ENHANCING:
			name = "4 Unavailable 3 Enhancing";
			break;
		case MonsterStatus.HEALING:
			name = "4 Unavailable 2 Healing";
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

		bigHelper.ResetAlpha(false);
		mediumHelper.ResetAlpha(true);

		mediumRemove.SetActive(false);
		mediumBottomBG.SetActive(true);
		mediumRibbonHelper.SetActive(true);
		mediumBottomlabel.text = goon.enhanceXP + "xp";

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
	public void InitEvolve(MSGoonCard buddy = null)
	{
		bottomHolder.SetActive(false);
		this.buddy = buddy;

		if (buddy != null)
		{
			foreach (var item in GetComponentsInChildren<UIWidget>()) 
			{
				item.depth += 10;
			}

			isBuddyParent = true;
			buddy.buddy = this;
			buddy.transform.parent = transform;
			TweenPosition.Begin(buddy.gameObject, .2f, buddyOffset);
			buddy.bottomHolder.SetActive(false);

			nameLabel.text = " ";

			if (MSMonsterManager.instance.GetMonstersByMonsterId(monster.monster.evolutionCatalystMonsterId).Count > 0)
			{
				name = "1 2 Card " + monster.monster.monsterId + " " + monster.userMonster.userMonsterId;
			}
			else
			{
				name = "2 2 NoSci " + monster.monster.monsterId + " " + monster.userMonster.userMonsterId;
			}
		}
		else
		{
			name = "4 3 NoBuddy " + monster.monster.monsterId + " " + monster.userMonster.currentLvl + " " + monster.userMonster.userMonsterId;
		}
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

		SetName();

		healCostLabel.text = "$" + goon.sellValue;
		healCostLabel.color = Color.green;

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
		bigHelper.ResetAlpha(true);
		mediumHelper.ResetAlpha (false);
		smallHelper.ResetAlpha(false);

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

		bottomHolder.SetActive(true);
		
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
		
		nameLabel.text = goon.monster.displayName + "[4a7eae]L" + goon.userMonster.currentLvl + "[-]";
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
		goonPose.color = darken ? darkenColor : Color.white;
	}
	
	void AddToTeam()
	{
		if (monster.userMonster.teamSlotNum == 0)
		{
			if (MSMonsterManager.instance.AddToTeam(monster) == 0) //Returns 0 if the team is full
			{
				//MSPopupManager.instance.popups.goonScreen.DisplayErrorMessage("Team is already full!");
			}
			else
			{
				Debug.Log("Here, parent: " + transform.parent.name);
				transform.parent = MSTeamScreen.instance.playerTeam[monster.userMonster.teamSlotNum-1].transform;
				Debug.Log("Here, parent: " + transform.parent.name);
				SpringPosition.Begin(gameObject, Vector3.zero, 15);
				bigHelper.FadeOut();
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

		bigHelper.FadeIn();
		mediumHelper.FadeOut();

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
		MSMonsterManager.instance.AddToEnhanceQueue(monster);
		MoveToEnhanceQueue ();
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
		mediumHelper.FadeOut ();
		smallHelper.FadeIn ();
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
		
		mediumHelper.FadeIn();
		smallHelper.FadeOut();
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
		
		bigHelper.FadeOut();
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
		
		bigHelper.FadeIn();
		smallHelper.FadeOut();
	}

	void AddToSellQueue()
	{
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
		
		bigHelper.FadeOut();
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
		
		bigHelper.FadeIn();
		smallHelper.FadeOut();
	}

	void PickForEvolve()
	{
		if (monster.monster.evolutionMonsterId > 0)
		{
			MSEvolutionManager.instance.TryEvolveMonster(monster, (buddy!=null) ? buddy.monster : null);
			//MSPopupManager.instance.popups.goonScreen.OrganizeEvolution(this);
		}
		else
		{
			//MSPopupManager.instance.popups.goonScreen.DisplayErrorMessage(monster.monster.displayName + " is already at maximum evolution level");
		}
	}
	
	void SpeedUpCombine()
	{
		MSMonsterManager.instance.SpeedUpCombine(monster);
	}

	void OnClick()
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
		TweenAlpha tween = TweenAlpha.Begin(cardBackground.gameObject, .5f, 0);
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
			if (monster.enhancement.expectedStartTimeMillis <= MSUtil.timeNowMillis)
			{
				smallBarRoot.SetActive(true);
				smallBottomLabel.text = " ";
				smallBar.fill = 1 - ((float)monster.enhanceTimeLeft) / ((float)monster.timeToUseEnhance);
				smallBarLabel.text = MSUtil.TimeStringShort(monster.enhanceTimeLeft);
			}
			else
			{
				smallBarRoot.SetActive(false);
				smallBottomLabel.text = monster.enhanceXP + "xp";
			}
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

}
