//----------------------------------------------
//    GoogleFu: Google Doc Unity integration
//         Copyright ?? 2013 Litteratus
//
//        This file has been auto-generated
//              Do not manually edit
//----------------------------------------------

using UnityEngine;

namespace GoogleFu
{
	[System.Serializable]
	public class Sheet1Row 
	{
		public string _EN;
		public string _FR;
		public string _JA;
		public string _KO;
		public Sheet1Row(string __EN, string __FR, string __JA, string __KO) 
		{
			_EN = __EN;
			_FR = __FR;
			_JA = __JA;
			_KO = __KO;
		}

		public string GetStringData( string colID )
		{
			string ret = System.String.Empty;
			switch( colID.ToUpper() )
			{
				case "EN":
					ret = _EN.ToString();
					break;
				case "FR":
					ret = _FR.ToString();
					break;
				case "JA":
					ret = _JA.ToString();
					break;
				case "KO":
					ret = _KO.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "EN" + " : " + _EN.ToString() + "} ";
			ret += "{" + "FR" + " : " + _FR.ToString() + "} ";
			ret += "{" + "JA" + " : " + _JA.ToString() + "} ";
			ret += "{" + "KO" + " : " + _KO.ToString() + "} ";
			return ret;
		}
	}
	public sealed class Sheet1
	{
		public enum rowIds {
			ID_BACK, ID_QUEST_TAB_HEADER, ID_ACHIEVEMENT_TAB_HEADER, ID_QUEST_DETAILS_REWARDS, ID_ACHIEVEMENT_DETAILS_REWARD, ID_ACHIEVEMENT_RANK, ID_ACHIEVEMENT_COLLECT, ID_QUEST_JOB_TASK, ID_QUEST_JOB_INSTRUCTIONS, ID_QUEST_JOB_PROGRESS, ID_QUEST_JOB_VISIT
		};
		public string [] rowNames = {
			"ID_BACK", "ID_QUEST_TAB_HEADER", "ID_ACHIEVEMENT_TAB_HEADER", "ID_QUEST_DETAILS_REWARDS", "ID_ACHIEVEMENT_DETAILS_REWARD", "ID_ACHIEVEMENT_RANK", "ID_ACHIEVEMENT_COLLECT", "ID_QUEST_JOB_TASK", "ID_QUEST_JOB_INSTRUCTIONS", "ID_QUEST_JOB_PROGRESS", "ID_QUEST_JOB_VISIT"
		};
		public System.Collections.Generic.List<Sheet1Row> Rows = new System.Collections.Generic.List<Sheet1Row>();

		public static Sheet1 Instance
		{
			get { return NestedSheet1.instance; }
		}

		private class NestedSheet1
		{
			static NestedSheet1() { }
			internal static readonly Sheet1 instance = new Sheet1();
		}

		private Sheet1()
		{
			Rows.Add( new Sheet1Row("Back",
														"Arri\u00e8re",
														"\u30d0\u30c3\u30af",
														"\ubc31"));
			Rows.Add( new Sheet1Row("QUEST",
														"QUEST",
														"\u30af\u30a8\u30b9\u30c8",
														"QUEST"));
			Rows.Add( new Sheet1Row("ACHIEVEMENTS",
														"R\u00c9ALISATIONS",
														"\u30a2\u30c1\u30fc\u30d6\u30e1\u30f3\u30c8",
														"\uc5c5\uc801"));
			Rows.Add( new Sheet1Row("REWARDS",
														"R\u00c9COMPENSES",
														"\u30ea\u30ef\u30fc\u30c9",
														"\ubcf4\uc0c1"));
			Rows.Add( new Sheet1Row("REWARD",
														"R\u00c9COMPENSE",
														"REWARD",
														"REWARD"));
			Rows.Add( new Sheet1Row("RANK",
														"RANG",
														"RANK",
														"RANK"));
			Rows.Add( new Sheet1Row("COLLECT",
														"COLLECT",
														"COLLECT",
														"COLLECT"));
			Rows.Add( new Sheet1Row("TASK",
														"GROUPE",
														"\u30bf\u30b9\u30af",
														"TASK"));
			Rows.Add( new Sheet1Row("INSTRUCTIONS",
														"INSTRUCTIONS",
														"\u30a4\u30f3\u30b9\u30c8\u30e9\u30af\u30b7\u30e7\u30f3",
														"\uc9c0\uce68"));
			Rows.Add( new Sheet1Row("PROGRESS",
														"PROGRESS",
														"PROGRESS",
														"PROGRESS"));
			Rows.Add( new Sheet1Row("VISIT",
														"VISITE",
														"VISIT",
														"VISIT"));
		}
		public Sheet1Row GetRow(rowIds rowID)
		{
			Sheet1Row ret = null;
			try
			{
				ret = Rows[(int)rowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( rowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public Sheet1Row GetRow(string rowString)
		{
			Sheet1Row ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), rowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( rowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}

	}

}
