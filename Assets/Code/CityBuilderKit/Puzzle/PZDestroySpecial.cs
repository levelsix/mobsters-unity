using UnityEngine;
using System.Collections;

public class PZDestroySpecial : MonoBehaviour {

	public PZGem target;

	void OnTriggerEnter(Collider other)
	{
		PZGem gem = other.GetComponent<PZGem>();
		if (gem != null && gem.lockedBySpecial && (target == null || gem == target))
		{
			gem.lockedBySpecial = false;
			gem.Destroy();
		}
	}

}
