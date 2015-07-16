using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;

public class MSSoomlaPurchaseManager : MonoBehaviour 
{
	
	// Use this for initialization
	
	void Start () {
		Debug.Log( "MSSoomlaPurchaseManager starting" );
		
		SoomlaStore.Initialize(new MSAssets());

		SoomlaStore.StartIabServiceInBg();

		StoreEvents.OnMarketPurchaseStarted      += OnMarketPurchaseStarted;
		StoreEvents.OnMarketPurchase             += OnMarketPurchase;
		StoreEvents.OnItemPurchaseStarted        += OnItemPurchaseStarted;
		StoreEvents.OnItemPurchased              += OnItemPurchased;
		StoreEvents.OnUnexpectedStoreError       += OnUnexpectedStoreError;
	}
	
	string s = "<nothing>";
	
	public void OnMarketPurchaseStarted( PurchasableVirtualItem pvi ) {
		Debug.Log( "OnMarketPurchaseStarted: " + pvi.ItemId );
		s += "OnMarketPurchaseStarted: " + pvi.ItemId;
	}
	
	public void OnMarketPurchase( PurchasableVirtualItem pvi, string s1, Dictionary<string, string> dict ) {
		Debug.Log( "OnMarketPurchase: " + pvi.ItemId + ", " + s1 + ", " + dict );
		s += "OnMarketPurchase: " + pvi.ItemId;
	}
	
	public void OnItemPurchaseStarted( PurchasableVirtualItem pvi ) {
		Debug.Log( "OnItemPurchaseStarted: " + pvi.ItemId );
		s += "OnItemPurchaseStarted: " + pvi.ItemId;
	}
	
	public void OnItemPurchased( PurchasableVirtualItem pvi, string s1 ) {
		Debug.Log( "OnItemPurchased: " + pvi.ItemId + ", " + s1 );
		s += "OnItemPurchased: " + pvi.ItemId;
	}
	
	public void OnStoreControllerInitialized( ) {
		Debug.Log( "OnStoreControllerInitialized" );
		s += "OnStoreControllerInitialized";
	}
	
	public void OnUnexpectedStoreError( int err ) {
		Debug.Log( "OnUnexpectedErrorInStore" + err );
		s += "OnUnexpectedErrorInStore" + err;
	}

	public static void Buy(MSAssets.IAPSize size)
	{
		StoreInventory.BuyItem(MSAssets.packNames[size]);
	}

}
