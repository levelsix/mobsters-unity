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

	[SerializeField]
	MSLoadLock loadLock;

	const string greenButton = "greensmallbutton";
	const string redButton = "redsmallbutton";

	const string joinLabel = "Join";
	const string requestLabel = "Request";
	const string leaveLabel = "Leave";

	public void Init(FullClanProtoWithClanSize clan)
	{
		this.clan = clan.clan;

		button.enabled = true;
		gameObject.SetActive (true);
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
			SetFull();
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

	void SetFull()
	{
		button.enabled = false;
		label.text = "FULL";
		mode = JoinButtonMode.DISABLED;
	}

	void SetDisabled()
	{
		gameObject.SetActive(false);
	}

	void OnClick()
	{
		switch(mode)
		{
		case JoinButtonMode.JOIN:
		case JoinButtonMode.REQUEST:
			StartCoroutine(DoJoinOrRequest());
			break;
		case JoinButtonMode.LEAVE:
			StartCoroutine(DoLeave());
			break;
		default:
			break;
		}
	}

	IEnumerator DoLeave()
	{
		if (loadLock != null) loadLock.Lock();

		yield return StartCoroutine(MSClanManager.instance.LeaveClan());

		if (loadLock != null) loadLock.Unlock();
	}

	IEnumerator DoJoinOrRequest()
	{
		if (loadLock != null) loadLock.Lock();

		if (!MSClanManager.instance.HasRequestedClan(clan.clanId))
		{
			 yield return StartCoroutine(MSClanManager.instance.JoinOrApplyToClan(clan.clanId));
		}

		if (loadLock != null) loadLock.Unlock();
	}
}
