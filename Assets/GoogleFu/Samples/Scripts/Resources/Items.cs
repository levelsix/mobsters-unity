using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GoogleFuSample
{
	[System.Serializable]
	public class ItemsRow 
	{
		public int _LEVEL;
		public string _NAME;
		public int _STRENGTH;
		public int _ENDURANCE;
		public int _INTELLIGENCE;
		public int _DEXTERITY;
		public float _EROTICISM;
		public ItemsRow(string __LEVEL, string __NAME, string __STRENGTH, string __ENDURANCE, string __INTELLIGENCE, string __DEXTERITY, string __EROTICISM) 
		{
			{
			int res;
				if(int.TryParse(__LEVEL, out res))
					_LEVEL = res;
				else
					Debug.LogError("Failed To Convert LEVEL string: "+ __LEVEL +" to int");
			}
			_NAME = __NAME;
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
			float res;
				if(float.TryParse(__EROTICISM, out res))
					_EROTICISM = res;
				else
					Debug.LogError("Failed To Convert EROTICISM string: "+ __EROTICISM +" to float");
			}
		}

		public string GetStringData( string colID )
		{
			string ret = String.Empty;
			switch( colID.ToUpper() )
			{
				case "LEVEL":
					ret = _LEVEL.ToString();
					break;
				case "NAME":
					ret = _NAME.ToString();
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
				case "EROTICISM":
					ret = _EROTICISM.ToString();
					break;
			}

			return ret;
		}
	}
	public sealed class Items
	{
		public enum rowIds {
			ITEM_RING, ITEM_BRACERS, ITEM_HELMET, item1, item2, item3, item4, item5, item6, item7
		};
		public string [] rowNames = {
			"ITEM_RING", "ITEM_BRACERS", "ITEM_HELMET", "item1", "item2", "item3", "item4", "item5", "item6", "item7"
		};
		public List<ItemsRow> Rows = new List<ItemsRow>();

		public static Items Instance
		{
			get { return NestedItems.instance; }
		}

		private class NestedItems
		{
			static NestedItems() { }
			internal static readonly Items instance = new Items();
		}

		private Items()
		{
			Rows.Add( new ItemsRow("1",
														"Ring of McLovin",
														"0",
														"0",
														"0",
														"4",
														"0"));
			Rows.Add( new ItemsRow("3",
														"Bracers of Gender Bender",
														"0",
														"0",
														"5",
														"0",
														"25"));
			Rows.Add( new ItemsRow("5",
														"Helmet of Fabulousness",
														"0",
														"6",
														"0",
														"5",
														"50"));
			Rows.Add( new ItemsRow("7",
														"Nicks Unwashed Jock",
														"7",
														"0",
														"0",
														"0",
														"75"));
			Rows.Add( new ItemsRow("1",
														"Ring of McLovin",
														"0",
														"0",
														"0",
														"4",
														"100"));
			Rows.Add( new ItemsRow("3",
														"Bracers of Gender Bender",
														"0",
														"0",
														"5",
														"0",
														"125"));
			Rows.Add( new ItemsRow("5",
														"Helmet of Fabulousness",
														"0",
														"6",
														"0",
														"5",
														"150"));
			Rows.Add( new ItemsRow("7",
														"Nicks Unwashed Jock",
														"7",
														"0",
														"0",
														"0",
														"175"));
			Rows.Add( new ItemsRow("1",
														"Ring of McLovin",
														"0",
														"0",
														"0",
														"4",
														"200"));
			Rows.Add( new ItemsRow("3",
														"Bracers of Gender Bender",
														"0",
														"0",
														"5",
														"0",
														"225"));
		}
		public ItemsRow GetRow(rowIds rowID)
		{
			ItemsRow ret = null;
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
		public ItemsRow GetRow(string rowString)
		{
			ItemsRow ret = null;
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
