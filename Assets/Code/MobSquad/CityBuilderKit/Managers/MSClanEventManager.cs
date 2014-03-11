using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSClanEventManager
/// </summary>
public class MSClanEventManager : MonoBehaviour {

	public static MSClanEventManager instance;

	public PersistentClanEventClanInfoProto currClanInfo;

	public PersistentClanEventUserInfoProto currUserInfo;

	public PersistentClanEventProto currPersisRaid;

	public ClanRaidProto currRaid;

	public ClanRaidStageProto currStage;

	public long currStageTimeLeft
	{
		get
		{
			return (currClanInfo.stageStartTime + currStage.durationMinutes * 60000) - MSUtil.timeNowMillis;
		}
	}

	void Awake()
	{
		instance = this;
	}

	public void Init()
	{

	}
}
