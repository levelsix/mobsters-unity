using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Quick and dirty component to get in-game UI objects to easily be pointed
/// in the right direction to be aligned with the camera, regardless of how their
/// root object is oriented.
/// </summary>
public class CBKPointAtCamera : MonoBehaviour {

	Transform trans;
	
	Transform mainCam;
	
	void Awake()
	{
		trans = transform;
		mainCam = Camera.main.transform;
	}
	
	public void Start()
	{
		trans.forward = mainCam.forward;
	}
}
