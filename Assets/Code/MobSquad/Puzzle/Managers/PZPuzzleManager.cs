using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class PZPuzzleManager : MonoBehaviour {
	
	public static PZPuzzleManager instance;

	[SerializeField]
	public Camera puzzleCamera;

	[SerializeField]
	UISprite boardSprite;

	[SerializeField]
	UILabel comboLabel;

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
	
	[SerializeField]
	PZGem gemPrefab;
	
	public PZGem[,] board;
	
	[SerializeField]
	public string[] gemTypes;
	
	public int[] currGems;

	//public int[] gemsOnBoardByType = new int[GEM_TYPES];

	public List<PZGem>[] columnQueues = new List<PZGem>[STANDARD_BOARD_HEIGHT];

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

	public float BASE_FALL_SPEED = -250;
	
	public float BASE_BOUNCE_SPEED = 80;
	
	public float SECOND_BOUNCE_MODIFIER = .25f;
	
	public float GRAVITY = -600f;
	
	public float SWAP_TIME = .2f;

	public void Awake()
	{
		instance = this;
		
		board = new PZGem[boardWidth, boardHeight];
		
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

	public void InitBoard(int w = STANDARD_BOARD_WIDTH, int h = STANDARD_BOARD_HEIGHT, string boardFile = "")
	{
		//Make sure we clear the board with the old values for width and height!
		ClearBoard();

		boardWidth = w;
		boardHeight = h;

		board = new PZGem[w, h];

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
		
		setUpBoard = true;
	}

	void RigBoard(string boardFile)
	{
		try
		{
			string fileText = File.ReadAllText(boardFile);
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
				ResetCombo ();
				if (!CheckForMatchMoves ()) {
					InitBoard ();
				}
//				Debug.Log ("Board has matches: " + CheckForMatchMoves ());
			}
			processingSwap = false;
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
			PZRocket rocket = (MSPoolManager.instance.Get(rocketPrefab.GetComponent<MSSimplePoolable>(), gem.transf.position) as MonoBehaviour).GetComponent<PZRocket>();
			rocket.Init(MSValues.Direction.EAST);
			rocket.trans.parent = puzzleParent;
			rocket.trans.localRotation = Quaternion.identity;
			rocket.trans.localScale = Vector3.one;
			rocket.trans.localPosition = gem.transf.localPosition;

			rocket = (MSPoolManager.instance.Get(rocketPrefab.GetComponent<MSSimplePoolable>(), gem.transf.position) as MonoBehaviour).GetComponent<PZRocket>();
			rocket.Init(MSValues.Direction.WEST);
			rocket.trans.parent = puzzleParent;
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
			PZRocket rocket = (MSPoolManager.instance.Get(rocketPrefab.GetComponent<MSSimplePoolable>(), gem.transf.position) as MonoBehaviour).GetComponent<PZRocket>();
			rocket.Init(MSValues.Direction.NORTH);
			rocket.trans.parent = puzzleParent;
			rocket.trans.localRotation = new Quaternion(0,0,.707f,.707f);
			rocket.trans.localScale = Vector3.one;
			rocket.trans.localPosition = gem.transf.localPosition;

			rocket = (MSPoolManager.instance.Get(rocketPrefab.GetComponent<MSSimplePoolable>(), gem.transf.position) as MonoBehaviour).GetComponent<PZRocket>();
			rocket.Init(MSValues.Direction.SOUTH);
			rocket.trans.parent = puzzleParent;
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
						return true;
					}
					if (gem.colorIndex == board[j+1,i].colorIndex && gem.colorIndex == board[j+3,i].colorIndex)
					{
						return true;
					}
				}
				if (i > 0)
				{
					if (gem.colorIndex == board[j+1,i].colorIndex && gem.colorIndex == board[j+2,i-1].colorIndex)
					{
						return true;
					}
					if (gem.colorIndex == board[j+2,i].colorIndex && gem.colorIndex == board[j+1,i-1].colorIndex)
					{
						return true;
					}
					if (gem.colorIndex == board[j+1,i-1].colorIndex && gem.colorIndex == board[j+2,i-1].colorIndex)
					{
						return true;
					}
				}
				if (i < boardHeight-1)
				{
					if (gem.colorIndex == board[j+1,i].colorIndex && gem.colorIndex == board[j+2,i+1].colorIndex)
					{
						return true;
					}
					if (gem.colorIndex == board[j+2,i].colorIndex && gem.colorIndex == board[j+1,i+1].colorIndex)
					{
						return true;
					}
					if (gem.colorIndex == board[j+2,i+1].colorIndex && gem.colorIndex == board[j+1,i+1].colorIndex)
					{
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
						return true;
					}
					if (gem.colorIndex == board[i, j+1].colorIndex && gem.colorIndex == board[i, j+3].colorIndex)
					{
						return true;
					}
				}
				if (i > 0)
				{
					if (gem.colorIndex == board[i,j+1].colorIndex && gem.colorIndex == board[i-1,j+2].colorIndex)
					{
						return true;
					}
					if (gem.colorIndex == board[i,j+2].colorIndex && gem.colorIndex == board[i-1,j+1].colorIndex)
					{
						return true;
					}
					if (gem.colorIndex == board[i-1, j+1].colorIndex && gem.colorIndex == board[i-1, j+2].colorIndex)
					{
						return true;
					}
				}
				if (i < boardHeight-1)
				{
					if (gem.colorIndex == board[i,j+1].colorIndex && gem.colorIndex == board[i+1,j+2].colorIndex)
					{
						return true;
					}
					if (gem.colorIndex == board[i,j+2].colorIndex && gem.colorIndex == board[i+1,j+1].colorIndex)
					{
						return true;
					}
					if (gem.colorIndex == board[i+1,j+1].colorIndex && gem.colorIndex == board[i+1,j+2].colorIndex)
					{
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

	[ContextMenu ("Print board")]
	public void PrintBoard()
	{
		string str = "Board";
		for (int i = boardHeight-1; i >= 0; i--) 
		{
			str += "\n";
			for (int j = 0; j < boardWidth; j++) 
			{
				if (board[j,i] == null)
				{
					str += "e ";
				}
				else if (board[j,i].colorIndex < 0)
				{
					str += "n ";
				}
				else
				{
					str += board[j,i].colorIndex + " ";
				}
			}
		}
		Debug.Log(str);
	}
}
