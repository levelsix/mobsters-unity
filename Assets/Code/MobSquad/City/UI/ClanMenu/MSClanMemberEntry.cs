using UnityEngine;
using System.Collections;
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
	UILabel levelLabel;

	[SerializeField]
	UILabel clanRaidContribution;

	[SerializeField]
	MSMiniGoonBox[] teamSprites;

	[SerializeField]
	MSActionButton profileButton;

	const string LEADER_TEXT = "Clan Leader";

	public void Init(MinimumUserProtoForClans user, UserCurrentMonsterTeamProto monsters)
	{
		nameLabel.text = user.minUserProtoWithLevel.minUserProto.name;
		levelLabel.text = user.minUserProtoWithLevel.level.ToString();
		leaderLabel.text = user.clanStatus.ToString();
		clanRaidContribution.text = ((int)(user.raidContribution * 100)).ToString();

		for (int i = 0; i < teamSprites.Length; i++) 
		{
			if (monsters.currentTeam.Count > i)
			{
				teamSprites[i].Init(new PZMonster(monsters.currentTeam[i]));
			}
			else
			{
				teamSprites[i].Init(null, false);
			}
		}
	}
}
