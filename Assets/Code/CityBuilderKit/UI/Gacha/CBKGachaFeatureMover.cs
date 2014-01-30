using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// CBKGachaFeatureMover
/// @author Rob Giusti
/// </summary>
public class CBKGachaFeatureMover : MonoBehaviour {

	List<BoosterItemProto> featuredMobsters;

	[SerializeField]
	CBKActionButton forwardButton;

	[SerializeField]
	CBKActionButton backwardButton;

	[SerializeField]
	CBKGachaFeaturedMobster mobster;

	Transform mobsterTrans;

	[SerializeField]
	float speed;

	bool moving;

	bool hasLooped;

	int currIndex = 0;

	void Awake()
	{
		mobsterTrans = mobster.transform;
	}

	void OnEnable()
	{
		mobster.looper.onLoop += OnLoop;
		forwardButton.onClick += MoveForward;
		backwardButton.onClick += MoveBackward;
	}

	void OnDisable()
	{
		mobster.looper.onLoop -= OnLoop;
		forwardButton.onClick -= MoveForward;
		backwardButton.onClick -= MoveBackward;
	}

	public void Init(List<BoosterItemProto> features)
	{
		featuredMobsters = features;

		mobster.Init (features[0]);
		mobsterTrans.localPosition = Vector3.zero;
	}

	void MoveForward()
	{
		if (!moving)
		{
			currIndex++;
			if (currIndex >= featuredMobsters.Count)
			{
				currIndex = 0;
			}
			StartCoroutine(DoMove (1));
		}
	}

	void MoveBackward()
	{
		if (!moving)
		{
			currIndex--;
			if (currIndex < 0) 
			{
				currIndex = featuredMobsters.Count-1;
			}
			StartCoroutine(DoMove(-1));
		}
	}

	void OnLoop()
	{
		mobster.Init(featuredMobsters[currIndex]);
		hasLooped = true;
	}

	IEnumerator DoMove(int dir)
	{
		moving = true;
		hasLooped = false;

		while(!hasLooped)
		{
			mobsterTrans.localPosition += new Vector3(speed, 0, 0) * dir * Time.deltaTime;
			yield return null;
		}

		while ((dir > 0) ? mobsterTrans.localPosition.x < 0 : mobsterTrans.localPosition.x > 0)
		{
			mobsterTrans.localPosition += new Vector3(speed, 0, 0) * dir * Time.deltaTime;
			yield return null;
		}

		mobsterTrans.localPosition = Vector3.zero;

		moving = false;
	}
}
