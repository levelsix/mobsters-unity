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

	UISprite sprite;

	void Awake()
	{
		trans = transform;
		pool = GetComponent<MSSimplePoolable>();
		sprite = GetComponent<UISprite>();
	}

	public void Init(MSValues.Direction dir)
	{
		this.dir = dir;
		MSSoundManager.instance.PlayOneShot (MSSoundManager.instance.rocket);
	}

	void Update()
	{
		trans.localPosition += MSValues.dirVectors[dir] * speed * Time.deltaTime;
		if (!sprite.isVisible)
		{
			pool.Pool();
		}
	}

}
