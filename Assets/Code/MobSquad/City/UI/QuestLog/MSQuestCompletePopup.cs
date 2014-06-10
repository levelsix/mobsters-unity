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
	MSCheckBox CheckBox;

	[SerializeField]
	UILabel questName;

	List<PZPrize> prizes = new List<PZPrize>();

	[SerializeField]
	int prizeSize = 110;

	[SerializeField]
	int buffer = 5;

	void OnDisable(){
		RecyclePrizes();
	}

	[ContextMenu ("testPopUp")]
	public void InitSelf(){
		gameObject.SetActive (true);
		GetComponent<TweenAlpha>().ResetToBeginning();
		GetComponent<TweenAlpha> ().PlayForward ();
		InitCompletedQuest(MSQuestManager.instance.completeQuests.Peek());
	}

	public void InitCompletedQuest(MSFullQuest quest){
		int xp = quest.quest.expReward;
		int cash = quest.quest.cashReward;
		int oil = quest.quest.oilReward;
		int gem = quest.quest.gemReward;
		int monsterId = quest.quest.monsterIdReward;

		questName.text = quest.quest.name;

		PZPrize prize;
		
//		foreach (var item in pieces) 
//		{
//			prize = GetPrize ();
//			
//			prize.InitEnemy(item);
//			prizes.Add (prize);
//			prize.border.alpha = 0f;
//		}

		if (gem > 0) {
			prize = GetPrize();
			
			prize.InitDiamond(gem);
			prizes.Add(prize);
			prize.border.alpha = 0f;
		}

		if (monsterId > 0) {
			prize = GetPrize();

			prize.InitEnemy(monsterId);
			prizes.Add(prize);
			prize.border.alpha = 0f;
		}
		
		if (cash > 0)
		{
			prize = GetPrize();
			prize.InitCash(cash);
			prizes.Add(prize);
			prize.border.alpha = 0f;
		}
		
		if (oil > 0)
		{
			prize = GetPrize();
			prize.InitOil(oil);
			prizes.Add(prize);
			prize.border.alpha = 0f;
		}
		
		if (xp > 0)
		{
			prize = GetPrize();
			prize.InitXP(xp);
			prizes.Add (prize);
			prize.border.alpha = 0f;
		}

		StartCoroutine (SlideInPrizes ());
	}

	IEnumerator SlideInPrizes(){
		yield return new WaitForSeconds (1f);
		float spaceNeeded = prizes.Count * prizeSize + (prizes.Count-1) * buffer;
		for (int i = 0; i < prizes.Count; i++) 
		{
			Vector3 endPosition = new Vector3(i * (prizeSize + buffer) - spaceNeeded/2, 0, 0);
			prizes[i].SlideIn(endPosition);
			yield return new WaitForSeconds(0.5f);
			//prizes[i].GetComponent<TweenPosition>().to = new Vector3(i * (prizeSize + buffer) - spaceNeeded/2, 0, 0);
		}
		yield return new WaitForSeconds (0.5f);
		//StartCoroutine (ZoomButtons ());
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

	void RecyclePrizes ()
	{
		foreach (var item in prizes) {
			item.GetComponent<MSSimplePoolable> ().Pool ();
		}
		prizes.Clear ();
	}	
}
