using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSClanManager : MonoBehaviour 
{
	public FullUserClanProto playerClan;

	public static string userClanUuid = "";
	static UserClanStatus userClanStatus;
	public static bool isLeader = false;

	static List<string> pendingClanInvites = new List<string>();

	public bool isInClan
	{
		get
		{
			return !userClanUuid.Equals("");
		}
	}
	
	List<FullClanProtoWithClanSize> _postedClans;
	public List<FullClanProtoWithClanSize> postedClans
	{
		get
		{
			List<FullClanProtoWithClanSize> clans = _postedClans;
			_postedClans = null;
			return clans;
		}
	}
	
	List<MinimumUserProtoForClans> _postedClanMembers;
	public List<MinimumUserProtoForClans> postedClanMembers
	{
		get
		{
			List<MinimumUserProtoForClans> members = _postedClanMembers;
			_postedClanMembers = null;
			return members;
		}
	}
	
	public List<ClanHelpProto> clanHelpRequests = new List<ClanHelpProto>();

	public int currHelpable
	{
		get
		{
			int helpable = 0;
			foreach(ClanHelpProto help in clanHelpRequests)
			{
				if(!help.mup.userUuid.Equals( MSWhiteboard.localMup.userUuid) && help.helperUuids.Count < help.maxHelpers && !help.helperUuids.Contains(MSWhiteboard.localMup.userUuid))
				{
					helpable++;
				}
			}
			
			return helpable;
		}
	}

	public bool canHelp
	{
		get
		{
			return currHelpable > 0;
		}

	}

	public static MSClanManager instance;

	void Awake()
	{
		instance = this;
		MSActionManager.Loading.OnStartup += InitClanHelp;

		MSActionManager.Clan.OnGiveClanHelp += DealWithGiveClanHelp;
		MSActionManager.Clan.OnSolicitClanHelp += DealWithSoliciteClanHelp;
		MSActionManager.Clan.OnEndClanHelp += DealWithEndClanHelp;

		MSActionManager.Clan.OnRetrieveClanData += RecieveClanData;
	}

	void InitClanHelp(StartupResponseProto startup)
	{
		foreach(ClanHelpProto help in startup.clanHelpings)
		{
			clanHelpRequests.Add(help);
		}
	}

	public void Init(List<FullUserClanProto> clans)
	{
		pendingClanInvites.Clear();
		if (clans.Count > 0)
		{
			if (clans[0].status != UserClanStatus.REQUESTING)
			{
				playerClan = clans[0];
				userClanUuid = clans[0].clanUuid;
				userClanStatus = clans[0].status;
				UMQNetworkManager.instance.CreateClanChatQueue(MSWhiteboard.localMup, clans[0].clanUuid);
			}
			else
			{
				foreach (FullUserClanProto clan in clans)
				{
					pendingClanInvites.Add(clan.clanUuid);
				}
				userClanStatus = UserClanStatus.REQUESTING;
			}
		}
	}

	public bool HasRequestedClan(string clanUuid)
	{
		return (pendingClanInvites.Contains(clanUuid));
	}

	/// <summary>
	/// Searchs the clan listing.
	/// </summary>
	/// <returns>Coroutine: Posts</returns>
	/// <param name="search">Search.</param>
	/// <param name="beforeID">Before I.</param>
	public IEnumerator SearchClanListing(string search = "", int beforeID = 0)
	{
		RetrieveClanInfoRequestProto request = new RetrieveClanInfoRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clanUuid = "";
		request.clanName = search;
		request.grabType = RetrieveClanInfoRequestProto.ClanInfoGrabType.CLAN_INFO;
		request.beforeThisClanId = beforeID;
		request.isForBrowsingList = true;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RETRIEVE_CLAN_INFO_EVENT, null);
		
		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		RetrieveClanInfoResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetrieveClanInfoResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != RetrieveClanInfoResponseProto.RetrieveClanInfoStatus.SUCCESS)
		{
			Debug.LogError("Problem searching clans: " + response.status.ToString());
		}
		else
		{
			_postedClans = response.clanInfo;
		}
	}

	/// <summary>
	/// Gets all the clan details for the specified clan.
	/// </summary>
	/// <returns>Coroutine: Posts the clan details to postedClans and members to postedClanMembers</returns>
	/// <param name="clanId">Clan identifier.</param>
	public IEnumerator GetClanDetails(string clanUuid)
	{
		RetrieveClanInfoRequestProto request = new RetrieveClanInfoRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clanUuid = clanUuid;
		request.grabType = RetrieveClanInfoRequestProto.ClanInfoGrabType.ALL;
		request.isForBrowsingList = false;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RETRIEVE_CLAN_INFO_EVENT, null);
		
		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		RetrieveClanInfoResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetrieveClanInfoResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != RetrieveClanInfoResponseProto.RetrieveClanInfoStatus.SUCCESS)
		{
			Debug.LogError("Problem searching clans: " + response.status.ToString());
		}
		else
		{
			_postedClans = response.clanInfo;
			_postedClanMembers = response.members;

			//If we happen to be loading the user's clan, check if the user is the leader of the clan so that we can adjust
			//UI interaction options appropriately
			////if (_postedClans[0].clan.clanId == userClanId && _postedClans[0].clan.owner.userId == CBKWhiteboard.localMup.userId)
			//{
			//	isLeader = true;
			//}
		}
	}

	public IEnumerator GetClanMembers(string clanUuid)
	{
		RetrieveClanInfoRequestProto request = new RetrieveClanInfoRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clanUuid = clanUuid;
		request.grabType = RetrieveClanInfoRequestProto.ClanInfoGrabType.MEMBERS;
		request.isForBrowsingList = false;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RETRIEVE_CLAN_INFO_EVENT, null);
		
		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		RetrieveClanInfoResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetrieveClanInfoResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != RetrieveClanInfoResponseProto.RetrieveClanInfoStatus.SUCCESS)
		{
			Debug.LogError("Problem searching squads: " + response.status.ToString());
		}
		else
		{
			_postedClanMembers = response.members;
		}
	}

	/// <summary>
	/// Joins the or apply to clan.
	/// PRECONDITION: Player must not belong to a clan.
	/// </summary>
	/// <returns>The or apply to clan.</returns>
	/// <param name="clanId">Clan identifier.</param>
	public IEnumerator JoinOrApplyToClan(string clanUuid)
	{
		RequestJoinClanRequestProto request = new RequestJoinClanRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clanUuid = clanUuid;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REQUEST_JOIN_CLAN_EVENT);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		RequestJoinClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as RequestJoinClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		switch (response.status) 
		{
			case RequestJoinClanResponseProto.RequestJoinClanStatus.SUCCESS_REQUEST:
				pendingClanInvites.Add (response.clanUuid);
				break;
			case RequestJoinClanResponseProto.RequestJoinClanStatus.SUCCESS_JOIN:

				userClanUuid = response.clanUuid;
				userClanStatus = response.requester.clanStatus;

				pendingClanInvites.Clear();

				if (MSActionManager.Clan.OnPlayerClanChange != null)
				{
					MSActionManager.Clan.OnPlayerClanChange(userClanUuid, userClanStatus, response.minClan.clanIconId);
				}
				break;
			default:
				Debug.LogError("Problem joining squad: " + response.status.ToString());
				break;
		}
	}

	/// <summary>
	/// Retracts the join request.
	/// PRECONDITION: Player clan status must be pending a response.
	/// </summary>
	/// <returns>The join request.</returns>
	/// <param name="clanId">Clan identifier.</param>
	public IEnumerator RetractJoinRequest(string clanUuid)
	{
		RetractRequestJoinClanRequestProto request = new RetractRequestJoinClanRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clanUuid = clanUuid;

		int tagNum= UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RETRACT_REQUEST_JOIN_CLAN_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		RetractRequestJoinClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetractRequestJoinClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		userClanUuid = "";

		if (response.status != RetractRequestJoinClanResponseProto.RetractRequestJoinClanStatus.SUCCESS)
		{
			Debug.LogError("Problem retracting join request: " + response.status.ToString());
		}
		else
		{
			pendingClanInvites.Remove(clanUuid);
		}
	}

	/// <summary>
	/// Leaves the current clan.
	/// PRECONDITION: User must be part of a clan
	/// </summary>
	public IEnumerator LeaveClan()
	{
		LeaveClanRequestProto request = new LeaveClanRequestProto();
		request.sender = MSWhiteboard.localMup;

		userClanUuid = "";
		playerClan = null;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_LEAVE_CLAN_EVENT);
	
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		if (MSActionManager.Clan.OnPlayerClanChange != null)
		{
			MSActionManager.Clan.OnPlayerClanChange(userClanUuid, UserClanStatus.MEMBER, 0);
		}

		LeaveClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as LeaveClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != LeaveClanResponseProto.LeaveClanStatus.SUCCESS)
		{
			Debug.LogError("Problem leaving squad: " + response.status.ToString());
		}
		else
		{
			Debug.LogError("success leaving squad");
		}
	}

	/// <summary>
	/// Creates the clan.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="tag">Tag.</param>
	/// <param name="requestRequired">If set to <c>true</c> request required.</param>
	/// <param name="description">Description.</param>
	public IEnumerator CreateClan(MSLoadLock loadLock, string name, string tag, bool requestRequired, string description, int iconId, int cash, int gems = 0)
	{
		loadLock.Lock ();

		CreateClanRequestProto request = new CreateClanRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.name = name;
		request.tag = tag;
		request.requestToJoinClanRequired = requestRequired;
		request.description = description;
		request.clanIconId = iconId;
		request.cashChange = -cash;
		request.gemsSpent = gems;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_CREATE_CLAN_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		CreateClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as CreateClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != CreateClanResponseProto.CreateClanStatus.SUCCESS)
		{
			Debug.LogError("Problem creating squad: " + response.status.ToString());
		}
		else
		{
			userClanUuid = response.clanInfo.clanUuid;
			userClanStatus = UserClanStatus.LEADER;
			isLeader = true;

			if (MSActionManager.Clan.OnPlayerClanChange != null)
			{
				MSActionManager.Clan.OnPlayerClanChange(userClanUuid, userClanStatus, iconId);
			}
		}

		loadLock.Unlock();
	}

	public IEnumerator TransferClanOwnership(string newClanOwnerUuid)
	{
		TransferClanOwnershipRequestProto request = new TransferClanOwnershipRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clanOwnerUuidNew = newClanOwnerUuid;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_TRANSFER_CLAN_OWNERSHIP, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		TransferClanOwnershipResponseProto response = UMQNetworkManager.responseDict[tagNum] as TransferClanOwnershipResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != TransferClanOwnershipResponseProto.TransferClanOwnershipStatus.SUCCESS)
		{
			Debug.LogError("Problem transfering squad ownership: " + response.status.ToString());
		}
		else
		{
			isLeader = false;
		}
	}

	public void EditClan(FullClanProto clan, string description, bool requestNeeded, int shield = 1)
	{
		ChangeClanSettingsRequestProto request = new ChangeClanSettingsRequestProto();
		request.sender = MSWhiteboard.localMup;

		if (clan.description != description)
		{
			request.isChangeDescription = true;
			request.descriptionNow = description;
		}

		if (clan.requestToJoinRequired != requestNeeded)
		{
			request.isChangeJoinType = true;
			request.requestToJoinRequired = requestNeeded;
		}

		if (clan.clanIconId != shield)
		{
			request.isChangeIcon = true;
			request.iconId = shield;
		}

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_CHANGE_CLAN_SETTINGS_EVENT, DealWithChangeSettingsResponse);
	}

	public IEnumerator ChangeClanDescription(string description)
	{
		ChangeClanSettingsRequestProto request = new ChangeClanSettingsRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.isChangeDescription = true;
		request.descriptionNow = description;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_CHANGE_CLAN_SETTINGS_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		ChangeClanSettingsResponseProto response = UMQNetworkManager.responseDict[tagNum] as ChangeClanSettingsResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != ChangeClanSettingsResponseProto.ChangeClanSettingsStatus.SUCCESS)
		{
			Debug.LogError("Problem changing clan description: " + response.status.ToString());
		}
	}

	public void ChangeClanJoinType(bool joinType)
	{
		ChangeClanSettingsRequestProto request = new ChangeClanSettingsRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.isChangeJoinType = true;
		request.requestToJoinRequired = joinType;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_CHANGE_CLAN_SETTINGS_EVENT, DealWithChangeSettingsResponse);
	}

	void DealWithChangeSettingsResponse(int tagNum)
	{

		ChangeClanSettingsResponseProto response = UMQNetworkManager.responseDict[tagNum] as ChangeClanSettingsResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != ChangeClanSettingsResponseProto.ChangeClanSettingsStatus.SUCCESS)
		{
			Debug.LogError("Problem changing clan join type: " + response.status.ToString());
		}
	}

	public void PromoteDemoteClanMember(string playerUuid, UserClanStatus clanStatus)
	{
		PromoteDemoteClanMemberRequestProto request = new PromoteDemoteClanMemberRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.victimUuid = playerUuid;
		request.userClanStatus = clanStatus;

		UMQNetworkManager.instance.SendRequest(request, (int) EventProtocolRequest.C_PROMOTE_DEMOTE_CLAN_MEMBER_EVENT, DealWithPromoteDemoteResponse);
	}

	void DealWithPromoteDemoteResponse(int tagNum)
	{
		PromoteDemoteClanMemberResponseProto response = UMQNetworkManager.responseDict[tagNum] as PromoteDemoteClanMemberResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != PromoteDemoteClanMemberResponseProto.PromoteDemoteClanMemberStatus.SUCCESS)
		{
			Debug.LogError("Problem promoting/demoting member: " + response.status.ToString());
		}
	}

	public void BootPlayerFromClan(string playerUuid)
	{
		BootPlayerFromClanRequestProto request = new BootPlayerFromClanRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.playerToBootUuid = playerUuid;

		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BOOT_PLAYER_FROM_CLAN_EVENT, DealWithBootResponse);
	}

	void DealWithBootResponse(int tagNum)
	{

		BootPlayerFromClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as BootPlayerFromClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != BootPlayerFromClanResponseProto.BootPlayerFromClanStatus.SUCCESS)
		{
			Debug.LogError("Problem booting player from clan: " + response.status.ToString());
		}
	}

	public void DoSolicitClanHelp(GameActionType type, int staticId, string userUuid, int maxHelpers, Action OnComplete = null)
	{
		ClanHelpNoticeProto notice = new ClanHelpNoticeProto();
		notice.helpType = type;
		notice.staticDataId = staticId;//static ID for thing
		notice.userDataUuid = userUuid;//userid ID for thing

		List<ClanHelpNoticeProto> notices = new List<ClanHelpNoticeProto>();
		notices.Add(notice);

		DoSolicitClanHelp(notices, maxHelpers, OnComplete);
	}

	public void DoSolicitClanHelp(List<ClanHelpNoticeProto> notices, int maxHelpers, Action OnComplete = null)
	{
		StartCoroutine(SolicitClanHelp(notices, maxHelpers, OnComplete));
	}

	IEnumerator SolicitClanHelp( List<ClanHelpNoticeProto> notices, int maxHelpers, Action OnComplete = null)
	{
		SolicitClanHelpRequestProto request = new SolicitClanHelpRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.maxHelpers = maxHelpers;//get form clan house struct
		request.clientTime = MSUtil.timeNowMillis;
		foreach(ClanHelpNoticeProto notice in notices)
		{
			request.notice.Add(notice);
		}
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_SOLICIT_CLAN_HELP_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		SolicitClanHelpResponseProto response = UMQNetworkManager.responseDict[tagNum] as SolicitClanHelpResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != SolicitClanHelpResponseProto.SolicitClanHelpStatus.SUCCESS)
		{
			Debug.LogError("Problem Soliciting Clan Help: " + response.status.ToString());
		}
		else
		{
			if(MSActionManager.Clan.OnSolicitClanHelp != null)
			{
				MSActionManager.Clan.OnSolicitClanHelp(response, true);
			}
		}

		if(OnComplete != null)
		{
			OnComplete();
		}
	}
	
	public void DoGiveClanHelp(List<string> helpIds, Action OnComplete = null)
	{
		StartCoroutine(GiveClanHelp(helpIds, OnComplete));
	}
	
	IEnumerator GiveClanHelp(List<string> helpIds, Action OnComplete)
	{
		GiveClanHelpRequestProto request = new GiveClanHelpRequestProto();
		foreach(string id in helpIds)
		{
			request.clanHelpUuids.Add(id);
		}
		request.sender = MSWhiteboard.localMup;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_GIVE_CLAN_HELP_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		GiveClanHelpResponseProto response = UMQNetworkManager.responseDict[tagNum] as GiveClanHelpResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if(response.status != GiveClanHelpResponseProto.GiveClanHelpStatus.SUCCESS)
		{
			Debug.LogError("Problem Giving Clan Help: " + response.status.ToString());
		}
		else
		{
			if(MSActionManager.Clan.OnGiveClanHelp != null)
			{
				MSActionManager.Clan.OnGiveClanHelp(response, true);
			}
		}

		if(OnComplete != null)
		{
			OnComplete();
		}
	}
	
	public void DoEndClanHelp(List<string> helpIds, Action OnComplete = null)
	{
		StartCoroutine(EndClanHelp(helpIds));
	}
	
	IEnumerator EndClanHelp(List<string> helpIds, Action OnComplete = null)
	{
		EndClanHelpRequestProto request = new EndClanHelpRequestProto();
		foreach(string id in helpIds)
		{
			request.clanHelpUuids.Add(id);
		}
		request.sender = MSWhiteboard.localMup;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_END_CLAN_HELP_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}
		
		EndClanHelpResponseProto response = UMQNetworkManager.responseDict[tagNum] as EndClanHelpResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if(response.status != EndClanHelpResponseProto.EndClanHelpStatus.SUCCESS)
		{
			Debug.LogError("Problem Ending Clan Help: " + response.status.ToString());
		}
		else
		{
			if(MSActionManager.Clan.OnEndClanHelp != null)
			{
				MSActionManager.Clan.OnEndClanHelp(response, true);
			}
		}

		if(OnComplete != null)
		{
			OnComplete();
		}
	}

	/// <summary>
	/// Search current list of active requests for help to see if the user has already requested help for a task
	/// </summary>
	/// <returns><c>true</c>, if already requested was helped, <c>false</c> otherwise.</returns>
	/// <param name="type">Type.</param>
	/// <param name="staticId">Static identifier for requested object.</param>
	/// <param name="userId">User id for requested object.</param>
	public bool HelpAlreadyRequested(GameActionType type, int staticId, string userId)
	{
		return GetClanHelp(type, staticId, userId) != null;
	}

	public bool HelpAlreadyRequested(GameActionType type, string userId)
	{
		return GetClanHelp(type, userId) != null;
	}

	public ClanHelpProto GetClanHelp(GameActionType type, int staticId, string userUuid)
	{
		foreach(ClanHelpProto proto in clanHelpRequests)
		{
			if(proto.helpType == type &&
			   proto.staticDataId == staticId &&
			   proto.userDataUuid.Equals(userUuid))
			{
				return proto;
			}
		}

		
		return null;
	}

	public ClanHelpProto GetClanHelp(GameActionType type, string userUuid)
	{
		foreach(ClanHelpProto proto in clanHelpRequests)
		{
			if(proto.helpType == type &&
			   proto.userDataUuid.Equals(userUuid))
			{
				return proto;
			}
		}
		
		return null;
	}

	/// <summary>
	/// Deals the with give clan help.
	/// </summary>
	/// <param name="self">If set to <c>true</c> then this function was triggered by the user and was not sent to us from the server.</param>
	void DealWithGiveClanHelp(GiveClanHelpResponseProto proto, bool self)
	{
		if(self || !proto.sender.userUuid.Equals(MSWhiteboard.localMup.userUuid))
		{
			foreach(ClanHelpProto helpProto in proto.clanHelps)
			{
				for(int i = 0; i < clanHelpRequests.Count;i++)
				{
					if(helpProto.clanHelpUuid.Equals(clanHelpRequests[i].clanHelpUuid))
					{
						clanHelpRequests[i] = helpProto;
					}
				}
			}
		}
	}

	void DealWithSoliciteClanHelp(SolicitClanHelpResponseProto proto, bool self)
	{
		if(self || !proto.sender.userUuid.Equals(MSWhiteboard.localMup.userUuid))
		{
			foreach(ClanHelpProto helpProto in proto.helpProto)
			{
				clanHelpRequests.Add(helpProto);
			}
		}
	}

	void DealWithEndClanHelp(EndClanHelpResponseProto proto, bool self)
	{
		if(self || !proto.sender.userUuid.Equals(MSWhiteboard.localMup.userUuid))
		{
			for(int i = 0; i < clanHelpRequests.Count; i++)
			{
				if(proto.clanHelpUuids.Contains(clanHelpRequests[i].clanHelpUuid))
				{
					if(!clanHelpRequests.Remove(clanHelpRequests[i]))
					{
						Debug.LogError("removing task didn't work");
					}
					i--;
				}
			}
		}
	}

	void RecieveClanData(RetrieveClanDataResponseProto proto)
	{
		clanHelpRequests = proto.clanData.clanHelpings;
		MSClanHelpManager.instance.InitHelp();
	}
}
