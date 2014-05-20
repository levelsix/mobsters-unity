using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class MSGoonCard : MonoBehaviour {
	
	#region Editor Pointers
	
	[SerializeField]
	GameObject goonElementParent;
	
	[SerializeField]
	UISprite cardBackground;
	
	[SerializeField]
	UI2DSprite goonPose;
	
	[SerializeField]
	public MSActionButton addRemoveTeamButton;
	
	[SerializeField]
	UISprite addRemoveButtonBackground;

	[SerializeField]
	public MSActionButton healButton;
	
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
	MSFillBar underHealthBar;

	[SerializeField]
	UISprite healthBarBackground;
	
	[SerializeField]
	UILabel nameLabel;
	
	[SerializeField]
	UILabel bottomCardLabel;

	[SerializeField]
	UILabel enhancePercentageLabel;
	
	[SerializeField]
	Color darkenColor;

	[SerializeField]
	MSActionButton infoButton;

	[SerializeField]
	GameObject bottomHolder;
	
	public MSGoonInfoPopup infoPopup;

	bool isBaseEnhanceMonster = false;

	[HideInInspector]
	public MSGoonCard buddy = null;

	bool isBuddyParent = false;

	Vector3 buddyOffset = new Vector3(15,-15);

	[SerializeField]
	GameObject extraDots;
	
	#endregion
	
	#region Image Constants
	
	static readonly Dictionary<Element, string> backgroundsForElements = new Dictionary<Element, string>()
	{
		{Element.DARK, "nightcard"},
		{Element.FIRE, "firecard"},
		{Element.EARTH, "earthcard"},
		{Element.LIGHT, "lightcard"},
		{Element.WATER, "watercard"},
		{Element.ROCK, "earthcard"},
		{Element.NO_ELEMENT, "nightcard"}
	};
	
	static readonly Dictionary<Quality, string> ribbonsForRarity = new Dictionary<Quality, string>()
	{
		{Quality.COMMON, "commontag"},
		{Quality.EPIC, "epictag"},
		{Quality.LEGENDARY, "legendarytag"},
		{Quality.RARE, "raretag"},
		{Quality.ULTRA, "ultratag"}
	};
	
	const string addButtonSpriteName = "addteam";
	const string onTeamButtonSpriteName = "onteam";
	const string removeButtonSpriteName = "removeteam";
	
	const string emptyBackground = "emptyslot";
	
	const string healIcon = "healbutton";
	const string gemIcon = "diamond";

	const int HEALTH_BAR_WIDTH = 80;
	const int ENHANCE_BAR_WIDTH = 160;
	const int BAR_BG_WIDTH = 4;

	#endregion
	
	public PZMonster goon;

	const string teamMemberToHealWarning = "Are you sure you want to heal this goon? You won't be able to use them in your team until " +
		"they are fully healed.";

	bool healMode;

	public MonsterStatus cardMode = MonsterStatus.HEALTHY;

	GoonScreenMode goonScreenMode;

	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory += CheckRemovedMonster;
		MSActionManager.Goon.OnEnhanceQueueChanged += OnEnhancementQueueChanged;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory -= CheckRemovedMonster;
		MSActionManager.Goon.OnEnhanceQueueChanged -= OnEnhancementQueueChanged;
		addRemoveTeamButton.onClick = null;
		healButton.onClick = null;
	}

	public void Init(PZMonster goon, GoonScreenMode mode)
	{
		goonScreenMode = mode;
		switch (mode) {
		case GoonScreenMode.HEAL:
			InitHeal(goon);
			break;
		case GoonScreenMode.SELL:
			InitSell();
			break;
		case GoonScreenMode.ENHANCE:
			InitLab(goon);
			break;
		default:
				break;
		}
	}

	public void InitHeal(PZMonster goon)
	{
		healMode = true;

		Setup (goon);
		SetHealButton(goon);

		bottomCardLabel.text = " ";
		enhancePercentageLabel.text = " ";

		healthBar.max = HEALTH_BAR_WIDTH;
		healthBarBackground.width = HEALTH_BAR_WIDTH + BAR_BG_WIDTH;

		isBaseEnhanceMonster = false;

		cardMode = goon.monsterStatus;

		SetName();
	}

	void SetName()
	{
		switch (cardMode)
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
		name += " " + goon.monster.monsterId + " " + goon.userMonster.currentLvl + " " + goon.userMonster.userMonsterId;
	}
	
	public void InitLab(PZMonster goon)
	{
		healMode = false;

		cardMode = MonsterStatus.HEALTHY;
		Setup (goon);
		SetEnhanceButton(goon);
		
		SetName ();

		healCostLabel.text = " ";

		if (goon.userMonster.teamSlotNum == 0)
		{
			addRemoveTeamButton.button.isEnabled = false;
			addRemoveTeamButton.onClick = null;
		}

		healthBar.max = ENHANCE_BAR_WIDTH;
		healthBarBackground.width = ENHANCE_BAR_WIDTH + BAR_BG_WIDTH;
		healthBarText.text = "";

		isBaseEnhanceMonster = false;
		if (MSMonsterManager.instance.currentEnhancementMonster == goon)
		{
			isBaseEnhanceMonster = true;
			addRemoveTeamButton.button.isEnabled = true;
			addRemoveButtonBackground.spriteName = removeButtonSpriteName;
			addRemoveTeamButton.button.normalSprite = removeButtonSpriteName;
			addRemoveTeamButton.onClick = ClearEnhanceQueue;
			bottomCardLabel.text = " ";
			healthBarBackground.alpha = 1;

		}
		else if (MSMonsterManager.instance.currentEnhancementMonster == null)
		{
			enhancePercentageLabel.alpha = 1;
			if (goon.userMonster.currentLvl >= goon.monster.maxLevel)
			{
				healthBar.fill = 1;
				enhancePercentageLabel.text = "MAX LEVEL";
			}
			else
			{
				healthBar.fill = goon.percentageTowardsNextLevel;
				enhancePercentageLabel.text = ((int)(goon.percentageTowardsNextLevel * 100)) + "%";
			}
		}
		else
		{
			healthBarBackground.alpha = 0;
			underHealthBar.fill = 0;
			enhancePercentageLabel.text = " ";
			SetEnhancementBonusText();
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

			if (MSMonsterManager.instance.GetMonstersByMonsterId(goon.monster.evolutionCatalystMonsterId).Count > 0)
			{
				name = "1 2 Card " + goon.monster.monsterId + " " + goon.userMonster.userMonsterId;
			}
			else
			{
				name = "2 2 NoSci " + goon.monster.monsterId + " " + goon.userMonster.userMonsterId;
			}
		}
		else
		{
			extraDots.SetActive(true);

			name = "4 3 NoBuddy " + goon.monster.monsterId + " " + goon.userMonster.currentLvl + " " + goon.userMonster.userMonsterId;
		}

		healButton.onClick = AddToEvolveScreen;
	}

	public void InitScientist(long userMonsterId)
	{
		if (userMonsterId > 0)
		{
			goon = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterId==userMonsterId);
			Setup (goon);
			bottomHolder.SetActive(false);
			infoButton.icon.alpha = 1;
			goonPose.alpha = 1;
		}
		else
		{
			nameLabel.text = "Missing Scientist";
			cardBackground.spriteName = emptyBackground;
			goonPose.alpha = 0;
			infoButton.icon.alpha = 0;
		}
	}

	/// <summary>
	/// Inits the card to sell mode.
	/// </summary>
	public void InitSell()
	{
		//Since we've already got a goon from heal mode, that makes this part easy
		if (goon.sellable)
		{
			cardMode = MonsterStatus.HEALTHY;
		}

		SetName();

		gameObject.SetActive(goon.sellable);
		healButton.onClick = AddToSellQueue;

		healthBarBackground.alpha = 0;
		enhancePercentageLabel.text = "";
		healCostLabel.text = "";
		bottomCardLabel.text = "$" + goon.sellValue;

	}

	public void Refresh()
	{

	}

	void RefreshHeal()
	{

	}

	void RefreshEnhance()
	{

	}

	void RefreshEvolve()
	{

	}

	void SetEnhancementBonusText()
	{
		if (goon == MSMonsterManager.instance.currentEnhancementMonster)
		{
			bottomCardLabel.text = " ";
		}
		else
		{
			bottomCardLabel.text = (MSMonsterManager.instance.currentEnhancementMonster.PercentageOfAddedLevelup(goon.enhanceXP) * 100).ToString("N0") + "% for $" + goon.enhanceCost;
		}
	}

	void Setup(PZMonster goon)
	{
		extraDots.SetActive(false);

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

		gameObject.SetActive(true);
		
		this.goon = goon;
		
		goonElementParent.SetActive(true);

		string goonImageBase = MSUtil.StripExtensions(goon.monster.imagePrefix);
		StartCoroutine(MSSpriteUtil.instance.SetSprite(goonImageBase, goonImageBase + "Card", goonPose));

		if (goon.monster.monsterElement == Element.NO_ELEMENT)
		{
			Debug.LogWarning("Why the fuck does " + goon.monster.displayName + " have no element?!");
		}
		cardBackground.spriteName = backgroundsForElements[goon.monster.monsterElement];
		if (goon.userMonster.teamSlotNum > 0)
		{
			addRemoveTeamButton.button.isEnabled = true;
			addRemoveButtonBackground.spriteName = onTeamButtonSpriteName;
			addRemoveTeamButton.button.normalSprite = onTeamButtonSpriteName;
		}
		else
		{
			if (MSMonsterManager.monstersOnTeam < MSMonsterManager.TEAM_SLOTS 
				&& !goon.isHealing && !goon.isEnhancing && goon.userMonster.isComplete)
			{
				addRemoveTeamButton.onClick = AddToTeam;
				addRemoveTeamButton.button.isEnabled = true;
			}
			else
			{
				addRemoveTeamButton.onClick = null;
				addRemoveTeamButton.button.isEnabled = false;
			}
			addRemoveButtonBackground.spriteName = addButtonSpriteName;
			addRemoveTeamButton.button.normalSprite = addButtonSpriteName;
		}

		SetTextOverCard (goon);
		
		rarityRibbon.spriteName = ribbonsForRarity[goon.monster.quality];
		rarityLabel.text = goon.monster.quality.ToString();
		
		healthBar.fill = ((float)goon.currHP) / goon.maxHP;
		healthBarText.text = goon.currHP + "/" + goon.maxHP;
		
		nameLabel.text = goon.monster.displayName + "(LVL " + goon.userMonster.currentLvl + ")";

		infoButton.onClick = delegate 
			{
				MSActionManager.Popup.OnPopup(infoPopup.GetComponent<MSPopup>());
				infoPopup.Init(goon);
			};
	}

	void SetTextOverCard (PZMonster goon)
	{
		if (!goon.userMonster.isComplete)
		{
			healthBarBackground.alpha = 0;
			underHealthBar.fill = 0;
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
		else if (goon.isHealing)
		{
			healthBarBackground.alpha = 0;
			underHealthBar.fill = 0;
			bottomCardLabel.text = "Healing";
			TintElements(true);
		}
		else if (goon.isEnhancing)
		{
			healthBarBackground.alpha = 0;
			underHealthBar.fill = 0;
			bottomCardLabel.text = "Enhancing";
			TintElements(true);
		}
		else
		{
			healthBarBackground.alpha = 1;
			underHealthBar.fill = 0;
			bottomCardLabel.text = " ";
			TintElements(false);
		}
	}
	
	void SetEnhanceButton(PZMonster goon)
	{
		if (goon.userMonster.isComplete && !goon.isHealing && !goon.isEnhancing)
		{
			healButton.onClick = AddToEnhanceQueue;
		}
	}
	
	void SetHealButton(PZMonster goon)
	{
		if (goon.userMonster.isComplete && !goon.isHealing && !goon.isEnhancing && goon.currHP < goon.maxHP)
		{
			healButton.onClick = AddToHealQueue;
		}
		else
		{
			healButton.onClick = null;
		}

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
	
	public void InitEmptyTeam()
	{
		gameObject.SetActive(true);
		goonElementParent.SetActive(false);
		
		cardBackground.spriteName = emptyBackground;
		
		bottomCardLabel.text = "Team Slot \nEmpty";
	}
	
	public void InitEmptyReserve()
	{
		gameObject.SetActive(true);
		
		goonElementParent.SetActive(false);
		
		cardBackground.spriteName = emptyBackground;
		
		bottomCardLabel.text = "Reserve Slot \nEmpty";
	}
	
	public void InitSlotForPurchase()
	{
		
		gameObject.SetActive(true);
		
		goonElementParent.SetActive(false);
		
		cardBackground.spriteName = emptyBackground;
		
		bottomCardLabel.text = "Slot For \nPurchase";
	}
	
	void Update()
	{
		if (goon != null && goon.userMonster.numPieces >= goon.monster.numPuzzlePieces && !goon.userMonster.isComplete)
		{
			bottomCardLabel.text = "Combining\n" + MSUtil.TimeStringMed(goon.combineTimeLeft);
			//healButton.label.text = goon.combineFinishGems.ToString();
		}

		if (isBaseEnhanceMonster)
		{
			if (MSMonsterManager.instance.enhancementFeeders.Count > 0)
			{
				int totalExpToAdd = 0;
				foreach (var item in MSMonsterManager.instance.enhancementFeeders) 
				{
					totalExpToAdd += item.enhanceXP;
				}
				int finalPercentage = Mathf.FloorToInt(goon.LevelForMonster(goon.userMonster.currentExp + totalExpToAdd) * 100);

				float progress = ((float)(MSMonsterManager.instance.enhancementFeeders[0].timeToUseEnhance - MSMonsterManager.instance.enhancementFeeders[0].enhanceTimeLeft)) 
					/ MSMonsterManager.instance.enhancementFeeders[0].timeToUseEnhance;
						
				int currAddedExp = Mathf.FloorToInt(MSMonsterManager.instance.enhancementFeeders[0].enhanceXP * progress);

				float currentLevel = goon.LevelForMonster(goon.userMonster.currentExp + currAddedExp);

				float currentPercentage = (currentLevel - goon.userMonster.currentLvl) * 100;
				//Debug.Log ("Final: " + finalPercentage + ", Current: " + currentPercentage);

				//Debug.LogWarning("Final perc: " + finalPercentage + ", progress: " + progress + ", currPerc: " + currentPercentage);

				if (currentPercentage > 100)
				{
					healthBar.fill = 0;
				}
				else
				{
					healthBar.fill = goon.percentageTowardsNextLevel;
				}

				underHealthBar.fill = (currentPercentage / 100f) % 1;
				enhancePercentageLabel.text = (Mathf.FloorToInt(currentPercentage)%100) + "% + " + (finalPercentage - Mathf.FloorToInt(currentLevel*100)) + "%";
			}
			else
			{
				underHealthBar.fill = 0;
				healthBar.fill = goon.percentageTowardsNextLevel;
				enhancePercentageLabel.text = ((int)(goon.percentageTowardsNextLevel * 100)) + "%";
			}
		}
	}
	
	void AddToTeam()
	{
		if (goon.userMonster.teamSlotNum == 0)
		{
			MSMonsterManager.instance.AddToTeam(goon);
		}
	}

	void ClearEnhanceQueue()
	{
		MSMonsterManager.instance.ClearEnhanceQueue();
	}
	
	void AddToEnhanceQueue()
	{
		if (MSMonsterManager.instance.currentEnhancementMonster != null
		    && MSMonsterManager.instance.currentEnhancementMonster.monster != null
		    && MSMonsterManager.instance.currentEnhancementMonster.monster.monsterId > 0)
		{
			if (goon.userMonster.teamSlotNum > 0)
			{
				PopupTeamMemberToEnhanceQueue(goon);
			}
			else
			{
				MSMonsterManager.instance.AddToEnhanceQueue(goon);
			}
		}
		else
		{
			MSMonsterManager.instance.AddToEnhanceQueue(goon);
		}
	}
	
	void AddToHealQueue()
	{
		if (goon.isHealing)
		{
			return;
		}
		MSHospitalManager.instance.AddToHealQueue(goon);
		SetName();
		MSPopupManager.instance.popups.goonScreen.goonTable.Reposition();
	}

	void AddToSellQueue()
	{
		MSActionManager.Goon.OnMonsterAddQueue(goon);
		gameObject.SetActive(false);
		MSPopupManager.instance.popups.goonScreen.goonTable.Reposition();
	}

	void AddToEvolveScreen()
	{
		if (goon.monster.evolutionMonsterId > 0)
		{
			MSEvolutionManager.instance.TryEvolveMonster(goon, (buddy!=null) ? buddy.goon : null);
			MSPopupManager.instance.popups.goonScreen.OrganizeEvolution(this);
		}
		else
		{
			MSPopupManager.instance.popups.goonScreen.DisplayErrorMessage(goon.monster.displayName + " is already at maximum evolution level");
		}
	}
	
	void SpeedUpCombine()
	{
		MSMonsterManager.instance.SpeedUpCombine(goon);
	}
	
	void PopupTeamMemberToEnhanceQueue(PZMonster monster)
	{
		MSActionManager.Popup.CreateButtonPopup(teamMemberToHealWarning, new string[]{"Yes", "No"},
			new Action[]{delegate{MSMonsterManager.instance.AddToEnhanceQueue(monster);
				MSActionManager.Popup.CloseTopPopupLayer();}, 
				MSActionManager.Popup.CloseTopPopupLayer});
	}
	
	void PopupTeamMemberToHealingQueue(PZMonster monster)
	{
		MSActionManager.Popup.CreateButtonPopup(teamMemberToHealWarning, new string[]{"Yes", "No"},
			new Action[]{delegate{MSHospitalManager.instance.AddToHealQueue(monster); 
				MSActionManager.Popup.CloseTopPopupLayer();}, 
				MSActionManager.Popup.CloseTopPopupLayer});
	}

	void CheckRemovedMonster(long userMonsterId)
	{
		if (goon != null && goon.userMonster != null && goon.userMonster.userMonsterId == userMonsterId)
		{
			//GetComponent<MSSimplePoolable>().Pool();
		}
	}

	void OnEnhancementQueueChanged()
	{
		if (MSMonsterManager.instance.currentEnhancementMonster != null 
		    && goon != null && goon.monster != null && goon.monster.monsterId > 0)
		{
			SetEnhancementBonusText();
		}
	}

}
