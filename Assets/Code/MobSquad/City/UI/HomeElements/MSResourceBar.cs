using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSResourceBar : MonoBehaviour {

	[SerializeField]
	ResourceType resourceType;

	MSFillBar fillBar;
	
	void Awake()
	{
		fillBar = GetComponent<MSFillBar>();
	}
	
	void OnEnable()
	{
		MSActionManager.UI.OnSetResourceMaxima += OnSetResourceMaxima;
		MSActionManager.UI.OnChangeResource[resourceType] += OnChangeResource;

		if (MSResourceManager.instance != null)
		{
			Reset();
		}
	}
	
	void OnDisable()
	{
		MSActionManager.UI.OnSetResourceMaxima -= OnSetResourceMaxima;
		MSActionManager.UI.OnChangeResource[resourceType] -= OnChangeResource;
	}
	
	void OnSetResourceMaxima(int[] maxes)
	{
		Reset();
	}

	void OnChangeResource(int resource)
	{
		Reset ();
	}

	void Reset()
	{
		fillBar.fill = ((float)MSResourceManager.resources[resourceType]) / MSResourceManager.maxes[(int)resourceType - 1];
	}

	/*
	void Update()
	{
		if (fillBar.fill < target)
		{
			fillBar.fill = Mathf.Max(fillBar.fill + speed * Time.deltaTime, target);
		}
		else if (fillBar.fill > target)
		{
			fillBar.fill = Mathf.Min(fillBar.fill + speed * Time.deltaTime, target);
		}
	}
	*/
}
