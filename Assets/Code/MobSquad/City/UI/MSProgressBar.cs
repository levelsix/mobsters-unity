using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MSProgressBar : MonoBehaviour {

	[SerializeField]
	UISprite bg;
	
	[SerializeField]
	UISprite[] caps;
	
	[SerializeField]
	UISprite barSprite;
	
	[SerializeField]
	UILabel timeLabel;
	
	[SerializeField]
	MSFillBar bar;

	/// <summary>
	/// Start Time
	/// </summary>
	public long start;

	/// <summary>
	/// End time
	/// </summary>
	long duration;

	public long end
	{
		get{
			return start + duration;
		}
	}

	/// <summary>
	/// Action that is called when the bar is 100%
	/// </summary>
	public Action BarCompleted;

	public bool isActiveTimeFrame
	{
		get{
			return  MSUtil.timeNowMillis > start && MSUtil.timeNowMillis < end;
		}
	}

	const int STEPS_TO_COMPLETE_FAST_LERP = 20;

	/// <summary>
	/// Disables the bar and will have to be re-initiated
	/// </summary>
	void OnDisable()
	{
		StopAllCoroutines();
	}

	/// <summary>
	/// Init the progress bar where 0% is startTime and 100% at endTime
	/// </summary>
	/// <param name="startTime">The Time this bar started. (In Miliseconds)</param>
	/// <param name="duration">How long it will take for the bar to fill. (In Miliseconds)</param>
	public void init(long startTime, long duration)
	{
		start = startTime;
		this.duration = duration;
		init();
	}

	void init()
	{
		gameObject.SetActive(true);

		long now  = MSUtil.timeNowMillis;
		if(now < end)
		{
			StartCoroutine(LerpBar());
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Replaces OnUpdate logic and lerps the bar every frame
	/// </summary>
 	IEnumerator LerpBar()
	{
		while(MSUtil.timeNowMillis < end)
		{
			float fill = (float)MSUtil.timeSince(start) / (float)duration;
			bar.fill = fill;
			timeLabel.text = MSUtil.TimeStringShort(MSUtil.timeUntil(end));
			yield return new WaitForEndOfFrame();
		}
		CompleteBar();
	}

	public void FastComplete()
	{
		StopAllCoroutines();
		StartCoroutine(FastLerp());
	}

	IEnumerator FastLerp()
	{
		long now = MSUtil.timeNowMillis;
		long increment = duration / STEPS_TO_COMPLETE_FAST_LERP;
		while(now < end)
		{
			now += increment;
			bar.fill = (((float)now - (float)start) / (float)duration);
			timeLabel.text = MSUtil.TimeStringShort(Math.Max(end - now, 0));
			yield return null;
		}
		CompleteBar();
	}

	/// <summary>
	/// Called when the bar hits 100%
	/// </summary>
	void CompleteBar()
	{
		bar.fill = 1f;
		if(BarCompleted != null)
		{
			BarCompleted();
		}

		TweenAlpha.Begin(gameObject, 0.2f, 0f);
		GetComponent<TweenAlpha>().AddOnFinished(delegate {	gameObject.SetActive(false); GetComponent<UISprite>().alpha = 1f; });
	}
}
