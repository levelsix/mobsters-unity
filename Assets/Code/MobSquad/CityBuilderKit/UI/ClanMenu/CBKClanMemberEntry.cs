using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKClanMemberEntry : MonoBehaviour, MSPoolable {

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
			return trans;
		}
	}

	CBKClanMemberEntry _prefab;
	public MSPoolable prefab
	{
		get
		{
			return _prefab;
		}
		set
		{
			_prefab = value as CBKClanMemberEntry;
		}
	}

	public MSPoolable Make(Vector3 origin)
	{
		CBKClanMemberEntry entry = Instantiate(this, origin, Quaternion.identity) as CBKClanMemberEntry;
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
	UISprite[] teamSprites;

	[SerializeField]
	CBKActionButton profileButton;

	const string LEADER_TEXT = "Clan Leader";

	public void Init(MinimumUserProtoForClans user, bool isLeader)
	{
		nameLabel.text = user.minUserProto.minUserProtoWithLevel.minUserProto.name;
		levelLabel.text = user.minUserProto.minUserProtoWithLevel.level.ToString();
		leaderLabel.text = isLeader ? LEADER_TEXT : " ";

		//TODO: Team members
	}
}
