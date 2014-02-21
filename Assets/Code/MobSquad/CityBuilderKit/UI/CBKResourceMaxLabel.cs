using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class CBKResourceMaxLabel : MonoBehaviour {

	[SerializeField]
	ResourceType resourceType;

	UILabel maxLabel;

	void Awake()
	{
		maxLabel = GetComponent<UILabel>();
	}

	void OnEnable()
	{
		CBKEventManager.UI.OnSetResourceMaxima += OnSetResourceMaxima;
	}

	void OnDisable()
	{
		CBKEventManager.UI.OnSetResourceMaxima -= OnSetResourceMaxima;
	}

	void OnSetResourceMaxima(int[] maxes)
	{
		maxLabel.text = "Max: " + String.Format("{0:#,###,###,##0}", maxes[(int)resourceType-1]);
	}
}
