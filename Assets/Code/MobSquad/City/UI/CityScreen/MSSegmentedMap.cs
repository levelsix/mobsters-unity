using UnityEngine;
using System.Collections;

public class MSSegmentedMap : MonoBehaviour {

	/// <summary>
	/// List of map objects that make up the full map.
	/// The size of the maps array is defined in the editor.
	/// </summary>
	public UI2DSprite[] maps = new UI2DSprite[1];

	static private int MAP_DEPTH = 10;

	public float Height{
		get{
			float height = 0;
			LineUpMaps();
			foreach(UI2DSprite map in maps)
			{
				height += map.height;
			}
			return height;
		}
	}

	void Awake()
	{
		LineUpMaps();
	}

	[ContextMenu("Line up maps")]
	public void LineUpMaps(){
		maps[0].depth = MAP_DEPTH;
		for (int i = 1; i < maps.Length; i++) {
			maps[i].MakePixelPerfect();
			float newY = maps[i-1].transform.localPosition.y + (maps[i-1].height / 2f) + (maps[i].height / 2f);
			maps[i].transform.localPosition = new Vector3(0f ,newY ,0f);
			maps[i].depth = MAP_DEPTH;
		}

	}
}
