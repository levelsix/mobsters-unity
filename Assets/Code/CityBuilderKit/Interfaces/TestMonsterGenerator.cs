using UnityEngine;
using System.Collections;

public class TestMonsterGenerator : MonoBehaviour {

	[SerializeField]
	Transform[] prefabs;
	
	int index;
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) && index < prefabs.Length)
		{
			Transform trans = Instantiate(prefabs[index++]) as Transform;
			trans.parent = transform;
		}
	}
	
}
