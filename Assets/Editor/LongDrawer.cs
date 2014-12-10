using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer (typeof (long))]
public class LongDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position = EditorGUI.PrefixLabel(position, label);

		GUI.Label(position, property.ToString());
	}
}
