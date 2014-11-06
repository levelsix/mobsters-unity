using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSClanEventManager
/// </summary>
public class MSClanEventManager : MonoBehaviour {

	public static MSClanEventManager instance;

	public PersistentClanEventClanInfoProto currClanInfo;

	public List<PersistentClanEventUserInfoProto> currUserInfos;

	public PersistentClanEventProto currPersisRaid;

	public ClanRaidProto currRaid;

	public int currDamage;

	public UserCurrentMonsterTeamProto myTeam
	{
		get
		{
			if (currClanInfo == null)
			{
				return null;
			}
			PersistentClanEventUserInfoProto userInfo = currUserInfos.Find(x=>x.userId == MSWhiteboard.localMup.userId);
			if (userInfo == null)
			{
				return null;
			}
			return userInfo.userMonsters;
		}
	}

	public ClanRaidStageProto currStage
	{
		get
		{
			if (currClanInfo == null)
			{
				return null;
			}
			return currRaid.raidStages.Find(x=>x.clanRaidStageId == currClanInfo.clanRaidStageId);
		}
	}

	public bool inProgress
	{
		get
		{
			return currClanInfo != null && currClanInfo.clanEventId > 0;
		}
	}

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

	void OnEnable()
	{
		MSActionManager.Loading.OnStartup += OnStartup;
	}

	void OnDisable()
	{
		MSActionManager.Loading.OnStartup -= OnStartup;
	}

	#region clanHelp

	public void DealWithClanHelpEnd(EndClanHelpResponseProto proto)
	{
		if(MSActionManager.Clan.OnEndClanHelp != null)
		{
			Debug.Log("Recieving EndClanHelp Request");
			MSActionManager.Clan.OnEndClanHelp(proto, false);
		}
	}

	public void DealWithClanHelpSolicitation(SolicitClanHelpResponseProto proto)
	{
		if(MSActionManager.Clan.OnSolicitClanHelp != null)
		{
			Debug.Log("Recieving SolicitClanHelp Request");
			MSActionManager.Clan.OnSolicitClanHelp(proto, false);
		}
	}

	public void DealWithClanHelpGive(GiveClanHelpResponseProto proto)
	{
		if(MSActionManager.Clan.OnGiveClanHelp != null)
		{
			Debug.Log("Recieving GiveClanHelp Request");
			MSActionManager.Clan.OnGiveClanHelp(proto, false);
		}
	}

	#endregion clanHelp

	#region clanAttack

	/// <summary>
	/// Gets the current stage monsters.
	/// Returns them in the proper order, too.
	/// </summary>
	/// <returns>The current stage monsters.</returns>
	public List<ClanRaidStageMonsterProto> GetCurrentStageMonsters()
	{
		return currRaid.raidStages.Find(x=>x.clanRaidStageId==currClanInfo.clanRaidStageId).monsters;
	}

