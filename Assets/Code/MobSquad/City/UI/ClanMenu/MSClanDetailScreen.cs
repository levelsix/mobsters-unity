using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class MSClanDetailScreen : MonoBehaviour {

	[SerializeField]
	UI2DSprite clanLogo;

	[SerializeField]
	UILabel clanName;

	[SerializeField]
	UILabel clanDescription;

	[SerializeField]
	UILabel membersLabel;

	[SerializeField]
	MSClanJoinButton joinButton;

	public UIGrid memberGrid;

	[SerializeField]
	MSClanMemberEntry clanMemberEntryPrefab;

	[SerializeField]
	GameObject loadingObjects;

	public List<MSClanMemberEntry> memberList = new List<MSClanMemberEntry>();

	FullClanProtoWithClanSize clan;

	public void Init(int clanId)
	{
		StartCoroutine(RetrieveClanValues(clanId));
	}

	void SetLoadingMode()
	{
		clanLogo.alpha = 0;
		clanName.text = "Loading...";
		clanDescription.text = "Fetching clan information";

		foreach (var item in memberList) 
		{
			item.Pool();
		}
		
		memberList.Clear();
	}

	IEnumerator RetrieveClanValues(int clanId)
	{
		loadingObjects.SetActive(true);

		SetLoadingMode();

		RetrieveClanInfoRequestProto request = new RetrieveClanInfoRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.clanId = clanId;
		request.grabType = RetrieveClanInfoRequestProto.ClanInfoGrabType.ALL;
		request.isForBrowsingList = false;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RETRIEVE_CLAN_INFO_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		clanLogo.alpha = 1;

		RetrieveClanInfoResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetrieveClanInfoResponseProto;

		joinButton.Init(response.clanInfo[0]);

		clan = response.clanInfo[0];

		clanName.text = clan.clan.name;
		clanDescription.text = clan.clan.description;
		MSSpriteUtil.instance.SetSprite("clanicon", "clanicon" + clan.clan.clanIconId, clanLogo);

		DateTime foundationDate = (new DateTime(1970,1,1)).AddMilliseconds(clan.clan.createTime);
		membersLabel.text = clan.clanSize + "/" + MSWhiteboard.constants.clanConstants.maxClanSize + " MEM.";

		foreach (var item in response.members) 
		{
			if (item.clanStatus != UserClanStatus.REQUESTING)
			{
				AddMemberEntryToGrid(item, response.monsterTeams.Find(x => x.userId == item.minUserProtoWithLevel.minUserProto.userId));
			}
		}
		memberGrid.Reposition();

		if (MSClanManager.userClanId == 0)
		{
			//TODO: Set join button up
		}
		else if (MSClanManager.userClanId == clan.clan.clanId && MSClanManager.isLeader)
		{
			//TODO: Set edit button up
		}

		loadingObjects.SetActive(false);
	}

	public void CloseAllOptions()
	{
		foreach (var item in memberList) 
		{
			item.CloseOptions();
		}
	}

	void AddMemberEntryToGrid(MinimumUserProtoForClans member, UserCurrentMonsterTeamProto monsters)
	{
		MSClanMemberEntry entry = MSPoolManager.instance.Get(clanMemberEntryPrefab, Vector3.zero) as MSClanMemberEntry;
		entry.transf.parent = memberGrid.transform;
		entry.transf.localScale = Vector3.one;
		entry.Init(member, monsters, this);
		foreach (var item in entry.GetComponentsInChildren<UIWidget>()) 
		{
			item.ParentHasChanged();
		}

		memberList.Add(entry);
	}
}
