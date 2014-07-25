using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using com.lvl6.proto;

public class MSQuestCompletePopup : MonoBehaviour {

	[SerializeField]
	PZPrize prizePrefab;

	[SerializeField]
	Transform prizeParent;

	[SerializeField]
	MSCheckBox checkBox;

	[SerializeField]
	TweenPosition continueButton;

	[SerializeField]
	UILabel questName;

	List<PZPrize> prizes = new List<PZPrize>();

	[SerializeField]
	int prizeSize = 110;

	[SerializeField]
	int buffer = 5;

	[SerializeField]
	TweenPosition questTween;

	[SerializeField]
	TweenPosition completeTween;

	[SerializeField]
	TweenPosition titleTween;

	[SerializeField]
	TweenScale itemZone;

	[SerializeField]
	UISprite spinner;

	bool hasQuest = false;

	void OnEnable(){
		RecyclePrizes();
		continueButton.GetComponent<MSActionButton> ().onClick += ClickContinueButton;
	}

	void OnDisable(){
		continueButton.GetComponent<MSActionButton> ().onClick -= ClickContinueButton;
	}

	[ContextMenu ("testPopUp")]
	public void InitSelf(){
		gameObject.SetActive (true);
		GetComponent<TweenAlpha>().ResetToBeginning();
		GetComponent<UITweener> ().PlayForward ();
		InitCompletedQuest(null);
	}

	void InitObjects(){
		//setup Item Zone
		Vector3 scale = itemZone.transform.localScale;
		scale.y = 0f;
		itemZone.transform.localScale = scale;

		//Quest Title Sprite
		questTween.transform.localScale = Vector3.zero;
		questTween.GetComponent<UISprite> ().alpha = 0f;
		questTween.transform.localPosition = questTween.from;

		//Complete Title Sprite
		completeTween.transform.localScale = Vector3.zero;
		completeTween.GetComponent<UISprite> ().alpha = 0f;
		completeTween.transform.localPosition = completeTween.from;

		//Quest Name setup
		titleTween.transform.localScale = Vector3.zero;
		titleTween.GetComponent<UISprite> ().alpha = 0f;
		titleTween.transform.localPosition = titleTween.from;

		//CheckBox
		checkBox.transform.localPosition = checkBox.GetComponent<TweenPosition>().from;
		checkBox.GetComponent<UISprite> ().alpha = 0f;

		//coninue button
		continueButton.transform.localPosition = continueButton.from;
		continueButton.GetComponent<UISprite> ().alpha = 0f;

		//spinner
		spinner.alpha = 0f;
	}

