﻿using UnityEngine;
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
	UILabel rankings;

	[SerializeField]
	UILabel rematchCost;

	[SerializeField]
	TweenPosition posTween;

	public void Init(PvpProto defender)
	{

		nameLabel.text = defender.defender.minUserProto.name;

		money.text = "$" + defender.prospectiveCashWinnings;

		oil.text = defender.prospectiveOilWinnings.ToString();

		rankings.text = defender.pvpLeagueStats.elo + ". " + defender.defender.minUserProto.name;

		rematchCost.text = "$" + PZCombatManager.instance.pvpMatchCost + "\nNext Match";
		
		Reset();
		posTween.PlayForward();
	}

	public void Reset()
	{
		posTween.Sample(0, false);
	}

	public void Retract()
	{
		posTween.PlayReverse();
	}
}
