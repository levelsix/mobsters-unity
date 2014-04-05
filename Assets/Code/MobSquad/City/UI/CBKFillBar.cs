using UnityEngine;
using System.Collections;

public class CBKFillBar : MonoBehaviour {

	UISprite bar;
	
	[SerializeField]
	int maxSize;
	
	[SerializeField]
	int minSize;

	[SerializeField]
	float v;

	public float fill
	{
		set
		{
			v = value;
			if (bar == null)
			{
				bar = GetComponent<UISprite>();
			}
			bar.width = Mathf.Clamp((int)((maxSize - minSize) * value + minSize), minSize, maxSize);
		}
		get
		{
			return v;
		}
	}
}
