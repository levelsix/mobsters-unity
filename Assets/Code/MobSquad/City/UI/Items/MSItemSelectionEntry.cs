using UnityEngine;
using System;
using System.Collections;
using com.lvl6.proto;

[RequireComponent (typeof(MSSimplePoolable))]
public class MSItemSelectionEntry : MonoBehaviour {

	[SerializeField]
	UILabel amount;

	[SerializeField]
	UISprite icon;

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel quantity;

	[SerializeField]
	UISprite buttonSprite;

	[SerializeField]
	UILabel buttonLabel;

	[SerializeField]
	UIButton button;

	MSLoadLock loadLock;

	/// <summary>
	/// The Item this entry is displaying
	/// </summary>
	ItemProto currItem;

	/// <summary>
	/// The current amount of time left
	/// </summary>
	MSTimer currTimer;

	bool canBeFree = false;
	ResourceType _resourceType;

	[SerializeField] Color oilTextColor;
	[SerializeField] Color cashTextColor;
	[SerializeField] Color itemTextColor;

	readonly Vector3 CASH_SCALE = new Vector3 (0.5f, 0.5f, 1f);
	readonly Vector3 OIL_SCALE = new Vector3 (0.8f, 0.8f, 1f);
	readonly Vector3 ITEM_SCALE = new Vector3 (1f, 1f, 1f);

	const string DIAMOND_SPRITE = "diamond";

	const string PURPLE_BUTTON = "purpleitemsbutton";
	const string GREEN_BUTTON = "greenitemsbutton";
	const string YELLOW_BUTTON = "yellowitemsbutton";
	const string GREY_BUTTON = "greyitemsbutton";

	void Awake()
	{
		loadLock = button.GetComponent<MSLoadLock>();
	}

	/// <summary>
	/// Inits the gem entry
	/// </summary>
	/// <param name="gems">The number of gems required to accomplish whatever task is happening.</param>
	/// <param name="buttonAction">Button action.</param>
	public void InitGem(MSTimer timer, bool canBeFree, Action buttonAction)
	{
		currTimer = timer;
		this.canBeFree = canBeFree;

		int gems = MSMath.GemsForTime(timer.timeLeft, canBeFree);
		amount.alpha = 0f;
		nameLabel.text = "Gems";
		quantity.text = MSResourceManager.resources[ResourceType.GEMS].ToString();

		//button label is set in UpdateGemAmount()
		icon.spriteName = DIAMOND_SPRITE;

		buttonSprite.spriteName = PURPLE_BUTTON;

		gameObject.name = "0";

		button.onClick.Clear();
		EventDelegate.Add(button.onClick, delegate{buttonAction();});
		StartCoroutine(UpdateGemAmount());
	}

	public void InitGem(ResourceType resourceType, int gems, Action buttonAction)
	{
		currTimer = null;
		canBeFree = false;
		_resourceType = resourceType;

		amount.alpha = 0f;
		nameLabel.text = "Gems";
		quantity.text = MSResourceManager.resources[ResourceType.GEMS].ToString();
		
		icon.spriteName = DIAMOND_SPRITE;
		
		buttonSprite.spriteName = PURPLE_BUTTON;
		
		gameObject.name = "0";

		buttonLabel.text = "(G) " + gems;
		buttonSprite.spriteName = PURPLE_BUTTON;

		button.onClick.Clear();
		EventDelegate.Add(button.onClick, delegate{buttonAction();});
	}

	IEnumerator UpdateGemAmount()
	{
		while(!currTimer.done)
		{
			int gems = MSMath.GemsForTime(currTimer.timeLeft, canBeFree);

			if(gems == 0)
			{
				buttonLabel.text = "FREE";
			}
			else
			{
				buttonLabel.text = "(G) " + gems;
			}

			yield return new WaitForEndOfFrame();
		}
	}

	public void InitItem(int itemId, Action buttonAction, UIScrollView view)
	{
		ItemProto item = MSDataManager.instance.Get<ItemProto>(itemId);
		InitItem(item, buttonAction, view);
	}

	public void InitItem(ItemProto item, Action buttonAction, UIScrollView view)
	{
		currItem = item;

		UserItemProto userItem;

		GetComponent<UIDragScrollView>().scrollView = view;

		userItem = MSItemManager.instance.GetUserItem(item.itemId);

		icon.spriteName = MSUtil.StripExtensions(item.imgName);

		nameLabel.text = item.name;

		amount.alpha = 1f;
		if(item.itemType == ItemType.ITEM_CASH)
		{
			amount.color = cashTextColor;
			amount.text = MSUtil.FormatNumber(item.amount);
			icon.transform.localScale = CASH_SCALE;
		}
		else if(item.itemType == ItemType.ITEM_OIL)
		{
			amount.color = oilTextColor;
			amount.text = MSUtil.FormatNumber(item.amount);
			icon.transform.localScale = OIL_SCALE;
		}
		else
		{
			amount.color = itemTextColor;
			amount.text = MSUtil.TimeStringShort((long)item.amount * 60 * 1000);
			icon.transform.localScale = ITEM_SCALE;
		}

		button.onClick.Clear();

		if(userItem != null && userItem.quantity > 0)
		{
			buttonSprite.spriteName = GREEN_BUTTON;
			quantity.text = userItem.quantity.ToString();
			EventDelegate.Add(button.onClick, delegate{buttonAction();});
		}
		else
		{
			buttonSprite.spriteName = GREY_BUTTON;
			quantity.text = "0";
			//TODO: other grey options
		}

		icon.MakePixelPerfect();
		gameObject.name = item.itemId.ToString();

	}
	public void SpeedUpWithGemsOnClick()
	{
		currTimer.gemsUsed = currTimer.gemsNeededToComplete;
		//TODO: popup close?
	}

	public void SpeedUpOnClick()
	{
		Debug.Log("clicked on item button : " + currItem.name);
		UserItemProto userItem = MSItemManager.instance.GetUserItem(currItem.itemId);
		if(userItem != null)
		{
			loadLock.Lock();
			MSItemManager.instance.DoTradeItemsForSpeedUps(userItem, currTimer, loadLock.Unlock);
		}
		else
		{
			Debug.LogError("This item usages button should be disabled", this);
		}
		//TODO: popup close when done?
	}

	public void FillResourceWithGemsOnClick(int amount)
	{
		loadLock.Lock();
		MSResourceManager.instance.SpendGemsForOtherResource(_resourceType, amount, loadLock.Unlock);
		//TODO: popup close when done?
	}

	public void AddResourceOnClick()
	{
			if((_resourceType == ResourceType.CASH && currItem.itemType == ItemType.ITEM_CASH) || (_resourceType == ResourceType.OIL && currItem.itemType == ItemType.ITEM_OIL))
		{
			loadLock.Lock();
			MSItemManager.instance.DoTradeItemsForResource(MSItemManager.instance.GetUserItem(currItem.itemId), loadLock.Unlock);
		}
		else
		{
			Debug.LogError("resource type mismatch", this);
		}
		//TODO: popup close when done?
	}

	/// <summary>
	/// Generic actions that need to happen after we get a response from the server
	/// not implemented
	/// </summary>
	void OnComplete()
	{
		//or if the timer is done close the popup
		//TODO:re init this object
		loadLock.Unlock();
	}

}
