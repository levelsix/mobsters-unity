using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MSProgressBar : MonoBehaviour {
	
	public string barCap = "healingcap";
	public string barMiddle = "healingmiddle";

	[SerializeField]
	UISprite bg;
	
	[SerializeField]
	UISprite[] caps;
	
	[SerializeField]
	UISprite barSprite;
	
	[SerializeField]
	UILabel timeLabel;

	[SerializeField]
	UILabel freeLabel;
	
	[SerializeField]
	MSFillBar bar;

	/// <summary>
	/// Start Time
	/// </summary>
	public long start;
	
	long duration;

	/// <summary>
	/// End Time
	/// </summary>
	/// <value>The end.</value>
	public long end
	{
		get{
			return start + duration;
		}
	}

	public bool isActiveTimeFrame
	{
		get{
			return  MSUtil.timeNowMillis > start && MSUtil.timeNowMillis < end;
		}
	}

	Func<long> TimeLeft;

	bool canBeFree = false;
	
	/// <summary>
	/// Action that is called when the bar is 100%
	/// </summary>
	public Action BarCompleted;
	
	IEnumerator fadeRoutine;

	const int STEPS_TO_COMPLETE_FAST_LERP = 20;
	const float CYCLE_TIME = 3f;
	const float FADE_TIME = 0.3f;

	/// <summary>
	/// Disables the bar and will have to be re-initiated
	/// </summary>
	void OnDisable()
	{
		StopAllCoroutines();
	}

	/// <summary>
	/// Don't call this function, if you need the current time left, call TimeLeft()
	/// </summary>
	/// <returns>The time left.</returns>
	long DefaultTimeLeft()
	{
		return Math.Max(end - MSUtil.timeNowMillis, 0);
	}

	/// <summary>
	/// Init the progress bar where 0% is startTime and 100% at endTime
	/// </summary>
	/// <param name="startTime">The Time this bar started. (In Miliseconds)</param>
	/// <param name="duration">How long it will take for the bar to fill. (In Miliseconds)</param>
	public void init(long startTime, long duration, bool canBeFree)
	{
		start = startTime;
		TimeLeft = DefaultTimeLeft;
		this.duration = duration;
		this.canBeFree = canBeFree;
		init();
	}

	public void init(long startTime, Func<long> timeLeftfunc, bool canBeFree)
	{
		start = startTime;
		TimeLeft = timeLeftfunc;
		duration = TimeLeft() + MSUtil.timeNowMillis;
		this.canBeFree = canBeFree;
		init();
	}

	void init()
	{
		gameObject.SetActive(true);

		freeLabel.alpha = 0f;
		timeLabel.alpha = 1f;
		StopAllCoroutines();
		
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
			float fill =  1f - ((float)TimeLeft() / (float)duration);
			bar.fill = fill;


			timeLabel.text = MSUtil.TimeStringShort(TimeLeft());

			CheckFreeBar();
			yield return new WaitForEndOfFrame();
		}
		CompleteBar();
	}

	public void FastComplete()
	{
		freeLabel.alpha = 0f;
		timeLabel.alpha = 1f;
		StopAllCoroutines();
		StartCoroutine(FastLerp());
	}

	IEnumerator FastLerp()
	{
		long now = MSUtil.timeNowMillis;
		long increment = duration / (STEPS_TO_COMPLETE_FAST_LERP * (2));
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

	void CheckFreeBar()
	{
		if(MSMath.GemsForTime(end - MSUtil.timeNowMillis, canBeFree) == 0)
		{
			SetBarFree();
		}
		else
		{
			freeLabel.alpha = 0f;
			timeLabel.alpha = 1f;
			if(fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
				fadeRoutine = null;
			}
		}
	}
	
	void SetBarFree()
	{
		foreach (var item in caps)
		{
			item.spriteName = "instantcap";
		}
		barSprite.spriteName = "instantmiddle";
		
		if(fadeRoutine == null)
		{
			fadeRoutine = TextFadeAnimation();
			StartCoroutine(fadeRoutine);
		}
	}
	
	IEnumerator TextFadeAnimation()
	{
		bool showingFree = false;
		float fadeTime = 0;
		float cycleTime = 0;
		freeLabel.gameObject.SetActive(true);
		while(bg.gameObject.activeSelf)
		{
			if(cycleTime < CYCLE_TIME)
			{
				cycleTime += Time.deltaTime;
				if(showingFree)
				{
					freeLabel.alpha = 1f;
					timeLabel.alpha = 0f;
				}
				else
				{
					freeLabel.alpha = 0f;
					timeLabel.alpha = 1f;
				}
			}
			else if(fadeTime < FADE_TIME)
			{
				fadeTime += Time.deltaTime;
				if(showingFree)
				{
					timeLabel.alpha = fadeTime / FADE_TIME;
					freeLabel.alpha = 1f - fadeTime / FADE_TIME;
				}
				else
				{
					freeLabel.alpha = fadeTime / FADE_TIME;
					timeLabel.alpha = 1f - fadeTime / FADE_TIME;
				}
			}
			else
			{
				showingFree = !showingFree;
				fadeTime = 0f;
				cycleTime = 0f;
			}
			
			yield return null;
		}
		freeLabel.gameObject.SetActive(false);
		timeLabel.alpha = 1f;
		fadeRoutine = null;
	}

}
