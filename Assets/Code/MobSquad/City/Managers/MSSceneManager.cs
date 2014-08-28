using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the swap between City mode and Puzzle mode
/// </summary>
using System;


public class MSSceneManager : MonoBehaviour {

	public static MSSceneManager instance;

	[SerializeField]
	GameObject gameUIParent;

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
	float fadeTime = 1f;

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
			t += Time.deltaTime / Time.timeScale;
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
		gameUIParent.SetActive(true);
		puzzleParent.SetActive(false);
		yield return StartCoroutine(Fade (loadingPanel, false));
		loadingParent.SetActive(false);
	}

	IEnumerator FadeToCity()
	{
		cityParent.SetActive(true);
		gameUIParent.SetActive(true);
		StartCoroutine(Fade (puzzlePanel, false));
		yield return StartCoroutine(FadePuzzleBackground(false));
		puzzleParent.SetActive(false);
	}

	IEnumerator FadeToPuzzle()
	{
		puzzleParent.SetActive(true);
		if (MSTutorialManager.instance.inTutorial)
		{
			Time.timeScale = .001f;
			yield return null;
			StartCoroutine(Fade (puzzlePanel, true));
		}
		else
		{
			puzzlePanel.alpha = 1;
		}
		yield return StartCoroutine(FadePuzzleBackground(true));
		Time.timeScale = 1;
		cityParent.SetActive(false);
		gameUIParent.SetActive(false);
	}

	IEnumerator Fade (UIPanel pan, bool fadeIn)
	{
		float t = 0;
		while (t < fadeTime)
		{
			t += Time.deltaTime / Time.timeScale;
			pan.alpha = (fadeIn ? t/fadeTime : 1 - t/fadeTime);
			//Debug.Log("DT: " + Time.deltaTime + ", Fade: " + t);
			yield return null;
		}
	}

	public void ReconnectPopup()
	{
		MSPopupManager.instance.CreatePopup("Connection Problems",
		                                    "Sorry, there seem to problems between you and the server. Reconnect?",
		                                    new string[] {"Reconnect"},
		new string[] {"orangemenusprite"},
		new Action[] {delegate{MSActionManager.Popup.CloseAllPopups();Reconnect();}},
		"orange"
		);
	}

	public void Reconnect()
	{
		loadingParent.SetActive(true);
		loadingPanel.alpha = 1;
		loadingState = true;
		StartCoroutine(UMQLoader.instance.Start());
	}
}
