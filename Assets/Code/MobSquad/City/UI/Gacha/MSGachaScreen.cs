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
	MSGachaTab gachaTabPrefab;
	
	[SerializeField]
	UIGrid gachaTabGrid;
	
	[SerializeField]
	UILabel oneSpinCostLabel;
	
	[SerializeField]
	UILabel tenSpinCostLabel;
	
	List<MSGachaTab> gachaTabs = new List<MSGachaTab>();
	
	public BoosterPackProto currPack;
	
	int nextLeftIndex;
	int nextRightIndex;
	
	public void OnEnable()
	{
		RecycleTabs();
		
		currPack = null;
		foreach (BoosterPackProto booster in MSDataManager.instance.GetAll<BoosterPackProto>().Values) 
		{
			if (currPack == null || currPack.boosterPackId == 0)
			{
				currPack = booster;
			}
			AddTab(booster);
		}
		Init (currPack); //Gotta call this after, else the tabs won't init properly
		
		gachaTabGrid.Reposition();
	}
	
	public void Init(BoosterPackProto pack)
	{
		currPack = pack;
		
		spinner.Init(pack);
		
		nextLeftIndex = LoopDisplayItemIndex(-1);
		nextRightIndex = LoopDisplayItemIndex(2);
		
		foreach (var item in gachaTabs) 
		{
			item.OnNewBoosterActive(pack);
		}
		
		foreach (var item in featuredMobsters) 
		{
			item.Init(PickGoonLeft());
		}

		if( MSUtil.timeSince(MSWhiteboard.localUser.lastFreeBoosterPackTime) > 24 * 60 * 60 * 1000)
		{
			oneSpinCostLabel.text = "Free";
		}
		else
		{
			oneSpinCostLabel.text = pack.gemPrice.ToString();
		}
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
	
	#region Tabs
	
	void AddTab(BoosterPackProto booster)
	{
		MSGachaTab tab = (MSPoolManager.instance.Get(gachaTabPrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, gachaTabGrid.transform) 
		                  as MSSimplePoolable).GetComponent<MSGachaTab>();
		tab.transform.localScale = Vector3.one;
		tab.Init (booster, this);
		gachaTabs.Add(tab);
	}
	
	void RecycleTabs()
	{
		foreach (var item in gachaTabs) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}
	}
	
	#endregion
}
