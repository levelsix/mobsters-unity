using UnityEngine;
using System.Collections;

public class PZMobsterCounter : MonoBehaviour {
	UIWidget widget;

	[SerializeField]
	UIWidget left;

	[SerializeField]
	UIWidget right;

	void Awake()
	{
		widget = GetComponent<UIWidget>();
	}

	void OnEnable()
	{
		widget.alpha = 0f;
	}

	public void MoveToMidPoint()
	{
		float y = transform.position.y;
		float z = transform.position.z;

		float leftEdge = left.transform.TransformPoint(new Vector3(left.transform.localPosition.x + (left.width/2), left.transform.localPosition.y, left.transform.localPosition.z)).x;

		Debug.LogError(left.gameObject.name + leftEdge);

		float rightEdge = right.transform.TransformPoint(new Vector3(right.transform.localPosition.x, right.transform.localPosition.y, right.transform.localPosition.z)).x;

		Debug.LogError(right.gameObject.name + rightEdge);
		
		Vector3 middle = new Vector3((rightEdge + leftEdge) / 2f, transform.position.y, transform.position.z);

		transform.position = new Vector3(middle.x, y, z);
	}
}
