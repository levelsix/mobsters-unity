#define DEBUG

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// Manager which keeps a static dictionary of all data loaded into the game
/// </summary>
public class CBKDataManager : MonoBehaviour {
	
	public static CBKDataManager instance;
	
	/// <summary>
	/// A dictionary that maps types to lists of their 
	/// </summary>
	static Dictionary<Type, Dictionary<int, object>> dataDict = new Dictionary<Type, Dictionary<int,object>>();
	
	bool enemiesDone = false;
	
	bool loadComplete{
		get
		{
			return enemiesDone;
		}
		set
		{
			enemiesDone = value;
		}
	}
	
	void Awake()
	{
		instance = this;
	}
	
	IEnumerator Start()
	{
		while(true)
		{
			yield return new WaitForSeconds(5);
			GC.Collect();
		}
	}
	
	/// <summary>
	/// Adds the specified type to the data dictionary and creates a list
	/// for it.
	/// </summary>
	/// <param name='typ'>
	/// Type.
	/// </param>
	void AddType(Type typ)
	{
		dataDict.Add(typ, new Dictionary<int, object>());
	}
	
	/// <summary>
	/// Checks whether there already exists a dictionary entry for this type.
	/// If there isn't, adds it.
	/// </summary>
	/// <param name='typ'>
	/// Type.
	/// </param>
	void CheckType(Type typ)
	{
		if (!dataDict.ContainsKey(typ))
		{
			AddType(typ);
		}
	}
	
	/// <summary>
	/// Gets the data of the specified type and id.
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	public object Get(Type type, int id)
	{
		//Debug.Log("Getting: " + type.ToString() + " #" + id);
		CheckType(type);
		if (!dataDict[type].ContainsKey(id))
		{
			Debug.LogWarning("Failed to find " + type.ToString() + " #" + id);
			return null;
		}
		//Debug.Log("Returning " + dataDict[type][id]);
		return dataDict[type][id];
	}
	
	public bool Has(Type type, int id)
	{
		CheckType(type);
		return dataDict[type].ContainsKey(id);
	}
	
	public IDictionary GetAll(Type type)
	{
		return dataDict[type];
	}
	
	/// <summary>
	/// Load the specified obj and id.
	/// </summary>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	public void Load(object obj, int id)
	{
		Type typ = obj.GetType();
		CheckType(typ);
		dataDict[typ][id] = obj;
		
		Debug.Log("Loading " + typ.ToString() + " " + id);
	}
	
	/// <summary>
	/// Insures that we have loaded locally all necessary jobs for the given quest.
	/// </summary>
	/// <param name='quest'>
	/// Quest.
	/// </param>
	public void RequestQuestData(FullQuestProto quest)
	{	
		
	}
	
	public void LoadStaticData(PurgeClientStaticDataResponseProto response)
	{
		LoadStaticData(response.staticDataStuff);
	}
	
	public void LoadStaticData(StaticDataProto data)
	{
		foreach (var item in data.expansionCosts) 
		{
			CBKDataManager.instance.Load(item, item.expansionNum);
		}
		foreach (var item in data.allCities) 
		{
			CBKDataManager.instance.Load(item, item.cityId);
		}
		foreach (var item in data.allGenerators)
		{
			CBKCombinedBuildingProto building = new CBKCombinedBuildingProto(item);
			CBKDataManager.instance.Load(building, building.id);
		}
		foreach (var item in data.allStorages)
		{
			CBKCombinedBuildingProto building = new CBKCombinedBuildingProto(item);
			CBKDataManager.instance.Load(building, building.id);
		}
		foreach (var item in data.allHospitals) 
		{
			CBKCombinedBuildingProto building = new CBKCombinedBuildingProto(item);
			CBKDataManager.instance.Load(building, building.id);
		}
		foreach (var item in data.allLabs) 
		{
			CBKCombinedBuildingProto building = new CBKCombinedBuildingProto(item);
			CBKDataManager.instance.Load(building, building.id);
		}
		foreach (var item in data.allResidences) 
		{
			CBKCombinedBuildingProto building = new CBKCombinedBuildingProto(item);
			CBKDataManager.instance.Load(building, building.id);
		}
		foreach (var item in data.allTownHalls) 
		{
			CBKCombinedBuildingProto building = new CBKCombinedBuildingProto(item);
			CBKDataManager.instance.Load(building, building.id);
		}
		foreach (var item in data.allTasks) 
		{
			CBKDataManager.instance.Load(item, item.taskId);
		}
		foreach (var item in data.allMonsters) 
		{
			CBKDataManager.instance.Load(item, item.monsterId);
		}
		foreach (var item in data.slip) 
		{
			CBKDataManager.instance.Load(item, item.level);
		}

		foreach (var item in data.inProgressQuests) 
		{
			CBKDataManager.instance.Load(item, item.questId);
		}
		foreach (var item in data.unredeemedQuests) 
		{
			CBKDataManager.instance.Load(item, item.questId);
		}
		foreach (var item in data.availableQuests) 
		{
			CBKDataManager.instance.Load(item, item.questId);
		}
	}
}
