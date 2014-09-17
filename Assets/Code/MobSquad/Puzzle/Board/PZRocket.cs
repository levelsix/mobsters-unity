using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(MSSimplePoolable))]
public class PZRocket : MonoBehaviour {

	MSValues.Direction dir;

	[SerializeField]
	float speed;

	[HideInInspector]
	public Transform trans;

	MSSimplePoolable pool;

	PZDestroySpecial destroySpecial;

	UISprite sprite;

	UISprite board
	{
		get
		{
			return PZPuzzleManager.instance.boardSprite;
		}
	}

	void Awake()
	{
		trans = transform;
		pool = GetComponent<MSSimplePoolable>();
		sprite = GetComponent<UISprite>();
		destroySpecial = GetComponent<PZDestroySpecial>();
	}

	public void Init(MSValues.Direction dir)
	{
		this.dir = dir;
		MSSoundManager.instance.PlayOneShot (MSSoundManager.instance.rocket);
	}

	void Update()
	{
		trans.localPosition += MSValues.dirVectors[dir] * speed * Time.deltaTime;
		// you cannot add or subtract board height from global coordinates.  Height is still in local units.
		// instead convert the rocket's position into the board's local coordinates.
		switch(dir)
		{
		case MSValues.Direction.NORTH:
			if (board.transform.InverseTransformPoint(trans.position).y > board.transform.localPosition.y + board.height)
			{
				destroySpecial.DisableLock();
			}
			break;
		case MSValues.Direction.SOUTH:
			if (trans.position.y < board.transform.position.y)
			{
				destroySpecial.DisableLock();
			}
			break;
		case MSValues.Direction.EAST:
			if (trans.position.x > board.transform.position.x)
			{
				destroySpecial.DisableLock();
			}
			break;
		case MSValues.Direction.WEST:
			if (board.transform.InverseTransformPoint(trans.position).x < board.transform.localPosition.x - board.width)
			{
				destroySpecial.DisableLock();
			}
			break;
		}


		if (!sprite.isVisible)
		{
			pool.Pool();
		}
	}

}
