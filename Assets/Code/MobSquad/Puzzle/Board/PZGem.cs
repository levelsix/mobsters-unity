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
public class PZGem : MonoBehaviour, MSPoolable {

	PZGem _prefab;
	public MSPoolable prefab {
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
					PZCombatManager.instance.battleStats.grenades++;
					sprite.spriteName = baseSprite + "grenade";
					break;
				case GemType.MOLOTOV:
					PZCombatManager.instance.battleStats.rainbows++;
					sprite.spriteName = "allcocktail";
					break;
				case GemType.ROCKET:
					PZCombatManager.instance.battleStats.rockets++;
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
	
	public int colorIndex = 0;

	public bool lockedBySpecial = false;

	[SerializeField]
	UISprite blocker;

	[SerializeField]
	Vector2 currDrag = Vector2.zero;
	
	[HideInInspector]
	public bool moving = false;
	
	public bool dragged = false;

	public int id = 0;
	static int nextId = 0;

	TweenScale scaleTween;
	TweenColor colorTween;
	int curTweenLoop = 0;
	const int TOTAL_HINT_TWEEN = 4;
	
	static readonly Dictionary<MSValues.Direction, Vector3> dirVals = new Dictionary<MSValues.Direction, Vector3>()
	{
		{MSValues.Direction.NORTH, new Vector3(0,1)},
		{MSValues.Direction.SOUTH, new Vector3(0,-1)},
		{MSValues.Direction.EAST, new Vector3(1,0)},
		{MSValues.Direction.WEST, new Vector3(-1,0)}
	};
	
	void Awake()
	{
		sprite = GetComponent<UISprite>();
		trans = transform;
		gameObj = gameObject;
		id = nextId++;

		scaleTween = GetComponent<TweenScale>();
		colorTween = GetComponent<TweenColor>();
	}
	
	public MSPoolable Make (Vector3 origin)
	{
		PZGem gem = Instantiate(this, origin, Quaternion.identity) as PZGem;
		gem.prefab = this;
		return gem;
	}

	void OnEnable()
	{
		MSActionManager.Puzzle.OnNewPlayerTurn += OnNewTurn;
	}

	void OnDisable()
	{
		MSActionManager.Puzzle.OnNewPlayerTurn -= OnNewTurn;
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
		PZPuzzleManager.instance.columnQueues[boardX].Add (this);
		enqueued = true;

		CheckFall();
	}

	void Init(int colr, int column)
	{
		colorIndex = colr;
		if (colr >= 0) //Chance that a saved game loads a rainbow
		{
			baseSprite = PZPuzzleManager.instance.gemTypes[colorIndex];
		}
		
		boardX = column;
		boardY = PZPuzzleManager.instance.boardHeight;

		trans.localScale = Vector3.one;
		gemType = GemType.NORMAL;
	}

	void CreateMatchParticle()
	{
		PZMatchParticle part = (MSPoolManager.instance.Get(MSPrefabList.instance.matchParticle[colorIndex].GetComponent<MSSimplePoolable>(), trans.localPosition)
		                        as MonoBehaviour).GetComponent<PZMatchParticle>();
		part.trans.parent = trans.parent;
		part.trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, -2);
		part.Init();
	}

	void CreateSparkle()
	{
		ParticleSystem sys = (MSPoolManager.instance.Get(MSPrefabList.instance.sparkleParticle, trans.position) as MonoBehaviour).particleSystem;
		Color newStartColor = MSValues.Colors.gemColors [colorIndex];
		newStartColor.a = sys.startColor.a;
		sys.startColor = newStartColor;
	}

	public void Destroy()
	{
		if (!lockedBySpecial) //Specials need to disable this lock before destroying the gem
		{
			if (colorIndex >= 0)
			{
				PZDamageNumber damNum = MSPoolManager.instance.Get(PZPuzzleManager.instance.damageNumberPrefab, transf.position) as PZDamageNumber;
				damNum.Init(this);
				PZPuzzleManager.instance.currGems[colorIndex]++;
			}

			//MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.gemPop);

			if (colorIndex >= 0)
			{
				CreateMatchParticle();
				CreateSparkle();
			}

			if (boardX < PZPuzzleManager.instance.boardWidth && boardY < PZPuzzleManager.instance.boardHeight)
			{
				PZPuzzleManager.instance.board[boardX, boardY] = null; //Remove from board
			}
			
			Detonate();

			for (int j = boardY; j < PZPuzzleManager.instance.boardHeight; j++) //Tell everything that was above this to fall
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

			SpawnAbove(PZPuzzleManager.instance.PickColor(boardX), boardX); //Respawn at top of board

		}
	}

