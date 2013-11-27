using UnityEngine;
using System.Collections;
using com.lvl6.proto;


public class CBKClanListEntry : MonoBehaviour, CBKPoolable {

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

	CBKClanListEntry _prefab;

	public CBKPoolable prefab
	{
		get
		{
			return _prefab;
		}
		set
		{
			_prefab = value as CBKClanListEntry;
		}
	}

	[SerializeField]
	UISprite clanLogo;

	[SerializeField]
	UILabel clanName;

	[SerializeField]
	UILabel clanJoinTypeLabel;

	[SerializeField]
	UILabel memberCount;

	[SerializeField]
	CBKActionButton joinButton;

	[SerializeField]
	CBKActionButton selectButton;

	FullClanProtoWithClanSize clan;



	const string JOIN_BY_REQUEST_LABEL = "By request only";
	const string OPEN_JOIN_LABEL = "Anyone can join";

	const string JOIN_BUTTON_LABEL = "JOIN CLAN";
	const string REQUEST_BUTTON_LABEL = "REQUEST";
	const string CANCEL_BUTTON_LABEL = "CANCEL";

	public CBKPoolable Make(Vector3 origin)
	{
		CBKClanListEntry entry = Instantiate(this, origin, Quaternion.identity) as CBKClanListEntry;
		entry.prefab = this;
		return entry;
	}

	public void Pool()
	{
		CBKPoolManager.instance.Pool(this);
	}

	void Awake()
	{
		gameObj = gameObject;
		trans = transform;
	}

	public void Init(FullClanProtoWithClanSize clan)
	{
		this.clan = clan;

		clanName.text = clan.clan.name;

		SetupJoinButton ();


		//TODO: Set number of members
		//TODO: Set clan logo

	}

	/// <summary>
	/// Setups the join button.
	/// PREREQ: clan has been set.
	/// </summary>
	void SetupJoinButton()
	{
		if (clan.clan.requestToJoinRequired) {
			clanJoinTypeLabel.text = JOIN_BY_REQUEST_LABEL;
			joinButton.label.text = REQUEST_BUTTON_LABEL;
		}
		else {
			clanJoinTypeLabel.text = OPEN_JOIN_LABEL;
			joinButton.label.text = JOIN_BUTTON_LABEL;
		}

		if (CBKClanManager.instance.HasRequestedClan (clan.clan.clanId)) {
			joinButton.label.text = CANCEL_BUTTON_LABEL;
			joinButton.onClick = CancelJoinRequest;
		}
		else {
			joinButton.onClick = Join;
		}
	}

	public void CancelJoinRequest()
	{
		StartCoroutine(CancelRequest());
	}

	IEnumerator CancelRequest()
	{
		IEnumerator applier = CBKClanManager.instance.RetractJoinRequest(clan.clan.clanId);
		while(applier.MoveNext())
		{
			yield return applier.Current;
		}
		SetupJoinButton();
	}

	public void Join()
	{
		StartCoroutine(JoinRequest());
	}

	IEnumerator JoinRequest()
	{
		IEnumerator applier = CBKClanManager.instance.JoinOrApplyToClan(clan.clan.clanId);
		while(applier.MoveNext())
		{
			yield return applier.Current;
		}
		SetupJoinButton();
	}


}
