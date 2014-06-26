using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSClanIcon : MonoBehaviour {

	UI2DSprite icon;

	const string noClanIconName = "noclanlilguys";

	void Awake()
	{
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

	void OnPlayerChangeClan(int clanId, UserClanStatus status, int clanIconId)
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
		MSSpriteUtil.instance.SetSprite("clanicon", noClanIconName, icon);
	}

	void SetClan(int clanIconId)
	{
		MSSpriteUtil.instance.SetSprite("clanicon", "clanicon" + clanIconId, icon);
	}
}
