using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

public class CBKAtlasUtil : MonoBehaviour {
	
	[SerializeField]
	string[] xmls;

	public static CBKAtlasUtil instance;
	
	static Dictionary<string, string> imgToAtlas = new Dictionary<string, string>();
	
	static Dictionary<string, UIAtlas> atlases;
	
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
			string atlasName = imgToAtlas[CBKUtil.StripExtensions(item)];
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
			atlasName = imgToAtlas[CBKUtil.StripExtensions(item.spriteName)];
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
		return (Resources.Load("Controllers/" + CBKUtil.StripExtensions(imageName))) as RuntimeAnimatorController;
	}

	public Sprite GetBuildingSprite(string spriteName)
	{
		return Resources.Load("Sprites/Buildings/" + CBKUtil.StripExtensions(spriteName), typeof(Sprite)) as Sprite;
	}
	
	public void UnloadAllAtlases()
	{
		atlases.Clear();
		Resources.UnloadUnusedAssets();
	}
}
