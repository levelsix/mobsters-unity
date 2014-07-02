using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(MSSimplePoolable))]
public class PZRocket : MonoBehaviour {

	MSValues.Direction dir;

	[SerializeField]
	float speed;

	[HideInInspector]
	public Transform trans;

	MSSimplePoolable pool;

	void Awake()
	{
		trans = transform;
		pool = GetComponent<MSSimplePoolable>();
	}

	public void Init(MSValues.Direction dir)
	{
		this.dir = dir;
		MSSoundManager.instance.PlayOneShot (MSSoundManager.instance.rocket);
	}

	void Update()
	{
		trans.localPosition += MSValues.dirVectors[dir] * speed * Time.deltaTime;
		if (Mathf.Abs (trans.localPosition.x) > Screen.width/2 || Mathf.Abs(trans.localPosition.y) > Screen.height/2)
		{
			pool.Pool();
		}
	}

}
