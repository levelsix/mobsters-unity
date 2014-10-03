using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSFadeOnTap : MonoBehaviour {

	public float endAlpha = 0f;
	public float fadeDuration = 0.3f;

	/// <summary>
	/// Disable this script at the end of the fade.
	/// This is for objects that don't always want MSFadeOnTap to be active
	/// and instead only want it active in some cases.
	/// </summary>
	public bool disableObjectAtEnd = true;
	public bool disableScriptAtEnd = false;

	bool removeEvent;
	bool fading = false;

	void OnEnable()
	{
		removeEvent = false;
		StartCoroutine(WaitThenAdd());
	}

	void OnDisable()
	{
		if(removeEvent)
		{
			MSActionManager.Controls.OnAnyTap[0] -= Fade;
		}
	}

	IEnumerator WaitThenAdd()
	{
		//trying to prevent the click that initializes this from also turning it off
		yield return null;
		if(gameObject.activeSelf)
		{
			MSActionManager.Controls.OnAnyTap[0] += Fade;
			removeEvent = true;
		}
	}

	void Fade(TCKTouchData touch)
	{
		if(!fading)
		{
			fading = true;
			TweenAlpha alpha;
			alpha = TweenAlpha.Begin(gameObject, fadeDuration, endAlpha);

			List<EventDelegate> originalOnFinish = alpha.onFinished;
			float originalAlpha = GetComponent<UIWidget>().alpha;

			if(disableObjectAtEnd)
			{
				alpha.AddOnFinished(delegate { gameObject.SetActive(false); });
			}
			if(disableScriptAtEnd)
			{
				alpha.AddOnFinished(delegate { this.enabled = false; });
			}

			//revert to before we messed with the object
			alpha.AddOnFinished(delegate { fading = false; alpha.onFinished.Clear(); alpha.onFinished = originalOnFinish; GetComponent<UIWidget>().alpha = originalAlpha; });
		}
	}
}
