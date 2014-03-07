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
	Transform goonCardParent;

	[SerializeField]
	CBKGoonInfoPopup infoPopup;

	[SerializeField]
	GameObject labButtons;

	[SerializeField]
	CBKUIHelper goonPanelElements;

	[SerializeField]
	CBKUIHelper evolveElements;

	[SerializeField]
	CBKActionButton backButton;

	[SerializeField]
	CBKUIHelper scientistIcons;

	[SerializeField]
	UILabel errorLabel;

	[SerializeField]
	MSEvolutionElements evolutionElements;
	
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
			return MSWhiteboard.constants.userMonsterConstants.maxNumTeamSlots;
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
	
	public void InitHeal(bool fromMenu = false)
	{	
		goonPanelElements.ResetAlpha(true);
		evolveElements.ResetAlpha(false);
		scientistIcons.ResetAlpha(false);
		backButton.Disable();

		bringGoonIn = false;
		healMode = true;
		OrganizeCards();	
		OrganizeHealingQueue (MSHospitalManager.instance.healingMonsters);
		speedUpButton.onClick = TrySpeedUpHeal;
		labButtons.SetActive(false);
	}

	/// <summary>
	/// Hacky little way for the logic to be sliiiightly different if we're coming
	/// from evolve mode rather than coming straight from another menu
	/// </summary>
	public void InitEnhanceFromButton()
	{
		InitEnhance(true);
	}
	
	public void InitEnhance(bool fromEvolve = false)
	{
		backButton.Disable();
		goonPanelElements.ResetAlpha(true);
		evolveElements.ResetAlpha(false);
		if (fromEvolve)
		{
			scientistIcons.FadeOut();
		}
		else
		{
			scientistIcons.ResetAlpha(false);
		}
		healMode = false;
		OrganizeCards();
		OrganizeEnhanceQueue();
		speedUpButton.onClick = TrySpeedUpEnhance;
		labButtons.SetActive(true);

		if (MSMonsterManager.currentEnhancementMonster != null)
		{
			enhanceBaseBox.InitLab(MSMonsterManager.currentEnhancementMonster);
			bringGoonIn = true;
			FinishGoonInNow();
		}
	}

	public void InitEvolve()
	{
		evolveElements.ResetAlpha(false);
		goonPanelElements.ResetAlpha(true);
		
		OrganizeEvolveCards();

		if (MSEvolutionManager.instance.currEvolution != null && MSEvolutionManager.instance.currEvolution.userMonsterIds.Count > 0)
		{
			CBKGoonCard card = reserveCards.Find(x=>x.goon.userMonster.userMonsterId == MSEvolutionManager.instance.currEvolution.userMonsterIds[0]);
			OrganizeEvolution(card);
		}
		else
		{
			scientistIcons.FadeIn();
			bottomBarLabel.GetComponent<CBKUIHelper>().FadeOut();
			backButton.Disable();
		}


		bringGoonIn = false;

		//OrganizeScientists
	}

	public void CancelEvolve()
	{
		MSEvolutionManager.instance.currEvolution = null;
		evolutionElements.evolvingCard.transform.parent = goonCardParent;
		evolutionElements.evolvingCard.gameObject.SetActive(false);
		evolutionElements.evolvingCard.gameObject.SetActive(true);
		if (evolutionElements.evolvingCard.buddy != null)
		{
			evolutionElements.evolvingCard.buddy.transform.parent = goonCardParent;
			evolutionElements.evolvingCard.buddy.buddy = null;
			evolutionElements.evolvingCard.buddy = null;
		}
		InitEvolve();
	}

	public void OnStartEvolve()
	{
		backButton.Disable();
	}
	
	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterAddTeam += OnAddTeamMember;
		MSActionManager.Goon.OnMonsterRemoveTeam += OnRemoveTeamMember;
		MSActionManager.Goon.OnHealQueueChanged += OnHealQueueChanged;
		MSActionManager.Goon.OnEnhanceQueueChanged += OnEnhanceQueueChanged;
		MSActionManager.Goon.OnMonsterListChanged += OrganizeReserveCards;
	}
	
	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterAddTeam -= OnAddTeamMember;
		MSActionManager.Goon.OnMonsterRemoveTeam -= OnRemoveTeamMember;
		MSActionManager.Goon.OnHealQueueChanged -= OnHealQueueChanged;
		MSActionManager.Goon.OnEnhanceQueueChanged -= OnEnhanceQueueChanged;
		MSActionManager.Goon.OnMonsterListChanged -= OrganizeReserveCards;

		if (!MSEvolutionManager.instance.active)
		{
			MSEvolutionManager.instance.currEvolution = null;
		}
	}

	void OrganizeTeamCards ()
	{
		int i;
		for (i = 0; i < MSMonsterManager.userTeam.Length; i++) 
		{
			if (healMode)
			{
				teamCards[i].InitHeal(MSMonsterManager.userTeam[i]);
			}
			else
			{
				teamCards[i].InitLab(MSMonsterManager.userTeam[i]);
			}
		}
	}
	
	void OrganizeReserveCards()
	{
		OrganizeReserveCards(MSMonsterManager.userTeam, MSMonsterManager.instance.userMonsters);
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

			if (reserveCards[i].transform.parent != goonCardParent)
			{
				reserveCards[i].transform.parent = goonCardParent;
				reserveCards[i].gameObject.SetActive(false);
				reserveCards[i].gameObject.SetActive(true);
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

	/// <summary>
	/// If we've got any sort of UserMonsterEvolutionProto going, even if
	/// it's missing parts (buddies or scientists), we call this
	/// </summary>
	/// <param name="currEvolution">Curr evolution.</param>
	public void OrganizeEvolution(CBKGoonCard card)
	{
		evolutionElements.Init(card);

		evolveElements.FadeIn();
		goonPanelElements.FadeOut();
		scientistIcons.FadeOut();

		if (!MSEvolutionManager.instance.active)
		{
			backButton.Enable();
			backButton.label.text = "Back";
			backButton.onClick = CancelEvolve;
		}

		bottomBarLabel.GetComponent<CBKUIHelper>().FadeIn();

		string catalystColorString = "[ff0000]";
		if (MSEvolutionManager.instance.currEvolution.catalystUserMonsterId > 0)
		{
			catalystColorString = "[00ff00]";
		}

		string mobsterColorString = "[ff0000]";
		if (MSEvolutionManager.instance.currEvolution.userMonsterIds.Count > 1)
		{
			mobsterColorString = "[00ff00]";
		}

		MonsterProto catalyst = MSDataManager.instance.Get<MonsterProto>(card.goon.monster.evolutionCatalystMonsterId);
		MonsterProto evoMonster = MSDataManager.instance.Get<MonsterProto>(card.goon.monster.evolutionMonsterId);

		bottomBarLabel.text = "You need " + mobsterColorString + "2 Lvl " + card.goon.monster.maxLevel + " " + card.goon.monster.displayName + "s[-] and "
			+ catalystColorString + card.goon.monster.numCatalystMonstersRequired + " " + catalyst.displayName + "(Evo " + catalyst.evolutionLevel + ")[-]" 
				+ "\nto evolve to " + evoMonster.displayName;
	}

	/// <summary>
	/// Like organizing the reserve cards for other things, except the cards are all already on the
	/// table and we're going to try to stack them, then reorder, rename, and rearrange
	/// </summary>
	void OrganizeEvolveCards()
	{
		foreach (var item in reserveCards) 
		{
			if (item.buddy == null)
			{
				PZMonster buddy = MSMonsterManager.instance.FindEvolutionBuddy(item.goon);
				item.InitEvolve(GetCardForMonster(buddy));
				if (item.transform.parent != goonCardParent)
				{
					item.transform.parent = goonCardParent;
					item.gameObject.SetActive(false);
					item.gameObject.SetActive(true);
				}
			}
		}
		StartCoroutine(RepositionAfterMove(.25f));
	}

	IEnumerator RepositionAfterMove(float moveTime)
	{
		float currTime = 0;
		while (currTime < moveTime)
		{
			currTime += Time.deltaTime;
			table.Reposition();
			yield return null;
		}
		table.Reposition();
	}
	
	void OrganizeHealingQueue()
	{
		OrganizeHealingQueue(MSHospitalManager.instance.healingMonsters);
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
			bottomMiniBoxes[i].SetBar(healingMonsters[i].healingMonster.queuedTimeMillis <= MSUtil.timeNowMillis);
		}
		for (; i < bottomMiniBoxes.Count; i++) 
		{
			bottomMiniBoxes[i].gameObject.SetActive(false);
		}
	}
	
	void OrganizeEnhanceQueue()
	{
		if(MSMonsterManager.currentEnhancementMonster == null)
		{
			bottomBarLabel.text = bottomEnhanceDialogue;
			bringGoonIn = false;
			return;
		}

		bringGoonIn = true;
		
		enhanceBaseBox.InitLab(MSMonsterManager.currentEnhancementMonster);
		
		int expFromQueue = 0;
		foreach (var item in MSMonsterManager.enhancementFeeders) 
		{
			expFromQueue += item.enhanceXP;
		}

		bottomBarLabel.text = bottomSacrificeDialogue;
		
		while(bottomMiniBoxes.Count < MSMonsterManager.enhancementFeeders.Count)
		{
			AddHealBox();
		}
		
		int i;
		for (i = 0; i < MSMonsterManager.enhancementFeeders.Count;i++)
		{
			bottomMiniBoxes[i].Init(MSMonsterManager.enhancementFeeders[i]);
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
		if (healMode && MSHospitalManager.instance.healingMonsters.Count > 0)
		{
			long totalHealTimeLeft = 0;
			foreach (var item in MSHospitalManager.instance.healingMonsters) {
				if (item.finishHealTimeMillis > totalHealTimeLeft)
				{
					totalHealTimeLeft = item.finishHealTimeMillis;
				}
			}
			totalHealTimeLeft -= MSUtil.timeNowMillis;
			totalTimeLabel.text = MSUtil.TimeStringShort(totalHealTimeLeft);
			speedUpButton.label.text = Mathf.Ceil((float)totalHealTimeLeft / (MSWhiteboard.constants.minutesPerGem * 60000)).ToString();
		}
		else if (MSMonsterManager.enhancementFeeders.Count > 0)
		{
			long timeLeft = MSMonsterManager.enhancementFeeders[MSMonsterManager.enhancementFeeders.Count-1].enhanceTimeLeft;
			totalTimeLabel.text = MSUtil.TimeStringShort(timeLeft);
			speedUpButton.label.text = Mathf.Ceil((float)timeLeft / (MSWhiteboard.constants.minutesPerGem * 60000)).ToString();
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

	CBKGoonCard GetCardForMonster(PZMonster monster)
	{
		return reserveCards.Find(x=>x.goon == monster);
	}
	
	void TrySpeedUpHeal()
	{
		int gemCost = Mathf.CeilToInt((MSHospitalManager.instance.healingMonsters[MSHospitalManager.instance.healingMonsters.Count-1].timeToHealMillis) * 1f/60000);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost, TrySpeedUpHeal))
		{
			MSHospitalManager.instance.SpeedUpHeal(gemCost);
		}
	}
	
	void TrySpeedUpEnhance()
	{
		int gemCost = Mathf.CeilToInt((MSMonsterManager.enhancementFeeders[MSMonsterManager.enhancementFeeders.Count-1].combineTimeLeft) * 1f/60000 / MSWhiteboard.constants.minutesPerGem);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost, TrySpeedUpEnhance))
		{
			MSMonsterManager.instance.SpeedUpEnhance(gemCost);
		}
	}

	public void DisplayErrorMessage(string error)
	{
		errorLabel.text = error;
		errorLabel.alpha = 1;
		errorLabel.GetComponent<CBKUIHelper>().FadeOut();
	}
	
	void OnHealQueueChanged()
	{
		if (MSHospitalManager.instance.healingMonsters.Count > 0)
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
		if (MSMonsterManager.enhancementFeeders.Count > 0)
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
