using UnityEngine;
using System.Collections;
using System;

public class CBKResourceMaxLabel : MonoBehaviour {

	[SerializeField]
	int resource = 0;

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

	void OnSetResourceMaxima(int cash, int oil)
	{
		maxLabel.text = "Max: " + String.Format("{0:#,###,###,##0}", (resource==0) ? cash : oil);
	}
}
