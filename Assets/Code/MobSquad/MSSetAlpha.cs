using UnityEngine;
using System.Collections;

public class MSSetAlpha : MonoBehaviour {

	UIWidget widget;

	[SerializeField]
	float alpha = 1f;

	[SerializeField]
	bool onAwake = false;

	[SerializeField]
	bool onEnable = false;

	[SerializeField]
	bool onDisable = false;

	void Awake()
	{
		widget = GetComponent<UIWidget>();
		if(onAwake)
		{
			widget.alpha = alpha;
		}
	}

	void OnEnable()
	{
		if(onEnable)
		{
			widget.alpha = alpha;
		}
	}

	void OnDisable()
	{
		if(onDisable)
		{
			widget.alpha = alpha;
		}
	}
}
