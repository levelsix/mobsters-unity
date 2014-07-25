using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System;
using com.lvl6.proto;

public class MSSpriteUtil : MonoBehaviour {

	#region Sprite Dictionaries
	
	public static readonly Dictionary<string, string> ribbonsForLeague = new Dictionary<string, string>()
	{
		{"bronze", "bronzegoldribbon"},
		{"gold", "bronzegoldribbon"},
		{"champion", "championribbon"},
		{"diamond", "diamondribbon"},
		{"silver", "silverribbon"},
		{"platinumribbon", "platinumribbon"}
	};

	#endregion

	public static MSSpriteUtil instance;

	static Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

	[SerializeField] RuntimeAnimatorController defaultAnimator;

	[SerializeField] Sprite defaultSprite;

	const string AWS = "https://s3-us-west-1.amazonaws.com/lvl6mobsters/Resources/Android/";

	[SerializeField] bool AWS_On = false;
	
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
	
	public RuntimeAnimatorController GetAnimator(string imageName)
	{
		return (Resources.Load("Controllers/" + MSUtil.StripExtensions(imageName))) as RuntimeAnimatorController;
	}

	public RuntimeAnimatorController GetUnitAnimator(string imageName)
	{
		string path = "Bundles/" + imageName + "/" + imageName + "Controller";
		return (Resources.Load(path)) as RuntimeAnimatorController;
	}

	public IEnumerator SetSprite(string bundleName, string spriteName, SpriteRenderer sprite)
	{
		if (AWS_On)
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
		else
		{
			string path = "Bundles/" + bundleName + "/" + spriteName;
			sprite.sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
			if (sprite.sprite == null)
			{
				Debug.Log("Failed to get " + path);
			}
		}
	}

	public void SetSprite(string bundleName, string spriteName, UI2DSprite sprite)
	{
		StartCoroutine(SetSpriteCoroutine(bundleName, spriteName, sprite));
	}

	IEnumerator SetSpriteCoroutine(string bundleName, string spriteName, UI2DSprite sprite)
	{
		if (AWS_On)
		{
			sprite.alpha = 0;
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
				sprite.alpha = 1;
			}
		}
		else
		{
			string path = "Bundles/" + bundleName + "/" + spriteName;
			sprite.sprite2D = Resources.Load(path, typeof(Sprite)) as Sprite;
			if (sprite.sprite2D == null)
			{
				Debug.LogError("Failed to get " + path);
				sprite.alpha = 0;
			}
			else
			{
				sprite.alpha = 1;
				sprite.MakePixelPerfect();
			}
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
