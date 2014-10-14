using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MSSegmentedMap : MonoBehaviour {

	/// <summary>
	/// List of map objects that make up the full map.
	/// The size of the maps array is defined in the editor.
	/// </summary>
	public List<UI2DSprite> maps = new List<UI2DSprite>();

	[SerializeField] UI2DSprite mapSegmentPrefab;

	static private int MAP_DEPTH = 1;

	public float Height{
		get{
			float height = 0;
			foreach(UI2DSprite map in maps)
			{
				height += map.height;
			}
			return height;
		}
	}

	Transform trans;
	
	/// <summary>
	/// Begins loading in maps which may take some time because internet
	/// </summary>
	public void LoadAllMaps(Action OnFinish)
	{
		trans = transform;
		
		//We have to clear the maps because I have some loaded maps in the editor
		maps.Clear();
		trans.DestroyChildren();

		//This is to sneak LineUpMaps in before OnFinish
		Action newAction = LineUpMaps + OnFinish;

		MSSpriteUtil.instance.RunForEachTypeInBundle<Sprite>("TaskMaps", CreateMapSegment, newAction);
	}

	void CreateMapSegment(Sprite sprite)
	{
		UI2DSprite newSegment = MSPoolManager.instance.Get<UI2DSprite>(mapSegmentPrefab, trans);
		newSegment.sprite2D = sprite;
		newSegment.transform.localScale = Vector3.one;
		maps.Add(newSegment);
	}

	[ContextMenu("Line up maps")]
	public void LineUpMaps(){
		maps[0].MakePixelPerfect();
		float newY = (maps[0].height / 2f);
		maps[0].transform.localPosition = new Vector3(0f ,newY ,0f);
		maps[0].depth = MAP_DEPTH;
		for (int i = 1; i < maps.Count; i++) {
			maps[i].MakePixelPerfect();
			newY = maps[i-1].transform.localPosition.y + (maps[i-1].height / 2f) + (maps[i].height / 2f);
			maps[i].transform.localPosition = new Vector3(0f ,newY ,0f);
			maps[i].depth = MAP_DEPTH;
		}

	}
}
