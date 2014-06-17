using UnityEngine;
using System.Collections;
using System;

public class MSLoopingElement : MonoBehaviour {
	
	Transform trans;
	
	/// <summary>
	/// If we're within a scroll panel, we might need to apply the movement of the parent
	/// in order to get the "actual" position of this element
	/// </summary>
	public bool useParent;
	
	public float xLimit = 640;
	
	public Action<bool> onLoop;
	
	public float width;
	
	public float parentOffset = 0;
	
	float myX
	{
		get
		{
			if (useParent)
			{
				return trans.localPosition.x + trans.parent.localPosition.x - parentOffset;
			}
			else
			{
				return trans.localPosition.x;
			}
		}
	}
	
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
		while (myX > xLimit + width/2)
		{
			trans.localPosition -= offset;
			if (onLoop != null)
			{
				onLoop(false);
			}
		}
		while (myX < -xLimit + width/2)
		{
			trans.localPosition += offset;
			if (onLoop != null)
			{
				onLoop(true);
			}
		}
	}
}