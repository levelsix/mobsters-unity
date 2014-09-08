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

	[SerializeField]
	MSUIHelper backButton;

	ClanPopupMode currMode;

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
		backButton.TurnOff();
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

	/// <summary>
	/// Refreshs the tabs at the top of the popup.
	/// Run whenever the player joins or leaves a clan, since
	/// the setup of the buttons is different between the two.
	/// </summary>
	public void RefreshTabs()
	{
		if (MSClanManager.userClanId > 0)
		{
			rightTab.Init(ClanPopupMode.DETAILS, true);
			leftTab.Init(ClanPopupMode.BROWSE, false);
		}
		else
		{
			leftTab.Init(ClanPopupMode.BROWSE, true);
			rightTab.Init(ClanPopupMode.CREATE, false);
		}
	}

	/// <summary>
	/// Goes to mode.
	/// </summary>
	/// <param name="mode">Mode to transition to.</param>
	/// <param name="instant">If set to <c>true</c> instant. Only applies if we're going to DETAILS mode.</param>
	public void GoToMode(ClanPopupMode mode, bool instant = false)
	{
		backButton.TurnOff(); //We want to make sure the back button is off when jumping to a mode
		currMode = mode;
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

	/// <summary>
	/// Shifts to details.
	/// Called when a clan is clicked on.
	/// If the player opens the popup and is already in a clan,
	/// GoToMode will get called elsewhere with the GoToDetails being
	/// instant instead.
	/// Also, we set up the back button so that the player can
	/// get back to the list view
	/// </summary>
	/// <param name="clanId">Clan identifier.</param>
	public void ShiftToDetails(int clanId)
	{
		GoToDetails(clanId, false);
		currMode = ClanPopupMode.DETAILS;
		backButton.ResetAlpha(false);
		backButton.TurnOn();
		backButton.FadeIn();
	}

	/// <summary>
	/// Shifts the back to list.
	/// Called by the back button, assigned in the editor.
	/// Checks that the mode is details so that we don't get a
	/// weird tween if the player manages to double-tap the back
	/// button before it fades out.
	/// </summary>
	public void ShiftBackToList()
	{
		if (currMode == ClanPopupMode.DETAILS)
		{
			Debug.Log("Shifting back");
			mover.PlayReverse();
			backButton.FadeOutAndOff();
			currMode = ClanPopupMode.BROWSE;
		}
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

	void OnClanChange(int clanId, UserClanStatus clanStatus, int clanIconId)
	{
		if (clanId == 0)
		{
			GoToList(false);
		}
		else
		{
			GoToDetails(clanId, false);
		}
		RefreshTabs();
	}
}
