using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSPvpUI
/// </summary>
public class MSPvpUI : MonoBehaviour {

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel money;

	[SerializeField]
	UILabel oil;

	[SerializeField]
	UILabel rankNumber;

	[SerializeField]
	UILabel rankSuffix;

	[SerializeField]
	UILabel rematchCost;

	[SerializeField]
	TweenPosition posTween;

	public void Init(PvpProto defender)
	{
		
		nameLabel.text = defender.defender.minUserProto.name;

		money.text = "$" + defender.prospectiveCashWinnings;

		oil.text = defender.prospectiveOilWinnings.ToString();

		rankNumber.text = defender.pvpLeagueStats.elo.ToString();

		rankSuffix.text = MSUtil.LeagueRankSuffix(defender.pvpLeagueStats.elo);

		rematchCost.text = "$" + PZCombatManager.instance.pvpMatchCost + "\nNext Match";
		
		Reset();
		transform.localPosition = new Vector3(-300f, -50f, 0f);
		gameObject.SetActive(true);
		//posTween.PlayForward();
		//MSSwoopAnimation.SwoopGroup(1,0.3f);
	}

	public void Reset()
	{
		posTween.Sample(0, false);
	}

	public void Retract()
	{
		//posTween.PlayReverse();
		if(MSSwoopAnimation.SwoopGroupOut != null)
		{
			Debug.Log("swoooops");
			MSSwoopAnimation.SwoopGroupOut(MSSwoopAnimation.SwoopIDs.PVPMENU);
		}
		StartCoroutine(DelayedDisable());
	}

	IEnumerator DelayedDisable()
	{
		yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
	}
}
