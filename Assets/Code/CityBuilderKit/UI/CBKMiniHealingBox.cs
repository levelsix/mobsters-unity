using UnityEngine;
using System.Collections;

public class CBKMiniHealingBox : MonoBehaviour {

	[SerializeField]
	UISprite goonPortrait;
	
	[SerializeField]
	UISprite bar;
	
	[SerializeField]
	UILabel time;
	
	PZMonster monster;
	
	public void Init(PZMonster monster)
	{
		this.monster = monster;
	}
	
	void Update()
	{
		
	}
}
