using UnityEngine;
using System.Collections;

public class PZMatchParticle : MonoBehaviour {

	[SerializeField]
	float startForce;

	[SerializeField]
	float lerpTime;

	[SerializeField]
	float totalTime;

	Vector3 velocity;

	Vector3 startVelocity;

	Vector3 dest;

	MSSimplePoolable pool;

	[HideInInspector]
	public Transform trans;

	Vector3 dir
	{
		get
		{
			Vector3 d = (dest - trans.localPosition);
			d.z = 0;
			return d.normalized;
		}
	}

	void Awake()
	{
		pool = GetComponent<MSSimplePoolable>();
		trans = transform;
	}

	public void Init()
	{
		trans.parent = PZCombatManager.instance.activePlayer.transform;
		trans.localScale = Vector3.one;
		
		dest = Vector3.zero;

		startVelocity = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f)).normalized * startForce;

		StartCoroutine(LerpVelocity());
	}


	IEnumerator LerpVelocity()
	{
		float curr = 0;
		do 
		{
			curr += Time.deltaTime;
			velocity = Vector3.Lerp(startVelocity, dir * startForce, curr/lerpTime);
			trans.localPosition += velocity * Time.deltaTime;
			yield return null;
		}while (curr < lerpTime);
		Vector3 midPos = trans.localPosition;
		while (curr < totalTime)
		{
			curr += Time.deltaTime;
			trans.localPosition = Vector3.Lerp(midPos, dest, (curr-lerpTime)/(totalTime-lerpTime));
			yield return null;
		}
		yield return new WaitForSeconds(.15f);
		pool.Pool();
	}
}
