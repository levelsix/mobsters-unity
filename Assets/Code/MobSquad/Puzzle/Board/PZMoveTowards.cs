using UnityEngine;
using System.Collections;

public class PZMoveTowards : MonoBehaviour {

	[SerializeField]
	float startForce;

	[SerializeField]
	float lerpTime;

	[SerializeField]
	public float totalTime;
	
	Vector3 velocity;
	
	Vector3 startVelocity;
	
	Vector3 dest;

	Vector3 dir
	{
		get
		{
			Vector3 d = (dest - transform.localPosition);
			d.z = 0;
			return d.normalized;
		}
	}

	public Coroutine RunMoveTowards(Vector3 outPos, Vector3 startDir)
	{
		dest = outPos;
		startVelocity = startDir.normalized * startForce;

		return StartCoroutine(MoveTowards());
	}

	IEnumerator MoveTowards()
	{
		float curr = 0;
		do 
		{
			curr += Time.deltaTime;
			velocity = Vector3.Lerp(startVelocity, dir * startForce, curr/lerpTime);
			transform.localPosition += velocity * Time.deltaTime;
			yield return null;
		}while (curr < lerpTime);
		Vector3 midPos = transform.localPosition;
		while (curr < totalTime)
		{
			curr += Time.deltaTime;
			transform.localPosition = Vector3.Lerp(midPos, dest, (curr-lerpTime)/(totalTime-lerpTime));
			yield return null;
		}
	}
}
