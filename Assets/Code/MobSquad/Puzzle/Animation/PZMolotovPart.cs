using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MSSimplePoolable))]
[RequireComponent (typeof(PZDestroySpecial))]
public class PZMolotovPart : MonoBehaviour {

	[SerializeField]
	float speed;

	[SerializeField]
	float toleranceSqr = 10;

	MSSimplePoolable pool;

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
		pool = GetComponent<MSSimplePoolable>();
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

		/*
		Transform particle = (CBKPoolManager.instance.Get(CBKPrefabList.instance.molotovParticle, trans.position) as MonoBehaviour).transform;
		particle.parent = trans;
		particle.localPosition = Vector3.zero;
		*/
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
