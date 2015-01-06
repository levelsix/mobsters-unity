using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSHomeScreenSecretGift : MonoBehaviour {

	[SerializeField]
	UILabel timer;

	[SerializeField]
	UISprite timerBg;

	[SerializeField]
	Animator giftAnimator;

	[SerializeField]
	MSPopup giftPopup;

	UserItemSecretGiftProto currGift;

	SpriteRenderer renderer;

	/// <summary>
	/// The time that we started counting down till the next gift is unlocked
	/// </summary>
	long startTimerTime;

	void Awake()
	{
		renderer = GetComponent<SpriteRenderer>();
	}

	void OnEnable()
	{
		if(currGift == null || currGift != MSItemManager.instance.nextRedeemGift)
		{
			startTimerTime = MSUtil.timeNowMillis;
			currGift = MSItemManager.instance.nextRedeemGift;
		}
		MSActionManager.Items.OnRedeemSeecretGift += OnRedeemGift;
	}

	void OnDisable()
	{
		MSActionManager.Items.OnRedeemSeecretGift -= OnRedeemGift;
	}

	void Update()
	{
		if(currGift != null)
		{
			renderer.enabled = true;
			long timeUntil = MSUtil.timeUntil(startTimerTime + (currGift.secsTillCollection * 1000));
			if(timeUntil > 0)
			{
				giftAnimator.SetBool("GiftAvailable", false);
				timerBg.alpha = 1f;
				timer.text = MSUtil.TimeStringShort(timeUntil);
			}
			else
			{
				timerBg.alpha = 0f;
				giftAnimator.SetBool("GiftAvailable", true);
			}
		}
		else
		{
			renderer.enabled = false;
			currGift = MSItemManager.instance.nextRedeemGift;
			startTimerTime = MSUtil.timeNowMillis;
		}
	}

	void OnRedeemGift()
	{
		currGift = MSItemManager.instance.nextRedeemGift;
		startTimerTime = MSUtil.timeNowMillis;
	}

	public void OnClick()
	{
		if(MSUtil.timeNowMillis > startTimerTime + (currGift.secsTillCollection * 1000))
		{
			giftPopup.Popup();
		}
	}
}
