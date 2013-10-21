using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(CBKUnit))]
public class CBKCityUnit : MonoBehaviour, CBKISelectable {
	
	public Color baseColor;
	public Color selectColor;
	private Color _currColor;
	
	CBKUnit unit;
	
	Transform trans;
	
	CBKGridNode target = null;
	
	Stack<CBKGridNode> path = new Stack<CBKGridNode>();
	
	const float MIN_DIST = .03f;
	
	bool moving = true;
	
	bool _selected = false;
	
	/// <summary>
	/// The amount of time it takes the tint to ping-pong
	/// when this building is selected
	/// </summary>
	private const float COLOR_SPEED = 1.5f;
	
	const float SQUARES_PER_SECOND = 1;
	
	void Awake()
	{
		unit = GetComponent<CBKUnit>();
		trans = transform;
	}
	
	public void Init()
	{
		//Put on a random walkable square
		CBKGridNode node = CBKGridManager.instance.randomWalkable;
		trans.position = node.worldPos;
		
		path = PlanPath(null, ChooseTarget());
		MoveNext();
		trans.position = target.worldPos;
	}
	
	CBKGridNode ChooseTarget()
	{
		CBKGridNode node = CBKGridManager.instance.randomWalkable;
		return node;
	}
	
	void Update()
	{
		if (moving)
		{
			Vector3 move = Vector3.zero;
			float dist = Time.deltaTime * SQUARES_PER_SECOND * CBKGridManager.instance.spaceSize;
			switch (unit.direction) 
			{
				case CBKValues.Direction.NORTH:
					move.z = dist;
					break;
				case CBKValues.Direction.SOUTH:
					move.z = -dist;
					break;
				case CBKValues.Direction.EAST:
					move.x = dist;
					break;
				case CBKValues.Direction.WEST:
					move.x = -dist;
					break;
				default:
					break;
			}
			trans.Translate(move);
		}
		if ((trans.position - target.worldPos).sqrMagnitude < MIN_DIST)
		{
			//trans.position = target.worldPos;
			MoveNext();
		}
	}
	
	void MoveNext()
	{
		if (path == null || path.Count == 0)
		{
			path = PlanPath(target, ChooseTarget());
		}
		SetTarget(path.Pop());
		unit.animat = CBKUnit.AnimationType.RUN;
	}
	
	public void SetTarget(CBKGridNode node)
	{
		//Debug.Log("Setting target to " + node.pos);
		target = node;
		unit.direction = node.direction;
		unit.sprite.depth = -(node.x + node.z);
	}
	
	public void Select()
	{
		unit.anim.framesPerSecond = 0;
		moving = false;
		_selected = true;
		
		_currColor = selectColor;
		StartCoroutine(ColorPingPong());
	}
	
	public void Deselect ()
	{
		unit.anim.framesPerSecond = 15;
		moving = true;
		_selected = false;
	}
	
	#region Pathfinding Methods
	
	Stack<CBKGridNode> PlanPath(CBKGridNode start, CBKGridNode end)
	{
		if (start == null)
		{
			start = new CBKGridNode(CBKGridManager.instance.PointToGridCoords(trans.position));
		}
		
		//Debug.Log("Path from " + start.pos + " to " + end.pos);
		
		start.parent = null;
		
		List<CBKGridNode> open = new List<CBKGridNode>();
		Dictionary<Vector2, CBKGridNode> closed = new Dictionary<Vector2, CBKGridNode>();
		
		open.Add(start);
		//Debug.Log("Added " + start.pos + " to open list");
		
		CBKGridNode current;
		while(open.Count > 0)
		{
			current = open[0];
			open.RemoveAt(0);
			
			if (current.pos == end.pos)
			{
				return BuildPath(current);
			}
			
			closed[current.pos] = current;
			
			foreach (KeyValuePair<CBKValues.Direction,CBKGridNode> item in current.GetNeighs())
			{
				item.Value.SetHeur(end);
				if (closed.ContainsKey(item.Value.pos) && closed[item.Value.pos].cost <= item.Value.cost)
				{
					continue;
				}
				
				open.Add(item.Value);
				closed[item.Value.pos] = item.Value;
			}
			
			open.Sort();
		}
		Debug.LogError("Null path...");
		return null;
		
	}
	
	private static Stack<CBKGridNode> BuildPath(CBKGridNode end)
	{
		Stack<CBKGridNode> buildPath = new Stack<CBKGridNode>();
		string str = "Path: ";
		CBKGridNode current = end;
		while(current != null)
		{
			buildPath.Push(current);
			str += "\n" + current.pos;
			current = current.parent;
		}
		//Debug.Log(str);
		
		return buildPath;
	}
	
	#endregion
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		foreach (CBKGridNode item in path) 
		{
			Gizmos.DrawCube(item.worldPos, new Vector3(.2f, .2f, .2f));
		}
	}
	
	IEnumerator ColorPingPong()
	{
		int _ppDir = 1;
		float _ppPow = 0;
		while(_selected)
		{
			_ppPow += _ppDir * Time.deltaTime * COLOR_SPEED;
			
			//See if we need to change direction
			if (_ppPow >= 1)
			{
				_ppPow = 1;
				_ppDir = -1;
			}
			else if (_ppPow <= 0)
			{
				_ppPow = 0;
				_ppDir = 1;
			}
			
			unit.sprite.color = Color.Lerp(baseColor, _currColor, _ppPow);
			
			yield return new WaitForEndOfFrame();
		}
		unit.sprite.color = baseColor;
	}
}
