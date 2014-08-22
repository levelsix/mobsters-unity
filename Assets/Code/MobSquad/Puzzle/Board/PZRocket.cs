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
	}

	public void Init(MSValues.Direction dir)
	{
		this.dir = dir;
		MSSoundManager.instance.PlayOneShot (MSSoundManager.instance.rocket);
	}

	void Update()
	{
		trans.localPosition += MSValues.dirVectors[dir] * speed * Time.deltaTime;

		switch(dir)
		{
		case MSValues.Direction.NORTH:
			if (trans.position.y > board.transform.position.y + board.height)
			{
				GetComponent<PZDestroySpecial>().DisableLock();
			}
			break;
		case MSValues.Direction.SOUTH:
			if (trans.position.y < board.transform.position.y)
			{
				GetComponent<PZDestroySpecial>().DisableLock();
			}
			break;
		case MSValues.Direction.EAST:
			if (trans.position.x > board.transform.position.x)
			{
				GetComponent<PZDestroySpecial>().DisableLock();
			}
			break;
		case MSValues.Direction.WEST:
			if (trans.position.x < board.transform.position.x - board.width)
			{
				GetComponent<PZDestroySpecial>().DisableLock();
			}
			break;
		}


		if (!sprite.isVisible)
		{
			pool.Pool();
		}
	}

}
