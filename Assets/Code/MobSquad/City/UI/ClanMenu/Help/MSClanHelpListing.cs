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

	public UIDragScrollView dragView;

	public int helpLength
	{
		get
		{
			return protos.Count;
		}
	}

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

		helpButton.onClick.Clear();
		EventDelegate.Add(helpButton.onClick, delegate {OnClick();});
	}

	void UpdateFields()
	{
		int numHelpers = 999;
		alreadyHelped = true;
		ClanHelpProto proto = null;
		foreach(ClanHelpProto helpProto in protos)
		{
			numHelpers = Mathf.Min(numHelpers, helpProto.helperIds.Count);

			alreadyHelped = helpProto.helperIds.Contains(MSWhiteboard.localUser.userId) && alreadyHelped;

			proto = helpProto;
			Debug.Log("updating fields : " + proto.helpType.ToString() + " user: " + proto.mup.userId + ", entry contains " + protos.Count + "protos");
			Debug.Log("already helps by : " + helpProto.helperIds.ToString());
			break;
		}

		if(proto != null)
		{
			Debug.Log("FINAL updating fields : " + proto.helpType.ToString() + " user: " + proto.mup.userId + ", entry contains " + protos.Count + "protos");

			int maxHelpers = proto.maxHelpers;
			
			helpFraction.text = numHelpers.ToString() + "/" + maxHelpers.ToString();
			
			bar.tweenToVal = false;
			bar.fill = (float)numHelpers/(float)maxHelpers;
//			bar.tweenToVal = true;

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
			
			switch(proto.helpType)
			{
			case ClanHelpType.HEAL:
				if(protos.Count > 1)
				{
					helpDescription.text = "Help me heal my " + protos.Count + " Mobsters!";
				}
				else
				{
					MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(proto.staticDataId);
					helpDescription.text = "Help me heal " + monster.displayName + "!";
				}
				break;
			case ClanHelpType.MINI_JOB:
				helpDescription.text = "Help me finish my " + ((Quality)proto.staticDataId).ToString() + " MiniJob!";
				break;
			case ClanHelpType.UPGRADE_STRUCT:
				StructureInfoProto structure = MSDataManager.instance.Get<StructureInfoProto>(proto.staticDataId);
				if(structure != null)
				{
					helpDescription.text = "Help me finish upgrading my level " + structure.level + " " + structure.name + "!";
				}
				else
				{
					helpDescription.text = "structure info was NULL, could not load";
				}
				break;
			default:
				helpDescription.text = "An un accounted for help type: " + proto.helpType.ToString();
				break;
			}

		}
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
				i--;
			}
		}

		if(protos.Count < 1)
		{
			GetComponent<MSSimplePoolable>().Pool();
		}
		else
		{
			UpdateFields();
		}
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

	public List<long> GetIdsThatCanBeHelped()
	{
		List<long> ids = new List<long>();
		foreach(ClanHelpProto proto in protos)
		{
			if(!proto.helperIds.Contains(MSWhiteboard.localMup.userId))
			{
				ids.Add(proto.clanHelpId);
			}
		}
		return ids;
	}

	public void OnClick()
	{
		helpButton.GetComponent<MSLoadLock>().Lock();

		MSClanManager.instance.DoGiveClanHelp(GetIdsThatCanBeHelped(), unlockButton);
	}

	void unlockButton()
	{
		helpButton.GetComponent<MSLoadLock>().Unlock();

		helpButton.gameObject.SetActive(false);
		helped.gameObject.SetActive(true);
	}
}
