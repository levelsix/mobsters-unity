using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;
using com.lvl6.proto;

public class MSSoomlaPurchaseManager : MonoBehaviour 
{
	
	// Use this for initialization
	
	void Start () {
		Debug.Log( "MSSoomlaPurchaseManager starting" );

		StoreEvents.OnMarketPurchaseStarted      += OnMarketPurchaseStarted;
		StoreEvents.OnMarketPurchase             += OnMarketPurchase;
		StoreEvents.OnItemPurchaseStarted        += OnItemPurchaseStarted;
		StoreEvents.OnItemPurchased              += OnItemPurchased;
		StoreEvents.OnUnexpectedStoreError       += OnUnexpectedStoreError;
		
		SoomlaStore.Initialize(new MSAssets());
		
		SoomlaStore.StartIabServiceInBg();
	}
	
	string s = "<nothing>";
	
	public void OnMarketPurchaseStarted( PurchasableVirtualItem pvi ) {
		Debug.Log( "OnMarketPurchaseStarted: " + pvi.ItemId );
		s += "OnMarketPurchaseStarted: " + pvi.ItemId;
	}
	
	public void OnMarketPurchase( PurchasableVirtualItem pvi, string s1, Dictionary<string, string> dict ) {
		Debug.Log( "OnMarketPurchase: " + pvi.ItemId + ", " + s1 + ", " + dict );
		s += "OnMarketPurchase: " + pvi.ItemId;

		foreach (string k in dict.Keys) {
			Debug.Log (k + ": " + dict[k]);
		}

		JSONObject json = new JSONObject (dict);
		string receiptData = json.ToString();
		
		InAppPurchaseRequestProto req = new InAppPurchaseRequestProto ();
		req.sender = MSWhiteboard.localMup;
		req.receipt = receiptData;
		
		Debug.Log ("Sending receipt: " + receiptData);
		
		UMQNetworkManager.instance.SendRequest(req, (int)EventProtocolRequest.C_IN_APP_PURCHASE_EVENT);
	}
	
	public void OnItemPurchaseStarted( PurchasableVirtualItem pvi ) {
		Debug.Log( "OnItemPurchaseStarted: " + pvi.ItemId );
		s += "OnItemPurchaseStarted: " + pvi.ItemId;
	}
	
	public void OnItemPurchased( PurchasableVirtualItem pvi, string s1 ) {
		Debug.Log( "OnItemPurchased: " + pvi.ItemId + ", " + s1 );
		s += "OnItemPurchased: " + pvi.ItemId;
	}
	
	public void OnSoomlaStoreInitialized( ) {
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
