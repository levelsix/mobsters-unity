using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the swap between City mode and Puzzle mode
/// </summary>
public class MSSceneManager : MonoBehaviour {

	public static MSSceneManager instance;

	[SerializeField]
	GameObject cityParent;

	[SerializeField]
	GameObject puzzleParent;

	[SerializeField]
	GameObject loadingParent;

	[SerializeField]
	UIPanel cityPanel;
	
	[SerializeField]
	UIPanel puzzlePanel;

	[SerializeField]
	UIPanel loadingPanel;

	public bool cityState = true;

	public bool loadingState = true;

	[SerializeField]
	float fadeTime = .6f;

	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		MSActionManager.Scene.OnCity += OnCity;
		MSActionManager.Scene.OnPuzzle += OnPuzzle;
	}
	
	void OnDisable()
	{
		MSActionManager.Scene.OnCity -= OnCity;
		MSActionManager.Scene.OnPuzzle -= OnPuzzle;
	}
	
	void OnCity()
	{
		if (loadingState)
		{
			StartCoroutine(FadeFromLoading());
		}
		if (!cityState)
		{
			StartCoroutine(FadeToCity());
			cityState = true;
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

	IEnumerator FadeFromLoading()
	{
		loadingState = false;
		cityParent.SetActive(true);
		puzzleParent.SetActive(false);
		yield return StartCoroutine(Fade (loadingPanel, cityPanel));
		loadingParent.SetActive(false);
	}

	IEnumerator FadeToCity()
	{
		yield return StartCoroutine(FadePuzzleBackground(false));
		puzzleParent.SetActive(false);
	}

	IEnumerator FadeToPuzzle()
	{
		puzzleParent.SetActive(true);
		yield return StartCoroutine(FadePuzzleBackground(true));
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
