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
			PZCombatManager.instance.activePlayer.unit.sprite.color = new Color(1,1,1,(fadeIn) ? t/fadeTime : 1 - t/fadeTime);
			PZCombatManager.instance.activeEnemy.unit.sprite.color = new Color(1,1,1,(fadeIn) ? t/fadeTime : 1 - t/fadeTime);
			yield return null;
		}
	}

	IEnumerator FadeFromLoading()
	{
		loadingState = false;
		cityParent.SetActive(true);
		puzzleParent.SetActive(false);
		yield return StartCoroutine(Fade (loadingPanel, false));
		loadingParent.SetActive(false);
	}

	IEnumerator FadeToCity()
	{
		cityParent.SetActive(true);
		StartCoroutine(Fade (puzzlePanel, false));
		yield return StartCoroutine(FadePuzzleBackground(false));
		puzzleParent.SetActive(false);
	}

	IEnumerator FadeToPuzzle()
	{
		puzzleParent.SetActive(true);
		puzzlePanel.alpha = 1;
		yield return StartCoroutine(FadePuzzleBackground(true));
		cityParent.SetActive(false);
#if UNITY_IPHONE || UNITY_ANDROID
		Kamcord.StartRecording();
		Debug.Log("Started Recording");
#endif
	}

	IEnumerator Fade (UIPanel pan, bool fadeIn)
	{
		float t = 0;
		while (t < fadeTime)
		{
			t += Time.deltaTime;
			pan.alpha = (fadeIn ? t/fadeTime : 1 - t/fadeTime);
			yield return null;
		}
	}
}
