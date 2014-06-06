//----------------------------------------------
//    GoogleFu: Google Doc Unity integration
//         Copyright Â© 2013 Litteratus
//----------------------------------------------

using UnityEngine;
using UnityEditor;

namespace GoogleFu
{
	public partial class GoogleFuEditor : EditorWindow
	{
		// Add menu item named "GoogleFu" to the Window menu
		[MenuItem("Window/GoogleFu")]
		public static void ShowWindow()
		{
			//Show existing window instance. If one doesn't exist, make one.
			EditorWindow googleFuWindow = EditorWindow.GetWindow(typeof(GoogleFuEditor));
			googleFuWindow.title = "GoogleFu";
		}
		
		void OnEnable()
		{
			EditorApplication.update += Update;
		}
		
		void Init()
		{
			
			_authorized = false;
			
			_unityLogo = (Texture2D)Resources.Load("Pwrd_By_Unity_Pri_In_sm", typeof(Texture2D));
			_litteratusLogo = (Texture2D)Resources.Load("Litteratus_Logo_sm", typeof(Texture2D));
			_separator = (Texture2D)Resources.Load("separator", typeof(Texture2D));
			_helpButton = (Texture2D)Resources.Load("help",typeof(Texture2D));
			_browseButton = (Texture2D)Resources.Load("folder",typeof(Texture2D));
			
			_username = GetString( "username", _username );
			_password = GetString( "password", _password );
			_activeWorkbookname = GetString( "activeworkbookname", _activeWorkbookname );
			_objDBResourcesDirectory = GetString ( "objDBResourcesDirectory", _objDBResourcesDirectory );
			_objDBEditorDirectory = GetString ( "objDBEditorDirectory", _objDBEditorDirectory );
			_staticDBResourcesDirectory = GetString ( "staticDBResourcesDirectory", _staticDBResourcesDirectory );
			_nguiDirectory = GetString ( "nguiDirectory",_nguiDirectory );
			_xmlDirectory = GetString ( "xmlDirectory",_xmlDirectory );
			_jsonDirectory = GetString ( "jsonDirectory",_jsonDirectory );
			_playmakerDirectory = GetString ( "playmakerDirectory",_playmakerDirectory );
			_daikonforgeDirectory = GetString ( "daikonforgeDirectory",_daikonforgeDirectory );
			_editorLanguage = GetString( "editorLanguage", _editorLanguage );
			_saveCredentials = GetBool( "saveCredientials", _saveCredentials );
			_autoLogin = GetBool( "autoLogin", _autoLogin );
			_useObjDB = GetBool( "useObjDB", _useObjDB );
			_useStaticDB = GetBool( "useStaticDB", _useStaticDB );
			_useNGUI = GetBool( "useNGUI", _useNGUI );
			_useNGUILegacy = GetBool ("useNGUILegacy", _useNGUILegacy);
			_useXML = GetBool( "useXML", _useXML );
			_useJSON = GetBool( "useJSON", _useJSON );
			_useDaikonForge = GetBool( "useDaikonForge", _useDaikonForge );
			_usePlaymaker = GetBool( "usePlaymaker", _usePlaymaker );
			_languagesIndex = GetInt( "languagesIndex", _languagesIndex );
			_projectPath = Application.dataPath;
			_currentHelpDoc = string.Empty;
			
			
			
			System.Net.ServicePointManager.ServerCertificateValidationCallback = Validator;
			_service = new Google.GData.Spreadsheets.SpreadsheetsService("UnityGoogleFu");
			_authenticateTick = -1;
			
			if ( System.IO.Directory.GetFiles( Application.dataPath, "NGUITools.cs", System.IO.SearchOption.AllDirectories).Length > 0 )
				_foundNGUI = true;
			
			if ( System.IO.Directory.GetFiles( Application.dataPath, "PlayMaker.dll", System.IO.SearchOption.AllDirectories).Length > 0 )
				_foundPlaymaker = true;
			
			if ( System.IO.Directory.GetFiles( Application.dataPath, "dfScriptLite.dll", System.IO.SearchOption.AllDirectories).Length > 0 )
				_foundDaikonForge = true;
			
			if ( _autoLogin == true )
			{
				DoRefreshWorkbooks = true;
			}
			else
			{
				string tmpManualWorkbooks = GetString( "manualworkbookurls", System.String.Empty );
				string [] split_ManualWorkbooks = tmpManualWorkbooks.Split(new char[]{'|'}, System.StringSplitOptions.RemoveEmptyEntries);
				foreach ( string s in split_ManualWorkbooks )
				{
					WorkBookInfo info = AddManualWorkbookByURL(s);
					if ( info != null && info.Title == _activeWorkbookname )
						_activeWorkbook = info;
				}
			}
		}
		