	[ContextMenu("CheckFall")]
	public bool CheckFall()
	{
		if (lockedBySpecial || PZPuzzleManager.instance.specialBoardLock > 0)
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
				//Debug.LogWarning("Dropping from queue: " + id);
				PZPuzzleManager.instance.columnQueues[boardX].RemoveAt(0);
				enqueued = false;
			}

			//If this gem is above the board, it is new and doesn't need to be removed.
			//Otherwise, it's falling from within the board, so we need to mark its old
			//space as empty
			if (boardY < PZPuzzleManager.instance.boardHeight)
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
			if (!enqueued && boardY >= PZPuzzleManager.instance.boardHeight)
			{
				//Debug.LogWarning("Queuing: " + id);
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
		float fallSpeed = PZPuzzleManager.instance.BASE_FALL_SPEED;
		while(trans.localPosition.y > boardY * SPACE_SIZE)
		{
			yield return null;
			fallSpeed += PZPuzzleManager.instance.GRAVITY * Time.deltaTime;
			trans.localPosition = new Vector3(trans.localPosition.x,
				trans.localPosition.y + fallSpeed * Time.deltaTime, -1);
		}
		trans.localPosition = new Vector3(SPACE_SIZE * boardX, SPACE_SIZE * boardY + 1, -1);

		fallSpeed = PZPuzzleManager.instance.BASE_BOUNCE_SPEED;
		while(trans.localPosition.y > boardY * SPACE_SIZE)
		{
			yield return null;
			fallSpeed += PZPuzzleManager.instance.GRAVITY * Time.deltaTime;
			trans.localPosition = new Vector3(trans.localPosition.x,
			                                  trans.localPosition.y + fallSpeed * Time.deltaTime,
			                                  -1);
		}
		trans.localPosition = new Vector3(SPACE_SIZE * boardX, SPACE_SIZE * boardY + 1, -1);

		fallSpeed = PZPuzzleManager.instance.BASE_BOUNCE_SPEED * PZPuzzleManager.instance.SECOND_BOUNCE_MODIFIER;
		while(trans.localPosition.y > boardY * SPACE_SIZE)
		{
			yield return null;
			fallSpeed += PZPuzzleManager.instance.GRAVITY * Time.deltaTime;
			trans.localPosition = new Vector3(trans.localPosition.x,
			                                  trans.localPosition.y + fallSpeed * Time.deltaTime,
			                                  -1);
		}
		trans.localPosition = new Vector3(SPACE_SIZE * boardX, SPACE_SIZE * boardY, -1);

		moving = false;
		PZPuzzleManager.instance.OnStopMoving(this);
	}

	public void Detonate()
	{
		switch (gemType) {
		case GemType.ROCKET:
			Debug.LogWarning("Detonating Rocket: " + boardX + ", " + boardY);
			PZPuzzleManager.instance.DetonateRocket(this);
			break;
		case GemType.MOLOTOV:
			Debug.LogWarning("Detonating Molly: " + boardX + ", " + boardY);
			PZPuzzleManager.instance.DetonateMolotovFromSwap(this, this);
			break;
		case GemType.BOMB:
			Debug.LogWarning("Detonating Bomb: " + boardX + ", " + boardY);
			PZPuzzleManager.instance.GetBombMatch(this).Destroy();
			break;
		default:
			break;
		}
		gemType = GemType.NORMAL;
	}
	
	void OnPress(bool pressed)
	{
		if (!pressed)
		{
			currDrag = Vector2.zero;
		}
		else
		{
			if (MSActionManager.Puzzle.OnGemPressed != null)
			{
				MSActionManager.Puzzle.OnGemPressed();
			}

			if (PZPuzzleManager.instance.swapLock == 0)
			{
				dragged = false;
			}
		}
	}
	
	void OnDrag(Vector2 delta)
	{
		if (blocker.gameObject.activeSelf)
		{
			return;
		}

		if (!dragged && !moving && PZPuzzleManager.instance.swapLock == 0)
		{
			currDrag += delta;
			if (currDrag.x > DRAG_THRESHOLD && boardX < PZPuzzleManager.instance.boardWidth-1)
			{
				StartCoroutine(Shift(MSValues.Direction.EAST));
			}
			else if (currDrag.x < -DRAG_THRESHOLD && boardX > 0)
			{
				StartCoroutine(Shift(MSValues.Direction.WEST));
			}
			else if (currDrag.y > DRAG_THRESHOLD && boardY < PZPuzzleManager.instance.boardHeight-1)
			{
				StartCoroutine(Shift(MSValues.Direction.NORTH));
			}
			else if (currDrag.y < -DRAG_THRESHOLD && boardY > 0)
			{
				StartCoroutine(Shift(MSValues.Direction.SOUTH));
			}
		}
	}

	void OnNewTurn()
	{
		dragged = false;
	}
	
