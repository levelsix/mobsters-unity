using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// Created by Kenneth Cox,
/// 
/// Put on hold at the start of the great anti-andriod act of Jan 2015
/// The initialization of the popup does not take into acount that the popup may need to animation its position.
/// SetHorizontalPosition and SetVerticalPosition need to be changed _if_ it does.
/// Both of these fuctions are currently left out of the initialization pipeline.
/// right now this popup is mostly functional, there is missing funcionality for the buttons as we refactor code into MSTimer,
 /// so how we're getting certain information to the server is unclear
/// 
/// There should be two popups, one for speedups and one for resources
/// really all that's left is to fix up the position logic to work with animation,
/// figure out making entries black and white,
/// and code all buttons to open this popup instead of paying gems
/// </summary>

public class MSItemSelectionPopup : MonoBehaviour {

	enum ItemScreenType
	{
		SPEED_UP,
		FILL_RESOURCES,

	}

	UISprite BG;

	[SerializeField]
	UILabel title;

	[SerializeField]
	MSNaturalSortTable table;

	[SerializeField]
	MSSimplePoolable selectionEntry;

	[SerializeField]
	UISprite sideTriangle;

	[SerializeField]
	UISprite topTriangle;

	List<MSItemSelectionEntry> entries = new List<MSItemSelectionEntry>();

	[SerializeField]
	UIScrollView scrollView;

	[SerializeField]
	MSFillBar timeLeftBar;

	[SerializeField]
	UILabel timeLeftLabel;

	[SerializeField]
	MSPopup selfPopup;

	MSTimer currTimer;

	const float PANEL_BUFFER_WITH_TIMER = -140f;
	const float PANEL_BUFFER_WITHOUT_TIMER = -75f;

	public void Init(ResourceType resourceType)
	{
		foreach(var item in entries)
		{
			GetComponent<MSSimplePoolable>().Pool();
		}
		
		entries.Clear();

		int resourceAmountNeeded = MSResourceManager.maxes[(int)resourceType-1] - MSResourceManager.resources[resourceType];
		int gemCost = MSResourceManager.instance.SpendGemsForOtherResource(resourceType, resourceAmountNeeded);

		MSItemSelectionEntry nextEntry = MSPoolManager.instance.Get<MSItemSelectionEntry>(selectionEntry, table.transform);
		nextEntry.InitGem(resourceType, gemCost, delegate{ nextEntry.FillResourceWithGemsOnClick(resourceAmountNeeded);});
		nextEntry.transform.localScale = Vector3.one;

		foreach(ItemProto item in MSDataManager.instance.GetAll<ItemProto>().Values)
		{
			if(item.itemType != ItemType.ITEM_CASH || item.itemType != ItemType.ITEM_OIL)
			{
				//Skip if it's not a resource item
				continue;
			}
			nextEntry = MSPoolManager.instance.Get<MSItemSelectionEntry>(selectionEntry, table.transform);
			nextEntry.InitItem(item.itemId, nextEntry.SpeedUpOnClick, scrollView);
			nextEntry.transform.localScale = Vector3.one;
		}
		
		table.Reposition();
	}

	/// <summary>
	/// Init the specified actionType, timer and clickedButton.
	/// </summary>
	/// <param name="actionType">The type of action that is being sped up.</param>
	/// <param name="timer">The MSTimer in charge of keeping track of how long this action should take.</param>
	/// <param name="clickedButton">The sprite that this popup is going to point to.</param>
	public void Init(GameActionType actionType, MSTimer timer, UISprite clickedButton)
	{
		currTimer = timer;
		switch(actionType)
		{
		case GameActionType.COMBINE_MONSTER:
			InitSpeedUp(false, clickedButton);
			break;
		case GameActionType.ENHANCE_TIME:
			InitSpeedUp(false, clickedButton);
			break;
		case GameActionType.ENTER_PERSISTENT_EVENT:
			InitSpeedUp(false, clickedButton);
			break;
		case GameActionType.EVOLVE:
			InitSpeedUp(false, clickedButton);
			break;
		case GameActionType.GAME_ACTION_TYPE_RESEARCH:
			InitSpeedUp(false, clickedButton);
			break;
		case GameActionType.HEAL:
			InitSpeedUp(true, clickedButton);
			break;
		case GameActionType.MINI_JOB:
			InitSpeedUp(false, clickedButton);
			break;
		case GameActionType.NO_HELP:
			Debug.LogError("Not sure what no help is supposed to represent");
			break;
		case GameActionType.REMOVE_OBSTACLE:
			InitSpeedUp(false, clickedButton);
			break;
		case GameActionType.UPGRADE_STRUCT:
			InitSpeedUp(true, clickedButton);
			break;
		default:
			Debug.LogError("An unknown Game Action Type of used to create the item selection popup: " + actionType.ToString(), this);
			break;
		}
	}

	void InitSpeedUp(bool canBefree, UISprite clickedButton)
	{
		foreach(var item in entries)
		{
			GetComponent<MSSimplePoolable>().Pool();
		}
		
		entries.Clear();
		
		MSItemSelectionEntry nextEntry = MSPoolManager.instance.Get<MSItemSelectionEntry>(selectionEntry, table.transform);
		nextEntry.InitGem(currTimer, canBefree, nextEntry.SpeedUpWithGemsOnClick);
		nextEntry.transform.localScale = Vector3.one;

		foreach(ItemProto item in MSDataManager.instance.GetAll<ItemProto>().Values)
		{
			if(item.itemType != ItemType.SPEED_UP)
			{
				//skip if it's not a speed up item
				continue;
			}
			nextEntry = MSPoolManager.instance.Get<MSItemSelectionEntry>(selectionEntry, table.transform);
			nextEntry.InitItem(item.itemId, nextEntry.SpeedUpOnClick, scrollView);
			nextEntry.transform.localScale = Vector3.one;
		}
		
		table.Reposition();
	}

