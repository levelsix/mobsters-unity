using UnityEngine;
using System.Collections;

public class CBKFillBar : MonoBehaviour {

	UISprite bar;
	
	[SerializeField]
	float maxSize;
	
	[SerializeField]
	float minSize;
	
	public float fill
	{
		set
		{
			bar.width = (int)((maxSize - minSize) * value + minSize);
		}
	}
	
	void Awake()
	{
		bar = GetComponent<UISprite>();
	}
	
}
