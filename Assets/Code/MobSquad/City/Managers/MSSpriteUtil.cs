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
		{"silver", "silverplatinumribbon"},
		{"platinum", "silverplatinumribbon"}
	};

	#endregion

	public BundleAtlas[] immediateBundles;

	public static MSSpriteUtil instance;

	static Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

	[SerializeField] RuntimeAnimatorController defaultAnimator;

	[SerializeField] Sprite defaultSprite;

	public HashSet<string> internalBundles = new HashSet<string>();

	public List<string> internalBundleNames;

	const string AWS = "https://s3-us-west-1.amazonaws.com/lvl6mobsters/Android/";
	
	public void Awake()
	{
		instance = this;

		BuildInternalBundleList();
	}

	void BuildInternalBundleList()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		foreach (var item in internalBundleNames) {
			internalBundles.Add(item);
		}
#else
		DirectoryInfo[] dirs = (new DirectoryInfo(Application.dataPath + "/Resources/Bundles")).GetDirectories();
		string str = "Internal Bundles:";
		foreach (var item in dirs) 
		{
			internalBundles.Add(item.Name);
			str += "\n" + item.Name;
		}
		Debug.Log(str);
#endif
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

	public void SetSprite(string bundleName, string spriteName, SpriteRenderer sprite)
	{
		StartCoroutine(SetSpriteCoroutine(bundleName, spriteName, sprite));
	}

	public IEnumerator SetSpriteCoroutine(string bundleName, string spriteName, SpriteRenderer sprite)
	{
		if (internalBundles.Contains(bundleName))
		{
			string path = "Bundles/" + bundleName + "/" + spriteName;
			sprite.sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
			if (sprite.sprite == null)
			{
				Debug.Log("Failed to get " + path);
			}
		}
		else
		{
			if (!bundles.ContainsKey(bundleName))
			{
				sprite.sprite = defaultSprite;
				yield return StartCoroutine(DownloadAndCache(bundleName));
			}

			while (bundles[bundleName] == null) yield return null;
			
			if (bundles.ContainsKey (bundleName))
			{
				sprite.sprite = bundles[bundleName].Load(spriteName, typeof(Sprite)) as Sprite;
			}
		}
	}

	public void SetSprite(string bundleName, string spriteName, UI2DSprite sprite, float finalAlpha = 1f, Action after = null)
	{
		StartCoroutine(SetSpriteCoroutine(bundleName, spriteName, sprite, finalAlpha, after));
	}

	IEnumerator SetSpriteCoroutine(string bundleName, string spriteName, UI2DSprite sprite, float finalAlpha, Action after = null)
	{
		if (internalBundles.Contains(bundleName))
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
				sprite.alpha = finalAlpha;
				sprite.MakePixelPerfect();
			}
		}
		else
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
				while (bundles[bundleName] == null)
				{
					yield return null;
				}
				
				sprite.sprite2D = bundles[bundleName].Load(spriteName, typeof(Sprite)) as Sprite;
				sprite.MakePixelPerfect();
				sprite.alpha = finalAlpha;
			}
		}

		if (after != null)
		{
			after();
		}
	}

	public void RunForEachTypeInBuncle<T>(string bundleName, Action<T> ForEach, Action OnFinish = null) where T :class
	{
		StartCoroutine(ForEachTypeInBundle<T>(bundleName, ForEach, OnFinish));
	}

	IEnumerator ForEachTypeInBundle<T>(string bundleName, Action<T> ForEach, Action OnFinish) where T : class
	{
		if(internalBundles.Contains(bundleName))
		{
			UnityEngine.Object[] entireBundleOfTypeT = Resources.LoadAll("Bundles/"+bundleName, typeof(T));
			
			IComparer<UnityEngine.Object> comparer = new MSNaturalSortObject();
			Array.Sort<UnityEngine.Object>(entireBundleOfTypeT, comparer);
			
			foreach(UnityEngine.Object bundleItem in entireBundleOfTypeT)
			{
				ForEach(bundleItem as T);
			}
		}
		else
		{
			if (!bundles.ContainsKey(bundleName))
			{
				yield return StartCoroutine(DownloadAndCache(bundleName));
			}
			
			if (bundles.ContainsKey(bundleName))
			{
				UnityEngine.Object[] entireBundleOfTypeT = bundles[bundleName].LoadAll(typeof(T));
				
				IComparer<UnityEngine.Object> comparer = new MSNaturalSortObject();
				Array.Sort<UnityEngine.Object>(entireBundleOfTypeT, comparer);
				
				foreach(UnityEngine.Object bundleItem in entireBundleOfTypeT)
				{
					ForEach(bundleItem as T);
				}
			}
		}

		if(OnFinish != null)
		{
			OnFinish();
		}

	}

	public IEnumerator SetUnitAnimator(MSUnit unit)
	{
		yield return StartCoroutine(SetAnimator(unit.spriteBaseName, unit.anim, "Controller"));

		unit.ResetAnimation();
	}

	public IEnumerator SetBuildingAnimator(MSBuilding building, string structName)
	{
		yield return StartCoroutine(SetAnimator(MSUtil.StripExtensions(structName), building.sprite.GetComponent<Animator>(), "Controller"));
	}

	public IEnumerator SetAnimator(string baseName, Animator animator, string controllerSuffix = "")
	{
		if (internalBundles.Contains(baseName))
		{
			string path = "Bundles/" + baseName + "/" + baseName + controllerSuffix;
			animator.runtimeAnimatorController = (Resources.Load(path)) as RuntimeAnimatorController;
			if (animator.runtimeAnimatorController == null)
			{
				Debug.LogWarning("Problem getting animator for path: " + path);
			}
			animator.GetComponent<SpriteRenderer>().color = Color.white;
		}
		else
		{
			if (!bundles.ContainsKey(baseName))
			{
				animator.runtimeAnimatorController = null;
				yield return StartCoroutine(DownloadAndCache(baseName));
			}

			if (bundles.ContainsKey(baseName))
			{
				//If something else has marked this bundle as downloading, but hasn't finished, we'll hang here
				while (bundles[baseName] == null)
				{
					yield return null;
				}

				animator.runtimeAnimatorController = bundles[baseName].Load(baseName + controllerSuffix, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
				animator.GetComponent<SpriteRenderer>().color = Color.white;
				//Debug.Log("Assigned it: " + animator.runtimeAnimatorController);
			}
		}
	}

	public Sprite GetBuildingSprite(string spriteName)
	{
		return Resources.Load("Sprites/Buildings/" + MSUtil.StripExtensions(spriteName), typeof(Sprite)) as Sprite;
	}

	public UIAtlas GetAtlas(string atlasName)
	{
		return bundles[atlasName].Load(atlasName, typeof(UIAtlas)) as UIAtlas;
	}

	public bool HasBundle (string bundleName)
	{
		return internalBundles.Contains(bundleName) || (bundles.ContainsKey(bundleName) && bundles[bundleName] != null);
	}

	public Coroutine RunDownloadAndCache(string bundleName)
	{
		return StartCoroutine(DownloadAndCache(bundleName));
	}

	/// <summary>
	/// Downloads the and cache.
	/// Sample code from: http://docs.unity3d.com/Documentation/Manual/DownloadingAssetBundles.html
	/// </summary>
	/// <returns>The and cache.</returns>
	IEnumerator DownloadAndCache (string bundleName, int attempts = 0)
	{
		if (attempts > 5)
		{
			yield break;
		}

		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;
		
		Debug.Log ("Grabbing bundle: " + bundleName);

		if (bundles.ContainsKey(bundleName))
		{
			while (bundles[bundleName] == null)
			{
				yield return null;
			}
			yield break;
		}
		
		Debug.Log ("Actually grabbing bundle: " + bundleName);
		
		bundles[bundleName] = null;
		
		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
		using(WWW www = WWW.LoadFromCacheOrDownload (AWS + bundleName + ".unity3d", 1)){
			yield return www;
			Debug.Log("In here: " + bundleName + ((www.error != null) ? ("\nError: " + www.error) : ("Fine")));

			if (www.error != null)
			{
				Debug.LogError("WWW download of " + bundleName + " had an error:" + www.error);
				yield return StartCoroutine(DownloadAndCache(bundleName, attempts+1));
				yield break;
			}

			AssetBundle bundle = www.assetBundle;

			Debug.Log("Loaded bundle: " + bundleName);

			bundles[bundleName] = bundle;
			
		}
	}

	[ContextMenu ("Clean Cache")]
	void CleanCache()
	{
		Caching.CleanCache();
	}
}

[System.Serializable]
public class BundleAtlas
{
	[SerializeField] UIAtlas referenceAtlas;
	[SerializeField] string atlasName;

	[SerializeField] MSResetFont[] fonts;

	public bool loaded = false;

	public void Download()
	{
		MSSpriteUtil.instance.StartCoroutine(DoDownload());
	}

	IEnumerator DoDownload()
	{
		if (MSSpriteUtil.instance.internalBundles.Contains(atlasName))
		{
			referenceAtlas.replacement = (Resources.Load<UIAtlas>("Bundles/" + atlasName + "/" + atlasName));
		}
		else
		{
			yield return MSSpriteUtil.instance.RunDownloadAndCache(atlasName);
			referenceAtlas.replacement = MSSpriteUtil.instance.GetAtlas(atlasName);
		}
		loaded = true;
		foreach (var item in fonts) 
		{
			item.SetAtlas(referenceAtlas);
		}
	}
}
