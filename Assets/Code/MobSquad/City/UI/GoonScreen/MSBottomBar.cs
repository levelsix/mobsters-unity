using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSBottomBar
/// </summary>
public class MSBottomBar : MonoBehaviour {

	[SerializeField]
	MSMiniGoonBox healBoxPrefab;

	[SerializeField]
	MSUIHelper emptyBoxPrefab;

	[SerializeField]
	UIGrid boxParent;

	[SerializeField]
	Vector3 bottomOffset = new Vector3(0, 200, 0);

	[SerializeField]
	UILabel slotsRemainingLabel;

	[SerializeField]
	UISprite arrowSprite;

	[SerializeField]
	MSUIHelper queueHelper;

	[SerializeField]
	UILabel bottomText;

	[SerializeField]
	MSUIHelper bottomTextHelper;

	[SerializeField]
	MSUIHelper scientists;

	[SerializeField]
	MSActionButton rightSideButton;

	[SerializeField]
	UILabel buttonHeader;

	[SerializeField]
	UILabel timeLeftHeader;

	[SerializeField]
	UILabel timeLeftLabel;

	[SerializeField]
	MSBottomBarModeButton topButton;

	[SerializeField]
	MSBottomBarModeButton bottomButton;

	[SerializeField]
	Color redTextColor = Color.red;

	[SerializeField]
	Color greenTextColor = Color.green;

	List<MSMiniGoonBox> boxes = new List<MSMiniGoonBox>();
	List<MSUIHelper> empties = new List<MSUIHelper>();

	GoonScreenMode mode = GoonScreenMode.HEAL;

	int currSellValue = 0;

	bool updateBottom = true;

	int numSlots
	{
		get
		{
			switch (mode) {
				case GoonScreenMode.HEAL:
					return MSHospitalManager.instance.queueSize;
				case GoonScreenMode.SELL:
					return boxes.Count + 1;
				case GoonScreenMode.DO_ENHANCE:
					//TODO: Tie this into lab shit and make this actually matter
					return 5;
				default:
					break;
			}
			return 0;
		}
	}

	const string GREEN_ARROW = "hospitalopenarrow";
	const string RED_ARROW = "closedhospitalarrow";

