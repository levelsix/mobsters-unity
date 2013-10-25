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
	GameObject loaderParent;
	
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
		cityParent.SetActive(true);
		puzzleParent.SetActive(false);
	}
	
	void OnPuzzle()
	{
		cityParent.SetActive(false);
		puzzleParent.SetActive(true);
	}
}
