using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Quick and dirty component to get in-game UI objects to easily be pointed
/// in the right direction to be aligned with the camera, regardless of how their
/// root object is oriented.
/// </summary>
public class MSPointAtCamera : MonoBehaviour {
	
	public void Start()
	{
		transform.forward = MSGridManager.instance.cityCamera.transform.forward;
	}
}
