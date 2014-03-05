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
	
	public float speed = 1;

	public bool rushing = false;

	public UISprite hoverIcon;

	bool locked = false;

	[SerializeField]
	TweenRotation lockRotateTween;
	[SerializeField]
	TweenPosition arrowPosTween;
	[SerializeField]
	TweenPosition arrowScaleTween;
	
	const string LOCK_SPRITE_NAME = "lockedup";
	const string ARROW_SPRITE_NAME = "arrow";

	void Awake()
	{
		unit = GetComponent<CBKUnit>();
		trans = transform;
	}
	
	public void Init()
	{
		hoverIcon.gameObject.SetActive(false);
		//Put on a random walkable square
		CBKGridNode node = MSGridManager.instance.randomWalkable;
		trans.position = node.worldPos;
		
		path = PlanPath(null, ChooseTarget());
		MoveNext();
		trans.position = target.worldPos;
	}
	
	CBKGridNode ChooseTarget()
	{
		CBKGridNode node = MSGridManager.instance.randomWalkable;
		return node;
	}
	
	void Update()
	{
		if (moving)
		{
			Vector3 move = Vector3.zero;
			float dist = Time.deltaTime * speed * MSGridManager.instance.spaceSize;
			if (rushing)
			{
				dist *= 4;
			}
			switch (unit.direction) 
			{
				case MSValues.Direction.NORTH:
					move.z = dist;
					break;
				case MSValues.Direction.SOUTH:
					move.z = -dist;
					break;
				case MSValues.Direction.EAST:
					move.x = dist;
					break;
				case MSValues.Direction.WEST:
					move.x = -dist;
					break;
				default:
					break;
			}
			trans.Translate(move);
		}
		if (IsPastTarget())
		{
			//trans.position = target.worldPos;
			MoveNext();
		}
	}

	bool IsPastTarget()
	{
		switch (unit.direction)
		{
		case MSValues.Direction.NORTH:
			return trans.position.z > target.worldPos.z - MIN_DIST;
		case MSValues.Direction.SOUTH:
			return trans.position.z < target.worldPos.z + MIN_DIST;
		case MSValues.Direction.WEST:
			return trans.position.x < target.worldPos.x + MIN_DIST;
		case MSValues.Direction.EAST:
			return trans.position.x > target.worldPos.x - MIN_DIST;
		default:
			return true;
		}
	}
	
	void MoveNext()
	{
		if (path == null || path.Count == 0)
		{
			path = PlanPath(target, ChooseTarget());
		}
		SetTarget(path.Pop());
	}
	
	public void SetTarget(CBKGridNode node)
	{
		//Debug.Log("Setting target to " + node.pos);
		target = node;
		if (unit.direction != node.direction)
		{
			unit.direction = node.direction;
			unit.animat = CBKUnit.AnimationType.RUN;
		}
		if (unit.direction == MSValues.Direction.NORTH || unit.direction == MSValues.Direction.SOUTH)
		{
			trans.position = new Vector3(node.worldPos.x, trans.position.y, trans.position.z);
		}
		else
		{
			trans.position = new Vector3(trans.position.x, trans.position.y, node.worldPos.z);
		}
		//unit.sprite.depth = -(node.x + node.z) - 10;
	}

	public void SetLocked()
	{
		locked = true;
		unit.sprite.color = selectColor;
		hoverIcon.gameObject.SetActive(true);
		hoverIcon.spriteName = LOCK_SPRITE_NAME;
	}

	public void SetUnlocked()
	{
		locked = false;
		unit.sprite.color = baseColor;
		hoverIcon.gameObject.SetActive(false);
	}

	public void SetArrow()
	{
		hoverIcon.spriteName = ARROW_SPRITE_NAME;
		arrowPosTween.PlayForward();
		arrowScaleTween.PlayForward();
	}
	
	public void Select()
	{
		if (locked)
		{
			lockRotateTween.ResetToBeginning();
			lockRotateTween.PlayForward();
		}
		else
		{
			//unit.anim.framesPerSecond = 0;
			moving = false;
			_selected = true;
			
			_currColor = selectColor;
			StartCoroutine(ColorPingPong());
		}
	}
	
	public void Deselect ()
	{
		//unit.anim.framesPerSecond = 15;
		moving = true;
		_selected = false;
	}
	
	#region Pathfinding Methods
	
	Stack<CBKGridNode> PlanPath(CBKGridNode start, CBKGridNode end)
	{
		rushing = false;

		if (start == null)
		{
			start = new CBKGridNode(MSGridManager.instance.PointToGridCoords(trans.position));
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
			
			foreach (KeyValuePair<MSValues.Direction,CBKGridNode> item in current.GetNeighs())
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
		string str = "Null path: " + start.pos + " to " + end.pos;
		str += "\nOpen: ";
		foreach (var item in open)
		{
			str += item.pos + ", ";
		}
		str += "\nClosed: ";
		foreach (var item in closed)
		{
			str += item.Key + ", ";
		}
		
		Debug.LogError(str);
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
			Gizmos.DrawCube(item.worldPos + new Vector3(MSGridManager.instance.spaceSize/2, 0, MSGridManager.instance.spaceSize/2), new Vector3(.2f, .2f, .2f));
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
