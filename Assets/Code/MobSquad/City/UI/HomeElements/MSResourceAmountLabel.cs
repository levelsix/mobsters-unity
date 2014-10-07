using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class MSResourceAmountLabel : MonoBehaviour {
	
	UILabel label;
	
	[SerializeField]
	ResourceType resource;

	[SerializeField] float time = 1;

	float speed = 1000f;

	float target = 0;

	float current = 0;

	bool needsToBeSet = true;
	
	void Awake()
	{
		label = GetComponent<UILabel>();
	}
	
	void OnEnable()
	{
		OnChangeResource(MSResourceManager.resources[resource]);
		MSActionManager.UI.OnChangeResource[resource] += OnChangeResource;
	}
	
	void OnDisable()
	{
		MSActionManager.UI.OnChangeResource[resource] -= OnChangeResource;
	}
	
	void OnChangeResource(int amount)
	{
		target = amount;
		speed = (target - current) * time;
		needsToBeSet = true;
	}

	void Update()
	{
		if (current < target)
		{
			SetAmount (Mathf.Min(target, current + speed * Time.deltaTime));
		}
		else if (current > target)
		{
			SetAmount (Mathf.Max(target, current + speed * Time.deltaTime));
		}
		else if (needsToBeSet)
		{
			SetAmount (target);
			needsToBeSet = false;
		}
	}

	void SetAmount (float amount)
	{
		current = amount;
		string formatted = String.Format ("{0:#,##0}", ((int)amount));
		label.text = formatted;
		if (resource == ResourceType.CASH) 
		{
			label.text = "$" + label.text;
		}
	}
}
