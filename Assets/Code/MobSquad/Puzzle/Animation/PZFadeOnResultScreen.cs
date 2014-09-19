using UnityEngine;
using System.Collections;

public class PZFadeOnResultScreen : MonoBehaviour {

	//logic below assumes this can only be set in awake
	[SerializeField]
	UIWidget widget;

	//If set to true, once the object is disabled alpha will be set back to what it started at before the tween
	[SerializeField]
	bool returnToAlphaState = true;

	float initialAlpha;

	const float TWEEN_DUR = 0.3f;

	void Awake()
	{
		if(widget == null)
		{
			widget = GetComponent<UIWidget>();
		}
	}

	void OnEnable()
	{
		if(widget != null)
		{
			MSActionManager.Puzzle.OnResultScreen += FadeOut;
			initialAlpha = widget.alpha;
		}
	}

	void OnDisable()
	{
		if(widget != null)
		{
			MSActionManager.Puzzle.OnResultScreen -= FadeOut;
			if(returnToAlphaState)
			{
				widget.alpha = initialAlpha;
			}
		}
	}

	void FadeOut()
	{
		initialAlpha = widget.alpha;
		TweenAlpha.Begin(widget.gameObject, TWEEN_DUR, 0f);
	}

	void FadeIn()
	{
		TweenAlpha.Begin(widget.gameObject, TWEEN_DUR, 1f);
	}
}
