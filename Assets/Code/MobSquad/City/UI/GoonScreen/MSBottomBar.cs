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
	Color redTextColor = Color.red;

	[SerializeField]
	Color greenTextColor = Color.green;

	List<MSMiniGoonBox> boxes = new List<MSMiniGoonBox>();
	List<MSUIHelper> empties = new List<MSUIHelper>();

	GoonScreenMode mode = GoonScreenMode.HEAL;

	MSGoonScreen goonScreen;

	int numSlots;

	const string GREEN_ARROW = "hospitalopenarrow";
	const string RED_ARROW = "closedhospitalarrow";

	const string CLICK_A_MOBSTER_TEXT = "Tap a Mobster to Begin";
	const string NO_SLOTS_TEXT = "No slots available";
	const string QUEUE_FULL_TEXT = "QUEUE\nFULL";
	const string SLOTS_REMAINING_TEXT = " SLOTS\nOPEN";

	void OnEnable()
	{
		MSActionManager.Goon.OnMonsterAddQueue += OnQueueAdd;
		MSActionManager.Goon.OnMonsterRemoveQueue += OnQueueRemove;
	}

	void OnDisable()
	{
		MSActionManager.Goon.OnMonsterAddQueue -= OnQueueAdd;
		MSActionManager.Goon.OnMonsterRemoveQueue -= OnQueueRemove;
	}

	public void Init (GoonScreenMode mode)
	{
		Pool();
		switch (mode) {
			case GoonScreenMode.HEAL:
				InitHeal();
				break;
			case GoonScreenMode.ENHANCE:
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
		RefreshBottomDetails();
	}

	void InitHeal()
	{
		numSlots = MSHospitalManager.instance.queueSize;

		FillBoxes(MSHospitalManager.instance.healingMonsters);
	}

	void InitEnhance()
	{

	}

	void InitEvolve()
	{

	}

	void InitSell()
	{

	}

	void FillBoxes(List<PZMonster> monsters)
	{
		Debug.Log("Filling slots: " + numSlots);

		int i;
		for (i = 0; i < monsters.Count; i++) 
		{
			Debug.Log("Box: " + i);
			AddBox (monsters[i], (int.MaxValue - i));
		}
		for (;i < numSlots; i++)
		{
			Debug.Log("Empty: " + i);
			AddEmpty((int.MaxValue - i));
		}

		Debug.Log("Repositioning");
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
			bottomText.text = CLICK_A_MOBSTER_TEXT;
			bottomTextHelper.FadeIn();
			queueHelper.FadeOut();
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

	void OnQueueRemove(PZMonster monster)
	{
		MSMiniGoonBox box = boxes.Find(x=>x.monster == monster);
		if (box != null)
		{
			Debug.Log("Queue removing: " + monster.monster.name);

			boxes.Remove(box);

			MSUIHelper empty = AddEmpty(empties.Count);
			empties.Insert(0, empty);

			empty.FadeIn();

			empty.transform.localPosition = box.transform.localPosition;

			box.transform.parent = box.transform.parent.parent;
			box.helper.FadeOutAndPool();
			
			boxParent.animateSmoothly = true;
			boxParent.Reposition();

			RefreshBottomDetails();
		}
	}

	void OnQueueAdd(PZMonster monster)
	{
		if (empties.Count == 0)
		{
			Debug.LogError("QUEUE IS FULL!\nYou've got problems, man...");
			return;
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
