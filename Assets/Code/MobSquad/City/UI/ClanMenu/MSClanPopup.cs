using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public enum ClanPopupMode {BROWSE, DETAILS, CREATE, RAIDS, HELP};

public class MSClanPopup : MonoBehaviour 
{
	[SerializeField]
	MSClanCreateScreen clanCreateScreen;

	[SerializeField]
	MSClanListScreen clanListScreen;

	[SerializeField]
	MSClanDetailScreen clanDetailScreen;

	[SerializeField]
	MSClanHelpScreen ClanHelpScreen;

	[SerializeField]
	TweenPosition mover;

	[SerializeField]
	MSClanTab leftTab;

	[SerializeField]
	MSClanTab middleTab;

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

	const float TAB_OFFSET_2 = 90f;
	const float TAB_OFFSET_3 = 180f;

	const float LIST_SCREEN_X = 0;
	const float DETAIL_SCREEN_X = 920;
	const float HELP_SCREEN_X = 1840;

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
			if(MSClanManager.instance.canHelp)
			{
				GoToMode(ClanPopupMode.HELP, true);
			}
			else
			{
				GoToMode(ClanPopupMode.DETAILS, true);
			}

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
			middleTab.gameObject.SetActive(true);

			if(MSClanManager.instance.canHelp)
			{
				rightTab.Init(ClanPopupMode.DETAILS, false);
				middleTab.Init(ClanPopupMode.HELP, true);
				leftTab.Init(ClanPopupMode.BROWSE, false);
			}
			else
			{
				rightTab.Init(ClanPopupMode.DETAILS, true);
				middleTab.Init(ClanPopupMode.HELP, false);
				leftTab.Init(ClanPopupMode.BROWSE, false);
			}

			Vector3 local = new Vector3();
			local = rightTab.transform.localPosition;
			rightTab.transform.localPosition = new Vector3(TAB_OFFSET_3, local.y, local.z);
			local = middleTab.transform.localPosition;
			middleTab.transform.localPosition = new Vector3(0f, local.y, local.z);
			local = leftTab.transform.localPosition;
			leftTab.transform.localPosition = new Vector3(-TAB_OFFSET_3, local.y, local.z);
		}
		else
		{
			leftTab.Init(ClanPopupMode.BROWSE, true);
			middleTab.gameObject.SetActive(false);
			middleTab.Init(ClanPopupMode.HELP, false);
			rightTab.Init(ClanPopupMode.CREATE, false);

			Vector3 local = new Vector3();
			local = rightTab.transform.localPosition;
			rightTab.transform.localPosition = new Vector3(TAB_OFFSET_2, local.y, local.z);
			local = leftTab.transform.localPosition;
			leftTab.transform.localPosition = new Vector3(-TAB_OFFSET_2, local.y, local.z);
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
		case ClanPopupMode.HELP:
			GoToHelp(instant);
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

		mover.to = new Vector3(-LIST_SCREEN_X, mover.to.y, mover.to.z);
		mover.ResetToBeginning();

		if (instant)
		{
			mover.Sample(1, true);
		}
		else
		{
			mover.PlayForward();
		}
	}

	void GoToDetails(int clanId, bool instant)
	{
		listAndDetailsStuff.SetActive(true);
		raidStuff.SetActive(false);
		createStuff.SetActive(false);

		clanDetailScreen.Init(clanId);

		mover.to = new Vector3(-DETAIL_SCREEN_X, mover.to.y, mover.to.z);
		mover.ResetToBeginning();
		
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

	void GoToHelp(bool instant)
	{
		listAndDetailsStuff.SetActive(true);
		raidStuff.SetActive(false);
		createStuff.SetActive(false);

		ClanHelpScreen.Init();

		mover.to = new Vector3(-HELP_SCREEN_X, mover.to.y, mover.to.z);
		mover.ResetToBeginning();
		
		if (instant)
		{
			mover.Sample(1, true);
		}
		else
		{
			mover.PlayForward();
		}

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
