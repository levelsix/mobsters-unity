using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSRaidStageBoxUI
/// </summary>
public class MSRaidStageBoxUI : MonoBehaviour {

	[SerializeField]
	UILabel stageName;

	[SerializeField]
	MSCenterGrid prizeGrid;

	[SerializeField]
	UILabel unlockLabel;

	[SerializeField]
	GameObject beginParent;

	[SerializeField]
	GameObject battleParent;

	[SerializeField]
	UISprite progressBar;

	[SerializeField]
	UILabel progressLabel;

	[SerializeField]
	UISprite header;

	[SerializeField]
	CBKMiniHealingBox[] prizes;

	ClanRaidStageProto stage;

	PersistentClanEventProto eventInfo;

	bool isBeginning = false;

	const string openStage = "openstage";
	const string closedStage = "lockedstage";

	void OnEnable()
	{
		MSActionManager.Clan.OnRaidBegin += OnRaidBegin;
	}

	void OnDisable()
	{
		MSActionManager.Clan.OnRaidBegin -= OnRaidBegin;
	}

	public void Init(ClanRaidStageProto stage, PersistentClanEventProto info, PersistentClanEventClanInfoProto clanInfo = null)
	{ 
		this.stage = stage;
		eventInfo = info;

		stageName.text = stage.name;

		//Prizes
		int i = 0;
		while (i < stage.monsters.Count)
		{
			//Debug.Log(i);
			prizes[i].Init(MSDataManager.instance.Get<MonsterProto>(stage.monsters[i].monsterId));
			i++;
		}

		while (i < prizes.Length)
		{
			prizes[i++].gameObject.SetActive(false);
		}

		prizeGrid.Reposition();

		unlockLabel.text = " ";
		battleParent.SetActive(false);
		beginParent.SetActive(false);
		if (clanInfo == null)
		{
			if (stage.stageNum == 1)
			{
				beginParent.SetActive(true);
			}
			else
			{
				SetAsCompleteBefore(stage);
			}
		}
		else if (stage.clanRaidStageId == clanInfo.clanRaidStageId)
		{
			SetBattle (stage, clanInfo);
		}
		else if (stage.clanRaidStageId > clanInfo.clanRaidStageId) //Closed stage
		{
			SetAsCompleteBefore (stage);

		}
		else //Completed stage
		{
			SetAsCompleted();
		}
	}

	void SetBattle (ClanRaidStageProto stage, PersistentClanEventClanInfoProto clanInfo)
	{
		beginParent.SetActive(false);
		battleParent.SetActive (true);
		//Figure out percentage
		float percentagePerMonster = 1f / stage.monsters.Count;
		float percentage = ((clanInfo.crsmId-1) + ((float)(MSClanEventManager.instance.currDamage))/(stage.monsters[clanInfo.crsmId-1].monsterHp)) * percentagePerMonster;
		progressBar.fillAmount = percentage;
		long timeLeft = (clanInfo.stageStartTime + stage.durationMinutes * 60000) - MSUtil.timeNowMillis;
		int percentageDisplay = (int)(percentage * 100);
		progressLabel.text = percentageDisplay + "% Done / " + MSUtil.TimeStringShort (timeLeft) + " Left";
	}

	void SetAsCompleted()
	{
		battleParent.SetActive(false);
		unlockLabel.text = "Stage Complete!";
		header.spriteName = openStage;
	}

	void SetAsCompleteBefore (ClanRaidStageProto stage)
	{
		unlockLabel.text = "Complete stage " + (stage.clanRaidStageId - 1) + " to Unlock";
		header.spriteName = closedStage;
	}

	public void OnBegin()
	{
		if (!isBeginning && MSClanManager.instance.playerClan.status != UserClanStatus.MEMBER)
		{
			isBeginning = true;
			MSClanEventManager.instance.Begin(eventInfo);
		}
	}

	void OnRaidBegin()
	{
		SetBattle(stage, MSClanEventManager.instance.currClanInfo);
		isBeginning = false;
	}

	public void OnBattle()
	{
		//TODO: Check-in user monster team
		if (MSClanEventManager.instance.myTeam == null)
		{
			MSClanEventManager.instance.SetRaidTeam();
		}

		PZCombatManager.instance.InitRaid();
		PZPuzzleManager.instance.InitBoard();

		MSActionManager.Scene.OnPuzzle();
	}
}
