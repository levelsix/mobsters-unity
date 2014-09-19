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
		if(gem != null)
		{
			//this was outside of the below if statement, I don't know why but I moved it into this one to prevent null refs
			gem.SetPrefallPosition();
		}
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
