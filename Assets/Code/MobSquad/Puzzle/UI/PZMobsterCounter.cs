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

	[ContextMenu("center")]
	public void MoveToMidPoint()
	{
		float y = transform.position.y;
		float z = transform.position.z;

		Vector3 leftPos = left.transform.parent.TransformPoint(new Vector3(left.transform.localPosition.x + (left.width / 2f), left.transform.localPosition.y, left.transform.localPosition.z));
		float leftEdge = leftPos.x;
		
		Vector3 rightPos = right.transform.parent.TransformPoint(new Vector3(right.transform.localPosition.x - (right.width), right.transform.localPosition.y, right.transform.localPosition.z));
		float rightEdge = rightPos.x;
		
		Vector3 middle = new Vector3((rightEdge + leftEdge) / 2f, transform.position.y, transform.position.z);

		transform.position = new Vector3(middle.x, y, z);
	}
}
