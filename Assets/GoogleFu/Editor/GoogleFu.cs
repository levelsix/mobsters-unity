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
		public static string _editorException = System.String.Empty;
		public static bool DoRefreshWorkbooks = false;
		public static bool CreatingWorkbook = false;
		private static int ANONCOLNAME = 0;
		
		private Google.GData.Spreadsheets.SpreadsheetsService _service;
		private Google.GData.Spreadsheets.SpreadsheetQuery _query = new Google.GData.Spreadsheets.SpreadsheetQuery();
		private WorkBookInfo _activeWorkbook = null;
		private System.Collections.Generic.List<WorkBookInfo> _workbooks = new System.Collections.Generic.List<WorkBookInfo>();
		private System.Collections.Generic.List<WorkBookInfo> _manualworkbooks = new System.Collections.Generic.List<WorkBookInfo>();
		private System.Collections.Generic.List<string> _manualworkbookurls = new System.Collections.Generic.List<string>();
		private System.Collections.Generic.List<AdvancedDatabaseInfo> _advancedDatabaseObjectInfo = new System.Collections.Generic.List<AdvancedDatabaseInfo>();
		private System.Collections.Generic.Dictionary<string,GameObject> _advancedDatabaseObjects = new System.Collections.Generic.Dictionary<string, GameObject>();
		private System.Collections.Generic.Dictionary<string,bool> _showHelp = new System.Collections.Generic.Dictionary<string, bool>();
		private GF_PAGE _currentPage = GF_PAGE.Settings;
		private Vector2 scrollPos = new Vector2(0.0f,0.0f);
		private Color _defaultBGColor;
		private Color _defaultFGColor;
		private Color _labelHeaderColor;
		private Color _selectedTabColor;
		private Color _unselectedTabColor;
		private Color _pathLabelBGColor;
		private Color _pathLabelFGColor;
		private GUIStyle _largeLabelStyle;
		private GUIStyle _pathLabelStyle = null;
		private Texture2D _pathLabelBG = null;
		private Texture _unityLogo = null;
		private Texture _litteratusLogo = null;
		private Texture _separator = null;
		private Texture _helpButton = null;
		private Texture _browseButton = null;
		private string _username = System.String.Empty;
		private string _password = System.String.Empty;
		private string _editorInfo = System.String.Empty;
		private string _editorWarning = System.String.Empty;
		private string _editorWorking = System.String.Empty;
		private string _editorPathInfo = System.String.Empty;
		private string _manualurl = System.String.Empty;
		private string _currentHelpDoc = System.String.Empty;
		private string _editorLanguage = "EN";
		private string _objDBResourcesDirectory = System.String.Empty;
		private string _objDBEditorDirectory = System.String.Empty;
		private string _staticDBResourcesDirectory = System.String.Empty;
		private string _nguiDirectory = System.String.Empty;
		private string _daikonforgeDirectory = System.String.Empty;
		private string _xmlDirectory = System.String.Empty;
		private string _jsonDirectory = System.String.Empty;
		private string _playmakerDirectory = System.String.Empty;
		private string _activeWorkbookname = System.String.Empty;
		private string _workbookUploadPath = System.String.Empty;
		private string _projectPath = System.String.Empty;
		private int _initDraw = 0;
		private int _authenticateTick = -1;
		private int _languagesIndex = 8;
		private int _curIndent = 0;
		private bool _saveCredentials = false;
		private bool _autoLogin = false;
		private bool _useObjDB = false;
		private bool _useStaticDB = false;
		private bool _useNGUI = false;
		private bool _useNGUILegacy = false;
		private bool _useDaikonForge = false;
		private bool _useXML = false;
		private bool _useJSON = false;
		private bool _usePlaymaker = false;
		private bool _isProSkin = false;
		private bool _authorized = false;
		private bool _foundPlaymaker = false;
		private bool _foundNGUI = false;
		private bool _foundDaikonForge = false;
		
		// Settings GUI
		private bool _bShowAuthentication = true;
		private bool _bShowLanguage = false;
		private bool _bShowPaths = false;
		
		// Workbooks GUI
		private bool _bShowAccountWorkbooks = true;
		private bool _bShowManualWorkbooks = false;
		private bool _bShowCreateWorkbook = false;
		
		// Tools GUI
		
		// Help GUI
		private bool _bShowMain = true;
		private bool _bShowDocs = false;
		
		public static bool Validator (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, 
		                              System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
		
		void RefreshWorkbooks()
		{
			if ( _username != System.String.Empty && _password != System.String.Empty )
			{
				Google.GData.Spreadsheets.SpreadsheetFeed feed = null;
				bool doWorkbookQuery = false;
				try
				{
					_service.setUserCredentials(_username, _password);
					var token = _service.QueryClientLoginToken();
					_service.SetAuthenticationToken(token);
					_workbooks.Clear();
					
					feed = _service.Query(_query);
					
					if ( feed.Entries.Count == 0 )
					{
						_editorInfo = Localize( "ID_NO_DB_ERROR" );
					}
					else
					{
						doWorkbookQuery = true;
					}
				}
				catch( System.Exception ex )
				{
					Debug.Log( ex.Message );
					_editorInfo = Localize( "ID_AUTH_ERROR" );
				}
				
				if( doWorkbookQuery == true && feed != null )
				{
					
					foreach( Google.GData.Spreadsheets.SpreadsheetEntry entry in feed.Entries )
					{
						try
						{
							WorkBookInfo info = _workbooks.Find( 
							                                    delegate(WorkBookInfo i)
							                                    {
								return i.Title == entry.Feed.Title.Text;
							}
							);
							if ( info == null )
							{
								info = new WorkBookInfo( entry );
								_workbooks.Add( info );
							}
							if ( info.Title == _activeWorkbookname )
								_activeWorkbook = info;
						}
						catch( System.Exception ex )
						{
							Debug.Log ( Localize( "ID_INACCESSIBLE_ERROR") );
							Debug.Log( ex.Message );
						}
						
					}
					
					_authorized = true;
					
				}
				
			}
			
			_manualworkbookurls.Clear();
			string tmpManualWorkbooks = GetString( "manualworkbookurls", System.String.Empty );
			string [] split_ManualWorkbooks = tmpManualWorkbooks.Split(new char[]{'|'},System.StringSplitOptions.RemoveEmptyEntries);
			foreach( string s in split_ManualWorkbooks )
			{
				WorkBookInfo info = AddManualWorkbookByURL ( s );
				if ( info != null && info.Title == _activeWorkbookname )
					_activeWorkbook = info;
				
			}
		}
		
		void DrawAuthenticationGUI ()
		{
			
			if ( !_authorized )
			{
				EditorGUILayout.BeginHorizontal();
				DrawHelpButtonGUI ("ID_HELP_AUTHENTICATION");
				GUI.color = Color.yellow;
				DrawLabelHeader ( "ID_AUTH_OPTIONAL" );
				GUI.color = _defaultFGColor;
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				DrawHelpButtonGUI ("ID_HELP_GOOGLE_USER_NAME");
				string oldUsername = _username;
				_username = EditorGUILayout.TextField( Localize( "ID_GOOGLE_USERNAME" ), _username );
				if ( _username != oldUsername && _saveCredentials == true )
				{
					SetString( "username", _username );
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				DrawHelpButtonGUI ("ID_HELP_GOOGLE_PASSWORD");
				string oldPassword = _password;
				_password = EditorGUILayout.PasswordField( Localize( "ID_GOOGLE_PASSWORD" ), _password );
				if ( _password != oldPassword && _saveCredentials == true )
				{
					SetString( "password", _password );
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				DrawHelpButtonGUI ("ID_HELP_SAVE_CREDENTIALS");
				GUI.SetNextControlName("SaveCredentials");
				bool oldSaveCredentials = _saveCredentials;
				_saveCredentials = DrawToggle( "ID_SAVE_CREDENTIALS", _saveCredentials);
				if ( _saveCredentials != oldSaveCredentials )
				{
					SetBool( "saveCredientials", _saveCredentials );
					if ( _saveCredentials == false )
					{
						_username = System.String.Empty;
						_password = System.String.Empty;
						SetString( "username", System.String.Empty );
						SetString( "password", System.String.Empty );
						GUI.FocusControl("SaveCredentials");
						Repaint();
					}
					else
					{
						SetString( "username", _username );
						SetString( "password", _password );
					}
				}
				if( _saveCredentials == true )
				{
					DrawHelpButtonGUI ("ID_HELP_AUTO_LOG_IN");
					bool oldAutoLogin = _autoLogin;
					_autoLogin = DrawToggle( "ID_AUTO_LOGIN", _autoLogin );
					if ( _autoLogin != oldAutoLogin )
					{
						SetBool ("autoLogin", _autoLogin);
					}
				}
				else
				{
					_autoLogin = false;
					SetBool ("autoLogin", _autoLogin);
				}
				EditorGUILayout.EndHorizontal();
				
				if ( DoRefreshWorkbooks == true )
					GUI.enabled = false;
				if ( GUILayout.Button( Localize( "ID_AUTHORIZE" ) ) )
				{
					ClearMessages();
					DoRefreshWorkbooks = true;
				}
				GUI.enabled = true;
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				DrawHelpButtonGUI("ID_HELP_ACTIVE_ACCOUNT");
				DrawLabelHeader ( "ID_ACTIVE_ACCOUNT" );
				GUILayout.Label ( _username, _largeLabelStyle );
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Separator();
				
				if ( GUILayout.Button( Localize( "ID_LOGOUT" ) ) )
				{
					ClearMessages();
					
					_authorized = false;
					_workbooks.Clear();
					_activeWorkbook = null;
					_activeWorkbookname = System.String.Empty;
					
					if ( GetBool( "saveCredientials", _saveCredentials ) != true )
					{
						_username = System.String.Empty;
						_password = System.String.Empty;
						SetString( "username", _username );
						SetString( "password", _password );
					}
				}
				
			}
		}
		
		bool DrawActiveWorkbookGUI ()
		{
			bool bActive = true;
			
			
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI("ID_HELP_ACTIVE_WORKBOOK");
			DrawLabelHeader ( "ID_ACTIVE_WORKBOOK" );
			GUILayout.Label ( _activeWorkbook.Title, _largeLabelStyle );
			if ( GUILayout.Button( Localize( "ID_OPEN_URL" ), EditorStyles.miniButton, GUILayout.Width(70) ) )
			{
				ClearMessages();
				Application.OpenURL( _activeWorkbook.Url );
			}
			if ( GUILayout.Button( Localize( "ID_DEACTIVATE" ), EditorStyles.miniButton, GUILayout.Width(70) ) )
			{
				ClearMessages();
				bActive = false;
				_activeWorkbook = null;
				_activeWorkbookname = System.String.Empty;
				SetString( "activeworkbookname", System.String.Empty);
				_currentPage = GF_PAGE.Workbooks;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();
			return bActive;
		}
		
		WorkBookInfo AddManualWorkbookByURL (string manualUrl)
		{
			WorkBookInfo info = null;
			if ( manualUrl == System.String.Empty )
			{
				_editorInfo = Localize( "ID_NO_URL_ERROR" );
				return null;
			}
			string key = manualUrl.Substring(manualUrl.IndexOf("key=") + 4);
			key = key.Split(new char[]{ '&' })[0];
			try
			{
				Google.GData.Spreadsheets.WorksheetQuery singleQuery = new Google.GData.Spreadsheets.WorksheetQuery(key, "public", "values");
				Google.GData.Spreadsheets.WorksheetFeed feed = _service.Query(singleQuery);
				string finalUrl = manualUrl.Split(new char[]{ '&' })[0];
				if ( feed != null )
				{
					foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in feed.Entries )
					{
						info = _manualworkbooks.Find( 
						                             delegate(WorkBookInfo i)
						                             {
							return i.Url == finalUrl;
						}
						);
						if ( info == null )
						{
							info = _workbooks.Find( 
							                       delegate(WorkBookInfo i)
							                       {
								return i.Url == finalUrl;
							}
							);
						}
						if ( info == null )
						{
							info = new WorkBookInfo();
							
							_manualworkbooks.Add( info );
							if ( _manualworkbookurls.Contains( manualUrl ) == false )
								_manualworkbookurls.Add(manualUrl);
							string newManualWorkbookUrls = System.String.Empty;
							foreach( string s in _manualworkbookurls )
							{
								newManualWorkbookUrls += s + "|";
							}
							SetString( "manualworkbookurls", newManualWorkbookUrls);
						}
						
						info.AddWorksheetEntry(entry, finalUrl);
					}
				}
			}
			catch
			{
				_editorInfo = Localize( "ID_INVALID_URL_ERROR" );
			}
			
			return info;
		}
		
		void DrawAccountWorkbooksGUI ()
		{
			
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_ACCOUNT_WORKBOOKS");
			DrawLabelHeader ( "ID_ACCOUNT_WORKBOOK" );
			EditorGUILayout.EndHorizontal();
			
			foreach( WorkBookInfo wbInfo in _workbooks )
			{
				if ( wbInfo == _activeWorkbook )
					GUI.enabled = false;
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label ( wbInfo.Title, EditorStyles.miniLabel  );
				
				if ( GUILayout.Button( Localize( "ID_OPEN_URL" ), EditorStyles.miniButton, GUILayout.Width(65) ) )
				{
					ClearMessages();
					Application.OpenURL( wbInfo.Url );
				}
				
				if ( GUILayout.Button( Localize( "ID_ACTIVATE" ), EditorStyles.miniButton, GUILayout.Width(65) ) )
				{
					ClearMessages();
					_activeWorkbook = wbInfo;
					SetString( "activeworkbookname", wbInfo.Title);
					_currentPage = GF_PAGE.Toolbox;
				}
				EditorGUILayout.EndHorizontal();
				GUI.enabled = true;
			}
		}
		
		void DrawPathLabel ( string path )
		{
			if ( _pathLabelBG == null )
				_pathLabelBG = new Texture2D(1,1);
			if ( _pathLabelStyle == null )
				_pathLabelStyle = new GUIStyle();
			
			_pathLabelBG.SetPixel(0,0, _pathLabelBGColor );
			_pathLabelStyle.normal.textColor = _pathLabelFGColor;
			_pathLabelStyle.normal.background = _pathLabelBG;
			GUILayout.Label( path, _pathLabelStyle );
		}
		
		void DrawCreateWorkbookGUI()
		{
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_UPLOAD_WORKBOOK");
			DrawLabelHeader ( "ID_CREATE_WORKBOOK" );
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.BeginHorizontal();
			Indent(1);
			GUI.backgroundColor = Color.clear;
			GUI.color = Color.yellow;
			bool bDoSave = false;
			GUI.SetNextControlName("Clear");
			if ( GUILayout.Button( _browseButton, EditorStyles.toolbarButton, GUILayout.Width(24)) )
			{
				ClearMessages();
				string workbookpath = EditorUtility.OpenFilePanel(
					Localize ("ID_SELECT_UPLOAD_WORKBOOK_PATH" ), EditorApplication.applicationPath, "*.xls;*.xlsx;*.ods;*.csv;*.txt;*.tsv");
				
				if ( workbookpath != System.String.Empty )
				{
					if ( ( workbookpath.ToLower().IndexOf(".xls") != -1 ) ||
					    ( workbookpath.ToLower().IndexOf(".xlsx") != -1 ) ||
					    ( workbookpath.ToLower().IndexOf(".ods") != -1 ) ||
					    ( workbookpath.ToLower().IndexOf(".csv") != -1 ) ||
					    ( workbookpath.ToLower().IndexOf(".txt") != -1 ) ||
					    ( workbookpath.ToLower().IndexOf(".tsv") != -1 ) )
					{
						bDoSave = true;
					}
					
					if ( bDoSave == true )
					{
						_workbookUploadPath = workbookpath;
						GUI.FocusControl("Clear");
						Repaint();
					}
					else
					{
						_editorWarning = Localize( "ID_ERROR_UPLOAD_WORKBOOK_PATH" );
						Debug.LogWarning( _editorWarning );
					}
				}
			}
			GUI.backgroundColor = _defaultBGColor;
			GUI.color = _defaultFGColor;
			EditorGUILayout.TextField( _workbookUploadPath  );
			GUILayout.Label ( System.String.Empty, GUILayout.Width(5) );
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ( System.String.Empty );
			
			if( CreatingWorkbook == true )
				GUI.enabled = false;
			
			if ( GUILayout.Button( Localize( "ID_CREATE" ), EditorStyles.miniButton, GUILayout.Width(65) ) )
			{
				ClearMessages();
				
				// We need a DocumentService
				Google.GData.Documents.DocumentsService service = new Google.GData.Documents.DocumentsService("GoogleFuUploader");
				Google.GData.Client.ClientLoginAuthenticator authenticator = new Google.GData.Client.ClientLoginAuthenticator( "UnityGoogleFu", Google.GData.Client.ServiceNames.Documents,_username,_password);
				//service.setUserCredentials(_username, _password);
				
				// Instantiate a DocumentEntry object to be inserted.
				Google.GData.Documents.DocumentEntry entry = new Google.GData.Documents.DocumentEntry();
				
				CreatingWorkbook = true;
				entry.MediaSource = new Google.GData.Client.MediaFileSource(_workbookUploadPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
				
				// Define the resumable upload link
				System.Uri createUploadUrl = new System.Uri("https://docs.google.com/feeds/upload/create-session/default/private/full");
				Google.GData.Client.AtomLink link = new Google.GData.Client.AtomLink(createUploadUrl.AbsoluteUri);
				link.Rel = Google.GData.Client.ResumableUpload.ResumableUploader.CreateMediaRelation;
				
				entry.Links.Add(link);
				
				// Set the service to be used to parse the returned entry
				entry.Service = service;
				
				
				// Instantiate the ResumableUploader component.
				Google.GData.Client.ResumableUpload.ResumableUploader uploader = new Google.GData.Client.ResumableUpload.ResumableUploader();
				
				// Set the handlers for the completion and progress events
				uploader.AsyncOperationCompleted += new Google.GData.Client.AsyncOperationCompletedEventHandler(OnSpreadsheetUploadDone);
				uploader.AsyncOperationProgress += new Google.GData.Client.AsyncOperationProgressEventHandler(OnSpreadsheetUploadProgress);
				
				// Start the upload process
				uploader.InsertAsync(authenticator, entry, new object());
				_editorInfo = Localize( "ID_CREATING_DATABASE_MESSAGE" );
				
				//Repaint();
			}
			GUI.enabled = true;
			GUILayout.Label ( System.String.Empty, GUILayout.Width(5) );
			EditorGUILayout.EndHorizontal();
		}
		
		static void OnSpreadsheetUploadDone(object sender, Google.GData.Client.AsyncOperationCompletedEventArgs e)
		{
			Google.GData.Documents.DocumentEntry entry = e.Entry as Google.GData.Documents.DocumentEntry;
			
			if ( entry.IsSpreadsheet != true )
				GoogleFuEditor._editorException = e.Error.Message;
			
			GoogleFuEditor.DoRefreshWorkbooks = true;
			GoogleFuEditor.CreatingWorkbook = false;
		}
		
		static void OnSpreadsheetUploadProgress(object sender, Google.GData.Client.AsyncOperationProgressEventArgs e)
		{
			//int percentage = e.ProgressPercentage;
		}
		
		void DrawManualWorkbooksGUI ()
		{
			if ( _manualworkbooks.Count > 0 )
			{
				EditorGUILayout.BeginHorizontal();
				DrawHelpButtonGUI ("ID_HELP_MANUAL_WORKBOOKS");
				DrawLabelHeader ( "ID_MANUAL_WORKBOOK" );
				EditorGUILayout.EndHorizontal();
				
				foreach( WorkBookInfo wbInfo in _manualworkbooks )
				{
					if ( wbInfo == _activeWorkbook )
						GUI.enabled = false;
					
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label ( wbInfo.Title, EditorStyles.miniLabel );
					
					if ( GUILayout.Button( Localize( "ID_OPEN_URL" ), EditorStyles.miniButton, GUILayout.Width(65) ) )
					{
						ClearMessages();
						Application.OpenURL( wbInfo.Url );
					}
					
					if ( GUILayout.Button( Localize( "ID_ACTIVATE" ), EditorStyles.miniButton, GUILayout.Width(65) ) )
					{
						ClearMessages();
						_activeWorkbook = wbInfo;
						SetString( "activeworkbookname", wbInfo.Title);
						_currentPage = GF_PAGE.Toolbox;
					}
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
			}
			
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_ADD_MANUAL_WORKBOOK");
			DrawLabelHeader ( "ID_ADD_WORKBOOK_MANUALLY" );
			EditorGUILayout.EndHorizontal();
			
			
			
			_manualurl = EditorGUILayout.TextField( _manualurl  );
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ( System.String.Empty );
			
			if ( GUILayout.Button( Localize( "ID_OPEN_URL" ), EditorStyles.miniButton, GUILayout.Width(65) ) )
			{
				ClearMessages();
				Application.OpenURL( _manualurl );
			}
			if ( GUILayout.Button( Localize( "ID_ADD_WORKBOOK" ), EditorStyles.miniButton, GUILayout.Width(65) ) )
			{
				ClearMessages();
				AddManualWorkbookByURL (_manualurl);
			}
			EditorGUILayout.EndHorizontal();
		}
		
		
		void DrawHelpButtonGUI (string hdString)
		{
			bool bShow = false;
			if ( _showHelp.ContainsKey( hdString ) )
				bShow = _showHelp[ hdString ];
			else
				_showHelp.Add( hdString, false );
			
			if ( bShow == false )
			{
				GUI.backgroundColor = Color.clear;
				bShow = GUILayout.Button(_helpButton, EditorStyles.toolbarButton, GUILayout.Width(24));
				GUI.backgroundColor = _defaultBGColor;
				_showHelp[ hdString ] = bShow;
			}
			else
			{
				GUI.backgroundColor = Color.clear;
				bShow = !GUILayout.Button(_helpButton, EditorStyles.toolbarButton, GUILayout.Width(24));
				GUI.backgroundColor = _defaultBGColor;
				{
					EditorGUILayout.HelpBox( Localize( hdString ), MessageType.Info );
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					Indent(_curIndent);
					GUILayout.Label("", GUILayout.Width(24));
				}
				_showHelp[ hdString ] = bShow;
			}
		}
		
		void DrawLabelHeader ( string labelID )
		{
			GUI.color = _labelHeaderColor;
			GUILayout.Label (Localize( labelID ), _largeLabelStyle);
			GUI.color = _defaultFGColor;
		}
		
		void DrawChooseLanguageGUI ()
		{
			
			string [] ls_options = new string[ Language.languageStrings.GetUpperBound(0) ];
			string [] ls_code = new string[ Language.languageStrings.GetUpperBound(0) ];
			string [] ls_name = new string[ Language.languageStrings.GetUpperBound(0) ];
			for( int i = 0; i < Language.languageStrings.GetUpperBound(0); ++i )
			{
				ls_options[i] = Language.languageStrings[i,0];
				ls_code[i] = Language.languageStrings[i,1];
				ls_name[i] = Language.languageStrings[i,2];
			}
			
			int oldIdx = _languagesIndex;
			
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_SELECT_LANGUAGE");
			DrawLabelHeader ( "ID_SELECT_LANGUAGE" );
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			
			GUI.SetNextControlName("Clear");
			_languagesIndex = EditorGUILayout.Popup(_languagesIndex, ls_options);
			
			if( oldIdx != _languagesIndex)
			{
				_editorLanguage = Language.languageStrings[_languagesIndex,1];
				SetString( "editorLanguage", _editorLanguage );
				SetInt( "languagesIndex", _languagesIndex);
				GUI.FocusControl("Clear");
				Repaint();
			}
		}
		
		void DrawCreatePathsGUI()
		{
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_CREATE_PATHS");
			
			GUI.SetNextControlName("PathRefocus");
			if ( GUILayout.Button( Localize("ID_GENERATE_PATHS"), EditorStyles.toolbarButton ) )
			{
				string googlefugenPath = System.IO.Path.Combine( _projectPath, "GoogleFuGen" );
				if( !System.IO.Directory.Exists( googlefugenPath ) )
					System.IO.Directory.CreateDirectory( googlefugenPath );
				
				// ObjDB
				string objdbPath = System.IO.Path.Combine( googlefugenPath, "ObjDB" );
				if( !System.IO.Directory.Exists( objdbPath ) )
					System.IO.Directory.CreateDirectory( objdbPath );
				
				string objdbResourcesPath = System.IO.Path.Combine( objdbPath, "Resources" );
				if( !System.IO.Directory.Exists( objdbResourcesPath ) )
					System.IO.Directory.CreateDirectory( objdbResourcesPath );
				
				SetString ( "objDBResourcesDirectory", objdbResourcesPath );
				_objDBResourcesDirectory = objdbResourcesPath;
				
				string objdbEditorPath = System.IO.Path.Combine( objdbPath, "Editor" );
				if( !System.IO.Directory.Exists( objdbEditorPath ) )
					System.IO.Directory.CreateDirectory( objdbEditorPath );
				
				SetString ( "objDBEditorDirectory", objdbEditorPath );
				_objDBEditorDirectory = objdbEditorPath;
				
				// StaticDB
				string staticdbPath = System.IO.Path.Combine( googlefugenPath, "StaticDB" );
				if( !System.IO.Directory.Exists( staticdbPath ) )
					System.IO.Directory.CreateDirectory( staticdbPath );
				
				string staticdbResourcesPath = System.IO.Path.Combine( staticdbPath, "Resources" );
				if( !System.IO.Directory.Exists( staticdbResourcesPath ) )
					System.IO.Directory.CreateDirectory( staticdbResourcesPath );
				
				SetString ( "staticDBResourcesDirectory", staticdbResourcesPath );
				_staticDBResourcesDirectory = staticdbResourcesPath;
				
				//JSON
				string jsonPath = System.IO.Path.Combine( googlefugenPath, "JSON" );
				if( !System.IO.Directory.Exists( jsonPath ) )
					System.IO.Directory.CreateDirectory( jsonPath );
				
				SetString ( "jsonDirectory", jsonPath );
				_jsonDirectory = jsonPath;
				
				//XML
				string xmlPath = System.IO.Path.Combine( googlefugenPath, "XML" );
				if( !System.IO.Directory.Exists( xmlPath ) )
					System.IO.Directory.CreateDirectory( xmlPath );
				
				SetString ( "xmlDirectory", xmlPath );
				_xmlDirectory = xmlPath;
				
				//NGUI
				if ( _foundNGUI == true )
				{
					string nguiPath = System.IO.Path.Combine( googlefugenPath, "NGUI" );
					if( !System.IO.Directory.Exists( nguiPath ) )
						System.IO.Directory.CreateDirectory( nguiPath );
					
					SetString ( "nguiDirectory", nguiPath );
					_nguiDirectory = nguiPath;
				}
				
				//Daikon Forge
				if ( _foundDaikonForge == true )
				{
					string daikonforgePath = System.IO.Path.Combine( googlefugenPath, "DAIKONFORGE" );
					if( !System.IO.Directory.Exists( daikonforgePath ) )
						System.IO.Directory.CreateDirectory( daikonforgePath );
					
					SetString ( "daikonforgeDirectory", daikonforgePath );
					_daikonforgeDirectory = daikonforgePath;
				}
				
				// PLAYMAKER
				if ( _foundPlaymaker == true )
				{
					// attempt to find the playmaker actions directory
					// We already know that the playmaker dll exists, but we need to find the actual path
					string [] playmakerPaths = System.IO.Directory.GetFiles( Application.dataPath, "PlayMaker.dll", System.IO.SearchOption.AllDirectories);
					string playmakerPath = System.String.Empty;
					if (playmakerPaths.Length > 0 )
					{
						// We are just going to use the first entry. If there is more than 1 entry, there are bigger issues
						string fileName = playmakerPaths[0];
						System.IO.FileInfo fileInfo= new System.IO.FileInfo(fileName);
						playmakerPath = fileInfo.DirectoryName;
					}
					
					if ( playmakerPath != System.String.Empty )
					{
						string playmakerActionsPath = System.IO.Path.Combine( playmakerPath, "Actions" );
						if( System.IO.Directory.Exists( playmakerActionsPath ) )
						{
							// We have found the Playmaker Actions dir!
							SetString ( "playmakerDirectory", playmakerActionsPath );
							_playmakerDirectory = playmakerActionsPath;
						}
						else
						{
							// The actions subdirectory doesn't exist? Rather than making it in the playmaker directory,
							// We will just use our GoogleFuGen path instead and let the user figure it out
							string pmPath = System.IO.Path.Combine( googlefugenPath, "PlayMaker" );
							if( !System.IO.Directory.Exists( pmPath ) )
								System.IO.Directory.CreateDirectory( pmPath );
							
							SetString ( "playmakerDirectory", pmPath );
							_playmakerDirectory = pmPath;
						}
					}	
				}
				GetPathErrors();
				GUI.FocusControl("PathRefocus");
				Repaint();
			}
			EditorGUILayout.EndHorizontal();
		}
		
		void DrawEnableDBObjGUI()
		{
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_GAMEOBJECT_DB_ENABLE");
			bool oldUseObjDB = _useObjDB;
			_useObjDB = DrawToggle( "ID_ENABLE_DB_OBJ", oldUseObjDB );
			EditorGUILayout.EndHorizontal();
			
			if ( oldUseObjDB != _useObjDB )
			{
				SetBool( "useObjDB", _useObjDB );
			}
			
			if( _useObjDB == true )
			{
				EditorGUILayout.Separator();
				bool bDoSave = true;
				string oldObjDBResourcesDirectory = _objDBResourcesDirectory;
				string oldObjDBEditorDirectory = _objDBEditorDirectory;
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				DrawHelpButtonGUI ("ID_HELP_GAMEOBJECT_RESOURCES_PATH");
				DrawLabelHeader ( "ID_SELECT_RESOURCES_DIRECTORY" );
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				GUI.backgroundColor = Color.clear;
				GUI.color = Color.yellow;
				if ( GUILayout.Button( _browseButton, EditorStyles.toolbarButton, GUILayout.Width(24)) )
				{
					ClearMessages();
					string objDBResourcesDirectory = EditorUtility.SaveFolderPanel(
						oldObjDBResourcesDirectory,EditorApplication.applicationPath, System.String.Empty);
					
					if ( ( objDBResourcesDirectory.Length == 0 ) ||
					    ( objDBResourcesDirectory.IndexOf( _projectPath ) != 0 ) ||
					    ( objDBResourcesDirectory.ToUpper().IndexOf("/RESOURCES") == -1 ) )
					{
						_editorPathInfo = Localize( "ID_ERROR_RESOURCES_DIRECTORY" );
						Debug.LogWarning( _editorPathInfo );
						bDoSave = false;
					}
					
					if ( (bDoSave == true) && (oldObjDBResourcesDirectory != objDBResourcesDirectory) )
					{
						SetString ( "objDBResourcesDirectory", objDBResourcesDirectory );
						_objDBResourcesDirectory = objDBResourcesDirectory;
					}
				}
				GUI.color = _defaultFGColor;
				GUI.backgroundColor = _defaultBGColor;
				DrawPathLabel ( _objDBResourcesDirectory );
				
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				DrawHelpButtonGUI ("ID_HELP_GAMEOBJECT_EDITOR_PATH");
				DrawLabelHeader ( "ID_SELECT_EDITOR_DIRECTORY" );
				EditorGUILayout.EndHorizontal();
				
				bDoSave = true;
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				GUI.backgroundColor = Color.clear;
				GUI.color = Color.yellow;
				if ( GUILayout.Button( _browseButton, EditorStyles.toolbarButton, GUILayout.Width(24)) )
				{
					ClearMessages();
					string objDBEditorDirectory = EditorUtility.SaveFolderPanel(
						oldObjDBEditorDirectory,EditorApplication.applicationPath, System.String.Empty);
					
					if ( ( objDBEditorDirectory.Length == 0 ) ||
					    ( objDBEditorDirectory.IndexOf( _projectPath ) != 0 ) ||
					    ( objDBEditorDirectory.ToUpper().IndexOf("/EDITOR") == -1 ) )
					{
						_editorPathInfo = Localize( "ID_ERROR_EDITOR_DIRECTORY" );
						Debug.LogWarning( _editorPathInfo );
						bDoSave = false;
					}
					
					if ( (bDoSave == true) && (oldObjDBEditorDirectory != objDBEditorDirectory) )
					{
						SetString ( "objDBEditorDirectory", objDBEditorDirectory );
						_objDBEditorDirectory = objDBEditorDirectory;
					}
				}
				GUI.color = _defaultFGColor;
				GUI.backgroundColor = _defaultBGColor;
				DrawPathLabel ( _objDBEditorDirectory );
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}
		
		void DrawEnableStaticObjGUI()
		{
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_STATIC_DB_ENABLE");
			bool oldUseStaticDB = _useStaticDB;
			_useStaticDB = DrawToggle( "ID_ENABLE_DB_STATIC", oldUseStaticDB );
			EditorGUILayout.EndHorizontal();
			
			if ( oldUseStaticDB != _useStaticDB )
			{
				SetBool( "useStaticDB", _useStaticDB );
			}
			
			if( _useStaticDB == true )
			{
				EditorGUILayout.Separator();
				bool bDoSave = true;
				string oldStaticDBResourcesDirectory = _staticDBResourcesDirectory;
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				DrawHelpButtonGUI ("ID_HELP_STATIC_RESOURCES_PATH");
				DrawLabelHeader ( "ID_SELECT_RESOURCES_DIRECTORY" );
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				GUI.backgroundColor = Color.clear;
				GUI.color = Color.yellow;
				if ( GUILayout.Button( _browseButton, EditorStyles.toolbarButton, GUILayout.Width(24)) )
				{
					ClearMessages();
					string staticDBResourcesDirectory = EditorUtility.SaveFolderPanel(
						oldStaticDBResourcesDirectory,EditorApplication.applicationPath, System.String.Empty);
					
					if ( ( staticDBResourcesDirectory.Length == 0 ) ||
					    ( staticDBResourcesDirectory.IndexOf( _projectPath ) != 0 ) ||
					    ( staticDBResourcesDirectory.ToUpper().IndexOf("/RESOURCES") == -1 ) )
					{
						_editorPathInfo = Localize( "ID_ERROR_RESOURCES_DIRECTORY" );
						Debug.LogWarning( _editorPathInfo );
						bDoSave = false;
					}
					
					if ( (bDoSave == true) && (oldStaticDBResourcesDirectory != staticDBResourcesDirectory) )
					{
						SetString ( "staticDBResourcesDirectory", staticDBResourcesDirectory );
						_staticDBResourcesDirectory = staticDBResourcesDirectory;
					}
				}
				GUI.color = _defaultFGColor;
				GUI.backgroundColor = _defaultBGColor;
				DrawPathLabel ( _staticDBResourcesDirectory );
				EditorGUILayout.EndHorizontal();
				
				bDoSave = true;
			}
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}
		
		void DrawChooseNGUIPathGUI()
		{
			if ( _foundNGUI == false )
				return;
			
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_NGUI_ENABLE");
			bool oldUseNGUI = _useNGUI;
			_useNGUI = DrawToggle( "ID_ENABLE_NGUI", _useNGUI );

			EditorGUILayout.EndHorizontal();
			
			if ( oldUseNGUI != _useNGUI )
			{
				SetBool( "useNGUI", _useNGUI );
			}

			if( _useNGUI == true )
			{
				EditorGUILayout.Separator();
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				DrawHelpButtonGUI ("ID_HELP_NGUI_PATH");
				DrawLabelHeader ( "ID_SELECT_NGUI_DIRECTORY" );
				EditorGUILayout.EndHorizontal();

				bool bDoSave = true;
				string oldNGUIPath = _nguiDirectory;
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				GUI.backgroundColor = Color.clear;
				GUI.color = Color.yellow;
				if ( GUILayout.Button( _browseButton, EditorStyles.toolbarButton, GUILayout.Width(24)) )
				{
					ClearMessages();
					string nguipath = EditorUtility.SaveFolderPanel(
						oldNGUIPath,EditorApplication.applicationPath, System.String.Empty);
					
					if ( ( nguipath.Length == 0 ) ||
					    ( nguipath.IndexOf( _projectPath ) != 0 ) )
					{
						_editorPathInfo = Localize( "ID_ERROR_NGUI_DIRECTORY" );
						Debug.LogWarning( _editorPathInfo );
						bDoSave = false;
					}
					
					if ( (bDoSave == true) && (oldNGUIPath != nguipath) )
					{
						SetString ( "nguiDirectory", nguipath );
						_nguiDirectory = nguipath;
					}
				}
				GUI.color = _defaultFGColor;
				GUI.backgroundColor = _defaultBGColor;
				DrawPathLabel ( _nguiDirectory );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				Indent(1);
				DrawHelpButtonGUI ("ID_HELP_NGUI_LEGACY");
				bool oldUseNGUILegacy = _useNGUILegacy;

				_useNGUILegacy = DrawToggle ("ID_ENABLE_NGUI_LEGACY", _useNGUILegacy);
				
				if ( oldUseNGUILegacy !=  _useNGUILegacy )
				{
					SetBool( "useNGUILegacy", _useNGUILegacy );
				}
				EditorGUILayout.EndHorizontal();



			}
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}
		
		void DrawChooseDaikonForgePathGUI()
		{
			if ( _foundDaikonForge == false )
				return;
			
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_DAIKONFORGE_ENABLE");
			bool oldUseDaikonForge = _useDaikonForge;
			_useDaikonForge = DrawToggle( "ID_ENABLE_DAIKONFORGE", _useDaikonForge );
			EditorGUILayout.EndHorizontal();
			
			if ( oldUseDaikonForge != _useDaikonForge )
			{
				SetBool( "useDaikonForge", _useDaikonForge );
			}
			
			if( _useDaikonForge == true )
			{
				EditorGUILayout.Separator();
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				DrawHelpButtonGUI ("ID_HELP_DAIKONFORGE_PATH");
				DrawLabelHeader ( "ID_SELECT_DAIKONFORGE_DIRECTORY" );
				EditorGUILayout.EndHorizontal();
				
				bool bDoSave = true;
				string oldDaikonForgePath = _daikonforgeDirectory;
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				GUI.backgroundColor = Color.clear;
				GUI.color = Color.yellow;
				if ( GUILayout.Button( _browseButton, EditorStyles.toolbarButton, GUILayout.Width(24)) )
				{
					ClearMessages();
					string daikonforgepath = EditorUtility.SaveFolderPanel(
						oldDaikonForgePath,EditorApplication.applicationPath, System.String.Empty);
					
					if ( ( daikonforgepath.Length == 0 ) ||
					    ( daikonforgepath.IndexOf( _projectPath ) != 0 ) )
					{
						_editorPathInfo = Localize( "ID_ERROR_DAIKONFORGE_DIRECTORY" );
						Debug.LogWarning( _editorPathInfo );
						bDoSave = false;
					}
					
					if ( (bDoSave == true) && (oldDaikonForgePath != daikonforgepath) )
					{
						SetString ( "daikonforgeDirectory", daikonforgepath );
						_daikonforgeDirectory = daikonforgepath;
					}
				}
				GUI.color = _defaultFGColor;
				GUI.backgroundColor = _defaultBGColor;
				DrawPathLabel ( _daikonforgeDirectory );
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}
		
		void DrawChooseXMLPathGUI()
		{
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_XML_ENABLE");
			bool oldUseXML = _useXML;
			_useXML = DrawToggle( "ID_ENABLE_XML", _useXML );
			EditorGUILayout.EndHorizontal();
			
			if ( oldUseXML != _useXML )
			{
				SetBool( "useXML", _useXML );
			}
			
			if( _useXML == true )
			{
				EditorGUILayout.Separator();
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				DrawHelpButtonGUI ("ID_HELP_XML_PATH");
				DrawLabelHeader ( "ID_SELECT_XML_DIRECTORY" );
				EditorGUILayout.EndHorizontal();
				
				bool bDoSave = true;
				string oldXMLPath = _xmlDirectory;
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				GUI.backgroundColor = Color.clear;
				GUI.color = Color.yellow;
				if ( GUILayout.Button( _browseButton, EditorStyles.toolbarButton, GUILayout.Width(24)) )
				{
					ClearMessages();
					string xmlpath = EditorUtility.SaveFolderPanel(
						oldXMLPath,EditorApplication.applicationPath, System.String.Empty);
					
					if ( ( xmlpath.Length == 0 ) ||
					    ( xmlpath.IndexOf( _projectPath ) != 0 ) )
					{
						_editorPathInfo = Localize( "ID_ERROR_XML_DIRECTORY" );
						Debug.LogWarning( _editorPathInfo );
						bDoSave = false;
					}
					
					if ( (bDoSave == true) && (oldXMLPath != xmlpath) )
					{
						SetString ( "xmlDirectory", xmlpath );
						_xmlDirectory = xmlpath;
					}
				}
				GUI.color = _defaultFGColor;
				GUI.backgroundColor = _defaultBGColor;
				DrawPathLabel ( _xmlDirectory );
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}
		
		void DrawChooseJSONPathGUI()
		{
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_JSON_ENABLE");
			bool oldUseJSON = _useJSON;
			_useJSON = DrawToggle( "ID_ENABLE_JSON", _useJSON );
			EditorGUILayout.EndHorizontal();
			
			if ( oldUseJSON != _useJSON )
			{
				SetBool( "useJSON", _useJSON );
			}
			
			if( _useJSON == true )
			{
				EditorGUILayout.Separator();
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				DrawHelpButtonGUI ("ID_HELP_JSON_PATH");
				DrawLabelHeader ( "ID_SELECT_JSON_DIRECTORY" );
				EditorGUILayout.EndHorizontal();
				
				bool bDoSave = true;
				string oldJSONPath = _jsonDirectory;
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				GUI.backgroundColor = Color.clear;
				GUI.color = Color.yellow;
				if ( GUILayout.Button( _browseButton, EditorStyles.toolbarButton, GUILayout.Width(24)) )
				{
					ClearMessages();
					string jsonpath = EditorUtility.SaveFolderPanel(
						oldJSONPath,EditorApplication.applicationPath, System.String.Empty);
					
					if ( ( jsonpath.Length == 0 ) ||
					    ( jsonpath.IndexOf( _projectPath ) != 0 ) )
					{
						_editorPathInfo = Localize( "ID_ERROR_JSON_DIRECTORY" );
						Debug.LogWarning( _editorPathInfo );
						bDoSave = false;
					}
					
					if ( (bDoSave == true) && (oldJSONPath != jsonpath) )
					{
						SetString ( "jsonDirectory", jsonpath );
						_jsonDirectory = jsonpath;
					}
				}
				GUI.color = _defaultFGColor;
				GUI.backgroundColor = _defaultBGColor;
				DrawPathLabel ( _jsonDirectory );
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}
		
		void DrawChoosePlaymakerPathGUI()
		{
			if( _foundPlaymaker == false )
				return;
			
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_PLAYMAKER_ENABLE");
			bool oldUsePlaymaker = _usePlaymaker;
			_usePlaymaker = DrawToggle( "ID_ENABLE_PLAYMAKER", _usePlaymaker );
			EditorGUILayout.EndHorizontal();
			
			if ( oldUsePlaymaker != _usePlaymaker )
			{
				SetBool( "usePlaymaker", _usePlaymaker );
			}
			
			if( _usePlaymaker == true )
			{
				EditorGUILayout.Separator();
				
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				DrawHelpButtonGUI ("ID_HELP_PLAYMAKER_PATH");
				DrawLabelHeader ( "ID_SELECT_PLAYMAKER_DIRECTORY" );
				EditorGUILayout.EndHorizontal();
				
				bool bDoSave = true;
				string oldPlaymakerPath = _playmakerDirectory;
				EditorGUILayout.BeginHorizontal();
				Indent(1);
				GUI.backgroundColor = Color.clear;
				GUI.color = Color.yellow;
				if ( GUILayout.Button( _browseButton, EditorStyles.toolbarButton, GUILayout.Width(24)) )
				{
					ClearMessages();
					string playmakerpath = EditorUtility.SaveFolderPanel(
						oldPlaymakerPath,EditorApplication.applicationPath, System.String.Empty);
					
					if ( ( playmakerpath.Length == 0 ) ||
					    ( playmakerpath.IndexOf( _projectPath ) != 0 ) )
					{
						_editorPathInfo = Localize( "ID_ERROR_PLAYMAKER_DIRECTORY" );
						Debug.LogWarning( _editorPathInfo );
						bDoSave = false;
					}
					
					if ( (bDoSave == true) && (oldPlaymakerPath != playmakerpath) )
					{
						SetString ( "playmakerDirectory", playmakerpath );
						_playmakerDirectory = playmakerpath;
					}
				}
				GUI.color = _defaultFGColor;
				GUI.backgroundColor = _defaultBGColor;
				DrawPathLabel ( _playmakerDirectory );
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}
		
		void Export (string WorkbookName, System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> activeEntries)
		{
			ANONCOLNAME = 0;
			
			System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> objDBEntries = new System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry>();
			System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> staticDBEntries = new System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry>();
			System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> nguiEntries = new System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry>();
			System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> xmlEntries = new System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry>();
			System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> jsonEntries = new System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry>();
			System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> daikonforgeEntries = new System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry>();
			
			string sanitizedWorkbookName = MakeValidFileName(WorkbookName);
			
			foreach(Google.GData.Spreadsheets.WorksheetEntry entry in activeEntries)
			{
				if ( GetBool( WorkbookName + "." + entry.Title.Text + ".EXPORTOBJDB", false ) )
					objDBEntries.Add(entry);
				else if ( GetBool( WorkbookName + "." + entry.Title.Text + ".EXPORTSTATICDB", false ) )
					staticDBEntries.Add(entry);
				else if ( GetBool ( WorkbookName + "." + entry.Title.Text + ".EXPORTNGUI", false ) )
					nguiEntries.Add(entry);
				else if ( GetBool( WorkbookName + "." + entry.Title.Text + ".EXPORTXML", false ) )
					xmlEntries.Add(entry);
				else if ( GetBool( WorkbookName + "." + entry.Title.Text + ".EXPORTJSON", false ) )
					jsonEntries.Add(entry);
				else if ( GetBool ( WorkbookName + "." + entry.Title.Text + ".EXPORTDAIKONFORGE", false ) )
					daikonforgeEntries.Add(entry);
			}
			
			if(objDBEntries.Count > 0)
			{
				string path = _objDBResourcesDirectory;
				
				foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in objDBEntries )
				{
					
					string filename = MakeValidFileName(entry.Title.Text);
					
					ExportDatabase( path, filename, entry, false );
					
					string dbattachName = GetString( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB", System.String.Empty );
					GameObject dbattach = GameObject.Find( dbattachName );
					
					AdvancedDatabaseInfo info = null;
					if ( dbattach == null )
						info = new AdvancedDatabaseInfo( filename, entry, _service, null, GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".PM", true ), GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".UFR", true ) );
					else
						info = new AdvancedDatabaseInfo( filename, entry, _service, dbattach, GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".PM", true ), GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".UFR", true ) );
					_advancedDatabaseObjectInfo.Add( info );
					
					
					AssetDatabase.ImportAsset(filename, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
					AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
				}
			}
			
			if(staticDBEntries.Count > 0)
			{
				string path = _staticDBResourcesDirectory;
				
				foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in staticDBEntries )
				{
					string filename = MakeValidFileName(entry.Title.Text);
					ExportDatabase( path, filename, entry, true );
				}
			}
			
			if(nguiEntries.Count > 0)
			{
				// output this book as localized
				ExportNGUI( _nguiDirectory, nguiEntries );
			}
			
			if(xmlEntries.Count > 0)
			{
				
				string path = System.IO.Path.Combine( _xmlDirectory, sanitizedWorkbookName + ".xml" );
				ExportXML( path, xmlEntries);
			}
			
			if(daikonforgeEntries.Count > 0 )
			{
				string path = System.IO.Path.Combine( _daikonforgeDirectory, sanitizedWorkbookName + ".csv" );
				ExportDaikonForge( path, daikonforgeEntries);
			}
			
			if(jsonEntries.Count > 0)
			{
				string path = System.IO.Path.Combine( _jsonDirectory, sanitizedWorkbookName + ".json" );
				if (path.Length != 0)
				{
					ShowNotification( new GUIContent( "Saving to: " + path ) );
					Debug.Log("Saving to: " + path );
					System.IO.FileStream fs = null;
					if( System.IO.File.Exists(path))
					{
						fs = System.IO.File.Open(path, System.IO.FileMode.Truncate, System.IO.FileAccess.Write);
					}
					else
						fs = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
					//FileStream fs = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate | System.IO.FileMode.Truncate, System.IO.FileAccess.ReadWrite);
					if (fs != null )
					{
						System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
						if( sw != null )
						{
							ExportJSON( path, sw, jsonEntries);
							sw.Close();	
						}
						fs.Close();
					}
				}
			}
			
		}
		
		private static string MakeValidFileName( string name )
		{
			char [] invalidChars = System.IO.Path.GetInvalidFileNameChars();
			string tmp = "";
			foreach( char c in name)
			{
				bool foundInvalid = false;
				foreach( char ic in invalidChars )
					if( c == ic )
						foundInvalid = true;
				if( !foundInvalid )
					tmp += c;
			}
			tmp = tmp.Replace(" ", System.String.Empty);
			return tmp;
		}
		
		void Indent (int tabs)
		{
			_curIndent = tabs;
			GUILayout.Label("", GUILayout.Width(24 * tabs));
		}
		
		void DrawSelectAndExportGUI ()
		{
			if ( _activeWorkbook == null )
				return;
			
			System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> activeEntries = new System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry>();
			
			EditorGUILayout.BeginHorizontal();
			DrawHelpButtonGUI ("ID_HELP_EXPORT_WORKSHEETS");
			DrawLabelHeader ( "ID_SELECT_WORKSHEETS" );
			EditorGUILayout.EndHorizontal();
			
			System.Collections.Generic.List< Google.GData.Spreadsheets.WorksheetEntry > AllEntries = new System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry>();
			if ( _activeWorkbook.WorksheetEntries != null )
				foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in _activeWorkbook.WorksheetEntries )
					AllEntries.Add( entry );
			if ( _activeWorkbook.ManualEntries.Count > 0 )
				foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in _activeWorkbook.ManualEntries )
					AllEntries.Add( entry );
			
			if ( AllEntries.Count > 0 )
			{
				foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in AllEntries )
				{
					
					bool useEntry = GetBool( _activeWorkbook.Title + "." + entry.Title.Text, false );
					EditorGUILayout.BeginHorizontal();
					Indent(1);
					DrawHelpButtonGUI ("ID_HELP_SELECT_WORKSHEET");
					if ( GUILayout.Toggle( useEntry, entry.Title.Text ) )
					{
						SetBool( _activeWorkbook.Title + "." + entry.Title.Text, true );
						useEntry = true;
					}
					else if ( useEntry == true )
					{
						SetBool( _activeWorkbook.Title + "." + entry.Title.Text, false );
						useEntry = false;
					}
					EditorGUILayout.EndHorizontal();
					
					
					if ( useEntry == true )
					{
						bool useExportObjDB = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTOBJDB", false );
						bool useExportStaticDB = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTSTATICDB", false );
						bool useExportNGUI = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTNGUI", false );
						bool useExportXML = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTXML", false );
						bool useExportJSON = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTJSON", false );
						bool useExportDaikonForge = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTDAIKONFORGE", false );
						
						if ( _useObjDB == true )
						{
							EditorGUILayout.BeginHorizontal();
							Indent(2);
							DrawHelpButtonGUI ("ID_HELP_EXPORT_AS_GAMEOBJECT");
							if ( DrawToggle( "ID_EXPORT_OBJ_DB", useExportObjDB ) )
							{
								useExportObjDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTOBJDB", true );
								useExportStaticDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTSTATICDB", false );
								useExportNGUI = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTNGUI", false );
								useExportXML = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTXML", false );
								useExportJSON = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTJSON", false );
								useExportDaikonForge = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTDAIKONFORGE", false );
							}
							else if ( useExportObjDB == true )
							{
								SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTOBJDB", false );
							}
							EditorGUILayout.EndHorizontal();
							
							if ( useExportObjDB == true )
							{
								EditorGUILayout.BeginHorizontal();
								Indent(3);
								bool oldUFR = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".UFR", false );
								DrawHelpButtonGUI ("ID_HELP_EXPORT_FIRST_ROW_AS_VALUES");
								if ( DrawToggle( "ID_EXPORT_UFR", oldUFR ) )
								{
									SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".UFR", true );
								}
								else if ( oldUFR == true )
								{
									SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".UFR", false );
								}
								EditorGUILayout.EndHorizontal();
								
								EditorGUILayout.BeginHorizontal();
								Indent(3);
								DrawHelpButtonGUI ("ID_HELP_EXPORT_DO_NOT_DESTROY");
								bool oldDND = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".DND", false );
								if ( DrawToggle( "ID_EXPORT_DND", oldDND ) )
								{
									SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".DND", true );
								}
								else if ( oldDND == true )
								{
									SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".DND", false );
								}
								EditorGUILayout.EndHorizontal();
								
								if ( _foundPlaymaker )
								{
									EditorGUILayout.BeginHorizontal();
									Indent(3);
									bool oldPM = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".PM", false );
									DrawHelpButtonGUI ("ID_HELP_EXPORT_PLAYMAKER_ACTIONS");
									if ( DrawToggle( "ID_EXPORT_PM", oldPM ) )
									{
										SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".PM", true );
									}
									else if ( oldPM == true )
									{
										SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".PM", false );
									}
									EditorGUILayout.EndHorizontal();
								}
								
								
								GameObject oldObjDB = null;
								string objDBString = GetString( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB", System.String.Empty );
								// Attempt the faster lookup first
								if ( objDBString != System.String.Empty && _advancedDatabaseObjects.ContainsKey( objDBString ))
									oldObjDB = _advancedDatabaseObjects[ objDBString ];
								
								// If that doesn't work, try the slow method
								if( objDBString != System.String.Empty && oldObjDB == null )
									oldObjDB = GameObject.Find( objDBString );
								
								
								EditorGUILayout.BeginHorizontal();
								Indent(3);
								DrawHelpButtonGUI ("ID_HELP_EXPORT_TO_SPECIFIED_GAMEOBJECT");
								GUILayout.Label( Localize ("ID_EXPORT_SELECT_OBJECT"));
								EditorGUILayout.EndHorizontal();
								
								EditorGUILayout.BeginHorizontal();
								Indent(3);
								GameObject newObjDB = (GameObject)EditorGUILayout.ObjectField( oldObjDB, typeof(GameObject), true, GUILayout.Width(140));
								
								if( oldObjDB != newObjDB )
								{
									if (newObjDB == null )
									{
										SetString( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB", System.String.Empty );
									}
									else
									{
										SetString( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB", newObjDB.name );
										if( _advancedDatabaseObjects.ContainsKey( newObjDB.name ) == false )
											_advancedDatabaseObjects.Add( newObjDB.name, newObjDB );
									}
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						if ( _useStaticDB == true )
						{
							EditorGUILayout.BeginHorizontal();
							Indent(2);
							DrawHelpButtonGUI ("ID_HELP_EXPORT_AS_STATIC");
							if ( DrawToggle( "ID_EXPORT_STATIC_DB", useExportStaticDB ) )
							{
								useExportObjDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTOBJDB", false );
								useExportStaticDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTSTATICDB", true );
								useExportNGUI = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTNGUI", false );
								useExportXML = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTXML", false );
								useExportJSON = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTJSON", false );
								useExportDaikonForge = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTDAIKONFORGE", false );
							}
							else if ( useExportStaticDB == true )
							{
								SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTSTATICDB", false );
							}
							
							EditorGUILayout.EndHorizontal();
							
							if ( useExportStaticDB )
							{
								EditorGUILayout.BeginHorizontal();
								Indent (3);
								bool oldUFR = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".STATICDB" + ".UFR", false );
								DrawHelpButtonGUI ("ID_HELP_EXPORT_FIRST_ROW_AS_VALUES");
								if ( DrawToggle( "ID_EXPORT_UFR", oldUFR ) )
								{
									SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".STATICDB" + ".UFR", true );
								}
								else if ( oldUFR == true )
								{
									SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".STATICDB" + ".UFR", false );
								}
								EditorGUILayout.EndHorizontal();	
							}
						}
						if ( _foundNGUI && _useNGUI == true )
						{
							EditorGUILayout.BeginHorizontal();
							Indent(2);
							DrawHelpButtonGUI ("ID_HELP_EXPORT_AS_NGUI");
							if ( DrawToggle( "ID_EXPORT_NGUI", useExportNGUI ) )
							{
								useExportObjDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTOBJDB", false );
								useExportStaticDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTSTATICDB", false );
								useExportNGUI = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTNGUI", true );
								useExportXML = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTXML", false );
								useExportJSON = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTJSON", false );
								useExportDaikonForge = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTDAIKONFORGE", false );
							}
							else if ( useExportNGUI == true )
							{
								SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTNGUI", false );
							}
							EditorGUILayout.EndHorizontal();
						}
						if ( _foundDaikonForge && _useDaikonForge == true )
						{
							EditorGUILayout.BeginHorizontal();
							Indent(2);
							DrawHelpButtonGUI ("ID_HELP_EXPORT_AS_DAIKONFORGE");
							if ( DrawToggle( "ID_EXPORT_DAIKONFORGE", useExportDaikonForge ) )
							{
								useExportObjDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTOBJDB", false );
								useExportStaticDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTSTATICDB", false );
								useExportNGUI = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTNGUI", false );
								useExportXML = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTXML", false );
								useExportJSON = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTJSON", false );
								useExportDaikonForge = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTDAIKONFORGE", true );
							}
							else if ( useExportNGUI == true )
							{
								SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTDAIKONFORGE", false );
							}
							EditorGUILayout.EndHorizontal();
						}
						if ( _useXML == true )
						{
							EditorGUILayout.BeginHorizontal();
							Indent(2);
							DrawHelpButtonGUI ("ID_HELP_EXPORT_AS_XML");
							if ( DrawToggle( "ID_EXPORT_XML", useExportXML ) )
							{
								useExportObjDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTOBJDB", false );
								useExportStaticDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTSTATICDB", false );
								useExportNGUI = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTNGUI", false );
								useExportXML = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTXML", true );
								useExportJSON = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTJSON", false );
								useExportDaikonForge = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTDAIKONFORGE", false );
							}
							else if ( useExportXML == true )
							{
								SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTXML", false );
							}
							EditorGUILayout.EndHorizontal();
						}
						if ( _useJSON == true )
						{
							EditorGUILayout.BeginHorizontal();
							Indent(2);
							DrawHelpButtonGUI ("ID_HELP_EXPORT_AS_JSON");
							if ( DrawToggle( "ID_EXPORT_JSON", useExportJSON ) )
							{
								useExportObjDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTOBJDB", false );
								useExportStaticDB = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTSTATICDB", false );
								useExportNGUI = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTNGUI", false );
								useExportXML = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTXML", false );
								useExportJSON = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTJSON", true );
								useExportDaikonForge = SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTDAIKONFORGE", false );
							}
							else if ( useExportJSON == true )
							{
								SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".EXPORTJSON", false );
							}
							
							EditorGUILayout.EndHorizontal();
							
							if ( useExportJSON )
							{
								EditorGUILayout.BeginHorizontal();
								Indent (3);
								bool oldEscapeUni = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".JSON" + ".ESCAPEUNICODE", true );
								DrawHelpButtonGUI ("ID_HELP_ESCAPE_UNICODE");
								if ( DrawToggle( "ID_ESCAPE_UNICODE", oldEscapeUni ) )
								{
									SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".JSON" + ".ESCAPEUNICODE", true );
								}
								else if ( oldEscapeUni == true )
								{
									SetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".JSON" + ".ESCAPEUNICODE", false );
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						
						if ( ( _useObjDB && useExportObjDB ) ||
						    ( _useStaticDB && useExportStaticDB ) ||
						    ( _useNGUI && useExportNGUI ) ||
						    ( _useJSON && useExportJSON ) ||
						    ( _useXML && useExportXML )  ||
						    ( _useDaikonForge && useExportDaikonForge ) )
							activeEntries.Add( entry );
					}
					
					EditorGUILayout.Space();
				}
			}
			
			if ( activeEntries.Count <= 0 )
				GUI.enabled = false;
			if ( GUILayout.Button( Localize( "ID_EXPORT" ) ) )
			{
				ClearMessages();
				Export(_activeWorkbook.Title, activeEntries );
			}
			GUI.enabled = true;
			
		}
		
		void SettingsGUI ()
		{
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.BeginVertical(GUILayout.Width(120));
			if ( _bShowAuthentication == true )
				GUI.backgroundColor = _selectedTabColor;
			else
				GUI.backgroundColor = _unselectedTabColor;
			if ( GUILayout.Button( Localize( "ID_CREDENTIALS"), EditorStyles.toolbarButton ) )
			{
				ClearMessages();
				_bShowAuthentication = true;
				_bShowLanguage = false;
				_bShowPaths = false;
			}
			GUI.backgroundColor = _defaultBGColor;
			
			if ( _bShowLanguage == true )
				GUI.backgroundColor = _selectedTabColor;
			else
				GUI.backgroundColor = _unselectedTabColor;
			if ( GUILayout.Button( Localize("ID_LANGUAGE"), EditorStyles.toolbarButton ) )
			{
				ClearMessages();
				_bShowAuthentication = false;
				_bShowLanguage = true;
				_bShowPaths = false;
			}
			GUI.backgroundColor = _defaultBGColor;
			
			if ( _bShowPaths == true )
				GUI.backgroundColor = _selectedTabColor;
			else
				GUI.backgroundColor = _unselectedTabColor;
			if ( GUILayout.Button( Localize("ID_PATHS"), EditorStyles.toolbarButton ) )
			{
				ClearMessages();
				_bShowAuthentication = false;
				_bShowLanguage = false;
				_bShowPaths = true;
			}
			GUI.backgroundColor = _defaultBGColor;
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.BeginVertical();
			if ( _bShowAuthentication == true )
			{
				DrawAuthenticationGUI();
			}
			else if ( _bShowLanguage == true )
			{
				DrawChooseLanguageGUI ();
			}
			else if ( _bShowPaths == true )
			{
				DrawCreatePathsGUI();
				DrawEnableDBObjGUI();
				DrawEnableStaticObjGUI();
				DrawChooseNGUIPathGUI();
				DrawChooseXMLPathGUI();
				DrawChooseJSONPathGUI();
				DrawChoosePlaymakerPathGUI();
				DrawChooseDaikonForgePathGUI();
			}
			EditorGUILayout.EndVertical();
			GUILayout.Label ( System.String.Empty );
			EditorGUILayout.EndHorizontal();
		}
		
		void WorkbooksGUI ()
		{
			if ( _authorized == false )
			{
				_bShowAccountWorkbooks = false;
				_bShowManualWorkbooks = true;
				_bShowCreateWorkbook = false;
			}
			
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.BeginVertical(GUILayout.Width(120));
			if ( _bShowAccountWorkbooks == true )
				GUI.backgroundColor = _selectedTabColor;
			else
				GUI.backgroundColor = _unselectedTabColor;
			if ( _authorized == false )
				GUI.enabled = false;
			if ( GUILayout.Button( Localize("ID_ACCOUNT_WORKBOOK"), EditorStyles.toolbarButton ) )
			{
				ClearMessages();
				_bShowAccountWorkbooks = true;
				_bShowManualWorkbooks = false;
				_bShowCreateWorkbook = false;
			}
			GUI.enabled = true;
			GUI.backgroundColor = _defaultBGColor;
			
			if ( _bShowManualWorkbooks == true )
				GUI.backgroundColor = _selectedTabColor;
			else
				GUI.backgroundColor = _unselectedTabColor;
			if ( GUILayout.Button( Localize("ID_MANUAL_WORKBOOK"), EditorStyles.toolbarButton ) )
			{
				ClearMessages();
				_bShowAccountWorkbooks = false;
				_bShowManualWorkbooks = true;
				_bShowCreateWorkbook = false;
			}
			GUI.backgroundColor = _defaultBGColor;
			
			if ( _bShowCreateWorkbook == true )
				GUI.backgroundColor = _selectedTabColor;
			else
				GUI.backgroundColor = _unselectedTabColor;
			if ( _authorized == false )
				GUI.enabled = false;
			if ( GUILayout.Button( Localize("ID_CREATE_WORKBOOK"), EditorStyles.toolbarButton ) )
			{
				ClearMessages();
				_bShowAccountWorkbooks = false;
				_bShowManualWorkbooks = false;
				_bShowCreateWorkbook = true;
			}
			GUI.enabled = true;
			GUI.backgroundColor = _defaultBGColor;
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.BeginVertical();
			
			if ( _bShowAccountWorkbooks == true )
			{
				DrawAccountWorkbooksGUI();
			}
			else if ( _bShowManualWorkbooks == true )
			{
				DrawManualWorkbooksGUI();
			}
			else if ( _bShowCreateWorkbook == true )
			{
				DrawCreateWorkbookGUI();
			}
			
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
		
		void ToolboxGUI ()
		{
			DrawSelectAndExportGUI ();
		}
		
		void DrawHelpMainGUI()
		{
			EditorGUILayout.Separator();
			
			EditorGUILayout.BeginHorizontal();
			GUI.backgroundColor = Color.black;
			if ( GUILayout.Button(_litteratusLogo) )
			{
				ClearMessages();
				Application.OpenURL( "http://www.litteratus.net" );
			}
			GUI.backgroundColor = _defaultBGColor;
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUI.backgroundColor = Color.black;
			if ( GUILayout.Button(_unityLogo) )
			{
				ClearMessages();
				Application.OpenURL( "http://www.unity3d.com" );
			}
			GUI.backgroundColor = _defaultBGColor;
			EditorGUILayout.EndHorizontal();
			GUILayout.Label( Localize( "ID_CREATED_WITH_UNITY" ) + " \u00a9 " + Localize( "ID_COPYRIGHT_UNITY" ));
		}
		
		void DrawHelpDocEntry (string entryID, string Title, string HelpText, ref bool bPreviousActive)
		{
			if (EditorGUILayout.BeginToggleGroup( Title, _currentHelpDoc == entryID && !bPreviousActive ) )
			{
				EditorGUILayout.HelpBox( HelpText, MessageType.None);
				_currentHelpDoc = entryID;
				bPreviousActive = true;
			}
			EditorGUILayout.EndToggleGroup();
			
		}
		
		void DrawHelpDocsGUI()
		{
			bool bPreviousActive = false;
			
			DrawHelpDocEntry ("HELP_LOGGING_IN", Localize( "ID_HELP_LOGGING_IN_TITLE" ), Localize( "ID_HELP_LOGGING_IN_TEXT" ), ref bPreviousActive);
			DrawHelpDocEntry ("HELP_SAVE_CREDENTIALS", Localize( "ID_HELP_SAVE_CREDENTIALS_TITLE" ), Localize( "ID_HELP_SAVE_CREDENTIALS_TEXT" ), ref bPreviousActive);
			DrawHelpDocEntry ("HELP_ACTIVE_WORKBOOK", Localize( "ID_HELP_ACTIVE_WORKBOOK_TITLE" ), Localize( "ID_HELP_ACTIVE_WORKBOOK_TEXT" ), ref bPreviousActive);
			DrawHelpDocEntry ("HELP_NGUI_EXPORT", Localize( "ID_HELP_NGUI_EXPORT_TITLE" ), Localize( "ID_HELP_NGUI_EXPORT_TEXT" ), ref bPreviousActive);
			DrawHelpDocEntry ("HELP_PLAYMAKER", Localize( "ID_HELP_PLAYMAKER_TITLE" ), Localize( "ID_HELP_PLAYMAKER_TEXT" ), ref bPreviousActive);
		}
		
		void HelpGUI()
		{
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.BeginVertical(GUILayout.Width(120));
			if ( _bShowMain == true )
				GUI.backgroundColor = _selectedTabColor;
			else
				GUI.backgroundColor = _unselectedTabColor;
			
			if ( GUILayout.Button( Localize( "ID_HELP_MAIN" ), EditorStyles.toolbarButton) )
			{
				ClearMessages();
				_bShowMain = true;
				_bShowDocs = false;
			}
			GUI.backgroundColor = _defaultBGColor;
			
			if ( _bShowDocs == true )
				GUI.backgroundColor = _selectedTabColor;
			else
				GUI.backgroundColor = _unselectedTabColor;
			if ( GUILayout.Button( Localize( "ID_DOCUMENTATION" ), EditorStyles.toolbarButton) )
			{
				ClearMessages();
				_bShowMain = false;
				_bShowDocs = true;
			}
			GUI.backgroundColor = _defaultBGColor;
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.BeginVertical();
			if ( _bShowMain == true )
			{
				DrawHelpMainGUI();
			}
			else if ( _bShowDocs == true )
			{
				DrawHelpDocsGUI();
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
		
		bool CheckEnabledPaths ()
		{
			
			// If we have NOTHING selected, that's a problem too
			if ( ( _useObjDB == false ) && ( _useStaticDB == false) &&
			    ( _foundNGUI && ( _useNGUI == false ) )  && ( _useXML == false )	&&
			    ( _useJSON == false )  && ( _foundPlaymaker && (_usePlaymaker == false) ) &&
			    ( _foundDaikonForge && ( _useDaikonForge == false ) ) )
				return false;
			
			if ( _editorPathInfo != System.String.Empty )
				return false;
			
			if	( ( _useObjDB == false) 	|| ( _useObjDB == true && ( _objDBResourcesDirectory != System.String.Empty && _objDBEditorDirectory != System.String.Empty ) ) )
				return true;
			if	( ( _useStaticDB == false)	|| ( _useStaticDB == true && ( _staticDBResourcesDirectory != System.String.Empty ) ) )
				return true;
			if	( _foundNGUI && ( ( _useNGUI == false )		|| ( _useNGUI == true && ( _nguiDirectory != System.String.Empty ) ) ) )
				return true;
			if	( ( _useXML == false )		|| ( _useXML == true && ( _xmlDirectory != System.String.Empty ) ) )
				return true;
			if	( ( _useJSON == false )		|| ( _useJSON == true && ( _jsonDirectory == System.String.Empty ) ) )
				return true;
			if	( _foundPlaymaker && ( ( _usePlaymaker == false )|| ( _usePlaymaker == true && ( _playmakerDirectory == System.String.Empty ) ) ) )
				return true;
			
			if	( _foundDaikonForge && ( ( _useDaikonForge == false )|| ( _useDaikonForge == true && ( _daikonforgeDirectory == System.String.Empty ) ) ) )
				return true;
			
			return false;
			
		}
		
		void GetPathErrors()
		{
			if ( _useObjDB == true && _objDBResourcesDirectory == System.String.Empty )
			{
				_editorPathInfo = Localize( "ID_ERROR_RESOURCES_DIRECTORY" );	
			}
			else if ( _useObjDB == true && _objDBEditorDirectory == System.String.Empty )
			{
				_editorPathInfo = Localize( "ID_ERROR_EDITOR_DIRECTORY" );	
			}
			else if ( _useStaticDB == true && _staticDBResourcesDirectory == System.String.Empty )
			{
				_editorPathInfo = Localize( "ID_ERROR_RESOURCES_DIRECTORY" );	
			}
			else if (_foundNGUI && ( _useNGUI == true &&  _nguiDirectory == System.String.Empty ) )
			{
				_editorPathInfo = Localize( "ID_ERROR_NGUI_DIRECTORY" );
			}
			else if (_useXML == true &&  _xmlDirectory == System.String.Empty )
			{
				_editorPathInfo = Localize( "ID_ERROR_XML_DIRECTORY" );
			}
			else if (_useJSON == true &&  _jsonDirectory == System.String.Empty )
			{
				_editorPathInfo = Localize( "ID_ERROR_JSON_DIRECTORY" );
			}
			else if ( _foundPlaymaker && ( _usePlaymaker == true &&  _playmakerDirectory == System.String.Empty ) )
			{
				_editorPathInfo = Localize( "ID_ERROR_PLAYMAKER_DIRECTORY" );
			}
			else if ( _foundDaikonForge && ( _useDaikonForge == true &&  _daikonforgeDirectory == System.String.Empty ) )
			{
				_editorPathInfo = Localize( "ID_ERROR_DAIKONFORGE_DIRECTORY" );
			}
			else
			{
				_editorPathInfo = System.String.Empty;
			}
		}
		
		void OnGUI()
		{
			GetPathErrors();
			if ( _editorPathInfo != System.String.Empty )
			{
				if ( _currentPage != GF_PAGE.Help )
				{
					_currentPage = GF_PAGE.Settings;
					_bShowAuthentication = false;
					_bShowLanguage = false;
					_bShowPaths = true;
				}
			}
			
			_isProSkin = EditorGUIUtility.isProSkin;
			_defaultBGColor = GUI.backgroundColor;
			
			_defaultFGColor = GUI.color;
			if ( _isProSkin )
			{
				_labelHeaderColor = Color.gray;
				_unselectedTabColor = Color.gray;
				_selectedTabColor = Color.green;
				_pathLabelBGColor = Color.gray;
				_pathLabelFGColor = Color.black;
				_largeLabelStyle = EditorStyles.whiteLargeLabel;
			}
			else
			{
				_labelHeaderColor = Color.black;
				_unselectedTabColor = Color.grey;
				_selectedTabColor = Color.green;
				_pathLabelBGColor = Color.gray;
				_pathLabelFGColor = Color.black;
				_largeLabelStyle = EditorStyles.largeLabel;
			}
			
			
			
			if (  _initDraw < 2 )
			{
				DrawLabelHeader( "ID_INITIALIZING" );
				_initDraw++;
				return;
			}
			
			if ( _service == null )
			{
				Init(); 
			}
			
			if ( _authorized == false && _activeWorkbook == null && _currentPage == GF_PAGE.Toolbox )
			{
				if ( _manualworkbooks.Count > 0 )
				{
					_editorWarning = Localize( "ID_ERROR_ACTIVATE_WORKBOOK") ;
					_currentPage = GF_PAGE.Workbooks;
					_bShowAccountWorkbooks = false;
					_bShowManualWorkbooks = true;
					_bShowCreateWorkbook = false;
				}
				else
				{
					_editorWarning = Localize( "ID_ERROR_ACTIVATE_WORKBOOK") ;
					_currentPage = GF_PAGE.Settings;
					_bShowAuthentication = true;
					_bShowLanguage = false;
					_bShowPaths = false;
				}
			}
			else
				if ( _useObjDB == false &&
				    _useStaticDB == false &&
				    _useJSON == false &&
				    _useXML == false &&
				    ( _foundNGUI == false || (_foundNGUI == true && _useNGUI == false ) ) && 
				    ( _foundDaikonForge == false || (_foundDaikonForge == true && _useDaikonForge == false ) ) && 
				    _currentPage == GF_PAGE.Toolbox )
			{
				// Well you have to enable SOMETHING..
				_editorWarning = Localize( "ID_NO_EXPORT_TYPE_ERROR") ;
				_currentPage = GF_PAGE.Settings;
				_bShowAuthentication = false;
				_bShowLanguage = false;
				_bShowPaths = true;
			}
			
			EditorGUILayout.BeginVertical();
			if ( _editorWarning != System.String.Empty )
			{
				EditorGUILayout.HelpBox( _editorWarning, MessageType.Warning );
			}
			if ( _editorInfo != System.String.Empty )
			{
				EditorGUILayout.HelpBox( _editorInfo, MessageType.Error );
			}
			if ( _editorWorking != System.String.Empty )
			{
				EditorGUILayout.HelpBox( _editorWorking, MessageType.Info );
			}
			if ( _editorPathInfo != System.String.Empty )
			{
				EditorGUILayout.HelpBox( _editorPathInfo, MessageType.Error );
			}
			if ( CreatingWorkbook == false && _editorException != System.String.Empty)
			{
				EditorGUILayout.HelpBox( _editorException, MessageType.Error );
			}
			EditorGUILayout.EndVertical();
			
			if ( _activeWorkbook != null )
			{
				DrawActiveWorkbookGUI ();
			}
			
			EditorGUILayout.Separator();
			
			bool checkpaths = CheckEnabledPaths();
			
			EditorGUILayout.BeginHorizontal();
			
			GUI.backgroundColor = _defaultBGColor;
			GUI.enabled = true;
			if ( _currentPage != GF_PAGE.Settings )
			{
				GUI.backgroundColor = _unselectedTabColor;
			}
			else
			{
				GUI.backgroundColor = _selectedTabColor;
			}
			if ( GUILayout.Button( Localize( "ID_SETTINGS" ), EditorStyles.toolbarButton ) )
			{
				ClearMessages();
				_currentPage = GF_PAGE.Settings;
			}
			
			GUI.enabled = true;
			GUI.backgroundColor = _defaultBGColor;
			if ( checkpaths == false )
			{
				GUI.enabled = false;
			}
			else if ( _currentPage != GF_PAGE.Workbooks )
			{
				GUI.backgroundColor = _unselectedTabColor;
			}
			else
			{
				GUI.backgroundColor = _selectedTabColor;
			}
			
			if ( GUILayout.Button( Localize( "ID_WORKBOOKS" ), EditorStyles.toolbarButton ) )
			{
				ClearMessages();
				_currentPage = GF_PAGE.Workbooks;
			}
			
			GUI.enabled = true;
			GUI.backgroundColor = _defaultBGColor;
			if ( checkpaths == false )
			{
				GUI.enabled = false;
			}
			if ( _currentPage != GF_PAGE.Toolbox )
			{
				GUI.backgroundColor = _unselectedTabColor;
			}
			else
			{
				GUI.backgroundColor = _selectedTabColor;
			}
			if ( GUILayout.Button( Localize( "ID_TOOLS" ), EditorStyles.toolbarButton) )
			{
				ClearMessages();
				_currentPage = GF_PAGE.Toolbox;
			}
			
			GUI.backgroundColor = _defaultBGColor;
			GUI.enabled = true;
			if ( _currentPage != GF_PAGE.Help )
			{
				GUI.backgroundColor = _unselectedTabColor;
			}
			else
			{
				GUI.backgroundColor = _selectedTabColor;
			}
			if ( GUILayout.Button( Localize( "ID_HELP" ), EditorStyles.toolbarButton) )
			{
				ClearMessages();
				_currentPage = GF_PAGE.Help;
			}
			
			GUI.enabled = true;
			GUI.backgroundColor = _defaultBGColor;
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			
			scrollPos = EditorGUILayout.BeginScrollView( scrollPos );
			
			switch ( _currentPage )
			{
			case GF_PAGE.Settings:
				SettingsGUI ();
				break;
			case GF_PAGE.Workbooks:
				WorkbooksGUI ();
				break;
			case GF_PAGE.Toolbox:
				ToolboxGUI ();
				break;
			case GF_PAGE.Help:
				HelpGUI ();
				break;
			}
			
			EditorGUILayout.LabelField( "\n\n\n\n\n\n", EditorStyles.wordWrappedLabel);
			
			EditorGUILayout.EndScrollView();
			
			
		}
		
		void ExportNGUI( string path, System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> entries)
		{
			if ( _foundNGUI == false )
				return;
			
			if( _useNGUILegacy == true )
			{
				ExportNGUILegacy( path, entries );
				return;
			}
			
			ExportCSV (path + "/Localization.csv", entries);
		}
		
		void ExportNGUILegacy( string path, System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> entries)
		{
			if ( _foundNGUI == false )
				return;
			
			ShowNotification( new GUIContent( "Saving to: " + path ) );
			Debug.Log("Saving to: " + path );
			
			System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string,string>> languages = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();
			
			// for each page
			foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in entries )
			{
				// Define the URL to request the list feed of the worksheet.
				Google.GData.Client.AtomLink listFeedLink = entry.Links.FindService(Google.GData.Spreadsheets.GDataSpreadsheetsNameTable.ListRel, null);
				
				// Fetch the list feed of the worksheet.
				Google.GData.Spreadsheets.ListQuery listQuery = new Google.GData.Spreadsheets.ListQuery(listFeedLink.HRef.ToString());
				Google.GData.Spreadsheets.ListFeed listFeed = _service.Query(listQuery);
				
				if ( listFeed.Entries.Count > 0 )
				{
					//int rowCt = listFeed.Entries.Count;
					//int colCt = ((Google.GData.Spreadsheets.ListEntry)listFeed.Entries[0]).Elements.Count;
					int curRow = 0;
					int curCol = 0;
					// Iterate through each row, printing its cell values.
					foreach (Google.GData.Spreadsheets.ListEntry row in listFeed.Entries)
					{
						
						// skip the first row. This is the title row, and we can get the values later
						//if ( curRow > 0 )
						{
							// Iterate over the remaining columns, and print each cell value
							foreach (Google.GData.Spreadsheets.ListEntry.Custom element in row.Elements)
							{
								if(curCol > 0)
								{
									if(!languages.ContainsKey(element.LocalName))
										languages.Add(element.LocalName, new System.Collections.Generic.Dictionary<string,string>());
									languages[element.LocalName].Add(row.Title.Text, element.Value);
								}
								curCol++;
							}
						}
						curCol = 0;
						curRow++;
					}
				}
			}
			foreach(System.Collections.Generic.KeyValuePair<string,System.Collections.Generic.Dictionary<string,string>> lang in languages) 
			{
				//string filepath = EditorUtility.SaveFilePanel(
				//        "test",EditorApplication.applicationPath,"test","txt");
				string filepath = path + "/" + lang.Key + ".txt";
				System.IO.FileStream fs = null;
				if( System.IO.File.Exists(filepath))
				{
					fs = System.IO.File.Open(filepath, System.IO.FileMode.Truncate, System.IO.FileAccess.Write);
				}
				else
					fs = System.IO.File.Open(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
				
				if (fs != null )
				{
					System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
					string fileString = System.String.Empty;
					
					fileString += formatLine("Flag = Flag-" + lang.Key); 
					foreach(System.Collections.Generic.KeyValuePair<string,string> word in lang.Value)
					{
						fileString += formatLine( word.Key + " = " + word.Value);
					}
					sw.Write(fileString);
					sw.Close();
				}
				fs.Close();
			}
		}
		
		//////////////////////////////////////
		// format from: http://www.daikonforge.com/docs/df-gui/classdf_language_manager.html
		//Data File Format
		//
		//All data files must conform to the following format in order to be used by the dfLanguageManager class.
		//Localization data is stored as comma-seperated values (CSV) in a text file which must follow these rules:
		//
		//Each record is located on a separate line, delimited by a newline (LF) character.
		//	Note that CRLF-style line breaks will be converted internally during processing to single-LF characters.
		//The last record in the file may or may not have an ending newline.
		//The first line of the file must contain a header record in the same format as normal record lines, containing names
		//	corresponding to the fields in the file and should contain the same number of fields as the records in the rest of the file.
		//	The name of the first field is not used, but is KEY by default. The following fields must be an uppercase two-letter ISO 639-1
		//  country code that indicates the language for that column.
		//Within the header and each record, there may be one or more fields, separated by commas.
		//  Each line should contain the same number of fields throughout the file.
		//Fields containing newline characters, double-quote characters, or comma characters must be enclosed in double-quotes.
		//If double-quotes are used to enclose fields, then a double-quote appearing inside a field must
		//  be escaped by preceding it with another double quote.
		//Example:
		//
		//  KEY,EN,ES,FR,DE
		//  GREET,"Greetings, citizen!","Saludos, ciudadano!","Salutations, citoyens!","Grüße, Bürger!"
		//  ENTER,Enter the door,Entra por la puerta,Entrez dans la porte,Geben Sie die Tür
		//  QUOTE,"""Quickly now!"", he said","""¡Rápido!"", Dijo","""Vite!"", At-il dit","""Schnell jetzt!"", Sagte er"
		//
		//
		//	The goal here is to take all sheets selected as Daikon Forge output and compine them into one CSV
		//  So if the user splits the localization into multiple sheets, there will still only be 1 CSV file at the end
		////////////////////////////////////
		
		void ExportDaikonForge(string path, System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> entries)
		{
			if ( _foundDaikonForge == false )
				return;
			
			ExportCSV (path, entries);
		}
		
		void ExportCSV(string path, System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> entries)
		{
			ShowNotification( new GUIContent( "Saving to: " + path ) );
			Debug.Log("Saving to: " + path );
			
			// System.Collections.Generic.Dictionary< String Key, System.Collections.Generic.Dictionary< Language, String Value > >
			System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string,string>> AllRows = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();
			System.Collections.Generic.List<string> Languages = new System.Collections.Generic.List<string>();
			// for each page
			foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in entries )
			{
				// Define the URL to request the list feed of the worksheet.
				Google.GData.Client.AtomLink listFeedLink = entry.Links.FindService(Google.GData.Spreadsheets.GDataSpreadsheetsNameTable.ListRel, null);
				
				// Fetch the list feed of the worksheet.
				Google.GData.Spreadsheets.ListQuery listQuery = new Google.GData.Spreadsheets.ListQuery(listFeedLink.HRef.ToString());
				Google.GData.Spreadsheets.ListFeed listFeed = _service.Query(listQuery);
				
				if ( listFeed.Entries.Count > 0 )
				{
					int curRow = 0;
					int curCol = 0;
					// Iterate through each row, printing its cell values.
					foreach (Google.GData.Spreadsheets.ListEntry row in listFeed.Entries)
					{
						// Iterate over the columns, and print each cell value
						foreach (Google.GData.Spreadsheets.ListEntry.Custom element in row.Elements)
						{
							if(curCol > 0)
							{
								if(!AllRows.ContainsKey(row.Title.Text))
									AllRows.Add(row.Title.Text, new System.Collections.Generic.Dictionary<string,string>());
								AllRows[row.Title.Text].Add(element.LocalName, element.Value);
								
								// Maintain a single list of available languages, we will use this to
								// iterate the columns later
								if(!Languages.Contains( element.LocalName ))
									Languages.Add( element.LocalName );
							}
							curCol++;
						}
						
						curCol = 0;
						curRow++;
					}
				}
			}
			
			System.IO.FileStream fs = null;
			if( System.IO.File.Exists(path))
			{
				fs = System.IO.File.Open(path, System.IO.FileMode.Truncate, System.IO.FileAccess.Write);
			}
			else
				fs = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
			
			if (fs != null )
			{
				System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
				string fileString = "KEY,";
				
				foreach( string lang in Languages )
				{
					fileString += lang.ToUpper();
					if ( lang != Languages[ Languages.Count - 1] )
						fileString += ",";
				}
				fileString += System.Environment.NewLine;
				
				foreach(System.Collections.Generic.KeyValuePair<string,System.Collections.Generic.Dictionary<string,string>> CurRow in AllRows) 
				{
					
					fileString += CurRow.Key + ",";
					System.Collections.Generic.Dictionary<string,string> LocStrings = CurRow.Value;
					
					foreach( string lang in Languages )
					{
						if( LocStrings.ContainsKey( lang ) )
							fileString += SanitizeDF( LocStrings[ lang ] );
						if ( lang != Languages[ Languages.Count - 1] )
							fileString += ",";
					}
					fileString += System.Environment.NewLine;
				}
				
				sw.Write(fileString);
				sw.Close();
			}
			
			fs.Close();
		}
		
		string SanitizeDF( string inString )
		{
			inString = inString.Replace("\"", "\"\"");
			return "\"" + inString + "\"";
		}
		
		void ExportXML(string path, System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> entries)
		{
			
			ShowNotification( new GUIContent( "Saving to: " + path ) );
			Debug.Log("Saving to: " + path );
			
			// Create the System.Xml.XmlDocument.
			System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
			System.Xml.XmlNode rootNode = xmlDoc.CreateElement("Sheets");
			xmlDoc.AppendChild(rootNode);
			
			
			
			foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in entries )
			{
				// Define the URL to request the list feed of the worksheet.
				Google.GData.Client.AtomLink listFeedLink = entry.Links.FindService(Google.GData.Spreadsheets.GDataSpreadsheetsNameTable.ListRel, null);
				
				// Fetch the list feed of the worksheet.
				Google.GData.Spreadsheets.ListQuery listQuery = new Google.GData.Spreadsheets.ListQuery(listFeedLink.HRef.ToString());
				Google.GData.Spreadsheets.ListFeed listFeed = _service.Query(listQuery);
				
				//int rowCt = listFeed.Entries.Count;
				//int colCt = ((Google.GData.Spreadsheets.ListEntry)listFeed.Entries[0]).Elements.Count;
				
				System.Xml.XmlNode sheetNode = xmlDoc.CreateElement("sheet");
				System.Xml.XmlAttribute sheetName = xmlDoc.CreateAttribute("name");
				sheetName.Value = entry.Title.Text;
				sheetNode.Attributes.Append(sheetName);
				rootNode.AppendChild(sheetNode);
				
				if ( listFeed.Entries.Count > 0 )
				{
					int curRow = 0;
					// Iterate through each row, printing its cell values.
					foreach (Google.GData.Spreadsheets.ListEntry row in listFeed.Entries)
					{
						System.Xml.XmlNode rowNode = xmlDoc.CreateElement("row");
						System.Xml.XmlAttribute rowName = xmlDoc.CreateAttribute("name");
						rowName.Value = row.Title.Text;
						rowNode.Attributes.Append(rowName);
						sheetNode.AppendChild(rowNode);
						
						// skip the first row. This is the title row, and we can get the values later
						//if ( rowCt > 0 )
						{
							int curCol = 0;
							// Iterate over the remaining columns, and print each cell value
							foreach (Google.GData.Spreadsheets.ListEntry.Custom element in row.Elements)
							{
								System.Xml.XmlNode colNode = xmlDoc.CreateElement("col");
								System.Xml.XmlAttribute colName = xmlDoc.CreateAttribute("name");
								colName.Value = element.LocalName;
								colNode.Attributes.Append(colName);
								colNode.InnerText = element.Value;
								rowNode.AppendChild(colNode);
								curCol++;
							}
						}
						curRow++;
					}
				}
			}
			
			// Save the document to a file and auto-indent the output.
			System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(path,null);
			writer.Formatting = System.Xml.Formatting.Indented;
			xmlDoc.Save(writer);
			writer.Close();
			ShowNotification( new GUIContent( "Saving to: " + path ) );
			Debug.Log("Saving to: " + path );
		}
		
		//////////////////////////////////////
		// format
		//{
		//    "sheetName": 
		//    [
		//        {
		//            "rowName": [
		//                {
		//                    "colName": "value",
		//                    "lastName": "value2"
		//                }
		//            ]
		//        },
		//        {
		//            "rowName2": [
		//                {
		//                    "colName": "value",
		//                    "colName2": "value2"
		//                }
		//            ]
		//        }
		//    ]
		//}
		////////////////////////////////////
		void ExportJSON(string path, System.IO.StreamWriter sw, System.Collections.Generic.List<Google.GData.Spreadsheets.WorksheetEntry> entries)
		{
			
			ShowNotification( new GUIContent( "Saving to: " + path ) );
			Debug.Log("Saving to: " + path );
			
			string fileString = System.String.Empty;
			fileString += ("{");
			
			int sheetCount = entries.Count;
			int curSheet = 0;
			// for each page
			foreach ( Google.GData.Spreadsheets.WorksheetEntry entry in entries )
			{
				// Define the URL to request the list feed of the worksheet.
				Google.GData.Client.AtomLink listFeedLink = entry.Links.FindService(Google.GData.Spreadsheets.GDataSpreadsheetsNameTable.ListRel, null);
				
				// Fetch the list feed of the worksheet.
				Google.GData.Spreadsheets.ListQuery listQuery = new Google.GData.Spreadsheets.ListQuery(listFeedLink.HRef.ToString());
				Google.GData.Spreadsheets.ListFeed listFeed = _service.Query(listQuery);
				
				bool escapeUnicode = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".JSON" + ".ESCAPEUNICODE", true );
				fileString += ("\"" + SanitizeJson(listFeed.Title.Text, escapeUnicode) + "\":[");
				
				int rowCt = listFeed.Entries.Count;
				if ( rowCt > 0 )
				{
					
					int colCt = ((Google.GData.Spreadsheets.ListEntry)listFeed.Entries[0]).Elements.Count;
					int curRow = 0;
					int curCol = 0;
					
					
					// Iterate through each row, printing its cell values.
					foreach (Google.GData.Spreadsheets.ListEntry row in listFeed.Entries)
					{
						fileString += ("{");
						fileString += ("\"" + SanitizeJson(row.Title.Text, escapeUnicode) + "\":[");
						
						// skip the first row. This is the title row, and we can get the values later
						//if ( curRow > 0 )
						{
							fileString += ("{");
							// Iterate over the remaining columns, and print each cell value
							foreach (Google.GData.Spreadsheets.ListEntry.Custom element in row.Elements)
							{
								if(curCol > 0)
								{
									if(curCol == colCt-1)
										fileString += ("\"" + SanitizeJson(element.LocalName, escapeUnicode) +"\":\"" + SanitizeJson(element.Value, escapeUnicode) + "\"");
									else
										fileString += ("\"" + SanitizeJson(element.LocalName, escapeUnicode) +"\":\"" + SanitizeJson(element.Value, escapeUnicode) + "\",");
								}
								curCol++;
							}
							fileString += ("}");
						}
						fileString += ("]");
						curCol = 0;
						if(curRow == rowCt-1)
							fileString += ("}");
						else
							fileString += ("},");
						curRow++;
					}
					
				}
				if(curSheet == sheetCount-1)
					fileString += ("]");
				else
					fileString += ("],");
				curSheet++;
				
			}
			fileString += ("}");
			sw.Write(fileString);
		}
		static string SanitizeJson( string value, bool escapeUnicode )
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach( char c in value )
			{
				if( (c > 127) && (escapeUnicode == true) ) 
				{
					// change this character into a unicode escape
					string encodedValue = "\\u" + ((int) c).ToString( "x4" );
					sb.Append( encodedValue );
				}
				else
				{
					switch(c)
					{
					case '\n':
						sb.Append("\\n");
						break;
					case '\r':
						sb.Append("\\r");
						break;
					case '\t':
						sb.Append("\\t");
						break;
					case '\b':
						sb.Append("\\b");
						break;
					case '\a':
						sb.Append("\\a");
						break;
					case '\f':
						sb.Append("\\f");
						break;
					case '\\':
						sb.Append("\\\\");
						break;
					case '\"':
						sb.Append("\\\"");
						break;
					default:
						sb.Append( c );
						break;
					}
				}
			}
			return sb.ToString();
		}
		
		void ExportDatabase( string path, string fileName, Google.GData.Spreadsheets.WorksheetEntry entry, bool staticClass )
		{
			
			////////////////////////////////////////////
			// gathering the data
			System.Collections.Generic.List<string> types = new System.Collections.Generic.List<string>();
			System.Collections.Generic.List<string> colNames = new System.Collections.Generic.List<string>();
			System.Collections.Generic.List<string> varNames = new System.Collections.Generic.List<string>();
			System.Collections.Generic.List<string> rowNames = new System.Collections.Generic.List<string>();
			
			bool typesInFirstRow = false;
			if( staticClass )
				typesInFirstRow = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".STATICDB" + ".UFR", false );
			else
				typesInFirstRow = GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".UFR", false );
			
			// Define the URL to request the list feed of the worksheet.
			Google.GData.Client.AtomLink listFeedLink = entry.Links.FindService(Google.GData.Spreadsheets.GDataSpreadsheetsNameTable.ListRel, null);
			
			// Fetch the list feed of the worksheet.
			Google.GData.Spreadsheets.ListQuery listQuery = new Google.GData.Spreadsheets.ListQuery(listFeedLink.HRef.ToString());
			Google.GData.Spreadsheets.ListFeed listFeed = _service.Query(listQuery);
			
			if ( listFeed.Entries.Count > 0 )
			{
				int rowCt = 0;
				// Iterate through each row, printing its cell values.
				foreach (Google.GData.Spreadsheets.ListEntry row in listFeed.Entries)
				{
					// skip the first row. This is the title row, and we can get the values later
					if ( rowCt == 0 )
					{
						int colCt = 0;
						// Iterate over the remaining columns, and print each cell value
						foreach (Google.GData.Spreadsheets.ListEntry.Custom element in row.Elements)
						{
							if(colCt > 0)
							{
								string vartype = element.Value;
								if(!typesInFirstRow)
									vartype = "string";
								else if(string.Compare(vartype, "float", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = vartype.ToLower();
								else if(string.Compare(vartype, "int", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = vartype.ToLower();
								else if(string.Compare(vartype, "unsigned int", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = vartype.ToLower();
								else if(string.Compare(vartype, "boolean", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = "bool";
								else if(string.Compare(vartype, "bool", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = vartype.ToLower();
								else if(string.Compare(vartype, "char", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = vartype.ToLower();
								else if(string.Compare(vartype, "byte", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = "char";
								else if(string.Compare(vartype, "double", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = vartype.ToLower();
								else if(string.Compare(vartype, "string", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = vartype.ToLower();
								else if(string.Compare(vartype, "short", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = vartype.ToLower();
								else if(string.Compare(vartype, "unsigned short", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = vartype.ToLower();
								else if(string.Compare(vartype, "vector2", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = "Vector2";
								else if(string.Compare(vartype, "vector3", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = "Vector3";
								else if(string.Compare(vartype, "vector", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = "Vector3";
								else if(string.Compare(vartype, "color", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = "Color";
								else if(string.Compare(vartype, "color32", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = "Color32";
								else if (string.Compare(vartype, "quaternion", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = "Quaternion";
								else if (string.Compare(vartype, "quat", System.StringComparison.OrdinalIgnoreCase) == 0)
									vartype = "Quaternion";
								types.Add(vartype);
								colNames.Add( element.LocalName.ToUpper() );
								varNames.Add( MakeValidVariableName( element.LocalName.ToUpper() ) );
							}
							colCt++;
						}
						
						if ( typesInFirstRow == false )
						{
							string name = row.Elements[0].Value;
							rowNames.Add(name);
						}
					}
					else
					{
						// store the row names to write out into the enum
						string name = row.Elements[0].Value;
						rowNames.Add(name);
						
					}
					rowCt++;
				}
			}
			
			if ( typesInFirstRow )
			{
				if(!isDataValid(types, colNames, rowNames))
				{
					Debug.LogError("Cannot output data for " + fileName + " until all errors with the data are fixed");
					
					// dont nuke their data if the new data is bad
					return;
				}
			}
			else
			{
				if(!isDataValid(colNames, rowNames))
				{
					Debug.LogError("Cannot output data for " + fileName + " until all errors with the data are fixed");
					// dont nuke their data if the new data is bad
					return;
				}	
			}
			
			///////////////////////////////////////////////
			// open the file 
			System.IO.StreamWriter sw = null;
			
			System.IO.FileStream fs = null;
			if( System.IO.File.Exists(path + "/" + fileName + ".cs"))
			{
				fs = System.IO.File.Open(path + "/" + fileName + ".cs", System.IO.FileMode.Truncate, System.IO.FileAccess.Write);
			}
			else
				fs = System.IO.File.Open(path + "/" + fileName + ".cs", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
			
			if (fs == null )
			{
				return;
			}
			
			sw = new System.IO.StreamWriter(fs);
			if( sw == null )
			{
				return;
			}
			
			////////////////////////////////////////
			// writing out the class
			string fileString = System.String.Empty;
			
			fileString += formatLine("//----------------------------------------------");
			fileString += formatLine("//    GoogleFu: Google Doc Unity integration");
			fileString += formatLine("//         Copyright © 2013 Litteratus");
			fileString += formatLine("//");
			fileString += formatLine("//        This file has been auto-generated");
			fileString += formatLine("//              Do not manually edit");
			fileString += formatLine("//----------------------------------------------");
			fileString += formatLine( System.String.Empty );
			fileString += formatLine("using UnityEngine;");
			fileString += formatLine( System.String.Empty );
			fileString += formatLine("namespace GoogleFu");
			fileString += formatLine("{");
			fileString += formatLine("	[System.Serializable]");
			fileString += formatLine("	public class " + fileName + "Row ");
			fileString += formatLine("	{");
			
			// variable declarations
			for(int i = 0; i < types.Count; i++)
			{
				fileString += formatLine("		public " + types[i] + " " + varNames[i] + ";");
			}
			// constructor parameter list
			fileString += ("		public " + fileName + "Row(");
			for(int i = 0; i < types.Count; i++)
			{	
				fileString += ("string _" + varNames[i]);
				if(i!=types.Count-1)
					fileString += (", ");
			}
			fileString += formatLine(") "+ System.Environment.NewLine+"		{");
			// processing each of the input parameters and copying it into the members
			for(int i = 0; i < types.Count; i++)
			{
				//nightmare time
				switch(types[i].ToUpper())
				{
				case "GAMEOBJECT":
					fileString += formatLine("			" + varNames[i] + " = GameObject.Find(\"" + colNames[i] + "\");");
					break;
				case "BOOLEAN":
				case "BOOL":
					fileString += formatLine("			{");
					fileString += formatLine("			"+types[i]+" res;");
					fileString += formatLine("				if("+types[i]+".TryParse(_"+varNames[i]+", out res))");
					fileString += formatLine("					"+varNames[i]+" = res;");
					fileString += formatLine("				else");
					fileString += formatLine("					Debug.LogError(\"Failed To Convert " + colNames[i] + " string: \"+ _" + varNames[i]+ " +\" to bool\");");
					fileString += formatLine("			}");
					break;
				case "CHAR":
					fileString += formatLine("			{");
					fileString += formatLine("			"+types[i]+" res;");
					fileString += formatLine("				if("+types[i]+".TryParse(_"+varNames[i]+", out res))");
					fileString += formatLine("					"+varNames[i]+" = res;");
					fileString += formatLine("				else");
					fileString += formatLine("					Debug.LogError(\"Failed To Convert " + colNames[i] + " string: \"+ _" + varNames[i]+ " +\" to char\");");
					fileString += formatLine("			}");
					break;
				case "DOUBLE":
					fileString += formatLine("			{");
					fileString += formatLine("			"+types[i]+" res;");
					fileString += formatLine("				if("+types[i]+".TryParse(_"+varNames[i]+", out res))");
					fileString += formatLine("					"+varNames[i]+" = res;");
					fileString += formatLine("				else");
					fileString += formatLine("					Debug.LogError(\"Failed To Convert " + colNames[i] + " string: \"+ _" + varNames[i]+ " +\" to double\");");
					fileString += formatLine("			}");
					break;
				case "FLOAT":
					fileString += formatLine("			{");
					fileString += formatLine("			"+types[i]+" res;");
					fileString += formatLine("				if("+types[i]+".TryParse(_"+varNames[i]+", out res))");
					fileString += formatLine("					"+varNames[i]+" = res;");
					fileString += formatLine("				else");
					fileString += formatLine("					Debug.LogError(\"Failed To Convert " + colNames[i] + " string: \"+ _" + varNames[i]+ " +\" to float\");");
					fileString += formatLine("			}");
					break;
				case "UNSIGNED INT":
				case "INT":
					fileString += formatLine("			{");
					fileString += formatLine("			"+types[i]+" res;");
					fileString += formatLine("				if(int.TryParse(_"+varNames[i]+", out res))");
					fileString += formatLine("					"+varNames[i]+" = res;");
					fileString += formatLine("				else");
					fileString += formatLine("					Debug.LogError(\"Failed To Convert " + colNames[i] + " string: \"+ _" + varNames[i]+ " +\" to int\");");
					fileString += formatLine("			}");
					break;
				case "UNSIGNED SHORT":
				case "SHORT":
					fileString += formatLine("			{");
					fileString += formatLine("			"+types[i]+" res;");
					fileString += formatLine("				if(short.TryParse(_"+varNames[i]+", out res))");
					fileString += formatLine("					"+varNames[i]+" = res;");
					fileString += formatLine("				else");
					fileString += formatLine("					Debug.LogError(\"Failed To Convert " + colNames[i] + " string: \"+ _" + varNames[i] + " +\" to short\");");
					fileString += formatLine("			}");
					break;
				case "STRING":	
					fileString += formatLine("			" + varNames[i] + " = _" + varNames[i] + ";");
					break;
				case "VECTOR2":
					fileString += formatLine("			{");
					fileString += formatLine("				string [] splitpath = _"+varNames[i]+".Split(new char[]{',', ' '},System.StringSplitOptions.RemoveEmptyEntries);");
					fileString += formatLine("				if(splitpath.Length != 2)");
					fileString += formatLine("					Debug.LogError(\"Incorrect number of paramaters for " + types[i] + " in \" + _" + varNames[i] + " );");
					fileString += formatLine("				float []results = new float[splitpath.Length];");
					fileString += formatLine("				for(int i = 0; i < 2; i++)");
					fileString += formatLine("				{");
					fileString += formatLine("					float res;");
					fileString += formatLine("					if(float.TryParse(splitpath[i], out res))");
					fileString += formatLine("					{");
					fileString += formatLine("						results[i] = res;");
					fileString += formatLine("					}");
					fileString += formatLine("					else ");
					fileString += formatLine("					{");
					fileString += formatLine("						Debug.LogError(\"Error parsing \" + " 
					                         + "_"+varNames[i] 
					                         + " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable " 
					                         + colNames[i] +"\");");
					fileString += formatLine("					}");
					fileString += formatLine("				}");
					fileString += formatLine("				"+varNames[i]+".x = results[0];");
					fileString += formatLine("				"+varNames[i]+".y = results[1];");
					fileString += formatLine("			}");
					break;
				case "VECTOR3":
					fileString += formatLine("			{");
					fileString += formatLine("				string [] splitpath = _"+varNames[i]+".Split(new char[]{',', ' '},System.StringSplitOptions.RemoveEmptyEntries);");
					fileString += formatLine("				if(splitpath.Length != 3)");
					fileString += formatLine("					Debug.LogError(\"Incorrect number of paramaters for " + types[i] + " in \" + _" + varNames[i] + " );");
					fileString += formatLine("				float []results = new float[splitpath.Length];");
					fileString += formatLine("				for(int i = 0; i < 3; i++)");
					fileString += formatLine("				{");
					fileString += formatLine("					float res;");
					fileString += formatLine("					if(float.TryParse(splitpath[i], out res))");
					fileString += formatLine("					{");
					fileString += formatLine("						results[i] = res;");
					fileString += formatLine("					}");
					fileString += formatLine("					else ");
					fileString += formatLine("					{");
					fileString += formatLine("						Debug.LogError(\"Error parsing \" + " 
					                         + "_"+varNames[i] 
					                         + " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable " 
					                         + colNames[i] +"\");");
					fileString += formatLine("					}");
					fileString += formatLine("				}");
					fileString += formatLine("				"+varNames[i]+".x = results[0];");
					fileString += formatLine("				"+varNames[i]+".y = results[1];");
					fileString += formatLine("				"+varNames[i]+".z = results[2];");
					fileString += formatLine("			}");
					break;
				case "COLOR":
					fileString += formatLine("			{");
					fileString += formatLine("				string [] splitpath = _"+varNames[i]+".Split(new char[]{',', ' '},System.StringSplitOptions.RemoveEmptyEntries);");
					fileString += formatLine("				if(splitpath.Length != 3 && splitpath.Length != 4)");
					fileString += formatLine("					Debug.LogError(\"Incorrect number of paramaters for " + types[i] + " in \" + _" + varNames[i] + " );");
					fileString += formatLine("				float []results = new float[splitpath.Length];");
					fileString += formatLine("				for(int i = 0; i < splitpath.Length; i++)");
					fileString += formatLine("				{");
					fileString += formatLine("					float res;");
					fileString += formatLine("					if(float.TryParse(splitpath[i], out res))");
					fileString += formatLine("					{");
					fileString += formatLine("						results[i] = res;");
					fileString += formatLine("					}");
					fileString += formatLine("					else ");
					fileString += formatLine("					{");
					fileString += formatLine("						Debug.LogError(\"Error parsing \" + " 
					                         + "_"+varNames[i] 
					                         + " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable " 
					                         + colNames[i] +"\");");
					fileString += formatLine("					}");
					fileString += formatLine("				}");
					fileString += formatLine("				"+varNames[i]+".r = results[0];");
					fileString += formatLine("				"+varNames[i]+".g = results[1];");
					fileString += formatLine("				"+varNames[i]+".b = results[2];");
					fileString += formatLine("				if(splitpath.Length == 4)");
					fileString += formatLine("					"+varNames[i]+".a = results[3];");
					fileString += formatLine("			}");
					break;
				case "COLOR32":
					fileString += formatLine("			{");
					fileString += formatLine("				string [] splitpath = _"+varNames[i]+".Split(new char[]{',', ' '},System.StringSplitOptions.RemoveEmptyEntries);");
					fileString += formatLine("				if(splitpath.Length != 3 && splitpath.Length != 4)");
					fileString += formatLine("					Debug.LogError(\"Incorrect number of paramaters for " + types[i] + " in \" + _" + varNames[i] + " );");
					fileString += formatLine("				byte []results = new byte[splitpath.Length];");
					fileString += formatLine("				for(int i = 0; i < splitpath.Length; i++)");
					fileString += formatLine("				{");
					fileString += formatLine("					byte res;");
					fileString += formatLine("					if(byte.TryParse(splitpath[i], out res))");
					fileString += formatLine("					{");
					fileString += formatLine("						results[i] = res;");
					fileString += formatLine("					}");
					fileString += formatLine("					else ");
					fileString += formatLine("					{");
					fileString += formatLine("						Debug.LogError(\"Error parsing \" + " 
					                         + "_"+varNames[i] 
					                         + " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable " 
					                         + colNames[i] +"\");");
					fileString += formatLine("					}");
					fileString += formatLine("				}");
					fileString += formatLine("				"+varNames[i]+".r = results[0];");
					fileString += formatLine("				"+varNames[i]+".g = results[1];");
					fileString += formatLine("				"+varNames[i]+".b = results[2];");
					fileString += formatLine("				if(splitpath.Length == 4)");
					fileString += formatLine("					"+varNames[i]+".a = results[3];");
					fileString += formatLine("			}");
					break;
				case "QUATERNION":
					fileString += formatLine("			{");
					fileString += formatLine("				string [] splitpath = _"+varNames[i]+".Split(new char[]{',', ' '},System.StringSplitOptions.RemoveEmptyEntries);");
					fileString += formatLine("				if(splitpath.Length != 4)");
					fileString += formatLine("					Debug.LogError(\"Incorrect number of paramaters for " + types[i] + " in \" + _" + varNames[i] + " );");
					fileString += formatLine("				float []results = new float[splitpath.Length];");
					fileString += formatLine("				for(int i = 0; i < 4; i++)");
					fileString += formatLine("				{");
					fileString += formatLine("					float res;");
					fileString += formatLine("					if(float.TryParse(splitpath[i], out res))");
					fileString += formatLine("					{");
					fileString += formatLine("						results[i] = res;");
					fileString += formatLine("					}");
					fileString += formatLine("					else ");
					fileString += formatLine("					{");
					fileString += formatLine("						Debug.LogError(\"Error parsing \" + " 
					                         + "_"+varNames[i] 
					                         + " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable " 
					                         + colNames[i] +"\");");
					fileString += formatLine("					}");
					fileString += formatLine("				}");
					fileString += formatLine("				"+varNames[i]+".x = results[0];");
					fileString += formatLine("				"+varNames[i]+".y = results[1];");
					fileString += formatLine("				"+varNames[i]+".z = results[2];");
					fileString += formatLine("				"+varNames[i]+".w = results[3];");
					fileString += formatLine("			}");
					break;
				default: // ????	
					fileString += formatLine("			" + varNames[i] + " = _" + varNames[i] + ";");
					break;
				}
			}
			fileString += formatLine("		}");
			
			fileString += formatLine( System.String.Empty );	
			
			fileString += formatLine("		public string GetStringData( string colID )" );
			fileString += formatLine("		{" );
			fileString += formatLine("			string ret = System.String.Empty;" );
			fileString += formatLine("			switch( colID.ToUpper() )" );
			fileString += formatLine("			{" );
			
			for(int i = 0; i < colNames.Count; i++)
			{
				fileString += formatLine("				case \"" + colNames[i].ToUpper() + "\":" );
				fileString += formatLine("					ret = " + varNames[i] + ".ToString();" );
				fileString += formatLine("					break;" );
			}
			
			fileString += formatLine("			}" );
			fileString += formatLine( System.String.Empty );
			fileString += formatLine("			return ret;" );
			fileString += formatLine("		}" );
			
			fileString += formatLine("		public override string ToString()" );
			fileString += formatLine("		{" );
			fileString += formatLine("			string ret = System.String.Empty;" );
			for( int i = 0; i < colNames.Count; i++ )
			{
				fileString += formatLine("			ret += \"{\" + \"" + colNames[i] + "\" + \" : \" + " + varNames[i] + ".ToString() + \"} \";" );
			}
			fileString += formatLine("			return ret;" );
			fileString += formatLine("		}" );
			
			fileString += formatLine("	}");
			
			
			
			///////////////////////////////////////////////////////////////////////////////
			// the database class itself, this contains the rows defined above
			if ( staticClass )
				fileString += formatLine("	public sealed class " + fileName);
			else
				fileString += formatLine("	public class " + fileName + " :  GoogleFuComponentBase");
			fileString += formatLine("	{");
			
			
			// this is the enums, the enum matches the name of the row
			fileString += formatLine("		public enum rowIds {");
			fileString += ("			");
			for(int i = 0; i < rowNames.Count; i++)
			{
				fileString += (rowNames[i]);
				if(i != rowNames.Count -1)
					fileString += (", ");
				if((i+1) % 20 == 0)
					fileString += System.Environment.NewLine +"			";
			}
			fileString += formatLine( System.String.Empty );
			fileString += formatLine("		};");
			
			
			
			fileString += formatLine("		public string [] rowNames = {");
			fileString += "			";
			for(int i = 0; i < rowNames.Count; i++)
			{
				fileString += "\"" + rowNames[i] + "\"";
				if(i != rowNames.Count - 1)
					fileString += ", ";
				if((i+1) % 20 == 0)
					fileString += System.Environment.NewLine +"			";
			}
			fileString += formatLine(System.Environment.NewLine +"		};");
			// the declaration of the storage for the row data
			fileString += formatLine("		public System.Collections.Generic.List<" + fileName + "Row> Rows = new System.Collections.Generic.List<" + fileName + "Row>();");
			
			// declare the instance as well as the get functionality, if this is going to be a static class
			if(staticClass)
			{
				fileString += formatLine( System.String.Empty );
				fileString += formatLine("		public static " + fileName + " Instance");
				fileString += formatLine("		{");
				fileString += formatLine("			get { return Nested" + fileName + ".instance; }");
				fileString += formatLine("		}");
				fileString += formatLine( System.String.Empty );
				fileString += formatLine("		private class Nested" + fileName + "");
				fileString += formatLine("		{");
				fileString += formatLine("			static Nested" + fileName + "() { }");
				fileString += formatLine("			internal static readonly " + fileName + " instance = new " + fileName + "();");
				fileString += formatLine("		}");
				fileString += formatLine( System.String.Empty );
				fileString += formatLine("		private "+fileName+"()");
				fileString += formatLine("		{");
				
				int rowCt = 0;
				// Iterate through each row, printing its cell values.
				foreach (Google.GData.Spreadsheets.ListEntry row in listFeed.Entries)
				{
					if(typesInFirstRow)
					{
						// skip the first row. This is the title row, and we can get the values later
						if ( rowCt == 0 )
						{
							rowCt++;
							continue;
						}
					}
					
					int colCt = 0;
					System.Collections.Generic.List<string> thisRow = new System.Collections.Generic.List<string>();
					// Iterate over the remaining columns, and print each cell value
					foreach (Google.GData.Spreadsheets.ListEntry.Custom element in row.Elements)
					{
						thisRow.Add ( SanitizeJson(element.Value, true) );
						colCt++;
					}
					
					// Prevent empty row entries
					if ( thisRow[0] == System.String.Empty )
					{
						rowCt++;
						continue;
					}
					
					fileString += "			Rows.Add( new "+fileName+"Row(";
					for(int i = 1; i < thisRow.Count; i++)
					{
						fileString += "\""+thisRow[i]+"\"";
						if(i < thisRow.Count-1)
							fileString += "," + System.Environment.NewLine + "														";
					}
					fileString += formatLine("));");
					
					rowCt++;
				}
				fileString += formatLine("		}");
			}
			else
			{
				// the dont destroy on awake flag
				if ( GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".DND", false ) == true )
				{
					fileString += formatLine( System.String.Empty );
					fileString += formatLine("		void Awake()");
					fileString += formatLine("		{");
					fileString += formatLine("			DontDestroyOnLoad(this);");
					fileString += formatLine("		}");
				}
				
				// this is the processing that actually gets the data into the object itself later on, 
				// this loops through the generic input and seperates it into strings for the above
				// row class to handle and parse into its members
				fileString += formatLine("		public override void AddRowGeneric (System.Collections.Generic.List<string> input)");
				fileString += formatLine("		{");
				fileString += ("			Rows.Add(new "+fileName+"Row(" );
				for(int i = 0; i < types.Count; i++)
				{
					fileString += ("input[" + i + "]");
					if(i != types.Count -1)
						fileString += (",");
				}
				fileString += formatLine("));");
				fileString += formatLine("		}");
				
				fileString += formatLine("		public override void Clear ()");
				fileString += formatLine("		{");
				fileString += formatLine("			Rows.Clear();");
				fileString += formatLine("		}");
			}
			
			
			fileString += formatLine("		public "+fileName+"Row GetRow(rowIds rowID)");
			fileString += formatLine("		{");
			fileString += formatLine("			"+fileName+"Row ret = null;");
			fileString += formatLine("			try");
			fileString += formatLine("			{");
			fileString += formatLine("				ret = Rows[(int)rowID];");
			fileString += formatLine("			}");
			fileString += formatLine("			catch( System.Collections.Generic.KeyNotFoundException ex )");
			fileString += formatLine("			{");
			fileString += formatLine("				Debug.LogError( rowID + \" not found: \" + ex.Message );");
			fileString += formatLine("			}");
			fileString += formatLine("			return ret;");
			fileString += formatLine("		}");
			
			
			fileString += formatLine("		public "+fileName+"Row GetRow(string rowString)");
			fileString += formatLine("		{");
			fileString += formatLine("			"+fileName+"Row ret = null;");
			fileString += formatLine("			try");
			fileString += formatLine("			{");
			fileString += formatLine("				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), rowString)];");
			fileString += formatLine("			}");
			fileString += formatLine("			catch(System.ArgumentException) {");
			fileString += formatLine("				Debug.LogError( rowString + \" is not a member of the rowIds enumeration.\");");
			fileString += formatLine("			}");
			fileString += formatLine("			return ret;");
			fileString += formatLine("		}");
			fileString += formatLine( System.String.Empty );
			fileString += formatLine("	}");
			fileString += formatLine( System.String.Empty );
			fileString += formatLine("}");
			
			sw.Write(fileString);
			
			///////////////////////////////////
			// done writing, clean up
			sw.Flush();
			sw.Close();
			fs.Close();
			
			if ( staticClass == false )
			{
				// Writing out the custom inspector
				///////////////////////////////////////////////
				// open the file 
				sw = null;
				Debug.Log("Saving to: " + _objDBEditorDirectory + "/" + fileName + "Editor.cs" );
				fs = null;
				if( System.IO.File.Exists(_objDBEditorDirectory + "/" + fileName + "Editor.cs"))
				{
					fs = System.IO.File.Open(_objDBEditorDirectory + "/" + fileName + "Editor.cs", System.IO.FileMode.Truncate, System.IO.FileAccess.Write);
				}
				else
					fs = System.IO.File.Open(_objDBEditorDirectory + "/" + fileName + "Editor.cs", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
				
				if (fs == null )
				{
					return;
				}
				
				sw = new System.IO.StreamWriter(fs);
				if( sw == null )
				{
					return;
				}
				
				fileString = System.String.Empty;
				fileString += formatLine("using UnityEngine;");
				fileString += formatLine("using UnityEditor;");
				fileString += formatLine( System.String.Empty );
				fileString += formatLine("namespace GoogleFu");
				fileString += formatLine("{");
				fileString += formatLine("	[CustomEditor(typeof(" + fileName + "))]");
				fileString += formatLine("	public class " + fileName + "Editor : Editor");
				fileString += formatLine("	{");
				fileString += formatLine("		public int Index = 0;");
				fileString += formatLine("		public override void OnInspectorGUI ()");
				fileString += formatLine("		{");
				fileString += formatLine("			" + fileName + " s = target as " + fileName + ";");
				fileString += formatLine("			" + fileName + "Row r = s.Rows[ Index ];");
				fileString += formatLine( System.String.Empty );
				fileString += formatLine("			EditorGUILayout.BeginHorizontal();");
				fileString += formatLine("			if ( GUILayout.Button(\"<<\") )");
				fileString += formatLine("			{");
				fileString += formatLine("				Index = 0;");
				fileString += formatLine("			}");
				fileString += formatLine("			if ( GUILayout.Button(\"<\") )");
				fileString += formatLine("			{");
				fileString += formatLine("				Index -= 1;");
				fileString += formatLine("				if ( Index < 0 )");
				fileString += formatLine("					Index = s.Rows.Count - 1;");
				fileString += formatLine("			}");
				fileString += formatLine("			if ( GUILayout.Button(\">\") )");
				fileString += formatLine("			{");
				fileString += formatLine("				Index += 1;");
				fileString += formatLine("				if ( Index >= s.Rows.Count )");
				fileString += formatLine("					Index = 0;");
				fileString += formatLine("			}");
				fileString += formatLine("			if ( GUILayout.Button(\">>\") )");
				fileString += formatLine("			{");
				fileString += formatLine("				Index = s.Rows.Count - 1;");
				fileString += formatLine("			}");
				fileString += formatLine( System.String.Empty );
				fileString += formatLine("			EditorGUILayout.EndHorizontal();");
				fileString += formatLine( System.String.Empty );
				fileString += formatLine("			EditorGUILayout.BeginHorizontal();");
				fileString += formatLine("			GUILayout.Label( \"ID\", GUILayout.Width( 150.0f ) );");
				fileString += formatLine("			{");
				fileString += formatLine("				EditorGUILayout.LabelField( s.rowNames[ Index ] );");
				fileString += formatLine("			}");
				fileString += formatLine("			EditorGUILayout.EndHorizontal();");
				fileString += formatLine( System.String.Empty );
				
				for(int i = 0; i < types.Count; i++)
				{
					fileString += formatLine("			EditorGUILayout.BeginHorizontal();");
					
					switch ( types[i].ToUpper() )
					{
						
					case "FLOAT":
					case "DOUBLE":
						fileString += formatLine("			GUILayout.Label( \"" + colNames[i] + "\", GUILayout.Width( 150.0f ) );");
						fileString += formatLine("			{");
						fileString += formatLine("				EditorGUILayout.FloatField( r." + varNames[i] + " );");
						fileString += formatLine("			}");
						break;
						
					case "UNSIGNED INT":
					case "INT":
					case "UNSIGNED SHORT":
					case "SHORT":
						fileString += formatLine("			GUILayout.Label( \"" + colNames[i] + "\", GUILayout.Width( 150.0f ) );");
						fileString += formatLine("			{");
						fileString += formatLine("				EditorGUILayout.IntField( r." + varNames[i] + " );");
						fileString += formatLine("			}");
						break;
					case "CHAR":
						fileString += formatLine("			GUILayout.Label( \"" + colNames[i] + "\", GUILayout.Width( 150.0f ) );");
						fileString += formatLine("			{");
						fileString += formatLine("				EditorGUILayout.LabelField( System.Convert.ToString( r." + varNames[i] + " ) );");
						fileString += formatLine("			}");
						break;
					case "BOOL":
						fileString += formatLine("			GUILayout.Label( \"" + colNames[i] + "\", GUILayout.Width( 150.0f ) );");
						fileString += formatLine("			{");
						fileString += formatLine("				EditorGUILayout.Toggle( System.Convert.ToBoolean( r." + varNames[i] + " ) );");
						fileString += formatLine("			}");
						break;
					case "STRING":
						fileString += formatLine("			GUILayout.Label( \"" + colNames[i] + "\", GUILayout.Width( 150.0f ) );");
						fileString += formatLine("			{");
						fileString += formatLine("				EditorGUILayout.LabelField( r." + varNames[i] + " );");
						fileString += formatLine("			}");
						break;
					case "GAMEOBJECT":
						fileString += formatLine("			GUILayout.Label( \"" + colNames[i] + "\", GUILayout.Width( 150.0f ) );");
						fileString += formatLine("			{");
						fileString += formatLine("				EditorGUILayout.ObjectField( r." + varNames[i] + " );");
						fileString += formatLine("			}");
						break;
					case "VECTOR2":
						fileString += formatLine("			EditorGUILayout.Vector2Field( \"" + colNames[i] + "\", r." + varNames[i] + " );");
						break;
					case "VECTOR3":
						fileString += formatLine("			EditorGUILayout.Vector3Field( \"" + colNames[i] + "\", r." + varNames[i] + " );");
						break;
					case "COLOR":
						fileString += formatLine("			GUILayout.Label( \"" + colNames[i] + "\", GUILayout.Width( 150.0f ) );");
						fileString += formatLine("			{");
						fileString += formatLine("				EditorGUILayout.ColorField( r." + varNames[i] + " );");
						fileString += formatLine("			}");	
						break;
					case "COLOR32":
						fileString += formatLine("			GUILayout.Label( \"" + colNames[i] + "\", GUILayout.Width( 150.0f ) );");
						fileString += formatLine("			{");
						fileString += formatLine("				EditorGUILayout.ColorField( r." + varNames[i] + " );");
						fileString += formatLine("			}");
						break;
					case "QUATERNION":
						fileString += formatLine("			EditorGUILayout.Vector4Field( \"" + colNames[i] + "\", r." + varNames[i] + " );");
						break;
					default: // ????	
						
						break;
					}
					
					fileString += formatLine("			EditorGUILayout.EndHorizontal();");
					fileString += formatLine( System.String.Empty );
				}
				
				fileString += formatLine("		}");
				fileString += formatLine("	}");
				fileString += formatLine("}");
				
				
				sw.Write(fileString);
				
				///////////////////////////////////
				// done writing, clean up
				sw.Flush();
				sw.Close();
				fs.Close();
				
				///////////////////////////////////
				// export playmaker actions (check if playmaker is installed first)
				if( _foundPlaymaker && ( GetBool( _activeWorkbook.Title + "." + entry.Title.Text + ".OBJDB" + ".PM", false ) ) )
				{
					/////////////////////////////
					// Generate the Action for Get*DataByID
					sw = null;
					Debug.Log("Saving to: " + _playmakerDirectory + "/GoogleFu/Get" + fileName + "DataByID.cs" );
					fs = null;
					
					if ( System.IO.Directory.Exists( _playmakerDirectory + "/GoogleFu" ) == false )
						System.IO.Directory.CreateDirectory( _playmakerDirectory + "/GoogleFu" );
					
					if( System.IO.File.Exists(_playmakerDirectory + "/GoogleFu/Get" + fileName + "DataByID.cs"))
					{
						fs = System.IO.File.Open(_playmakerDirectory + "/GoogleFu/Get" + fileName + "DataByID.cs", System.IO.FileMode.Truncate, System.IO.FileAccess.Write);
					}
					else
						fs = System.IO.File.Open(_playmakerDirectory + "/GoogleFu/Get" + fileName + "DataByID.cs", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
					
					if (fs == null )
					{
						return;
					}
					
					sw = new System.IO.StreamWriter(fs);
					if( sw == null )
					{
						return;
					}
					
					fileString = System.String.Empty;
					fileString += formatLine("using UnityEngine;");
					fileString += formatLine( System.String.Empty );
					
					fileString += formatLine("namespace HutongGames.PlayMaker.Actions");
					fileString += formatLine("{");
					fileString += formatLine("	[ActionCategory(\"GoogleFu\")]");
					fileString += formatLine("	[Tooltip(\"Gets the specified entry in the " + fileName + " Database.\")]");
					fileString += formatLine("	public class Get" + fileName + "DataByID : FsmStateAction");
					fileString += formatLine("	{");
					fileString += formatLine("		[RequiredField]");
					fileString += formatLine("		[UIHint(UIHint.Variable)]");
					fileString += formatLine("		[Tooltip(\"The object that contains the " + fileName + " database.\")]");
					fileString += formatLine("		public FsmGameObject databaseObj;");
					
					fileString += formatLine("		[RequiredField]");
					fileString += formatLine("		[UIHint(UIHint.Variable)]");
					fileString += formatLine("		[Tooltip(\"Row name of the entry you wish to retrieve.\")]");
					fileString += formatLine("		public FsmString rowName;");
					
					for(int i = 0; i < types.Count; i++)
					{
						string fsmvarType = System.String.Empty;
						string varType = types[i];
						string varName = varNames[i];
						
						switch ( types[i].ToUpper() )
						{
						case "FLOAT":
						case "DOUBLE":
							fsmvarType = "FsmFloat";
							break;
						case "UNISGNED INT":
						case "INT":
						case "UNSIGNED SHORT":
						case "SHORT":
						case "CHAR":
							fsmvarType = "FsmInt";
							break;
						case "BOOLEAN":
						case "BOOL":
							fsmvarType = "FsmBool";
							break;
						case "STRING":
							fsmvarType = "FsmString";
							break;
						case "GAMEOBJECT":
							fsmvarType = "FsmGameObject";
							break;
						case "VECTOR2":
							fsmvarType = "FsmVector2";
							break;
						case "VECTOR3":
							fsmvarType = "FsmVector3";
							break;
						case "COLOR":
						case "COLOR32":
							fsmvarType = "FsmColor";
							break;
						case "QUATERNION":
							fsmvarType = "FsmQuaternion";
							break;
						default: // ????	
							break;
						}
						
						fileString += formatLine("		[UIHint(UIHint.Variable)]");
						fileString += formatLine("		[Tooltip(\"Store the " + varName + " in a " + varType + " variable.\")]");
						
						fileString += formatLine("		public " + fsmvarType + " " + varName + ";");
					}
					
					fileString += formatLine("		public override void Reset()");
					fileString += formatLine("		{");
					fileString += formatLine("			databaseObj = null;");
					fileString += formatLine("			rowName = null;");
					for(int i = 0; i < varNames.Count; i++)
					{
						string varName = varNames[i];
						fileString += formatLine("			" + varName + " = null;");
					}
					fileString += formatLine("		}");
					
					fileString += formatLine("		public override void OnEnter()");
					fileString += formatLine("		{");
					fileString += formatLine("			if ( databaseObj != null && rowName != null && rowName.Value != System.String.Empty )");
					fileString += formatLine("			{");
					fileString += formatLine("				GoogleFu." + fileName + " db = databaseObj.Value.GetComponent<GoogleFu." + fileName + ">();");
					fileString += formatLine("				GoogleFu." + fileName + "Row row = db.GetRow( rowName.Value );");
					
					for(int i = 0; i < varNames.Count; i++)
					{
						string varName = varNames[i];
						fileString += formatLine("				if ( " + varName + " != null )");
						fileString += formatLine("				" + varName + ".Value = row." + varName + ";");
					}
					
					fileString += formatLine("			}");
					fileString += formatLine("			Finish();");
					fileString += formatLine("		}");
					fileString += formatLine("	}");
					fileString += formatLine("}");
					
					sw.Write(fileString);
					
					///////////////////////////////////
					// done writing, clean up
					sw.Flush();
					sw.Close();
					fs.Close();	
					
					/////////////////////////////
					// Generate the Action for Get*DataByIndex
					sw = null;
					Debug.Log("Saving to: " + _playmakerDirectory + "/GoogleFu/Get" + fileName + "DataByIndex.cs" );
					fs = null;
					
					if ( System.IO.Directory.Exists( _playmakerDirectory + "/GoogleFu" ) == false )
						System.IO.Directory.CreateDirectory( _playmakerDirectory + "/GoogleFu" );
					
					if( System.IO.File.Exists(_playmakerDirectory + "/GoogleFu/Get" + fileName + "DataByIndex.cs"))
					{
						fs = System.IO.File.Open(_playmakerDirectory + "/GoogleFu/Get" + fileName + "DataByIndex.cs", System.IO.FileMode.Truncate, System.IO.FileAccess.Write);
					}
					else
						fs = System.IO.File.Open(_playmakerDirectory + "/GoogleFu/Get" + fileName + "DataByIndex.cs", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
					
					if (fs == null )
					{
						return;
					}
					
					sw = new System.IO.StreamWriter(fs);
					if( sw == null )
					{
						return;
					}
					
					fileString = System.String.Empty;
					fileString += formatLine("using UnityEngine;");
					fileString += formatLine( System.String.Empty );
					
					fileString += formatLine("namespace HutongGames.PlayMaker.Actions");
					fileString += formatLine("{");
					fileString += formatLine("	[ActionCategory(\"GoogleFu\")]");
					fileString += formatLine("	[Tooltip(\"Gets the specified entry in the " + fileName + " Database By Index.\")]");
					fileString += formatLine("	public class Get" + fileName + "DataByIndex : FsmStateAction");
					fileString += formatLine("	{");
					fileString += formatLine("		[RequiredField]");
					fileString += formatLine("		[UIHint(UIHint.Variable)]");
					fileString += formatLine("		[Tooltip(\"The object that contains the " + fileName + " database.\")]");
					fileString += formatLine("		public FsmGameObject databaseObj;");
					
					fileString += formatLine("		[RequiredField]");
					fileString += formatLine("		[UIHint(UIHint.Variable)]");
					fileString += formatLine("		[Tooltip(\"Row index of the entry you wish to retrieve.\")]");
					fileString += formatLine("		public FsmInt rowIndex;");
					
					fileString += formatLine("		[UIHint(UIHint.Variable)]");
					fileString += formatLine("		[Tooltip(\"Row ID of the entry.\")]");
					fileString += formatLine("		public FsmString rowName;");
					
					for(int i = 0; i < types.Count; i++)
					{
						string fsmvarType = System.String.Empty;
						string varType = types[i];
						string varName = varNames[i];
						
						switch ( types[i].ToUpper() )
						{
						case "FLOAT":
						case "DOUBLE":
							fsmvarType = "FsmFloat";
							break;
						case "UNISGNED INT":
						case "INT":
						case "UNSIGNED SHORT":
						case "SHORT":
						case "CHAR":
							fsmvarType = "FsmInt";
							break;
						case "BOOLEAN":
						case "BOOL":
							fsmvarType = "FsmBool";
							break;
						case "STRING":
							fsmvarType = "FsmString";
							break;
						case "GAMEOBJECT":
							fsmvarType = "FsmGameObject";
							break;
						case "VECTOR2":
							fsmvarType = "FsmVector2";
							break;
						case "VECTOR3":
							fsmvarType = "FsmVector3";
							break;
						case "COLOR":
						case "COLOR32":
							fsmvarType = "FsmColor";
							break;
						case "QUATERNION":
							fsmvarType = "FsmQuaternion";
							break;
						default: // ????	
							break;
						}
						
						fileString += formatLine("		[UIHint(UIHint.Variable)]");
						fileString += formatLine("		[Tooltip(\"Store the " + varName + " in a " + varType + " variable.\")]");
						
						fileString += formatLine("		public " + fsmvarType + " " + varName + ";");
					}
					
					fileString += formatLine("		public override void Reset()");
					fileString += formatLine("		{");
					fileString += formatLine("			databaseObj = null;");
					fileString += formatLine("			rowIndex = null;");
					for(int i = 0; i < varNames.Count; i++)
					{
						string varName = varNames[i];
						fileString += formatLine("			" + varName + " = null;");
					}
					fileString += formatLine("		}");
					
					fileString += formatLine("		public override void OnEnter()");
					fileString += formatLine("		{");
					fileString += formatLine("			if ( databaseObj != null && rowIndex != null )");
					fileString += formatLine("			{");
					fileString += formatLine("				GoogleFu." + fileName + " db = databaseObj.Value.GetComponent<GoogleFu." + fileName + ">();");
					
					fileString += formatLine("				// For sanity sake, we are going to do an auto-wrap based on the input");
					fileString += formatLine("				// This should prevent accessing the array out of bounds");
					fileString += formatLine("				int i = rowIndex.Value;");
					fileString += formatLine("				int L = db.Rows.Count;");
					fileString += formatLine("				while ( i < 0 )");
					fileString += formatLine("					i += L;");
					fileString += formatLine("				while ( i > L-1 )");
					fileString += formatLine("					i -= L;");
					fileString += formatLine("				GoogleFu." + fileName + "Row row = db.Rows[i];");
					
					fileString += formatLine("				if ( rowName != null && rowName.Value == string.Empty )");
					fileString += formatLine("					rowName.Value = db.rowNames[i];");
					
					for(int i = 0; i < varNames.Count; i++)
					{
						string varName = varNames[i];
						fileString += formatLine("				if ( " + varName + " != null )");
						fileString += formatLine("				" + varName + ".Value = row." + varName + ";");
					}
					
					fileString += formatLine("			}");
					fileString += formatLine("			Finish();");
					fileString += formatLine("		}");
					fileString += formatLine("	}");
					fileString += formatLine("}");
					
					sw.Write(fileString);
					
					///////////////////////////////////
					// done writing, clean up
					sw.Flush();
					sw.Close();
					fs.Close();	
					
					
					/////////////////////////////
					// Generate the Action for Get*DataByIndex
					sw = null;
					Debug.Log("Saving to: " + _playmakerDirectory + "/GoogleFu/Get" + fileName + "Count.cs" );
					fs = null;
					
					if ( System.IO.Directory.Exists( _playmakerDirectory + "/GoogleFu" ) == false )
						System.IO.Directory.CreateDirectory( _playmakerDirectory + "/GoogleFu" );
					
					if( System.IO.File.Exists(_playmakerDirectory + "/GoogleFu/Get" + fileName + "Count.cs"))
					{
						fs = System.IO.File.Open(_playmakerDirectory + "/GoogleFu/Get" + fileName + "Count.cs", System.IO.FileMode.Truncate, System.IO.FileAccess.Write);
					}
					else
						fs = System.IO.File.Open(_playmakerDirectory + "/GoogleFu/Get" + fileName + "Count.cs", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
					
					if (fs == null )
					{
						return;
					}
					
					sw = new System.IO.StreamWriter(fs);
					if( sw == null )
					{
						return;
					}
					
					fileString = System.String.Empty;
					fileString += formatLine("using UnityEngine;");
					fileString += formatLine( System.String.Empty );
					
					fileString += formatLine("namespace HutongGames.PlayMaker.Actions");
					fileString += formatLine("{");
					fileString += formatLine("	[ActionCategory(\"GoogleFu\")]");
					fileString += formatLine("	[Tooltip(\"Gets the specified entry in the " + fileName + " Database By Index.\")]");
					fileString += formatLine("	public class Get" + fileName + "Count : FsmStateAction");
					fileString += formatLine("	{");
					fileString += formatLine("		[RequiredField]");
					fileString += formatLine("		[UIHint(UIHint.Variable)]");
					fileString += formatLine("		[Tooltip(\"The object that contains the " + fileName + " database.\")]");
					fileString += formatLine("		public FsmGameObject databaseObj;");
					
					fileString += formatLine("		[UIHint(UIHint.Variable)]");
					fileString += formatLine("		[Tooltip(\"Row Count of the database.\")]");
					fileString += formatLine("		public FsmInt rowCount;");
					
					fileString += formatLine("		public override void Reset()");
					fileString += formatLine("		{");
					fileString += formatLine("			databaseObj = null;");
					fileString += formatLine("			rowCount = null;");
					fileString += formatLine("		}");
					
					fileString += formatLine("		public override void OnEnter()");
					fileString += formatLine("		{");
					fileString += formatLine("			if ( databaseObj != null && rowCount != null )");
					fileString += formatLine("			{");
					fileString += formatLine("				GoogleFu." + fileName + " db = databaseObj.Value.GetComponent<GoogleFu." + fileName + ">();");
					fileString += formatLine("				rowCount.Value = db.Rows.Count;");
					fileString += formatLine("			}");
					fileString += formatLine("			Finish();");
					fileString += formatLine("		}");
					fileString += formatLine("	}");
					fileString += formatLine("}");
					
					sw.Write(fileString);
					
					///////////////////////////////////
					// done writing, clean up
					sw.Flush();
					sw.Close();
					fs.Close();
				}
			}
			
			ShowNotification( new GUIContent( "Saving to: " + path + "/" + fileName + ".cs" ) );
			Debug.Log("Saving to: " + path );
		}
		
		string formatLine(string line)
		{
			return line + System.Environment.NewLine;
		}
		
		bool isDataValid(System.Collections.Generic.List<string> colNames, System.Collections.Generic.List<string> rowNames)
		{
			bool ret = true;
			
			// colNames cannot contain language keywords, and must also be unique
			{
				// linq GroupBy is slow and confusing.. not using it
				System.Collections.Generic.HashSet<string> hashset = new System.Collections.Generic.HashSet<string>();
				foreach(var name in colNames)
				{
					if(!hashset.Add(name))
					{
						Debug.LogError("Duplicate column name (" + name + ") please check your column names for duplicate names");
						ret = false;
						break;
					}
					if (ContainsKeyword(name))
					{
						Debug.LogError("Unsupported column name (" + name + ") please check you name is not a reserved word or keyword and change this value");
						ret = false;
						break;
					}
				}
			}
			
			// rowNames must be unique, valid enumerations
			if( ret == true )
			{
				// linq GroupBy is slow and confusing.. not using it
				System.Collections.Generic.HashSet<string> hashset = new System.Collections.Generic.HashSet<string>();
				foreach(var name in rowNames)
				{
					if(!hashset.Add(name))
					{
						Debug.LogError("Duplicate row name (" + name + ") please check your row names for duplicate names");
						ret = false;
						break;
					}
					if ( !isValidEnumerationName(name) )
					{
						Debug.LogError("Unsupported row name (" + name + ") please check you name is not a reserved word or keyword and change this value");
						ret = false;
						break;
					}
				}
			}
			
			return ret;
		}
		
		bool isDataValid(System.Collections.Generic.List<string> types, System.Collections.Generic.List<string> colNames, System.Collections.Generic.List<string> rowNames)
		{
			bool ret = true;
			// types must be a type we support
			foreach ( string type in types )
			{
				if ( !isSupportedType( type ) )
				{
					Debug.LogError("Unsupported type " + type + " please check your database column types and change this value to a supported type");
					ret = false;
					break;
				}
			}
			
			// colNames cannot contain language keywords, and must also be unique
			if( ret == true )
			{
				// linq GroupBy is slow and confusing.. not using it
				System.Collections.Generic.HashSet<string> hashset = new System.Collections.Generic.HashSet<string>();
				foreach(var name in colNames)
				{
					if(!hashset.Add(name))
					{
						Debug.LogError("Duplicate column name (" + name + ") please check your column names for duplicate names");
						ret = false;
						break;
					}
					if (ContainsKeyword(name))
					{
						Debug.LogError("Unsupported column name (" + name + ") please check you name is not a reserved word or keyword and change this value");
						ret = false;
						break;
					}
				}
			}
			
			// rowNames must be unique, valid enumerations
			if( ret == true )
			{
				// linq GroupBy is slow and confusing.. not using it
				System.Collections.Generic.HashSet<string> hashset = new System.Collections.Generic.HashSet<string>();
				foreach(var name in rowNames)
				{
					if(!hashset.Add(name))
					{
						Debug.LogError("Duplicate row name (" + name + ") please check your row names for duplicate names");
						ret = false;
						break;
					}
					if ( !isValidEnumerationName(name) )
					{
						Debug.LogError("Unsupported row name (" + name + ") please check you name is not a reserved word or keyword and change this value");
						ret = false;
						break;
					}
				}
			}
			
			return ret;
		}
		
		private bool isSupportedType( string _inType )
		{
			bool ret = false;
			string[] supportedTypes = { "FLOAT", "INT", "UNSIGNED INT", "CHAR", "BYTE", "BOOL",
				"DOUBLE", "STRING", "SHORT", "UNSIGNED SHORT", "VECTOR2", "VECTOR3", "COLOR", "COLOR32", "QUATERNION" };
			
			foreach ( string x in supportedTypes )
			{
				if( _inType.ToUpper().Equals(x) )
				{
					ret = true;
					break;
				}
			}
			return ret;
		}
		
		string MakeValidVariableName( string _inString )
		{
			string ret = _inString;
			if ( ret == System.String.Empty )
			{
				ret = "ANON_" + ANONCOLNAME;
				ANONCOLNAME++;
			}
			
			string[] invalidCharacters = { " ", ",", ".", "?", "\"", ";", ":", 
				"\'", "[", "]", "{", "}", "!", "@", "#", 
				"$", "%", "^", "&", "*", "(", ")", "-", 
				"/", "\\" };
			
			foreach ( string x in invalidCharacters )
				ret = ret.Replace( x, "_" );
			
			ret = "_" + ret;
			
			return ret;
		}
		
		private bool isValidEnumerationName( System.String _inString )
		{
			bool ret = true;
			
			if ( _inString == System.String.Empty )
				ret = false;
			
			string[] invalidStarts = { " ", "0", "1", "2", "3", "4", "5", 
				"6", "7", "8", "9", "\t", "\n", "\r" };
			foreach ( string x in invalidStarts )
			{
				if( _inString.StartsWith(x) )
				{
					Debug.LogError("Found invalid starting character: ( " + x + " ) in word " + _inString);
					ret = false;
					break;
				}
			}
			return ret;
		}
		
		private bool isValidCharacters( System.String _inString )
		{
			string[] invalidCharacters = { " ", ",", ".", "?", "\"", ";", ":", 
				"\'", "[", "]", "{", "}", "!", "@", "#", 
				"$", "%", "^", "&", "*", "(", ")", "-", 
				"/", "\\" };
			foreach ( string x in invalidCharacters )
			{
				if(_inString.Contains(x))
				{
					Debug.LogError("Found bad character: ( " + x + " ) in word " + _inString);
					return false;
				}
			}
			return true;
		}
		
		private bool ContainsKeyword( System.String _inString )
		{
			bool ret = false;
			
			string[] stringArray = { "abstract", "event", "new", "struct",
				"as", "explicit", "null", "switch",
				"base", "extern", "object", "this",
				"bool", "false", "operator", "throw",
				"break", "finally", "out", "true",
				"byte", "fixed", "override", "try",
				"case", "float", "params", "typeof",
				"catch", "for", "private", "uint",
				"char", "foreach", "protected", "ulong",
				"checked", "goto", "public", "unchecked",
				"class", "if", "readonly", "unsafe",
				"const", "implicit", "ref", "ushort",
				"continue", "in", "return", "using",
				"decimal", "int", "sbyte", "virtual",
				"default", "interface", "sealed", "volatile",
				"delegate", "internal", "short", "void",
				"do", "is", "sizeof", "while",
				"double", "lock", "stackalloc",
				"else", "long", "static",
				"enum", "namespace", "string" }; 
			
			foreach (string x in stringArray)
			{
				if (x.Equals(_inString))
				{
					Debug.LogError("Found the keyword: ( " + x + " ) this is not allowed");
					ret = true;
					break;
				}
			}
			
			return ret;
		}
	}
}