		private int count = 0;
		
		void Update()
		{
			
			if ( DoRefreshWorkbooks == true )
			{
				_editorWorking = Localize( "ID_AUTHENTICATING" );
				if ( _authenticateTick == -1 )
					_authenticateTick = 4;
				
				if ( _authenticateTick == 0 )
				{
					DoRefreshWorkbooks = false;
					RefreshWorkbooks();
					_editorWorking = System.String.Empty;
				}
				_authenticateTick--;
				Repaint();
			}
			
			if ( EditorApplication.isCompiling == false &&
			    _advancedDatabaseObjectInfo.Count > 0 )
			{
				foreach( AdvancedDatabaseInfo info in _advancedDatabaseObjectInfo)
				{
					if ( info == null )
						continue;
					
					if ( info.ComponentName == string.Empty )
						continue;
					
					Component toDestroy = info.DatabaseAttachObject.GetComponent(info.ComponentName);
					if(toDestroy != null)
						UnityEngine.Object.DestroyImmediate(toDestroy);
					GoogleFuComponentBase comp = (GoogleFuComponentBase)info.DatabaseAttachObject.AddComponent(info.ComponentName);
					
					if(comp == null)
					{
						AssetDatabase.ImportAsset(info.ComponentName + ".cs", ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
						
						if (count < 20)
						{
							count++;
							return;
						}
						else
						{
							Debug.LogError("Could not add google fu component base " + info.ComponentName + ".cs");
							continue;
						}
					}
					
					System.Collections.Generic.List<string> rowInputs = new System.Collections.Generic.List<string>();
					
					count = 0;
					
					// Iterate through each row, printing its cell values.
					foreach (string entryVal in info.entryStrings)
					{
						
						rowInputs.Add(entryVal);
						count++;
						
						if ( count % info.entryStride == 0 )
						{
							comp.AddRowGeneric(rowInputs);
							rowInputs.Clear();
						}
					}
					
				}
				
				_advancedDatabaseObjectInfo.Clear();
			}
		}
		
		void ClearMessages()
		{
			_editorInfo = System.String.Empty;
			_editorWarning = System.String.Empty;
			_editorWorking = System.String.Empty;
			_editorPathInfo = System.String.Empty;
			_editorException = System.String.Empty;
		}
		
		void SetString ( string stringID, string input )
		{
			EditorPrefs.SetString( System.IO.Path.Combine(Application.dataPath, stringID ), input );
		}
		
		string GetString ( string stringID, string defaultString )
		{
			return EditorPrefs.GetString( System.IO.Path.Combine(Application.dataPath, stringID ), defaultString );
		}
		
		void SetInt ( string intID, int input )
		{
			EditorPrefs.SetInt( System.IO.Path.Combine(Application.dataPath, intID ), input );
		}
		
		int GetInt ( string intID, int defaultInt )
		{
			return EditorPrefs.GetInt( System.IO.Path.Combine(Application.dataPath, intID ), defaultInt );
		}
		
		void SetFloat ( string floatID, float input )
		{
			EditorPrefs.SetFloat( System.IO.Path.Combine(Application.dataPath, floatID ), input );
		}
		
		float GetFloat ( string floatID, float defaultFloat )
		{
			return EditorPrefs.GetFloat( System.IO.Path.Combine(Application.dataPath, floatID ), defaultFloat );
		}
		
		bool SetBool ( string boolID, bool input )
		{
			EditorPrefs.SetBool( System.IO.Path.Combine(Application.dataPath, boolID ), input );
			return input;
		}
		
		bool GetBool ( string boolID, bool defaultBool )
		{
			return EditorPrefs.GetBool( System.IO.Path.Combine(Application.dataPath, boolID ), defaultBool );
		}
		
		string Localize( string textID )
		{
			LocalizationRow row =  Localization.Instance.GetRow( textID );
			if( row != null )
			{
				return row.GetStringData( _editorLanguage );
			}
			return "Unable to find string ID: " + textID;
			
		}
		
		bool DrawToggle( string textID, bool defaultValue )
		{
			return GUILayout.Toggle(defaultValue, " " + Localize( textID ));
		}
		
		void DrawSeparator ()
		{
			GUILayout.Label (_separator);
		}
	}
}
