using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSDragDropLimited
/// </summary>
public class MSDragDropLimited : UIDragObject {

	public Vector2 max;

	public Vector2 min;

	Transform trans;

	[SerializeField]
	bool minOnEnable;

	void OnEnable()
	{
		if (minOnEnable)
		{
			GoToMin();
		}
	}

	protected override void OnDrag (Vector2 delta)
	{
		base.OnDrag (delta);
		transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, min.x, max.x), Mathf.Clamp(transform.localPosition.y, min.y, max.y));
	}

	public void GoToMin()
	{
		transform.localPosition = new Vector3(min.x, min.y);
	}

	public void GoToMax()
	{
		transform.localPosition = new Vector3(max.x, max.y);
	}

	[ContextMenu ("Set Max X")]
	void SetMaxX()
	{
		max.x = transform.localPosition.x;
	}
	
	[ContextMenu ("Set Max Y")]
	void SetMaxY()
	{
		max.y = transform.localPosition.y;
	}
	
	[ContextMenu ("Set Max")]
	void SetMax()
	{
		SetMaxX();
		SetMaxY();
	}
	
	[ContextMenu ("Set Min X")]
	void SetMinX()
	{
		min.x = transform.localPosition.x;
	}
	
	[ContextMenu ("Set Min Y")]
	void SetMinY()
	{
		min.y = transform.localPosition.y;
	}
	
	[ContextMenu ("Set Min")]
	void SetMin()
	{
		SetMinX();
		SetMinY();
	}

}
