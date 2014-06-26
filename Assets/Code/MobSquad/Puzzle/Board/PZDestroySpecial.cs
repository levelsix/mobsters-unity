using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PZDestroySpecial : MonoBehaviour {

	public Action onTrigger;

	public PZGem target;

	PZGem aboveTarget;

	void OnEnable(){
		PZPuzzleManager.instance.specialBoardLock += 1;
	}

	void OnDisable(){
		PZPuzzleManager.instance.specialBoardLock -= 1;
	}

	void OnTriggerEnter(Collider other)
	{
		PZGem gem = other.GetComponent<PZGem>();
		if (gem != null && gem.lockedBySpecial && (target == null || gem == target))
		{
			gem.lockedBySpecial = false;
			MSPoolManager.instance.Get(MSPrefabList.instance.orbBlowUpParticle, gem.transf.position);
			gem.Destroy();

			if (onTrigger != null) {
				onTrigger();
			}
		}

	}

}
