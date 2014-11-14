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

	[SerializeField]
	public bool tweenToVal = false;

	[SerializeField]
	float speed = .5f;

	/// <summary>
	/// The width guide.
	/// If this is set, this bar will set
	/// maxSize = widthGuide.width + widthAdjustment
	/// </summary>
	[SerializeField]
	UIWidget widthGuide;

	[SerializeField]
	int widthAdjustment;

	float targetVal;

	[HideInInspector]
	public bool isVisible
	{
		get
		{
			return gameObject.activeSelf && bar.alpha >= 0f;
		}
	}

	public float fill
	{
		set
		{
			if (tweenToVal)
			{
				targetVal = value;
			}
			else
			{
				SetFill (value);
			}
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
		get
		{
			return maxSize;
		}
	}

	void Start()
	{
		if (widthGuide != null)
		{
			maxSize = widthGuide.width + widthAdjustment;
		}
	}

	void SetFill(float val)
	{
		currVal = val;
		if (bar == null)
		{
			bar = GetComponent<UISprite>();
		}
		bar.width = Mathf.Clamp((int)((maxSize - minSize) * val + minSize), minSize, maxSize);
		bar.alpha = (val <= 0) ? 0 : 1;
	}

	void Update()
	{
		if (tweenToVal)
		{
			if (currVal > targetVal)
			{
				SetFill(Mathf.Max(targetVal, currVal - speed * Time.deltaTime));
			}
			else if (currVal < targetVal)
			{
				SetFill(Mathf.Min(targetVal, currVal + speed * Time.deltaTime));
			}
		}
	}
}