	IEnumerator Shift(MSValues.Direction dir)
	{
		PZGem swapee;
		switch (dir) {
			case MSValues.Direction.NORTH:
				swapee = PZPuzzleManager.instance.board[boardX, boardY+1];
				break;
			case MSValues.Direction.SOUTH:
				swapee = PZPuzzleManager.instance.board[boardX, boardY-1];
				break;
			case MSValues.Direction.EAST:
				swapee = PZPuzzleManager.instance.board[boardX+1, boardY];
				break;
			case MSValues.Direction.WEST:
				swapee = PZPuzzleManager.instance.board[boardX-1, boardY];
				break;
			default:
				swapee = null;
				break;
		}
		if (!swapee.moving && !swapee.blocker.gameObject.activeSelf)
		{
			if (gemType == GemType.MOLOTOV)
			{
				PZPuzzleManager.instance.DetonateMolotovFromSwap(this, swapee); //Takes care of all MOLOTOV-SPECIAL combos
				yield break;

			}
			else if (swapee.gemType == GemType.MOLOTOV)
			{
				PZPuzzleManager.instance.DetonateMolotovFromSwap(swapee, this);
				yield break;
			}
			else if (gemType != GemType.NORMAL && swapee.gemType != GemType.NORMAL) //Only cases left are BOMB-BOMB, ROCKET-ROCKET, and BOMB-ROCKET
			{
				if (gemType == swapee.gemType)
				{
					if (gemType == GemType.BOMB) //BOMB-BOMB
					{
						PZPuzzleManager.instance.DetonateBombFromSwap(this, swapee);
						yield break;
					}
					else //ROCKET-ROCKET
					{
						horizontal = true;
						PZPuzzleManager.instance.DetonateRocket(this);
						swapee.horizontal = false;
						PZPuzzleManager.instance.DetonateRocket(swapee);
						yield break;
					}
				}
				else //BOMB-ROCKET
				{
					PZPuzzleManager.instance.DetonateBombFromSwap(this, swapee);
					yield break;
				}
			}
			else
			{
				swapee.StartCoroutine(swapee.Swap(MSValues.opp[dir]));
				StartCoroutine(Swap(dir));
				//Debug.Log("Swapping: " + ToString() + " with " + swapee.ToString());
				dragged = true;
				swapee.dragged = true;
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
			StartCoroutine(Swap(MSValues.opp[dir]));

			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.wrongMove);
		}
		else
		{
			if (MSActionManager.Puzzle.OnGemSwapSuccess != null)
			{
				MSActionManager.Puzzle.OnGemSwapSuccess();
			}
		}
	}
	
	IEnumerator Swap(MSValues.Direction dir)
	{
		moving = true;
		PZPuzzleManager.instance.OnStartMoving(this);
		Vector3 startPos = trans.localPosition;
		Vector3 endPos = trans.localPosition + SPACE_SIZE * dirVals[dir];
		float currTime = 0;
		while (currTime < PZPuzzleManager.instance.SWAP_TIME)
		{
			currTime += Time.deltaTime;
			trans.localPosition = Vector3.Lerp(startPos, endPos, currTime/PZPuzzleManager.instance.SWAP_TIME);
			yield return null;
		}
		boardX += (int)Mathf.Round(dirVals[dir].x);
		boardY += (int)Mathf.Round(dirVals[dir].y);
		
		PZPuzzleManager.instance.board[boardX, boardY] = this;
		
		moving = false;
		PZPuzzleManager.instance.OnStopMoving(this);
	}

	public void Block()
	{
		blocker.gameObject.SetActive(true);
		MSActionManager.Puzzle.OnGemMatch += Unblock;
	}

	void Unblock()
	{
		blocker.gameObject.SetActive(false);
		MSActionManager.Puzzle.OnGemMatch -= Unblock;
	}
		
	public void Pool()
	{
		MSPoolManager.instance.Pool(this);
		if (blocker.gameObject.activeSelf)
		{
			Unblock ();
		}
	}
	
	public override string ToString()
	{
		return "Gem " + id + ": " + boardX + "-" + boardY;
	}

	/// <summary>
	/// This function calls itself at the end of the animation
	/// </summary>
	public void HintAnimation()
	{
		if(curTweenLoop < TOTAL_HINT_TWEEN)
		{
			scaleTween.ResetToBeginning();
			scaleTween.PlayForward();
//			colorTween.ResetToBeginning();
//			colorTween.PlayForward();

			curTweenLoop++;
		}
		else
		{
			curTweenLoop = 0;
		}
	}

	public void CancelHintAnimation()
	{
		scaleTween.enabled = false;
		colorTween.enabled = false;
		scaleTween.Sample(0f, false);
		colorTween.Sample(0f, false);
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
