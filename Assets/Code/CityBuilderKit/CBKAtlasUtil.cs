using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class CBKAtlasUtil : MonoBehaviour {
	
	[SerializeField]
	UIAtlas building001;
	
	[SerializeField]
	UIAtlas building002;
	
	[SerializeField]
	UIAtlas gangsterMafia;
	
	[SerializeField]
	UIAtlas clown001;
	
	/// <summary>
	/// Maps building names to the atlas that they're contained in
	/// </summary>
	static Dictionary<string, UIAtlas> buildingAtlasDict;
	
	static Dictionary<string, UIAtlas> goonAtlasDict;
	
	public static CBKAtlasUtil instance;
	
	public void Awake()
	{
		instance = this;
		if (buildingAtlasDict == null)
		{
			Setup();
		}
	}
	
	private void Setup()
	{
		//Build the atlas dictionary
		//HARD-CODED! Try to find a way to soft-code this!!! XML?
		buildingAtlasDict = new Dictionary<string, UIAtlas>();
		goonAtlasDict = new Dictionary<string, UIAtlas>();
		
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
		buildingAtlasDict["house"] = building001;
		buildingAtlasDict["IndianRestaurant"] = building001;
		buildingAtlasDict["JewelryStore"] = building001;
		buildingAtlasDict["Lofts"] = building001;
		buildingAtlasDict["Motel6"] = building001;
		buildingAtlasDict["MovieTheater"] = building001;
		
		buildingAtlasDict["NightClub"] = building002;
		buildingAtlasDict["ParkingGarage"] = building002;
		buildingAtlasDict["pizzeria"] = building002;
		buildingAtlasDict["PoliceStation"] = building002;
		buildingAtlasDict["PowerPlant"] = building002;
		buildingAtlasDict["PublicSchool"] = building002;
		buildingAtlasDict["Supermarket"] = building002;
		buildingAtlasDict["SushiBar"] = building002;
		buildingAtlasDict["TeaShop"] = building002;
		buildingAtlasDict["TVStation"] = building002;
		
		goonAtlasDict["GangsterBrute"] = gangsterMafia;
		goonAtlasDict["GangsterMan"] = gangsterMafia;
		goonAtlasDict["GangsterWoman"] = gangsterMafia;
		goonAtlasDict["MafiaBrute"] = gangsterMafia;
		goonAtlasDict["MafiaMan"] = gangsterMafia;
		goonAtlasDict["MafiaWoman"] = gangsterMafia;
		goonAtlasDict["Clown_Boss"] = clown001;
	}
	
	public UISpriteData LookupBuildingSprite(string name)
	{
		string withoutSpaces = StripSpaces(name);
		
		if (!buildingAtlasDict.ContainsKey(withoutSpaces))
		{
			Debug.Log("Dictionary does not contain key for " + withoutSpaces);
			return null;
		}
		
		return buildingAtlasDict[withoutSpaces].GetSprite(withoutSpaces);
	}
	
	public UIAtlas GetBuildingAtlas(string name)
	{
		string stripped = StripSpaces(name);
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
	
	public UIAtlas LookupGoonAtlas(string fileName)
	{
		string goonName = StripExtensions(fileName);
		if (!goonAtlasDict.ContainsKey(goonName))
		{
			Debug.Log("Did not find Goon: " + goonName + " in atlas references!");
			return clown001;
		}
		return goonAtlasDict[goonName];
	}
	
	public string StripExtensions(string file)
	{
		return Path.GetFileNameWithoutExtension(file);
	}
}
