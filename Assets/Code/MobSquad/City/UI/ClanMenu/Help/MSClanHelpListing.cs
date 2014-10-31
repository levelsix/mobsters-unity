using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSClanHelpListing : MonoBehaviour {

	[SerializeField] MSChatAvatar avatar;

	[SerializeField] UILabel playerName;

	[SerializeField] UILabel helpDescription;

	[SerializeField] MSFillBar bar;

	[SerializeField] UILabel helpFraction;

	[SerializeField] UISprite helped;

	[SerializeField] UIButton helpButton;

	List<ClanHelpProto> protos = new List<ClanHelpProto>();

	bool alreadyHelped = false;

	public void Init(ClanHelpProto proto)
	{
		Init(new List<ClanHelpProto>{proto});
	}

	//for some reason I thought this might get initialized with a list...  Maybe ONE DAY
	public void Init(List<ClanHelpProto> protoList)
	{
		protos.Clear();
		protos = protoList;

		UpdateFields();
	}

	void UpdateFields()
	{
		ClanHelpProto proto = protos[0];
		
		int numHelpers = proto.helperIds.Count;
		int maxHelpers = proto.maxHelpers;
		
		helpFraction.text = numHelpers.ToString() + "/" + maxHelpers.ToString();
		
		bar.tweenToVal = false;
		bar.max = maxHelpers;
		bar.fill = numHelpers;
		bar.tweenToVal = true;
		
		alreadyHelped = proto.helperIds.Contains(MSWhiteboard.localUser.userId);
		if(!alreadyHelped)
		{
			helpButton.gameObject.SetActive(true);
			helped.gameObject.SetActive(false);
		}
		else
		{
			helpButton.gameObject.SetActive(false);
			helped.gameObject.SetActive(true);
		}

		avatar.Init(proto.mup.avatarMonsterId);
		playerName.text = proto.mup.name;

		//TODO: add descriptions
	}

	public void UpdateListing(ClanHelpProto update)
	{
		for(int i = 0; i < protos.Count; i ++)
		{
			if(protos[i].clanHelpId == update.clanHelpId)
			{
				protos[i] = update;
				UpdateFields();
				return;
			}
		}

		Debug.LogError("Trying to update listing, but couldn't find correct clanHelpId");
	}

	public bool AddHealingProto(ClanHelpProto proto)
	{
		if(proto.helpType != ClanHelpType.HEAL)
		{
			Debug.LogError("Don't add non Healing ClanHelpProtos to Healing Listings");
			return false;
		}

		if(protos.Count > 0 && protos[0].helpType == ClanHelpType.HEAL)
		{
			protos.Add(proto);
			UpdateFields();
			return true;
		}

		return false;
	}

	public void RemoveClanHelp(List<long> ids)
	{
		for(int i = 0; i < protos.Count; i ++)
		{
			if(ids.Contains(protos[i].clanHelpId))
			{
				protos.Remove(protos[i]);
			}
		}

		UpdateFields();
	}

	public bool Contains(ClanHelpProto proto)
	{
		foreach(ClanHelpProto help in protos)
		{
			if(help.clanHelpId == proto.clanHelpId)
			{
				return true;
			}
		}

		return false;
	}

	public void OnClick()
	{
		helpButton.GetComponent<MSLoadLock>().Lock();

		//TODO: add send help info

		unlockButton();
	}

	void unlockButton()
	{
		helpButton.GetComponent<MSLoadLock>().Unlock();
	}
}
