using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// 
/// Ground component, which has a sprite.
/// Also, used as a check for whether a tap/click raycasts to the ground
/// images
/// </summary>
public class CBKGround : MonoBehaviour {
	
	public float widthAspect = 1;
	
	void OnEnable()
	{
		MSActionManager.Cam.OnCameraChangeOrientation += Start;	
	}
	
	void OnDisable()
	{
		MSActionManager.Cam.OnCameraChangeOrientation -= Start;
	}
	
	/// <summary>
	/// Start this instance.
	/// Makes the background mesh
	/// </summary>
	void Start ()
	{
		transform.rotation = Camera.main.transform.rotation;
	}
}
