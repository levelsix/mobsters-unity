using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// CBK monster manager.
/// Keeps track of what monsters are healing, what monsters are 
/// </summary>
using System;


public class MSMonsterManager : MonoBehaviour {
	
	public List<PZMonster> userMonsters = new List<PZMonster>();
	
	public PZMonster[] userTeam = new PZMonster[TEAM_SLOTS];
	
	public static List<PZMonster> combiningMonsters = new List<PZMonster>();

	public int totalResidenceSlots;

	public const int TEAM_SLOTS = 3;

	private static int _monstersCount;
	
	public static int monstersOwned
	{
		get
		{
			return _monstersCount;
		}
	}
	
	CombineUserMonsterPiecesRequestProto combineRequestProto = null;

	SellUserMonsterRequestProto sellRequest = null;

	public static MSMonsterManager instance;

	public int currTeamPower
	{
		get
		{
			int teamPower = 0;
			foreach (var item in userTeam) 
			{
				if (item != null)
				{
					teamPower += item.teamCost;
				}
			}
			return teamPower;
	    }
	}

//	public long lastEnhance
//	{
//		get
//		{
//			long last = 0;
//			foreach (var item in enhancementFeeders) {
//				last = Math.Max(last, item.finishEnhanceTime);
//			}
//			return last;
//		}
//	}
	
