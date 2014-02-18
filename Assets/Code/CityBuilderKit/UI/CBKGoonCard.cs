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
	UISprite goonPose;
	
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
	
	public CBKGoonInfoPopup infoPopup;

	bool isBaseEnhanceMonster = false;
	
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
	
	PZMonster goon;
	
	const string teamMemberToHealWarning = "Are you sure you want to heal this goon? You won't be able to use them in your team until " +
		"they are fully healed.";

	void OnEnable()
	{
		CBKEventManager.Goon.OnMonsterRemoved += CheckRemovedMonster;
	}

	void OnDisable()
	{
		CBKEventManager.Goon.OnMonsterRemoved -= CheckRemovedMonster;
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
			name = "4 Unavailalbe 3 Enhancing";
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
	
	public void InitLab(PZMonster goon)
	{
		Init (goon);
		SetEnhanceButton(goon);
		
		healCostLabel.text = " ";

		if (goon.userMonster.teamSlotNum == 0)
		{
			addRemoveTeamButton.button.isEnabled = false;
			addRemoveTeamButton.onClick = null;
		}

		healthBar.width = ENHANCE_BAR_WIDTH;
		healthBarBackground.width = ENHANCE_BAR_WIDTH + BAR_BG_WIDTH;

		isBaseEnhanceMonster = false;
		if (CBKMonsterManager.currentEnhancementMonster == goon)
		{
			isBaseEnhanceMonster = true;
			addRemoveTeamButton.button.isEnabled = true;
			addRemoveButtonBackground.spriteName = removeButtonSpriteName;
			addRemoveTeamButton.onClick = ClearEnhanceQueue;
		}
		else if (CBKMonsterManager.currentEnhancementMonster == null)
		{
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
			enhancePercentageLabel.alpha = 0;
			bottomCardLabel.text = (CBKMonsterManager.currentEnhancementMonster.PercentageOfAddedLevelup(goon.enhanceXP) * 100).ToString("N0") + "% for $" + goon.enhanceCost;
		}

	}

	public void InitEvolve(PZMonster goon)
	{

	}

	public void Init(PZMonster goon)
	{
		gameObject.SetActive(true);
		
		this.goon = goon;
		
		goonElementParent.SetActive(true);
		
		goonPose.spriteName = CBKUtil.StripExtensions(goon.monster.imagePrefix) + "Card";
		
		cardBackground.spriteName = backgroundsForElements[goon.monster.monsterElement];
		if (goon.userMonster.teamSlotNum > 0)
		{
			addRemoveButtonBackground.spriteName = onTeamButtonSpriteName;
			addRemoveTeamButton.onClick = RemoveFromTeam;
			addRemoveTeamButton.button.isEnabled = true;
		}
		else
		{
			if (CBKMonsterManager.monstersOnTeam < CBKMonsterManager.TEAM_SLOTS 
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
				CBKEventManager.Popup.OnPopup(infoPopup.gameObject);
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
				bottomCardLabel.text = "Completing in " + CBKUtil.TimeStringShort(goon.combineTimeLeft);
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
			bottomCardLabel.text = "Combining\n" + CBKUtil.TimeStringMed(goon.combineTimeLeft);
			//healButton.label.text = goon.combineFinishGems.ToString();
		}

		if (isBaseEnhanceMonster)
		{
			if (CBKMonsterManager.enhancementFeeders.Count > 0)
			{
				float totalExpToAdd = 0;
				foreach (var item in CBKMonsterManager.enhancementFeeders) 
				{
					totalExpToAdd += item.enhanceXP;
				}
				int finalPercentage = Mathf.FloorToInt((goon.percentageTowardsNextLevel + goon.PercentageOfAddedLevelup(totalExpToAdd)) * 100);

				float progress = ((float)(CBKMonsterManager.enhancementFeeders[0].timeToUseEnhance - CBKMonsterManager.enhancementFeeders[0].enhanceTimeLeft)) 
					/ CBKMonsterManager.enhancementFeeders[0].timeToUseEnhance;
						
				int currentPercentage = Mathf.FloorToInt((goon.percentageTowardsNextLevel +
					progress * goon.PercentageOfAddedLevelup(CBKMonsterManager.enhancementFeeders[0].enhanceXP)) * 100);
				//Debug.Log ("Final: " + finalPercentage + ", Current: " + currentPercentage);

				healthBar.fillAmount = (currentPercentage % 1);
				enhancePercentageLabel.text = (currentPercentage%100) + "% + " + (finalPercentage - currentPercentage) + "%";
			}
			else
			{
				healthBar.fillAmount = goon.percentageTowardsNextLevel;
				enhancePercentageLabel.text = ((int)(goon.percentageTowardsNextLevel * 100)) + "%";
			}
		}
	}
	
	void AddToTeam()
	{
		Debug.Log("Add to team");
		CBKMonsterManager.instance.AddToTeam(goon);
	}
	
	void RemoveFromTeam()
	{
		//CBKMonsterManager.instance.RemoveFromTeam(goon);
	}

	void ClearEnhanceQueue()
	{
		CBKMonsterManager.instance.ClearEnhanceQueue();
	}
	
	void AddToEnhanceQueue()
	{
		if (CBKMonsterManager.currentEnhancementMonster != null)
		{
			if (CBKResourceManager.resources[(int)ResourceType.CASH-1] < goon.enhanceCost)
			{
				CBKEventManager.Popup.CreateButtonPopup("Need more mulah", new string[]{"Okay"}, new Action[]{CBKEventManager.Popup.CloseTopPopupLayer}, true);
				return;
			}
			if (goon.userMonster.teamSlotNum > 0)
			{
				PopupTeamMemberToEnhanceQueue(goon);
			}
			else
			{
				CBKMonsterManager.instance.AddToEnhanceQueue(goon);
			}
		}
		else
		{
			CBKMonsterManager.instance.AddToEnhanceQueue(goon);
		}
	}
	
	void AddToHealQueue()
	{
		if (goon.isHealing)
		{
			return;
		}
		if (CBKResourceManager.resources[(int)ResourceType.CASH-1] < goon.healCost)
		{
			CBKEventManager.Popup.CreateButtonPopup("Need more mulah", new string[]{"Okay"}, new Action[]{CBKEventManager.Popup.CloseTopPopupLayer}, true);
		}
		else if (goon.userMonster.teamSlotNum > 0)
		{
			PopupTeamMemberToHealingQueue(goon);
		}
		else
		{
			CBKMonsterManager.instance.AddToHealQueue(goon);
		}
	}
	
	void SpeedUpCombine()
	{
		CBKMonsterManager.instance.SpeedUpCombine(goon);
	}
	
	void PopupTeamMemberToEnhanceQueue(PZMonster monster)
	{
		CBKEventManager.Popup.CreateButtonPopup(teamMemberToHealWarning, new string[]{"Yes", "No"},
			new Action[]{delegate{RemoveFromTeam(); CBKMonsterManager.instance.AddToEnhanceQueue(monster);
				CBKEventManager.Popup.CloseTopPopupLayer();}, 
				CBKEventManager.Popup.CloseTopPopupLayer}, true);
	}
	
	void PopupTeamMemberToHealingQueue(PZMonster monster)
	{
		CBKEventManager.Popup.CreateButtonPopup(teamMemberToHealWarning, new string[]{"Yes", "No"},
			new Action[]{delegate{RemoveFromTeam();CBKMonsterManager.instance.AddToHealQueue(monster); 
				CBKEventManager.Popup.CloseTopPopupLayer();}, 
				CBKEventManager.Popup.CloseTopPopupLayer}, true);
	}

	void CheckRemovedMonster(long userMonsterId)
	{
		if (goon != null && goon.userMonster != null && goon.userMonster.userMonsterId == userMonsterId)
		{
			gameObject.SetActive(false);
		}
	}

}
