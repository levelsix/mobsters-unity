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
	GameObject reviveButton;

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

	public void InitLose()
	{
		lostSticker.SetActive(true);
		shareButton.SetActive(false);
		reviveButton.SetActive(true);

		titlePow.spriteName = LOST_POW;
		title.spriteName = LOST_TITLE;

		RecyclePrizes();
	}

	public void InitWin(int xp, int cash, List<MonsterProto> pieces)
	{
		lostSticker.SetActive(false);
		shareButton.SetActive(true);
		reviveButton.SetActive(false);
		
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
		
		if (cash > 0)
		{
			prize = GetPrize();
			prize.InitCash(cash);
			prizes.Add (prize);
		}

		if (xp > 0)
		{
			prize = GetPrize();
			prize.InitXP(xp);
			prizes.Add (prize);
		}

		float spaceNeeded = prizes.Count * prizeSize + (prizes.Count-1) * buffer;
		for (int i = 0; i < prizes.Count; i++) 
		{
			prizes[i].transform.localPosition = new Vector3(i * (prizeSize + buffer) - spaceNeeded/2, 0, 0);
		}

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
		return prize;
	}

	public void OnDoneButton()
	{
		MSActionManager.Scene.OnCity();
	}

	public void OnManageButton()
	{
		MSActionManager.Scene.OnCity();
		//Then do managerial bullshit
	}
}
