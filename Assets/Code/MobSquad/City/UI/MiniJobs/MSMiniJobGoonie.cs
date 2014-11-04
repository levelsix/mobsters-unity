using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSMiniJobGoonie
/// </summary>
public class MSMiniJobGoonie : MonoBehaviour {

	public PZMonster goonie;

	[SerializeField]
	MSMiniJobGoonPortrait portraitPrefab;

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel levelLabel;

	[SerializeField]
	UILabel hpLabel;

	[SerializeField]
	MSFillBar hpBar;

	[SerializeField]
	UILabel atkLabel;

	[SerializeField]
	MSFillBar atkBar;

	[SerializeField]
	Transform portraitParent;

	static int nextId = 0;

	public int id;

	MSUIHelper rootHelper;

	MSMiniJobGoonPortrait portrait;

	MSMiniJobPopup popup;

	void Awake()
	{
		rootHelper = GetComponent<MSUIHelper>();
		id = nextId++;
	}

	/// <summary>
	/// Init with the specified monster, reqHp and reqAtk.
	/// </summary>
	/// <param name="monster">Monster.</param>
	/// <param name="reqHp">Req hp. Float for fill bar math.</param>
	/// <param name="reqAtk">Req atk. Float for fill bar math.</param>
	public void Init(PZMonster monster, float reqHp, float reqAtk, MSMiniJobPopup popup)
	{

		rootHelper.ResetAlpha(true);

		this.goonie = monster;
		this.popup = popup;
		GetPortrait(monster);
		portrait.minusButton.TurnOff();
		nameLabel.text = monster.monster.displayName;
		levelLabel.text = "LVL. " + monster.userMonster.currentLvl;

		hpLabel.text = "HP: " + monster.currHP;
		hpBar.fill = monster.currHP / reqHp;

		atkLabel.text = "ATTACK: " + Mathf.FloorToInt(monster.totalDamage);
		atkBar.fill = monster.totalDamage / reqAtk;
	}

	public void Init(MSMiniJobGoonPortrait portrait, float reqHp, float reqAtk, MSMiniJobPopup popup)
	{
		rootHelper.ResetAlpha(true);

		this.portrait = portrait;
		
		goonie = portrait.monster;
		this.popup = popup;
		nameLabel.text = goonie.monster.displayName;
		levelLabel.text = "LVL. " + goonie.userMonster.currentLvl;
		
		hpLabel.text = "HP: " + goonie.currHP;
		hpBar.fill = goonie.currHP / reqHp;
		
		atkLabel.text = "ATTACK: " + Mathf.FloorToInt(goonie.totalDamage);
		atkBar.fill = goonie.totalDamage / reqAtk;

		StartCoroutine(TweenInPortrait());
	}

	IEnumerator TweenInPortrait()
	{
//		yield return null;
		portrait.transform.parent = portraitParent;

//		TweenPosition tp = TweenPosition.Begin(portrait.gameObject, .3f, Vector3.zero);
//		while (tp.tweenFactor < 1) yield return null;
//		portrait.ResetPanel();
		
		SpringPosition spring = SpringPosition.Begin(portrait.gameObject, Vector3.zero, 15f);
		spring.onFinished += delegate {portrait.ResetPanel();};
		yield return null;
	}

	void GetPortrait(PZMonster monster)
	{
		portrait = (MSPoolManager.instance.Get(portraitPrefab.GetComponent<MSSimplePoolable>(),
		                                      Vector3.zero,
		                                       portraitParent) as MSSimplePoolable).GetComponent<MSMiniJobGoonPortrait>();
		portrait.transform.localPosition = Vector3.zero;
		portrait.transform.localScale = Vector3.one;
		portrait.ResetPanel();
		portrait.Init(monster);
	}

	void OnClick()
	{
		if (popup.TryPickMonster(goonie, portrait))
		{
			portrait.minusButton.FadeIn();
			portrait = null;
			transform.parent = transform.parent.parent;
			rootHelper.FadeOutAndPool();
			popup.goonGrid.Reposition();
			popup.goonEntries.Remove(this);
		}
	}

	public void Pool()
	{
		if (portrait != null) portrait.Pool();
		GetComponent<MSSimplePoolable>().Pool();
	}
}
