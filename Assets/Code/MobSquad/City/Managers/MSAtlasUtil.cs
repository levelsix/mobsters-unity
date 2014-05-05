using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System;

public class MSAtlasUtil : MonoBehaviour {
	
	[SerializeField]
	string[] xmls;

	public static MSAtlasUtil instance;
	
	static Dictionary<string, string> imgToAtlas = new Dictionary<string, string>();
	
	static Dictionary<string, UIAtlas> atlases;

	static Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

	[SerializeField] RuntimeAnimatorController defaultAnimator;

	[SerializeField] Sprite defaultSprite;

	const string AWS = "https://s3-us-west-1.amazonaws.com/lvl6mobsters/Resources/Android/";
	
	public void Awake()
	{
		instance = this;
		if (atlases == null)
		{
			atlases = new Dictionary<string, UIAtlas>();
			foreach (string item in xmls) 
			{
				//WarmAtlasDictionaryFromXML(item);
			}
		}
		Caching.CleanCache();
	}
	
	public void WarmAtlasDictionaryFromXML(string filename)
	{
		//Debug.Log("Warming: " + filename);
		TextAsset text = Resources.Load(filename) as TextAsset;
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml (text.ToString());
		XmlNodeList spriteList = xmlDoc.GetElementsByTagName("sprite");
		
		string spriteName = "";
		string atlasName = "";
		foreach (XmlNode item in spriteList) 
		{
			foreach (XmlNode node in item.ChildNodes)
			{
				if (node.Name == "name")
				{
					spriteName = node.InnerText;
				}
				else if (node.Name == "atlas")
				{
					atlasName = node.InnerText;
				}
			}
			//Debug.Log(spriteName + ", " + atlasName);
			imgToAtlas.Add(spriteName, atlasName);
		}
	}
	
	public void LoadAtlasesForSpriteNames(List<string> sprites)
	{
		foreach (string item in sprites) 
		{
			if (!imgToAtlas.ContainsKey(item))
			{
				Debug.LogError("No atlas known for: " + item);
				return;
			}
			string atlasName = imgToAtlas[MSUtil.StripExtensions(item)];
			LoadAtlas(atlasName);
		}
	}
	
	public void LoadAtlasesForSprites(List<UISprite> sprites)
	{
		foreach (UISprite item in sprites) 
		{
			SetAtlasForSprite(item);
		}
	}
	
	public void LoadAtlas(string atlasName)
	{
		if (!atlases.ContainsKey(atlasName))
		{
			UIAtlas atlas = Resources.Load(atlasName, typeof(UIAtlas)) as UIAtlas;
			atlases.Add(atlasName, atlas);
			Debug.Log("Loaded atlas: " + atlasName + ": " + (atlases[atlasName] != null ? "Yes" : "No"));
		}
	}
	
	public void SetAtlasForSprite(UISprite item)
	{
		string atlasName;
		try{
			atlasName = imgToAtlas[MSUtil.StripExtensions(item.spriteName)];
		}
		catch (KeyNotFoundException e)
		{
			Debug.LogError("Didn't find key for " + item.spriteName + ": " + e.ToString());
			item.spriteName = "Church";
			atlasName = imgToAtlas[item.spriteName];
		}
		LoadAtlas(atlasName);
		item.atlas = atlases[atlasName];
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

	public IEnumerator SetSprite(string baseName, string spriteName, SpriteRenderer sprite)
	{
		if (!bundles.ContainsKey(baseName))
		{
			sprite.sprite = defaultSprite;
			yield return StartCoroutine(DownloadAndCache(baseName));
			
		}
		sprite.sprite = bundles[baseName].Load(spriteName, typeof(Sprite)) as Sprite;
	}

	public IEnumerator SetSprite(string baseName, string spriteName, UI2DSprite sprite)
	{
		//Debug.Log("Setting sprite: " + spriteName);
		if (!bundles.ContainsKey(baseName))
		{
			sprite.sprite2D = defaultSprite;
			yield return StartCoroutine(DownloadAndCache(baseName));
			
		}

		if (bundles.ContainsKey(baseName))
		{
			sprite.sprite2D = bundles[baseName].Load(spriteName, typeof(Sprite)) as Sprite;
		}

		sprite.MakePixelPerfect();
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
	
	public void UnloadAllAtlases()
	{
		atlases.Clear();
		Resources.UnloadUnusedAssets();
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
