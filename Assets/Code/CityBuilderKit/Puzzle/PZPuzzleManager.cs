using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PZPuzzleManager : MonoBehaviour {
	
	public static PZPuzzleManager instance;
	
	[SerializeField]
	UILabel comboLabel;
	
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
			}
			else
			{
				comboLabel.text = "";
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
	
	public int[] gemsOnBoardByType = new int[GEM_TYPES];
	
	public int gemsByType;
	
	public const int BOARD_WIDTH = 8;
	public const int BOARD_HEIGHT = 8;
	
	public const int GEM_TYPES = 5;
	
	public const float WAIT_BETWEEN_LINES = .3f;
	
	bool setUpBoard = false;

	[SerializeField]
	public PZDamageNumber damageNumberPrefab;

	int currTurn = 0;

	int playerTurns = 3;
	
	public void Awake()
	{
		instance = this;
		
		board = new PZGem[BOARD_WIDTH, BOARD_HEIGHT];
		
		currGems = new int[gemTypes.Length];
		
		ResetCombo();
		
		setUpBoard = false;
		
		swapLock = 0;
	}
	
	void OnEnable()
	{
		CBKEventManager.Puzzle.OnDeploy += OnDeploy;
	}
	
	void OnDisable()
	{
		CBKEventManager.Puzzle.OnDeploy -= OnDeploy;
	}
	
	void OnDeploy(PZMonster monster)
	{
		if (!setUpBoard)
		{
			StartCoroutine(InitBoard());
			setUpBoard = true;
		}
	}
	
	public void ResetCombo()
	{
		combo = 0;
		for (int i = 0; i < currGems.Length; i++) {
			currGems[i] = 0;
		}
	}
	
	public IEnumerator InitBoard()
	{
		ClearBoard();
		PZGem gem;
		for (int i = 0; i < BOARD_HEIGHT; i++) 
		{
			for (int j = 0; j < BOARD_WIDTH; j++) 
			{
				gem = CBKPoolManager.instance.Get(gemPrefab, Vector3.zero) as PZGem;
				gem.transf.parent = puzzleParent;
				gem.Init(PickColor(i, j), j);
			}
			yield return new WaitForSeconds(WAIT_BETWEEN_LINES);
		}
	}

	public void ClearBoard ()
	{
		if (board[0,0] != null)
		{
			for (int i = 0; i < BOARD_HEIGHT; i++) 
			{
				for (int j = 0; j < BOARD_WIDTH; j++) 
				{
					board[j,i].Pool();
					board[j,i] = null;
				}
			}
		}
		for (int i = 0; i < GEM_TYPES; i++) 
		{
			gemsOnBoardByType[i] = 0;
		}
	}
	
	public void BreakTiles(List<PZGem> match)
	{
		List<int> columns = new List<int>();
		foreach (PZGem item in match) 
		{
			board[item.boardX, item.boardY] = null;
			if (!columns.Contains(item.boardX))
			{
				columns.Add(item.boardX);
			}
		}
		foreach (int item in columns)
		{
			for (int i = 0; i < BOARD_HEIGHT; i++) 
			{
				if (board[item,i] != null)
				{
					board[item,i].CheckFall();
				}
			}
		}
		foreach (PZGem item in match) 
		{
			item.Init(PickColor(), item.boardX);
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
	public int PickColor(int row = 0, int col = 0)
	{
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
			picked = Random.Range(0,gemTypes.Length);
			if (picked != rowCol && picked != colCol)
			{
				break;
			}
		}
		gemsOnBoardByType[picked]++;
		return picked;
	}
	
	public void OnStartMoving(PZGem gem)
	{
		//Debug.Log("Lock");
		swapLock += 1;
		movingGems.Add(gem);
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
		
		//Debug.Log(movingGems.Count);
		if (movingGems.Count == 0)
		{
			CheckWholeBoard();
			gemsToCheck.Clear();
			if (movingGems.Count == 0)
			{
				if (combo == 0)
				{
					lastSwapSuccessful = false;
				}
				else
				{
					lastSwapSuccessful = true;
					PZCombatManager.instance.OnBreakGems(currGems, combo);
				}
				processingSwap = false;
				
				ResetCombo();
				
				Debug.Log("Board has matches: " + CheckForMatchMoves());
			}
			processingSwap = false;
		}
		
		//After the board is checked, if nothing is falling, unlock the board
		
	}

	void GetMatchesOnBoard (List<PZMatch> matchList)
	{
		List<PZGem> currGems = new List<PZGem>();
		PZGem curr;
		
		//Determine horizontal matches
		for (int i = 0; i < BOARD_WIDTH; i++) 
		{
			currGems.Add(board[i,0]);
			for (int j = 1; j < BOARD_HEIGHT; j++) 
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
		
		for (int i = 0; i < BOARD_HEIGHT; i++) {
			currGems.Add (board[0,i]);
			for (int j = 1; j < BOARD_WIDTH; j++) {
				curr = board[j,i];
				if (currGems[0].colorIndex != curr.colorIndex)
				{
					if (currGems.Count >= 3)
					{
						foreach (PZGem item in currGems)
						{
							if (item.gemType != PZGem.GemType.ROCKET)
							{
								item.horizontal = true;
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
						item.horizontal = true;
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
			if (match.special)
			{
				continue;
			}
			foreach (PZMatch other in matchList)
			{
				if (match != other && !other.special)
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
						break;
					case PZGem.GemType.MOLOTOV:
						match = GetMolotovGroup(gem, gem.colorIndex);
						break;
					case PZGem.GemType.ROCKET:
						match = GetRocketMatch(gem);
						break;
					default:
						match = null; //Fuck fuck fuck fuck fuck
						break;
					}
					RemoveRepeats(match, matchList);
					matchList.Add(match);
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
	
	public void DetonateMolotovFromSwap(PZGem molly, int colorIndex)
	{
		List<PZMatch> matchList = new List<PZMatch>();
		matchList.Add(GetMolotovGroup(molly, colorIndex));
		molly.gemType = PZGem.GemType.NORMAL;
		
		DetonateSpecialsInMatches(matchList);
		DestroyMatches(matchList);
		IncrementCombo(matchList);
	}
	
	PZMatch GetMolotovGroup(PZGem molly, int colorIndex)
	{
		List<PZGem> gems = new List<PZGem>();
		for (int i = 0; i < BOARD_WIDTH; i++) {
			for (int j = 0; j < BOARD_HEIGHT; j++) {
				if (board[i,j] != null && (board[i,j] == molly || board[i,j].colorIndex == colorIndex))
				{
					gems.Add(board[i,j]);
				}
			}
		}
		return new PZMatch(gems, true);
	}
	
	PZMatch GetBombMatch(PZGem bomb)
	{
		List<PZGem> gems = new List<PZGem>();
		if (bomb.boardX > 0)
		{
			gems.Add(board[bomb.boardX-1, bomb.boardY]);
			if (bomb.boardY > 0)
			{
				gems.Add(board[bomb.boardX-1, bomb.boardY-1]);
			}
			if (bomb.boardY < BOARD_HEIGHT-1)
			{
				gems.Add(board[bomb.boardX-1, bomb.boardY+1]);
			}
		}
		if (bomb.boardX < BOARD_WIDTH-1)
		{
			gems.Add(board[bomb.boardX+1, bomb.boardY]);
			if (bomb.boardY > 0)
			{
				gems.Add(board[bomb.boardX+1, bomb.boardY-1]);
			}
			if (bomb.boardY < BOARD_HEIGHT-1)
			{
				gems.Add(board[bomb.boardX+1, bomb.boardY+1]);
			}
		}
		if (bomb.boardY > 0)
		{
			gems.Add(board[bomb.boardX, bomb.boardY-1]);
		}
		if (bomb.boardY < BOARD_HEIGHT-1)
		{
			gems.Add(board[bomb.boardX, bomb.boardY+1]);
		}
		foreach (PZGem item in gems) {
			CheckIfMolly(item, bomb.colorIndex);
		}
		return new PZMatch(gems, true);
	}
	
	PZMatch GetRocketMatch(PZGem rocket)
	{
		List<PZGem> gems = new List<PZGem>();
		if (rocket.horizontal)
		{
			for (int i = 0; i < BOARD_WIDTH; i++) 
			{
				gems.Add(board[i, rocket.boardY]);
				CheckIfMolly(board[i, rocket.boardY], rocket.colorIndex);
			}
		}
		else
		{
			for (int i = 0; i < BOARD_HEIGHT; i++) {
				gems.Add (board[rocket.boardX, i]);
				CheckIfMolly(board[rocket.boardX,i], rocket.colorIndex);
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
		for (int i = 0; i < BOARD_HEIGHT; i++) 
		{
			for (int j = 0; j < BOARD_WIDTH-2; j++) 
			{
				gem = board[j, i];
				if (j < BOARD_WIDTH-3)
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
				if (i < BOARD_HEIGHT-1)
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
		for (int i = 0; i < BOARD_WIDTH; i++) 
		{
			for (int j = 0; j < BOARD_HEIGHT-2; j++) 
			{
				gem = board[i,j];
				if (gem.gemType == PZGem.GemType.MOLOTOV)
				{
					return true;
				}
				if (j < BOARD_HEIGHT-3)
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
				if (i < BOARD_HEIGHT-1)
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
}
