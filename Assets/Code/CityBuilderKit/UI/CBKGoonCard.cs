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
	UISprite cardBorder;
	
	[SerializeField]
	UISprite goonPose;
	
	[SerializeField]
	CBKActionButton addRemoveTeamButton;
	
	[SerializeField]
	UISprite addRemoveButtonBackground;
	
	[SerializeField]
	CBKActionButton healButton;
	
	[SerializeField]
	GameObject healButtonParent;
	
	[SerializeField]
	UILabel healCostLabel;
	
	[SerializeField]
	UISprite rarityRibbon;
	
	[SerializeField]
	UILabel rarityLabel;
	
	[SerializeField]
	GameObject isHealingParent;
	
	[SerializeField]
	UISprite healthBar;
	
	[SerializeField]
	UILabel levelLabel;
	
	[SerializeField]
	UILabel nameLabel;
	
	[SerializeField]
	UISprite[] stars;
	
	[SerializeField]
	UILabel overCardLabel;
	
	[SerializeField]
	UIWidget[] darkenedElements;
	
	[SerializeField]
	Color darkenColor;
	
	#endregion
	
	#region Image Constants
	
	static readonly Dictionary<MonsterProto.MonsterElement, string> backgroundsForElements = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARKNESS, "nightcard"},
		{MonsterProto.MonsterElement.FIRE, "firecard"},
		{MonsterProto.MonsterElement.GRASS, "earthcard"},
		{MonsterProto.MonsterElement.LIGHTNING, "suncard"},
		{MonsterProto.MonsterElement.WATER, "watercard"}
	};
	
	static readonly Dictionary<MonsterProto.MonsterElement, string> bordersForElements = new Dictionary<MonsterProto.MonsterElement, string>()
	{
		{MonsterProto.MonsterElement.DARKNESS, "nightborder"},
		{MonsterProto.MonsterElement.FIRE, "fireborder"},
		{MonsterProto.MonsterElement.GRASS, "earthborder"},
		{MonsterProto.MonsterElement.LIGHTNING, "sunborder"},
		{MonsterProto.MonsterElement.WATER, "waterborder"}
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
	const string removeButtonSpriteName = "removeteam";
	
	const string emptyBackground = "emptyslot";
	
	const string healIcon = "healbutton";
	const string gemIcon = "diamond";
	
	#endregion
	
	PZMonster goon;
	
	const string teamMemberToHealWarning = "Are you sure you want to heal this goon? You won't be able to use them in your team until" +
		"they are fully healed.";
	
	void OnDisable()
	{
		addRemoveTeamButton.onClick = null;
		healButton.onClick = null;
	}
	
	public void InitHeal(PZMonster goon)
	{
		Init (goon);
		SetHealButton(goon);
	}
	
	public void InitLab(PZMonster goon)
	{
		Init (goon);
		SetEnhanceButton(goon);
		
		addRemoveTeamButton.button.isEnabled = false;
		addRemoveButtonBackground.alpha = 0;
		addRemoveTeamButton.onClick = null;
	}
	
	public void Init(PZMonster goon)
	{
		gameObject.SetActive(true);
		
		this.goon = goon;
		
		goonElementParent.SetActive(true);
		
		goonPose.spriteName = CBKAtlasUtil.instance.StripExtensions(goon.monster.imagePrefix) + "Card";
		
		cardBackground.spriteName = backgroundsForElements[goon.monster.element];
		cardBorder.spriteName = bordersForElements[goon.monster.element];
		if (goon.userMonster.teamSlotNum > 0)
		{
			addRemoveButtonBackground.spriteName = removeButtonSpriteName;
			addRemoveButtonBackground.alpha = 1;
			addRemoveTeamButton.onClick = RemoveFromTeam;
			Debug.Log("button: " + ((addRemoveTeamButton.button == null) ? "no" : "yes"));
			addRemoveTeamButton.button.isEnabled = true;
		}
		else
		{
			if (CBKMonsterManager.monstersOnTeam < CBKMonsterManager.TEAM_SLOTS 
				&& !goon.isHealing && !goon.isEnhancing && goon.userMonster.isComplete)
			{
				addRemoveButtonBackground.spriteName = addButtonSpriteName;
				addRemoveButtonBackground.alpha = 1;
				addRemoveTeamButton.onClick = AddToTeam;
				addRemoveTeamButton.button.isEnabled = true;
			}
			else
			{
				addRemoveButtonBackground.alpha = 0;
				addRemoveTeamButton.button.isEnabled = false;
			}
		}
		
		SetTextOverCard (goon);
		
		rarityRibbon.spriteName = ribbonsForRarity[goon.monster.quality];
		rarityLabel.text = goon.monster.quality.ToString();
		
		isHealingParent.SetActive(goon.isHealing);
		
		healthBar.fillAmount = ((float)goon.currHP) / goon.maxHP;
		
		levelLabel.text = "LVL " + goon.userMonster.currentLvl.ToString();
		
		nameLabel.text = goon.monster.displayName;
		
		//TODO: Stars
	}

	void SetTextOverCard (PZMonster goon)
	{
		if (!goon.userMonster.isComplete)
		{
			if (goon.userMonster.numPieces < goon.monster.numPuzzlePieces)
			{
				overCardLabel.text = goon.userMonster.numPieces + "/" + goon.monster.numPuzzlePieces;  
			}
			else
			{
				overCardLabel.text = "Completing...";
			}
			TintElements (true);
		}
		else if (goon.isHealing)
		{
			overCardLabel.text = "Healing...";
			TintElements(true);
		}
		else if (goon.isEnhancing)
		{
			overCardLabel.text = "Enhancing...";
			TintElements(true);
		}
		else
		{
			overCardLabel.text = "";
			TintElements(false);
		}
	}
	
	void SetEnhanceButton(PZMonster goon)
	{
		if (goon.userMonster.isComplete && !goon.isHealing && !goon.isEnhancing)
		{
			healButtonParent.SetActive(true);
			if (CBKMonsterManager.currentEnhancementMonster == null)
			{
				healButton.label.text = "ENHANCE";
			}
			else
			{
				healButton.label.text = goon.enhanceCost.ToString();
			}
			healButton.onClick = AddToEnhanceQueue;
		}
		else
		{
			healButtonParent.SetActive(false);
		}
	}
	
	void SetHealButton(PZMonster goon)
	{
		if (goon.userMonster.isComplete && !goon.isHealing && !goon.isEnhancing && goon.currHP < goon.maxHP)
		{
			healButtonParent.SetActive(true);
			healButton.label.text = goon.healCost.ToString();
			healButton.onClick = AddToHealQueue;
		}
		else
		{
			healButtonParent.SetActive(false);
		}
	}
	
	void TintElements(bool darken)
	{
		foreach (UIWidget item in darkenedElements) 
		{
			item.color = darken ? darkenColor : Color.white;
		}
	}
	
	public void InitEmptyTeam()
	{
		gameObject.SetActive(true);
		goonElementParent.SetActive(false);
		
		cardBackground.spriteName = emptyBackground;
		
		overCardLabel.text = "Team Slot \nEmpty";
	}
	
	public void InitEmptyReserve()
	{
		gameObject.SetActive(true);
		
		goonElementParent.SetActive(false);
		
		cardBackground.spriteName = emptyBackground;
		
		overCardLabel.text = "Reserve Slot \nEmpty";
	}
	
	public void InitSlotForPurchase()
	{
		
		gameObject.SetActive(true);
		
		goonElementParent.SetActive(false);
		
		cardBackground.spriteName = emptyBackground;
		
		overCardLabel.text = "Slot For \nPurchase";
	}
	
	void Update()
	{
		if (goon != null && goon.userMonster.numPieces >= goon.monster.numPuzzlePieces && !goon.userMonster.isComplete)
		{
			overCardLabel.text = "Combining\n" + CBKUtil.TimeStringMed(goon.combineTimeLeft);
			healButton.label.text = goon.combineFinishGems.ToString();
		}
	}
	
	void AddToTeam()
	{
		Debug.Log("Add to team");
		CBKMonsterManager.instance.AddToTeam(goon);
	}
	
	void RemoveFromTeam()
	{
		CBKMonsterManager.instance.RemoveFromTeam(goon);
		InitEmptyTeam();
	}
	
	void AddToEnhanceQueue()
	{
		if (CBKMonsterManager.currentEnhancementMonster != null)
		{
			if (CBKResourceManager.resources[(int)CBKResourceManager.ResourceType.FREE] < goon.enhanceCost)
			{
				CBKEventManager.Popup.CreateButtonPopup("Need more mulah", new string[]{"Okay"}, new Action[]{CBKEventManager.Popup.CloseTopPopupLayer});
				return;
			}
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
	
	void AddToHealQueue()
	{
		if (CBKResourceManager.resources[(int)CBKResourceManager.ResourceType.FREE] < goon.healCost)
		{
			CBKEventManager.Popup.CreateButtonPopup("Need more mulah", new string[]{"Okay"}, new Action[]{CBKEventManager.Popup.CloseTopPopupLayer});
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
			CBKEventManager.Popup.CloseTopPopupLayer});
	}
	
	void PopupTeamMemberToHealingQueue(PZMonster monster)
	{
		CBKEventManager.Popup.CreateButtonPopup(teamMemberToHealWarning, new string[]{"Yes", "No"},
			new Action[]{delegate{RemoveFromTeam();CBKMonsterManager.instance.AddToHealQueue(monster); 
				isHealingParent.SetActive(true);CBKEventManager.Popup.CloseTopPopupLayer();}, 
			CBKEventManager.Popup.CloseTopPopupLayer});
	}
}
