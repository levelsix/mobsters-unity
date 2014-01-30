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
public class PZGem : MonoBehaviour, CBKPoolable {

	PZGem _prefab;
	public CBKPoolable prefab {
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

	string baseSprite = "";
	
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
			switch(value)
			{
				case GemType.BOMB:
					sprite.spriteName = baseSprite + "grenade";
					break;
				case GemType.MOLOTOV:
					sprite.spriteName = "allcocktail";
					break;
				case GemType.ROCKET:
					if (horizontal)
					{
						sprite.spriteName = baseSprite + "sideways";
					}
					else
					{
						sprite.spriteName = baseSprite + "updown";
					}
					break;
				case GemType.NORMAL:
					sprite.spriteName = baseSprite + "orb";
					break;
			}

			if (sprite.GetAtlasSprite() != null)
			{
				sprite.width = sprite.GetAtlasSprite().width;
				sprite.height = sprite.GetAtlasSprite().height;
			}

			_gemType = value;
		}
	}
	
	public bool horizontal = true;

	public bool enqueued = false;
	
	public int boardX, boardY;
	
	const float DRAG_THRESHOLD = 70;
	
	const float SPACE_SIZE = 72;
	
	const float BASE_FALL_SPEED = -250;

	const float BASE_BOUNCE_SPEED = 80;

	const float SECOND_BOUNCE_MODIFIER = .25f;
	
	const float GRAVITY = -600f;
	
	const float SWAP_TIME = .2f;
	
	public int colorIndex = 0;

	public bool lockedBySpecial = false;
	
	[SerializeField]
	Vector2 currDrag = Vector2.zero;
	
	[HideInInspector]
	public bool moving = false;
	
	bool dragged = false;

	public int id = 0;
	static int nextId = 0;
	
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
		id = nextId++;
	}
	
	public CBKPoolable Make (Vector3 origin)
	{
		PZGem gem = Instantiate(this, origin, Quaternion.identity) as PZGem;
		gem.prefab = this;
		return gem;
	}

	public void SpawnOnMap(int colr, int column)
	{
		Init (colr, column);

		while (boardY > 0)
		{
			if (PZPuzzleManager.instance.board[boardX, boardY-1] != null)
			{
				break;
			}
			boardY--;
		}

		trans.localPosition = new Vector3(boardX * SPACE_SIZE, boardY * SPACE_SIZE, -1) ;

		PZPuzzleManager.instance.board[boardX, boardY] = this;
	}

	public void SpawnAbove(int colr, int column)
	{
		Init (colr, column);

		PZPuzzleManager.instance.OnStartMoving(this);

		trans.localPosition = new Vector3(boardX * SPACE_SIZE, Mathf.Max(boardY * SPACE_SIZE, PZPuzzleManager.instance.HighestGemInColumn(boardX) + SPACE_SIZE), -1);

		CheckFall();
	}

	void Init(int colr, int column)
	{
		colorIndex = colr;
		baseSprite = PZPuzzleManager.instance.gemTypes[colorIndex];
		
		boardX = column;
		boardY = PZPuzzleManager.BOARD_HEIGHT;

		trans.localScale = Vector3.one;
		gemType = GemType.NORMAL;
	}

	void CreateMatchParticle()
	{
		PZMatchParticle part = (CBKPoolManager.instance.Get(CBKPrefabList.instance.matchParticle[colorIndex].GetComponent<CBKSimplePoolable>(), trans.localPosition)
		                        as MonoBehaviour).GetComponent<PZMatchParticle>();
		part.trans.parent = trans.parent;
		part.trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, -2);
		part.Init();
	}

	void CreateSparkle()
	{
		ParticleSystem sys = (CBKPoolManager.instance.Get(CBKPrefabList.instance.sparkleParticle, trans.position) as MonoBehaviour).particleSystem;
		sys.startColor = CBKValues.Colors.gemColors[colorIndex];
	}

	public void Destroy()
	{
		if (!lockedBySpecial)
		{
			Debug.LogWarning("Destroying: " + id);

			CBKSoundManager.instance.PlayOneShot(CBKSoundManager.instance.gemPop);

			if (colorIndex >= 0)
			{
				CreateMatchParticle();
				CreateSparkle();
			}

			PZPuzzleManager.instance.board[boardX, boardY] = null; //Remove from board
			for (int j = boardY; j < PZPuzzleManager.BOARD_HEIGHT; j++) //Tell everything that was above this to fall
			{
				if (PZPuzzleManager.instance.board[boardX, j] != null)
				{
					PZPuzzleManager.instance.board[boardX, j].CheckFall();
				}
			}
			while(PZPuzzleManager.instance.columnQueues[boardX].Count > 0)
			{
				if (!PZPuzzleManager.instance.columnQueues[boardX][0].CheckFall())
				{
					break;
				}
			}
			SpawnAbove(PZPuzzleManager.instance.PickColor(), boardX); //Respawn at top of board
		}
	}
	
	public bool CheckFall()
	{
		if (lockedBySpecial)
		{
			return false;
		}

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
			if (enqueued)
			{
				Debug.LogWarning("Dropping from queue: " + id);
				PZPuzzleManager.instance.columnQueues[boardX].RemoveAt(0);
				enqueued = false;
			}

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
			return true;
		}
		else
		{
			if (!enqueued && boardY >= PZPuzzleManager.BOARD_HEIGHT)
			{
				Debug.LogWarning("Queuing: " + id);
				PZPuzzleManager.instance.columnQueues[boardX].Add(this);
				enqueued = true;
			}
			return false;
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
				trans.localPosition.y + fallSpeed * Time.deltaTime, -1);
		}
		trans.localPosition = new Vector3(SPACE_SIZE * boardX, SPACE_SIZE * boardY + 1, -1);

		fallSpeed = BASE_BOUNCE_SPEED;
		while(trans.localPosition.y > boardY * SPACE_SIZE)
		{
			yield return null;
			fallSpeed += GRAVITY * Time.deltaTime;
			trans.localPosition = new Vector3(trans.localPosition.x,
			                                  trans.localPosition.y + fallSpeed * Time.deltaTime,
			                                  -1);
		}
		trans.localPosition = new Vector3(SPACE_SIZE * boardX, SPACE_SIZE * boardY + 1, -1);

		fallSpeed = BASE_BOUNCE_SPEED * SECOND_BOUNCE_MODIFIER;
		while(trans.localPosition.y > boardY * SPACE_SIZE)
		{
			yield return null;
			fallSpeed += GRAVITY * Time.deltaTime;
			trans.localPosition = new Vector3(trans.localPosition.x,
			                                  trans.localPosition.y + fallSpeed * Time.deltaTime,
			                                  -1);
		}
		trans.localPosition = new Vector3(SPACE_SIZE * boardX, SPACE_SIZE * boardY, -1);

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
			//Debug.Log("Lock");
			PZPuzzleManager.instance.swapLock += 1;
			yield return new WaitForSeconds(0.2f);
			
			//Debug.Log("Unlock");
			PZPuzzleManager.instance.swapLock -= 1;
			swapee.StartCoroutine(swapee.Swap(dir));
			StartCoroutine(Swap(CBKValues.opp[dir]));

			CBKSoundManager.instance.PlayOneShot(CBKSoundManager.instance.wrongMove);
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
		
	public void Pool()
	{
		CBKPoolManager.instance.Pool(this);
	}
	
	public override string ToString()
	{
		return "Gem " + id + ": " + boardX + "-" + boardY;
	}

	#region Debug

#if UNITY_EDITOR
	[ContextMenu ("Make Rocket")]
	void MakeRocket()
	{
		gemType = GemType.ROCKET;
	}

	[ContextMenu ("Make Grenade")]
	void MakeGrenade()
	{
		gemType = GemType.BOMB;
	}

	[ContextMenu ("Make Molotov")]
	void MakeMolotov()
	{
		gemType = GemType.MOLOTOV;
	}
#endif

	#endregion
}
