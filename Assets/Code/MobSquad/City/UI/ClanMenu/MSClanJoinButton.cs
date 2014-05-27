using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

public enum JoinButtonMode {JOIN, REQUEST, LEAVE, DISABLED};

/// <summary>
/// @author Rob Giusti
/// MSClanJoinButton
/// </summary>
public class MSClanJoinButton : MonoBehaviour {

	JoinButtonMode mode = JoinButtonMode.JOIN;

	FullClanProto clan;

	[SerializeField]
	UIButton button;

	[SerializeField]
	UILabel label;

	const string greenButton = "greensmallbutton";
	const string redButton = "redsmallbutton";

	const string joinLabel = "Join";
	const string requestLabel = "Request";
	const string leaveLabel = "Leave";

	public void Init(FullClanProtoWithClanSize clan)
	{
		this.clan = clan.clan;

		button.enabled = true;
		if (MSClanManager.instance.isInClan)
		{
			if (MSClanManager.userClanId == clan.clan.clanId)
			{
				SetLeave();
			}
			else
			{
				SetDisabled();
			}
		}
		else if (clan.clanSize >= MSWhiteboard.constants.clanConstants.maxClanSize)
		{
			SetDisabled(true);
		}
		else
		{
			if (clan.clan.requestToJoinRequired)
			{
				SetRequest();
			}
			else
			{
				SetJoin();
			}
		}

	}

	void SetLeave()
	{
		button.normalSprite = redButton;
		label.text = leaveLabel;
		mode = JoinButtonMode.LEAVE;
	}

	void SetJoin()
	{
		button.normalSprite = greenButton;
		label.text = joinLabel;
		mode = JoinButtonMode.JOIN;
	}

	void SetRequest()
	{
		button.normalSprite = redButton;
		label.text = requestLabel;
		mode = JoinButtonMode.REQUEST;
	}

	void SetDisabled(bool full = false)
	{
		button.enabled = false;
		if (full) label.text = "FULL";
		else label.text = "";
		mode = JoinButtonMode.DISABLED;
	}

	void OnClick()
	{
		switch(mode)
		{
		case JoinButtonMode.JOIN:
		case JoinButtonMode.REQUEST:
			DoJoinOrRequest();
			break;
		case JoinButtonMode.LEAVE:
			DoLeave();
			break;
		default:
			break;
		}
	}

	void DoLeave()
	{
		MSClanManager.instance.LeaveClan();
	}

	void DoJoinOrRequest()
	{
		if (!MSClanManager.instance.HasRequestedClan(clan.clanId))
		{
			MSClanManager.instance.JoinOrApplyToClan(clan.clanId);
		}
	}
}
