using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(CBKSimplePoolable))]
public class PZDestroyOnTimer : MonoBehaviour {

	[SerializeField]
	float seconds;

	//CBKSimplePoolable pool;

	void Awake()
	{
		//pool = GetComponent<CBKSimplePoolable>();
	}

	void OnEnable()
	{
		StartCoroutine(Timer());
	}

	IEnumerator Timer()
	{
		yield return new WaitForSeconds(seconds);
		//pool.Pool();
		Destroy(gameObject);
	}

}
