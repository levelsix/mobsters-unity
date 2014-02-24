using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class CBKGoonScreen : MonoBehaviour {

	public static CBKGoonScreen instance;

	#region UI Elements

	[SerializeField]
	UIDragScrollView dragPanel;

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
	CBKActionButton speedUpButton;
	
	[SerializeField]
	UILabel totalTimeLabel;
	
	[SerializeField]
	CBKGoonCard enhanceBaseBox; 

	[SerializeField]
	TweenPosition enhanceLeftSideElements;

	[SerializeField]
	CBKUIHelper bottomFadeInElements;

	[SerializeField]
	UIPanel scrollPanel;

	[SerializeField]
	UITable table;

	[SerializeField]
	UILabel bottomBarLabel;

	[SerializeField]
	Transform goonPanelParent;

	[SerializeField]
	CBKGoonInfoPopup infoPopup;
	
	#endregion
	
	bool healMode = true;

	bool goonIn = false;

	[SerializeField]
	bool bringGoonIn = false;
	
	#region Properties
	
	int playerSlots
	{
		get
		{
			return CBKWhiteboard.constants.userMonsterConstants.maxNumTeamSlots;
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

	const string bottomHealDialogue = "Tap an injured mobster to begin healing";
	const string bottomEnhanceDialogue = "Select a mobster to enhance";
	const string bottomSacrificeDialogue = "Select a mobster to sacrifice";

	const int rightShiftOnMobsterEnhance = 130;
	const float TWEEN_TIME = 0.6f;
	
	#endregion

	void Awake()
	{
		instance = this;
	}
	
	public void OrganizeCards()
	{
		OrganizeTeamCards();
		OrganizeReserveCards();
	}
	
	public void InitHeal()
	{	
		bringGoonIn = false;
		healMode = true;
		OrganizeCards();	
		OrganizeHealingQueue (CBKMonsterManager.instance.healingMonsters);
		speedUpButton.onClick = TrySpeedUpHeal;
	}
	
	public void InitEnhance()
	{
		healMode = false;
		OrganizeCards();
		OrganizeEnhanceQueue();
		speedUpButton.onClick = TrySpeedUpEnhance;

		if (CBKMonsterManager.currentEnhancementMonster != null)
		{
			enhanceBaseBox.InitLab(CBKMonsterManager.currentEnhancementMonster);
			bringGoonIn = true;
			FinishGoonInNow();
		}
	}
	
	void OnEnable()
	{
		CBKEventManager.Goon.OnMonsterAddTeam += OnAddTeamMember;
		CBKEventManager.Goon.OnMonsterRemoveTeam += OnRemoveTeamMember;
		CBKEventManager.Goon.OnHealQueueChanged += OnHealQueueChanged;
		CBKEventManager.Goon.OnEnhanceQueueChanged += OnEnhanceQueueChanged;
		CBKEventManager.Goon.OnMonsterListChanged += OrganizeReserveCards;
	}
	
	void OnDisable()
	{
		CBKEventManager.Goon.OnMonsterAddTeam -= OnAddTeamMember;
		CBKEventManager.Goon.OnMonsterRemoveTeam -= OnRemoveTeamMember;
		CBKEventManager.Goon.OnHealQueueChanged -= OnHealQueueChanged;
		CBKEventManager.Goon.OnEnhanceQueueChanged -= OnEnhanceQueueChanged;
		CBKEventManager.Goon.OnMonsterListChanged -= OrganizeReserveCards;
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
		OrganizeReserveCards(CBKMonsterManager.userTeam, CBKMonsterManager.instance.userMonsters);
	}
	
	void OrganizeReserveCards (PZMonster[] teamGoons, List<PZMonster> playerGoons)
	{
		int i = 0;
		foreach (var item in playerGoons) 
		{
			if (!healMode && (!item.userMonster.isComplete || item.isEnhancing || item.isHealing))
			{
				continue;
			}

			if (lastReserveCardIndex < i)
			{
				AddReserveCardSlot();
			}
			if (healMode)
			{
				reserveCards[i].InitHeal(item);
			}
			else
			{
				reserveCards[i].InitLab(item);
			}
			i++;
		}
		
		while (++i < reserveCards.Count)
		{
			reserveCards[i].gameObject.SetActive(false);
		}

		dragPanel.collider.enabled = false;
		dragPanel.collider.enabled = true;

		table.Reposition();
	}
	
	void OrganizeHealingQueue()
	{
		OrganizeHealingQueue(CBKMonsterManager.instance.healingMonsters);
	}
	
	void OrganizeHealingQueue(List<PZMonster> healingMonsters)
	{
		
		while(bottomMiniBoxes.Count < healingMonsters.Count)
		{
			AddHealBox();
		}
		int i;
		for (i = 0; i < healingMonsters.Count; i++) 
		{
			bottomMiniBoxes[i].Init (healingMonsters[i]);
			bottomMiniBoxes[i].SetBar(healingMonsters[i].healingMonster.queuedTimeMillis <= CBKUtil.timeNowMillis);
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
			bottomBarLabel.text = bottomEnhanceDialogue;
			bringGoonIn = false;
			return;
		}

		bringGoonIn = true;
		
		enhanceBaseBox.InitLab(CBKMonsterManager.currentEnhancementMonster);
		
		int expFromQueue = 0;
		foreach (var item in CBKMonsterManager.enhancementFeeders) 
		{
			expFromQueue += item.enhanceXP;
		}

		bottomBarLabel.text = bottomSacrificeDialogue;
		
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
		//card.transform.localPosition = reserveCards[lastReserveCardIndex].transform.localPosition + cardOffset;
		card.addRemoveTeamButton.dragBehind = reserveCards[lastReserveCardIndex].addRemoveTeamButton.dragBehind;
		card.healButton.dragBehind = reserveCards[lastReserveCardIndex].healButton.dragBehind;
		card.infoPopup = infoPopup;
		reserveCards.Add (card);
	}
	
	void AddHealBox()
	{
		CBKMiniHealingBox box = Instantiate(healBoxPrefab) as CBKMiniHealingBox;
		box.transform.parent = bottomMiniBoxes[lastBox].transform.parent;
		box.transform.localScale = Vector3.one;
		box.transform.localPosition = bottomMiniBoxes[lastBox].transform.localPosition + boxOffset;
		box.on = false;
		bottomMiniBoxes.Add (box);
	}
	
	void Update()
	{
		if (healMode && CBKMonsterManager.instance.healingMonsters.Count > 0)
		{
			long totalHealTimeLeft = 0;
			foreach (var item in CBKMonsterManager.instance.healingMonsters) {
				if (item.finishHealTimeMillis > totalHealTimeLeft)
				{
					totalHealTimeLeft = item.finishHealTimeMillis;
				}
			}
			totalHealTimeLeft -= CBKUtil.timeNowMillis;
			totalTimeLabel.text = CBKUtil.TimeStringShort(totalHealTimeLeft);
			speedUpButton.label.text = Mathf.Ceil((float)totalHealTimeLeft / (CBKWhiteboard.constants.minutesPerGem * 60000)).ToString();
		}
		else if (CBKMonsterManager.enhancementFeeders.Count > 0)
		{
			long timeLeft = CBKMonsterManager.enhancementFeeders[CBKMonsterManager.enhancementFeeders.Count-1].enhanceTimeLeft;
			totalTimeLabel.text = CBKUtil.TimeStringShort(timeLeft);
			speedUpButton.label.text = Mathf.Ceil((float)timeLeft / (CBKWhiteboard.constants.minutesPerGem * 60000)).ToString();
		}

		if (!goonIn && bringGoonIn)
		{
			goonIn = true;
			StartCoroutine(BringInEnhanceGoon());
		}
		if (goonIn && !bringGoonIn)
		{
			goonIn = false;
			StartCoroutine(TakeOutEnhanceGoon());
		}
	}

	IEnumerator BringInEnhanceGoon()
	{
		enhanceLeftSideElements.PlayForward();
		enhanceLeftSideElements.ResetToBeginning();

		while (enhanceLeftSideElements.tweenFactor < 1)
		{
			dragPanel.scrollView.RestrictWithinBounds(false);
			yield return null;
		}
		dragPanel.scrollView.RestrictWithinBounds(false);

	}

	void FinishGoonInNow()
	{
		enhanceLeftSideElements.transform.localPosition = enhanceLeftSideElements.to;

		table.Reposition();
	}

	IEnumerator TakeOutEnhanceGoon()
	{
		enhanceLeftSideElements.PlayReverse();
		while (enhanceLeftSideElements.tweenFactor > 0)
		{
			dragPanel.scrollView.RestrictWithinBounds(false);
			yield return null;
		}
		dragPanel.scrollView.RestrictWithinBounds(false);
	}
	
	void TrySpeedUpHeal()
	{
		int gemCost = Mathf.CeilToInt((CBKMonsterManager.instance.healingMonsters[CBKMonsterManager.instance.healingMonsters.Count-1].timeToHealMillis) * 1f/60000);
		if (CBKResourceManager.instance.Spend(ResourceType.GEMS, gemCost, TrySpeedUpHeal))
		{
			CBKMonsterManager.instance.SpeedUpHeal(gemCost);
		}
	}
	
	void TrySpeedUpEnhance()
	{
		int gemCost = Mathf.CeilToInt((CBKMonsterManager.enhancementFeeders[CBKMonsterManager.enhancementFeeders.Count-1].finishCombineTime) * 1f/60000);
		if (CBKResourceManager.instance.Spend(ResourceType.GEMS, gemCost, TrySpeedUpEnhance))
		{
			CBKMonsterManager.instance.SpeedUpEnhance(gemCost);
		}
	}
	
	void OnHealQueueChanged()
	{
		if (CBKMonsterManager.instance.healingMonsters.Count > 0)
		{
			bottomFadeInElements.FadeIn();
			bottomBarLabel.GetComponent<CBKUIHelper>().FadeOut();
		}
		else
		{
			bottomFadeInElements.FadeOut();
			bottomBarLabel.GetComponent<CBKUIHelper>().FadeIn();
		}
		OrganizeHealingQueue();
		OrganizeCards();
	}
	
	void OnEnhanceQueueChanged()
	{
		if (CBKMonsterManager.enhancementFeeders.Count > 0)
		{
			bottomFadeInElements.FadeIn();
			bottomBarLabel.GetComponent<CBKUIHelper>().FadeOut();
		}
		else
		{
			bottomFadeInElements.FadeOut();
			bottomBarLabel.GetComponent<CBKUIHelper>().FadeIn();
		}
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
