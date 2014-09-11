using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using com.lvl6.proto;

public class PZPuzzleManager : MonoBehaviour {
	
	public static PZPuzzleManager instance;

	[SerializeField]
	public Camera puzzleCamera;

	public UISprite boardSprite;

	[SerializeField]
	UILabel comboLabel;

	[SerializeField]
	int _specialBoardLock = 0;

	public int specialBoardLock
	{
		get{
			return _specialBoardLock;
		}

		set{
			_specialBoardLock = value;
			if(value == 0){
				ReCheckBoard();
			}
		}
	}

	[SerializeField]
	int _swapLock = 0;

	public int swapLock
	{
		get
		{
			return _swapLock;
		}
		set
		{
			_swapLock = value;
			//Debug.Log("Swaplock: " + _swapLock);
		}
	}

	public bool processingSwap = false;
	
	public bool lastSwapSuccessful = true;
	
	int _combo = 0;
	
	int combo
	{
		get
		{
			return _combo;
		}
		set
		{
			_combo = value;
			if (_combo > 0)
			{
				comboLabel.text = _combo.ToString();

				MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.combos[Mathf.Min(_combo,MSSoundManager.instance.combos.Length)-1]);
			}
			else
			{
				comboLabel.text = " ";
			}
		}
	}

	public Transform puzzleParent;
	
	List<PZGem> movingGems = new List<PZGem>();
	List<PZGem> gemsToCheck = new List<PZGem>();
	PZGem[] hintgems = new PZGem[3];
	
	[SerializeField]
	PZGem gemPrefab;
	
	public PZGem[,] board;

	public PZJelly[,] jellyBoard;
	
	[SerializeField]
	public string[] gemTypes;
	
	public int[] currGems;

	//public int[] gemsOnBoardByType = new int[GEM_TYPES];

	public List<PZGem>[] columnQueues = new List<PZGem>[STANDARD_BOARD_HEIGHT];

	/// <summary>
	/// This list contains all the gems that are above the board but have not yet fallen in
	/// </summary>
	//public List<PZGem> prewarmedGems = new List<PZGem>();

	public Stack<int>[] riggedBoardStacks = new Stack<int>[STANDARD_BOARD_WIDTH];
	
	public int gemsByType;

	public const int STANDARD_BOARD_WIDTH = 8;
	public const int STANDARD_BOARD_HEIGHT = 8;

	public const int SPACE_SIZE = 71;
	
	public int boardWidth = 8;
	public int boardHeight = 8;
	
	public const int GEM_TYPES = 6;
	
	public const float WAIT_BETWEEN_LINES = .3f;
	
	bool setUpBoard = false;

	[SerializeField]
	public PZDamageNumber damageNumberPrefab;

	[SerializeField]
	PZRocket rocketPrefab;

	[SerializeField]
	PZMolotovPart molotovPartPrefab;

	[SerializeField]
	PZJelly jellyPrefab;

	public float BASE_FALL_SPEED = -250;
	
	public float BASE_BOUNCE_SPEED = 80;
	
	public float SECOND_BOUNCE_MODIFIER = .25f;
	
	public float GRAVITY = -600f;
	
	public float SWAP_TIME = .2f;

	bool _gemHints = false;

	/// <summary>
	/// gem hints are when the gems animating to show a match.  Set to true to start.
	/// </summary>
	bool showGemHints
	{
		set
		{
			if(value && !_gemHints)
			{
				StartCoroutine("HintCycle");//has to be started with a string so I can stop it later.
			}
			else if (!value)
			{
				foreach(PZGem gem in hintgems)
				{
					if(gem!=null)
					{
						gem.CancelHintAnimation();
						StopCoroutine("HintCycle");
					}
				}
			}
			_gemHints = value;
		}

		get
		{
			return _gemHints;
		}
	}

	public void Awake()
	{
		instance = this;
		
		board = new PZGem[boardWidth, boardHeight];
		jellyBoard = new PZJelly[boardWidth, boardHeight];
		
		currGems = new int[GEM_TYPES];

		//gemsOnBoardByType = new int[GEM_TYPES];
		
		ResetCombo();
		
		setUpBoard = false;
		
		swapLock = 0;

		for (int i = 0; i < columnQueues.Length; i++) 
		{
			columnQueues[i] = new List<PZGem>();
		}
		for (int i = 0; i < riggedBoardStacks.Length; i++) 
		{
			riggedBoardStacks[i] = new Stack<int>();
		}

		MSActionManager.Puzzle.OnGemMatch += delegate { showGemHints = false;};
	}

	public void Start()
	{
		MSPoolManager.instance.Warm(rocketPrefab.GetComponent<MSSimplePoolable>(), 6);
	}
	
	public void ResetCombo()
	{
		combo = 0;
		for (int i = 0; i < currGems.Length; i++) {
			currGems[i] = 0;
		}
		if (MSActionManager.Puzzle.OnComboChange != null)
		{
			MSActionManager.Puzzle.OnComboChange(combo);
		}
	}

	public void InitBoardFromSave(PZCombatSave save, MinimumUserTaskProto minTask)
	{
		FullTaskProto task = MSDataManager.instance.Get<FullTaskProto>(minTask.taskId);

		ClearBoard();

		boardWidth = task.boardWidth;
		boardHeight = task.boardHeight;
		
		board = new PZGem[boardWidth, boardHeight];
		jellyBoard = new PZJelly[boardWidth, boardHeight];
		
		boardSprite.width = boardWidth * SPACE_SIZE;
		boardSprite.height = boardHeight * SPACE_SIZE;

		PZGem gem;

		for (int x = 0; x < boardWidth; x++) 
		{
			for (int y = 0; y < boardHeight; y++) 
			{
				gem = MSPoolManager.instance.Get(gemPrefab, Vector3.zero, puzzleParent) as PZGem;
				gem.SpawnOnMap(save.gemColors[x, y], x);
				gem.gemType = (PZGem.GemType)save.gemTypes[x,y];
				if (save.jelly[x,y])
				{
					PZJelly jelly = MSPoolManager.instance.Get<PZJelly>(jellyPrefab, puzzleParent);
					jelly.InitOnBoard(1, x, y);
					jellyBoard[x,y] = jelly;
				}
			}
		}

		setUpBoard = true;
	}

	public void InitBoard(int w = STANDARD_BOARD_WIDTH, int h = STANDARD_BOARD_HEIGHT, string boardFile = "")
	{
		//Make sure we clear the board with the old values for width and height!
		ClearBoard();

		boardWidth = w;
		boardHeight = h;

		board = new PZGem[w, h];
		jellyBoard = new PZJelly[w, h];

		boardSprite.width = w * SPACE_SIZE;
		boardSprite.height = h * SPACE_SIZE;
		
		foreach (var item in riggedBoardStacks) 
		{
			item.Clear();
		}
		if (boardFile != "")
		{
			RigBoard(boardFile);
		}

		PZGem gem;

		do
		{
			for (int i = 0; i < boardHeight; i++) 
			{
				for (int j = 0; j < boardWidth; j++) 
				{
					gem = MSPoolManager.instance.Get(gemPrefab, Vector3.zero, puzzleParent) as PZGem;
					gem.SpawnOnMap(PickColor(i, j), j);
				}
			}
		}while(!CheckForMatchMoves());
		showGemHints = true;
		setUpBoard = true;
	}

	void RigBoard(string boardFile)
	{
		try
		{

			string fileText = Resources.Load<TextAsset>(boardFile).text;
			string[] lines = fileText.Split('\n');

			foreach (var item in lines) 
			{
				for (int i = 0; i < item.Length; i++) 
				{
					riggedBoardStacks[i].Push((int)char.GetNumericValue(item[i]) - 1);
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Problem opening file: " + e.Message);
		}
	}

	public void ClearBoard ()
	{
		if (setUpBoard)
		{
			for (int i = 0; i < boardHeight; i++) 
			{
				for (int j = 0; j < boardWidth; j++) 
				{
					board[j,i].Pool();
					board[j,i] = null;

					if (jellyBoard[j,i] != null)
					{
						jellyBoard[j,i].Pool();
						jellyBoard[j,i] = null;
					}
				}
			}
		}
		for (int i = 0; i < GEM_TYPES; i++) 
		{
			//gemsOnBoardByType[i] = 0;
		}
	}

	/// <summary>
	/// Picks a color.
	/// If coordiantes are suppiled, this uses the space two to the
	/// left and the space two down to determine what colors to not be,
	/// in order to guarentee no starting matches. 
	/// </summary>
	/// <returns>
	/// The color index
	/// </returns>
	/// <param name='row'>
	/// Row.
	/// </param>
	/// <param name='col'>
	/// Column
	/// </param>
	public int PickColor(int row, int col)
	{
		if (riggedBoardStacks[col].Count > 0)
		{
			return riggedBoardStacks[col].Pop();
		}
		
		int picked;
		int rowCol = -1;
		if (row >= 2)
		{
			rowCol = board[col,row-2].colorIndex;
		}
		int colCol = -1;
		if (col >= 2)
		{
			colCol = board[col-2,row].colorIndex;
		}
		while(true)
		{
			picked = UnityEngine.Random.Range(0,gemTypes.Length);
			if (picked != rowCol && picked != colCol)
			{
				break;
			}
		}
		//gemsOnBoardByType[picked]++;
		return picked;
	}

	public int PickColor(int col)
	{
		if (riggedBoardStacks[col].Count > 0)
		{
			return riggedBoardStacks[col].Pop();
		}

		return UnityEngine.Random.Range(0,gemTypes.Length);
	}
	
	public void OnStartMoving(PZGem gem)
	{
		//Debug.Log("Lock");
		if (!movingGems.Contains(gem))
		{
			swapLock += 1;
			movingGems.Add(gem);
		}
	}
	
	public void OnStopMoving(PZGem gem)
	{
		movingGems.Remove(gem);
		//Debug.Log("Unlock");
		swapLock -= 1;
		if (!gemsToCheck.Contains(gem))
		{
			gemsToCheck.Add(gem);
		}

		CheckIfTurnFinished();
		
		//After the board is checked, if nothing is falling, unlock the board
		
	}

	void CheckIfTurnFinished ()
	{
		if (movingGems.Count == 0) 
		{
			CheckWholeBoard ();
			gemsToCheck.Clear ();
			if (movingGems.Count == 0) {
				if (combo == 0) {
					lastSwapSuccessful = false;
				}
				else {
					lastSwapSuccessful = true;
					PZCombatManager.instance.OnBreakGems (currGems, combo);
				}
				processingSwap = false;
				showGemHints = true;
				ResetCombo ();
				if (!CheckForMatchMoves ()) {
					InitBoard (boardWidth, boardHeight);
				}
			}
			processingSwap = false;

			if (MSActionManager.Puzzle.OnNewPlayerTurn != null)
			{
				MSActionManager.Puzzle.OnNewPlayerTurn();
			}
		}
	}

	void GetMatchesOnBoard (List<PZMatch> matchList)
	{
		List<PZGem> currGems = new List<PZGem>();
		PZGem curr;
		
		//Determine horizontal matches
		for (int i = 0; i < boardWidth; i++) 
		{
			currGems.Add(board[i,0]);
			for (int j = 1; j < boardHeight; j++) 
			{
				curr = board[i,j];
				
				if (currGems[0].colorIndex != curr.colorIndex)
				{
					if (currGems.Count >= 3)
					{
						foreach (PZGem item in currGems)
						{
							if (item.gemType != PZGem.GemType.ROCKET)
							{
								item.horizontal = true;
							}
						}
						matchList.Add(new PZMatch(currGems));
					}
					currGems.Clear ();
				}
				currGems.Add(curr);
			}
			if (currGems.Count >= 3)
			{
				foreach (PZGem item in currGems)
				{
					if (item.gemType != PZGem.GemType.ROCKET)
					{
						item.horizontal = true;
					}
				}
				matchList.Add(new PZMatch(currGems));
			}
			
			currGems.Clear ();
		}
		
		for (int i = 0; i < boardHeight; i++) {
			currGems.Add (board[0,i]);
			for (int j = 1; j < boardWidth; j++) {
				curr = board[j,i];
				if (currGems[0].colorIndex != curr.colorIndex)
				{
					if (currGems.Count >= 3)
					{
						foreach (PZGem item in currGems)
						{
							if (item.gemType != PZGem.GemType.ROCKET)
							{
								item.horizontal = false;
								//Debug.Log(item);
							}
						}
						matchList.Add(new PZMatch(currGems));
					}
					currGems.Clear ();
				}
				currGems.Add(curr);
			}
			if (currGems.Count >= 3)
			{
				foreach (PZGem item in currGems)
				{
					if (item.gemType != PZGem.GemType.ROCKET)
					{
						item.horizontal = false;
					}
				}
				matchList.Add(new PZMatch(currGems));
			}
			currGems.Clear ();
		}
	}

	void CombineMultiMatches (List<PZMatch> matchList)
	{
		//Combine all matches that are connected (T's and L's)
		foreach (PZMatch match in matchList)
		{
			/*
			if (match.special)
			{
				continue;
			}
			*/
			foreach (PZMatch other in matchList)
			{
				if (match != other)// && !other.special)
				{
					match.CheckAgainst(other);
				}
			}
		}
	}
	
	/// <summary>
	/// Detonates all special matches, creating new matches for them
	/// and adding them to the destruction list
	/// </summary>
	/// <param name='matchList'>
	/// Match list.
	/// </param>
	void DetonateSpecialsInMatches (List<PZMatch> matchList)
	{
		PZMatch match;
		
		//Bomb and rocket logic, to create the matches
		//before we go into match processing
		for (int i = 0; i < matchList.Count; i++) 
		{
			foreach (PZGem gem in matchList[i].gems)
			{
				if (gem.gemType != PZGem.GemType.NORMAL)
				{
					switch(gem.gemType)
					{
					case PZGem.GemType.BOMB:
						match = GetBombMatch(gem);
						RemoveRepeats(match, matchList);
						matchList.Add(match);
						break;
					case PZGem.GemType.MOLOTOV:
						match = GetMolotovGroup(gem, gem.colorIndex);
						break;
					case PZGem.GemType.ROCKET:
						match = DetonateRocket(gem);
						break;
					default:
						match = null;
						break;
					}
				}
			}
		}
	}

	void DestroyMatches (List<PZMatch> matchList)
	{
		//Process and destroy each match
		foreach (PZMatch match in matchList)
		{
			if (match.gems.Count > 0)
			{
				match.Destroy();
			}
		}
	}

	void IncrementCombo (List<PZMatch> matchList)
	{
		foreach (PZMatch item in matchList) 
		{
			if (item.gems.Count > 0)
			{
				combo++;
				if (MSActionManager.Puzzle.OnComboChange != null)
				{
					MSActionManager.Puzzle.OnComboChange(combo);
				}
			}
		}
	}
	
	/// <summary>
	/// Checks the whole board.
	/// TODO: Do this more efficiently when only a select
	/// group of gems is moving
	/// </summary>
	public void CheckWholeBoard()
	{
		//Debug.Log("Checking whole board");
		
		List<PZMatch> matchList = new List<PZMatch>();
		
		GetMatchesOnBoard (matchList);

		if (matchList.Count > 0)
		{
			if (MSActionManager.Puzzle.OnGemMatch != null)
			{
				MSActionManager.Puzzle.OnGemMatch();
			}
		}
		
		CombineMultiMatches (matchList);
		
		DetonateSpecialsInMatches (matchList);
		
		IncrementCombo (matchList);
		
		DestroyMatches (matchList);
	}
	
	/// <summary>
	/// Removes gems from a given match that appear in other matches.
	/// Used for special groups since we need to make sure we're not detonating
	/// any gems multiple times
	/// </summary>
	/// <param name='match'>
	/// The new match being joined into the main match list
	/// </param>
	/// <param name='otherMatches'>
	/// The existing list of all other current matches
	/// </param>
	public void RemoveRepeats(PZMatch match, List<PZMatch> otherMatches)
	{
		for (int i = match.gems.Count - 1; i >= 0; i--) //Go backwards through group since we're removing from it
		{
			foreach (PZMatch item in otherMatches) 
			{
				if (item.gems.Contains(match.gems[i]))
				{
					match.gems.RemoveAt(i);
					break;
				}
			}
		}
	}

	/// <summary>
	/// For when a bomb is swapped with iether a rocket or another bomb
	/// </summary>
	/// <param name="source">Gem being dragged</param>
	/// <param name="destination">Gem that the dragged gem is being swapped with</param>
	public void DetonateBombFromSwap(PZGem source, PZGem destination){
		PZGem gem;
		if (source.gemType == PZGem.GemType.ROCKET || destination.gemType == PZGem.GemType.ROCKET) {

			PZGem rocket = source.gemType == PZGem.GemType.ROCKET ? source : destination;
			Vector2 offset = rocket.horizontal? new Vector2(0,1) : new Vector2(1,0);

			if(rocket.boardX + offset.x < boardWidth && rocket.boardY + offset.y < boardHeight){
				gem = board[rocket.boardX + (int)offset.x,rocket.boardY + (int)offset.y];
				gem.gemType = PZGem.GemType.ROCKET;
				gem.Detonate();
			}

			rocket.Detonate();

			if(rocket.boardX - offset.x >= 0 && rocket.boardY - offset.y >= 0){
				gem = board[rocket.boardX - (int)offset.x,rocket.boardY - (int)offset.y];
				gem.gemType = PZGem.GemType.ROCKET;
				gem.Detonate();
			}

		} else { //else bomb-bomb swap
			int originx = destination.boardX;
			int originy = destination.boardY;
			PZMatch bombMatch = new PZMatch();

			if(originx + 1 < boardWidth){
				gem = board[originx + 1,originy];
				bombMatch = GetBombMatch(gem);
			}
			if(originx - 1 >= 0){
				gem = board[originx - 1,originy];
				bombMatch.CheckAgainst(GetBombMatch(gem));
			}

			if(originy + 1 < boardHeight){
				gem = board[originx,originy + 1];
				bombMatch.CheckAgainst(GetBombMatch(gem));
			}
			if(originy - 1 >= 0){
				gem = board[originx,originy - 1];
				bombMatch.CheckAgainst(GetBombMatch(gem));
			}
			source.gemType = PZGem.GemType.NORMAL;
			destination.gemType = PZGem.GemType.NORMAL;
			bombMatch.Destroy();
			
		}
	}
	
	public void DetonateMolotovFromSwap(PZGem molly, PZGem other)
	{
		if (molly == other)
		{
			molly.colorIndex = UnityEngine.Random.Range(0, GEM_TYPES);
		}

		List<PZMatch> matchList = new List<PZMatch>();
		PZMatch matching = GetMolotovGroup(molly, other.colorIndex);
		switch (other.gemType) {
		case PZGem.GemType.BOMB:
			foreach (var item in matching.gems) 
			{
				item.gemType = PZGem.GemType.BOMB;
			}
			break;
		case PZGem.GemType.MOLOTOV:

			return;
		case PZGem.GemType.ROCKET:
			foreach (var item in matching.gems) 
			{
				item.horizontal = UnityEngine.Random.value > .5f;
				item.gemType = PZGem.GemType.ROCKET;
			}
			break;
		default:
			break;
		}
		
		matchList.Add(matching);
		molly.gemType = PZGem.GemType.NORMAL;
		
		//DetonateSpecialsInMatches(matchList);
		DestroyMatches(matchList);
		IncrementCombo(matchList);



	}
	
	PZMatch GetMolotovGroup(PZGem molly, int colorIndex)
	{
		List<PZGem> gems = new List<PZGem>();
		PZGem gem;
		gems.Add(molly);
		int index = 0;
		for (int i = 0; i < boardWidth; i++) {
			for (int j = 0; j < boardHeight; j++) {
				gem = board[i,j];
				if (gem != null && gem.colorIndex == colorIndex && gem != molly)
				{
					PZMolotovPart mp = (MSPoolManager.instance.Get(molotovPartPrefab.GetComponent<MSSimplePoolable>(), molly.transf.localPosition) as MonoBehaviour).GetComponent<PZMolotovPart>();
					mp.trans.parent = puzzleParent;
					mp.trans.localScale = Vector3.one;
					mp.Init(molly.transf.localPosition, gem.transf.localPosition, gem, index++);
					gem.lockedBySpecial = true;
					OnStartMoving(gem);
					gems.Add(board[i,j]);
				}
			}
		}
		return new PZMatch(gems, true);
	}
	
	public PZMatch GetBombMatch(PZGem bomb)
	{
		MSPoolManager.instance.Get(MSPrefabList.instance.grenadeParticle, bomb.transf.position);

		List<PZGem> gems = new List<PZGem>();
		if (bomb.boardX > 0)
		{
			gems.Add(board[bomb.boardX-1, bomb.boardY]);
			if (bomb.boardY > 0)
			{
				gems.Add(board[bomb.boardX-1, bomb.boardY-1]);
			}
			if (bomb.boardY < boardHeight-1)
			{
				gems.Add(board[bomb.boardX-1, bomb.boardY+1]);
			}
		}
		if (bomb.boardX < boardWidth-1)
		{
			gems.Add(board[bomb.boardX+1, bomb.boardY]);
			if (bomb.boardY > 0)
			{
				gems.Add(board[bomb.boardX+1, bomb.boardY-1]);
			}
			if (bomb.boardY < boardHeight-1)
			{
				gems.Add(board[bomb.boardX+1, bomb.boardY+1]);
			}
		}
		if (bomb.boardY > 0)
		{
			gems.Add(board[bomb.boardX, bomb.boardY-1]);
		}
		if (bomb.boardY < boardHeight-1)
		{
			gems.Add(board[bomb.boardX, bomb.boardY+1]);
		}
		for (int i = 0; i < gems.Count; ) 
		{
			if (gems[i] == null)
			{
				gems.RemoveAt(i);
				continue;
			}
			CheckIfMolly(gems[i], bomb.colorIndex);
			i++;
		}
		return new PZMatch(gems, true);
	}
	
	public PZMatch DetonateRocket(PZGem gem)
	{

		List<PZGem> gems = new List<PZGem>();
		if (gem.horizontal)
		{
			PZRocket rocket = (MSPoolManager.instance.Get(rocketPrefab.GetComponent<MSSimplePoolable>(), gem.transf.position, puzzleParent) as MonoBehaviour).GetComponent<PZRocket>();
			rocket.Init(MSValues.Direction.EAST);
			rocket.trans.localRotation = Quaternion.identity;
			rocket.trans.localScale = Vector3.one;
			rocket.trans.localPosition = gem.transf.localPosition;

			rocket = (MSPoolManager.instance.Get(rocketPrefab.GetComponent<MSSimplePoolable>(), gem.transf.position, puzzleParent) as MonoBehaviour).GetComponent<PZRocket>();
			rocket.Init(MSValues.Direction.WEST);
			rocket.trans.localRotation = Quaternion.identity;
			rocket.trans.localScale = new Vector3(-1,1,1);
			rocket.trans.localPosition = gem.transf.localPosition;

			for (int i = 0; i < boardWidth; i++) 
			{
				PZGem target = board[i, gem.boardY];
				if (target == null)
				{
					continue;
				}
				target.lockedBySpecial = true;
				gems.Add(target);
				OnStartMoving(target);
				CheckIfMolly(target, gem.colorIndex);
			}
		}
		else
		{
			PZRocket rocket = (MSPoolManager.instance.Get(rocketPrefab.GetComponent<MSSimplePoolable>(), gem.transf.position, puzzleParent) as MonoBehaviour).GetComponent<PZRocket>();
			rocket.Init(MSValues.Direction.NORTH);
			rocket.trans.localRotation = new Quaternion(0,0,.707f,.707f);
			rocket.trans.localScale = Vector3.one;
			rocket.trans.localPosition = gem.transf.localPosition;

			rocket = (MSPoolManager.instance.Get(rocketPrefab.GetComponent<MSSimplePoolable>(), gem.transf.position, puzzleParent) as MonoBehaviour).GetComponent<PZRocket>();
			rocket.Init(MSValues.Direction.SOUTH);
			rocket.trans.localRotation = new Quaternion(0,0,.707f,.707f);
			rocket.trans.localScale = new Vector3(-1,1,1);
			rocket.trans.localPosition = gem.transf.localPosition;

			for (int i = 0; i < boardHeight; i++) 
			{
				PZGem target = board[gem.boardX, i];
				if (target == null)
				{
					continue;
				}
				target.lockedBySpecial = true;
				gems.Add (target);
				OnStartMoving(target);
				CheckIfMolly(target, gem.colorIndex);
			}	
		}
		gem.gemType = PZGem.GemType.NORMAL;
		return new PZMatch(gems, true);
	}
	
	void CheckIfMolly(PZGem gem, int colorToGive)
	{
		if (gem.gemType == PZGem.GemType.MOLOTOV)
		{
			gem.colorIndex = colorToGive;
		}
	}
	
	/// <summary>
	/// Checks the whole board to see if there are any moves that
	/// could produce matches
	/// PRECONDITION: Board doesn't have any matches on it
	/// </summary>
	/// <returns>
	/// The for matches.
	/// </returns>
	bool CheckForMatchMoves()
	{
		PZGem gem;
		//Check horizontal possibilities
		for (int i = 0; i < boardHeight; i++) 
		{
			for (int j = 0; j < boardWidth-2; j++) 
			{
				gem = board[j, i];
				if (j < boardWidth-3)
				{
					if (gem.colorIndex == board[j+2,i].colorIndex && gem.colorIndex == board[j+3,i].colorIndex)
					{
						hintgems[0] = board[j,i];
						hintgems[1] = board[j+2,i];
						hintgems[2] = board[j+3,i];
						return true;
					}
					if (gem.colorIndex == board[j+1,i].colorIndex && gem.colorIndex == board[j+3,i].colorIndex)
					{
						hintgems[0] = board[j,i];
						hintgems[1] = board[j+1,i];
						hintgems[2] = board[j+3,i];
						return true;
					}
				}
				if (i > 0)
				{
					if (gem.colorIndex == board[j+1,i].colorIndex && gem.colorIndex == board[j+2,i-1].colorIndex)
					{
						hintgems[0] = board[j,i];
						hintgems[1] = board[j+1,i];
						hintgems[2] = board[j+2,i-1];
						return true;
					}
					if (gem.colorIndex == board[j+2,i].colorIndex && gem.colorIndex == board[j+1,i-1].colorIndex)
					{
						hintgems[0] = board[j,i];
						hintgems[1] = board[j+2,i];
						hintgems[2] = board[j+1,i-1];
						return true;
					}
					if (gem.colorIndex == board[j+1,i-1].colorIndex && gem.colorIndex == board[j+2,i-1].colorIndex)
					{
						hintgems[0] = board[j,i];
						hintgems[1] = board[j+1,i-1];
						hintgems[2] = board[j+2,i-1];
						return true;
					}
				}
				if (i < boardHeight-1)
				{
					if (gem.colorIndex == board[j+1,i].colorIndex && gem.colorIndex == board[j+2,i+1].colorIndex)
					{
						hintgems[0] = board[j,i];
						hintgems[1] = board[j+1,i];
						hintgems[2] = board[j+2,i+1];
						return true;
					}
					if (gem.colorIndex == board[j+2,i].colorIndex && gem.colorIndex == board[j+1,i+1].colorIndex)
					{
						hintgems[0] = board[j,i];
						hintgems[1] = board[j+2,i];
						hintgems[2] = board[j+1,i+1];
						return true;
					}
					if (gem.colorIndex == board[j+2,i+1].colorIndex && gem.colorIndex == board[j+1,i+1].colorIndex)
					{
						hintgems[0] = board[j,i];
						hintgems[1] = board[j+2,i+1];
						hintgems[2] = board[j+1,i+1];
						return true;
					}
				}
			}
		}
		
		//Check vertical possibilities
		for (int i = 0; i < boardWidth; i++) 
		{
			for (int j = 0; j < boardHeight-2; j++) 
			{
				gem = board[i,j];
				if (gem.gemType == PZGem.GemType.MOLOTOV)
				{
					return true;
				}
				if (j < boardHeight-3)
				{
					if (gem.colorIndex == board[i,j+2].colorIndex && gem.colorIndex == board[i, j+3].colorIndex)
					{
						hintgems[0] = board[i,j];
						hintgems[1] = board[i,j+2];
						hintgems[2] = board[i,j+3];
						return true;
					}
					if (gem.colorIndex == board[i, j+1].colorIndex && gem.colorIndex == board[i, j+3].colorIndex)
					{
						hintgems[0] = board[i,j];
						hintgems[1] = board[i,j+1];
						hintgems[2] = board[i,j+3];
						return true;
					}
				}
				if (i > 0)
				{
					if (gem.colorIndex == board[i,j+1].colorIndex && gem.colorIndex == board[i-1,j+2].colorIndex)
					{
						hintgems[0] = board[i,j];
						hintgems[1] = board[i,j+1];
						hintgems[2] = board[i-1,j+2];
						return true;
					}
					if (gem.colorIndex == board[i,j+2].colorIndex && gem.colorIndex == board[i-1,j+1].colorIndex)
					{
						hintgems[0] = board[i,j];
						hintgems[1] = board[i,j+2];
						hintgems[2] = board[i-1,j+1];
						return true;
					}
					if (gem.colorIndex == board[i-1, j+1].colorIndex && gem.colorIndex == board[i-1, j+2].colorIndex)
					{
						hintgems[0] = board[i,j];
						hintgems[1] = board[i-1,j+1];
						hintgems[2] = board[i-1,j+2];
						return true;
					}
				}
				if (i < boardHeight-1)
				{
					if (gem.colorIndex == board[i,j+1].colorIndex && gem.colorIndex == board[i+1,j+2].colorIndex)
					{
						hintgems[0] = board[i,j];
						hintgems[1] = board[i,j+1];
						hintgems[2] = board[i+1,j+2];
						return true;
					}
					if (gem.colorIndex == board[i,j+2].colorIndex && gem.colorIndex == board[i+1,j+1].colorIndex)
					{
						hintgems[0] = board[i,j];
						hintgems[1] = board[i,j+2];
						hintgems[2] = board[i+1,j+1];
						return true;
					}
					if (gem.colorIndex == board[i+1,j+1].colorIndex && gem.colorIndex == board[i+1,j+2].colorIndex)
					{
						hintgems[0] = board[i,j];
						hintgems[1] = board[i+1,j+1];
						hintgems[2] = board[i+1,j+2];
						return true;
					}
				}
			}
		}
		
		return false;
	}

	public float HighestGemInColumn(int col)
	{
		if (columnQueues[col].Count > 0)
		{
			return columnQueues[col][columnQueues[col].Count-1].transf.localPosition.y;
		}
		for (int i = boardHeight-1; i >= 0; i--) 
		{
			if (board[col,i] != null)
			{
				return board[col, i].transf.localPosition.y;
			}
		}
		return 0;
	}

	public void BlockBoard(List<Vector2> exceptSpaces)
	{
		for (int i = 0; i < boardWidth; i++) 
		{
			for (int j = 0; j < boardHeight; j++) 
			{
				if (!exceptSpaces.Contains(new Vector2(i,j)))
				{
					board[i,j].Block();
				}
			}
		}
	}

	[ContextMenu ("start hint")]
	public void StartHint()
	{
		showGemHints = true;
	}

	[ContextMenu ("stop hint")]
	public void StopHint()
	{
//		showingGemHints = false;
	}

	public IEnumerator HintCycle()
	{
		float timeBetweenCycles = 5f;
		yield return new WaitForSeconds(timeBetweenCycles); //wait on frame for showinggemshint to update
		while(showGemHints)
		{
			foreach(PZGem gem in hintgems)
			{
				if(gem != null)
				{
					//The hint animation playes the scale and color tween 4 times in a row.
					gem.HintAnimation();
				}
			}
			yield return new WaitForSeconds(timeBetweenCycles);
		}
	}

	[ContextMenu ("Print board")]
	public void PrintBoard()
	{
		string str = "Board";
		string gem = "";
		for (int i = boardHeight-1; i >= 0; i--) 
		{
			str += "\n";
			for (int j = 0; j < boardWidth; j++) 
			{
				if (board[j,i] == null)
				{
					gem = "e";
				}
				else if (board[j,i].colorIndex < 0)
				{
					gem = "n";
				}
				else
				{
					gem = board[j,i].colorIndex.ToString();
				}

				if (jellyBoard[j,i] != null)
				{
					gem = "(" + gem + ") ";
				}
				else
				{
					gem = " " + gem + "  ";
				}

				str += gem;
			}
		}
		Debug.Log(str);
	}

	/// <summary>
	/// Attempts to force all gems on and off the board to fall
	/// </summary>
	[ContextMenu ("checkFallAll")]
	public void ReCheckBoard(){
		int columnCount = 0;
		for (int i = 0; i < boardHeight; i++) 
		{
			for (int j = 0; j < boardWidth; j++) 
			{

				if(board[j,i] != null){
					board[j,i].CheckFall();
				}
			}
		}
		for(int i = 0; i < columnQueues.Length; i++){
			columnCount = columnQueues[i].Count;
			for(int j = 0; j < columnCount; j++)
			{
				columnQueues[i][0].CheckFall();
			}
			if(columnQueues[i].Count > 0)
			{
				Debug.LogError("Gems could not drop into board");
			}
		}
	}

	#region Jelly

	public bool ClearJelly(int x, int y, int gemId)
	{
		if (jellyBoard[x,y] != null)
		{
			jellyBoard[x,y].Damage();
			Debug.Log(gemId + " damaged jelly at " + x + ", " + y);
			return true;
		}
		Debug.Log("No jelly for " + gemId + " at " + x + ", " + y);
		return false;
	}

	public void ThrowJellies(int num)
	{
		for (int i = 0; i < num; i++) 
		{
			ThrowJelly();
		}
	}

	[ContextMenu ("Throw Jelly")]
	public void ThrowJelly()
	{
		Vector2 jellyBoardPos = FindJellyPosition();
		if (jellyBoardPos.x > -1 && jellyBoardPos.y > -1)
		{
			int boardX = (int)jellyBoardPos.x;
			int boardY = (int)jellyBoardPos.y;

			//Init Jelly
			PZJelly jelly = MSPoolManager.instance.Get<PZJelly>(jellyPrefab, puzzleParent);
			jelly.Init(PZCombatManager.instance.activeEnemy.transform.position, boardX, boardY);
			jellyBoard[boardX, boardY] = jelly;
		}
		else
		{
			Debug.Log("Not enough room for jelly!");
		}
	}

	Vector2 FindJellyPosition()
	{
		int boardX = UnityEngine.Random.Range(0, boardWidth);
		int boardY = UnityEngine.Random.Range(0, boardHeight);
		int xMod = 1;
		int yMod = 1;

		if (jellyBoard[boardX, boardY] == null) return new Vector2(boardX, boardY);
		while (xMod < boardWidth && yMod < boardWidth)
		{
			for (int x = -xMod; x <= xMod; x++) 
			{
				if (boardX+x < 0 || boardX+x >= boardWidth) continue;
				if (boardY+yMod < boardHeight && jellyBoard[boardX+x, boardY+yMod] == null) return new Vector2(boardX+x, boardY+yMod);
				if (boardY-yMod >= 0 && jellyBoard[boardX+x, boardY-yMod] == null) return new Vector2(boardX+x, boardY-yMod);
			}
			for (int y = -yMod+1; y < yMod; y++) 
			{
				if (boardY+y < 0 || boardY+y >= boardHeight) continue;
				if (boardX+xMod < boardWidth && jellyBoard[boardX+xMod, boardY+y] == null) return new Vector2(boardX+xMod, boardY+y);
				if (boardX-xMod >= 0 && jellyBoard[boardX-xMod, boardY+y] == null) return new Vector2(boardX-xMod, boardY+y);
			}
			xMod++;
			yMod++;
		}

		return new Vector2(-1, -1); //This is the error signal!
	}

	#endregion
}
