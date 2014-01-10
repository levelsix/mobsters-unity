using UnityEngine;
using System.Collections;

/// <summary>
/// Stretches a widget to fit the screen, while maintaining it's original aspect ratio. 
/// </summary>
[RequireComponent (typeof(UIWidget))]
public class CBKFitToScreen : MonoBehaviour {

	UIWidget obj;

	// Use this for initialization
	void Start () {
		obj = GetComponent<UIWidget>();

		float width = (float)Screen.width / obj.width;
		float height = (float)Screen.height / obj.height;

		Resize(Mathf.Min(width, height));
	}

	void Resize(float size)
	{
		//Can't just use *= because we've gotta recast it back to an int
		obj.width = (int)(obj.width * size);
		obj.height = (int)(obj.height * size);
	}
}
