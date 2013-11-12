using UnityEngine;
using System.Collections;
using System;

public class CBKResourceAmountLabel : MonoBehaviour {
	
	UILabel label;
	
	[SerializeField]
	CBKResourceManager.ResourceType resource;
	
	void Awake()
	{
		label = GetComponent<UILabel>();
	}
	
	void Start()
	{
		OnChangeResource(CBKResourceManager.resources[(int)resource]);
	}
	
	void OnEnable()
	{
		CBKEventManager.UI.OnChangeResource[(int)resource] += OnChangeResource;
	}
	
	void OnDisable()
	{
		CBKEventManager.UI.OnChangeResource[(int)resource] -= OnChangeResource;
	}
	
	void OnChangeResource(int amount)
	{
		string formatted = String.Format("{0:#,##0}", amount);
		label.text = formatted;
		if (resource == CBKResourceManager.ResourceType.FREE)
		{
			label.text = "$" + label.text;
		}
	}
}
