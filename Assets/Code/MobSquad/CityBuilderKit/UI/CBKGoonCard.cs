using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class CBKGoonCard : MonoBehaviour {
	
	#region Editor Pointers
	
	[SerializeField]
	GameObject goonElementParent;
	
	[SerializeField]
	UISprite cardBackground;
	
	[SerializeField]
	UI2DSprite goonPose;
	
	[SerializeField]
	public CBKActionButton addRemoveTeamButton;
	
	[SerializeField]
	UISprite addRemoveButtonBackground;

	[SerializeField]
	public CBKActionButton healButton;
	
	[SerializeField]
	UILabel healCostLabel;
	
	[SerializeField]
	UISprite rarityRibbon;
	
	[SerializeField]
	UILabel rarityLabel;
	
	[SerializeField]
	UISprite healthBar;

	[SerializeField]
	UISprite underHealthBar;

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
	CBKActionButton infoButton;

	[SerializeField]
	GameObject bottomHolder;
	
	public CBKGoonInfoPopup infoPopup;

	bool isBaseEnhanceMonster = false;

	[HideInInspector]
	public CBKGoonCard buddy = null;

	bool isBuddyParent = false;

	Vector3 buddyOffset = new Vector3(15,-15);

	[SerializeField]
	GameObject extraDots;
	
	#endregion
	
	#region Image Constants
	
	static readonly Dictionary<MonsterProto.MonsterElement, string> backgroundsForElements = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARKNESS, "nightcard"},
		{MonsterProto.MonsterElement.FIRE, "firecard"},
		{MonsterProto.MonsterElement.GRASS, "earthcard"},
		{MonsterProto.MonsterElement.LIGHTNING, "lightcard"},
		{MonsterProto.MonsterElement.WATER, "watercard"}
	};

	static readonly Dictionary<MonsterProto.MonsterElement, string> healthBarForElements = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARKNESS, "nightcardhealthbar"},
		{MonsterProto.MonsterElement.FIRE, "firecardhealthbar"},
		{MonsterProto.MonsterElement.GRASS, "earthcardhealthbar"},
		{MonsterProto.MonsterElement.LIGHTNING, "lightcardhealthbar"},
		{MonsterProto.MonsterElement.WATER, "watercardhealthbar"}
	};
	
	static readonly Dictionary<MonsterProto.MonsterQuality, string> ribbonsForRarity = new Dictionary<MonsterProto.MonsterQuality, string>()
	{
		{MonsterProto.MonsterQuality.COMMON, "commontag"},
		{MonsterProto.MonsterQuality.EPIC, "epictag"},
		{MonsterProto.MonsterQuality.LEGENDARY, "legendarytag"},
		{MonsterProto.MonsterQuality.RARE, "raretag"},
		{MonsterProto.MonsterQuality.ULTRA, "ultratag"}
	};
	
	const string addButtonSpriteName = "addteam";
	const string onTeamButtonSpriteName = "onteam";
	const string removeButtonSpriteName = "removeteam";
	
	const string emptyBackground = "emptyslot";
	
	const string healIcon = "healbutton";
	const string gemIcon = "diamond";

	const int HEALTH_BAR_WIDTH = 88;
	const int ENHANCE_BAR_WIDTH = 165;
	const int BAR_BG_WIDTH = 4;

	#endregion
	
	public PZMonster goon;
	
	const string teamMemberToHealWarning = "Are you sure you want to heal this goon? You won't be able to use them in your team until " +
		"they are fully healed.";

	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterRemoved += CheckRemovedMonster;
		MSActionManager.Goon.OnEnhanceQueueChanged += OnEnhancementQueueChanged;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterRemoved -= CheckRemovedMonster;
		MSActionManager.Goon.OnEnhanceQueueChanged -= OnEnhancementQueueChanged;
		addRemoveTeamButton.onClick = null;
		healButton.onClick = null;
	}
	
	public void InitHeal(PZMonster goon)
	{
		Init (goon);
		SetHealButton(goon);

		bottomCardLabel.text = " ";
		enhancePercentageLabel.text = " ";

		healthBar.width = HEALTH_BAR_WIDTH;
		healthBarBackground.width = HEALTH_BAR_WIDTH + BAR_BG_WIDTH;

		isBaseEnhanceMonster = false;

		SetName();
	}

	void SetName(bool healMode = true)
	{
		if (healMode)
		{
			if (goon.userMonster.numPieces < goon.monster.numPuzzlePieces)
			{
				name = "4 Unavailable 5 Piece";
			}
			else if (goon.combineTimeLeft > 0)
			{
				name = "4 Unavailable 4 Combining";
			}
			else if (goon.isEnhancing)
			{
				name = "4 Unavailable 3 Enhancing";
			}
			else if (goon.isHealing)
			{
				name = "4 Unavailable 2 Healing";
			}
			else if (goon.currHP < goon.maxHP)
			{
				name = "1 Injured 2 Card";
			}
			else
			{
				name = "3 Healthy 2 Card";
			}
		}
		else
		{
			name = "3 Healthy 2 Card";
		}
		name += " " + goon.monster.monsterId + " " + goon.userMonster.currentLvl + " " + goon.userMonster.userMonsterId;
	}
	
	public void InitLab(PZMonster goon)
	{

		Init (goon);
		SetEnhanceButton(goon);
		
		SetName (false);

		healCostLabel.text = " ";

		if (goon.userMonster.teamSlotNum == 0)
		{
			addRemoveTeamButton.button.isEnabled = false;
			addRemoveTeamButton.onClick = null;
		}

		healthBar.width = ENHANCE_BAR_WIDTH;
		healthBarBackground.width = ENHANCE_BAR_WIDTH + BAR_BG_WIDTH;

		isBaseEnhanceMonster = false;
		if (MSMonsterManager.currentEnhancementMonster == goon)
		{
			isBaseEnhanceMonster = true;
			addRemoveTeamButton.button.isEnabled = true;
			addRemoveButtonBackground.spriteName = removeButtonSpriteName;
			addRemoveTeamButton.onClick = ClearEnhanceQueue;
			bottomCardLabel.text = " ";
			healthBarBackground.alpha = 1;
			healthBar.alpha = 1;
			underHealthBar.alpha = 1;

		}
		else if (MSMonsterManager.currentEnhancementMonster == null)
		{
			enhancePercentageLabel.alpha = 1;
			if (goon.userMonster.currentLvl >= goon.monster.maxLevel)
			{
				healthBar.fillAmount = 1;
				enhancePercentageLabel.text = "MAX LEVEL";
			}
			else
			{
				healthBar.fillAmount = goon.percentageTowardsNextLevel;
				enhancePercentageLabel.text = ((int)(goon.percentageTowardsNextLevel * 100)) + "%";
			}
		}
		else
		{
			healthBar.alpha = 0;
			healthBarBackground.alpha = 0;
			underHealthBar.alpha = 0;
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
	public void InitEvolve(CBKGoonCard buddy = null)
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
			Init (goon);
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

	void SetEnhancementBonusText()
	{
		if (goon == MSMonsterManager.currentEnhancementMonster)
		{
			bottomCardLabel.text = " ";
		}
		else
		{
			bottomCardLabel.text = (MSMonsterManager.currentEnhancementMonster.PercentageOfAddedLevelup(goon.enhanceXP) * 100).ToString("N0") + "% for $" + goon.enhanceCost;
		}
	}

	public void Init(PZMonster goon)
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
		
		goonPose.sprite2D = CBKAtlasUtil.instance.GetSprite("Cards/HD/" + MSUtil.StripExtensions(goon.monster.imagePrefix) + "Card");
		if (goonPose.sprite2D != null)
		{
			goonPose.width = (int)goonPose.sprite2D.rect.width;
			goonPose.height = (int)goonPose.sprite2D.rect.height;
		}
		
		cardBackground.spriteName = backgroundsForElements[goon.monster.monsterElement];
		if (goon.userMonster.teamSlotNum > 0)
		{
			addRemoveButtonBackground.spriteName = onTeamButtonSpriteName;
			addRemoveTeamButton.button.isEnabled = true;
		}
		else
		{
			if (MSMonsterManager.monstersOnTeam < MSMonsterManager.TEAM_SLOTS 
				&& !goon.isHealing && !goon.isEnhancing && goon.userMonster.isComplete)
			{
				addRemoveButtonBackground.spriteName = addButtonSpriteName;
				addRemoveTeamButton.onClick = AddToTeam;
				addRemoveTeamButton.button.isEnabled = true;
			}
			else
			{
				addRemoveTeamButton.button.isEnabled = false;
			}
		}
		
		healthBar.spriteName = healthBarForElements[goon.monster.monsterElement];

		SetTextOverCard (goon);
		
		rarityRibbon.spriteName = ribbonsForRarity[goon.monster.quality];
		rarityLabel.text = goon.monster.quality.ToString();
		
		healthBar.fillAmount = ((float)goon.currHP) / goon.maxHP;
		
		nameLabel.text = goon.monster.displayName + "(LVL " + goon.userMonster.currentLvl + ")";

		infoButton.onClick = delegate 
			{
				infoPopup.Init(goon);
				MSActionManager.Popup.OnPopup(infoPopup.gameObject);
			};
	}

	void SetTextOverCard (PZMonster goon)
	{
		if (!goon.userMonster.isComplete)
		{
			healthBar.alpha = 0;
			healthBarBackground.alpha = 0;
			underHealthBar.alpha = 0;
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
			healthBar.alpha = 0;
			healthBarBackground.alpha = 0;
			underHealthBar.alpha = 0;
			bottomCardLabel.text = "Healing";
			TintElements(true);
		}
		else if (goon.isEnhancing)
		{
			healthBar.alpha = 0;
			healthBarBackground.alpha = 0;
			underHealthBar.alpha = 0;
			bottomCardLabel.text = "Enhancing";
			TintElements(true);
		}
		else
		{
			healthBar.alpha = 1;
			healthBarBackground.alpha = 1;
			underHealthBar.alpha = 0;
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
			if (MSMonsterManager.enhancementFeeders.Count > 0)
			{
				float totalExpToAdd = 0;
				foreach (var item in MSMonsterManager.enhancementFeeders) 
				{
					totalExpToAdd += item.enhanceXP;
				}
				int finalPercentage = Mathf.FloorToInt((goon.percentageTowardsNextLevel + goon.PercentageOfAddedLevelup(totalExpToAdd)) * 100);

				float progress = ((float)(MSMonsterManager.enhancementFeeders[0].timeToUseEnhance - MSMonsterManager.enhancementFeeders[0].enhanceTimeLeft)) 
					/ MSMonsterManager.enhancementFeeders[0].timeToUseEnhance;
						
				float currentPercentage = (goon.percentageTowardsNextLevel + progress * goon.PercentageOfAddedLevelup(MSMonsterManager.enhancementFeeders[0].enhanceXP)) * 100;
				//Debug.Log ("Final: " + finalPercentage + ", Current: " + currentPercentage);

				//Debug.LogWarning("Final perc: " + finalPercentage + ", progress: " + progress + ", currPerc: " + currentPercentage);

				if (currentPercentage > 100)
				{
					healthBar.fillAmount = 0;
				}
				else
				{
					healthBar.fillAmount = goon.percentageTowardsNextLevel;
				}

				underHealthBar.fillAmount = (currentPercentage / 100f) % 1;
				enhancePercentageLabel.text = (Mathf.FloorToInt(currentPercentage)%100) + "% + " + (finalPercentage - Mathf.FloorToInt(currentPercentage)) + "%";
			}
			else
			{
				underHealthBar.fillAmount = 0;
				healthBar.fillAmount = goon.percentageTowardsNextLevel;
				enhancePercentageLabel.text = ((int)(goon.percentageTowardsNextLevel * 100)) + "%";
			}
		}
	}
	
	void AddToTeam()
	{
		Debug.Log("Add to team");
		MSMonsterManager.instance.AddToTeam(goon);
	}
	
	void RemoveFromTeam()
	{
		//CBKMonsterManager.instance.RemoveFromTeam(goon);
	}

	void ClearEnhanceQueue()
	{
		MSMonsterManager.instance.ClearEnhanceQueue();
	}
	
	void AddToEnhanceQueue()
	{
		if (MSMonsterManager.currentEnhancementMonster != null)
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
		if (goon.userMonster.teamSlotNum > 0)
		{
			PopupTeamMemberToHealingQueue(goon);
		}
		else
		{
			MSMonsterManager.instance.AddToHealQueue(goon);
		}
	}

	void AddToEvolveScreen()
	{
		if (goon.monster.evolutionMonsterId > 0)
		{
			MSEvolutionManager.instance.TryEvolveMonster(goon, (buddy!=null) ? buddy.goon : null);
			CBKGoonScreen.instance.OrganizeEvolution(this);
		}
		else
		{
			CBKGoonScreen.instance.DisplayErrorMessage(goon.monster.displayName + " is already at maximum evolution level");
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
			new Action[]{delegate{MSMonsterManager.instance.AddToHealQueue(monster); 
				MSActionManager.Popup.CloseTopPopupLayer();}, 
				MSActionManager.Popup.CloseTopPopupLayer});
	}

	void CheckRemovedMonster(long userMonsterId)
	{
		if (goon != null && goon.userMonster != null && goon.userMonster.userMonsterId == userMonsterId)
		{
			gameObject.SetActive(false);
		}
	}

	void OnEnhancementQueueChanged()
	{
		if (MSMonsterManager.currentEnhancementMonster != null 
		    && goon != null && goon.monster != null && goon.monster.monsterId > 0)
		{
			SetEnhancementBonusText();
		}
	}

}
