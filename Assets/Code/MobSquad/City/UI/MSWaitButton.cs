using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MSLoadLock))]
public class MSWaitButton : MonoBehaviour 
{

	WaitFunction func;

	MSLoadLock loadLock;

	void Awake()
	{
		loadLock = GetComponent<MSLoadLock>();
	}

	public void Init(WaitFunction func)
	{
		this.func = func;
	}

	IEnumerator Run()
	{
		loadLock.Lock();
		yield return StartCoroutine(func());
		loadLock.Unlock();
	}

	void OnClick()
	{
		if (func != null)
		{
			StartCoroutine(Run());
		}
	}
}
