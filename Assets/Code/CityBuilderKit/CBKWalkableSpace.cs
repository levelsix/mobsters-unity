using UnityEngine;
using System.Collections;

public class CBKWalkableSpace : CBKITakesGridSpace {

	public bool walkable {
		get {
			return true;
		}
	}
	
	public CBKGridNode node
	{
		get
		{
			return new CBKGridNode(pos);
		}
	}
	
	public Vector2 pos;
	
	public CBKWalkableSpace(Vector2 gridPos)
	{
		pos = gridPos;
	}
}
