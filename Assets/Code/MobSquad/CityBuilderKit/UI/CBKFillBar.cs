using UnityEngine;
using System.Collections;

public class CBKFillBar : MonoBehaviour {

	UISprite bar;
	
	[SerializeField]
	int maxSize;
	
	[SerializeField]
	int minSize;
	
	public float fill
	{
		set
		{
			if (bar == null)
			{
				bar = GetComponent<UISprite>();
			}
			bar.width = Mathf.Clamp((int)((maxSize - minSize) * value + minSize), minSize, maxSize);
		}
	}
	
	void Awake()
	{
		bar = GetComponent<UISprite>();
	}
}
