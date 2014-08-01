using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(UIWidget))]
[RequireComponent (typeof(TweenPosition))]
public class MSSwoopAnimation : MonoBehaviour {

	public enum SwoopIDs
	{
		NONE = -1,
		PVPMENU = 1,

	}

	[SerializeField]
	bool swoopOnEnable = true;

	[SerializeField]
	SwoopIDs swoopId = SwoopIDs.NONE;

	[SerializeField]
	float swoopDistance = 20f;

	enum Direction
	{
		RIGHT,
		LEFT,
		UP,
		DOWN
	}

	[SerializeField]
	Direction swoopFrom = Direction.LEFT;

	[SerializeField]
	float delay = 0f;

	UIWidget widget;

	Vector3 startPosition;

	Vector3 offset;

	Transform trans;

	TweenPosition tweenPos;

	/// <summary>
	/// The duration of the swoop.  Can be set from swoopGroup.
	/// </summary>
	public float duration = 0.5f;

	/// <summary>
	/// Swoops an active swoopers with groupID INT over duration FLOAT
	/// </summary>
	public static Action<SwoopIDs> SwoopGroup;
	public static Action<SwoopIDs> SwoopGroupOut;

	void Awake()
	{
		widget = GetComponent<UIWidget>();
		trans = transform;
		startPosition = trans.localPosition;
		tweenPos = GetComponent<TweenPosition>();
	}

	void OnEnable()
	{
		SwoopGroup += SwoopByID;
		SwoopGroupOut += SwoopOutByID;
		widget.alpha = 0f;
		if(swoopOnEnable)
		{
			StartCoroutine( Swoop());
		}
	}

	void OnDisable()
	{
		SwoopGroup -= SwoopByID;
		SwoopGroupOut -= SwoopOutByID;
	}

	void UpdateOffset()
	{

		switch(swoopFrom)
		{
		case Direction.DOWN:
			offset = new Vector3(0f,-swoopDistance,0f);
			break;
		case Direction.LEFT:
			offset = new Vector3(-swoopDistance,0f,0f);
			break;
		case Direction.RIGHT:
			offset = new Vector3(swoopDistance,0f,0f);
			break;
		case Direction.UP:
			offset = new Vector3(0f,swoopDistance,0f);
			break;
		default:
			break;
		}

		tweenPos.duration = duration;
	}

	IEnumerator Swoop()
	{
		yield return new WaitForSeconds(duration);

		UpdateOffset();

		tweenPos.from = new Vector3(startPosition.x + offset.x, startPosition.y + offset.y, startPosition.z + offset.z);
		tweenPos.to = startPosition;

		trans.localPosition = new Vector3(startPosition.x + offset.x, startPosition.y + offset.y, startPosition.z + offset.z);
		tweenPos.ResetToBeginning();
		tweenPos.delay = delay;
		tweenPos.PlayForward();
		TweenAlpha.Begin(gameObject, duration, 1f);
		GetComponent<TweenAlpha>().delay = delay;
	}

	IEnumerator SwoopOut()
	{
		yield return new WaitForSeconds(duration);

		UpdateOffset();

		tweenPos.to = new Vector3(startPosition.x + offset.x, startPosition.y + offset.y, startPosition.z + offset.z);
		tweenPos.from = startPosition;

		trans.localPosition = startPosition;
		tweenPos.ResetToBeginning();
		tweenPos.delay = delay;
		tweenPos.PlayForward();
		TweenAlpha.Begin(gameObject, duration, 0f);
		GetComponent<TweenAlpha>().delay = delay;
		GetComponent<TweenAlpha>().AddOnFinished( delegate { transform.localPosition = startPosition; } );
	}

	/// <summary>
	/// Swoops all active swoop components with the given ID
	/// </summary>
	/// <param name="groupId">Group identifier.</param>
	void SwoopByID(SwoopIDs groupId)
	{
		if(groupId != SwoopIDs.NONE && groupId == this.swoopId)
		{
			StartCoroutine( this.Swoop ());
		}
	}

	void SwoopOutByID(SwoopIDs groupId)
	{
		if(groupId != SwoopIDs.NONE && groupId == this.swoopId)
		{
			StartCoroutine( this.SwoopOut ());
		}
	}
}
