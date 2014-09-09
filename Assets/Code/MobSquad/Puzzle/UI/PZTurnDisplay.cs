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
	[SerializeField] TweenPosition moveInTween;

	[SerializeField] PZTurnIcon iconPrefab;

	[SerializeField] Vector3 newIconPosition;

	[SerializeField] int numTurnsToDisplay;

	[SerializeField] UIGrid turnGrid;

	[SerializeField] Transform currentBorder;

	[SerializeField] float timeBetweenFlips;

	[SerializeField] float jumpTargetY;

	[SerializeField] float timeForJump;

	[SerializeField] AnimationCurve animationCurve;

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

	PZMonster player, enemy;

	public Coroutine RunInit(PZMonster player, PZMonster enemy)
	{
		return StartCoroutine(Init (player, enemy));
	}

	IEnumerator Init(PZMonster player, PZMonster enemy)
	{
		this.player = player;
		this.enemy = enemy;
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
			icons[i].RunFlip(PZCombatScheduler.instance.GetNthMove(i) == CombatTurn.PLAYER ? 
			                player : enemy);
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
		icon.transform.localScale = Vector3.one;
		icon.transform.localPosition = newIconPosition;
		icon.Init(PZCombatScheduler.instance.GetNthMove(turnsFromNow) == CombatTurn.PLAYER ? player : enemy);
		icon.name = turnsFromNow.ToString();
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
