using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class CBKGoonScreen : MonoBehaviour {
	
	#region UI Elements

	[SerializeField]
	UIDraggablePanel dragPanel;

	[SerializeField]
	CBKGoonTeamCard[] teamCards;
	
	[SerializeField]
	List<CBKGoonCard> reserveCards;
	
	[SerializeField]
	List<CBKMiniHealingBox> bottomMiniBoxes;
	
	[SerializeField]
	CBKGoonCard goonCardPrefab;
	
	[SerializeField]
	CBKMiniHealingBox healBoxPrefab;
	
	[SerializeField]
	GameObject queueParent;
	
	[SerializeField]
	CBKActionButton speedUpButton;
	
	[SerializeField]
	UILabel totalTimeLabel;
	
	[SerializeField]
	CBKMiniHealingBox enhanceBaseBox;
	
	[SerializeField]
	UILabel enhanceDetails;
	
	[SerializeField]
	UISprite enhanceUnderBar;
	
	[SerializeField]
	UISprite enhanceOverBar;
	
	[SerializeField]
	UIPanel scrollPanel;

	[SerializeField]
	CBKGoonGrid goonGrid;
	
	#endregion
	
	bool healMode = true;
	
	#region Properties
	
	int playerSlots
	{
		get
		{
			return CBKWhiteboard.constants.userMonsterConstants.maxNumTeamSlots + CBKWhiteboard.localUser.numAdditionalMonsterSlots;
		}
	}
			
	int lastReserveCardIndex
	{
		get
		{
			return reserveCards.Count - 1;
		}
	}
	
	int lastBox
	{
		get
		{
			return bottomMiniBoxes.Count - 1;
		}
	}
	
	#endregion
	
	#region Constants
	
	static readonly Vector3 cardOffset = new Vector3(240, 0, 0);
	
	static readonly Vector3 boxOffset = new Vector3(-142, 0, 0);
	
	const string removeDialogueBeforeCost = "Are you sure you want to remove this goon from the healing queue? You will be refunded $";
	const string removeDialogueAfterCost = " of the healing cost";
	
	#endregion
	
	public void OrganizeCards()
	{
		OrganizeTeamCards();
		OrganizeReserveCards();
	}
	
	public void InitHeal()
	{	
		healMode = true;
		OrganizeCards();	
		OrganizeHealingQueue (CBKMonsterManager.healingMonsters);
		speedUpButton.onClick = TrySpeedUpHeal;
	}
	
	public void InitEnhance()
	{
		healMode = false;
		OrganizeCards();
		OrganizeEnhanceQueue();
		speedUpButton.onClick = TrySpeedUpEnhance;
	}
	
	void Start()
	{
		scrollPanel.clipRange = new Vector4(scrollPanel.clipRange.x, scrollPanel.clipRange.y, 
			Mathf.Min(scrollPanel.clipRange.z, Screen.width), scrollPanel.clipRange.w);
	}
	
	void OnEnable()
	{
		CBKEventManager.Goon.OnMonsterAddTeam += OnAddTeamMember;
		CBKEventManager.Goon.OnMonsterRemoveTeam += OnRemoveTeamMember;
		CBKEventManager.Goon.OnHealQueueChanged += OnHealQueueChanged;
		CBKEventManager.Goon.OnEnhanceQueueChanged += OnEnhanceQueueChanged;
	}
	
	void OnDisable()
	{
		CBKEventManager.Goon.OnMonsterAddTeam -= OnAddTeamMember;
		CBKEventManager.Goon.OnMonsterRemoveTeam -= OnRemoveTeamMember;
		CBKEventManager.Goon.OnHealQueueChanged -= OnHealQueueChanged;
		CBKEventManager.Goon.OnEnhanceQueueChanged -= OnEnhanceQueueChanged;
	}

	void OrganizeTeamCards ()
	{
		int i;
		for (i = 0; i < CBKMonsterManager.userTeam.Length; i++) 
		{
			if (healMode)
			{
				teamCards[i].InitHeal(CBKMonsterManager.userTeam[i]);
			}
			else
			{
				teamCards[i].InitLab(CBKMonsterManager.userTeam[i]);
			}
		}
	}
	
	void OrganizeReserveCards()
	{
		OrganizeReserveCards(CBKMonsterManager.userTeam, CBKMonsterManager.userMonsters);
	}
	
	void OrganizeReserveCards (PZMonster[] teamGoons, Dictionary<long, PZMonster> playerGoons)
	{
		goonGrid.ClearGrid();

		int i = 0;
		foreach (KeyValuePair<long, PZMonster> item in playerGoons) 
		{
			if (lastReserveCardIndex < i)
			{
				AddReserveCardSlot();
			}
			if (healMode)
			{
				reserveCards[i].InitHeal(item.Value);
			}
			else
			{
				reserveCards[i].InitLab(item.Value);
			}
			goonGrid.AddItemToGrid(reserveCards[i].GetComponent<CBKGridItem>());

			i++;
		}

		if (playerGoons.Count < playerSlots)
		{
			if (lastReserveCardIndex < i)
			{
				AddReserveCardSlot();
			}
			reserveCards[i].InitEmptyReserve();
			goonGrid.AddItemToGrid(reserveCards[i].GetComponent<CBKGridItem>());
			i++;
		}
		
		if (lastReserveCardIndex < i)
		{
			AddReserveCardSlot();
		}
		reserveCards[i].InitSlotForPurchase();
		goonGrid.AddItemToGrid(reserveCards[i].GetComponent<CBKGridItem>());
		
		while (++i < reserveCards.Count)
		{
			reserveCards[i].gameObject.SetActive(false);
		}
	}
	
	void OrganizeHealingQueue()
	{
		OrganizeHealingQueue(CBKMonsterManager.healingMonsters);
	}
	
	void OrganizeHealingQueue(List<PZMonster> healingMonsters)
	{
		if (healingMonsters.Count == 0)
		{
			queueParent.SetActive(false);
			return;
		}
		
		queueParent.SetActive(true);
		
		while(bottomMiniBoxes.Count < healingMonsters.Count)
		{
			AddHealBox();
		}
		int i;
		for (i = 0; i < healingMonsters.Count; i++) 
		{
			bottomMiniBoxes[i].Init (healingMonsters[i]);
			bottomMiniBoxes[i].SetBar(i==0);
		}
		for (; i < bottomMiniBoxes.Count; i++) 
		{
			bottomMiniBoxes[i].gameObject.SetActive(false);
		}
	}
	
	void OrganizeEnhanceQueue()
	{
		if(CBKMonsterManager.currentEnhancementMonster == null)
		{
			queueParent.SetActive(false);
			return;
		}
		
		enhanceBaseBox.Init (CBKMonsterManager.currentEnhancementMonster);
		enhanceBaseBox.SetBar(false);
		enhanceDetails.text = CBKMonsterManager.currentEnhancementMonster.monster.displayName + "\nLvl: "
			+ CBKMonsterManager.currentEnhancementMonster.userMonster.currentLvl;
		Debug.Log(CBKMonsterManager.currentEnhancementMonster.enhancement.userMonsterId);
		
		enhanceOverBar.fillAmount = CBKMonsterManager.currentEnhancementMonster.PercentageTowardsNextLevel();
		
		int expFromQueue = 0;
		foreach (var item in CBKMonsterManager.enhancementFeeders) 
		{
			expFromQueue += item.enhanceXP;
		}
		
		enhanceUnderBar.fillAmount = CBKMonsterManager.currentEnhancementMonster.PercentageOfLevelup(
			CBKMonsterManager.currentEnhancementMonster.userMonster.currentExp + expFromQueue);
		
		queueParent.SetActive(true);
		
		while(bottomMiniBoxes.Count < CBKMonsterManager.enhancementFeeders.Count)
		{
			AddHealBox();
		}
		
		int i;
		for (i = 0; i < CBKMonsterManager.enhancementFeeders.Count;i++)
		{
			bottomMiniBoxes[i].Init(CBKMonsterManager.enhancementFeeders[i]);
			bottomMiniBoxes[i].SetBar(i==0);
		}
		for (;i < bottomMiniBoxes.Count;i++)
		{
			bottomMiniBoxes[i].gameObject.SetActive(false);
		}
	}
	
	void AddReserveCardSlot()
	{
		CBKGoonCard card = Instantiate(goonCardPrefab) as CBKGoonCard;
		card.transform.parent = reserveCards[lastReserveCardIndex].transform.parent;
		card.transform.localScale = Vector3.one;
		card.transform.localPosition = reserveCards[lastReserveCardIndex].transform.localPosition + cardOffset;
		card.addRemoveTeamButton.dragBehind = reserveCards[lastReserveCardIndex].addRemoveTeamButton.dragBehind;
		card.healButton.dragBehind = reserveCards[lastReserveCardIndex].healButton.dragBehind;
		reserveCards.Add (card);
	}
	
	void AddHealBox()
	{
		CBKMiniHealingBox box = Instantiate(healBoxPrefab) as CBKMiniHealingBox;
		box.transform.parent = bottomMiniBoxes[lastBox].transform.parent;
		box.transform.localScale = Vector3.one;
		box.transform.localPosition = bottomMiniBoxes[lastBox].transform.localPosition + boxOffset;
		bottomMiniBoxes.Add (box);
	}
	
	void Update()
	{
		if (queueParent.activeSelf)
		{
			long timeLeft = CBKMonsterManager.healingMonsters[CBKMonsterManager.healingMonsters.Count-1].healTimeLeftMillis;
			totalTimeLabel.text = CBKUtil.TimeStringShort(timeLeft);
			speedUpButton.label.text = Mathf.Ceil((float)timeLeft / (CBKWhiteboard.constants.minutesPerGem * 60000)).ToString();
		}
		else if (queueParent.activeSelf && CBKMonsterManager.enhancementFeeders.Count > 0)
		{
			long timeLeft = CBKMonsterManager.enhancementFeeders[CBKMonsterManager.enhancementFeeders.Count-1].enhanceTimeLeft;
			totalTimeLabel.text = CBKUtil.TimeStringShort(timeLeft);
			speedUpButton.label.text = Mathf.Ceil((float)timeLeft / (CBKWhiteboard.constants.minutesPerGem * 60000)).ToString();
		}
	}
	
	void TrySpeedUpHeal()
	{
		int gemCost = Mathf.CeilToInt((CBKMonsterManager.healingMonsters[CBKMonsterManager.healingMonsters.Count-1].timeToHealMillis) * 1f/60000);
		if (CBKResourceManager.instance.Spend(ResourceType.OIL, gemCost))
		{
			CBKMonsterManager.instance.SpeedUpHeal(gemCost);
		}
		else
		{
			CBKEventManager.Popup.CreatePopup("Not enough Gems");
		}
	}
	
	void TrySpeedUpEnhance()
	{
		int gemCost = Mathf.CeilToInt((CBKMonsterManager.enhancementFeeders[CBKMonsterManager.enhancementFeeders.Count-1].finishCombineTime) * 1f/60000);
		if (CBKResourceManager.instance.Spend(ResourceType.OIL, gemCost))
		{
			CBKMonsterManager.instance.SpeedUpEnhance(gemCost);
		}
		else
		{
			CBKEventManager.Popup.CreatePopup("Not enough Gems");
		}
	}
	
	void OnHealQueueChanged()
	{
		OrganizeHealingQueue();
		OrganizeCards();
	}
	
	void OnEnhanceQueueChanged()
	{
		OrganizeEnhanceQueue();
		OrganizeCards();
	}
	
	void OnRemoveTeamMember(PZMonster monster)
	{
		OrganizeCards();
	}
	
	void OnAddTeamMember(PZMonster monster)
	{
		teamCards[monster.userMonster.teamSlotNum-1].InitHeal(monster);
		OrganizeReserveCards();
	}
}
