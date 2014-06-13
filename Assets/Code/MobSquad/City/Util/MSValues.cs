using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// A collection of commonly used magic values, grouped for
/// maximum readibility
/// </summary>
public static class MSValues 
{
	public static class Layers
	{
		public static int DEFAULT = 0;
		public static int TRANSPARENT_FX = 1;
		public static int IGNORE_RAYCAST = 2;
		public static int WATER = 4;
		public static int UI = 5;
		public static int PUZZLE = 10;
	}
    
    public static class Scene
    {
        public enum Scenes
        {
            TOWN_SCENE,
            PUZZLE_SCENE,
			LOADING_SCENE,
			STARTING_SCENE
        }
        public static Dictionary<Scenes, string> sceneDict = new Dictionary<Scenes, string>()
        {
            {Scenes.TOWN_SCENE, "TownScene"},
            {Scenes.PUZZLE_SCENE, "PuzzleScene"},
			{Scenes.LOADING_SCENE, "LoadingScene"},
			{Scenes.STARTING_SCENE, "StartingScene"}
        };
        
        public static AsyncOperation ChangeScene(Scenes scene)
        {
            return UnityEngine.Application.LoadLevelAsync(sceneDict[scene]);
        }
    }
	
	public static class Buildings
	{
		public static float UPGRADE_STRUCT_COIN_COST_EXPONENT_BASE = 1.7f;
		public static float UPGRADE_STRUCT_DIAMOND_COST_EXPONENT_BASE = 1.1f;
	}
	
	public enum ChatMode
	{
		GLOBAL,
		CLAN,
		PRIVATE
	}
	
	public enum Direction
	{
		NORTH,
		SOUTH,
		WEST,
		EAST,
		NONE
	}
	
	public static readonly Dictionary<Direction, Direction> opp = new Dictionary<Direction, Direction>()
	{
		{Direction.NORTH, Direction.SOUTH},
		{Direction.SOUTH, Direction.NORTH},
		{Direction.EAST, Direction.WEST},
		{Direction.WEST, Direction.EAST}
	};

	public static readonly Dictionary<Direction, Vector3> dirVectors = new Dictionary<Direction, Vector3>()
	{
		{Direction.NORTH, new Vector3(0, 1)},
		{Direction.SOUTH, new Vector3(0, -1)},
		{Direction.EAST, new Vector3(1, 0)},
		{Direction.WEST, new Vector3(-1, 0)}
	};
	
	public static class Colors
	{
		public static Color[] gemColors = {
			new Color(243, 105, 0, 255) / 255,//red
			new Color(100, 171, 18, 255) / 255,//green
			new Color(19, 163, 208, 255) / 255,//blue
			new Color(255, 213, 104, 255) / 255,//yellow
			new Color(163, 84, 212, 255) / 255,//purple
			new Color(153, 158, 161, 255) / 255,//grey
		};

		public const int moneyRed = 213;
		public const int moneyGreen = 231;
		public const int moneyBlue = 73;
		public static int moneyFull{
			get
			{
				return (moneyRed << 16) | (moneyGreen << 8) | moneyBlue;
			}
		}
		public const string moneyText = "D5E749";
	}
}
