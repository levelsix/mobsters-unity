using UnityEngine;
using System.Collections;

public class PZDestroySpecial : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		PZGem gem = other.GetComponent<PZGem>();
		if (gem != null && gem.lockedBySpecial)
		{
			gem.lockedBySpecial = false;
			gem.Destroy();
		}
	}

}