	void Awake()
	{
		if (instance != null)
		{
			Destroy (gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
	}

	/// <summary>
	/// Sets up the monster lists according to data from the StartupResponse
	/// </summary>
	/// <param name="monsters">All user monsters</param>
	/// <param name="healing">List of monsters currently healing</param>
	/// <param name="enhancement">List of monsters enhancing and being used for enhancements</param>
	public void Init(List<FullUserMonsterProto> monsters, List<UserMonsterHealingProto> healing, UserEnhancementProto enhancement)
	{
		PZMonster mon;
		_monstersCount = 0;
		userMonsters.Clear();
		for (int i = 0; i < TEAM_SLOTS; i++) 
		{
			userTeam[i] = null;
		}
		foreach (FullUserMonsterProto item in monsters) 
		{
			mon = new PZMonster(item);
			//Debug.Log("Adding monster " + item.userMonsterId);
			userMonsters.Add(mon);
			if (item.teamSlotNum > 0)
			{
				userTeam[(item.teamSlotNum-1)] = mon; //Fucking off-by-ones.
				_monstersCount++;
			}
			if (!item.isComplete && item.numPieces >= mon.monster.numPuzzlePieces)
			{
				combiningMonsters.Add(mon);
			}
		}

		MSHospitalManager.instance.Init(healing);
		
		if (MSActionManager.Goon.OnTeamChanged != null)
		{
			MSActionManager.Goon.OnTeamChanged();
		}
	}
	
	void OnEnable()
	{
		MSActionManager.Popup.CloseAllPopups += SendRequests;
	}
	
	void OnDisable()
	{
		MSActionManager.Popup.CloseAllPopups -= SendRequests;
	}

	/// <summary>
	/// Update this instance.
	// TODO: Change this to a method that only gets called every second.
	/// We don't need to check all of these things every step, since they run on a second-to-second basis anyways.
	/// </summary>
	void Update()
	{
		//monster don't run on time any more so this is unneccisary un-nescisary unnessisary
//		CheckEnhancingMonsters();
		
		CheckCombiningMonsters();
	}

	public List<PZMonster> GetMonstersByMonsterId(long monsterId, string notMonster = "")
	{
		return userMonsters.FindAll(x=> x.monster.monsterId == monsterId && !x.userMonster.userMonsterUuid.Equals(notMonster));
	}

	public void RemoveMonster(string userMonsterUuid)
	{
		userMonsters.RemoveAll(x=>x.userMonster.userMonsterUuid.Equals(userMonsterUuid));
		if (MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory != null)
		{
			MSActionManager.Goon.OnMonsterRemovedFromPlayerInventory(userMonsterUuid);
		}
		if (MSActionManager.Goon.OnMonsterListChanged != null)
		{
			MSActionManager.Goon.OnMonsterListChanged();
		}
	}

	/// <summary>
	/// Updates or adds the specified monster.
	/// Monsters are updated with new pieces when the player collects monster pieces
	/// New monsters have just had their first piece collected, so they need a new entry
	/// in userMonsters
	/// </summary>
	/// <param name="monster">Monster.</param>
	public PZMonster UpdateOrAdd(FullUserMonsterProto monster)
	{
		PZMonster mon;
		if (userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(monster.userMonsterUuid)) != null)
		{
			Debug.Log("Updating monster: " + monster.userMonsterUuid);
			mon = userMonsters.Find(x=>x.userMonster.userMonsterUuid.Equals(monster.userMonsterUuid));
			mon.UpdateUserMonster(monster);
		}
		else
		{
			Debug.Log("Adding monster: " + monster.userMonsterUuid);
			mon = new PZMonster(monster);
			userMonsters.Add(new PZMonster(monster));
		}
		
		if (!mon.userMonster.isComplete && mon.userMonster.numPieces == mon.monster.numPuzzlePieces)
		{
			StartMonsterCombine(mon);
		}
		if (MSActionManager.Goon.OnMonsterListChanged != null)
		{
			MSActionManager.Goon.OnMonsterListChanged();
		}
		return mon;
	}
	
	/// <summary>
	/// Called during Dugeon Complete, uses the list of monsters sent in EndDungeonResponse
	/// to add new monsters and update incomplete monsters with new pieces
	/// </summary>
	/// <param name="monsters">Monsters.</param>
	public void UpdateOrAddAll(List<FullUserMonsterProto> monsters)
	{
		foreach (FullUserMonsterProto item in monsters) 
		{
			UpdateOrAdd(item);
		}
		if (combineRequestProto != null)
		{
			SendCombineRequest();
		}
		if (MSActionManager.Goon.OnMonsterListChanged != null)
		{
			MSActionManager.Goon.OnMonsterListChanged();
		}
	}

	public Coroutine RestrictMonster(string userMonsterUuid)
	{
		return StartCoroutine(Restrict(userMonsterUuid));
	}

	IEnumerator Restrict(string userMonsterUuid)
	{
		RestrictUserMonsterRequestProto request = new RestrictUserMonsterRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.userMonsterUuids.Add(userMonsterUuid);
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RESTRICT_USER_MONSTER_EVENT);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		RestrictUserMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as RestrictUserMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != RestrictUserMonsterResponseProto.RestrictUserMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem restricting monster: " + response.status.ToString());
		}
	}

	public Coroutine UnrestrictMonster(string userMonsterUuid)
	{
		return StartCoroutine(Unrestrict(userMonsterUuid));
	}
	
	IEnumerator Unrestrict(string userMonsterUuid)
	{
		UnrestrictUserMonsterRequestProto request = new UnrestrictUserMonsterRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.userMonsterUuids.Add(userMonsterUuid);
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_UNRESTRICT_USER_MONSTER_EVENT);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		UnrestrictUserMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as UnrestrictUserMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != UnrestrictUserMonsterResponseProto.UnrestrictUserMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem unrestricting monster: " + response.status.ToString());
		}
	}

	#region Selling

	public void SellRequest(List<PZMonster> monsters)
	{
		SellUserMonsterRequestProto sellRequest = new SellUserMonsterRequestProto();
		sellRequest.sender = MSWhiteboard.localMupWithResources;
		foreach (var item in monsters) 
		{
			sellRequest.sales.Add(item.GetSellProto());
		}

		UMQNetworkManager.instance.SendRequest(sellRequest, (int)EventProtocolRequest.C_SELL_USER_MONSTER_EVENT, DealWithSellResponse);
		
	}


	/// <summary>
	/// This now only get called after the server response for sell comes back with success
	/// </summary>
	/// <param name="monsters">Monsters.</param>
	public void SellMonsters(List<PZMonster> monsters)
	{
//		SellUserMonsterRequestProto sellRequest = new SellUserMonsterRequestProto();
//		sellRequest.sender = MSWhiteboard.localMupWithResources;

		foreach (var item in monsters) 
		{
			MSResourceManager.instance.Collect(ResourceType.CASH, item.sellValue);
//			sellRequest.sales.Add(item.GetSellProto());
			RemoveMonster(item.userMonster.userMonsterUuid);
		}

		if (MSActionManager.Quest.OnMonstersSold != null)
		{
			MSActionManager.Quest.OnMonstersSold(monsters.Count);
		}

	}

	void DealWithSellResponse(int tagNum)
	{
		SellUserMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as SellUserMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != SellUserMonsterResponseProto.SellUserMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem selling monsters: " + response.status.ToString());
		}
		else
		{
			if(MSActionManager.Goon.OnFinishSelling != null)
			{
				MSActionManager.Goon.OnFinishSelling();
			}
		}
	}

	#endregion

	#region Team Management
	
	public bool AddToTeam(PZMonster monster)
	{
		if (_monstersCount == TEAM_SLOTS)
		{
			MSActionManager.Popup.DisplayRedError("Team is full!");
			return false;
		}
		if (monster.teamCost + currTeamPower > MSBuildingManager.teamCenter.combinedProto.teamCenter.teamCostLimit)
		{
			MSActionManager.Popup.DisplayRedError("You need a higher power limit to add " + monster.monster.displayName + " to your team. Upgrade your town center!");
			return false;
		}

		for (int i = 0; i < userTeam.Length; i++) 
		{
			if (userTeam[i] == null || userTeam[i].monster.monsterId <= 0)
			{
				userTeam[i] = monster;
				monster.userMonster.teamSlotNum = (i+1); //Off by one
				_monstersCount++;
				
				AddMonsterToBattleTeamRequestProto request = new AddMonsterToBattleTeamRequestProto();
				request.sender = MSWhiteboard.localMup;
				request.userMonsterUuid = monster.userMonster.userMonsterUuid;
				request.teamSlotNum = (i+1);
				UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_ADD_MONSTER_TO_BATTLE_TEAM_EVENT, DealWithAddResponse);
				
				if (MSActionManager.Goon.OnMonsterAddTeam != null)
				{
					MSActionManager.Goon.OnMonsterAddTeam(monster);
				}
				
				if (MSActionManager.Goon.OnTeamChanged != null)
				{
					MSActionManager.Goon.OnTeamChanged();
				}
				
				return true;
			}
		}
		return false;
	}	
	
	void DealWithAddResponse(int tagNum)
	{
		AddMonsterToBattleTeamResponseProto response = UMQNetworkManager.responseDict[tagNum] as AddMonsterToBattleTeamResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != AddMonsterToBattleTeamResponseProto.AddMonsterToBattleTeamStatus.SUCCESS)
		{
			Debug.LogError("Problem adding monster to team: " + response.status.ToString());
			MSSceneManager.instance.ReconnectPopup();
		}
	}
	
	public void RemoveFromTeam(PZMonster monster)
	{
		userTeam[(monster.userMonster.teamSlotNum-1)] = null;
		
		RemoveMonsterFromBattleTeamRequestProto request = new RemoveMonsterFromBattleTeamRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.userMonsterUuid = monster.userMonster.userMonsterUuid;
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REMOVE_MONSTER_FROM_BATTLE_TEAM_EVENT, DealWithRemoveResponse);
		
		monster.userMonster.teamSlotNum = 0;
		
		_monstersCount--;
		
		if (MSActionManager.Goon.OnMonsterRemoveTeam != null)
		{
			MSActionManager.Goon.OnMonsterRemoveTeam(monster);
		}
		
		if (MSActionManager.Goon.OnTeamChanged != null)
		{
			MSActionManager.Goon.OnTeamChanged();
		}
	}

	void DealWithRemoveResponse(int tagNum)
	{
		RemoveMonsterFromBattleTeamResponseProto response = UMQNetworkManager.responseDict[tagNum] as RemoveMonsterFromBattleTeamResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != RemoveMonsterFromBattleTeamResponseProto.RemoveMonsterFromBattleTeamStatus.SUCCESS)
		{
			Debug.LogError("Problem removing monster from team: " + response.status.ToString());
			MSSceneManager.instance.ReconnectPopup();
		}
	}
	
	#endregion

	#region Combining
	
	/// <summary>
	/// Prepares a new combine pieces request.
	/// </summary>
	void PrepareNewCombinePiecesRequest()
	{
		combineRequestProto = new CombineUserMonsterPiecesRequestProto();
		combineRequestProto.sender = MSWhiteboard.localMup;
	}
	
	void SendCombineRequest()
	{
		UMQNetworkManager.instance.SendRequest(combineRequestProto, (int)EventProtocolRequest.C_COMBINE_USER_MONSTER_PIECES_EVENT, DealWithCombineResponse);
		combineRequestProto = null;
	}
	
	/// <summary>
	/// Starts the monster combine, given that enough pieces
	/// are present.
	/// </summary>
	/// <param name="monster">Monster.</param>
	void StartMonsterCombine(PZMonster monster)
	{
		if (monster.monster.minutesToCombinePieces == 0)
		{
			CombineMonster(monster);
		}
		else
		{
			monster.userMonster.combineStartTime = MSUtil.timeNowMillis;
			combiningMonsters.Add(monster);
		}
	}
	
	/// <summary>
	/// Turns an incomplete monster into a complete monster.
	/// Called after the combination time has passed.
	/// </summary>
	/// <param name="monster">Monster.</param>
	void CombineMonster(PZMonster monster)
	{
		if (combineRequestProto == null)
		{
			PrepareNewCombinePiecesRequest();
		}
		
		monster.userMonster.isComplete = true;
		
		combineRequestProto.userMonsterUuids.Add(monster.userMonster.userMonsterUuid);
		
		if (combiningMonsters.Contains (monster))
		{
			combiningMonsters.Remove(monster);
		}
	}
	
	/// <summary>
	/// Checks through the list of monsters that have enough pieces to see if we
	/// need to complete any of them.
	/// </summary>
	void CheckCombiningMonsters()
	{
		if (combineRequestProto == null)
		{
			for (int i = combiningMonsters.Count - 1; i >= 0; i--){
				PZMonster item = combiningMonsters[i];
				if (item.combineTimeLeft <= 0)
				CombineMonster(item);
			}
			if (combineRequestProto != null)
			{
				SendCombineRequest();
			}
		}
	}

	public void SpeedUpCombine(PZMonster monster)
	{
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, monster.combineFinishGems))
		{
			monster.userMonster.isComplete = true;

			combiningMonsters.Remove(monster);
			
			PrepareNewCombinePiecesRequest();
			
			combineRequestProto.userMonsterUuids.Add(monster.userMonster.userMonsterUuid);
			
			combineRequestProto.gemCost = monster.combineFinishGems;
			
			SendCombineRequest();
		}
	}
	
	#endregion

	void DealWithCombineResponse(int tagNum)
	{
		CombineUserMonsterPiecesResponseProto response = UMQNetworkManager.responseDict[tagNum] as CombineUserMonsterPiecesResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != CombineUserMonsterPiecesResponseProto.CombineUserMonsterPiecesStatus.SUCCESS)
		{
			Debug.LogError("Problem combining pieces: " + response.status.ToString());
		}
		
		if (MSActionManager.Goon.OnHealQueueChanged != null)
		{
			MSActionManager.Goon.OnHealQueueChanged();
		}
	}

	#region Evolution

	public PZMonster FindEvolutionBuddy(PZMonster monster)
	{
		if (monster.userMonster.currentLvl < monster.monster.maxLevel)
		{
			return null;
		}
		foreach (var item in GetMonstersByMonsterId(monster.monster.monsterId)) 
		{
			if (item.userMonster.currentLvl >= monster.monster.maxLevel 
			    && !item.userMonster.userMonsterUuid.Equals(monster.userMonster.userMonsterUuid))
			{
				return item;
			}
		}
		return null;
	}

	#endregion

	public void SetMobsterAsAvatar(int monsterId)
	{
		SetAvatarMonsterRequestProto request = new SetAvatarMonsterRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.monsterId = monsterId;

		MSWhiteboard.localUser.avatarMonsterId = monsterId;
		MSWhiteboard.localMup.avatarMonsterId = monsterId;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_SET_AVATAR_MONSTER_EVENT, DealWithSetAvatarResponse);
	}

	void DealWithSetAvatarResponse(int tagNum)
	{
		SetAvatarMonsterResponseProto response = UMQNetworkManager.responseDict[tagNum] as SetAvatarMonsterResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != SetAvatarMonsterResponseProto.SetAvatarMonsterStatus.SUCCESS)
		{
			Debug.LogError("Problem setting player avatar: " + response.status.ToString());
		}
	}


	void OnPopupClosed()
	{
		SendRequests();
	}
	
	void SendRequests()
	{
	}
	
	void OnApplicationQuit()
	{
		SendRequests();
	}

	[ContextMenu ("Debug Dump")]
	public void DebugDump()
	{
		foreach (var item in userMonsters) 
		{
			Debug.Log("Monster: " + item.monster.displayName
			          + "Level: " + item.userMonster.currentLvl
			          + "Status: " + item.monsterStatus);
		}
	}
}
					

