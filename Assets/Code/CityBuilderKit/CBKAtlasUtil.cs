using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

public class CBKAtlasUtil : MonoBehaviour {
	
	[SerializeField]
	string[] xmls;
	
	#region Atlases
	
	[SerializeField]
	UIAtlas building001;
	
	[SerializeField]
	UIAtlas building002;
	
	#endregion
	
	#region OLD
	
	/// <summary>
	/// Maps building names to the atlas that they're contained in
	/// </summary>
	static Dictionary<string, UIAtlas> buildingAtlasDict;
	
	public static CBKAtlasUtil instance;
	
	static Dictionary<string, string> imgToAtlas = new Dictionary<string, string>();
	
	static Dictionary<string, UIAtlas> atlases;
	
	public void Awake()
	{
		instance = this;
		if (buildingAtlasDict == null)
		{
			Setup();
		}
		if (atlases == null)
		{
			atlases = new Dictionary<string, UIAtlas>();
			foreach (string item in xmls) 
			{
				WarmAtlasDictionaryFromXML(item);
			}
		}
	}
	
	private void Setup()
	{
		//Build the atlas dictionary
		//TODO: Turn this into XML
		buildingAtlasDict = new Dictionary<string, UIAtlas>();
		
		buildingAtlasDict["ArtMuseum"] = building001;
		buildingAtlasDict["Bakery"] = building001;
		buildingAtlasDict["BarberShop"] = building001;
		buildingAtlasDict["Casino"] = building001;
		buildingAtlasDict["ChineseRestaurant"] = building001;
		buildingAtlasDict["Church"] = building001;
		buildingAtlasDict["CoffeeShop"] = building001;
		buildingAtlasDict["ConvenienceStore"] = building001;
		buildingAtlasDict["Factory"] = building001;
		buildingAtlasDict["FastFoodRestaurant"] = building001;
		buildingAtlasDict["FlowerShop"] = building001;
		buildingAtlasDict["Gym"] = building001;
		buildingAtlasDict["Hospital"] = building001;
		buildingAtlasDict["Bakery"] = building001;
		buildingAtlasDict["House"] = building001;
		buildingAtlasDict["IndianRestaurant"] = building001;
		buildingAtlasDict["JewelryStore"] = building001;
		buildingAtlasDict["Lofts"] = building001;
		buildingAtlasDict["Motel6"] = building001;
		buildingAtlasDict["MovieTheater"] = building001;
		
		buildingAtlasDict["NightClub"] = building001;
		buildingAtlasDict["ParkingGarage"] = building001;
		buildingAtlasDict["pizzeria"] = building001;
		buildingAtlasDict["PoliceStation"] = building001;
		buildingAtlasDict["PowerPlant"] = building001;
		buildingAtlasDict["PublicSchool"] = building001;
		buildingAtlasDict["Supermarket"] = building001;
		buildingAtlasDict["SushiBar"] = building001;
		buildingAtlasDict["TeaShop"] = building001;
		buildingAtlasDict["TVStation"] = building001;
	}
	
	public UISpriteData LookupBuildingSprite(string name)
	{
		string cleaned = StripExtensions(StripSpaces(name));
		
		Debug.Log("Building: " + cleaned);
		
		if (!buildingAtlasDict.ContainsKey(cleaned))
		{
			Debug.Log("Dictionary does not contain key for " + cleaned);
			return null;
		}
		
		return buildingAtlasDict[cleaned].GetSprite(cleaned);
	}
	
	public UIAtlas GetBuildingAtlas(string name)
	{
		string stripped = StripSpaces(name);
		stripped = StripExtensions(stripped);
		if (!buildingAtlasDict.ContainsKey(stripped))
		{
			Debug.Log("Dictionary does not contain key for " + stripped);
			return building001;
		}
		
		return buildingAtlasDict[stripped];
	}
	
	public string StripSpaces(string word)
	{
		StringBuilder sb = new StringBuilder(word.Length);
		char c;
		for (int i = 0; i < word.Length; i++) {
			c = word[i];
			if (c != ' ')
			{
				sb.Append(c);
			}
		}
		return sb.ToString();
	}
	
	public string StripExtensions(string file)
	{
		return Path.GetFileNameWithoutExtension(file);
	}

	#endregion
	
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
			string atlasName = imgToAtlas[StripExtensions(item)];
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
		string atlasName = imgToAtlas[StripExtensions(item.spriteName)];
		LoadAtlas(atlasName);
		item.atlas = atlases[atlasName];
	}
	
	public void UnloadAllAtlases()
	{
		atlases.Clear();
		Resources.UnloadUnusedAssets();
	}
}
