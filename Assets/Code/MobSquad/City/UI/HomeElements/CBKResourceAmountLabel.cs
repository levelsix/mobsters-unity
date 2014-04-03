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
		OnChangeResource(MSResourceManager.resources[(int)resource-1]);
	}
	
	void OnEnable()
	{
		MSActionManager.UI.OnChangeResource[(int)resource-1] += OnChangeResource;
	}
	
	void OnDisable()
	{
		MSActionManager.UI.OnChangeResource[(int)resource-1] -= OnChangeResource;
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
