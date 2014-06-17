using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[RequireComponent(typeof(MSTab))]
[RequireComponent(typeof(MSSimplePoolable))]
public class MSGachaTab : MonoBehaviour 
{
	MSTab tab;
	
	MSGachaScreen screen;
	
	BoosterPackProto booster;
	
	void Awake()
	{
		tab = GetComponent<MSTab>();
	}
	
	public void Init(BoosterPackProto booster, MSGachaScreen gachaScreen)
	{
		screen = gachaScreen;
		this.booster = booster;
		
		tab.label.text = booster.boosterPackName;
		//TODO: tab.spriteroot = booster.tabicon;
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
		screen.Init(booster);
	}
}