using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKClanManager : MonoBehaviour 
{
	public static int userClanId = 0;
	static UserClanStatus userClanStatus;
	public static bool isLeader = false;

	static List<int> pendingClanInvites = new List<int>();

	public bool isInClan
	{
		get
		{
			return (userClanId > 0);
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

	public static CBKClanManager instance;

	void Awake()
	{
		instance = this;
	}

	public void Init(List<FullUserClanProto> clans)
	{
		pendingClanInvites.Clear();
		if (clans.Count > 0)
		{
			if (clans[0].status == UserClanStatus.MEMBER)
			{
				userClanId = clans[0].clanId;
				userClanStatus = clans[0].status;
			}
			else
			{
				foreach (FullUserClanProto clan in clans)
				{
					pendingClanInvites.Add(clan.clanId);
				}
				userClanStatus = UserClanStatus.REQUESTING;
			}
		}
	}

	public bool HasRequestedClan(int clanId)
	{
		return (pendingClanInvites.Contains(clanId));
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
		request.sender = CBKWhiteboard.localMup;
		request.clanId = 0;
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
	public IEnumerator GetClanDetails(int clanId)
	{
		RetrieveClanInfoRequestProto request = new RetrieveClanInfoRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.clanId = clanId;
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
			if (_postedClans[0].clan.clanId == userClanId && _postedClans[0].clan.owner.userId == CBKWhiteboard.localMup.userId)
			{
				isLeader = true;
			}
		}
	}

	public IEnumerator GetClanMembers(int clanId)
	{
		RetrieveClanInfoRequestProto request = new RetrieveClanInfoRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.clanId = clanId;;
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
			Debug.LogError("Problem searching clans: " + response.status.ToString());
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
	public IEnumerator JoinOrApplyToClan(int clanId)
	{
		RequestJoinClanRequestProto request = new RequestJoinClanRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.clanId = clanId;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_REQUEST_JOIN_CLAN_EVENT, null);

		while(!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		RequestJoinClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as RequestJoinClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		switch (response.status) 
		{
		case RequestJoinClanResponseProto.RequestJoinClanStatus.REQUEST_SUCCESS:
			pendingClanInvites.Add (response.clanId);
			break;
		case RequestJoinClanResponseProto.RequestJoinClanStatus.JOIN_SUCCESS:

			userClanId = response.clanId;
			userClanStatus = response.requester.clanStatus;

			if (CBKEventManager.Clan.OnPlayerClanChange != null)
			{
				CBKEventManager.Clan.OnPlayerClanChange(userClanId, userClanStatus);
			}
			break;
		default:
			Debug.LogError("Problem joining clan: " + response.status.ToString());
			break;
		}
	}

	/// <summary>
	/// Retracts the join request.
	/// PRECONDITION: Player clan status must be pending a response.
	/// </summary>
	/// <returns>The join request.</returns>
	/// <param name="clanId">Clan identifier.</param>
	public IEnumerator RetractJoinRequest(int clanId)
	{
		RetractRequestJoinClanRequestProto request = new RetractRequestJoinClanRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.clanId = clanId;

		int tagNum= UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RETRACT_REQUEST_JOIN_CLAN_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		RetractRequestJoinClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetractRequestJoinClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		userClanId = 0;

		if (response.status != RetractRequestJoinClanResponseProto.RetractRequestJoinClanStatus.SUCCESS)
		{
			Debug.LogError("Problem retracting join request: " + response.status.ToString());
		}
		else
		{
			pendingClanInvites.Remove(clanId);
		}
	}

	/// <summary>
	/// Leaves the current clan.
	/// PRECONDITION: User must be part of a clan
	/// </summary>
	public IEnumerator LeaveClan()
	{
		LeaveClanRequestProto request = new LeaveClanRequestProto();
		request.sender = CBKWhiteboard.localMup;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_LEAVE_CLAN_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		LeaveClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as LeaveClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != LeaveClanResponseProto.LeaveClanStatus.SUCCESS)
		{
			Debug.LogError("Problem leaving clan: " + response.status.ToString());
		}
		else
		{
			userClanId = 0;
			if (CBKEventManager.Clan.OnPlayerClanChange != null)
			{
				CBKEventManager.Clan.OnPlayerClanChange(userClanId, UserClanStatus.MEMBER);
			}
		}
	}

	/// <summary>
	/// Creates the clan.
	/// PRECONDITION: User needs to have enough free currency to create a clan.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="tag">Tag.</param>
	/// <param name="requestRequired">If set to <c>true</c> request required.</param>
	/// <param name="description">Description.</param>
	public IEnumerator CreateClan(string name, string tag, bool requestRequired, string description)
	{
		CreateClanRequestProto request = new CreateClanRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.name = name;
		request.tag = tag;
		request.requestToJoinClanRequired = requestRequired;
		request.description = description;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_CREATE_CLAN_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		CreateClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as CreateClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != CreateClanResponseProto.CreateClanStatus.SUCCESS)
		{
			Debug.LogError("Problem creating clan: " + response.status.ToString());
		}
		else
		{
			CBKResourceManager.instance.Spend(ResourceType.CASH, CBKWhiteboard.constants.clanConstants.coinPriceToCreateClan);

			userClanId = response.clanInfo.clanId;
			userClanStatus = UserClanStatus.MEMBER;
			isLeader = true;
		}
	}

	public IEnumerator TransferClanOwnership(int newClanOwnerId)
	{
		TransferClanOwnershipRequestProto request = new TransferClanOwnershipRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.newClanOwnerId = newClanOwnerId;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_TRANSFER_CLAN_OWNERSHIP, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		TransferClanOwnershipResponseProto response = UMQNetworkManager.responseDict[tagNum] as TransferClanOwnershipResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != TransferClanOwnershipResponseProto.TransferClanOwnershipStatus.SUCCESS)
		{
			Debug.LogError("Problem transfering clan ownership: " + response.status.ToString());
		}
		else
		{
			isLeader = false;
		}
	}

	public IEnumerator ChangeClanDescription(string description)
	{
		ChangeClanDescriptionRequestProto request = new ChangeClanDescriptionRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.description = description;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_CHANGE_CLAN_DESCRIPTION_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		ChangeClanDescriptionResponseProto response = UMQNetworkManager.responseDict[tagNum] as ChangeClanDescriptionResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != ChangeClanDescriptionResponseProto.ChangeClanDescriptionStatus.SUCCESS)
		{
			Debug.LogError("Problem changing clan description: " + response.status.ToString());
		}
	}

	public IEnumerator ChangeClanJoinType(bool joinType)
	{
		ChangeClanJoinTypeRequestProto request = new ChangeClanJoinTypeRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.requestToJoinRequired = joinType;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_CHANGE_CLAN_JOIN_TYPE_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		ChangeClanJoinTypeResponseProto response = UMQNetworkManager.responseDict[tagNum] as ChangeClanJoinTypeResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != ChangeClanJoinTypeResponseProto.ChangeClanJoinTypeStatus.SUCCESS)
		{
			Debug.LogError("Problem changing clan join type: " + response.status.ToString());
		}
	}

	public IEnumerator BootPlayerFromClan(int playerId)
	{
		BootPlayerFromClanRequestProto request = new BootPlayerFromClanRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.playerToBoot = playerId;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_BOOT_PLAYER_FROM_CLAN_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		BootPlayerFromClanResponseProto response = UMQNetworkManager.responseDict[tagNum] as BootPlayerFromClanResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status != BootPlayerFromClanResponseProto.BootPlayerFromClanStatus.SUCCESS)
		{
			Debug.LogError("Problem booting player from clan: " + response.status.ToString());
		}
	}
}
