using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public enum GoonScreenMode {TEAM, SELL, HEAL, PICK_ENHANCE, DO_ENHANCE, PICK_EVOLVE, DO_EVOLVE, MINIJOB };

public class MSGoonScreen : MonoBehaviour 
{
	/// <summary>
	/// The screens.
	/// Indexed according to GoonScreenMode
	/// </summary>
	[SerializeField]
	MSFunctionalScreen[] screens;

	[SerializeField]
	MSEventScreen eventScreen;

	[SerializeField]
	Transform topIconParent;

	[SerializeField]
	MSFunctionalTopIcon topIconPrefab;

	MSFunctionalTopIcon topIcon;

	[SerializeField]
	Transform topMenuParent;

	[SerializeField]
	MSBarEvent barEvent;

	[SerializeField]
	UIWidget backButton;

	[SerializeField]
	UILabel topEventTitle;

	int currScreen;

	/// <summary>
	/// Setter for directly setting the title of the
	/// current screen.
	/// Useful for team view when we want to show X/3 slots
	/// filled.
	/// </summary>
	/// <value>The title.</value>
	public string title
	{
		set
		{
			topIcon.label.text = value;
		}
	}

	const int SIZE = 860;

	public void Init(GoonScreenMode mode)
	{
		currScreen = (int)mode;

		if (currScreen >= screens.Length)
		{
			Debug.LogError("Da fuck you doin?");
			return;
		}

		foreach (var item in screens) 
		{
			if (item != screens[(int)mode])
			{
				(item as MonoBehaviour).gameObject.SetActive(false);
			}
		}

		if (topIcon == null)
		{
			topIcon = MSPoolManager.instance.Get(topIconPrefab, topIconParent).GetComponent<MSFunctionalTopIcon>();
			topIcon.transform.localScale = Vector3.one;
		}
		topIcon.transform.localPosition = Vector3.zero;
		topIcon.helper.ResetAlpha(true);
		topIcon.Init(mode);

		screens[(int)mode].gameObject.SetActive(true);
		screens[(int)mode].transform.localPosition = Vector3.zero;
		screens[(int)mode].Init();

		barEvent.GetComponent<TweenAlpha>().Sample(0f, false);
		barEvent.GetComponent<UIWidget>().alpha = 0f;
		barEvent.gameObject.SetActive(false);

		eventScreen.gameObject.SetActive(false);

		topMenuParent.GetComponent<TweenAlpha>().Sample(0f, false);
		topMenuParent.GetComponent<TweenPosition>().Sample(0f, false);
		topMenuParent.GetComponent<UIWidget>().alpha = 1f;
		topMenuParent.transform.localPosition = Vector3.zero;

		backButton.GetComponent<TweenAlpha>().Sample(0f, false);
		backButton.alpha = 0f;
		backButton.gameObject.SetActive(false);

		topEventTitle.GetComponent<TweenAlpha>().Sample(0f, false);
		topEventTitle.alpha = 0f;

		CheckEventBarAction(currScreen, true);
	}

	public void ShiftRight()
	{
		DoShiftRight(true);
	}

	public void DoShiftRight(bool doIcon)
	{
		int nextScreen = currScreen;
		do
		{
			if (++nextScreen >= screens.Length)
			{
				nextScreen -= screens.Length;
			}
		}while(!screens[nextScreen].IsAvailable());
		
		MSFunctionalScreen curr = screens[currScreen];
		MSFunctionalScreen next = screens[nextScreen];

		next.gameObject.SetActive(true);
		next.transform.localPosition = new Vector3(SIZE, 0, 0);
		TweenPosition tp = TweenPosition.Begin(next.gameObject, .3f, Vector3.zero);
		tp.onFinished.Clear();
		tp = TweenPosition.Begin(curr.gameObject, .3f, new Vector3(-SIZE, 0, 0));
		tp.AddOnFinished(delegate{curr.gameObject.SetActive(false);});

		if (doIcon)
		{
			tp = TweenPosition.Begin(topIcon.gameObject, .3f, new Vector3(-100, 0, 0));
			topIcon.helper.FadeOutAndPool();

			topIcon = MSPoolManager.instance.Get(topIconPrefab, topIcon.transform.parent).GetComponent<MSFunctionalTopIcon>();
			topIcon.Init((GoonScreenMode)nextScreen);
			topIcon.transform.localScale = Vector3.one;
			topIcon.transform.localPosition = new Vector3(100, 0, 0);
			topIcon.helper.ResetAlpha(false);
			topIcon.helper.FadeIn();
			TweenPosition.Begin(topIcon.gameObject, .3f, Vector3.zero);
		}

		CheckEventBarAction(nextScreen);
		next.Init();

		currScreen = nextScreen;
	}

