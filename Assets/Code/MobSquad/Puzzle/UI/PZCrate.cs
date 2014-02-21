using UnityEngine;
using System.Collections;

public class PZCrate : MonoBehaviour {

	[SerializeField]
	float moveTime;

	[SerializeField]
	float moveSpeed;

	bool started = false;

	[SerializeField]
	Vector3 fallDist;

	[SerializeField]
	float fallTime;

	Transform trans;
	
	void Awake()
	{
		trans = transform;
	}

	void OnEnable()
	{
		started = false;
		StartCoroutine(Fall());
	}

	IEnumerator Fall()
	{
		Vector3 start = trans.localPosition;
		Vector3 target = start + fallDist;
		float curr = 0;
		while (curr < fallTime)
		{
			curr += Time.deltaTime;
			trans.localPosition = Vector3.Lerp(start, target, curr/fallTime);
			yield return null;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		PZCombatUnit combat = other.GetComponent<PZCombatUnit>();
		if (!started && combat != null && combat == PZCombatManager.instance.activePlayer)
		{
			StartCoroutine(Move());
		}
	}

	IEnumerator Move()
	{
		started = true;
		Vector3 originalScale = trans.localScale;
		float curr = 0;
		while (curr < moveTime)
		{
			curr += Time.deltaTime;
			
			float lp = curr/moveTime;
			trans.localPosition += Time.deltaTime * moveSpeed * lp *
				Vector3.Lerp(new Vector3(1, -1), new Vector3(-1, -1), lp);
			trans.localScale = Vector3.Lerp (originalScale, Vector3.zero, lp);

			yield return null;
		}

		PZCombatManager.instance.crate = null;
		GetComponent<CBKSimplePoolable>().Pool();
	}
}
