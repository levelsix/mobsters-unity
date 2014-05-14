using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(MSBuilding))]
public class MSPlacementGrid : MonoBehaviour {

	[SerializeField]
	MSSimplePoolable tile;

	MSPoolManager pool;
	
	MSBuilding building;

	Transform trans;

	List<MSSimplePoolable> tiles = new List<MSSimplePoolable>();

	void Awake(){
		pool = MSPoolManager.instance;
		building = GetComponent<MSBuilding> ();
		trans = transform;

		building.OnSelect += OnBuildingSelect;
		building.OnDeselect += OnBuildingDeselect;

		MSActionManager.Town.OnBuildingDragStart += OnDragStart;
		MSActionManager.Town.OnBuildingDragEnd += OnDragEnd;

		if (tile == null) {
			Debug.LogError("PlacementGrid.Tile should not be NULL");
		}
	}

	void OnDestroy(){
		building.OnSelect -= OnBuildingSelect;
		building.OnDeselect -= OnBuildingDeselect;

		MSActionManager.Town.OnBuildingDragStart -= OnDragStart;
		MSActionManager.Town.OnBuildingDragEnd -= OnDragEnd;
	}

	void OnBuildingSelect(){
		MSSimplePoolable tmpTile;
		float end = -(building.width - 1f) / 2f;
		float scale = 0.9f;

		for (int i = 0; i < building.width; i++) {
			for(int j = 0; j < building.length; j++){
				tmpTile = pool.Get(tile,Vector3.zero,trans) as MSSimplePoolable;
				tmpTile.transf.localPosition = new Vector3((end + i) * scale, 0, (end + j) * scale);
				tmpTile.transf.eulerAngles = new Vector3(90f, 45f ,0f);
				tmpTile.gameObject.SetActive(false);
				tiles.Add(tmpTile);
			}
		}
	}

	void OnDragStart(){
		foreach (var tile in tiles) {
			tile.gameObject.SetActive(true);
		}
	}

	public void updateSprites(){
		foreach (var tile in tiles) {
			MSGridManager gridMan = MSGridManager.instance;
			if(gridMan.IsOpen(gridMan.PointToGridCoords(tile.transf.position))){
				tile.GetComponent<UISprite>().spriteName = "greentile";
			}else{
				tile.GetComponent<UISprite>().spriteName = "redtile";
			}
		}
	}

	void OnDragEnd(){
		foreach (var tile in tiles) {
			tile.gameObject.SetActive(false);
		}
	}

	void OnBuildingDeselect(){
		foreach (var tile in tiles) {
			tile.GetComponent<UISprite>().spriteName = "greentile";
			tile.Pool();
		}
		tiles.Clear();
	}
}
