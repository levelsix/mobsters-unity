using UnityEngine;
using System.Collections;

public class MSWalkableSpace : CBKITakesGridSpace {

	public bool walkable {
		get {
			return true;
		}
	}
	
	public MSGridNode node
	{
		get
		{
			return new MSGridNode(pos);
		}
	}
	
	public Vector2 pos;
	
	public MSWalkableSpace(Vector2 gridPos)
	{
		pos = gridPos;
	}
}
