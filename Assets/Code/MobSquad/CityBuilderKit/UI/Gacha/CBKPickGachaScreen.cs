using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKPickGachaScreen : MonoBehaviour {

	[SerializeField]
	CBKGachaBanner bannerPrefab;

	List<CBKGachaBanner> banners = new List<CBKGachaBanner>();

	[SerializeField]
	UIGrid grid;

	[SerializeField]
	CBKGachaScreen gachaScreen;

	[SerializeField]
	CBKMenuSlideButton slider;

	void OnEnable()
	{
		Init ();
	}

	void Init()
	{
		Dictionary<int, object> boosters = CBKDataManager.instance.GetAll(typeof(BoosterPackProto)) as Dictionary<int, object>;

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
		CBKGachaBanner banner = Instantiate(bannerPrefab) as CBKGachaBanner;
		banner.transform.parent = grid.transform;
		banner.transform.localScale = Vector3.one;
		banner.screen = this;
		banners.Add (banner);
	}
	
	public void ChooseBanner(BoosterPackProto pack)
	{
		slider.Slide();
		gachaScreen.Init(pack);
	}
}
