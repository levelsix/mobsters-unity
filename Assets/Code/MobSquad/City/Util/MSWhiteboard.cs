using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// A static container for data, particularly that data which needs
/// to persist between scenes
/// </summary>
public static class MSWhiteboard {
	
	/// <summary>
	/// The local user.
	/// </summary>
	public static FullUserProto localUser;
	
	/// <summary>
	/// The local user's MUP
	/// </summary>
	public static MinimumUserProto localMup;

	public static MinimumUserProtoWithMaxResources localMupWithResources
	{
		get
		{
			MinimumUserProtoWithMaxResources mup = new MinimumUserProtoWithMaxResources();
			mup.minUserProto = localMup;
			mup.maxCash = MSResourceManager.maxes[0];
			mup.maxOil = MSResourceManager.maxes[1];
			return mup;
		}
	}

	public static MinimumUserProtoWithFacebookId localMupWithFacebook
	{
		get
		{
			MinimumUserProtoWithFacebookId mup = new MinimumUserProtoWithFacebookId();
			mup.minUserProto = localMup;
			if (FB.IsLoggedIn)
			{
				mup.facebookId = FB.UserId;
			}
			return mup;
		}
	}
	
	public static StartupResponseProto.StartupConstants constants;
	public static StartupResponseProto.TutorialConstants tutorialConstants;
	
	public enum CityType {PLAYER, NEUTRAL};
	
	public static CityType currCityType = CityType.PLAYER;
	/// <summary>
	/// The city id, which can be used for either city type.
	/// </summary>
	public static int cityID = 1;

	public static long currUserTaskId;
	
	public static BeginDungeonRequestProto dungeonToLoad;
	public static BeginDungeonResponseProto loadedDungeon;

	public static int currTaskId;
	public static List<TaskStageProto> currTaskStages;

	public static QueueUpResponseProto loadedPvps;
	
	public static LoadCityResponseProto loadedNeutralCity;
	public static LoadPlayerCityResponseProto loadedPlayerCity;
	
	public enum SceneType {CITY, PUZZLE};
	
	public static SceneType currSceneType = SceneType.CITY;
	
	public static StaticUserLevelInfoProto nextLevelInfo;
	
}
