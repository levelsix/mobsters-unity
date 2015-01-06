using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSItemManager : MonoBehaviour
{

	public Dictionary<int, UserItemProto> currUserItems = new Dictionary<int, UserItemProto> ();
	public List<UserItemUsageProto> itemsInUse = new List<UserItemUsageProto> ();

	//my current understanding is that this is a list of the next X gifts that will be in the secret gift box
	//updated everytime we redeem a gift
	List<UserItemSecretGiftProto> secretGifts = new List<UserItemSecretGiftProto>();

	public static MSItemManager instance;

	public UserItemSecretGiftProto nextRedeemGift
	{
		get
		{
			return secretGifts.Count > 0 ? secretGifts[0] : null;
		}
	}

	void Awake()
	{
		instance = this;
	}

	void OnEnable ()
	{
		MSActionManager.Loading.OnStartup += OnStartup;
	}

	void OnDisable ()
	{
		MSActionManager.Loading.OnStartup -= OnStartup;
	}

	void OnStartup (StartupResponseProto proto)
	{
		foreach (var item in proto.userItems)
		{
			currUserItems.Add (item.itemId, item);
		}

		itemsInUse = proto.itemsInUse;
		secretGifts = proto.gifts;
	}

	public void AddItem(int itemId, int quantity)
	{
		if(currUserItems.ContainsKey(itemId))
		{
			currUserItems[itemId].quantity += quantity;
		}
		else
		{
			UserItemProto newItem = new UserItemProto();
			newItem.itemId = itemId;
			newItem.quantity = quantity;
			newItem.userUuid = MSWhiteboard.localMup.userUuid;
			currUserItems.Add(itemId, newItem);
		}
	}

	public UserItemProto GetUserItem(int itemId)
	{
		if(currUserItems.ContainsKey(itemId))
		{
			return currUserItems[itemId];
		}
		else
		{
			return null;
		}
	}

	//------------------------------------------------------------------------------------------
	public void DoTradeForBoosters (UserItemProto item, Action OnComplete = null)
	{
		StartCoroutine(TradeItemForBooster(item, OnComplete));
	}

	IEnumerator TradeItemForBooster (UserItemProto item, Action OnComplete)
	{
		TradeItemForBoosterRequestProto request = new TradeItemForBoosterRequestProto ();
		request.sender = MSWhiteboard.localMup;
		request.clientTime = MSUtil.timeNowMillis;
		request.itemId = item.itemId;

		int tagNum = UMQNetworkManager.instance.SendRequest (request, (int)EventProtocolRequest.C_TRADE_ITEM_FOR_BOOSTER_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		TradeItemForBoosterResponseProto response = UMQNetworkManager.responseDict [tagNum] as TradeItemForBoosterResponseProto;
		UMQNetworkManager.responseDict.Remove (tagNum);

		if (response.status != TradeItemForBoosterResponseProto.TradeItemForBoosterStatus.SUCCESS)
		{
			Debug.LogError ("Error trading item for booster: " + response.status.ToString ());
		}
		else
		{
			if (OnComplete != null)
			{
				OnComplete ();
			}
		}
	}
	
	//------------------------------------------------------------------------------------------
//	TradeItemForSpeedUps

	public void DoTradeItemsForSpeedUps(UserItemProto item, MSTimer timer, Action OnComplete)
	{
		List<UserItemProto> items = new List<UserItemProto>();
		items.Add(item);
		StartCoroutine(TradeItemsForSpeedUps(items, timer, OnComplete));
	}
	
	IEnumerator TradeItemsForSpeedUps(List<UserItemProto> items, MSTimer timer, Action OnComplete)
	{
		Dictionary<int, UserItemProto> tempUserItems = currUserItems;
		TradeItemForSpeedUpsRequestProto request = new TradeItemForSpeedUpsRequestProto();
		request.sender = MSWhiteboard.localMup;
		foreach (var item in items)
		{
			tempUserItems[item.itemId].quantity--;
			request.nuUserItems.Add(tempUserItems[item.itemId]);
		}

		foreach(var item in items)
		{
			UserItemUsageProto newUsageItem = new UserItemUsageProto();
			newUsageItem.actionType = timer.timerType;
			newUsageItem.itemId = item.itemId;
			newUsageItem.timeOfEntry = MSUtil.timeNowMillis;
			newUsageItem.userUuid = MSWhiteboard.localMup.userUuid;
			newUsageItem.userDataUuid = "";//TODO: ????????

			request.itemsUsed.Add(newUsageItem);
		}

		int tagNum = UMQNetworkManager.instance.SendRequest (request, (int)EventProtocolRequest.C_TRADE_ITEM_FOR_SPEED_UPS_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		TradeItemForSpeedUpsResponseProto response = UMQNetworkManager.responseDict [tagNum] as TradeItemForSpeedUpsResponseProto;
		UMQNetworkManager.responseDict.Remove (tagNum);

		if(response.status != TradeItemForSpeedUpsResponseProto.TradeItemForSpeedUpsStatus.SUCCESS)
		{
			Debug.LogError("Trading item for speed ups failed : " + response.status);
		}
		else
		{
			currUserItems = tempUserItems;
			//TODO: add logic for adding a speedup to MSTimer
		}

		if(OnComplete != null)
		{
			OnComplete();
		}
	}
	
	//------------------------------------------------------------------------------------------
//		RemoveUserItemUsed
	public void DoRemoveUserItemUsed()
	{

	}

	//fuckin I don't even know right now
	IEnumerator RemoveUserItemUsed(List<UserItemUsageProto> items)
	{
		RemoveUserItemUsedRequestProto request = new RemoveUserItemUsedRequestProto();
		request.sender = MSWhiteboard.localMup;
		foreach(var item in items)
		{
			request.userItemUsedUuid.Add(item.usageUuid);
		}

		int tagNum = UMQNetworkManager.instance.SendRequest (request, (int)EventProtocolRequest.C_REMOVE_USER_ITEM_USED_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		RemoveUserItemUsedResponseProto response = UMQNetworkManager.responseDict [tagNum] as RemoveUserItemUsedResponseProto;
		UMQNetworkManager.responseDict.Remove (tagNum);

		if( response.status != RemoveUserItemUsedResponseProto.RemoveUserItemUsedStatus.SUCCESS)
		{
			Debug.LogError("There was an error removing a used user item : " + response.status.ToString());
		}
	}
	
	//------------------------------------------------------------------------------------------

	public void DoTradeItemsForResource (UserItemProto item, Action OnComplete = null)
	{
		List<UserItemProto> items = new List<UserItemProto>();
		items.Add(item);
		StartCoroutine (TradeItemsForResource (items, OnComplete));
	}

	public void DoTradeItemsForResource (List<UserItemProto> items, Action OnComplete = null)
	{
		StartCoroutine (TradeItemsForResource (items, OnComplete));
	}
		               
	IEnumerator TradeItemsForResource (List<UserItemProto> items, Action OnComplete)
	{
		Dictionary<int, UserItemProto> tempUserItems = currUserItems;
		TradeItemForResourcesRequestProto request = new TradeItemForResourcesRequestProto ();
		request.sender = MSWhiteboard.localMupWithResources;
		List<int> itemIds = new List<int> ();
		foreach (var item in items)
		{
			if (!request.itemIdsUsed.Contains (item.itemId))
			{
				request.itemIdsUsed.Add (item.itemId);
			}
	
			tempUserItems [item.itemId].quantity--;
			request.nuUserItems.Add(tempUserItems [item.itemId]);
		}
		foreach (UserItemProto item in tempUserItems.Values)
		{
			request.nuUserItems.Add (item);
		}
		request.clientTime = MSUtil.timeNowMillis;
	
		int tagNum = UMQNetworkManager.instance.SendRequest (request, (int)EventProtocolRequest.C_TRADE_ITEM_FOR_RESOURCES_EVENT, null);
	
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
	
		TradeItemForResourcesResponseProto response = UMQNetworkManager.responseDict [tagNum] as TradeItemForResourcesResponseProto;
		UMQNetworkManager.responseDict.Remove (tagNum);
	
		if (response.status != TradeItemForResourcesResponseProto.TradeItemForResourcesStatus.SUCCESS)
		{
			Debug.LogError ("Error trading Item for resource: " + response.status.ToString());
		}
		else 
		{
			currUserItems = tempUserItems;
		}
	
		if (OnComplete != null)
		{
			OnComplete ();
		}
		
	}
	//------------------------------------------------------------------------------------------
	
//		RedeemSecretGift
	public void DoRedeemSecretGift(Action OnComplete = null)
	{
		if(secretGifts.Count > 0)
		{
			StartCoroutine(RedeemSecretGift(new List<UserItemSecretGiftProto>{nextRedeemGift}, OnComplete));
		}
		else
		{
			Debug.LogError("There are no secret gifts to give!");
		}
	}

	/// <summary>
	/// Redeems the secret gift.
	/// </summary>
	/// <returns>The secret gift.</returns>
	/// <param name="secretGifts">gifts being redeemed.</param>
	/// <param name="OnComplete">On complete.</param>
	IEnumerator RedeemSecretGift(List<UserItemSecretGiftProto> secretGifts, Action OnComplete)
	{
		RedeemSecretGiftRequestProto request = new RedeemSecretGiftRequestProto();
		request.clientTime = MSUtil.timeNowMillis;
		request.sender = MSWhiteboard.localMup;
		foreach(var gift in secretGifts)
		{
			request.uisgUuid.Add(gift.uisgUuid);
		}

		int tagNum = UMQNetworkManager.instance.SendRequest (request, (int)EventProtocolRequest.C_REDEEM_SECRET_GIFT_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		RedeemSecretGiftResponseProto response = UMQNetworkManager.responseDict [tagNum] as RedeemSecretGiftResponseProto;
		UMQNetworkManager.responseDict.Remove (tagNum);

		if(response.status != RedeemSecretGiftResponseProto.RedeemSecretGiftStatus.SUCCESS)
		{
			Debug.LogError("Error redeeming secret gift: " + response.status);
		}
		else
		{
			foreach(var item in secretGifts)
			{
				AddItem(item.itemId, 1);
			}

			secretGifts = response.nuGifts;

			if(MSActionManager.Items.OnRedeemSeecretGift != null)
			{
				MSActionManager.Items.OnRedeemSeecretGift();
			}

			if(OnComplete != null)
			{
				OnComplete();
			}
		}
	}
	//------------------------------------------------------------------------------------------
}
