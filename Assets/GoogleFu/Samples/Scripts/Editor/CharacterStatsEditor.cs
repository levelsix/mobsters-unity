using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace GoogleFuSample
{
	[CustomEditor(typeof(CharacterStats))]
	public class CharacterStatsEditor : Editor
	{
		public int Index = 0;
		public override void OnInspectorGUI ()
		{
			CharacterStats s = target as CharacterStats;
			CharacterStatsRow r = s.Rows[ Index ];

			EditorGUILayout.BeginHorizontal();
			if ( GUILayout.Button("<<") )
			{
				Index = 0;
			}
			if ( GUILayout.Button("<") )
			{
				Index -= 1;
				if ( Index < 0 )
					Index = s.Rows.Count - 1;
			}
			if ( GUILayout.Button(">") )
			{
				Index += 1;
				if ( Index >= s.Rows.Count )
					Index = 0;
			}
			if ( GUILayout.Button(">>") )
			{
				Index = s.Rows.Count - 1;
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "ID", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( s.rowNames[ Index ] );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "NAME", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._NAME );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "LEVEL", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.IntField( r._LEVEL );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "BASEMODIFIER", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.FloatField( r._BASEMODIFIER );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "CLASS", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._CLASS );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "STRENGTH", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.IntField( r._STRENGTH );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "ENDURANCE", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.IntField( r._ENDURANCE );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "INTELLIGENCE", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.IntField( r._INTELLIGENCE );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "DEXTERITY", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.IntField( r._DEXTERITY );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "HEALTH", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.IntField( r._HEALTH );
			}
			EditorGUILayout.EndHorizontal();

		}
	}
}
