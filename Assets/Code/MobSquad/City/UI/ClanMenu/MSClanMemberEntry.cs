using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSClanMemberEntry : MonoBehaviour, MSPoolable {

	#region Poolable

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
			if (trans == null)
			{
				trans = transform;
			}
			return trans;
		}
	}

	MSClanMemberEntry _prefab;
	public MSPoolable prefab
	{
		get
		{
			return _prefab;
		}
		set
		{
			_prefab = value as MSClanMemberEntry;
		}
	}

	public MSPoolable Make(Vector3 origin)
	{
		MSClanMemberEntry entry = Instantiate(this, origin, Quaternion.identity) as MSClanMemberEntry;
		entry.prefab = this;
		return entry;
	}

	public void Pool()
	{
		PoolButtons();
		MSPoolManager.instance.Pool(this);
	}

	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
	}

	#endregion

	[SerializeField]
	UILabel nameLabel;

	[SerializeField]
	UILabel leaderLabel;

	[SerializeField]
	MSMiniGoonBox[] teamSprites;

	[SerializeField]
	MSActionButton profileButton;

	[SerializeField]
	MSUIHelper settingsRoot;

	[SerializeField]
	UIGrid settingsGrid;

	[SerializeField]
	MSClanMemberOptionButton settingsButtonPrefab;

	[SerializeField]
	GameObject openSettingsButton;

	public MinimumUserProtoForClans clanMember;

	List<MSClanMemberOptionButton> currButtons = new List<MSClanMemberOptionButton>();

	[SerializeField]
	MSChatAvatar avatar;

	[HideInInspector]
	public MSClanDetailScreen listScreen;

	bool isFading = false;

	const string LEADER_TEXT = "Clan Leader";

	public void Init(MinimumUserProtoForClans user, UserCurrentMonsterTeamProto monsters, MSClanDetailScreen listScreen)
	{
		avatar.Init(user.minUserProtoWithLevel.minUserProto.avatarMonsterId);

		clanMember = user;
		this.listScreen = listScreen;

		//Only have the settings button open if the player is a leader or a JR leader and the other player is lower
		openSettingsButton.SetActive (user.minUserProtoWithLevel.minUserProto.userId != MSWhiteboard.localMup.userId
			&& (MSClanManager.instance.playerClan.status == UserClanStatus.LEADER
		    || (MSClanManager.instance.playerClan.status == UserClanStatus.JUNIOR_LEADER
		    && (int)user.clanStatus >= 3)));

		settingsRoot.ResetAlpha(false);
		isFading = false;
		PoolButtons();

		nameLabel.text = user.minUserProtoWithLevel.minUserProto.name;
		ResetRoleLabel();

		for (int i = 0; i < teamSprites.Length; i++) 
		{
			if (monsters != null && monsters.currentTeam.Count > i)
			{
				teamSprites[i].Init(new PZMonster(monsters.currentTeam[i]));
			}
			else
			{
				teamSprites[i].Init(null, false);
			}
		}
	}

	public void ResetRoleLabel()
	{	
		leaderLabel.text = clanMember.clanStatus.ToString();
	}

	public void OpenSettings()
	{
		switch (MSClanManager.instance.playerClan.status) 
		{
		case UserClanStatus.LEADER:
			OpenLeaderSettings();
			break;
		case UserClanStatus.JUNIOR_LEADER:
			OpenJrLeaderSettings();
			break;
		default:
			break;
		}

		settingsRoot.FadeIn();
	}

	void OpenLeaderSettings()
	{
		switch (clanMember.clanStatus) 
		{
		case UserClanStatus.JUNIOR_LEADER:
			MakeButton(OptionButtonMode.TRANSFER_LEADER, true);
			MakeButton(OptionButtonMode.CAPTAIN, false);
			MakeButton(OptionButtonMode.MEMBER, false);
			MakeButton(OptionButtonMode.KICK, false);
			break;
		case UserClanStatus.CAPTAIN:
			MakeButton(OptionButtonMode.TRANSFER_LEADER, true);
			MakeButton(OptionButtonMode.JR_LEADER, true);
			MakeButton(OptionButtonMode.MEMBER, false);
			MakeButton(OptionButtonMode.KICK, false);
			break;
		case UserClanStatus.MEMBER:
			MakeButton(OptionButtonMode.TRANSFER_LEADER, true);
			MakeButton(OptionButtonMode.JR_LEADER, true);
			MakeButton(OptionButtonMode.CAPTAIN, true);
			MakeButton(OptionButtonMode.KICK, false);
			break;
		default:
			break;
		}

		settingsGrid.Reposition();
	}

	void OpenJrLeaderSettings()
	{
		switch (clanMember.clanStatus)
		{
		case UserClanStatus.CAPTAIN:
			MakeButton(OptionButtonMode.MEMBER, false);
			MakeButton(OptionButtonMode.KICK, false);
			break;
		case UserClanStatus.MEMBER:
			MakeButton(OptionButtonMode.CAPTAIN, true);
			MakeButton(OptionButtonMode.KICK, false);
			break;
		}
		settingsGrid.Reposition();
	}

	void MakeButton(OptionButtonMode mode, bool green)
	{
		MSClanMemberOptionButton button = (MSPoolManager.instance.Get(settingsButtonPrefab.GetComponent<MSSimplePoolable>(),
		                                                             Vector3.zero,
		                                                              settingsGrid.transform) as MSSimplePoolable).GetComponent<MSClanMemberOptionButton>();
		button.transform.localScale = Vector3.one;
		currButtons.Add(button);

		button.Init(clanMember, this, mode, green);
	}

	void RecycleButtons()
	{
		foreach (var item in currButtons) 
		{
			item.Pool();
		}
		currButtons.Clear();
	}

	void OnClick()
	{
		listScreen.CloseAllOptions();
	}

	public void CloseOptions()
	{
		if (currButtons.Count > 1 && !isFading)
		{
			StartCoroutine(FadeOutOptions());
		}
	}

	void PoolButtons()
	{
		foreach (var item in currButtons) 
		{
			item.Pool();
		}
		currButtons.Clear();
	}

	IEnumerator FadeOutOptions()
	{
		isFading = true;
		TweenAlpha alph = settingsRoot.FadeOut();
		while (alph.tweenFactor < 1)
		{
			yield return null;
		}
		isFading = false;
		PoolButtons();
	}
}
