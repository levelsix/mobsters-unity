using UnityEngine;
using System.Collections;

public class MSFillBar : MonoBehaviour {

	UISprite bar;
	
	[SerializeField]
	int maxSize;
	
	[SerializeField]
	int minSize;

	[SerializeField]
	float currVal;

	public float fill
	{
		set
		{
			currVal = value;
			if (bar == null)
			{
				bar = GetComponent<UISprite>();
			}
			bar.width = Mathf.Clamp((int)((maxSize - minSize) * value + minSize), minSize, maxSize);
			bar.alpha = (value <= 0) ? 0 : 1;
		}
		get
		{
			return currVal;
		}
	}

	public int max
	{
		set
		{
			maxSize = value;
			fill = currVal;
		}
	}
}
