using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @author Rob Giusti
/// PZ gem.
/// Represents a single gem in the puzzle game.
/// Handles its own falling and board checking.
/// Also, turns into special gems when necessary.
/// </summary>
public class PZGem : MonoBehaviour, CBKIPoolable {
	
	[SerializeField]
	UILabel specialLabel;
	
	PZGem _prefab;
	public CBKIPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as PZGem;
		}
	}
	
	Transform trans;
	GameObject gameObj;
	
	public Transform transf {
		get {
			return trans;
		}
	}
	
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	
	/// <summary>
	/// The sprite, which we use to change tint and image
	/// when necessary
	/// </summary>
	public UISprite sprite;
	
	public enum GemType {NORMAL, ROCKET, BOMB, MOLOTOV};
	
	private GemType _gemType;
	
	public GemType gemType
	{
		get
		{
			return _gemType;
		}
		set
		{
			specialLabel.transform.localRotation = Quaternion.identity;
			switch(value)
			{
				case GemType.BOMB:
					specialLabel.text = "B";
					break;
				case GemType.MOLOTOV:
					specialLabel.text = "M";
					break;
				case GemType.ROCKET:
					specialLabel.text = "R";
					if (!horizontal)
					{
						specialLabel.transform.localRotation = new Quaternion(0, 0, .707f, .707f);
					}
					break;
				case GemType.NORMAL:
					specialLabel.text = "";
					break;
			}
			_gemType = value;
		}
	}
	
	public bool horizontal = true;
	
	public int boardX, boardY;
	
	const float DRAG_THRESHOLD = 70;
	
	const float SPACE_SIZE = 72;
	
	const float BASE_FALL_SPEED = -150;
	
	const float GRAVITY = -150.1f;
	
	const float SWAP_TIME = .2f;
	
	public int colorIndex = 0;
	
	[SerializeField]
	Vector2 currDrag = Vector2.zero;
	
	public bool moving = false;
	
	bool dragged = false;
	
	static readonly Dictionary<CBKValues.Direction, Vector3> dirVals = new Dictionary<CBKValues.Direction, Vector3>()
	{
		{CBKValues.Direction.NORTH, new Vector3(0,1)},
		{CBKValues.Direction.SOUTH, new Vector3(0,-1)},
		{CBKValues.Direction.EAST, new Vector3(1,0)},
		{CBKValues.Direction.WEST, new Vector3(-1,0)}
	};
	
	void Awake()
	{
		sprite = GetComponent<UISprite>();
		trans = transform;
		gameObj = gameObject;
	}
	
	public CBKIPoolable Make (Vector3 origin)
	{
		PZGem gem = Instantiate(this, origin, Quaternion.identity) as PZGem;
		gem.prefab = this;
		return gem;
	}
	
	public void Init(int colr, int column)
	{
		colorIndex = colr;
		sprite.color = PZPuzzleManager.instance.colors[colorIndex];
		
		boardX = column;
		boardY = PZPuzzleManager.BOARD_HEIGHT;
		
		trans.localScale = Vector3.one;
		trans.localPosition = new Vector3(boardX * SPACE_SIZE, boardY * SPACE_SIZE);
		
		gemType = GemType.NORMAL;
		
		moving = false;
		CheckFall();
	}
	
	public void CheckFall()
	{
		int targetY = boardY;
		while(targetY > 0)
		{
			if (PZPuzzleManager.instance.board[boardX, targetY-1] != null)
			{
				break;
			}
			targetY--;
		}
		
		if (boardY != targetY)
		{
			//If this gem is above the board, it is new and doesn't need to be removed.
			//Otherwise, it's falling from within the board, so we need to mark its old
			//space as empty
			if (boardY < PZPuzzleManager.BOARD_HEIGHT)
			{
				PZPuzzleManager.instance.board[boardX, boardY] = null;
			}
			PZPuzzleManager.instance.board[boardX, targetY] = this;
			boardY = targetY;
			if (!moving)	
			{
				StartCoroutine(Fall());
			}
		}
	}
	
	IEnumerator Fall()
	{
		moving = true;
		PZPuzzleManager.instance.OnStartMoving(this);
		float fallSpeed = BASE_FALL_SPEED;
		while(trans.localPosition.y > boardY * SPACE_SIZE)
		{
			yield return null;
			fallSpeed += GRAVITY * Time.deltaTime;
			trans.localPosition = new Vector3(trans.localPosition.x,
				trans.localPosition.y + fallSpeed * Time.deltaTime);
		}
		trans.localPosition = new Vector3(SPACE_SIZE * boardX, SPACE_SIZE * boardY);
		moving = false;
		PZPuzzleManager.instance.OnStopMoving(this);
	}
	
	void OnPress(bool pressed)
	{
		if (!pressed)
		{
			currDrag = Vector2.zero;
		}
		else if (PZPuzzleManager.instance.swapLock == 0)
		{
			dragged = false;
		}
	}
	
	void OnDrag(Vector2 delta)
	{
		if (!dragged && !moving && PZPuzzleManager.instance.swapLock == 0)
		{
			currDrag += delta;
			if (currDrag.x > DRAG_THRESHOLD && boardX < PZPuzzleManager.BOARD_WIDTH-1)
			{
				StartCoroutine(Shift(CBKValues.Direction.EAST));
			}
			else if (currDrag.x < -DRAG_THRESHOLD && boardX > 0)
			{
				StartCoroutine(Shift(CBKValues.Direction.WEST));
			}
			else if (currDrag.y > DRAG_THRESHOLD && boardY < PZPuzzleManager.BOARD_HEIGHT-1)
			{
				StartCoroutine(Shift(CBKValues.Direction.NORTH));
			}
			else if (currDrag.y < -DRAG_THRESHOLD && boardY > 0)
			{
				StartCoroutine(Shift(CBKValues.Direction.SOUTH));
			}
		}
	}
	
	IEnumerator Shift(CBKValues.Direction dir)
	{
		PZGem swapee;
		switch (dir) {
			case CBKValues.Direction.NORTH:
				swapee = PZPuzzleManager.instance.board[boardX, boardY+1];
				break;
			case CBKValues.Direction.SOUTH:
				swapee = PZPuzzleManager.instance.board[boardX, boardY-1];
				break;
			case CBKValues.Direction.EAST:
				swapee = PZPuzzleManager.instance.board[boardX+1, boardY];
				break;
			case CBKValues.Direction.WEST:
				swapee = PZPuzzleManager.instance.board[boardX-1, boardY];
				break;
			default:
				swapee = null;
				break;
		}
		if (!swapee.moving)
		{
			if (gemType == GemType.MOLOTOV)
			{
				PZPuzzleManager.instance.DetonateMolotovFromSwap(this, swapee.colorIndex);
			}
			else if (swapee.gemType == GemType.MOLOTOV)
			{
				PZPuzzleManager.instance.DetonateMolotovFromSwap(swapee, colorIndex);
			}
			else
			{
				swapee.StartCoroutine(swapee.Swap(CBKValues.opp[dir]));
				StartCoroutine(Swap(dir));
				//Debug.Log("Swapping: " + ToString() + " with " + swapee.ToString());
				dragged = true;
			}
		}
		
		PZPuzzleManager.instance.lastSwapSuccessful = true;
		PZPuzzleManager.instance.processingSwap = true;
		
		while(PZPuzzleManager.instance.processingSwap)
		{
			yield return null;
		}
		if (!PZPuzzleManager.instance.lastSwapSuccessful)
		{
			PZPuzzleManager.instance.swapLock += 1;
			yield return new WaitForSeconds(0.2f);
			PZPuzzleManager.instance.swapLock -= 1;
			swapee.StartCoroutine(swapee.Swap(dir));
			StartCoroutine(Swap(CBKValues.opp[dir]));
		}
	}
	
	IEnumerator Swap(CBKValues.Direction dir)
	{
		moving = true;
		PZPuzzleManager.instance.OnStartMoving(this);
		Vector3 startPos = trans.localPosition;
		Vector3 endPos = trans.localPosition + SPACE_SIZE * dirVals[dir];
		float currTime = 0;
		while (currTime < SWAP_TIME)
		{
			currTime += Time.deltaTime;
			trans.localPosition = Vector3.Lerp(startPos, endPos, currTime/SWAP_TIME);
			yield return null;
		}
		boardX += (int)Mathf.Round(dirVals[dir].x);
		boardY += (int)Mathf.Round(dirVals[dir].y);
		
		PZPuzzleManager.instance.board[boardX, boardY] = this;
		
		moving = false;
		PZPuzzleManager.instance.OnStopMoving(this);
	}
	
	public List<List<PZGem>> CheckMatches()
	{
		List<List<PZGem>> matchList = new List<List<PZGem>>();
		List<PZGem> matchUD = CheckUpDown();
		
		List<PZGem> matchLR = CheckLeftRight();
		
		if (matchLR != null)
		{
			matchList.Add(matchLR);
		}
		if (matchUD != null)
		{
			matchList.Add(matchUD);
		}
		
		return matchList;
	}
	
	List<PZGem> CheckUpDown()
	{
		List<PZGem> match = new List<PZGem>();
		match.Add(this);
		PZGem check;
		for (int i = boardY+1; i < PZPuzzleManager.BOARD_HEIGHT; i++) 
		{
			check = PZPuzzleManager.instance.board[boardX,i];
			if (check != null && !check.moving && check != this &&
				check.colorIndex == colorIndex)
			{
				match.Add(check);
			}
			else
			{
				break;
			}
		}
		for (int i = boardY-1; i >= 0; i--) 
		{
			check = PZPuzzleManager.instance.board[boardX,i];
			if (check != null && !check.moving && check != this &&
				check.colorIndex == colorIndex)
			{
				match.Add(check);
			}
			else
			{
				break;
			}
		}
		if (match.Count >= 3)
		{
			Debug.Log("Up-down match");
			foreach (PZGem item in match) 
			{
				Debug.Log(item.ToString());
			}
			return match;
		}
		return null;
	}
	
	List<PZGem> CheckLeftRight()
	{
		List<PZGem> match = new List<PZGem>();
		match.Add(this);
		PZGem check;
		for (int i = boardX+1; i < PZPuzzleManager.BOARD_WIDTH; i++) 
		{
			check = PZPuzzleManager.instance.board[i, boardY];
			if (check != null && !check.moving && check != this &&
				check.colorIndex == colorIndex)
			{
				match.Add(check);
			}
			else
			{
				break;
			}
		}
		for (int i = boardX-1; i >= 0; i--) 
		{
			check = PZPuzzleManager.instance.board[i, boardY];
			if (check != null && !check.moving && check != this &&
				check.colorIndex == colorIndex)
			{
				match.Add(check);
			}
			else
			{
				break;
			}
		}
		if (match.Count >= 3)
		{
			Debug.Log("Left-right match");
			foreach (PZGem item in match) {
				Debug.Log(item.ToString());
			}
			return match;
		}
		return null;
	}
		
	public void Pool()
	{
		CBKPoolManager.instance.Pool(this);
	}
	
	public override string ToString()
	{
		return "Gem: " + boardX + "-" + boardY;
	}
}
