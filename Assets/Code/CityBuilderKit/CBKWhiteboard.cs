using UnityEngine;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// A static container for data, particularly that data which needs
/// to persist between scenes
/// </summary>
public static class CBKWhiteboard {
	
	/// <summary>
	/// The local user.
	/// </summary>
	public static FullUserProto localUser;
	
	/// <summary>
	/// The local user's MUP
	/// </summary>
	public static MinimumUserProto localMup;
	
	public static StartupResponseProto.StartupConstants constants;
	
	public enum CityType {PLAYER, NEUTRAL};
	
	public static CityType currCityType = CityType.PLAYER;
	/// <summary>
	/// The city id, which can be used for either city type.
	/// </summary>
	public static int cityID = 1;
	
	public static StructureInfoProto tempStructureProto;
	public static CBKGridNode tempStructurePos;

	public static long currUserTaskId;
	
	public static BeginDungeonRequestProto dungeonToLoad;
	public static BeginDungeonResponseProto loadedDungeon;
	
	public static LoadCityResponseProto loadedNeutralCity;
	public static LoadPlayerCityResponseProto loadedPlayerCity;
	
	public enum SceneType {CITY, PUZZLE};
	
	public static SceneType currSceneType = SceneType.CITY;
	
	public static StaticUserLevelInfoProto nextLevelInfo;
	
}
