using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the swap between City mode and Puzzle mode
/// </summary>
public class CBKSceneManager : MonoBehaviour {

	[SerializeField]
	GameObject cityParent;

	[SerializeField]
	GameObject puzzleParent;

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
			StartCoroutine(FadeToCity());
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
			StartCoroutine(FadeToPuzzle());
			cityState = false;
		}
	}

	IEnumerator FadePuzzleBackground(bool fadeIn)
	{
		float t = 0;
		while (t < fadeTime)
		{
			t += Time.deltaTime;
			PZScrollingBackground.instance.SetAlpha((fadeIn) ? t/fadeTime : 1 - t/fadeTime);
			yield return null;
		}
	}

	IEnumerator FadeToCity()
	{
		cityParent.SetActive(true);
		StartCoroutine(FadePuzzleBackground(false));
		yield return StartCoroutine(Fade(puzzlePanel, cityPanel));
		puzzleParent.SetActive(false);
	}

	IEnumerator FadeToPuzzle()
	{
		puzzleParent.SetActive(true);
		StartCoroutine(FadePuzzleBackground(true));
		yield return StartCoroutine(Fade(cityPanel, puzzlePanel));
		cityParent.SetActive(false);
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