	public void InitCompletedQuest(MSFullQuest quest){
		gameObject.SetActive(true);
		int xp;
		int cash;
		int oil;
		int gem;
		int monsterId;
		if (quest != null) {
			 xp = quest.quest.expReward;
			 cash = quest.quest.cashReward;
			 oil = quest.quest.oilReward;
			 gem = quest.quest.gemReward;
			 monsterId = quest.quest.monsterIdReward;
			questName.text = quest.quest.name;
		} else {
			 xp = 2;
			 cash = 3;
			 oil = 0;
			 gem = 0;
			 monsterId = 0;
			questName.text = "Text Test But Butt";
		}

		Debug.Log("Prizes:\nXP: " + xp + "\nCash: " + cash + "\nOil: " + oil + "\nGems: " + gem + "\nMonster: " + monsterId);

		InitObjects ();

		PZPrize prize;
		
//		foreach (var item in pieces) 
//		{
//			prize = GetPrize ();
//			
//			prize.InitEnemy(item);
//			prizes.Add (prize);
//		}

		if (gem > 0) {
			prize = GetPrize();
			
			prize.InitDiamond(gem);
			prizes.Add(prize);
		}

		if (monsterId > 0) {
			prize = GetPrize();

			prize.InitEnemy(monsterId);
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

		hasQuest = true;

	}

	void Update()
	{
		if (hasQuest)
		{
			StartCoroutine (SlideInTitles ());
			hasQuest = false;
		}
	}

	IEnumerator SlideInTitles(){
		float waitTime = 0.2f;

		yield return new WaitForSeconds (0.5f);

		TweenAlpha alpha;
		TweenScale scale;

		//the problem with Tween.Begin is it seems to destroy the animation curve
		questTween.ResetToBeginning ();
		questTween.PlayForward ();
		alpha = questTween.GetComponent<TweenAlpha> ();
		alpha.ResetToBeginning ();
		alpha.PlayForward ();
		scale = questTween.GetComponent<TweenScale>();
		scale.ResetToBeginning ();
		scale.PlayForward();

		yield return new WaitForSeconds (waitTime);
		completeTween.ResetToBeginning ();
		completeTween.PlayForward ();
		alpha = completeTween.GetComponent<TweenAlpha> ();
		alpha.ResetToBeginning ();
		alpha.PlayForward ();
		scale = completeTween.GetComponent<TweenScale>();
		scale.ResetToBeginning ();
		scale.PlayForward();

		yield return new WaitForSeconds (waitTime);
		titleTween.ResetToBeginning ();
		titleTween.PlayForward ();
		alpha = titleTween.GetComponent<TweenAlpha> ();
		alpha.ResetToBeginning ();
		alpha.PlayForward ();
		scale = titleTween.GetComponent<TweenScale> ();
		scale.ResetToBeginning ();
		scale.PlayForward ();

		yield return new WaitForSeconds (waitTime);
		StartCoroutine (SlideInPrizes ());
	}

	IEnumerator SlideInPrizes(){
		itemZone.ResetToBeginning ();
		itemZone.PlayForward ();

		float spaceNeeded = prizes.Count * prizeSize + (prizes.Count-1) * buffer;
		for (int i = 0; i < prizes.Count; i++) 
		{
			Vector3 endPosition = new Vector3(i * (prizeSize + buffer) - spaceNeeded/2, 0, 0);
			prizes[i].transform.localPosition = endPosition;
			TweenAlpha.Begin(prizes[i].gameObject,0.1f, 1f);
		}
		yield return new WaitForSeconds (0.5f);
		StartCoroutine (SlideInButtons ());
	}

	IEnumerator SlideInButtons(){
		checkBox.GetComponent<TweenPosition> ().ResetToBeginning ();
		checkBox.GetComponent<TweenPosition> ().PlayForward ();
		checkBox.GetComponent<TweenAlpha> ().ResetToBeginning ();
		checkBox.GetComponent<TweenAlpha> ().PlayForward ();
		yield return new WaitForSeconds (0.3f);

		continueButton.ResetToBeginning ();
		continueButton.PlayForward ();
		continueButton.GetComponent<TweenAlpha> ().ResetToBeginning ();
		continueButton.GetComponent<TweenAlpha> ().PlayForward ();

		TweenAlpha.Begin (spinner.gameObject, 1f, 1f);
	}

	PZPrize GetPrize()
	{
		PZPrize prize = (MSPoolManager.instance.Get(prizePrefab.GetComponent<MSSimplePoolable>(), Vector3.zero) as MonoBehaviour).GetComponent<PZPrize>();
		prize.transform.parent = prizeParent;
		prize.transform.localScale = Vector3.one;
		Color newColor = prize.GetComponent<UISprite>().color;
		newColor.a = 0f;
		prize.GetComponent<UISprite> ().color = newColor;
		
		return prize;
	}

	void RecyclePrizes ()
	{
		foreach (var item in prizes) {
			item.GetComponent<MSSimplePoolable> ().Pool ();
		}
		prizes.Clear ();
	}

	void ClickContinueButton()
	{
		gameObject.SetActive (false);
		if (!MSQuestManager.instance.TryCompleteNextQuest())
		{
			MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.questPopup.GetComponent<MSPopup>());
		}
	}
}
