using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;
using com.soomla.unity;

/// <summary>
/// @author Rob Giusti
/// MSAssets
/// </summary>
public class MSAssets : IStoreAssets {

	public int GetVersion()
	{
		return 1;
	}

	public VirtualCurrency[] GetCurrencies()
	{
		return new VirtualCurrency[] {GEM_CURRENCY};
	}

	public VirtualGood[] GetGoods() {return new VirtualGood[]{};}

	public VirtualCurrencyPack[] GetCurrencyPacks() {
		return new VirtualCurrencyPack[] {GEM50PACK, GEM120PACK, GEM250PACK, GEM650PACK, GEM1500PACK};
	}

	public VirtualCategory[] GetCategories() { return new VirtualCategory[]{};}

	public NonConsumableItem[] GetNonConsumableItems() { return new NonConsumableItem[]{};}

	public const string GEM_CURRENCY_ITEM_ID = "currency_gem";
	public const string GEM50PACK_ITEM_ID = "pile_of_gems";
	public const string GEM120PACK_ITEM_ID = "bag_of_gems";
	public const string GEM250PACK_ITEM_ID = "case_of_gems";
	public const string GEM650PACK_ITEM_ID = "safe_of_gems";
	public const string GEM1500PACK_ITEM_ID = "big_safe_of_gems";

	public static VirtualCurrency GEM_CURRENCY = new VirtualCurrency(
		"Gems",
		"",
		GEM_CURRENCY_ITEM_ID);

	public static VirtualCurrencyPack GEM50PACK = new VirtualCurrencyPack(
		"Pile of Gems",
		"50 Gems",
		GEM50PACK_ITEM_ID,
		50,
		GEM_CURRENCY_ITEM_ID,
		new PurchaseWithMarket(GEM50PACK_ITEM_ID, 4.99)
	);
	
	public static VirtualCurrencyPack GEM120PACK = new VirtualCurrencyPack(
		"Sack of Gems",
		"120 Gems",
		GEM120PACK_ITEM_ID,
		120,
		GEM_CURRENCY_ITEM_ID,
		new PurchaseWithMarket(GEM120PACK_ITEM_ID, 9.99)
	);
	public static VirtualCurrencyPack GEM250PACK = new VirtualCurrencyPack(
		"Sack of Gems",
		"120 Gems",
		GEM250PACK_ITEM_ID,
		250,
		GEM_CURRENCY_ITEM_ID,
		new PurchaseWithMarket(GEM250PACK_ITEM_ID, 19.99)
	);

	public static VirtualCurrencyPack GEM650PACK = new VirtualCurrencyPack(
		"Sack of Gems",
		"650 Gems",
		GEM650PACK_ITEM_ID,
		650,
		GEM_CURRENCY_ITEM_ID,
		new PurchaseWithMarket(GEM650PACK_ITEM_ID, 49.99)
	);
	
	public static VirtualCurrencyPack GEM1500PACK = new VirtualCurrencyPack(
		"Sack of Gems",
		"1,500 Gems",
		GEM1500PACK_ITEM_ID,
		1500,
		GEM_CURRENCY_ITEM_ID,
		new PurchaseWithMarket(GEM1500PACK_ITEM_ID, 99.99)
	);
}