	public void ShiftLeft()
	{
		DoShiftLeft(true);
	}

	public void DoShiftLeft(bool doIcon)
	{
		int nextScreen = currScreen;
		do
		{
			if (--nextScreen < 0)
			{
				nextScreen += screens.Length;
			}
		}while(!screens[nextScreen].IsAvailable());

		MSFunctionalScreen curr = screens[currScreen];
		MSFunctionalScreen next = screens[nextScreen];
		
		next.gameObject.SetActive(true);
		next.transform.localPosition = new Vector3(-SIZE, 0, 0);
		TweenPosition tp = TweenPosition.Begin(next.gameObject, .3f, Vector3.zero);
		tp.onFinished.Clear();
		tp = TweenPosition.Begin(curr.gameObject, .3f, new Vector3(SIZE, 0, 0));
		tp.AddOnFinished(delegate{Debug.Log("Curr: " + curr.name);curr.gameObject.SetActive(false);});

		if (doIcon)
		{
			tp = TweenPosition.Begin(topIcon.gameObject, .3f, new Vector3(100, 0, 0));
			topIcon.helper.FadeOutAndPool();

			topIcon = MSPoolManager.instance.Get(topIconPrefab, topIcon.transform.parent).GetComponent<MSFunctionalTopIcon>();
			topIcon.Init((GoonScreenMode)nextScreen);
			topIcon.transform.localScale = Vector3.one;
			topIcon.transform.localPosition = new Vector3(-100, 0, 0);
			topIcon.helper.ResetAlpha(false);
			topIcon.helper.FadeIn();
			TweenPosition.Begin(topIcon.gameObject, .3f, Vector3.zero);
		}

		CheckEventBarAction(nextScreen);
		next.Init();

		currScreen = nextScreen;
	}

	void CheckEventBarAction(int nextScreen, bool forceTransition = false)
	{
		PersistentEventProto perEvent = null;

		if(nextScreen == (int)GoonScreenMode.PICK_ENHANCE)
		{
			foreach(PersistentEventProto pEvent in MSEventManager.instance.GetActiveEvents())
			{
				if(pEvent.type == PersistentEventProto.EventType.ENHANCE)
				{
					perEvent = pEvent;
					barEvent.Init(pEvent);
					eventScreen.Init(barEvent.darkColor, barEvent.color, barEvent.lightColor, pEvent);
					break;
				}
			}
			if(perEvent == null)
			{
//				barEvent.GetComponent<TweenAlpha>().PlayReverse();
				barEvent.GetComponent<MSUIHelper>().FadeOutAndOff();
			}
		}
		else if(nextScreen == (int)GoonScreenMode.PICK_EVOLVE)
		{
			foreach(PersistentEventProto pEvent in MSEventManager.instance.GetActiveEvents())
			{
				if(pEvent.type == PersistentEventProto.EventType.EVOLUTION)
				{
					perEvent = pEvent;
					barEvent.Init(pEvent);
					eventScreen.Init(barEvent.darkColor, barEvent.color, barEvent.lightColor, pEvent);
					break;
				}
			}
			if(perEvent == null)
			{
				barEvent.GetComponent<MSUIHelper>().FadeOutAndOff();
//				barEvent.GetComponent<TweenAlpha>().PlayReverse();
			}
		}
		else
		{
//			barEvent.GetComponent<TweenAlpha>().PlayReverse();
			barEvent.GetComponent<MSUIHelper>().FadeOutAndOff();
		}

		if(perEvent != null && ((currScreen != (int)GoonScreenMode.PICK_ENHANCE && currScreen != (int)GoonScreenMode.PICK_EVOLVE) || forceTransition))
		{
			barEvent.gameObject.SetActive(true);
			TweenAlpha.Begin(barEvent.gameObject, 0.3f, 1f);
		}
	}

