using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class PZDeployPopup : MonoBehaviour {

	[SerializeField]
	PZDeployCard[] slots;
	
	const int NUM_SLOTS = 3;
	
	public void Init(List<PZMonster> userMonsters)
	{
		int i;
		for (i = 0; i < userMonsters.Count; i++) 
		{
			slots[i].Init(userMonsters[i]);
		}
		if (i < NUM_SLOTS)
		{
			for (; i < NUM_SLOTS; i++) 
			{
				slots[i].InitEmpty();
			}
		}
	}
}
