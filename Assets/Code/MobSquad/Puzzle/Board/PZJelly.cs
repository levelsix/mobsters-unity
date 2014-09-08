using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PZMoveTowards))]
[RequireComponent (typeof (MSSimplePoolable))]
[RequireComponent (typeof (TweenScale))]
public class PZJelly : MonoBehaviour {

	[SerializeField] UISprite fullSprite;
	[SerializeField] UISprite leftSprite;
	[SerializeField] UISprite rightSprite;

	[SerializeField] Vector3 throwDir;

	[SerializeField] float fallSideVel;
	[SerializeField] float fallAccel;

	[SerializeField] float startRotatVel;
	[SerializeField] float rotateAccel;

	int boardX, boardY;

	const int STARTING_HEALTH = 1;

	int _health = 0;
	int health
	{
		get
		{
			return _health;
		}
		set
		{
			_health = value;
			switch (_health)
			{
			case STARTING_HEALTH:
				fullSprite.alpha = 1;
				leftSprite.alpha = rightSprite.alpha = 0;
				break;
			case 0:
				StartCoroutine(Destroy());
				break;
			default:
				break;
			}
		}
	}

	PZMoveTowards _moveTowards;
	PZMoveTowards moveTowards
	{
		get
		{
			if (_moveTowards == null)
			{
				_moveTowards = GetComponent<PZMoveTowards>();
			}
			return _moveTowards;
		}
	}

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

	/// <summary>
	/// Init the specified inPos, outPos, boardX and boardY.
	/// </summary>
	/// <param name="inPos">In position, in World Coordinates</param>
	/// <param name="outPos">Out position, in World Coordinates.</param>
	/// <param name="boardX">Board x.</param>
	/// <param name="boardY">Board y.</param>
	public void Init(Vector3 inPos, int boardX, int boardY)
	{
		rightSprite.transform.localPosition = leftSprite.transform.localPosition = Vector3.zero;

		this.boardX = boardX;
		this.boardY = boardY;

		health = STARTING_HEALTH;

		transform.position = transform.InverseTransformPoint(inPos);

		tweenScale.ResetToBeginning();
		tweenScale.PlayForward();

		moveTowards.RunMoveTowards(new Vector3(boardX * PZGem.SPACE_SIZE, boardY * PZGem.SPACE_SIZE), throwDir);
	}

	public void InitOnBoard(int health, int boardX, int boardY)
	{
		transform.localScale = Vector3.one;
		transform.localPosition = new Vector3(boardX * PZGem.SPACE_SIZE, boardY * PZGem.SPACE_SIZE);
		this.health = health;
		this.boardX = boardX;
		this.boardY = boardY;
	}

	public void Damage()
	{
		health--;
	}

	IEnumerator Destroy()
	{
		//TODO: Play sound?
		//TODO: Particles?
		fullSprite.alpha = 0;
		leftSprite.alpha = rightSprite.alpha = 1;

		float fallSpeed = 0;
		float currRot = 0;
		float currRotVel = startRotatVel;
		while (rightSprite.isVisible || leftSprite.isVisible)
		{
			//Jelly falls w/ acceleration
			fallSpeed += fallAccel * Time.deltaTime;
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - fallSpeed * Time.deltaTime, transform.localPosition.z);

			//Pieces move to the sides w/ constant vel
			rightSprite.transform.localPosition = new Vector3(rightSprite.transform.localPosition.x + fallSideVel * Time.deltaTime, 0, 0);
			leftSprite.transform.localPosition = new Vector3(leftSprite.transform.localPosition.x - fallSideVel * Time.deltaTime, 0, 0);

			//Rotation slows over time, never reverses!
			currRotVel += rotateAccel * Time.deltaTime;
			if (currRotVel < 0)
			{
				currRot += currRotVel * Time.deltaTime;

				rightSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, currRot));
				leftSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -currRot));
			}

			yield return null;
		}

		Pool();
	}
	
	public void Pool()
	{
		PZPuzzleManager.instance.jellyBoard[boardX, boardY] = null;
		GetComponent<MSSimplePoolable>().Pool();
	}
}
