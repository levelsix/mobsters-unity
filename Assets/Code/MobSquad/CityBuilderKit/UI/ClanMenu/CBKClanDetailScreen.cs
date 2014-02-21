using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class CBKClanDetailScreen : MonoBehaviour {

	[SerializeField]
	UISprite clanLogo;

	[SerializeField]
	UILabel clanName;

	[SerializeField]
	UILabel clanDescription;

	[SerializeField]
	UILabel foundationMembersLabel;

	[SerializeField]
	CBKActionButton joinEditButton;

	[SerializeField]
	Transform memberGrid;

	[SerializeField]
	CBKClanMemberEntry clanMemberEntryPrefab;


	List<CBKClanMemberEntry> memberList = new List<CBKClanMemberEntry>();

	FullClanProtoWithClanSize clan;


	public void Init(int clanId)
	{
		StartCoroutine(RetrieveClanValues(clanId));
	}

	IEnumerator RetrieveClanValues(int clanId)
	{
		RetrieveClanInfoRequestProto request = new RetrieveClanInfoRequestProto();
		request.sender = CBKWhiteboard.localMup;
		request.clanId = clanId;
		request.grabType = RetrieveClanInfoRequestProto.ClanInfoGrabType.ALL;
		request.isForBrowsingList = false;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RETRIEVE_CLAN_INFO_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey(tagNum))
		{
			yield return null;
		}

		RetrieveClanInfoResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetrieveClanInfoResponseProto;

		clan = response.clanInfo[0];

		clanName.text = clan.clan.name;
		clanDescription.text = clan.clan.description;

		DateTime foundationDate = (new DateTime(1970,1,1)).AddMilliseconds(clan.clan.createTime);
		foundationMembersLabel.text = "Founded: " + foundationDate.Month + "/" + foundationDate.Day + "/" + foundationDate.Year
			+ "\nMembers: " + response.members.Count + "/" + clan.clanSize;

		memberList.Clear();
		foreach (var item in response.members) 
		{
			AddMemberEntryToGrid(item);
		}
		memberGrid.GetComponent<UIGrid>().Reposition();

		if (CBKClanManager.userClanId == 0)
		{
			//TODO: Set join button up
		}
		else if (CBKClanManager.userClanId == clan.clan.clanId && CBKClanManager.isLeader)
		{
			//TODO: Set edit button up
		}
	}

	void AddMemberEntryToGrid(MinimumUserProtoForClans member)
	{
		CBKClanMemberEntry entry = CBKPoolManager.instance.Get(clanMemberEntryPrefab, Vector3.zero) as CBKClanMemberEntry;
		entry.transf.parent = memberGrid;
		entry.transf.localScale = Vector3.one;
		entry.Init(member, member.clanStatus == UserClanStatus.LEADER);

		memberList.Add(entry);
	}
}
