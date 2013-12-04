using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class CBKResourceAmountLabel : MonoBehaviour {
	
	UILabel label;
	
	[SerializeField]
	ResourceType resource;
	
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
		if (resource == ResourceType.CASH)
		{
			label.text = "$" + label.text;
		}
	}
}
