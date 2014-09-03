// C# Example
// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class ExportAssetBundles 
{

	[MenuItem("Assets/Build AssetBundle From Selection %t")]
	static void ExportResource () {
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel ("Save Resource", "Bundles", Selection.activeObject.name, "unity3d");
		if (path.Length != 0) {
			// Build the resource file from the active selection.
			Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
			foreach (var item in selection) 
			{
				Debug.Log(item.name);
			}
			BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
			                               BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
			                               BuildTarget.Android);
			Selection.objects = selection;
		}
	}

	[MenuItem("Assets/Build Character Bundles")]
	static void TestBuild () 
	{
		string path = "Assets/Bundles";
		string savePath;
		Object[] files;
		FileInfo[] info;
		DirectoryInfo dir = new DirectoryInfo (path);
		DirectoryInfo[] dirs = dir.GetDirectories();
		for(int i = 0; i < dirs.Length; i++)
		{
			EditorUtility.DisplayProgressBar("Building bundles", "Bundle #" + i + "/" + dirs.Length, (float)i / dirs.Length);

			DirectoryInfo animationFolder = dirs[i];
			info = animationFolder.GetFiles().Where(x => !x.Name.EndsWith(".meta", System.StringComparison.CurrentCultureIgnoreCase)).ToArray();
			files = new Object[info.Length];
			savePath = "Bundles/" + animationFolder.Name + ".unity3d";
			Debug.LogWarning("Building: " + animationFolder.Name);
			for (int j = 0; j < info.Length; j++)
			{
				//Debug.Log("Adding File: " + path + "/" + animationFolder.Name + "/" + info[i].Name);
				files[j] = AssetDatabase.LoadAssetAtPath(path + "/" + animationFolder.Name + "/" + info[j].Name, typeof(Object));
			}
			BuildPipeline.BuildAssetBundle(files[0], files, savePath, 
				BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
				BuildTarget.Android);
		}
		Debug.Log ("Done!");
		EditorUtility.ClearProgressBar();
	}

}