	public void Begin(PersistentClanEventProto persisEvent)
	{
		BeginClanRaidRequestProto request = new BeginClanRaidRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.curTime = MSUtil.timeNowMillis;
		request.raidId = persisEvent.clanRaidId;
		request.clanEventId = persisEvent.clanEventId;
		request.isFirstStage = true;
		
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_CLAN_RAID_EVENT, DealWithBeginResponse);
	}

	public IEnumerator SetRaidTeam()
	{
		if (myTeam == null)
		{
			BeginClanRaidRequestProto request = new BeginClanRaidRequestProto();
			request.sender = MSWhiteboard.localMup;
			request.curTime = MSUtil.timeNowMillis;
			request.raidId = currRaid.clanRaidId;
			request.clanEventId = currPersisRaid.clanEventId;
			request.isFirstStage = false;
			request.setMonsterTeamForRaid = true;

			foreach (var item in MSMonsterManager.instance.userTeam) 
			{
				if (item != null)
				{
					request.userMonsters.Add(item.userMonster);
				}
			}

			int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BEGIN_CLAN_RAID_EVENT, null);

			MSActionManager.Popup.OnPopup(MSPopupManager.instance.popups.loadingScreenBlocker);
			while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
			{
				yield return null;
			}
			MSActionManager.Popup.CloseTopPopupLayer();

			DealWithBeginResponse(tagNum);
		}
	}

	public void DealWithBeginResponse(BeginClanRaidResponseProto response)
	{
		if (response.status == BeginClanRaidResponseProto.BeginClanRaidStatus.SUCCESS)
		{
			if (response.eventDetails != null)
			{
				currClanInfo = response.eventDetails;
				currRaid = MSDataManager.instance.Get<ClanRaidProto>(response.eventDetails.clanRaidId);
				currPersisRaid = MSDataManager.instance.Get<PersistentClanEventProto>(response.eventDetails.clanEventId);
				
				if (MSActionManager.Clan.OnRaidBegin != null)
				{
					MSActionManager.Clan.OnRaidBegin();
				}
			}
			if (response.userDetails != null)
			{
				MSClanEventManager.instance.currUserInfos.Add(response.userDetails);
			}
		}
		else
		{
			Debug.LogError("Problem beginning clan raid: " + response.status.ToString());
		}
	}

	public void DealWithBeginResponse(int tagNum)
	{
		BeginClanRaidResponseProto response = UMQNetworkManager.responseDict[tagNum] as BeginClanRaidResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		DealWithBeginResponse(response);
	}

	public void SendAttack(int damage, PZMonster userMonster, int enemyDamage)
	{
		AttackClanRaidMonsterRequestProto request = new AttackClanRaidMonsterRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.eventDetails = currClanInfo;
		request.clientTime = MSUtil.timeNowMillis;
		request.damageDealt = damage;

		UserMonsterCurrentHealthProto hp;
		foreach (var item in MSMonsterManager.instance.userTeam) 
		{
			if (item != null)
			{
				hp = new UserMonsterCurrentHealthProto();
				hp.userMonsterId = item.userMonster.userMonsterId;
				hp.currentHealth = item.currHP;
				if (item == userMonster)
				{
					hp.currentHealth = Math.Max(hp.currentHealth - enemyDamage, 0);
				}
				request.monsterHealths.Add(hp);
			}
		}

		request.userMonsterThatAttacked = userMonster.userMonster;

		request.userMonsterTeam = myTeam;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ATTACK_CLAN_RAID_MONSTER_EVENT, DealWithAttackResponse);
	}

	public void DealWithAttackResponse(AttackClanRaidMonsterResponseProto response)
	{
		ProcessAttackResponse(response);
		if (response.status == AttackClanRaidMonsterResponseProto.AttackClanRaidMonsterStatus.SUCCESS_MONSTER_JUST_DIED)
		{
			Debug.Log("Monster killed!");
			
			if (MSActionManager.Clan.OnRaidMonsterDied != null)
			{
				MSActionManager.Clan.OnRaidMonsterDied(response);
			}
			
			currDamage = 0;
		}
		else if (response.status == AttackClanRaidMonsterResponseProto.AttackClanRaidMonsterStatus.SUCCESS)
		{
			Debug.Log("Monster damaged: " + response.dmgDealt);
			
			if (MSActionManager.Clan.OnRaidMonsterAttacked != null)
			{
				MSActionManager.Clan.OnRaidMonsterAttacked(response);
			}
		}
		else
		{
			Debug.LogError(response.status.ToString());
		}
	}

	public void DealWithAttackResponse(int tagNum)
	{
		AttackClanRaidMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as AttackClanRaidMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		DealWithAttackResponse(response);
	}

	void ProcessAttackResponse (AttackClanRaidMonsterResponseProto response)
	{
		currDamage += response.dmgDealt;

		currClanInfo = response.eventDetails;

		foreach (var item in response.clanUsersDetails) {
			PersistentClanEventUserInfoProto userInfo = currUserInfos.Find (x => x.userId == item.userId);
			if (userInfo != null) 
			{
				userInfo.crDmgDone = item.crDmgDone;
				userInfo.crsDmgDone = item.crsDmgDone;
				userInfo.crsmDmgDone = item.crsmDmgDone;
			}
			else 
			{
				currUserInfos.Add (item);
			}
		}
	}

	void OnStartup(StartupResponseProto response)
	{
		currClanInfo = response.curRaidClanInfo;
		currUserInfos = response.curRaidClanUserInfo;

		if (currClanInfo != null)
		{
			currRaid = MSDataManager.instance.Get<ClanRaidProto>(currClanInfo.clanRaidId);
			currPersisRaid = MSDataManager.instance.Get<PersistentClanEventProto>(currClanInfo.clanEventId);

			currDamage = 0;
			foreach (var item in currUserInfos) 
			{
				currDamage += item.crsmDmgDone;
			}
		}
	}

	void SendRecordRequest()
	{
		RecordClanRaidStatsRequestProto request = new RecordClanRaidStatsRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clientTime = MSUtil.timeNowMillis;
		request.clanId = MSClanManager.instance.playerClan.clanId;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RECORD_CLAN_RAID_STATS_EVENT, DealWithRecordResponse);

		currClanInfo = null;
		currUserInfos.Clear ();
		currRaid = null;
		currPersisRaid = null;
		currDamage = 0;
	}

	void DealWithRecordResponse(int tagNum)
	{
		RecordClanRaidStatsResponseProto response = UMQNetworkManager.responseDict[tagNum] as RecordClanRaidStatsResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != RecordClanRaidStatsResponseProto.RecordClanRaidStatsStatus.SUCCESS)
		{
			Debug.LogError("Problem recording Clan Raid Stats: " + response.status.ToString());
		}
	}

	void Update()
	{
		if (currClanInfo != null && currClanInfo.clanEventId > 0 && currStageTimeLeft <= 0)
		{
			SendRecordRequest();
		}
	}

	#endregion clanAttack
}
