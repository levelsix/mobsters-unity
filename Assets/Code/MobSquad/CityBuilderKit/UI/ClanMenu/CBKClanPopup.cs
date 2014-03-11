using UnityEngine;
using System.Collections;

public class CBKClanPopup : MonoBehaviour {

	[SerializeField]
	CBKClanCreateScreen clanCreateScreen;

	[SerializeField]
	CBKClanListScreen clanListScreen;

	[SerializeField]
	CBKClanDetailScreen clanDetailScreen;

	[SerializeField]
	GameObject raidStuff;

	[SerializeField]
	GameObject buttons;

	void OnEnable()
	{
		if (MSClanManager.userClanId > 0)
		{
			buttons.SetActive(true);
			clanDetailScreen.gameObject.SetActive(true);
			clanDetailScreen.Init(MSClanManager.userClanId);
		}
		else
		{
			buttons.SetActive(false);
			clanListScreen.gameObject.SetActive(true);
			clanListScreen.Init();
		}
	}

	public void GoToDetails()
	{
		clanDetailScreen.gameObject.SetActive(true);
		raidStuff.SetActive(false);
	}

	public void GoToRaids()
	{
		clanDetailScreen.gameObject.SetActive(false);
		raidStuff.SetActive(true);
	}
}
