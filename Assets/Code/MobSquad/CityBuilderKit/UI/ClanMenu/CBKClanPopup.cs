using UnityEngine;
using System.Collections;

public class CBKClanPopup : MonoBehaviour {

	[SerializeField]
	CBKClanCreateScreen clanCreateScreen;

	[SerializeField]
	CBKClanListScreen clanListScreen;

	[SerializeField]
	CBKClanDetailScreen clanDetailScreen;

	void OnEnable()
	{
		if (MSClanManager.userClanId > 0)
		{
			clanDetailScreen.gameObject.SetActive(true);
			clanDetailScreen.Init(MSClanManager.userClanId);
		}
		else
		{
			clanListScreen.gameObject.SetActive(true);
			clanListScreen.Init();
		}
	}
}
