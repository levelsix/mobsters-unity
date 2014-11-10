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

	PZMoveTowards moveTowards;

	string baseSprite = "";
	
	/// <summary>
	/// The sprite, which we use to change tint and image
	/// when necessary
	/// </summary>
	public UISprite sprite;

	
	public enum GemType {
		NORMAL,
		ROCKET, //This is the mainly used version of Rockets
		HORIZONTAL_ROCKET, //This and VERTICAL_ROCKET exist pretty much entirely for saving
		VERTICAL_ROCKET, //Gems get converted in/out of these versions during saving/loading
		GRENADE, 
		MOLOTOV,
		CAKE,
		BOMB, //Skill bombs, NOT to be confused with grenades!
		POISON
	};
	
	private GemType _gemType;
	
	public GemType gemType
	{
		get
		{
			return _gemType;
		}
		set
		{
			bombTimerLabel.gameObject.SetActive(false);
			switch(value)
			{
				case GemType.GRENADE:
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
				case GemType.CAKE:
					Debug.Log("Baking cake");
					sprite.spriteName = "cakeorb";
					break;
				case GemType.HORIZONTAL_ROCKET:
					horizontal = true;
					gemType = GemType.ROCKET;
					return;
				case GemType.VERTICAL_ROCKET:
					horizontal = false;
					gemType = GemType.ROCKET;
					return;
				case GemType.BOMB:
					sprite.spriteName = baseSprite + "bomb";
					bombTimerLabel.gameObject.SetActive(true);
					break;
				case GemType.POISON:
					Debug.Log("Made Poison!");
					sprite.spriteName = baseSprite + "poison";
					break;
			}

			sprite.MakePixelPerfect();

			_gemType = value;
		}
	}

	/// <summary>
	/// indicates if this <see cref="PZGem"/> cause a special effect when swapped with a moltov.
	/// </summary>
	/// <value><c>true</c> if swapping with a moltov results in a special effect; otherwise, <c>false</c>.</value>
	public bool canComboWithMoltov
	{
		get
		{
			return _gemType == GemType.GRENADE || _gemType == GemType.MOLOTOV || _gemType == GemType.ROCKET;
		}
	}
	
	public bool horizontal = true;

	public bool enqueued = false;
	
	public int boardX, boardY;

	Vector3 boardPos
	{
		get
		{
			return new Vector3(boardX, boardY) * SPACE_SIZE;
		}
	}

	int prefallBoardX, prefallBoardY;

	int shuffleX, shuffleY;

	Vector3 shufflePos
	{
		get
		{
			return new Vector3(shuffleX, shuffleY) * SPACE_SIZE;
		}
	}
	
	const float DRAG_THRESHOLD = 70;
	
	public const float SPACE_SIZE = 72;
	
	public int colorIndex = 0;

	public bool lockedBySpecial = false;

	[SerializeField]
	UISprite blocker;

	[SerializeField]
	Vector2 currDrag = Vector2.zero;

	public bool moving = false;
	
	public bool dragged = false;

	public int id = 0;
	static int nextId = 0;

	[SerializeField] TweenScale specialScaleTween;
	[SerializeField] TweenScale hintScaleTween;
	[SerializeField] TweenScale tickScaleTween;

	TweenColor colorTween;
	int curTweenLoop = 0;
	const int TOTAL_HINT_TWEEN = 4;

	bool hijackCake = false;

	#region Bombs
	[SerializeField] UILabel bombTimerLabel;
	[SerializeField] TweenColor bombTimerLabelColorTween;
	[SerializeField] float bombTimerLabelColorTweenBaseTime = 1f;
	public int bombTicks;
	public int bombDamage;
	#endregion
	
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
		moveTowards = GetComponent<PZMoveTowards>();
		id = nextId++;

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
		MSActionManager.Puzzle.OnDragFinished += OnNewTurn;
	}

	void OnDisable()
	{
		MSActionManager.Puzzle.OnDragFinished -= OnNewTurn;
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

		try
		{
			PZPuzzleManager.instance.board[boardX, boardY] = this;
		}catch (System.IndexOutOfRangeException e)
		{
			Debug.Log("RAWB: " + e.ToString() + "\n" + boardX + ", " + boardY);
		}
	}

	public void SpawnAbove(int colr, int column)
	{
		Init (colr, column);
		if (PZPuzzleManager.instance.maxCakes > PZPuzzleManager.instance.cakes.Count && UnityEngine.Random.value < PZPuzzleManager.instance.currCakeChance)
		{
			gemType = GemType.CAKE;
			PZPuzzleManager.instance.cakes.Add(this);
			colorIndex = -1;
		}

		PZPuzzleManager.instance.OnStartMoving(this);

		trans.localPosition = new Vector3(boardX * SPACE_SIZE, Mathf.Max(boardY * SPACE_SIZE, PZPuzzleManager.instance.HighestGemInColumn(boardX) + SPACE_SIZE), -1);
		PZPuzzleManager.instance.columnQueues[boardX].Add (this);
		enqueued = true;

		CheckFall();
	}

	void Init(int colr, int column)
	{
		colorIndex = colr;
		SetBaseSprite ();

		boardX = column;
		boardY = PZPuzzleManager.instance.boardHeight;

		trans.localScale = Vector3.one;

		gemType = GemType.NORMAL;
	}

	void SetBaseSprite()
	{
		if (colorIndex >= 0)
		{
			baseSprite = PZPuzzleManager.instance.gemTypes[Mathf.Abs(colorIndex)]; //Need to make sure that orbs that are cakes can turn back into their colors
		}
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
	
	void RemoveAndReset (bool detonate = true)
	{
		if (gemType == GemType.BOMB)
		{
			BombDisposal();
		}

		if (boardX < PZPuzzleManager.instance.boardWidth && boardY < PZPuzzleManager.instance.boardHeight) {
			PZPuzzleManager.instance.board [boardX, boardY] = null;
			//Remove from board
		}
		if(detonate)
		{
			Detonate ();
		}
		for (int j = boardY; j < PZPuzzleManager.instance.boardHeight; j++)//Tell everything that was above this to fall
		{
			if (PZPuzzleManager.instance.board [boardX, j] != null) {
				PZPuzzleManager.instance.board [boardX, j].CheckFall ();
			}
		}
		while (PZPuzzleManager.instance.columnQueues [boardX].Count > 0) {
			if (!PZPuzzleManager.instance.columnQueues [boardX] [0].CheckFall ()) {
				break;
			}
		}
		SpawnAbove (PZPuzzleManager.instance.PickColor (boardX), boardX);

		if (PZPuzzleManager.instance.CheckSpawnBomb(this))
		{
			SpawnBomb();
		}
		else if (colorIndex >= 0 && PZPuzzleManager.instance.poisonColor == colorIndex)
		{
			gemType = GemType.POISON;
		}
	}

	/// <summary>
	/// Remove gem from board
	/// </summary>
	/// <param name="detonate">If set to <c>true</c> detonate will also be called, which sets off special gem powers.</param>
	public void Destroy(bool detonate = true)
	{
		if (gemType == GemType.CAKE)
		{
			CheckFall();
		}
		else if (!lockedBySpecial) //Specials need to disable this lock before destroying the gem
		{
			if (colorIndex >= 0)
			{
				if (!PZPuzzleManager.instance.ClearJelly(prefallBoardX, prefallBoardY, id)) //If there's a jelly, prevent the gem from being worth a shit and damage it instead
				{
					PZDamageNumber damNum = MSPoolManager.instance.Get(PZPuzzleManager.instance.damageNumberPrefab, transf.position) as PZDamageNumber;
					damNum.Init(this);
					PZPuzzleManager.instance.currGems[colorIndex]++;
					CreateMatchParticle();
				}
				
				if (colorIndex+1 == (int)PZCombatManager.instance.activePlayer.monster.monster.monsterElement)
				{
					PZCombatManager.instance.AddPlayerSkillPoint();
				}

				if (colorIndex+1 == (int)PZCombatManager.instance.activeEnemy.monster.monster.monsterElement)
				{
					PZCombatManager.instance.AddEnemySkillPoint();
				}
			}

			//MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.gemPop);

			if (colorIndex >= 0)
			{
				CreateSparkle();
			}

			RemoveAndReset (detonate);
		}
	}

	public void SetPrefallPosition()
	{
		prefallBoardX = boardX;
		prefallBoardY = boardY;
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
			fallSpeed += PZPuzzleManager.instance.GRAVITY * Time.deltaTime;
			Vector3 newPosition = new Vector3(trans.localPosition.x,
			                                  trans.localPosition.y + fallSpeed * Time.deltaTime,
			                                  -1);

			if(newPosition.y < boardY * SPACE_SIZE)
			{
				if(Mathf.Abs(fallSpeed) < PZPuzzleManager.instance.BOUNCE_THRESHHOLD)
				{
					break;
				}
				//to avoid sticky feeling bounces make up lost distance this frame
				float overShot = (boardY * SPACE_SIZE - newPosition.y) / Mathf.Abs(fallSpeed);
				newPosition.y = boardY * SPACE_SIZE;
				fallSpeed *= -1;
				fallSpeed *= PZPuzzleManager.instance.BOUNCE_REDUCTION;
				newPosition.y += overShot * fallSpeed;
			}

			trans.localPosition = newPosition;
			//The placement of this yield is actually very important for the animation
			yield return null;
		}

		trans.localPosition = new Vector3(SPACE_SIZE * boardX, SPACE_SIZE * boardY + 1, -1);
		moving = false;
		PZPuzzleManager.instance.OnStopMoving(this);
		if (gemType == GemType.CAKE) CheckCake();
	}


	public void Detonate()
	{
		switch (gemType) {
		case GemType.ROCKET:
			Debug.LogWarning("Detonating Rocket: " + boardX + ", " + boardY);
			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.rocket);
			PZPuzzleManager.instance.DetonateRocket(this);
			break;
		case GemType.MOLOTOV:
			Debug.LogWarning("Detonating Molly: " + boardX + ", " + boardY);
			PZPuzzleManager.instance.DetonateMolotovFromSwap(this, this);
			break;
		case GemType.GRENADE:
			Debug.LogWarning("Detonating Bomb: " + boardX + ", " + boardY);
			PZPuzzleManager.instance.GetGrenadeMatch(this).Destroy();
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
			else if (gemType != GemType.NORMAL && swapee.gemType != GemType.NORMAL)
			{
				if (gemType == swapee.gemType)
				{
					if (gemType == GemType.GRENADE) //BOMB-BOMB
					{
						PZPuzzleManager.instance.DetonateGrenadeFromSwap(this, swapee);
						yield break;
					}
					else //ROCKET-ROCKET
					{
						PZPuzzleManager.instance.DetonateDoubleRocket(this, swapee);
						yield break;
					}
				}
				else //BOMB-ROCKET or ROCKET-BOMB
				{
					PZPuzzleManager.instance.DetonateGrenadeFromSwap(this, swapee);
					yield break;
				}
			}
			else
			{
				MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.gemSwap);
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

	public void Block(float tweenTime = 0)
	{
		blocker.gameObject.SetActive(true);
		blocker.alpha = 0;
		TweenAlpha.Begin(blocker.gameObject, tweenTime, .3f);
		MSActionManager.Puzzle.OnGemMatch += Unblock;
	}

	public void Unblock()
	{
		Unblock(0);
	}

	public void Unblock(float tweenTime)
	{
		StartCoroutine(TweenUnblock(tweenTime));
	}

	public IEnumerator TweenUnblock(float tweenTime = 0)
	{
		MSActionManager.Puzzle.OnGemMatch -= Unblock;
		TweenAlpha alph = TweenAlpha.Begin(blocker.gameObject, tweenTime, 0);
		if (tweenTime > 0)
		{
			while (alph.tweenFactor < 1) yield return null;
		}
		blocker.gameObject.SetActive(false);
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
			hintScaleTween.ResetToBeginning();
			hintScaleTween.PlayForward();
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
		curTweenLoop = 0;

		hintScaleTween.enabled = false;
		colorTween.enabled = false;
		hintScaleTween.Sample(0f, false);
		colorTween.Sample(0f, false);
	}

	public void SetShufflePosition(int x, int y)
	{
		shuffleX = x;
		shuffleY = y;
	}

	public void SetShuffleProgress(float t)
	{
		transf.localPosition = Vector3.Lerp(boardPos, shufflePos, t);

		if (t >= 1)
		{
			boardX = shuffleX;
			boardY = shuffleY;
		}
	}

	#region Cake

	[ContextMenu ("Bake Cake")]
	public void DoBecomeCake()
	{
		StartCoroutine(BecomeCake());
	}

	public Coroutine RunBecomeCake()
	{
		return StartCoroutine(BecomeCake());
	}

	IEnumerator BecomeCake()
	{
		colorIndex = -1; //To keep the number around, but make sure that it doesn't get made a part of matches
		specialScaleTween.to = Vector3.zero;
		specialScaleTween.PlayForward();
		while (specialScaleTween.tweenFactor < 1)
		{
			yield return null;
		}
		gemType = GemType.CAKE;
		specialScaleTween.PlayReverse();
		while (specialScaleTween.tweenFactor > 0)
		{
			yield return null;
		}
	}

	[ContextMenu ("Revert from Cake")]
	public Coroutine RunRevertFromCake()
	{
		return StartCoroutine(RevertFromCake());
	}

	IEnumerator RevertFromCake()
	{
		specialScaleTween.to = Vector3.zero;
		specialScaleTween.PlayForward();
		while (specialScaleTween.tweenFactor < 1)
		{
			yield return null;
		}
		colorIndex = PZPuzzleManager.instance.PickRevertCakeColor(boardX, boardY);
		SetBaseSprite();
		gemType = GemType.NORMAL;
		specialScaleTween.PlayReverse();
		while (specialScaleTween.tweenFactor > 0)
		{
			yield return null;
		}
	}

	/// <summary>
	/// Checks whether the cake has hit the bottom of the board
	/// Precondition: Gem has finished falling
	/// </summary>
	public bool CheckCake()
	{
		if (dragged) return false;
		for (int i = boardY-1; i >= 0; i--) 
		{
			if (PZPuzzleManager.instance.board[boardX, i].gemType != GemType.CAKE)
			{
				return false;
			}
		}
		return true;
	}

	public bool isCaking = false;

	public Coroutine RunEatCake()
	{
		return StartCoroutine(EatCake());
	}

	IEnumerator EatCake()
	{
		PZPuzzleManager.instance.swapLock++;
		Debug.LogWarning("Cake lock");
		isCaking = true;
		yield return moveTowards.RunMoveTowards(transform.parent.InverseTransformPoint(PZCombatManager.instance.activeEnemy.transform.position), Vector3.left);
		//Enemy Eat Animation
		PZCombatManager.instance.activeEnemy.unit.animat = MSUnit.AnimationType.ATTACK;
		hijackCake = true;
		PZCombatManager.instance.hijackPlayerFlinch += HijackCake;
		PZPuzzleManager.instance.cakes.Remove(this);
		RemoveAndReset();
		while (hijackCake)
		{
			yield return null;
		}
		PZPuzzleManager.instance.swapLock--;
		Debug.LogWarning("Cake unlock");
		isCaking = false;
	}

	void HijackCake()
	{
		hijackCake = false;
	}


	#endregion

	#region Bombs

	/// <summary>
	/// Spawns a bomb.
	/// Used when a new gem is spawned as a bomb.
	/// Skips the scale tweening.
	/// </summary>
	public void SpawnBomb()
	{
		BombSetup(PZPuzzleManager.instance.bombTicks, PZPuzzleManager.instance.bombDamage);
		PZCombatManager.instance.bombs.Add (this);
	}

	/// <summary>
	/// Makes a bomb from an orb already on the board.
	/// </summary>
	public Coroutine MakeBomb()
	{
		return StartCoroutine(BecomeBomb());
	}

	IEnumerator BecomeBomb()
	{
		specialScaleTween.to = Vector3.zero;
		specialScaleTween.PlayForward();
		while (specialScaleTween.tweenFactor < 1)
		{
			yield return null;
		}
		BombSetup(PZPuzzleManager.instance.bombTicks, PZPuzzleManager.instance.bombDamage);
		specialScaleTween.PlayReverse();
		while (specialScaleTween.tweenFactor > 0)
		{
			yield return null;
		}
	}

	/// <summary>
	/// Sets up the bomb.
	/// Sets the right sprite, enables and starts the timer.
	/// </summary>
	/// <param name="ticks">Ticks.</param>
	void BombSetup(int ticks, int damage)
	{
		gemType = GemType.BOMB;
		bombTimerLabel.text = ticks.ToString();
		bombTicks = ticks;
		bombDamage = damage;
		bombTimerLabelColorTween.Sample(0, true);
	}
	
	void BombDisposal()
	{
		PZCombatManager.instance.bombs.Remove(this);
	}

	/// <summary>
	/// Called OnTurnChange.
	/// </summary>
	public bool BombTick()
	{
		bombTicks--;
		StartCoroutine(BombTickAnimation());
		if (bombTicks <= 0)
		{
			return true;
		}
		return false;
	}

	IEnumerator BombTickAnimation()
	{
		specialScaleTween.to = new Vector3(.7f, .7f, .7f);
		specialScaleTween.PlayForward();
		while (specialScaleTween.tweenFactor < 1)
		{
			yield return null;
		}
		BombLabelRefresh();
		specialScaleTween.PlayReverse();
		while (specialScaleTween.tweenFactor > 0)
		{
			yield return null;
		}

	}

	/// <summary>
	/// Refreshes the bomb label.
	/// Called when we're loading from a save, to make sure that
	/// the label doesn't display a 0 for a turn
	/// </summary>
	public void BombLabelRefresh()
	{
		bombTimerLabel.text = bombTicks.ToString();
		if (bombTicks <= 2)
		{
			bombTimerLabelColorTween.Play();
			bombTimerLabelColorTween.duration = bombTimerLabelColorTweenBaseTime;
			if (bombTicks == 1) bombTimerLabelColorTween.duration /= 2;
			else if (bombTicks == 0) bombTimerLabelColorTween.duration /= 4;
		}
		else
		{
			bombTimerLabelColorTween.Sample(0, true);
		}
	}

	public Coroutine BombDetonate()
	{
		return StartCoroutine(BombDetonationAnimation());
	}

	IEnumerator BombDetonationAnimation()
	{
		//TODO: Particle animation
		yield return new WaitForSeconds(.5f);
		specialScaleTween.to = Vector3.zero;
		specialScaleTween.PlayForward();
		while (specialScaleTween.tweenFactor < 1)
		{
			yield return null;
		}
		BombDisposal();
		gemType = GemType.NORMAL;
		specialScaleTween.PlayReverse();
		while (specialScaleTween.tweenFactor > 0)
		{
			yield return null;
		}
	}

	#endregion

	#region Poison

	public Coroutine MakePoison()
	{
		return StartCoroutine(BecomePoison());
	}

	IEnumerator BecomePoison()
	{
		specialScaleTween.to = Vector3.zero;
		specialScaleTween.PlayForward();
		while (specialScaleTween.tweenFactor < 1)
		{
			yield return null;
		}
		gemType = GemType.POISON;
		specialScaleTween.PlayReverse();
		while (specialScaleTween.tweenFactor > 0)
		{
			yield return null;
		}
	}

	public Coroutine RevertFromPoison()
	{
		return StartCoroutine(RevertToNormal());
	}

	IEnumerator RevertToNormal()
	{
		specialScaleTween.to = Vector3.zero;
		specialScaleTween.PlayForward();
		while (specialScaleTween.tweenFactor < 1)
		{
			yield return null;
		}
		gemType = GemType.NORMAL;
		specialScaleTween.PlayReverse();
		while (specialScaleTween.tweenFactor > 0)
		{
			yield return null;
		}
	}

	#endregion

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
		gemType = GemType.GRENADE;
	}

	[ContextMenu ("Make Molotov")]
	void MakeMolotov()
	{
		gemType = GemType.MOLOTOV;
	}
#endif

	#endregion
}
