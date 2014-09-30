using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// MSWave
/// @author Rob Giusti
/// </summary>
[RequireComponent (typeof (MSSimplePoolable))]
public class MSWave : MonoBehaviour 
{
	[SerializeField] SpriteRenderer sprite;
	[SerializeField] AnimationCurve distanceCurve;
	[SerializeField] AnimationCurve alphaCurve;
	[SerializeField] float maxDist;
	[SerializeField] Vector3 dir;

	public void Init(float time)
	{
		transform.localPosition = Vector3.zero;
		StartCoroutine(Run(time));
	}

	IEnumerator Run(float time)
	{
		float currTime = 0;
		while (currTime < time)
		{
			currTime += Time.deltaTime;
			transform.localPosition = dir * maxDist * distanceCurve.Evaluate(currTime/time);
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alphaCurve.Evaluate(currTime/time));
			yield return null;
		}
		GetComponent<MSSimplePoolable>().Pool();
	}
}