	/// <summary>
	/// Shifts for the enhance and evolve events
	/// This hides the 
	/// </summary>
	public void EnterEventScreen()
	{
		//shift left

		//The event screen is init through the barEvent object which stores the information for the event
		MSFunctionalScreen curr = screens[currScreen];
		MSFunctionalScreen next = eventScreen;

		next.gameObject.SetActive(true);
		next.transform.localPosition = new Vector3(SIZE, 0, 0);
		TweenPosition tp = TweenPosition.Begin(next.gameObject, .3f, Vector3.zero);
		tp.onFinished.Clear();
		tp = TweenPosition.Begin(curr.gameObject, .3f, new Vector3(-SIZE, 0, 0));
		tp.AddOnFinished(delegate{curr.gameObject.SetActive(false);});

		topMenuParent.GetComponent<TweenAlpha>().PlayForward();
		topMenuParent.GetComponent<TweenPosition>().PlayForward();

//		barEvent.GetComponent<TweenAlpha>().PlayReverse();
		barEvent.GetComponent<MSUIHelper>().FadeOutAndOff();

		backButton.gameObject.SetActive(true);
		TweenAlpha.Begin(backButton.gameObject, 0.3f, 1f);

		topEventTitle.GetComponent<TweenAlpha>().PlayForward();
	}

	public void ExitEventScreen()
	{
		//shift right
		MSFunctionalScreen curr = eventScreen;
		MSFunctionalScreen next = screens[currScreen];

		next.gameObject.SetActive(true);
		TweenPosition tp = TweenPosition.Begin(next.gameObject, .3f, Vector3.zero);
		tp.onFinished.Clear();
		tp = TweenPosition.Begin(curr.gameObject, .3f, new Vector3(SIZE, 0, 0));
		tp.AddOnFinished(delegate{curr.gameObject.SetActive(false);});

		topMenuParent.GetComponent<TweenAlpha>().PlayReverse();
		topMenuParent.GetComponent<TweenPosition>().PlayReverse();

		barEvent.gameObject.SetActive(true);
		TweenAlpha.Begin(barEvent.gameObject, 0.3f, 1f);

//		backButton.GetComponent<TweenAlpha>().PlayReverse();
		backButton.GetComponent<MSUIHelper>().FadeOutAndOff();

		topEventTitle.GetComponent<TweenAlpha>().PlayReverse();
	}