	IEnumerator UpdateTimerBar()
	{
		while(!currTimer.done)
		{
			timeLeftBar.max = (int)currTimer.length;
			timeLeftBar.fill = (float)currTimer.progress;
			timeLeftLabel.text = MSUtil.TimeStringShort(currTimer.timeLeft);
			yield return new WaitForEndOfFrame();
		}
		//TODO: when the timer is done does the popup auto close?
	}

	void SetHorizontalPosition(UISprite clickedButton)
	{
		//folowing code assumes 0,0 is the center of the screen
		//at the time of writing, this is true

		topTriangle.gameObject.SetActive(false);
		sideTriangle.gameObject.SetActive(true);

		BG.transform.localPosition = Vector3.zero;
		Vector3 relativeButtonPosition = transform.parent.InverseTransformPoint(clickedButton.transform.position);
		Vector3 newPosition = BG.transform.localPosition;

		bool createPopupToLeft = clickedButton.transform.position.x >= 0;
		if(createPopupToLeft)
		{
			newPosition.x = relativeButtonPosition.x - ((float)clickedButton.width/2f) - (float)sideTriangle.width - ((float)BG.width/2f);
			sideTriangle.transform.localScale = Vector3.one;
		}
		else
		{
			newPosition.x = relativeButtonPosition.x + ((float)clickedButton.width/2f) + (float)sideTriangle.width + ((float)BG.width/2f);
			sideTriangle.transform.localScale = new Vector3(1f, -1f, 1f);
		}

		sideTriangle.transform.position = clickedButton.transform.position;//get equivilent Ys

		//The top of the background must be atleast this high
		float topPoint = transform.parent.InverseTransformPoint(sideTriangle.transform.position).y + (sideTriangle.height/2);
		//the bottom of the background has to be at least this low
		float bottomPoint = transform.parent.InverseTransformPoint(sideTriangle.transform.position).y - (sideTriangle.height/2);

		if(newPosition.y + ((float)BG.height/2f) < topPoint)
		{
			newPosition.y += topPoint - (newPosition.y + ((float)BG.height/2f));
		}
		else if(newPosition.y - ((float)BG.height/2f) > bottomPoint)
		{
			newPosition.y += bottomPoint - (newPosition.y - ((float)BG.height/2f));
		}

		transform.localPosition = newPosition;

		Vector3 newSidePosition = sideTriangle.transform.localPosition;
		if(createPopupToLeft)
		{
			newSidePosition.x = BG.width/2 + sideTriangle.width/2;
		}
		else
		{
			newSidePosition.x = -BG.width/2 - sideTriangle.width/2;
		}

		BG.transform.localPosition = newPosition;
		sideTriangle.transform.localPosition = newSidePosition;
		
	}

	void SetVerticalPosition(Transform target)
	{
		
		topTriangle.gameObject.SetActive(true);
		sideTriangle.gameObject.SetActive(false);

		float YBUFFER = 15;
		transform.position = Vector3.zero;
		topTriangle.transform.position = target.position;
		topTriangle.transform.localPosition = new Vector3(topTriangle.transform.localPosition.x, topTriangle.transform.localPosition.y - YBUFFER - ((float)topTriangle.height/2f), 0f);
		float newXPosition = BG.transform.localPosition.x;
		if(topTriangle.transform.localPosition.x + ((float)topTriangle.width/2f) > ((float)BG.width/2f) + BG.transform.localPosition.x)
		{
			newXPosition = BG.transform.localPosition.x + ((topTriangle.transform.localPosition.x + ((float)topTriangle.width/2f)) - (((float)BG.width/2f) + BG.transform.localPosition.x));
		}
		else if(topTriangle.transform.localPosition.x - ((float)topTriangle.width/2f) < BG.transform.localPosition.x - ((float)BG.width/2f))
		{
			newXPosition = BG.transform.localPosition.x + ((topTriangle.transform.localPosition.x - ((float)topTriangle.width/2f)) - (BG.transform.localPosition.x - ((float)BG.width/2f)));
		}

		BG.transform.localPosition = new Vector3(newXPosition, topTriangle.transform.localPosition.y - ((float)topTriangle.height/2f) - ((float)BG.height/2f), 0f);
	}

	[ContextMenu("test")]
	void TestInit()
	{
		foreach(var item in entries)
		{
			GetComponent<MSSimplePoolable>().Pool();
		}
		
		entries.Clear();
		
		MSItemSelectionEntry nextEntry = MSPoolManager.instance.Get<MSItemSelectionEntry>(selectionEntry, table.transform);
		nextEntry.InitGem(null, true, delegate { Debug.Log("gem button"); });
		nextEntry.transform.localScale = Vector3.one;
		
		foreach(ItemProto item in MSDataManager.instance.GetAll<ItemProto>().Values)
		{
			if(item.itemType == ItemType.BOOSTER_PACK)
			{
				continue;
			}
			nextEntry = MSPoolManager.instance.Get<MSItemSelectionEntry>(selectionEntry, table.transform);
			nextEntry.InitItem(item.itemId, delegate { Debug.Log("clicked the item " + item.name); }, scrollView);
			nextEntry.transform.localScale = Vector3.one;
		}

		table.Reposition();
	}
}
