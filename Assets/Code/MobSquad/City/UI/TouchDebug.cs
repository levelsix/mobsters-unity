using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// TouchDebug
/// </summary>
public class TouchDebug : MonoBehaviour {

	UILabel label;

	void Awake()
	{
		label = GetComponent<UILabel>();
	}

	void OnEnable()
	{
		MSActionManager.Controls.OnKeepDrag[0] += print;
	}

	void OnDisable()
	{
		MSActionManager.Controls.OnKeepDrag[0] -= print;
	}

	void print(TCKTouchData touch)
	{
		label.text = touch.pos.ToString();
	}
}
