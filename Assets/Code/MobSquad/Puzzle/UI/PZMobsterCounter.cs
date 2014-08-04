using UnityEngine;
using System.Collections;

public class PZMobsterCounter : MonoBehaviour {
	UIWidget widget;

	void Awake()
	{
		widget = GetComponent<UIWidget>();
	}

	void OnEnable()
	{
		widget.alpha = 0f;
	}
}
