using UnityEngine;
using System.Collections;

public class Quicktest : MonoBehaviour {
	
	[SerializeField]
	int _test = 0;
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			Debug.Log(Increment());
		}
	}
	
	public int Increment()
	{
		return _test++;
	}
}
