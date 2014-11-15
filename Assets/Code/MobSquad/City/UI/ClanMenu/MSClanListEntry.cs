using UnityEngine;
using System.Collections;
using com.lvl6.proto;


public class MSClanListEntry : MonoBehaviour, MSPoolable {

	GameObject gameObj;
	Transform trans;

	public GameObject gObj
	{
		get
		{
			return gameObj;
		}
	}

	public Transform transf
	{
		get
		{
			return trans;
		}
	}

	MSClanListEntry _prefab;

	public MSPoolable prefab
	{
		get
		{
			return _prefab;
		}
		set
		{
			_prefab = value as MSClanListEntry;
		}
	}

	[SerializeField]
	UI2DSprite clanLogo;

	[SerializeField]
	UILabel clanName;

	[SerializeField]
	UILabel clanJoinTypeLabel;

	[SerializeField]
	UILabel memberCount;

	[SerializeField]
	MSClanJoinButton joinButton;

	FullClanProtoWithClanSize clan;

	MSClanPopup clanPopup;

	public MSPoolable Make(Vector3 origin)
	{
		MSClanListEntry entry = Instantiate(this, origin, Quaternion.identity) as MSClanListEntry;
		entry.prefab = this;
		return entry;
	}

	public void Pool()
	{
		MSPoolManager.instance.Pool(this);
	}

	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
	}

	public void Init(FullClanProtoWithClanSize clan, MSClanPopup clanPopup)
	{
		this.clan = clan;

		this.clanPopup = clanPopup;

		clanName.text = clan.clan.name;

		joinButton.Init(clan);

		memberCount.text = clan.clanSize + "/" + MSWhiteboard.constants.clanConstants.maxClanSize;

		//TODO: Set clan logo
		MSSpriteUtil.instance.SetSprite("clanicon", "clanicon" + clan.clan.clanIconId, clanLogo);

	}

	void OnClick()
	{
		clanPopup.ShiftToDetails(clan.clan.clanUuid);
	}

}
