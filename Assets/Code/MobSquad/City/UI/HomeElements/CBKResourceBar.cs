using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKResourceBar : MonoBehaviour {

	[SerializeField]
	ResourceType resourceType;

	MSFillBar fillBar;

	[SerializeField] float speed = 1;

	float target = 0;
	
	void Awake()
	{
		fillBar = GetComponent<MSFillBar>();
	}
	
	void OnEnable()
	{
		MSActionManager.UI.OnSetResourceMaxima += OnSetResourceMaxima;
		MSActionManager.UI.OnChangeResource[(int)resourceType-1] += OnChangeResource;
	}
	
	void OnDisable()
	{
		MSActionManager.UI.OnSetResourceMaxima -= OnSetResourceMaxima;
		MSActionManager.UI.OnChangeResource[(int)resourceType-1] -= OnChangeResource;
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
		target = ((float)MSResourceManager.resources[(int)resourceType - 1]) / MSResourceManager.maxes[(int)resourceType - 1];
	}

	void Update()
	{
		if (fillBar.fill < target)
		{
			fillBar.fill = Mathf.Min(fillBar.fill + speed * Time.deltaTime, target);
		}
		else if (fillBar.fill > target)
		{
			fillBar.fill = Mathf.Max(fillBar.fill - speed * Time.deltaTime, target);
		}
	}
}
