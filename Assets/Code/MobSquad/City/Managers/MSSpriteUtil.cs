using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System;

public class MSSpriteUtil : MonoBehaviour {

	public static MSSpriteUtil instance;

	static Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

	[SerializeField] RuntimeAnimatorController defaultAnimator;

	[SerializeField] Sprite defaultSprite;

	const string AWS = "https://s3-us-west-1.amazonaws.com/lvl6mobsters/Resources/Android/";
	
	public void Awake()
	{
		instance = this;
		Caching.CleanCache();
	}

	public Sprite GetSprite(string spritePath)
	{
		Sprite sprite = Resources.Load(spritePath, typeof(Sprite)) as Sprite;
		if (sprite == null)
		{
			Debug.LogWarning("Failed to load sprite: " + spritePath);
		}
		return sprite;
	}

	public Sprite GetMobsterSprite(string mobsterName)
	{
		Sprite mobster = (Resources.Load("Characters/HD/" + (mobsterName) + "Character", typeof(Sprite))) as Sprite;
		if (mobster == null)
		{
			Debug.LogWarning("Failed to get mobster sprite: " + mobsterName);
		}
		return mobster;
	}

	public RuntimeAnimatorController GetAnimator(string imageName)
	{
		return (Resources.Load("Controllers/" + MSUtil.StripExtensions(imageName))) as RuntimeAnimatorController;
	}

	public IEnumerator SetSprite(string bundleName, string spriteName, SpriteRenderer sprite)
	{
		if (!bundles.ContainsKey(bundleName))
		{
			sprite.sprite = defaultSprite;
			yield return StartCoroutine(DownloadAndCache(bundleName));
			
		}

		if (bundles.ContainsKey (bundleName))
	    {
			sprite.sprite = bundles[bundleName].Load(spriteName, typeof(Sprite)) as Sprite;
		}
	}

	public void SetSprite(string bundleName, string spriteName, UI2DSprite sprite)
	{
		StartCoroutine(SetSpriteCoroutine(bundleName, spriteName, sprite));
	}

	IEnumerator SetSpriteCoroutine(string bundleName, string spriteName, UI2DSprite sprite)
	{
		//Debug.Log("Setting sprite: " + spriteName);
		if (!bundles.ContainsKey(bundleName))
		{
			sprite.sprite2D = defaultSprite;
			yield return StartCoroutine(DownloadAndCache(bundleName));
			
		}

		if (bundles.ContainsKey(bundleName))
		{
			sprite.sprite2D = bundles[bundleName].Load(spriteName, typeof(Sprite)) as Sprite;
			sprite.MakePixelPerfect();
		}

	}

	public IEnumerator SetUnitAnimator(MSUnit unit)
	{
		yield return StartCoroutine(SetAnimator(unit.spriteBaseName, unit.anim));

		unit.ResetAnimation();
	}

	public IEnumerator SetAnimator(string baseName, Animator animator)
	{
		if (!bundles.ContainsKey(baseName))
		{
			animator.runtimeAnimatorController = null;
			yield return StartCoroutine(DownloadAndCache(baseName));
		}

		if (bundles.ContainsKey(baseName))
		{
			animator.runtimeAnimatorController = bundles[baseName].Load(baseName + "Controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
			animator.GetComponent<SpriteRenderer>().color = Color.white;
			//Debug.Log("Assigned it: " + animator.runtimeAnimatorController);
		}
	}

	public Sprite GetBuildingSprite(string spriteName)
	{
		return Resources.Load("Sprites/Buildings/" + MSUtil.StripExtensions(spriteName), typeof(Sprite)) as Sprite;
	}

	public bool HasBundle (string bundleName)
	{
		return bundles.ContainsKey(bundleName);
	}

	/// <summary>
	/// Downloads the and cache.
	/// Sample code from: http://docs.unity3d.com/Documentation/Manual/DownloadingAssetBundles.html
	/// </summary>
	/// <returns>The and cache.</returns>
	IEnumerator DownloadAndCache (string bundleName)
	{
		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;
		
		//Debug.Log ("Grabbing bundle: " + bundleName);
		
		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
		using(WWW www = WWW.LoadFromCacheOrDownload (AWS + bundleName + ".unity3d", 1)){
			yield return www;
			
			if (bundles.ContainsKey(bundleName))
			{
				yield break;
			}

			if (www.error != null)
				yield break;
				//throw new Exception("WWW download of " + bundleName + " had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;

			//Debug.Log("Loaded bundle: " + bundleName);

			bundles[bundleName] = bundle;
			
		} // memory is freed from the web stream (www.Dispose() gets called implicitly)
	}

	[ContextMenu ("Clean Cache")]
	void CleanCache()
	{
		Caching.CleanCache();
	}
}
