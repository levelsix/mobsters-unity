using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// PZWinLosePopup
/// </summary>
public class PZWinLosePopup : MonoBehaviour {

	bool didWin;

	[SerializeField]
	UISprite title;

	[SerializeField]
	UISprite titlePow;

	[SerializeField]
	Transform prizeParent;

	[SerializeField]
	GameObject lostSticker;

	[SerializeField]
	GameObject shareButton;

	[SerializeField]
	GameObject doneButton;

	[SerializeField]
	GameObject reviveButton;

	[SerializeField]
	GameObject manageButton;

	[SerializeField]
	PZPrize prizePrefab;

	[SerializeField]
	int prizeSize = 110;

	[SerializeField]
	int buffer = 5;

	public UITweener tweener;
	
	List<PZPrize> prizes = new List<PZPrize>();

	const string WIN_TITLE = "youwon";
	const string LOST_TITLE = "youlost";
	const string WIN_POW = "wonsplash";
	const string LOST_POW = "lostsplash";

	/// <summary>
	/// initialization of positions for elements required in both win and lose animations
	/// </summary>
	void genInit(){
		doneButton.transform.localScale = new Vector3 (0f, 0f, 0f);
		manageButton.transform.localPosition = manageButton.GetComponent<TweenPosition> ().from;
		titlePow.transform.localPosition = titlePow.transform.GetComponent<TweenPosition> ().from;
		title.GetComponent<TweenScale> ().enabled = false;
		title.transform.localScale = title.GetComponent<TweenScale> ().from;
	}

	public void InitLose()
	{
		didWin = false;

		lostSticker.SetActive(true);
		shareButton.SetActive(false);
		reviveButton.SetActive(true);

		reviveButton.transform.localScale = Vector3.zero;
		lostSticker.GetComponent<TweenRotation> ().enabled = false;
		lostSticker.transform.localScale = Vector3.zero;
		lostSticker.transform.localRotation = Quaternion.identity;
		genInit ();

		titlePow.spriteName = LOST_POW;
		title.spriteName = LOST_TITLE;

		RecyclePrizes();

		StartCoroutine (DropTheTitle ());
	}

	public void InitWin(int xp, int cash, int oil, List<MonsterProto> pieces, List<ItemProto> items)
	{
		didWin = true;

		lostSticker.SetActive(false);
		shareButton.SetActive(true);
		reviveButton.SetActive(false);

		shareButton.transform.localScale = new Vector3 (0f, 0f, 0f);
		genInit ();

		titlePow.spriteName = WIN_POW;
		title.spriteName = WIN_TITLE;

		RecyclePrizes();

		PZPrize prize;
		
		foreach (var item in pieces)
		{
			prize = GetPrize ();

			prize.InitEnemy(item);
			prizes.Add (prize);
		}

		foreach (ItemProto item in items)
		{
			prize = GetPrize();

			prize.InitItem(item);
			prizes.Add(prize);
		}
		
		if (cash > 0)
		{
			prize = GetPrize();
			prize.InitCash(cash);
			prizes.Add(prize);
		}

		if (oil > 0)
		{
			prize = GetPrize();
			prize.InitOil(oil);
			prizes.Add(prize);
		}

		if (xp > 0)
		{
			prize = GetPrize();
			prize.InitXP(xp);
			prizes.Add (prize);
		}
		StartCoroutine (DropTheTitle ());

	}

	IEnumerator DropTheTitle(){
		yield return new WaitForSeconds (0.5f);
		titlePow.GetComponent<TweenPosition> ().ResetToBeginning ();
		titlePow.GetComponent<TweenPosition> ().PlayForward ();
		yield return new WaitForSeconds (titlePow.GetComponent<TweenPosition> ().duration);
		if (didWin) {
			StartCoroutine (SlideInPrizes ());
		} else {
			StartCoroutine (ExpandDevil());
		}
	}

	IEnumerator ExpandDevil(){
		TweenScale stickerScale = lostSticker.GetComponent<TweenScale> ();
		stickerScale.ResetToBeginning ();
		stickerScale.PlayForward ();
		yield return new WaitForSeconds (stickerScale.duration);

		TweenRotation stickerRotation = lostSticker.GetComponent<TweenRotation> ();
		stickerRotation.ResetToBeginning ();
		stickerRotation.PlayForward ();
		StartCoroutine (ZoomButtons ());
	}

	IEnumerator SlideInPrizes(){
		float spaceNeeded = prizes.Count * prizeSize + (prizes.Count-1) * buffer;
		for (int i = 0; i < prizes.Count; i++) 
		{
			Vector3 endPosition = new Vector3(i * (prizeSize + buffer) - spaceNeeded/2, 0, 0);
			prizes[i].SlideIn(endPosition);
			yield return new WaitForSeconds(0.5f);
			//prizes[i].GetComponent<TweenPosition>().to = new Vector3(i * (prizeSize + buffer) - spaceNeeded/2, 0, 0);
		}
		yield return new WaitForSeconds (0.5f);
		StartCoroutine (ZoomButtons ());
	}

	IEnumerator ZoomButtons(){
		if (didWin) {
			TweenScale shareScale = shareButton.GetComponent<TweenScale> ();
			shareScale.ResetToBeginning ();
			shareScale.PlayForward ();
		} else {
			TweenScale reviveScale = reviveButton.GetComponent<TweenScale>();
			reviveScale.ResetToBeginning ();
			reviveScale.PlayForward ();
		}
		yield return new WaitForSeconds(0.2f);

		TweenScale doneScale = doneButton.GetComponent<TweenScale> ();
		doneScale.ResetToBeginning ();
		doneScale.PlayForward ();
		yield return new WaitForSeconds(0.2f);

		manageButton.GetComponent<TweenPosition> ().ResetToBeginning ();
		manageButton.GetComponent<TweenPosition> ().PlayForward ();
		yield return new WaitForSeconds(0.2f);

		title.GetComponent<TweenScale> ().ResetToBeginning ();
		title.GetComponent<TweenScale> ().PlayForward ();
	}

	void RecyclePrizes ()
	{
		foreach (var item in prizes) {
			item.GetComponent<MSSimplePoolable> ().Pool ();
		}
		prizes.Clear ();
	}

	PZPrize GetPrize()
	{
		PZPrize prize = (MSPoolManager.instance.Get(prizePrefab.GetComponent<MSSimplePoolable>(), Vector3.zero) as MonoBehaviour).GetComponent<PZPrize>();
		prize.transform.parent = prizeParent;
		prize.transform.localScale = Vector3.one;
		Color newColor = prize.sprite.color;
		newColor.a = 0f;
		prize.sprite.color = newColor;

		return prize;
	}

	public void OnDoneButton()
	{
		MSWhiteboard.currSceneType = MSWhiteboard.SceneType.CITY;
		MSActionManager.Scene.OnCity();
	}

	public void OnManageButton()
	{
		MSWhiteboard.currSceneType = MSWhiteboard.SceneType.CITY;
		MSActionManager.Scene.OnCity();
	}
}
