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

				//MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.combos[Mathf.Min(_combo,MSSoundManager.instance.combos.Length)-1]);
			}
			else
			{
				comboLabel.text = " ";
			}
		}
	}

	public Transform puzzleParent;
	
	List<PZGem> movingGems = new List<PZGem>();
	List<PZGem> hintgems = new List<PZGem>();

	public List<PZGem> cakes = new List<PZGem>();

	public int maxCakes;
	public float currCakeChance;
	
	[SerializeField]
	PZGem gemPrefab;
	
	public PZGem[,] board;

	public GemSaveData[,] lastBoardSnapshot;

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

	/// <summary>
	/// amount of speed lost per bounce
	/// </summary>
	public float BOUNCE_REDUCTION = 0.8f;

	/// <summary>
	/// When the speed of a gem is below this, it will no longer bounce
	/// </summary>
	public float BOUNCE_THRESHHOLD = 100;
		
	public float GRAVITY = -600f;
	
	public float SWAP_TIME = .2f;

	public bool canSpawnBombs;
	public int bombColor;
	public int maxBombs;
	public int minBombs;
	public int bombTicks;
	public float bombChance;
	public int bombDamage;

	public int poisonColor = -1;

	bool _gemHints = false;

	IEnumerator hintCycle;

	/// <summary>
	/// gem hints are when the gems animating to show a match.  Set to true to start.
	/// </summary>
	bool showGemHints
	{
		set
		{
			if(value && !_gemHints && !MSTutorialManager.instance.inTutorial)
			{
				StartCoroutine(hintCycle);//has to be started with a string so I can stop it later.
			}
			else if (!value && _gemHints)
			{
				foreach(PZGem gem in hintgems)
				{
					if(gem!=null)
					{
						gem.CancelHintAnimation();
					}
					StopCoroutine(hintCycle);
					hintCycle = HintCycle();
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

		hintCycle = HintCycle();
		MSActionManager.Puzzle.OnGemMatch += delegate { showGemHints = false; };
		MSActionManager.Puzzle.OnTurnChange += CheckStartHintGems;
	}

	public void Start()
	{
		MSPoolManager.instance.Warm(rocketPrefab, 6);
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
		GemSaveData gemSave;
		for (int x = 0; x < boardWidth; x++) 
		{
			for (int y = 0; y < boardHeight; y++) 
			{
				gemSave = save.board[x,y];
				gem = MSPoolManager.instance.Get(gemPrefab, Vector3.zero, puzzleParent) as PZGem;
				gem.SpawnOnMap(gemSave.gemColor, x);
				gem.gemType = gemSave.type;
				switch(gem.gemType)
				{
				case PZGem.GemType.CAKE:
					cakes.Add(gem);
					break;
				case PZGem.GemType.BOMB:
					PZCombatManager.instance.bombs.Add(gem);
					gem.bombTicks = gemSave.bombTicks;
					gem.bombDamage = gemSave.bombDamage;
					gem.BombLabelRefresh();
					break;
				}
				if (gemSave.jelly)
				{
					PZJelly jelly = MSPoolManager.instance.Get<PZJelly>(jellyPrefab, puzzleParent);
					jelly.InitOnBoard(x, y);
					jellyBoard[x,y] = jelly;
				}
			}
		}

		setUpBoard = true;
		BoardSnapshot();
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
		if (!boardFile.Equals(""))
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
		}while(!CheckForMatchMoves(board));
		setUpBoard = true;

		BoardSnapshot();
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
		cakes.Clear();
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
		return picked;
	}

	public int PickRevertCakeColor(int row, int col)
	{
		int picked;
		int rowColL = -1;
		if (row >= 2)
		{
			rowColL = board[col,row-2].colorIndex;
		}
		int rowColR = 1;
		if (row < boardWidth - 2)
		{
			rowColR = board[col, row+2].colorIndex;
		}
		int colColD = -1;
		if (col >= 2)
		{
			colColD = board[col-2,row].colorIndex;
		}
		int colColU = -1;
		if (col < boardHeight - 2)
		{
			colColU = board[col+2, row].colorIndex;
		}
		while(true)
		{
			picked = UnityEngine.Random.Range(0,gemTypes.Length);
			if (picked != rowColL 
			    && picked != colColD 
			    && picked != rowColR 
			    && picked != colColU)
			{
				break;
			}
		}
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
		if (!movingGems.Contains(gem))
		{
			//Debug.LogWarning("Gem Moving Lock");

			swapLock += 1;
			movingGems.Add(gem);
		}
	}
	
	public void OnStopMoving(PZGem gem)
	{
		movingGems.Remove(gem);
		//Debug.LogWarning("Gem Stopped Unlock");
		swapLock -= 1;

		StartCoroutine(CheckIfTurnFinished());
	}

	IEnumerator CheckIfTurnFinished ()
	{
		if (movingGems.Count == 0) 
		{
			bool hasCakes = false;
			foreach (var item in cakes) 
			{
				if (item.CheckCake())
				{
					item.RunEatCake();
					hasCakes = true;
				}
			}
			if (hasCakes)
			{
				while (cakes.Find(x=>x.isCaking) != null)
				{
					yield return null;
				}
				PZCombatScheduler.instance.CakeReset(PZCombatManager.instance.enemyDefSkill.properties.Find(x=>x.name == "SPEED_MULTIPLIER").skillValue);
			}

			CheckWholeBoard ();

			if (movingGems.Count == 0) {
				if (combo == 0) {
					lastSwapSuccessful = false;
				}
				else {
					lastSwapSuccessful = true;
					PZCombatManager.instance.OnBreakGems (currGems, combo);
				}
				processingSwap = false;
				ResetCombo ();

				if (lastSwapSuccessful && !CheckForMatchMoves (board)) 
				{
					Shuffle();
				}

				BoardSnapshot();
			}
			processingSwap = false;

			if (MSActionManager.Puzzle.OnDragFinished != null)
			{
				MSActionManager.Puzzle.OnDragFinished();
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
				
				if (curr.colorIndex < 0 || currGems[0].colorIndex != curr.colorIndex)
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
				if (curr.colorIndex < 0 || currGems[0].colorIndex != curr.colorIndex)
				{
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
		
		//Grenade and rocket logic, to create the matches
		//before we go into match processing
		for (int i = 0; i < matchList.Count; i++) 
		{
			foreach (PZGem gem in matchList[i].gems)
			{
				if (gem.gemType != PZGem.GemType.NORMAL)
				{
					switch(gem.gemType)
					{
					case PZGem.GemType.GRENADE:
						match = GetGrenadeMatch(gem);
						RemoveRepeats(match, matchList);
						matchList.Add(match);
						MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.gemExplode);
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

	bool DestroyMatches (List<PZMatch> matchList)
	{
		bool didDestroy = false;
		//Process and destroy each match
		foreach (PZMatch match in matchList)
		{
			if (match.gems.Count > 0)
			{
				match.Destroy();
				didDestroy = true;
			}
		}
		return didDestroy;
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

		if (DestroyMatches (matchList))
		{
			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.gemPop);
		}
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
	/// For when a rocket is swapped into another rocket.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="destination">Destination.</param>
	public void DetonateDoubleRocket(PZGem source, PZGem destination)
	{
		//may seem trivial but increment combo should stay private
		List<PZMatch> rocketMatches = new List<PZMatch>();
		source.horizontal = true;
		rocketMatches.Add(PZPuzzleManager.instance.DetonateRocket(source));
		destination.horizontal = false;
		rocketMatches.Add(PZPuzzleManager.instance.DetonateRocket(destination));
		IncrementCombo(rocketMatches);
	}

	/// <summary>
	/// For when a grenade is swapped with iether a rocket or another grenade
	/// </summary>
	/// <param name="source">Gem being dragged</param>
	/// <param name="destination">Gem that the dragged gem is being swapped with</param>
	public void DetonateGrenadeFromSwap(PZGem source, PZGem destination){
		PZGem gem;
		if (source.gemType == PZGem.GemType.ROCKET || destination.gemType == PZGem.GemType.ROCKET) {
			List<PZMatch> rocketMatches = new List<PZMatch>();

			PZGem rocket = source.gemType == PZGem.GemType.ROCKET ? source : destination;
			Vector2 offset = rocket.horizontal? new Vector2(0,1) : new Vector2(1,0);

			if(rocket.boardX + offset.x < boardWidth && rocket.boardY + offset.y < boardHeight){
				gem = board[rocket.boardX + (int)offset.x,rocket.boardY + (int)offset.y];
				gem.gemType = PZGem.GemType.ROCKET;
				gem.horizontal = rocket.horizontal;
				rocketMatches.Add(DetonateRocket(gem));
			}

			rocketMatches.Add(DetonateRocket(rocket));

			if(rocket.boardX - offset.x >= 0 && rocket.boardY - offset.y >= 0){
				gem = board[rocket.boardX - (int)offset.x,rocket.boardY - (int)offset.y];
				gem.gemType = PZGem.GemType.ROCKET;
				gem.horizontal = rocket.horizontal;
				rocketMatches.Add(DetonateRocket(gem));
			}
			IncrementCombo(rocketMatches);

		} else { //else grenade-grenade swap
			int originx = destination.boardX;
			int originy = destination.boardY;
			PZMatch grenadeMatch = new PZMatch();
			grenadeMatch.special = true;

			if(originx + 1 < boardWidth){
				gem = board[originx + 1,originy];
				grenadeMatch = GetGrenadeMatch(gem);
			}
			if(originx - 1 >= 0){
				gem = board[originx - 1,originy];
				grenadeMatch.CheckAgainst(GetGrenadeMatch(gem));
			}

			if(originy + 1 < boardHeight){
				gem = board[originx,originy + 1];
				grenadeMatch.CheckAgainst(GetGrenadeMatch(gem));
			}
			if(originy - 1 >= 0){
				gem = board[originx,originy - 1];
				grenadeMatch.CheckAgainst(GetGrenadeMatch(gem));
			}
			source.gemType = PZGem.GemType.NORMAL;
			destination.gemType = PZGem.GemType.NORMAL;
			grenadeMatch.Destroy();
			IncrementCombo(new List<PZMatch>{ grenadeMatch });

			MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.gemExplode);
		}
	}
	
	public void DetonateMolotovFromSwap(PZGem molly, PZGem other)
	{
		if(!other.canComboWithMoltov && other.gemType != PZGem.GemType.NORMAL)
		{
			return;
		}
		if(molly != other && other.gemType == PZGem.GemType.MOLOTOV)
		{
			StartCoroutine(DetonateDoubleMoltov(molly, other));
			return;
		}
		else if(molly == other)
		{
			return;
		}

//		if (molly == other)
//		{
//			molly.colorIndex = UnityEngine.Random.Range(0, GEM_TYPES);
//		}

		List<PZMatch> matchList = new List<PZMatch>();
		PZMatch matching = GetMolotovGroup(molly, other.colorIndex);
		switch (other.gemType) {
		case PZGem.GemType.GRENADE:
			foreach (var item in matching.gems) 
			{
				item.gemType = PZGem.GemType.GRENADE;
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

	IEnumerator DetonateDoubleMoltov(PZGem source, PZGem destination)
	{
		PZMatch fullBoard = new PZMatch();

		foreach(PZGem gem in board)
		{
			gem.lockedBySpecial = true;
			fullBoard.gems.Add(gem);
		}

		specialBoardLock++;
		for(int i = 0; i < boardWidth; i++)
		{
			for(int j = 0; j < boardHeight; j++)
			{
//				Debug.Log("board["+i+","+j+"]");
				PZGem gem = board[i,j];
				if( gem!= null)
				{
					gem.lockedBySpecial = false;
					gem.Destroy(false);
					yield return new WaitForSeconds(0.05f);
				}
			}
		}
		specialBoardLock--;

		IncrementCombo(new List<PZMatch>{ fullBoard });
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
	
	public PZMatch GetGrenadeMatch(PZGem grenade)
	{
		MSPoolManager.instance.Get(MSPrefabList.instance.grenadeParticle, grenade.transf.position);

		List<PZGem> gems = new List<PZGem>();
		if (grenade.boardX > 0)
		{
			gems.Add(board[grenade.boardX-1, grenade.boardY]);
			if (grenade.boardY > 0)
			{
				gems.Add(board[grenade.boardX-1, grenade.boardY-1]);
			}
			if (grenade.boardY < boardHeight-1)
			{
				gems.Add(board[grenade.boardX-1, grenade.boardY+1]);
			}
		}
		if (grenade.boardX < boardWidth-1)
		{
			gems.Add(board[grenade.boardX+1, grenade.boardY]);
			if (grenade.boardY > 0)
			{
				gems.Add(board[grenade.boardX+1, grenade.boardY-1]);
			}
			if (grenade.boardY < boardHeight-1)
			{
				gems.Add(board[grenade.boardX+1, grenade.boardY+1]);
			}
		}
		if (grenade.boardY > 0)
		{
			gems.Add(board[grenade.boardX, grenade.boardY-1]);
		}
		if (grenade.boardY < boardHeight-1)
		{
			gems.Add(board[grenade.boardX, grenade.boardY+1]);
		}
		for (int i = 0; i < gems.Count; ) 
		{
			if (gems[i] == null)
			{
				gems.RemoveAt(i);
				continue;
			}
			CheckIfMolly(gems[i], grenade.colorIndex);
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
				if (target == null || target.gemType == PZGem.GemType.CAKE)
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
	bool CheckForMatchMoves(PZGem[,] board)
	{
		PZGem gem;
		hintgems.Clear();
		//Check horizontal possibilities
		for (int i = 0; i < boardHeight; i++) 
		{
			for (int j = 0; j < boardWidth-2; j++) 
			{
				gem = board[j, i];
				if (gem.gemType == PZGem.GemType.MOLOTOV)
				{
					if(j + 1 < boardWidth && board[j+1, i].canComboWithMoltov)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+1,i]);
					}
					else if(j - 1 > 0 && board[j-1, i].canComboWithMoltov)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j-1,i]);
					}
					else if(i + 1 < boardHeight && board[j,i+1].canComboWithMoltov)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j,i+1]);
					}
					else if(i - 1 > 0 && board[j,i-1].canComboWithMoltov)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j,i-1]);
					}
					//if we get this far there are no special gems around the moltov
					else if(j + 1 < boardWidth)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+1,i]);
					}
					else
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j-1,i]);
					}
					return true;
				}
				if (gem.colorIndex < 0) continue;
				if (j < boardWidth-3)
				{
					if (gem.colorIndex == board[j+2,i].colorIndex && gem.colorIndex == board[j+3,i].colorIndex)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+2,i]);
						hintgems.Add(board[j+3,i]);
						return true;
					}
					if (gem.colorIndex == board[j+1,i].colorIndex && gem.colorIndex == board[j+3,i].colorIndex)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+1,i]);
						hintgems.Add(board[j+3,i]);
						return true;
					}
				}
				if (i > 0)
				{
					if (gem.colorIndex == board[j+1,i].colorIndex && gem.colorIndex == board[j+2,i-1].colorIndex)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+1,i]);
						hintgems.Add(board[j+2,i-1]);
						return true;
					}
					if (gem.colorIndex == board[j+2,i].colorIndex && gem.colorIndex == board[j+1,i-1].colorIndex)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+2,i]);
						hintgems.Add(board[j+1,i-1]);
						return true;
					}
					if (gem.colorIndex == board[j+1,i-1].colorIndex && gem.colorIndex == board[j+2,i-1].colorIndex)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+1,i-1]);
						hintgems.Add(board[j+2,i-1]);
						return true;
					}
				}
				if (i < boardHeight-2)
				{
					if (gem.colorIndex == board[j+1,i].colorIndex && gem.colorIndex == board[j+2,i+1].colorIndex)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+1,i]);
						hintgems.Add(board[j+2,i+1]);
						return true;
					}
					if (gem.colorIndex == board[j+2,i].colorIndex && gem.colorIndex == board[j+1,i+1].colorIndex)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+2,i]);
						hintgems.Add(board[j+1,i+1]);
						return true;
					}
					if (gem.colorIndex == board[j+2,i+1].colorIndex && gem.colorIndex == board[j+1,i+1].colorIndex)
					{
						hintgems.Add(board[j,i]);
						hintgems.Add(board[j+2,i+1]);
						hintgems.Add(board[j+1,i+1]);
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
				if (gem.colorIndex < 0) continue;
				if (j < boardHeight-3)
				{
					if (gem.colorIndex == board[i,j+2].colorIndex && gem.colorIndex == board[i, j+3].colorIndex)
					{
						hintgems.Add(board[i,j]);
						hintgems.Add(board[i,j+2]);
						hintgems.Add(board[i,j+3]);
						return true;
					}
					if (gem.colorIndex == board[i, j+1].colorIndex && gem.colorIndex == board[i, j+3].colorIndex)
					{
						hintgems.Add(board[i,j]);
						hintgems.Add(board[i,j+1]);
						hintgems.Add(board[i,j+3]);
						return true;
					}
				}
				if (i > 0)
				{
					if (gem.colorIndex == board[i,j+1].colorIndex && gem.colorIndex == board[i-1,j+2].colorIndex)
					{
						hintgems.Add(board[i,j]);
						hintgems.Add(board[i,j+1]);
						hintgems.Add(board[i-1,j+2]);
						return true;
					}
					if (gem.colorIndex == board[i,j+2].colorIndex && gem.colorIndex == board[i-1,j+1].colorIndex)
					{
						hintgems.Add(board[i,j]);
						hintgems.Add(board[i,j+2]);
						hintgems.Add(board[i-1,j+1]);
						return true;
					}
					if (gem.colorIndex == board[i-1, j+1].colorIndex && gem.colorIndex == board[i-1, j+2].colorIndex)
					{
						hintgems.Add(board[i,j]);
						hintgems.Add(board[i-1,j+1]);
						hintgems.Add(board[i-1,j+2]);
						return true;
					}
				}
				if (i < boardHeight-2)
				{
					if (gem.colorIndex == board[i,j+1].colorIndex && gem.colorIndex == board[i+1,j+2].colorIndex)
					{
						hintgems.Add(board[i,j]);
						hintgems.Add(board[i,j+1]);
						hintgems.Add(board[i+1,j+2]);
						return true;
					}
					if (gem.colorIndex == board[i,j+2].colorIndex && gem.colorIndex == board[i+1,j+1].colorIndex)
					{
						hintgems.Add(board[i,j]);
						hintgems.Add(board[i,j+2]);
						hintgems.Add(board[i+1,j+1]);
						return true;
					}
					if (gem.colorIndex == board[i+1,j+1].colorIndex && gem.colorIndex == board[i+1,j+2].colorIndex)
					{
						hintgems.Add(board[i,j]);
						hintgems.Add(board[i+1,j+1]);
						hintgems.Add(board[i+1,j+2]);
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

	public void BlockBoard(List<PZGem> exceptGems, float tweenTime = 0)
	{
		for (int i = 0; i < boardWidth; i++) 
		{
			for (int j = 0; j < boardHeight; j++) 
			{
				if (exceptGems.Find(x=>x.boardX==i&&x.boardY==j)==null)
				{
					board[i,j].Block(tweenTime);
				}
			}
		}
	}

	public void BlockBoard(List<Vector2> exceptSpaces = null, float tweenTime = 0)
	{
		for (int i = 0; i < boardWidth; i++) 
		{
			for (int j = 0; j < boardHeight; j++) 
			{
				if (exceptSpaces == null || !exceptSpaces.Contains(new Vector2(i,j)))
				{
					board[i,j].Block(tweenTime);
				}
			}
		}
	}

	public void UnblockBoard()
	{
		for (int i = 0; i < boardWidth; i++) 
		{
			for (int j = 0; j < boardHeight; j++) 
			{
				board[i,j].Unblock();
			}
		}
	}

	public void UnblockBoard(List<Vector2> spaces)
	{
		for (int i = 0; i < boardWidth; i++) 
		{
			for (int j = 0; j < boardHeight; j++) 
			{
				if (spaces.Contains(new Vector2(i,j)))
				{
					board[i,j].Unblock();
				}
			}
		}
	}

	public void BoardSnapshot()
	{
		lastBoardSnapshot = new GemSaveData[boardWidth, boardHeight];
		foreach (var item in board) 
		{
			lastBoardSnapshot[item.boardX, item.boardY] = new GemSaveData(item);
		}
	}

	#region Hints

	[ContextMenu ("start hint")]
	public void StartHint()
	{
		showGemHints = true;
	}

	[ContextMenu ("stop hint")]
	public void StopHint()
	{
		showGemHints = false;
	}

	/// <summary>
	/// inturupts any currently playing hint animation to play
	/// hint animation on the first 3 gem locations given
	/// </summary>
	/// <param name="gems">List of Board locations to hint</param>
	public void CustomHintGems(List<Vector2> gems)
	{
		showGemHints = false;
		_gemHints = true;
		hintgems.Clear();
		foreach(Vector2 gemPos in gems)
		{
			if(gemPos.x > boardWidth || gemPos.x < 0 || gemPos.y > boardHeight || gemPos.y < 0)
			{
				Debug.LogError("invalid gem position was given in CustomHintGems. Position: ["+gemPos.x+","+gemPos.y+"].");
			}
			else
			{
				hintgems.Add(board[(int)gemPos.x, (int)gemPos.y]);
			}
		}
		StartCoroutine(hintCycle);
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

	void CheckStartHintGems(int turnsLeft)
	{
		if(turnsLeft > 0)
		{
			showGemHints = true;
		}
		else
		{
			showGemHints = false;
		}
	}

	#endregion

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
				///Reasons this happened: checkFall was called on gems top to bottom, instead of the
				/// correct direction of bottom to top.  It could also be cause by  gems being locked in
				/// place by one of the many lock mechanisms we use.
				///This error will print once for every collumn for that did not fall correctly.
				Debug.LogError("Gems could not drop into board");
			}
		}
	}

	bool HasMatches(PZGem[,] thisBoard)
	{
		List<PZGem> currGems = new List<PZGem>();
		PZGem curr;

		//Determine horizontal matches
		for (int i = 0; i < boardWidth; i++) 
		{
			currGems.Add(thisBoard[i, 0]);
			for (int j = 1; j < boardHeight; j++) 
			{
				curr = thisBoard[i,j];
				
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
						return true;
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
				return true;
			}
			
			currGems.Clear ();
		}
		
		for (int i = 0; i < boardHeight; i++) {
			currGems.Add (thisBoard[0,i]);
			for (int j = 1; j < boardWidth; j++) {
				curr = thisBoard[j,i];
				if (currGems[0].colorIndex != curr.colorIndex)
				{
					if (currGems.Count >= 3)
					{
						foreach (PZGem item in currGems)
						{
							if (item.gemType != PZGem.GemType.ROCKET)
							{
								item.horizontal = false;
							}
						}
						return true;
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
				return true;
			}
			currGems.Clear ();
		}

		return false;
	}

	[SerializeField] float shuffleTime;
	[SerializeField] MSUIHelper shuffleLabel;

	[ContextMenu ("Test Shuffle")]
	public void Shuffle()
	{
		StartCoroutine(Suffle());
	}

	public IEnumerator Suffle()
	{
		swapLock++;
		//Debug.LogWarning("Shuffle Lock");

		List<PZGem> gems = new List<PZGem>(); //This is our list of target positions. As we pick one, it gets removed from the list.

		PZGem[,] newBoard = new PZGem[boardWidth, boardHeight];

		int x, y;

		shuffleLabel.FadeIn();
		PZCombatManager.instance.boardTint.FadeIn();

		do
		{
			gems.Clear();
			for (x = 0; x < boardWidth; x++) {
				for (y = 0; y < boardHeight; y++) 
				{
					gems.Add(board[x,y]);
				}
			}

			for (x = 0; x < boardWidth; x++) 
			{
				for (y = 0; y < boardHeight; y++) 
				{
					PZGem currGem = board[x,y];
					if (currGem.gemType == PZGem.GemType.CAKE)
					{
						currGem.SetShufflePosition(currGem.boardX, currGem.boardY);
						gems.Remove(currGem);
						newBoard[currGem.boardX, currGem.boardY] = currGem;
					}
					else
					{
						PZGem target;
						do
						{
							target = gems[UnityEngine.Random.Range(0, gems.Count)];
						} while ((gems.Count > 1  //If this is the last gem, and its target is itself, we've got to let it pass
						         && target == currGem )
						         || target.gemType == PZGem.GemType.CAKE);
						gems.Remove(target);
						currGem.SetShufflePosition(target.boardX, target.boardY);
						newBoard[target.boardX, target.boardY] = currGem;
					}
				}
			}
		}while(HasMatches(newBoard) || !CheckForMatchMoves(newBoard));

		float currTime = 0;
		do
		{
			currTime += Time.deltaTime;
			for (x = 0; x < boardWidth; x++) 
			{
				for (y = 0; y < boardHeight; y++) 
				{
					board[x,y].SetShuffleProgress(currTime/shuffleTime);
				}
			}
			yield return null;
		}while (currTime < shuffleTime);

		shuffleLabel.FadeOut();
		PZCombatManager.instance.boardTint.FadeOutAndOff();

		board = newBoard;

		swapLock--;
	}

	void PlaceRandomly(PZGem gem, PZGem[,] board)
	{
		if (gem.gemType == PZGem.GemType.CAKE)
		{
			PZGem temp = board[gem.boardX, gem.boardY];
			board[gem.boardX, gem.boardY] = gem;
			if (gem != null) PlaceRandomly(gem, board);
		}

		int boardX = UnityEngine.Random.Range(0, boardWidth);
		int boardY = UnityEngine.Random.Range(0, boardHeight);
		if (board[boardX, boardY] == null)
		{
			board[boardX, boardY] = gem;
			gem.SetShufflePosition(boardX, boardY);
		}
	}

	#region Jelly

	public bool ClearJelly(int x, int y, int gemId)
	{
		if (x < 0 || y < 0 || x >= boardWidth || y >= boardHeight)
		{
			Debug.LogError("Trying to clear jelly on: " + x + ", " + y + " not possible");
			return false;
		}
		if (jellyBoard[x,y] != null)
		{
			jellyBoard[x,y].Damage();
			//Debug.Log(gemId + " damaged jelly at " + x + ", " + y);
			return true;
		}
		//Debug.Log("No jelly for " + gemId + " at " + x + ", " + y);
		return false;
	}

	public List<Vector2> SpawnJellies(int num)
	{
		List<Vector2> jellyPoses = new List<Vector2>();
		for (int i = 0; i < num; i++) 
		{
			jellyPoses.Add(SpawnJelly());
		}
		return jellyPoses;
	}

	[ContextMenu ("Throw Jelly")]
	public Vector2 SpawnJelly()
	{
		Vector2 jellyBoardPos = FindJellyPosition();
		if (jellyBoardPos.x > -1 && jellyBoardPos.y > -1)
		{
			int boardX = (int)jellyBoardPos.x;
			int boardY = (int)jellyBoardPos.y;

			//Init Jelly
			PZJelly jelly = MSPoolManager.instance.Get<PZJelly>(jellyPrefab, puzzleParent);
			jelly.InitOnBoard(boardX, boardY);
			jellyBoard[boardX, boardY] = jelly;
		}
		else
		{
			Debug.LogWarning("Not enough room for jelly!");
		}
		return jellyBoardPos;
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

	#region Cake

	public void SetupForCakes(SkillProto skill)
	{
		maxCakes = (int)skill.properties.Find(x=>x.name == "MAX_CAKES").skillValue;
		currCakeChance = skill.properties.Find (x=>x.name == "CAKE_CHANCE").skillValue;
	}

	public Coroutine BakeCake()
	{
		return StartCoroutine(DoBakeCake());
	}

	IEnumerator DoBakeCake()
	{
		int boardX = UnityEngine.Random.Range(0, boardWidth);
		PZGem gem = board[boardX, boardHeight-1];
		cakes.Add(gem);
		yield return StartCoroutine(gem.TweenUnblock(.5f));
		yield return new WaitForSeconds(.5f);
		yield return gem.RunBecomeCake();
	}

	public void ResetCakes()
	{
		foreach (var item in cakes) 
		{
			item.RunRevertFromCake();
		}
		cakes.Clear();
		maxCakes = 0;
		currCakeChance = 0;
	}

	#endregion

	#region Bombs

	public List<PZGem> PickBombs(int colorIndex, int numBombs)
	{
		List<PZGem> bombs = new List<PZGem>();
		//Add all gems of that color
		foreach (var item in board) 
		{
			if (item.colorIndex == colorIndex) bombs.Add(item);
		}
		//While we have more gems than we need, trim the list randomly
		while(bombs.Count > numBombs)
		{
			int index = UnityEngine.Random.Range(0, bombs.Count);
			bombs.RemoveAt(index);
		}
		return bombs;
	}

	/// <summary>
	/// Checks a gem to see if it should spawn as a bomb.
	/// Called by a gem that is currently respawning, after it has picked its new color.
	/// </summary>
	/// <returns><c>true</c>, if spawn bomb was checked, <c>false</c> otherwise.</returns>
	/// <param name="gem">Gem.</param>
	public bool CheckSpawnBomb(PZGem gem)
	{
		if (!canSpawnBombs 
		    || gem.colorIndex != bombColor 
		    || PZCombatManager.instance.bombs.Count >= maxBombs)
		{
			return false;
		}

//		Debug.Log("Checking if new gem should be a bomb...");

		if (PZCombatManager.instance.bombs.Count < minBombs)
		{
			return true;
		}

		float roll = UnityEngine.Random.value;
//		Debug.Log("Target: " + bombChance + ", Roll: " + roll);

		return roll < bombChance;
	}

	#endregion

	#region Poison

	public List<PZGem> PickPoisons(int colorIndex)
	{
		List<PZGem> poisons = new List<PZGem>();
		//Add all gems of that color
		foreach (var item in board) 
		{
			if (item.colorIndex == colorIndex && item.gemType == PZGem.GemType.NORMAL) poisons.Add(item);
		}

		return poisons;
	}

	#endregion Poison

}
