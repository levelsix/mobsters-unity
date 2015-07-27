using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class NGUIHelper : MonoBehaviour {

	
	[MenuItem("NGUI/Helper/RefreshAtlas", false, 9)]
	public static void RefreshAtlas() {
		Object[] selection = Selection.GetFiltered (typeof(UIAtlas), SelectionMode.Assets);
		List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry> ();
		foreach (UIAtlas atlas in selection) {
			UIAtlasMaker.ExtractSprites(atlas, sprites);
			UIAtlasMaker.UpdateAtlas(atlas, sprites);
			sprites.Clear();
		}
	}
}