	/*
	public GoonScreenMode currMode = GoonScreenMode.EVOLVE;

	#region UI Elements

	[SerializeField]
	UIDragScrollView dragPanel;

	[SerializeField]
	MSGoonTeamCard[] teamCards;
	
	[SerializeField]
	List<MSGoonCard> cards;
	
	[SerializeField]
	List<MSMiniGoonBox> bottomMiniBoxes;
	
	[SerializeField]
	MSGoonCard goonCardPrefab;
	
	[SerializeField]
	UILabel totalTimeLabel;
	
	[SerializeField]
	MSGoonCard enhanceBaseBox; 

	[SerializeField]
	TweenPosition enhanceLeftSideElements;

	[SerializeField]
	MSUIHelper bottomFadeInElements;

	[SerializeField]
	UIPanel scrollPanel;

	[SerializeField]
	public UITable goonTable;

	[SerializeField]
	UILabel bottomBarLabel;

	[SerializeField]
	Transform goonCardParent;

	[SerializeField]
	MSGoonInfoPopup infoPopup;

	[SerializeField]
	MSUIHelper goonPanelElements;

	[SerializeField]
	MSUIHelper evolveElements;

	[SerializeField]
	MSUIHelper scientistIcons;

	[SerializeField]
	UILabel errorLabel;

	[SerializeField]
	MSEvolutionElements evolutionElements;

	[SerializeField]
	UILabel titleLabel;

	[SerializeField]
	MSUIHelper injuredTitle;

	[SerializeField]
	MSUIHelper healthyTitle;

	[SerializeField]
	MSUIHelper unavailableTitle;

	[SerializeField]
	GameObject injuredHeader;

	[SerializeField]
	GameObject healthyHeader;

	[SerializeField]
	GameObject unavailableHeader;

	int currInjured = 0;
	int currHealthy = 0;
	int currUnavail = 0;

	public MSBottomBar bottomBar;
	
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
			return cards.Count - 1;
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

	const string bottomHealDialogue = "Tap an injured toon to begin healing";
	const string bottomEnhanceDialogue = "Select a toon to enhance";
	const string bottomSacrificeDialogue = "Select a toon to sacrifice";

	const int rightShiftOnMobsterEnhance = 130;
	const float TWEEN_TIME = 0.6f;
	
	#endregion
	
	public void OrganizeCards()
	{
		OrganizeTeamCards();
		OrganizeReserveCards();
		titleLabel.text = "My Goonies(" + MSMonsterManager.instance.userMonsters.Count +
			"/" + MSMonsterManager.instance.totalResidenceSlots + ")";
	}

	public void Init(GoonScreenMode mode)
	{
		currMode = mode;
		Init ();
	}

	public void Init()
	{
		switch (currMode) {
			case GoonScreenMode.HEAL:
				InitHeal();
				break;
			case GoonScreenMode.DO_ENHANCE:
				InitEnhance();
				break;
			case GoonScreenMode.EVOLVE:
				InitEvolve();
				break;
			case GoonScreenMode.SELL:
				InitSell();
				break;
			default:
				break;
		}
	}

	public void InitHeal()
	{	
		goonPanelElements.ResetAlpha(true);
		evolveElements.ResetAlpha(false);
		scientistIcons.ResetAlpha(false);
		//backButton.Disable();

		bringGoonIn = false;
		healMode = true;
		OrganizeCards();	
		//OrganizeHealingQueue (MSHospitalManager.instance.healingMonsters);
		bottomBar.Init(GoonScreenMode.HEAL);
	}

	/// <summary>
	/// Hacky little way for the logic to be sliiiightly different if we're coming
	/// from evolve mode rather than coming straight from another menu
	/// </summary>
	public void InitEnhanceFromButton()
	{
		currMode = GoonScreenMode.DO_ENHANCE;
		InitEnhance(true);
	}
	
	public void InitEnhance(bool fromEvolve = false)
	{
		//backButton.Disable();
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
		bottomBar.Init(currMode);

		if (MSMonsterManager.instance.currentEnhancementMonster != null 
		    && MSMonsterManager.instance.currentEnhancementMonster.monster != null
		    && MSMonsterManager.instance.currentEnhancementMonster.monster.monsterId > 0)
		{
			enhanceBaseBox.InitLab(MSMonsterManager.instance.currentEnhancementMonster);
			bringGoonIn = true;
			FinishGoonInNow();
		}
	}

	public void InitEvolve()
	{
		evolveElements.ResetAlpha(false);
		goonPanelElements.ResetAlpha(true);

		bottomBar.Init(currMode);
		
		OrganizeEvolveCards();

		if (MSEvolutionManager.instance.currEvolution != null && MSEvolutionManager.instance.currEvolution.userMonsterIds.Count > 0)
		{
			MSGoonCard card = cards.Find(x=>x.monster.userMonster.userMonsterId == MSEvolutionManager.instance.currEvolution.userMonsterIds[0]);
			OrganizeEvolution(card);
		}
		else
		{
			scientistIcons.FadeIn();
			bottomBarLabel.GetComponent<MSUIHelper>().FadeOut();
			//backButton.Disable();
		}


		bringGoonIn = false;

		//OrganizeScientists
	}

	void InitSell()
	{
		bottomBar.Init(currMode);

		OrganizeCards();

		goonTable.Reposition();
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
		//backButton.Disable();
	}
	
	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterAddTeam += OnAddTeamMember;
		MSActionManager.Goon.OnMonsterRemoveTeam += OnRemoveTeamMember;
		MSActionManager.Goon.OnHealQueueChanged += OnQueueChanged;
		MSActionManager.Goon.OnEnhanceQueueChanged += OnEnhanceQueueChanged;
		MSActionManager.Goon.OnMonsterListChanged += OrganizeReserveCards;
		MSActionManager.Goon.OnMonsterRemoveQueue += OnQueueChanged;
	}
	
	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterAddTeam -= OnAddTeamMember;
		MSActionManager.Goon.OnMonsterRemoveTeam -= OnRemoveTeamMember;
		MSActionManager.Goon.OnHealQueueChanged -= OnQueueChanged;
		MSActionManager.Goon.OnEnhanceQueueChanged -= OnEnhanceQueueChanged;
		MSActionManager.Goon.OnMonsterListChanged -= OrganizeReserveCards;
		MSActionManager.Goon.OnMonsterRemoveQueue -= OnQueueChanged;

		if (!MSEvolutionManager.instance.hasEvolution)
		{
			MSEvolutionManager.instance.currEvolution = null;
		}
	}

	void OrganizeTeamCards ()
	{
		int i;
		for (i = 0; i < MSMonsterManager.instance.userTeam.Length; i++) 
		{
			if (currMode == GoonScreenMode.HEAL)
			{
				teamCards[i].Init(MSMonsterManager.instance.userTeam[i]);
			}
			else
			{
				teamCards[i].InitLab(MSMonsterManager.instance.userTeam[i]);
			}
		}
	}
	
	void OrganizeReserveCards()
	{
		OrganizeReserveCards(MSMonsterManager.instance.userTeam, MSMonsterManager.instance.userMonsters);
	}
	
	void OrganizeReserveCards (PZMonster[] teamGoons, List<PZMonster> playerGoons)
	{
		//Reset all these values
		currInjured = currHealthy = currUnavail = 0;

		if (currMode == GoonScreenMode.SELL)
		{
			currHealthy = 1;
			foreach (var item in cards) 
			{

			}
		}
		else
		{
			int i = 0;
			foreach (var item in playerGoons) 
			{
				if (currMode != GoonScreenMode.HEAL 
				    && (!item.userMonster.isComplete || item.isEnhancing || item.isHealing))
				{
					continue;
				}

				if (lastReserveCardIndex < i)
				{
					AddReserveCardSlot();
				}
				if (currMode == GoonScreenMode.HEAL)
				{
					cards[i].InitHeal(item);
				}
				else
				{
					if (item == MSMonsterManager.instance.currentEnhancementMonster)
					{
						continue;
					}
					if (item.monsterStatus != MonsterStatus.HEALTHY 
					    && item.monsterStatus != MonsterStatus.INJURED)
					{
						continue;
					}
					cards[i].InitLab(item);
				}


				switch (cards[i].cardMode) {
					case MonsterStatus.HEALTHY:
						currHealthy++;
						break;
					case MonsterStatus.INJURED:
						currInjured++;
						break;
					default:
						currUnavail++;
						break;
				}

				if (cards[i].transform.parent != goonCardParent)
				{
					cards[i].transform.parent = goonCardParent;
					cards[i].gameObject.SetActive(false);
					cards[i].gameObject.SetActive(true);
				}

				i++;
			}

			//Set all of the headers on or off


			for (;i < cards.Count; i++)
			{
				cards[i].gameObject.SetActive(false);
			}
		}

		SetGroupTitles();

		dragPanel.collider.enabled = false;
		dragPanel.collider.enabled = true;

		goonTable.Reposition();
	}

	/// <summary>
	/// If we've got any sort of UserMonsterEvolutionProto going, even if
	/// it's missing parts (buddies or scientists), we call this
	/// </summary>
	/// <param name="currEvolution">Curr evolution.</param>
	public void OrganizeEvolution(MSGoonCard card)
	{
		evolutionElements.Init(card);

		evolveElements.FadeIn();
		goonPanelElements.FadeOut();
		scientistIcons.FadeOut();

		bottomBarLabel.GetComponent<MSUIHelper>().FadeIn();

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

		MonsterProto catalyst = MSDataManager.instance.Get<MonsterProto>(card.monster.monster.evolutionCatalystMonsterId);
		MonsterProto evoMonster = MSDataManager.instance.Get<MonsterProto>(card.monster.monster.evolutionMonsterId);

		bottomBarLabel.text = "You need " + mobsterColorString + "2 Lvl " + card.monster.monster.maxLevel + " " + card.monster.monster.displayName + "s[-] and "
			+ catalystColorString + card.monster.monster.numCatalystMonstersRequired + " " + catalyst.displayName + "(Evo " + catalyst.evolutionLevel + ")[-]" 
				+ "\nto evolve to " + evoMonster.displayName;
	}

	/// <summary>
	/// Like organizing the reserve cards for other things, except the cards are all already on the
	/// table and we're going to try to stack them, then reorder, rename, and rearrange
	/// </summary>
	void OrganizeEvolveCards()
	{
		foreach (var item in cards) 
		{
			if (item.buddy == null)
			{
				PZMonster buddy = MSMonsterManager.instance.FindEvolutionBuddy(item.monster);
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

	void OrganizeEnhance()
	{
		if (MSMonsterManager.instance.currentEnhancementMonster == null)
		{
			bringGoonIn = false;
			return;
		}
		bringGoonIn = true;
		enhanceBaseBox.Init (MSMonsterManager.instance.currentEnhancementMonster, GoonScreenMode.DO_ENHANCE);
	}

	IEnumerator RepositionAfterMove(float moveTime)
	{
		float currTime = 0;
		while (currTime < moveTime)
		{
			currTime += Time.deltaTime;
			goonTable.Reposition();
			yield return null;
		}
		goonTable.Reposition();
	}
	
	void AddReserveCardSlot()
	{
		MSGoonCard card = (MSPoolManager.instance.Get(goonCardPrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, dragPanel.transform) as MSSimplePoolable).GetComponent<MSGoonCard>();
		card.transform.localScale = Vector3.one;
		cards.Add (card);
	}

	void SetGroupTitles ()
	{
		injuredTitle.Fade (currInjured > 0);
		injuredHeader.SetActive (currInjured > 0);
		healthyTitle.Fade (currHealthy > 0);
		healthyHeader.SetActive (currHealthy > 0);
		unavailableTitle.Fade (currUnavail > 0);
		unavailableHeader.SetActive (currUnavail > 0);
	}

	void Update()
	{
		if (!goonIn && bringGoonIn)
		{
			goonIn = true;
			StartCoroutine(BringInEnhanceGoon());
		}
		if (goonIn && !bringGoonIn)
		{
			goonIn = false;
			StartCoroutine(TakeOutEnhanceGoon(currMode != GoonScreenMode.DO_ENHANCE && currMode != GoonScreenMode.EVOLVE));
		}
	}

	IEnumerator BringInEnhanceGoon()
	{
		Debug.LogWarning("Bringing enhancement goon in");
		
		enhanceLeftSideElements.ResetToBeginning();
		enhanceLeftSideElements.PlayForward();

		while (enhanceLeftSideElements.tweenFactor < 1)
		{
			dragPanel.scrollView.RestrictWithinBounds(false);
			yield return null;
		}
		dragPanel.scrollView.RestrictWithinBounds(false);

	}

	void FinishGoonInNow()
	{
		Debug.LogWarning("Snapping enhancing goon in");

		enhanceLeftSideElements.Sample(1, true);

		goonTable.Reposition();
	}

	/// <summary>
	/// Takes out the enhancement goon on the left.
	/// If we're in any mode other than Enhancing, this should
	/// be done immediately rather than waiting for the tween.
	/// </summary>
	/// <returns>The out enhance goon.</returns>
	/// <param name="instant">If set to <c>true</c> instant.</param>
	IEnumerator TakeOutEnhanceGoon(bool instant)
	{
		Debug.LogWarning("Taking enhancement goon out");

		if (instant)
		{
			enhanceLeftSideElements.Sample(0, true);
		}
		else
		{
			enhanceLeftSideElements.PlayReverse();
			while (enhanceLeftSideElements.tweenFactor > 0)
			{
				dragPanel.scrollView.RestrictWithinBounds(false);
				yield return null;
			}
		}
		dragPanel.scrollView.RestrictWithinBounds(false);
	}

	MSGoonCard GetCardForMonster(PZMonster monster)
	{
		return cards.Find(x=>x.monster == monster);
	}
	
	void TrySpeedUpEnhance()
	{
		int gemCost = Mathf.CeilToInt((MSMonsterManager.instance.enhancementFeeders[MSMonsterManager.instance.enhancementFeeders.Count-1].combineTimeLeft) * 1f/60000 / MSWhiteboard.constants.minutesPerGem);
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, gemCost, TrySpeedUpEnhance))
		{
			MSMonsterManager.instance.SpeedUpEnhance(gemCost);
		}
	}

	public void DisplayErrorMessage(string error)
	{
		errorLabel.text = error;
		errorLabel.alpha = 1;
		errorLabel.GetComponent<MSUIHelper>().FadeOut();
	}
	
	void OnQueueChanged(PZMonster monster = null)
	{
		OrganizeCards();
	}
	
	void OnEnhanceQueueChanged()
	{
		OrganizeCards();
		OrganizeEnhance();
	}
	
	void OnRemoveTeamMember(PZMonster monster)
	{
		OrganizeCards();
	}
	
	void OnAddTeamMember(PZMonster monster)
	{
		teamCards[monster.userMonster.teamSlotNum-1].Init(monster);
		OrganizeReserveCards();
	}

	*/
}
