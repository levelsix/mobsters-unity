using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKGoonCard : MonoBehaviour {
	
	#region Editor Pointers
	
	[SerializeField]
	GameObject goonElementParent;
	
	[SerializeField]
	UISprite cardBackground;
	
	[SerializeField]
	UISprite cardBorder;
	
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
	GameObject hasNoGoonElements;
	
	[SerializeField]
	UILabel hasNoGoonLabel;
	
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
	
	void OnDisable()
	{
		addRemoveTeamButton.onClick = null;
		healButton.onClick = null;
	}
	
	public void Init(PZMonster goon)
	{
		this.goon = goon;
		
		goonElementParent.SetActive(true);
		hasNoGoonElements.SetActive(false);
		
		cardBackground.spriteName = backgroundsForElements[goon.monster.element];
		cardBorder.spriteName = bordersForElements[goon.monster.element];
		
		if (goon.userMonster.teamSlotNum > 0)
		{
			addRemoveButtonBackground.spriteName = removeButtonSpriteName;
			addRemoveTeamButton.onClick = RemoveFromTeam;
		}
		else
		{
			//TODO: Only show button if team slots available?
			addRemoveButtonBackground.spriteName = addButtonSpriteName;
			addRemoveTeamButton.onClick = AddToTeam;
		}
		
		if (!goon.isHealing && goon.currHP < goon.maxHP)
		{
			healButtonParent.SetActive(true);
			healButton.onClick = AddToHealQueue;
		}
		
		rarityRibbon.spriteName = ribbonsForRarity[goon.monster.quality];
		rarityLabel.text = goon.monster.quality.ToString();
		
		isHealingParent.SetActive(goon.isHealing);
		
		healthBar.fillAmount = ((float)goon.currHP) / goon.maxHP;
		
		levelLabel.text = "LVL " + goon.userMonster.currentLvl.ToString();
		
		nameLabel.text = goon.monster.displayName;
		
		//TODO: Stars
	}
	
	public void InitEmptyTeam()
	{
		hasNoGoonElements.SetActive(true);
		goonElementParent.SetActive(false);
		
		cardBackground.spriteName = emptyBackground;
		
		hasNoGoonLabel.text = "Team Slot \nEmpty";
	}
	
	public void InitEmptyReserve()
	{
		hasNoGoonElements.SetActive(true);
		goonElementParent.SetActive(false);
		
		cardBackground.spriteName = emptyBackground;
		
		hasNoGoonLabel.text = "Reserve Slot \nEmpty";
	}
	
	public void InitSlotForPurchase()
	{
		
		hasNoGoonElements.SetActive(true);
		goonElementParent.SetActive(false);
		
		cardBackground.spriteName = emptyBackground;
		
		hasNoGoonLabel.text = "Slot For \nPurchase";
	}
	
	void AddToTeam()
	{
		Debug.Log("Add to team");
	}
	
	void RemoveFromTeam()
	{
		Debug.Log("Remove from team");
	}
	
	void AddToHealQueue()
	{
		Debug.Log("Add to heal queue");
	}
}
