using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MSEnhancementManager))]
public class MSEnhancementManagerEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		MSEnhancementManager enMan = (MSEnhancementManager)target;

		if (enMan.tempEnhancementMonster != null)
		{
			EditorGUILayout.LabelField("Temp Monster: " + enMan.tempEnhancementMonster.monster.monsterId);
		}
		else
		{
			EditorGUILayout.LabelField("No temp monster");
		}

		EditorGUILayout.LabelField ("Start cost: " + enMan.startCost);
		EditorGUILayout.LabelField ("Has Enhancement: " + enMan.hasEnhancement);

		if (enMan.enhancementMonster != null)
		{
			EditorGUILayout.LabelField ("Enhancement Monster: " + enMan.enhancementMonster.monster.monsterId);
		}
		else
		{
			EditorGUILayout.LabelField ("No enhancement monster");
		}
	}

}