	const string CLICK_A_MOBSTER_TO_HEAL_TEXT = "Select a mobster to heal";
	const string CLICK_A_MOBSTER_TO_ENHANCE_TEXT = "Select a mobster to enhance";
	const string CLICK_A_MOBSTER_TO_SELL_TEXT = "Select a mobster to sell";
	const string CLICK_A_MOBSTER_TO_FEED_TEXT = "Select a mobster to use";
	const string NO_SLOTS_TEXT = "No slots available";
	const string QUEUE_FULL_TEXT = "QUEUE\nFULL";
	const string SLOTS_REMAINING_TEXT = " SLOTS\nOPEN";

	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterAddQueue += OnQueueAdd;
		MSActionManager.Goon.OnMonsterRemoveQueue += OnQueueRemove;
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory += OnMonsterRemove;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterAddQueue -= OnQueueAdd;
		MSActionManager.Goon.OnMonsterRemoveQueue -= OnQueueRemove;
		MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory -= OnMonsterRemove;
	}

	#region Initialization and Setup

	public void Init (GoonScreenMode mode)
	{
		this.mode = mode;
		Pool();
		switch (mode) {
			case GoonScreenMode.HEAL:
				InitHeal();
				break;
			case GoonScreenMode.DO_ENHANCE:
				InitEnhance();
				break;
			case GoonScreenMode.PICK_EVOLVE:
				InitEvolve();
				break;
			case GoonScreenMode.SELL:
				InitSell();
				break;
			default:
				break;
		}
		RefreshBottomDetails();
		SetupLeftButtons();
	}

	void InitHeal()
	{
		FillBoxes(MSHospitalManager.instance.healingMonsters);
	}

	void InitEnhance()
	{
		FillBoxes(MSMonsterManager.instance.enhancementFeeders);
	}

	void InitEvolve()
	{
		//For now, just keep letting GoonScreen take care of this
	}

	void InitSell()
	{
		currSellValue = 0;
		AddEmpty(int.MaxValue);
		boxParent.Reposition();
	}

	void FillBoxes(List<PZMonster> monsters)
	{
		//Debug.Log("Filling slots: " + numSlots);

		int i;
		for (i = 0; i < monsters.Count; i++) 
		{
			//Debug.Log("Box: " + i);
			AddBox (monsters[i], (int.MaxValue - i));
		}
		for (;i < numSlots; i++)
		{
			//Debug.Log("Empty: " + i);
			AddEmpty((int.MaxValue - i));
		}

		//Debug.Log("Repositioning");
		//boxParent.animateSmoothly = false;
		boxParent.Reposition();
	}

	MSUIHelper AddEmpty (int i)
	{
		MSUIHelper empty = (MSPoolManager.instance.Get(emptyBoxPrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, boxParent.transform) as MSSimplePoolable).GetComponent<MSUIHelper>();
		empty.name = "Empty" + i;
		empty.transform.localScale = Vector3.one;
		empty.FadeIn();
		empties.Add(empty);
		return empty;
	}

	MSMiniGoonBox AddBox (PZMonster monster, int i)
	{
		MSMiniGoonBox box = (MSPoolManager.instance.Get (healBoxPrefab.GetComponent<MSSimplePoolable> (), Vector3.zero, boxParent.transform) as MSSimplePoolable).GetComponent<MSMiniGoonBox> ();
		box.Init (monster);
		box.name = "Monster" + i;
		box.transform.localScale = Vector3.one;
		box.helper.FadeIn();
		boxes.Add (box);
		return box;
	}

	void SetupLeftButtons()
	{
		switch (mode) {
		case GoonScreenMode.HEAL:
		case GoonScreenMode.SELL:
			topButton.mode = GoonScreenMode.HEAL;
			topButton.Set(mode == GoonScreenMode.HEAL);
			bottomButton.mode = GoonScreenMode.SELL;
			bottomButton.Set(mode == GoonScreenMode.SELL);
			break;
		case GoonScreenMode.DO_ENHANCE:
		case GoonScreenMode.PICK_EVOLVE:
			topButton.mode = GoonScreenMode.DO_ENHANCE;
			topButton.Set(mode == GoonScreenMode.DO_ENHANCE);
			bottomButton.mode = GoonScreenMode.PICK_EVOLVE;
			bottomButton.Set(mode == GoonScreenMode.PICK_EVOLVE);
			break;
		default:
				break;
		}
	}

	#endregion

	void RefreshBottomDetails()
	{
		if (numSlots == 0)
		{
			bottomText.text = NO_SLOTS_TEXT;
			bottomTextHelper.FadeIn();
			queueHelper.FadeOut();
		}
		else if (boxes.Count == 0)
		{
			SetBottomText();
			bottomTextHelper.FadeIn();
			queueHelper.FadeOut();
		}
		else if (mode == GoonScreenMode.SELL)
		{
			slotsRemainingLabel.text = "";
			queueHelper.FadeIn();
			arrowSprite.spriteName = GREEN_ARROW;
			bottomTextHelper.FadeOut();
		}
		else if (boxes.Count == numSlots)
		{
			slotsRemainingLabel.text = QUEUE_FULL_TEXT;
			slotsRemainingLabel.color = redTextColor;
			queueHelper.FadeIn();
			arrowSprite.spriteName = RED_ARROW;
			bottomTextHelper.FadeOut();
		}
		else
		{
			slotsRemainingLabel.text = (numSlots-boxes.Count).ToString() + SLOTS_REMAINING_TEXT;
			slotsRemainingLabel.color = greenTextColor;
			queueHelper.FadeIn();
			arrowSprite.spriteName = GREEN_ARROW;
			bottomTextHelper.FadeOut();
		}
	}

	void SetBottomText()
	{
		switch (mode) 
		{
		case GoonScreenMode.HEAL:
			bottomText.text = CLICK_A_MOBSTER_TO_HEAL_TEXT;
			break;
		case GoonScreenMode.SELL:
			bottomText.text = CLICK_A_MOBSTER_TO_SELL_TEXT;
			break;
		case GoonScreenMode.DO_ENHANCE:
			if (MSMonsterManager.instance.currentEnhancementMonster == null)
			{
				bottomText.text = CLICK_A_MOBSTER_TO_ENHANCE_TEXT;
			}
			else
			{
				bottomText.text = CLICK_A_MOBSTER_TO_FEED_TEXT;
			}
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Updates the timer that lists total time remaining
	/// SELL MODE: Instead of listing a time, lists total sell value
	/// EVOLVE MODE: Ignore
	/// </summary>
	void UpdateTimer()
	{
		long timeLeft;
		switch (mode) {
			case GoonScreenMode.HEAL:
				timeLeft = MSHospitalManager.instance.lastFinishTime - MSUtil.timeNowMillis;
				timeLeftLabel.text = MSUtil.TimeStringShort(timeLeft);
				rightSideButton.label.text = "(g) " + MSMath.GemsForTime(timeLeft);
				buttonHeader.text = "FINISH";
				timeLeftHeader.text = "Time Left";
				break;
			case GoonScreenMode.DO_ENHANCE:
				timeLeft = MSMonsterManager.instance.lastEnhance - MSUtil.timeNowMillis;
				timeLeftLabel.text = MSUtil.TimeStringShort(timeLeft);
				buttonHeader.text = "FINISH";
				rightSideButton.label.text = "(g) " + MSMath.GemsForTime(timeLeft);
				timeLeftHeader.text = "Time Left";
				break;
			case GoonScreenMode.SELL:
				rightSideButton.label.text = "$" + currSellValue;
				buttonHeader.text = "SELL";
				timeLeftLabel.text = "";
				timeLeftHeader.text = "";
				break;
			default:
				break;
		}
	}

	void Update()
	{
		UpdateTimer();
	}

	/// <summary>
	/// Assigned to button in inspector
	/// Controls the logic that happens when the button on the very right is clicked
	/// HEAL: Finishes the current heal
	/// SELL: Sells all monsters in the queue
	/// ENHANCE: Finishes the current enhance
	/// EVOLVE: The button shouldn't exist in this mode
	/// </summary>
	public void OnClickButton()
	{
		switch (mode) {
		case GoonScreenMode.HEAL:
			//MSHospitalManager.instance.TrySpeedUpHeal();
			break;
		case GoonScreenMode.SELL:
			List<PZMonster> monsters = new List<PZMonster>();
			foreach (var item in boxes) 
			{
				monsters.Add (item.monster);
			}
			MSMonsterManager.instance.SellMonsters(monsters);
			break;
			default:
					break;
		}
	}

	void RemoveBox(MSMiniGoonBox box)
	{
		if (mode == GoonScreenMode.SELL)
		{
			currSellValue -= box.monster.sellValue;
		}
		
		//Debug.Log("Queue removing: " + box.monster.monster.name);
		
		boxes.Remove(box);
		
		if (boxes.Count + empties.Count < numSlots)
		{
			MSUIHelper empty = AddEmpty(empties.Count);
			empties.Insert(0, empty);
			
			empty.FadeIn();
			
			empty.transform.localPosition = box.transform.localPosition;
		}
		
		box.transform.parent = box.transform.parent.parent;
		box.helper.FadeOutAndPool();
		
		boxParent.animateSmoothly = true;
		boxParent.Reposition();
		
		RefreshBottomDetails();
	}

	/// <summary>
	/// When a monster is removed from the user's monsters (by selling 
	/// or feeding), we need to make sure
	/// that its box gets pulled out from the queue
	/// </summary>
	/// <param name="userMonsterId">User monster identifier.</param>
	void OnMonsterRemove(long userMonsterId)
	{
		MSMiniGoonBox box = boxes.Find(x=>x.monster.userMonster.userMonsterId == userMonsterId);
		if (box != null)
		{
			RemoveBox(box);
		}
	}

	/// <summary>
	/// When a monster is removed from the queue (by having its removal button clicked
	/// or by finishing a heal), we need to make sure that its box gets
	/// pulled out from the queue
	/// </summary>
	/// <param name="monster">Monster.</param>
	void OnQueueRemove(PZMonster monster)
	{
		MSMiniGoonBox box = boxes.Find(x=>x.monster == monster);
		if (box != null)
		{
			RemoveBox(box);
		}

		if (mode != GoonScreenMode.SELL ) {
			foreach (PZMonster item in MSMonsterManager.instance.userTeam) {
				if(monster == item){
					MSBuildingManager.instance.AddMonsterToScene (monster, MSBuildingManager.instance.playerUnits);
					break;
				}
			}
		}
	}

	void OnQueueAdd(PZMonster monster)
	{
		if (empties.Count == 0)
		{
			Debug.LogError("QUEUE IS FULL!\nYou've got problems, man...");
			return;
		}

		if (mode == GoonScreenMode.SELL) {
			currSellValue += monster.sellValue;
		} else {
			foreach (PZMonster item in MSMonsterManager.instance.userTeam) {
				if(monster == item){
					MSBuildingManager.instance.RemoveMonsterFromScene (monster, MSBuildingManager.instance.playerUnits);
					break;
				}
			}
		}

		//Grab an empty slot to fade out
		MSUIHelper empty = empties[0];
		empties.RemoveAt(0);

		//Now the fun, let's add a box
		MSMiniGoonBox box = AddBox(monster, int.MaxValue - boxes.Count);

		box.helper.FadeIn();

		//Place the new box at the X of the empty and way below the Y
		box.transform.localPosition = empty.transform.localPosition + bottomOffset;
		
		//Get it out of the grid so that we can reposition around it
		empty.transform.parent = empty.transform.parent.parent; 
		empty.FadeOutAndPool();

		while (empties.Count + boxes.Count < numSlots)
		{
			AddEmpty(int.MaxValue - empties.Count);
		}

		boxParent.animateSmoothly = true;
		boxParent.Reposition();

		RefreshBottomDetails();
	}

	void Pool()
	{
		foreach (var item in boxes) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}
		boxes.Clear();
		
		foreach (var item in empties) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}
		empties.Clear();
	}
}
