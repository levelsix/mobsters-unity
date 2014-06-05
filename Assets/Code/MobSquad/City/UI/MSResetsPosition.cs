using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// A quick script to attach to any UI element
/// that needs to reset it's position every time it's enabled.
/// Useful for things like scroll views, witch we want
/// to move back to their initial position
/// every time we reopen a popup.
/// </summary>
public class MSResetsPosition : MonoBehaviour {

	Vector3 startPos;

	bool set = false;

	UIPanel panel;

	/// <summary>
	/// Awake this instance
	/// </summary>
	void Awake()
	{
		//Awake is only ever called once, so we don't have to worry about this ever
		//getting changed to something we don't want it to be
		startPos = transform.localPosition;
		set = true;
		panel = GetComponent<UIPanel>();
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		Reset();
	}

	public void Reset()
	{
		if (set)
		{
			transform.localPosition = startPos;
			if (panel != null)
			{
				panel.clipOffset = Vector3.zero;
			}
		}
	}
}
