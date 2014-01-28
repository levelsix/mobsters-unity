using UnityEngine;
using System.Collections;
using System;

public class CBKLoopingElement : MonoBehaviour {

	Transform trans;

	public float xLimit = 640;

	public Action onLoop;

	public float width;

	Vector3 offset
	{
		get
		{
			return new Vector3(xLimit*2,0);
		}
	}

	void Awake()
	{
		trans = transform;
	}

	void Update () 
	{
		while (trans.localPosition.x > xLimit + width/2)
		{
			trans.localPosition -= offset;
			if (onLoop != null)
			{
				onLoop();
			}
		}
		while (trans.localPosition.x < -xLimit + width/2)
		{
			trans.localPosition += offset;
			if (onLoop != null)
			{
				onLoop();
			}
		}
	}
}
