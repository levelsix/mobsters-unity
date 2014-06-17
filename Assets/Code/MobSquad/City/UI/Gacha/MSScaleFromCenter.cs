using UnityEngine;
using System.Collections;

/// <summary>
/// MSScaleFromCenter
/// @author Rob Giusti
/// Script that determines this object's distance from the center of a panel, and makes the
/// root alpha and scale lower when further from the center.
/// Useful for scrolling panels where we want a centered item to "pop" more, such as Gacha
/// featured mobsters
/// </summary>
public class MSScaleFromCenter : MonoBehaviour {
	
	[SerializeField]
	UIWidget root;
	
	[SerializeField] float scaleDist = 100;
	
	[SerializeField] float minScale = .7f;
	
	[SerializeField] float minAlpha = .7f;
	
	[SerializeField] float panelCenter = -151.5f;
	
	void Update () 
	{
		float dist = Mathf.Abs(panelCenter - (transform.localPosition.x + transform.parent.localPosition.x));
		if (dist > scaleDist)
		{
			dist = 0;
		}
		else if (dist == 0)
		{
			dist = 1;
		}
		else
		{
			dist = 1 - (dist / scaleDist);
		}
		root.alpha = minAlpha + (1 - minAlpha) * dist;
		transform.localScale = Vector3.one * (minScale + (1 - minScale) * dist);
	}
}