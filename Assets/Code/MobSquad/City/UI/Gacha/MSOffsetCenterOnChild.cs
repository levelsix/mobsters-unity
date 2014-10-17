using UnityEngine;
using System.Collections;

public class MSOffsetCenterOnChild : UICenterOnChild {

	[SerializeField]
	Vector2 offset;

	public bool momentumAffectsSpring = false;

	[SerializeField]
	float momentumWeight = 1f;

	[SerializeField]
	float minMomentum = 0.1f;

	/// <summary>
	/// Recenter the draggable list on the center-most child.
	/// </summary>

	[ContextMenu("Recenter")]
	override public void Recenter ()
	{
		if (mScrollView == null)
		{
			mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);
			
			if (mScrollView == null)
			{
				Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView) + " on a parent object in order to work", this);
				enabled = false;
				return;
			}
			else
			{
				mScrollView.onDragFinished = OnDragFinished;
				
				if (mScrollView.horizontalScrollBar != null)
					mScrollView.horizontalScrollBar.onDragFinished = OnDragFinished;
				
				if (mScrollView.verticalScrollBar != null)
					mScrollView.verticalScrollBar.onDragFinished = OnDragFinished;
			}
		}
		if (mScrollView.panel == null) return;
		
		// Calculate the panel's center in world coordinates
		Vector3[] corners = mScrollView.panel.worldCorners;
		Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;
		Vector3 offsetPanelCenter = new Vector3(panelCenter.x + offset.x, panelCenter.y + offset.y, panelCenter.z);
		
		// Offset this value by the momentum
		Vector3 pickingPoint = offsetPanelCenter - mScrollView.currentMomentum * (mScrollView.momentumAmount * 0.1f);
		if(momentumAffectsSpring && mScrollView.currentMomentum.magnitude != 0)
		{
			springStrength = Mathf.Abs (mScrollView.currentMomentum.magnitude * mScrollView.momentumAmount * momentumWeight);
			springStrength = Mathf.Max(springStrength, minMomentum);
		}
		mScrollView.currentMomentum = Vector3.zero;
		
		float min = float.MaxValue;
		Transform closest = null;
		Transform trans = transform;
		int index = 0;

		// Determine the closest child
		for (int i = 0, imax = trans.childCount; i < imax; ++i)
		{
			Transform t = trans.GetChild(i);
			float sqrDist = Vector3.SqrMagnitude(t.position - pickingPoint);
			
			if (sqrDist < min)
			{
				min = sqrDist;
				closest = t;
				index = i;
			}
		}
		
		// If we have a touch in progress and the next page threshold set
		if (nextPageThreshold > 0f && UICamera.currentTouch != null)
		{
			// If we're still on the same object
			if (mCenteredObject != null && mCenteredObject.transform == trans.GetChild(index))
			{
				Vector2 totalDelta = UICamera.currentTouch.totalDelta;
				
				float delta = 0f;
				
				switch (mScrollView.movement)
				{
				case UIScrollView.Movement.Horizontal:
				{
					delta = totalDelta.x;
					break;
				}
				case UIScrollView.Movement.Vertical:
				{
					delta = totalDelta.y;
					break;
				}
				default:
				{
					delta = totalDelta.magnitude;
					break;
				}
				}
				
				if (delta > nextPageThreshold)
				{
					// Next page
					if (index > 0)
						closest = trans.GetChild(index - 1);
				}
				else if (delta < -nextPageThreshold)
				{
					// Previous page
					if (index < trans.childCount - 1)
						closest = trans.GetChild(index + 1);
				}
			}
		}
		
		CenterOn(closest, panelCenter);
	}

	/// <summary>
	/// Center the panel on the specified target.
	/// </summary>
	
	override protected void CenterOn (Transform target, Vector3 panelCenter)
	{
		base.CenterOn(target, new Vector3(panelCenter.x + offset.x, panelCenter.y + offset.y, panelCenter.z));
	}
}
