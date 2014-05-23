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

	[SerializeField]
	UIButton button;

	[SerializeField]
	UILabel label;

	public void Init(OptionButtonMode mode, bool green)
	{
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
	}

	void DoKick()
	{
		MSClanManager.instance.BootPlayerFromClan(clanMember.minUserProtoWithLevel.minUserProto.userId);
	}

	void DoTransferOwner()
	{
		MSClanManager.instance.TransferClanOwnership(clanMember.minUserProtoWithLevel.minUserProto.userId);
	}

	void DoPromoteDemote(UserClanStatus clanStatus)
	{
		MSClanManager.instance.PromoteDemoteClanMember(clanMember.minUserProtoWithLevel.minUserProto.userId,
		                                               clanStatus);
	}

	public void Pool()
	{
		GetComponent<MSSimplePoolable>().Pool();
	}
}
