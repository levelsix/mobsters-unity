using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Sprite that fades out when the buildings have been loaded
/// </summary>
[RequireComponent (typeof(UISprite))]
public class CBKSplashSprite : MonoBehaviour {
	
	/// <summary>
	/// The sprite.
	/// </summary>
	UISprite sprite;
	
	/// <summary>
	/// The game object.
	/// </summary>
	GameObject gameObj;
	
	/// <summary>
	/// The fade-out time
	/// </summary>
	public float FADE_OUT_TIME = 1f;
	
	/// <summary>
	/// Get component references
	/// </summary>
	void Awake()
	{
		sprite = GetComponent<UISprite>();
		gameObj = gameObject;
	}
	
	/// <summary>
	/// Establish event delegates
	/// </summary>
	void OnEnable()
	{
		CBKEventManager.Loading.OnBuildingsLoaded += OnBuildingsLoaded;
	}
	
	/// <summary>
	/// Release event delegates
	/// </summary>
	void OnDisable()
	{
		CBKEventManager.Loading.OnBuildingsLoaded -= OnBuildingsLoaded;
	}
	
	/// <summary>
	/// When buildings are loaded, starts the fade
	/// </summary>
	void OnBuildingsLoaded()
	{
		StartCoroutine(FadeOut());
	}
	
	/// <summary>
	/// Fades out the sprite over time, then disables the game object.
	/// </summary>
	IEnumerator FadeOut()
	{
		float time = 0;
		while (time < FADE_OUT_TIME)
		{
			time += Time.deltaTime;
			sprite.alpha = (FADE_OUT_TIME - time) / FADE_OUT_TIME;
			yield return null;
		}
		gameObj.SetActive(false);
	}
	
}
