using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the swap between City mode and Puzzle mode
/// </summary>
public class CBKSceneManager : MonoBehaviour {
	
	[SerializeField]
	UIPanel cityPanel;
	
	[SerializeField]
	UIPanel puzzlePanel;

	bool cityState = true;

	[SerializeField]
	float fadeTime = .6f;

	[SerializeField]
	CBKSnapshot snapShot;
	
	void OnEnable()
	{
		CBKEventManager.Scene.OnCity += OnCity;
		CBKEventManager.Scene.OnPuzzle += OnPuzzle;
	}
	
	void OnDisable()
	{
		CBKEventManager.Scene.OnCity -= OnCity;
		CBKEventManager.Scene.OnPuzzle -= OnPuzzle;
	}
	
	void OnCity()
	{
		if (!cityState)
		{
			StartCoroutine(Fade(puzzlePanel, cityPanel));
			StartCoroutine(FadePuzzleBackground(false));
			cityState = true;
		}
		else
		{
			snapShot.Snap();
		}
	}
	
	void OnPuzzle()
	{
		if (cityState)
		{
			StartCoroutine(Fade(cityPanel, puzzlePanel));
			StartCoroutine(FadePuzzleBackground(true));
			cityState = false;
		}
	}

	IEnumerator FadePuzzleBackground(bool fadeIn)
	{
		float t = 0;
		while (t < fadeTime)
		{
			t += Time.deltaTime;
			foreach(SpriteRenderer sprite in PZScrollingBackground.instance.sprites)
			{
				sprite.color = Color.Lerp(new Color(1,1,1,0), Color.white, (fadeIn) ? t/fadeTime : 1 - t/fadeTime);
			}
			yield return null;
		}
	}

	IEnumerator Fade (UIPanel from, UIPanel to)
	{
		to.gameObject.SetActive(true);
		float t = 0;
		while (t < fadeTime)
		{
			t += Time.deltaTime;
			to.alpha = t/fadeTime;
			from.alpha = 1 - to.alpha;
			yield return null;
		}
		from.gameObject.SetActive(false);
	}
}
