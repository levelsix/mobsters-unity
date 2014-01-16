using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CBKSimplePoolable))]
[RequireComponent (typeof(PZDestroySpecial))]
public class PZMolotovPart : MonoBehaviour {

	[SerializeField]
	float speed;

	[SerializeField]
	float toleranceSqr = 10;

	CBKSimplePoolable pool;

	PZDestroySpecial desSpec;

	Vector3 dest;

	Vector3 direction;

	[HideInInspector]
	public Transform trans;

	[SerializeField]
	float delayPerPart = .1f;

	void Awake()
	{
		trans = transform;
		pool = GetComponent<CBKSimplePoolable>();
		desSpec = GetComponent<PZDestroySpecial>();
	}

	public void Init(Vector3 pos, Vector3 desitination, PZGem target, int index)
	{
		trans.localPosition = pos;

		this.dest = desitination;

		direction = (desitination - pos).normalized;

		desSpec.target = target;

		StartCoroutine(Delay (index));
	}

	IEnumerator Delay(int index)
	{
		float storeSpeed = speed;
		speed = 0;
		yield return new WaitForSeconds(index * delayPerPart);
		speed = storeSpeed;
	}

	void Update()
	{
		trans.localPosition += direction * speed * Time.deltaTime;
		if ((trans.localPosition - dest).sqrMagnitude < toleranceSqr)
		{
			pool.Pool();
		}
	}
}
