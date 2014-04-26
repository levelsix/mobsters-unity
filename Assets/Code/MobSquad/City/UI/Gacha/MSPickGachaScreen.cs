using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSPickGachaScreen : MonoBehaviour {

	[SerializeField]
	MSGachaBanner bannerPrefab;

	List<MSGachaBanner> banners = new List<MSGachaBanner>();

	[SerializeField]
	UIGrid grid;

	[SerializeField]
	MSGachaScreen gachaScreen;

	void OnEnable()
	{
		Init ();
	}

	void Init()
	{
		Dictionary<int, object> boosters = MSDataManager.instance.GetAll(typeof(BoosterPackProto)) as Dictionary<int, object>;

		int i = 0;
		foreach (var item in boosters.Values) 
		{
			while (banners.Count <= i)
			{
				AddBanner();
			}
			banners[i].Init(item as BoosterPackProto);
			i++;
		}

		for(i++;i < banners.Count;i++)
		{
			banners[i].gameObject.SetActive(false);
		}

		grid.Reposition();

	}

	void AddBanner()
	{
		MSGachaBanner banner = Instantiate(bannerPrefab) as MSGachaBanner;
		banner.transform.parent = grid.transform;
		banner.transform.localScale = Vector3.one;
		banner.screen = this;
		banners.Add (banner);
	}
	
	public void ChooseBanner(BoosterPackProto pack)
	{
		MSActionManager.Popup.OnPopup(gachaScreen.GetComponent<MSPopup>());
		gachaScreen.Init(pack);
	}
}
