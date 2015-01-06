using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSGiftPopup : MonoBehaviour {

	[SerializeField]
	UIButton collectButton;

	[SerializeField]
	UILabel description;

	[SerializeField]
	UILabel quantity;

	[SerializeField]
	UISprite icon;

	MSLoadLock buttonLock;

	void Awake()
	{
		EventDelegate.Add( collectButton.onClick, delegate { OnClick(); });
		buttonLock = collectButton.GetComponent<MSLoadLock>();
	}

	void OnEnable()
	{
		InitCurrentGift();
	}

	void InitCurrentGift()
	{
		ItemProto curGift = MSDataManager.instance.Get<ItemProto>( MSItemManager.instance.nextRedeemGift.itemId);
		description.text = curGift.name;

		if(curGift.amount == null)
		{
			quantity.text = "";
		}
		else if(curGift.itemType == ItemType.SPEED_UP)
		{
			//gift amount is in minutes
			quantity.text = MSUtil.TimeStringShort((long)(curGift.amount * 60 * 1000));
		}
		else
		{
			quantity.text = curGift.amount.ToString();
		}

		icon.spriteName = MSUtil.StripExtensions(curGift.imgName);
	}

	void OnClick()
	{
		buttonLock.Lock();
		MSItemManager.instance.DoRedeemSecretGift(delegate{buttonLock.Unlock(); GetComponent<MSPopup>().CloseAll();});
	}
}
