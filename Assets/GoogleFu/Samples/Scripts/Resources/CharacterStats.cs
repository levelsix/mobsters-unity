//----------------------------------------------
//    GoogleFu: Google Doc Unity integration
//         Copyright Â© 2013 Litteratus
//
//        This file has been auto-generated
//              Do not manually edit
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GoogleFuSample
{
	[System.Serializable]
	public class CharacterStatsRow 
	{
		public string _NAME;
		public int _LEVEL;
		public float _BASEMODIFIER;
		public string _CLASS;
		public int _STRENGTH;
		public int _ENDURANCE;
		public int _INTELLIGENCE;
		public int _DEXTERITY;
		public int _HEALTH;
		public CharacterStatsRow(string __NAME, string __LEVEL, string __BASEMODIFIER, string __CLASS, string __STRENGTH, string __ENDURANCE, string __INTELLIGENCE, string __DEXTERITY, string __HEALTH) 
		{
			_NAME = __NAME;
			{
			int res;
				if(int.TryParse(__LEVEL, out res))
					_LEVEL = res;
				else
					Debug.LogError("Failed To Convert LEVEL string: "+ __LEVEL +" to int");
			}
			{
			float res;
				if(float.TryParse(__BASEMODIFIER, out res))
					_BASEMODIFIER = res;
				else
					Debug.LogError("Failed To Convert BASEMODIFIER string: "+ __BASEMODIFIER +" to float");
			}
			_CLASS = __CLASS;
			{
			int res;
				if(int.TryParse(__STRENGTH, out res))
					_STRENGTH = res;
				else
					Debug.LogError("Failed To Convert STRENGTH string: "+ __STRENGTH +" to int");
			}
			{
			int res;
				if(int.TryParse(__ENDURANCE, out res))
					_ENDURANCE = res;
				else
					Debug.LogError("Failed To Convert ENDURANCE string: "+ __ENDURANCE +" to int");
			}
			{
			int res;
				if(int.TryParse(__INTELLIGENCE, out res))
					_INTELLIGENCE = res;
				else
					Debug.LogError("Failed To Convert INTELLIGENCE string: "+ __INTELLIGENCE +" to int");
			}
			{
			int res;
				if(int.TryParse(__DEXTERITY, out res))
					_DEXTERITY = res;
				else
					Debug.LogError("Failed To Convert DEXTERITY string: "+ __DEXTERITY +" to int");
			}
			{
			int res;
				if(int.TryParse(__HEALTH, out res))
					_HEALTH = res;
				else
					Debug.LogError("Failed To Convert HEALTH string: "+ __HEALTH +" to int");
			}
		}

		public string GetStringData( string colID )
		{
			string ret = String.Empty;
			switch( colID.ToUpper() )
			{
				case "NAME":
					ret = _NAME.ToString();
					break;
				case "LEVEL":
					ret = _LEVEL.ToString();
					break;
				case "BASEMODIFIER":
					ret = _BASEMODIFIER.ToString();
					break;
				case "CLASS":
					ret = _CLASS.ToString();
					break;
				case "STRENGTH":
					ret = _STRENGTH.ToString();
					break;
				case "ENDURANCE":
					ret = _ENDURANCE.ToString();
					break;
				case "INTELLIGENCE":
					ret = _INTELLIGENCE.ToString();
					break;
				case "DEXTERITY":
					ret = _DEXTERITY.ToString();
					break;
				case "HEALTH":
					ret = _HEALTH.ToString();
					break;
			}

			return ret;
		}
	}
	public class CharacterStats :  GoogleFu.GoogleFuComponentBase
	{
		public enum rowIds {
			AI_GOBLIN, AI_ORC, AI_TROLL, AI_DEATH_KNIGHT
		};
		public string [] rowNames = {
			"AI_GOBLIN", "AI_ORC", "AI_TROLL", "AI_DEATH_KNIGHT"
		};
		public List<CharacterStatsRow> Rows = new List<CharacterStatsRow>();
		public override void AddRowGeneric (List<string> input)
		{
			Rows.Add(new CharacterStatsRow(input[0],input[1],input[2],input[3],input[4],input[5],input[6],input[7],input[8]));
		}
		public override void Clear ()
		{
			Rows.Clear();
		}
		public CharacterStatsRow GetRow(rowIds rowID)
		{
			CharacterStatsRow ret = null;
			try
			{
				ret = Rows[(int)rowID];
			}
			catch( KeyNotFoundException ex )
			{
				Debug.LogError( rowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public CharacterStatsRow GetRow(string rowString)
		{
			CharacterStatsRow ret = null;
			try
			{
				ret = Rows[(int)Enum.Parse(typeof(rowIds), rowString)];
			}
			catch(ArgumentException) {
				Debug.LogError( rowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}

	}

}
