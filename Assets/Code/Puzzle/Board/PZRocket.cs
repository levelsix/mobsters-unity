using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(CBKSimplePoolable))]
public class PZRocket : MonoBehaviour {

	CBKValues.Direction dir;

	[SerializeField]
	float speed;

	[HideInInspector]
	public Transform trans;

	CBKSimplePoolable pool;

	void Awake()
	{
		trans = transform;
		pool = GetComponent<CBKSimplePoolable>();
	}

	public void Init(CBKValues.Direction dir)
	{
		this.dir = dir;
		CBKSoundManager.instance.PlayOneShot (CBKSoundManager.instance.rocket);
	}

	void Update()
	{
		trans.localPosition += CBKValues.dirVectors[dir] * speed * Time.deltaTime;
		if (Mathf.Abs (trans.localPosition.x) > Screen.width || Mathf.Abs(trans.localPosition.y) > Screen.height + 300)
		{
			pool.Pool();
		}
	}

}
