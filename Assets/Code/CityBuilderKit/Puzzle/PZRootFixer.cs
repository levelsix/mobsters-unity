using UnityEngine;
using System.Collections;

/// <summary>
/// NGUI's UI Root shrinks everything to make it into more of a "pixel space" so that it's more easily resizeable, however
/// Our particle plugin for Particle Designer needs us to use world space as pixel space in order to maintain its scale.
/// 
/// NOTE: In a default NGUI 2D UI, this needs to go between the Root and Camera, with it's pointers set to those
/// We still need UIRoot to adjust the UI 
/// </summary>
public class PZRootFixer : MonoBehaviour {

	/// <summary>
	/// The UIRoot that this is fixing pixel space to world space
	/// </summary>
	[SerializeField]
	UIRoot root;

	/// <summary>
	/// The camera for this UIRoot, which we need to adjust the size of so that everything works
	/// all peachy-keen
	/// </summary>
	[SerializeField]
	Camera cam;

	/// <summary>
	/// Set up the scales properly
	/// </summary>
	void Start () {
		float scale = 1/root.transform.localScale.x; //All aspects of localScale will be equal to each other, we just need to invert one
		transform.localScale = new Vector3(scale, scale, scale);
		cam.orthographicSize = scale; //Camera size is absolute, regardless of scale, so we need to change this along with.
	}
}
