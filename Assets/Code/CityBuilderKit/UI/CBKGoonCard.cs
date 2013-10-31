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
	
	#endregion
	
	PZMonster goon;
	
	const string teamMemberToHealWarning = "Are you sure you want to heal this goon? You won't be able to use them in your team until" +
		"they are fully healed.";
	
	void OnDisable()
	{
		addRemoveTeamButton.onClick = null;
		healButton.onClick = null;
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
			addRemoveTeamButton.button.isEnabled = true;
		}
		else
		{
			if (CBKMonsterManager.instance.monstersOnTeam < CBKMonsterManager.TEAM_SLOTS && !goon.isHealing && goon.userMonster.isComplete)
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
		
		if (!goon.isHealing && goon.currHP < goon.maxHP)
		{
			healButtonParent.SetActive(true);
			healButton.onClick = AddToHealQueue;
			healButton.label.text = "$" + goon.healCost;
		}
		else
		{
			healButtonParent.SetActive(false);
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
	
	public void InitLab(PZMonster goon)
	{
		
	}

	void SetTextOverCard (PZMonster goon)
	{
		if (!goon.userMonster.isComplete)
		{
			overCardLabel.text = goon.userMonster.numPieces + "/" + goon.monster.numPuzzlePieces;  
			TintElements (true);
		}
		else if (goon.isHealing)
		{
			overCardLabel.text = "Healing...";
			TintElements (true);
		}
		else
		{
			overCardLabel.text = "";
			TintElements(false);
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
	
	void AddToHealQueue()
	{
		if (CBKResourceManager.instance.resources[(int)CBKResourceManager.ResourceType.FREE] < goon.healCost)
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
	
	void PopupTeamMemberToHealingQueue(PZMonster monster)
	{
		CBKEventManager.Popup.CreateButtonPopup(teamMemberToHealWarning, new string[]{"Yes", "No"},
			new Action[]{delegate{RemoveFromTeam();CBKMonsterManager.instance.AddToHealQueue(monster); 
				isHealingParent.SetActive(true);CBKEventManager.Popup.CloseTopPopupLayer();}, 
			CBKEventManager.Popup.CloseTopPopupLayer});
	}
}
