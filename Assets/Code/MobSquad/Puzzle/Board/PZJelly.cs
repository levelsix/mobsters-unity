using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MSSimplePoolable))]
[RequireComponent (typeof (TweenScale))]
public class PZJelly : MonoBehaviour {

	int boardX, boardY;

	TweenScale _tweenScale;
	TweenScale tweenScale
	{
		get
		{
			if (_tweenScale == null)
			{
				_tweenScale = GetComponent<TweenScale>();
			}
			return _tweenScale;
		}
	}

	public void InitOnBoard(int boardX, int boardY)
	{
		transform.localPosition = new Vector3(boardX * PZGem.SPACE_SIZE, boardY * PZGem.SPACE_SIZE);
		tweenScale.ResetToBeginning();
		tweenScale.PlayForward();
		this.boardX = boardX;
		this.boardY = boardY;
	}

	public void Damage()
	{
		StartCoroutine(Destroy());
	}

	IEnumerator Destroy()
	{
		tweenScale.PlayReverse();
		PZPuzzleManager.instance.jellyBoard[boardX, boardY] = null;

		while (tweenScale.tweenFactor > 0)
		{
			yield return null;
		}

		Pool();
	}
	
	public void Pool()
	{
		GetComponent<MSSimplePoolable>().Pool();
	}
}
