//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Similar to SpringPosition, but also moves the panel's clipping. Works in local coordinates.
/// </summary>

[RequireComponent(typeof(UIPanel))]
public class MSMomentumSpringPanel : SpringPanel
{
	Vector3 lastPosition;

	/// <summary>
	/// Cache the transform.
	/// </summary>
	
	protected void Start ()
	{
		mPanel = GetComponent<UIPanel>();
		mDrag = GetComponent<UIScrollView>();
		mTrans = transform;
	}
	
	/// <summary>
	/// Advance toward the target position.
	/// </summary>
	
	void Update ()
	{
		AdvanceTowardsPosition();
	}
	
	/// <summary>
	/// Advance toward the target position.
	/// </summary>
	
	protected virtual void AdvanceTowardsPosition ()
	{
		float delta = RealTime.deltaTime;
		
		bool trigger = false;
		Vector3 before = mTrans.localPosition;
		Vector3 after = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, delta);

		float myMag = (lastPosition - mTrans.localPosition).magnitude;
//		Debug.Log(myMag + " : " + (after - target).magnitude);
		if ((after - target).sqrMagnitude < 0.01f)
		{
			after = target;
			enabled = false;
			trigger = true;
		}
		mTrans.localPosition = after;
		lastPosition = before;

		Vector3 offset = after - before;
		Vector2 cr = mPanel.clipOffset;
		cr.x -= offset.x;
		cr.y -= offset.y;
		mPanel.clipOffset = cr;
		
		if (mDrag != null) mDrag.UpdateScrollbars(false);
		
		if (trigger && onFinished != null)
		{
			current = this;
			onFinished();
			current = null;
		}
	}
	
	/// <summary>
	/// Start the tweening process.
	/// </summary>
	
	static public SpringPanel Begin (GameObject go, Vector3 pos, float strength)
	{
		SpringPanel sp = go.GetComponent<SpringPanel>();
		if (sp == null) sp = go.AddComponent<SpringPanel>();
		sp.target = pos;
		sp.strength = strength;
		sp.onFinished = null;
		sp.enabled = true;
		return sp;
	}
}
