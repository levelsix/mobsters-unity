using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class PZDeployPopup : MonoBehaviour {

	[SerializeField]
	PZDeployCard[] slots;
	
	const int NUM_SLOTS = 3;
	
	public void Init(PZMonster[] userMonsters)
	{
		int i;
		for (i = 0; i < userMonsters.Length; i++) 
		{
			if (userMonsters[i] != null && userMonsters[i].monster != null && userMonsters[i].monster.monsterId > 0)
			{
				slots[i].Init(userMonsters[i]);
			}
			else
			{
				slots[i].InitEmpty();
			}
		}
	}
}
