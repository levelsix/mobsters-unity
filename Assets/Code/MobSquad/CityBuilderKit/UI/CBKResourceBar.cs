using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class CBKResourceBar : MonoBehaviour {

	[SerializeField]
	ResourceType resourceType;

	CBKFillBar fillBar;
	
	void Awake()
	{
		fillBar = GetComponent<CBKFillBar>();
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
		fillBar.fill = ((float)MSResourceManager.resources[(int)resourceType - 1]) / MSResourceManager.maxes[(int)resourceType - 1];
	}
}
