using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSProfileStats: MonoBehaviour {

	#region UI Elements

	[SerializeField]
	UISprite pvpRibbon;

	[SerializeField]
	UISprite pvpIcon;

	[SerializeField]
	UILabel pvpLeagueName;

	[SerializeField]
	UILabel pvpLeaguePlace;

	[SerializeField]
	UILabel pvpLeaguePlaceThstndrd;

	[SerializeField]
	UI2DSprite clanLogo;

	[SerializeField]
	UILabel clanName;

	[SerializeField]
	UILabel levelLabel;

	[SerializeField]
	UILabel winsLabel;

	[SerializeField]
	UI2DSprite avatarMobsterSprite;

	[SerializeField]
	Color clanTextColor = Color.blue;

	#endregion

	void Awake()
	{
		UIButton button = clanName.GetComponent<UIButton>();
		button.hover = clanTextColor;
		button.defaultColor = clanTextColor;
		button.pressed = new Color(clanTextColor.r, clanTextColor.g - 0.2f, clanTextColor.b);
	}

	public void Init(FullUserProto user)
	{
		PvpLeagueProto pvpLeague = MSDataManager.instance.Get<PvpLeagueProto>(user.pvpLeagueInfo.leagueId);

		pvpRibbon.spriteName = "prof" + MSSpriteUtil.ribbonsForLeague[pvpLeague.imgPrefix];
		pvpRibbon.MakePixelPerfect();
		pvpIcon.spriteName = pvpLeague.imgPrefix + "icon";
		pvpIcon.MakePixelPerfect();
		pvpLeagueName.text = pvpLeague.leagueName;
		pvpLeaguePlace.text = user.pvpLeagueInfo.rank.ToString();
		switch(user.pvpLeagueInfo.rank)
		{
		case 1:
			pvpLeaguePlaceThstndrd.text = "st";
			break;
		case 2:
			pvpLeaguePlaceThstndrd.text = "nd";
			break;
		case 3:
			pvpLeaguePlaceThstndrd.text = "rd";
			break;
		default:
			pvpLeaguePlaceThstndrd.text = "th";
			break;
		}

		if (user.clan != null && user.clan.clanId > 0)
		{
			MSSpriteUtil.instance.SetSprite("clanicon", "clanicon" + user.clan.clanIconId, clanLogo); 
			clanName.text = user.clan.name;
			clanName.color = clanTextColor;
			clanName.GetComponent<UIButton>().enabled = true;
		}
		else
		{
			clanLogo.width = 0;
			clanLogo.alpha = 0;
			clanName.text = "No Clan";
			clanName.color = Color.black;
			clanName.GetComponent<UIButton>().enabled = false;
		}

		levelLabel.text = user.level.ToString();
		winsLabel.text = user.pvpLeagueInfo.battlesWon.ToString();

		MonsterProto avatarMonster = MSDataManager.instance.Get<MonsterProto>(user.avatarMonsterId);
		MSSpriteUtil.instance.SetSprite(avatarMonster.imagePrefix, avatarMonster.imagePrefix + "Character", avatarMobsterSprite);
	}
}
