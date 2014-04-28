using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[System.Serializable]
public class MSGridNode : IComparable {
	
	/// <summary>
	/// The parent of this node.
	/// For retracing a path once we've found
	/// the solution.
	/// </summary>
	public MSGridNode parent;
	
	/// <summary>
	/// Grid coordinates
	/// </summary>
	public int x, z;
	
	/// <summary>
	/// The heuristic value of this node
	/// </summary>
	public float heur = 0;
	
	/// <summary>
	/// The distance from the root
	/// </summary>
	public float dist = 0;
	
	/// <summary>
	/// The direction that this node was entered from
	/// </summary>
	public MSValues.Direction direction = MSValues.Direction.NONE;
	
	private static readonly Dictionary<MSValues.Direction, Vector2> dirDict = new Dictionary<MSValues.Direction, Vector2>()
	{
		{MSValues.Direction.NORTH, new Vector2(0,1)},
		{MSValues.Direction.SOUTH, new Vector2(0,-1)},
		{MSValues.Direction.EAST, new Vector2(1,0)},
		{MSValues.Direction.WEST, new Vector2(-1,0)}
	};
	
	private static readonly Vector2[] moveDirs =
	{
		new Vector2(0,1),
		new Vector2(1,1),
		new Vector2(1,0),
		new Vector2(0,-1),
		new Vector2(-1,-1),
		new Vector2(-1,0),
		new Vector2(-1,1),
		new Vector2(1,-1)
	};
		
	
	/// <summary>
	/// Gets the cost of this node, for sorting
	/// </summary>
	/// <value>
	/// The cost of this node
	/// </value>
	public float cost{
		get
		{
			return dist + heur;
		}
	}
	
	public Vector2 pos{
		get
		{
			return new Vector2(x, z);
		}
	}
	
	public Vector3 worldPos{
		get
		{
			return new Vector3(x * MSGridManager.instance.spaceSize, 0,
				z * MSGridManager.instance.spaceSize);
		}
	}
	
	public MSGridNode()
	{
		x = 0;
		z = 0;
	}
	
	public MSGridNode(int _x, int _y)
	{
		x = _x;
		z = _y;
	}
	
	public MSGridNode(Vector2 _pos)
	{
		x = (int)_pos.x;
		z = (int)_pos.y;
	}
	
	public static MSGridNode operator +(MSGridNode n1, MSGridNode n2)
	{
		return new MSGridNode(n1.pos + n2.pos);
	}
	
	public static implicit operator Vector2(MSGridNode value)
	{
		return value.pos;
	}
	
	public static implicit operator MSGridNode(Vector2 value)
	{
		return new MSGridNode(value);
	}
	
	/// <summary>
	/// Sets the heuristic using manhattan distance
	/// </summary>
	/// <param name='destination'>
	/// Destination.
	/// </param>
	public void SetHeur(MSGridNode destination)
	{
		heur = Mathf.Abs(destination.x - x) + Mathf.Abs (destination.z - z);
	}
	
	public Dictionary<MSValues.Direction, MSGridNode> GetNeighs()
	{
		Dictionary<MSValues.Direction, MSGridNode> neighs = new Dictionary<MSValues.Direction, MSGridNode>();
		
		foreach (KeyValuePair<MSValues.Direction, Vector2> item in dirDict) 
		{
			if (MSGridManager.instance.CanWalkInDir(this, item.Value)) 
			{
				MSGridNode node = new MSGridNode(pos + item.Value);
				node.dist = dist + 1;
				node.parent = this;
				node.direction = item.Key;
				neighs.Add (item.Key, node);
			}
		}
		
		return neighs;
	}
	
	/// <summary>
	/// Gets all neighbors.
	/// </summary>
	/// <returns>
	/// The neighbors.
	/// </returns>
	public List<MSGridNode> GetNeighbors()
	{
		List<MSGridNode> neighs = new List<MSGridNode>();
		
		for (int i = 0; i < moveDirs.Length; i++) 
		{
			if (MSGridManager.instance.CanWalkInDir(this, moveDirs[i])) //This is never working!!!
			{
				MSGridNode node = new MSGridNode(pos + moveDirs[i]);
				node.dist = dist + 1;
				node.parent = this;
				neighs.Add(node);
			}
		}
		
		return neighs;
	}
	
	/// <summary>
	/// Compares costs
	/// </summary>
	/// <returns>
	/// Comparison turnery int
	/// </returns>
	/// <param name='obj'>
	/// Other grid node
	/// </param>
	public int CompareTo(object obj)
	{
		if (!(obj is MSGridNode))
		{
			Debug.LogError("Cannot compare grid nodes to other types");
			return -1;
		}
		
		return cost.CompareTo((obj as MSGridNode).cost);
	}
}
