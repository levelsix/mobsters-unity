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
			cityState = true;
		}
	}
	
	void OnPuzzle()
	{
		if (cityState)
		{
			StartCoroutine(Fade(cityPanel, puzzlePanel));
			cityState = false;
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
