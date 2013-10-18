using UnityEngine;
using System.Collections;

/// <summary>
/// Static class that maintains references to all
/// manager classes in the scene. All managers
/// need to add their references on Awake(), and
/// things can reference this class at Start() at
/// the earliest
/// </summary>
public static class CBKManagerReferences {

	public static TCKControlManager controlManager;
	//public static CBKGridManager gridManager;
	//public static CBKBuildingManager buildingManager;
	
	//public static CBKResourceManager resourceManager;
	
	//public static AOC2DataManager dataManager;
}
