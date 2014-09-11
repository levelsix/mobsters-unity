using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PZDestroySpecial : MonoBehaviour {

	public Action onTrigger;

	public PZGem target;

	PZGem aboveTarget;

	bool onLock = false;

	void OnEnable(){
		PZPuzzleManager.instance.specialBoardLock += 1;
		onLock = true;
	}

	void OnDisable(){
		DisableLock();
	}

	void OnTriggerEnter(Collider other)
	{
		PZGem gem = other.GetComponent<PZGem>();
		gem.SetPrefallPosition();
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

	public void DisableLock()
	{
		if(onLock)
		{
			onLock = false;
			PZPuzzleManager.instance.specialBoardLock -= 1;
		}
	}

}
