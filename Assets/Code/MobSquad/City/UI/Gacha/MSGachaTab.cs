using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[RequireComponent(typeof(MSTab))]
[RequireComponent(typeof(MSSimplePoolable))]
public class MSGachaTab : MonoBehaviour 
{
	[SerializeField]
	MSTab otherTab;

	MSTab tab;
	
	MSGachaScreen screen;
	
	BoosterPackProto booster;
	
	public void Init(BoosterPackProto booster, MSGachaScreen gachaScreen)
	{
		tab = GetComponent<MSTab>();

		screen = gachaScreen;
		this.booster = booster;
		
		tab.label.text = booster.boosterPackName;
	}
	
	public void OnNewBoosterActive(BoosterPackProto booster)
	{
		if (booster == this.booster)
		{
			tab.InitActive();
		}
		else
		{
			tab.InitInactive();
		}
	}
	
	void OnClick()
	{
		if(!tab.active)
		{
			screen.Init(booster);
			tab.InitActive();
			otherTab.InitInactive();
		}
	}
}