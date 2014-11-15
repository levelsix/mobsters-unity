using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSRaidTeamPopup
/// </summary>
public class MSRaidTeamPopup : MonoBehaviour {

	[SerializeField] MSMiniGoonBox[] team;

	[SerializeField] UILabel header;

	[SerializeField] UILabel body;

	const string SET_HEADER = "Enter Raid with Current Team";
	const string RESET_HEADER = "Switch to Raid Team";

	const string SET_BODY = "Would you like to enter this raid with your current team? You cannot change this team once you have entered the raid.";
	const string RESET_BODY = "You can only enter this raid with the following toons that you originally used. Would you like to enter now?";

	bool setMode = false;

	public void InitSetCurrentTeam()
	{
		setMode = true;
		header.text = SET_HEADER;
		body.text = SET_BODY;

		for (int i = 0; i < team.Length; i++)
		{
			team[i].Init(MSMonsterManager.instance.userTeam[i]);
			if (MSMonsterManager.instance.userTeam[i] != null && MSMonsterManager.instance.userTeam[i].userMonster != null && !MSMonsterManager.instance.userTeam[i].userMonster.userMonsterUuid.Equals(""))
			{
				team[i].label.text = "lvl " + MSMonsterManager.instance.userTeam[i].userMonster.currentLvl;
			}
			else
			{
				team[i].label.text = " ";
			}
		}
	}

	public void InitSwitchToRaidTeam()
	{
		setMode = false;
		header.text = RESET_HEADER;
		body.text = RESET_BODY;

		for (int i = 0; i < team.Length; i++) 
		{
			if (MSClanEventManager.instance.myTeam.currentTeam.Count > i)
			{
				PZMonster mon = MSMonsterManager.instance.userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(MSClanEventManager.instance.myTeam.currentTeam[i].userMonsterUuid));
				team[i].Init (mon);
				team[i].label.text = "lvl " + mon.userMonster.currentLvl;
			}
			else
			{
				team[i].Init (null, false);
				team[i].label.text = " ";
			}
		}
	}

	public void SetTeamAndGoToBattle()
	{
		StartCoroutine(SetTeamAndGo());
	}

	IEnumerator SetTeamAndGo()
	{
		yield return StartCoroutine(MSClanEventManager.instance.SetRaidTeam());

		PZCombatManager.instance.InitRaid();
		PZPuzzleManager.instance.InitBoard();
		
		MSActionManager.Scene.OnPuzzle();
	}

}
