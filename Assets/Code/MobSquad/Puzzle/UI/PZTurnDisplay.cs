using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// PZTurnDisplay
/// @author Rob Giusti
/// </summary>
public class PZTurnDisplay : MonoBehaviour 
{
	public static PZTurnDisplay instance;

	public TweenPosition moveInTween;

	[SerializeField] PZTurnIcon iconPrefab;

	Vector3 newIconPosition
	{
		get
		{
			return new Vector3(background.width + 150, 0, 0);
		}
	}

	[SerializeField] int numTurnsToDisplay;

	[SerializeField] int pixelsPerTurn;

	[SerializeField] UIGrid turnGrid;

	[SerializeField] Transform currentBorder;

	[SerializeField] float timeBetweenFlips;

	[SerializeField] float jumpTargetY;

	[SerializeField] float timeForJump;

	[SerializeField] AnimationCurve animationCurve;

	[SerializeField] UISprite background;

	List<PZTurnIcon> icons = new List<PZTurnIcon>();

	PZTurnIcon currentIcon;

	bool _isItIn = false;

	public bool isItIn
	{
		get
		{
			return _isItIn;
		}
	}

	void Awake()
	{
		instance = this;
	}

	void SetSize()
	{
		if (MSUtil.screenRatio > 1.5f)
		{
			numTurnsToDisplay = 4;
		}
		else
		{
			numTurnsToDisplay = 3;
		}
		numTurnsToDisplay += 8 - PZPuzzleManager.instance.boardWidth;
		numTurnsToDisplay = Mathf.Min(numTurnsToDisplay, 5);
		background.width = numTurnsToDisplay * pixelsPerTurn + 10;
	}

	public Coroutine RunInit(PZMonster player, PZMonster enemy)
	{
		return StartCoroutine(Init (player, enemy));
	}

	IEnumerator Init(PZMonster player, PZMonster enemy)
	{
		SetSize();
		turnGrid.onCustomSort = MSNaturalSortObject.Compare;
		if (_isItIn)
		{
			yield return RunInitAlreadyIn();
		}
		else
		{
			yield return RunInitAndMoveIn();
		}
	}

	Coroutine RunInitAndMoveIn()
	{
		return StartCoroutine(InitAndMoveIn());
	}

	IEnumerator InitAndMoveIn()
	{
		moveInTween.PlayForward();

		for (int i = 0; i < numTurnsToDisplay; i++) 
		{
			AddIcon(i);
		}

		currentIcon = icons[0];

		turnGrid.Reposition();

		_isItIn = true;

		while(moveInTween.tweenFactor < 1)
		{
			yield return null;
		}

		yield return RunMakeCurrentIconJump();
	}

	Coroutine RunInitAlreadyIn()
	{
		return StartCoroutine(InitAlreadyIn());
	}

	IEnumerator InitAlreadyIn()
	{
		for (int i = 0; i < numTurnsToDisplay; i++) 
		{
			icons[i].RunFlip(PZCombatScheduler.instance.GetNthMove(i) == CombatTurn.ENEMY);
			icons[i].name = (i+1).ToString();
			yield return new WaitForSeconds(timeBetweenFlips);
		}
	}

	public Coroutine RunOnNextTurn()
	{
		return StartCoroutine(OnNextTurn());
	}

	IEnumerator OnNextTurn()
	{
		currentIcon.Leave();
		icons.RemoveAt(0);

		currentIcon = icons[0];

		AddIcon(numTurnsToDisplay-1);

		turnGrid.Reposition();

		bool moving = true;
		currentIcon.GetComponent<SpringPosition>().onFinished = delegate { moving = false;};
		while(moving)
		{
			yield return null;
		}

		yield return RunMakeCurrentIconJump();
	}

	void AddIcon(int turnsFromNow)
	{
		PZTurnIcon icon = MSPoolManager.instance.Get<PZTurnIcon>(iconPrefab, turnGrid.transform);
		icon.transform.localScale = new Vector3(-1, 1, 1);
		icon.transform.localPosition = newIconPosition;
		icon.Init(PZCombatScheduler.instance.GetNthMove(turnsFromNow) == CombatTurn.ENEMY);
		icon.name = (PZCombatScheduler.instance.currInd + turnsFromNow).ToString();
		icons.Add (icon);
	}

	public void DoMoveOut()
	{
		StartCoroutine(MoveOut());
	}

	IEnumerator MoveOut()
	{
		moveInTween.PlayReverse();
		_isItIn = false;
		while (moveInTween.tweenFactor > 0)
		{
			yield return null;
		}
		Recycle();
	}

	void OnCity()
	{
		moveInTween.Sample(0, true);
		_isItIn = false;
		Recycle();
	}

	Coroutine RunMakeCurrentIconJump()
	{
		return StartCoroutine(MakeCurrentIconJump());
	}

	IEnumerator MakeCurrentIconJump()
	{
		currentBorder.parent = currentIcon.transform;
		currentBorder.transform.localPosition = Vector3.zero;

		TweenPosition tween = TweenPosition.Begin(currentIcon.gameObject, timeForJump, new Vector3(0, jumpTargetY));
		tween.from = Vector3.zero;
		tween.animationCurve = animationCurve;

		while (tween.tweenFactor < 1)
		{
			yield return null;
		}

		currentBorder.parent = currentBorder.parent.parent.parent;
	}

	void Recycle()
	{
		foreach (var item in icons) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}
		icons.Clear();
	}
}
