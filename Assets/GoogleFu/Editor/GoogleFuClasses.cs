//----------------------------------------------
//    GoogleFu: Google Doc Unity integration
//         Copyright © 2013 Litteratus
//----------------------------------------------

using UnityEngine;
using UnityEditor;

namespace GoogleFu
{	
	public partial class GoogleFuEditor : EditorWindow
	{
		private enum GF_PAGE
		{
			Settings,
			Workbooks,
			Toolbox,
			Help,
			Help_Main,
			Help_Docs
		}
		
		enum selectionContent
		{
			SimpleDatabase,
			NGUILocalization,
			XMLExport,
			JSONExport,
			AdvancedDatabase
		}
		
		public class WorkBookInfo
		{
			public Google.GData.Spreadsheets.SpreadsheetEntry spreadsheetEntry = null;
			public Google.GData.Client.AtomEntryCollection WorksheetEntries = null;
			public System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> ManualEntries = new System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry>();
			public string Url = "";
			public string Title = "";
			
			public WorkBookInfo()
			{
			}
			
			public WorkBookInfo( Google.GData.Spreadsheets.SpreadsheetEntry entry )
			{
				spreadsheetEntry = entry;
				WorksheetEntries = spreadsheetEntry.Worksheets.Entries;
				Title = entry.Title.Text;
				foreach ( var link in entry.Links )
				{
					if( link.Rel.ToLower() == "alternate" )
					{
						Url = link.HRef.ToString();
						break;
					}
				}
			}
	
			public void AddWorksheetEntry( Google.GData.Spreadsheets.WorksheetEntry entry, string url )
			{
				if ( spreadsheetEntry == null )
					spreadsheetEntry = new Google.GData.Spreadsheets.SpreadsheetEntry();
				Google.GData.Spreadsheets.SpreadsheetEntry.ImportFromFeed(entry);
				Title = entry.Feed.Title.Text;
				Url = url;
				ManualEntries.Add(entry);
			}
			
			public Google.GData.Spreadsheets.SpreadsheetFeed GetSpreadsheetFeed()
			{
				return spreadsheetEntry.Feed as Google.GData.Spreadsheets.SpreadsheetFeed;
			}
			
			public override string ToString ()
			{
				return string.Format ( Url + "." + Title );
			}
		}
		
		[System.Serializable]
		public class AdvancedDatabaseInfo
		{
			public GameObject DatabaseAttachObject;
			public string ComponentName;
			public Google.GData.Spreadsheets.WorksheetEntry entry;
			public System.Collections.Generic.List< string > entryStrings = new System.Collections.Generic.List <string>();
			public int entryStride = 0;
			public bool GeneratePlaymaker;
			public AdvancedDatabaseInfo(string ComponentName, Google.GData.Spreadsheets.WorksheetEntry _entry, Google.GData.Spreadsheets.SpreadsheetsService _service, GameObject _DatabaseAttachObject, bool bGeneratePlaymaker, bool bFirstRowValueTypes)
			{
				ParseWorksheetEntry ( _entry, _service, bFirstRowValueTypes );
				this.ComponentName = ComponentName;
				if ( _DatabaseAttachObject != null )
					DatabaseAttachObject = _DatabaseAttachObject;
				else
				{
					DatabaseAttachObject = GameObject.Find("databaseObj");
					if(DatabaseAttachObject == null)
						DatabaseAttachObject = new GameObject("databaseObj");
				}

				GeneratePlaymaker = bGeneratePlaymaker;
			}
			
			private void ParseWorksheetEntry ( Google.GData.Spreadsheets.WorksheetEntry entry, Google.GData.Spreadsheets.SpreadsheetsService _service, bool bFirstRowValueTypes )
			{
				if ( entry == null )
				{
					Debug.LogError("Could not read WorksheetEntry - retry count:  ");
					return;
				}

				// Define the URL to request the list feed of the worksheet.
				Google.GData.Client.AtomLink listFeedLink = entry.Links.FindService(Google.GData.Spreadsheets.GDataSpreadsheetsNameTable.ListRel, null);
				
				// Fetch the list feed of the worksheet.
				Google.GData.Spreadsheets.ListQuery listQuery = new Google.GData.Spreadsheets.ListQuery(listFeedLink.HRef.ToString());
				Google.GData.Spreadsheets.ListFeed listFeed = _service.Query(listQuery);
				
				//int rowCt = listFeed.Entries.Count;
				//int colCt = ((ListEntry)listFeed.Entries[0]).Elements.Count;
				
				if ( listFeed.Entries.Count > 0 )
				{
					
					int curRow = 0;
					// Iterate through each row, printing its cell values.
					foreach (Google.GData.Spreadsheets.ListEntry row in listFeed.Entries)
					{
						
						// skip the first row if this is a value type row
						if ( curRow == 0 && bFirstRowValueTypes == true )
						{
							curRow++;
							continue;
						}
						
						int curCol = 0;
						// Iterate over the remaining columns, and print each cell value
						foreach (Google.GData.Spreadsheets.ListEntry.Custom element in row.Elements)
						{
							// this will be the list of all the values in the row excluding the first 'name' column
							if(curCol > 0)
								entryStrings.Add ( element.Value );
							curCol++;
						}
						entryStride = curCol-1;

						curRow++;
					}
				}
			}
		}
	}
}
