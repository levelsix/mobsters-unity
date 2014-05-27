using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public enum OptionButtonMode {KICK, TRANSFER_LEADER, JR_LEADER, CAPTAIN, MEMBER};

/// <summary>
/// @author Rob Giusti
/// MSClanMemberOptionButton
/// </summary>
[RequireComponent (typeof (MSSimplePoolable))]
public class MSClanMemberOptionButton : MonoBehaviour {

	MinimumUserProtoForClans clanMember;

	OptionButtonMode mode;

	MSClanMemberEntry entry;

	[SerializeField]
	UIButton button;

	[SerializeField]
	UILabel label;

	public void Init(MinimumUserProtoForClans userFor, MSClanMemberEntry entry, OptionButtonMode mode, bool green)
	{
		this.entry = entry;
		clanMember = userFor;
		this.mode = mode;
		button.normalSprite = green ? "greensmallbutton" : "redsmallbutton";
		switch (mode)
		{
		case OptionButtonMode.KICK:
			label.text = "Kick From\nClan";
			break;
		case OptionButtonMode.TRANSFER_LEADER:
			label.text = "Transfer\nLeadership";
			break;
		case OptionButtonMode.MEMBER:
			label.text = "Demote to\nMember";
			break;
		case OptionButtonMode.JR_LEADER:
			label.text = "Promote to\nJr Leader";
			break;
		case OptionButtonMode.CAPTAIN:
			label.text = (green ? "Promote " : "Demote ") + "to\nCaptain";
			break;
		default:
			Debug.LogError("This should never happen. Get to da choppah!");
			break;
		}
	}

	void OnClick()
	{
		switch (mode) {
		case OptionButtonMode.KICK:
			DoKick();
			break;
		case OptionButtonMode.CAPTAIN:
			DoPromoteDemote(UserClanStatus.CAPTAIN);
			break;
		case OptionButtonMode.JR_LEADER:
			DoPromoteDemote(UserClanStatus.JUNIOR_LEADER);
			break;
		case OptionButtonMode.MEMBER:
			DoPromoteDemote(UserClanStatus.MEMBER);
			break;
		default:
			break;
		}
		entry.CloseOptions();
	}

	void DoKick()
	{
		MSClanManager.instance.BootPlayerFromClan(clanMember.minUserProtoWithLevel.minUserProto.userId);
		entry.Pool();
		entry.listScreen.memberGrid.Reposition();
	}

	void DoTransferOwner()
	{
		MSClanManager.instance.TransferClanOwnership(clanMember.minUserProtoWithLevel.minUserProto.userId);
		clanMember.clanStatus = UserClanStatus.LEADER;
		entry.ResetRoleLabel();
		MSClanManager.instance.playerClan.status = UserClanStatus.JUNIOR_LEADER;
		MSClanMemberEntry myEntry = entry.listScreen.memberList.Find(x=>x.clanMember.minUserProtoWithLevel.minUserProto.userId == MSWhiteboard.localMup.userId);
		myEntry.clanMember.clanStatus = UserClanStatus.JUNIOR_LEADER;
		myEntry.ResetRoleLabel();
	}

	void DoPromoteDemote(UserClanStatus clanStatus)
	{
		MSClanManager.instance.PromoteDemoteClanMember(clanMember.minUserProtoWithLevel.minUserProto.userId,
		                                               clanStatus);
		clanMember.clanStatus = clanStatus;
		entry.ResetRoleLabel();
	}

	public void Pool()
	{
		GetComponent<MSSimplePoolable>().Pool();
	}
}
