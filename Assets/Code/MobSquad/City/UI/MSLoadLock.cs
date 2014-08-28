using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Locks the UI until unlocked.
/// Can be passed a Func<bool> as a test, which will
/// automatically unlock it.
/// </summary>
public class MSLoadLock : MonoBehaviour 
{
	[SerializeField] GameObject loadingSprite;

	[SerializeField] GameObject[] disableWhileLocked;

	bool locked = false;

	Func<bool> unlockTest;

	void OnDisable()
	{
		if (locked)
		{
			Unlock();
		}
	}

	public void Lock(Func<bool> unlockTest = null)
	{
		loadingSprite.SetActive(true);
		foreach (var item in disableWhileLocked) 
		{
			item.SetActive(false);
		}

		this.unlockTest = unlockTest;
		locked = true;
		MSTutorialManager.instance.currUi = loadingSprite.gameObject;
	}

	public void Unlock()
	{
		loadingSprite.SetActive(false);
		foreach (var item in disableWhileLocked) 
		{
			item.SetActive(true);
		}

		this.unlockTest = null;
		locked = false;
		MSTutorialManager.instance.currUi = null;
	}

	void Update()
	{
		if (locked && unlockTest != null)
		{
			if (unlockTest())
			{
				Unlock();
			}
		}
	}


}
