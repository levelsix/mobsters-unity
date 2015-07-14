using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSGachaScreen : MonoBehaviour {
	
	[SerializeField]
	MSGachaSpinner spinner;
	
	[SerializeField]
	MSGachaFeaturedMobster[] featuredMobsters;
	
	[SerializeField]
	MSGachaTab basicGrab;

	[SerializeField]
	MSGachaTab goonieGrab;
	
	[SerializeField]
	UILabel oneSpinCostLabel;
	
	[SerializeField]
	UILabel tenSpinCostLabel;

	[SerializeField]
	UISprite machine;
	
	public BoosterPackProto currPack;
	
	int nextLeftIndex;
	int nextRightIndex;

	const int BASIC_GRAB_ID = 1;
	
	public void OnEnable()
	{
		//Set up the tabs
		foreach (BoosterPackProto booster in MSDataManager.instance.GetAll<BoosterPackProto>().Values) 
		{
			if (currPack == null)
				currPack = booster;

			if (booster.boosterPackName == "Basic Grab")
			{
				basicGrab.Init(booster, this);
			}
			else if (booster.boosterPackName == "Ultimate Grab")
			{
				goonieGrab.Init(booster, this);
			}
		}
		Init (currPack); //Gotta call this after, else the tabs won't init properly

		MSActionManager.Gacha.OnPurchaseBoosterSucces += PurchaseBoosterSucces;
		
	}

	void OnDisable()
	{
		MSActionManager.Gacha.OnPurchaseBoosterSucces -= PurchaseBoosterSucces;
	}

	void PurchaseBoosterSucces()
	{
		oneSpinCostLabel.text = currPack.gemPrice.ToString();
	}
	
	public void Init(BoosterPackProto pack)
	{
		currPack = pack;
		
		spinner.Init(pack);

		machine.spriteName = MSUtil.StripExtensions( pack.machineImgName);
		machine.MakePixelPerfect ();

		if (currPack.specialItems.Count == 0) {
			foreach (var item in featuredMobsters) {
				item.gameObject.SetActive(false);
			}
		} else {
			nextLeftIndex = LoopDisplayItemIndex (-1);
			nextRightIndex = LoopDisplayItemIndex (2);
			foreach (var item in featuredMobsters) {
				item.gameObject.SetActive(true);
				item.Init (PickGoonLeft ());
			}
		}

		//Only the basic grab can be free.  Basic grab is ID 1
		if( MSUtil.timeSince(MSWhiteboard.localUser.lastFreeBoosterPackTime) > 24 * 60 * 60 * 1000 && pack.boosterPackId == BASIC_GRAB_ID)
		{
			oneSpinCostLabel.text = "Free";
		}
		else
		{
			oneSpinCostLabel.text = pack.gemPrice.ToString();
		}
		//Ten spin button is disabled for now
		tenSpinCostLabel.text = (pack.gemPrice * 10).ToString();
	}
	
	#region Display Items
	
	public BoosterItemProto PickGoonLeft()
	{
		int index = nextLeftIndex;
		nextLeftIndex = LoopDisplayItemIndex(--nextLeftIndex);
		nextRightIndex = LoopDisplayItemIndex(--nextRightIndex);
		return currPack.specialItems[index];
	}
	
	public BoosterItemProto PickGoonRight()
	{
		int index = nextRightIndex;
		nextLeftIndex = LoopDisplayItemIndex(++nextLeftIndex);
		nextRightIndex = LoopDisplayItemIndex(++nextRightIndex);
		return currPack.specialItems[index];
	}
	
	public int LoopDisplayItemIndex(int index)
	{
		if (currPack.specialItems.Count == 0)
			return 0;

		while (index >= currPack.specialItems.Count)
		{
			index -= currPack.specialItems.Count;
		}
		while (index < 0)
		{
			index += currPack.specialItems.Count;	
		}
		return index;
	}
	
	#endregion
	
//	#region Tabs
//	
//	void AddTab(BoosterPackProto booster)
//	{
//		MSGachaTab tab = (MSPoolManager.instance.Get(gachaTabPrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, gachaTabGrid.transform) 
//		                  as MSSimplePoolable).GetComponent<MSGachaTab>();
//		tab.transform.localScale = Vector3.one;
//		tab.Init (booster, this);
//		gachaTabs.Add(tab);
//	}
//	
//	void RecycleTabs()
//	{
//		foreach (var item in gachaTabs) 
//		{
//			item.GetComponent<MSSimplePoolable>().Pool();
//		}
//	}
//	
//	#endregion
}
