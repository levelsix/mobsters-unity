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
		CBKEventManager.UI.OnSetResourceMaxima += OnSetResourceMaxima;
		CBKEventManager.UI.OnChangeResource[(int)resourceType-1] += OnChangeResource;
	}
	
	void OnDisable()
	{
		CBKEventManager.UI.OnSetResourceMaxima -= OnSetResourceMaxima;
		CBKEventManager.UI.OnChangeResource[(int)resourceType-1] -= OnChangeResource;
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
		fillBar.fill = ((float)CBKResourceManager.resources[(int)resourceType - 1]) / CBKResourceManager.maxes[(int)resourceType - 1];
	}
}
