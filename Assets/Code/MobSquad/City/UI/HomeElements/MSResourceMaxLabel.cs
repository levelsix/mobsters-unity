using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class MSResourceMaxLabel : MonoBehaviour {

	[SerializeField]
	ResourceType resourceType;

	UILabel maxLabel;

	void Awake()
	{
		maxLabel = GetComponent<UILabel>();
	}

	void OnEnable()
	{
		MSActionManager.UI.OnSetResourceMaxima += OnSetResourceMaxima;
	}

	void OnDisable()
	{
		MSActionManager.UI.OnSetResourceMaxima -= OnSetResourceMaxima;
	}

	void OnSetResourceMaxima(int[] maxes)
	{
		maxLabel.text = "Max: " + String.Format("{0:#,###,###,##0}", maxes[(int)resourceType-1]);
	}
}
