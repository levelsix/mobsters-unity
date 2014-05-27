using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public enum ClanPopupMode {BROWSE, DETAILS, CREATE, RAIDS};

public class MSClanPopup : MonoBehaviour 
{
	[SerializeField]
	MSClanCreateScreen clanCreateScreen;

	[SerializeField]
	MSClanListScreen clanListScreen;

	[SerializeField]
	MSClanDetailScreen clanDetailScreen;

	[SerializeField]
	TweenPosition mover;

	[SerializeField]
	MSClanTab leftTab;

	[SerializeField]
	MSClanTab rightTab;

	[SerializeField]
	GameObject raidStuff;

	[SerializeField]
	GameObject createStuff;

	[SerializeField]
	GameObject listAndDetailsStuff;

	void OnEnable()
	{
		MSActionManager.Clan.OnPlayerClanChange += OnClanChange;
		Init ();
	}

	void OnDisable()
	{
		MSActionManager.Clan.OnPlayerClanChange -= OnClanChange;
	}

	void Init()
	{
		RefreshTabs();
		if (MSClanManager.userClanId > 0)
		{
			GoToMode(ClanPopupMode.DETAILS, true);
		}
		else
		{
			GoToMode(ClanPopupMode.BROWSE, true);
		}
	}

	public void RefreshTabs()
	{
		if (MSClanManager.userClanId > 0)
		{
			leftTab.Init(ClanPopupMode.DETAILS, true);
			rightTab.Init(ClanPopupMode.BROWSE, false);
		}
		else
		{
			leftTab.Init(ClanPopupMode.BROWSE, true);
			rightTab.Init(ClanPopupMode.CREATE, false);
		}
	}

	public void GoToMode(ClanPopupMode mode, bool instant = false)
	{
		switch (mode) {
		case ClanPopupMode.BROWSE:
			GoToList(instant);
			break;
		case ClanPopupMode.DETAILS:
			GoToDetails(MSClanManager.userClanId, instant);
			break;
		case ClanPopupMode.CREATE:
			GoToCreate();
			break;
		case ClanPopupMode.RAIDS:
			GoToRaids();
			break;
		default:
			break;
		}
	}

	public void ShiftToDetails(int clanId)
	{
		GoToDetails(clanId, false);
	}

	void GoToList(bool instant)
	{
		listAndDetailsStuff.SetActive(true);
		raidStuff.SetActive(false);
		createStuff.SetActive(false);

		clanListScreen.Init();

		if (instant)
		{
			mover.Sample(0, true);
		}
		else
		{
			mover.PlayReverse();
		}
	}

	void GoToDetails(int clanId, bool instant)
	{
		listAndDetailsStuff.SetActive(true);
		raidStuff.SetActive(false);
		createStuff.SetActive(false);

		clanDetailScreen.Init(clanId);

		if (instant)
		{
			mover.Sample(1, true);
		}
		else
		{
			mover.PlayForward();
		}
	}

	void GoToCreate()
	{
		createStuff.SetActive(true);
		listAndDetailsStuff.SetActive(false);
		raidStuff.SetActive(false);

		clanCreateScreen.InitCreate();
	}

	void GoToRaids()
	{
		clanDetailScreen.gameObject.SetActive(false);
		raidStuff.SetActive(true);
	}

	void OnClanChange(int clanId, UserClanStatus clanStatus)
	{
		if (clanId == 0)
		{
			GoToList(false);
		}
		else
		{
			GoToDetails(clanId, false);
		}
	}
}
