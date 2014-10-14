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

	bool didBlackOut = false;

//	[SerializeField]
//	UISprite title;
//
//	[SerializeField]
//	UISprite titlePow;

	[SerializeField]
	UISprite topTitle;

	[SerializeField]
	UISprite botTitle;

	[SerializeField]
	UISprite ribbon;

	[SerializeField]
	UILabel ribbonLabel;

	[SerializeField]
	Transform prizeParent;

	[SerializeField]
	GameObject prizeBG;

	[SerializeField]
	UISprite sticker;

	[SerializeField]
	UISprite spinner;

	[SerializeField]
	GameObject shareButton;

	[SerializeField]
	GameObject doneButton;

	[SerializeField]
	GameObject reviveButton;

	[SerializeField]
	GameObject hint;

	[SerializeField]
	PZPrize prizePrefab;

	[SerializeField]
	int prizeSize = 110;

	[SerializeField]
	int prizeSpacingBuffer = 5;

	[SerializeField]
	float animationBuffer = -0.1f;

	public UITweener tweener;
	
	List<PZPrize> prizes = new List<PZPrize>();

	[SerializeField]
	Color ribbonGreen;
	[SerializeField]
	Color ribbonRed;

	const string YOU_FOUND = "YOU FOUND";
	const string YOU_LOST = "YOU LOST";
	const string GOOD_JOB = "GOOD JOB!";

	const string GREEN_YOU = "youwonyou";
	const string GREEN_WIN = "youwonwon";

	const string RED_YOU = "youlostyou";
	const string RED_LOST = "youlostlost";

	const string GREEN_STICKER = "wonstickerhead";
	const string RED_STICKER = "loststickerhead";

	const string GREEN_RIBBON = "youfoundribbon";
	const string RED_RIBBON = "youmissoutonribbon";
	
	const float HINT_MAX_HEIGHT = 60f;

	/// <summary>
	/// initialization of positions for elements required in both win and lose animations
	/// </summary>
	void GenInit(){
		if(MSActionManager.Puzzle.OnResultScreen != null)
		{
			MSActionManager.Puzzle.OnResultScreen();
		}

		doneButton.transform.localScale = new Vector3 (0f, 0f, 0f);
		hint.transform.localPosition = hint.GetComponent<TweenPosition> ().from;
//		titlePow.transform.localPosition = titlePow.transform.GetComponent<TweenPosition> ().from;
//		title.GetComponent<TweenScale> ().enabled = false;
//		title.transform.localScale = title.GetComponent<TweenScale> ().from;

		topTitle.alpha = 0f;
		topTitle.transform.localPosition = topTitle.GetComponent<TweenPosition>().from;

		botTitle.alpha = 0f;
		botTitle.transform.localPosition = botTitle.GetComponent<TweenPosition>().from;

		ribbon.alpha = 0f;

		prizeBG.transform.localScale = prizeBG.GetComponent<TweenScale>().from;

		sticker.transform.localScale = sticker.GetComponent<TweenScale>().from;

		spinner.alpha = 0f;
	}

	[ContextMenu("Inti Lose")]
	public void InitLose()
	{
		didWin = false;
		didBlackOut = false;

		sticker.gameObject.SetActive(true);
		shareButton.SetActive(false);
		reviveButton.SetActive(true);

		reviveButton.transform.localScale = Vector3.zero;
		sticker.GetComponent<TweenRotation> ().enabled = false;
		sticker.transform.localScale = Vector3.zero;
		sticker.transform.localRotation = Quaternion.identity;
		GenInit ();

//		titlePow.spriteName = LOST_POW;
//		title.spriteName = LOST_TITLE;
		ribbonLabel.text = YOU_LOST;

		RecyclePrizes();

		StartCoroutine (SwoopInTitle ());
	}

	public void InitBlackOut(int xp, int cash, int oil, List<MonsterProto> pieces, List<ItemProto> items)
	{
		didWin = false;
		didBlackOut = true;

		sticker.gameObject.SetActive(true);
		shareButton.SetActive(false);
		reviveButton.SetActive(true);
		
		reviveButton.transform.localScale = Vector3.zero;
		sticker.GetComponent<TweenRotation> ().enabled = false;
		sticker.transform.localScale = Vector3.zero;
		sticker.transform.localRotation = Quaternion.identity;
		GenInit ();
		
//		titlePow.spriteName = LOST_POW;
//		title.spriteName = LOST_TITLE;
		ribbonLabel.text = YOU_LOST;

		InitPrizes(xp, cash, oil, pieces, items);

		foreach(PZPrize prize in prizes)
		{
			prize.SetToLostPrize();
		}

		StartCoroutine (SwoopInTitle ());
	}

	public void InitWin(int xp, int cash, int oil, List<MonsterProto> pieces, List<ItemProto> items)
	{
		didWin = true;
		didBlackOut = false;

		sticker.gameObject.SetActive(false);
		shareButton.SetActive(true);
		reviveButton.SetActive(false);

		shareButton.transform.localScale = new Vector3 (0f, 0f, 0f);
		GenInit ();

//		titlePow.spriteName = WIN_POW;
//		title.spriteName = WIN_TITLE;

		ribbonLabel.text = YOU_FOUND;

		InitPrizes(xp,cash,oil,pieces,items);

		StartCoroutine (SwoopInTitle ());

	}

	void InitPrizes(int xp, int cash, int oil, List<MonsterProto> pieces, List<ItemProto> items)
	{
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
	}

	IEnumerator SwoopInTitle(){
		yield return new WaitForSeconds (0.5f);
//		titlePow.GetComponent<TweenPosition> ().ResetToBeginning ();
//		titlePow.GetComponent<TweenPosition> ().PlayForward ();
//		yield return new WaitForSeconds (titlePow.GetComponent<TweenPosition> ().duration);

		if(didWin)
		{
			topTitle.spriteName = GREEN_YOU;
			botTitle.spriteName = GREEN_WIN;
		}
		else
		{
			topTitle.spriteName = RED_YOU;
			botTitle.spriteName = RED_LOST;
		}

		TweenPosition tweenP = topTitle.GetComponent<TweenPosition>();
		TweenAlpha tweenA = topTitle.GetComponent<TweenAlpha>();
		tweenP.ResetToBeginning();
		tweenP.PlayForward();
		tweenA.ResetToBeginning();
		tweenA.PlayForward();

		yield return new WaitForSeconds(tweenP.duration + animationBuffer);

		tweenP = botTitle.GetComponent<TweenPosition>();
		tweenA = botTitle.GetComponent<TweenAlpha>();
		tweenP.ResetToBeginning();
		tweenP.PlayForward();
		tweenA.ResetToBeginning();
		tweenA.PlayForward();

		yield return new WaitForSeconds(tweenP.duration + animationBuffer);

		StartCoroutine(UnfoldPrizeBox());
	}

	IEnumerator UnfoldPrizeBox()
	{
		if(didWin)
		{
			ribbon.spriteName = GREEN_RIBBON;
			ribbonLabel.color = ribbonGreen;
		}
		else
		{
			ribbon.spriteName = RED_RIBBON;
			ribbonLabel.color = ribbonRed;
		}

		TweenAlpha tweenA = ribbon.GetComponent<TweenAlpha>();
		tweenA.ResetToBeginning();
		tweenA.PlayForward();
		yield return new WaitForSeconds(tweenA.duration + animationBuffer);

		TweenScale tweenS = prizeBG.GetComponent<TweenScale>();
		tweenS.ResetToBeginning();
		tweenS.PlayForward();
		yield return new WaitForSeconds(tweenS.duration);

		if ((didWin || didBlackOut) && prizes.Count > 0)
		{
			StartCoroutine(SlideInPrizes());
		}
		else
		{
			StartCoroutine(ExpandSticker());
		}
	}

	IEnumerator ExpandSticker(){
		if(didWin)
		{
			sticker.spriteName = GREEN_STICKER;
		}
		else
		{
			sticker.spriteName = RED_STICKER;
		}

		TweenScale stickerScale = sticker.GetComponent<TweenScale> ();
		stickerScale.ResetToBeginning ();
		stickerScale.PlayForward ();
		yield return new WaitForSeconds (stickerScale.duration);

		TweenRotation stickerRotation = sticker.GetComponent<TweenRotation> ();
		stickerRotation.ResetToBeginning ();
		stickerRotation.PlayForward ();
		StartCoroutine (ZoomButtons ());
	}

	IEnumerator SlideInPrizes(){
		float spaceNeeded = prizes.Count * prizeSize + ((prizes.Count-1) * prizeSpacingBuffer);
		for (int i = 0; i < prizes.Count; i++)
		{
			Vector3 endPosition = new Vector3(((i) * (prizeSize + prizeSpacingBuffer) + (prizeSize/2f)) - spaceNeeded/2f, 0, 0);
			prizes[i].SlideIn(endPosition);
			yield return new WaitForSeconds(0.2f);
			//prizes[i].GetComponent<TweenPosition>().to = new Vector3(i * (prizeSize + buffer) - spaceNeeded/2, 0, 0);
		}
//		yield return new WaitForSeconds (0.5f);
		StartCoroutine (ZoomButtons ());
	}

	[ContextMenu("math")]
	public void Math()
	{
		int numberOfPrizes = 1;
		float newPrizeSize = 100f;
		float buffer = 5f;

		float spaceNeeded = numberOfPrizes * newPrizeSize + ((numberOfPrizes-1) * buffer);
		Debug.Log(numberOfPrizes + " * " + newPrizeSize + " + ((" + (numberOfPrizes-1) + ") * " + buffer + ") = " + spaceNeeded);
		for (int i = 0; i < numberOfPrizes; i++)
		{
			Vector3 endPosition = new Vector3(((i) * (newPrizeSize + buffer) + (newPrizeSize/2f)) - spaceNeeded/2f, 0, 0);
			Debug.Log("(("+ i + ") * (" + newPrizeSize + " + " + buffer + ") + (" + newPrizeSize + "/2f)) - " + spaceNeeded + "/2f = " + endPosition.x);
		}
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

		hint.GetComponent<TweenPosition> ().to = new Vector3(hint.transform.localPosition.x,
		                                                     hint.transform.localPosition.y + hint.GetComponent<UILabel>().height + (HINT_MAX_HEIGHT - hint.GetComponent<UILabel>().height) / 2f,
		                                                     hint.transform.localPosition.z);
		hint.GetComponent<TweenPosition> ().ResetToBeginning ();
		hint.GetComponent<TweenPosition> ().PlayForward ();
		yield return new WaitForSeconds(0.2f);

//		title.GetComponent<TweenScale> ().ResetToBeginning ();
//		title.GetComponent<TweenScale> ().PlayForward ();

		spinner.GetComponent<TweenAlpha>().ResetToBeginning();
		spinner.GetComponent<TweenAlpha>().PlayForward();
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
		PZPrize prize = (MSPoolManager.instance.Get(prizePrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, prizeParent) as MonoBehaviour).GetComponent<PZPrize>();
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