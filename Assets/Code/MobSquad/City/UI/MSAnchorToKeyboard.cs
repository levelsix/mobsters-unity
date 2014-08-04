using UnityEngine;
using System.Collections;

/// <summary>
/// MSAnchorToKeyboard
/// @author Rob Giusti
/// Anchor point that keeps itself updated to be the top-center of the mobile keyboard
/// Assumes parent anchor is the bottom center of the screen
/// </summary>
public class MSAnchorToKeyboard : MonoBehaviour 
{
	void Update()
	{
		if (TouchScreenKeyboard.visible)
		{
			transform.position = new Vector3(0, 
          		TouchScreenKeyboard.area.yMin * Screen.height/640f);
		}
		else
		{
			transform.position = Vector3.zero;
		}
	}
}
