using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSClanIcon : MonoBehaviour {

	UIButton button;

	UI2DSprite icon;

	const string clanButtonname = "inaclanbutton";
	const string noClanButtonName = "notinaclanbutton";

	void Awake()
	{
		button = transform.parent.GetComponent<UIButton>();
		icon = GetComponent<UI2DSprite>();
		MSActionManager.Clan.OnPlayerClanChange += OnPlayerChangeClan;
		MSActionManager.Loading.OnStartup += OnStartup;
	}

	void OnDestroy()
	{
		MSActionManager.Clan.OnPlayerClanChange -= OnPlayerChangeClan;
		MSActionManager.Loading.OnStartup -= OnStartup;
	}

	void OnStartup(StartupResponseProto response)
	{
		if (response.sender == null
		    || response.sender.clan == null
		    || response.sender.clan.clanIconId == 0)
		{
			SetNoClan();
		}
		else
		{
			SetClan(response.sender.clan.clanIconId);
		}
	}

	void OnPlayerChangeClan(string clanId, UserClanStatus status, int clanIconId)
	{
		if (clanIconId > 0)
		{
			SetClan(clanIconId);
		}
		else
		{
			SetNoClan();
		}
	}

	void SetNoClan()
	{
		icon.alpha = 0;
		button.normalSprite = noClanButtonName;
	}

	void SetClan(int clanIconId)
	{
		button.normalSprite = clanButtonname;
		MSSpriteUtil.instance.SetSprite("clanicon", "clanicon" + clanIconId, icon);
	}
}
