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
	
	[SerializeField]
	UIAtlas cheerleader1;
	
	[SerializeField]
	UIAtlas cheerleader2;
	
	[SerializeField]
	UIAtlas cheerleader3;
	
	[SerializeField]
	UIAtlas rapper1;
	
	[SerializeField]
	UIAtlas rapper2;
	
	[SerializeField]
	UIAtlas rapper3;
	
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
		
		goonAtlasDict["GangsterBrute"] = gangsterMafia;
		goonAtlasDict["GangsterMan"] = gangsterMafia;
		goonAtlasDict["GangsterWoman"] = gangsterMafia;
		goonAtlasDict["MafiaBrute"] = gangsterMafia;
		goonAtlasDict["MafiaMan"] = gangsterMafia;
		goonAtlasDict["MafiaWoman"] = gangsterMafia;
		
		goonAtlasDict["Clown_Boss"] = clown001;
		
		goonAtlasDict["Cheerleader1SMG"] = cheerleader1;
		goonAtlasDict["Cheerleader1Bazooka"] = cheerleader1;
		goonAtlasDict["Cheerleader1Uzi"] = cheerleader1;
		goonAtlasDict["Cheerleader2SMG"] = cheerleader1;
		goonAtlasDict["Cheerleader2Bazooka"] = cheerleader1;
		goonAtlasDict["Cheerleader2Uzi"] = cheerleader1;
		goonAtlasDict["Cheerleader3SMG"] = cheerleader2;
		goonAtlasDict["Cheerleader3Bazooka"] = cheerleader2;
		goonAtlasDict["Cheerleader3Uzi"] = cheerleader2;
		goonAtlasDict["Cheerleader4SMG"] = cheerleader2;
		goonAtlasDict["Cheerleader4Bazooka"] = cheerleader2;
		goonAtlasDict["Cheerleader4Uzi"] = cheerleader2;
		goonAtlasDict["Cheerleader5SMG"] = cheerleader3;
		goonAtlasDict["Cheerleader5Bazooka"] = cheerleader3;
		goonAtlasDict["Cheerleader5Uzi"] = cheerleader3;
		
		goonAtlasDict["Rapper1AK"] = rapper1;
		goonAtlasDict["Rapper1Shotgun"] = rapper1;
		goonAtlasDict["Rapper1Uzi"] = rapper1;
		goonAtlasDict["Rapper2AK"] = rapper1;
		goonAtlasDict["Rapper2Shotgun"] = rapper1;
		goonAtlasDict["Rapper2Uzi"] = rapper1;
		goonAtlasDict["Rapper3AK"] = rapper2;
		goonAtlasDict["Rapper3Shotgun"] = rapper2;
		goonAtlasDict["Rapper3Uzi"] = rapper2;
		goonAtlasDict["Rapper4AK"] = rapper2;
		goonAtlasDict["Rapper4Shotgun"] = rapper2;
		goonAtlasDict["Rapper4Uzi"] = rapper2;
		goonAtlasDict["Rapper5AK"] = rapper3;
		goonAtlasDict["Rapper5Shotgun"] = rapper3;
		goonAtlasDict["Rapper5Uzi"] = rapper3;
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
