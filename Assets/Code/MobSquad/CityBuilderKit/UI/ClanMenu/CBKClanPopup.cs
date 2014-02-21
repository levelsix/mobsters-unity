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
		if (CBKClanManager.userClanId > 0)
		{
			clanDetailScreen.gameObject.SetActive(true);
			clanDetailScreen.Init(CBKClanManager.userClanId);
		}
		else
		{
			clanListScreen.gameObject.SetActive(true);
			clanListScreen.Init();
		}
	}
}
