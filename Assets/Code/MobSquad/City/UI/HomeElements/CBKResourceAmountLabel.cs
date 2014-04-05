using UnityEngine;
using System.Collections;
using System;
using com.lvl6.proto;

public class CBKResourceAmountLabel : MonoBehaviour {
	
	UILabel label;
	
	[SerializeField]
	ResourceType resource;

	[SerializeField] float time = 1;

	float speed = 1000f;

	[SerializeField] int target;

	int current = 0;
	
	void Awake()
	{
		label = GetComponent<UILabel>();
	}
	
	void OnEnable()
	{
		SetAmount(MSResourceManager.resources[(int)resource-1]);
		target = current;
		MSActionManager.UI.OnChangeResource[(int)resource-1] += OnChangeResource;
	}
	
	void OnDisable()
	{
		MSActionManager.UI.OnChangeResource[(int)resource-1] -= OnChangeResource;
	}
	
	void OnChangeResource(int amount)
	{
		target = amount;
		speed = (target - current) * time;
	}

	void Update()
	{
		if (current < target)
		{
			SetAmount (Mathf.Min(target, Mathf.FloorToInt(current + speed * Time.deltaTime)));
		}
		else if (current > target)
		{
			SetAmount (Mathf.Max(target, Mathf.CeilToInt(current + speed * Time.deltaTime)));
		}
	}

	void SetAmount (int amount)
	{
		current = amount;
		string formatted = String.Format ("{0:#,##0}", amount);
		label.text = formatted;
		if (resource == ResourceType.CASH) 
		{
			label.text = "$" + label.text;
		}
	}
}
