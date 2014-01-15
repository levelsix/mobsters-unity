using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CBKSimplePoolable))]
public class PZMolotovPart : MonoBehaviour {

	[SerializeField]
	float speed;

	[SerializeField]
	float toleranceSqr = 10;

	CBKSimplePoolable pool;

	Vector3 dest;

	Vector3 direction;

	[HideInInspector]
	public Transform trans;

	void Awake()
	{
		trans = transform;
		pool = GetComponent<CBKSimplePoolable>();
	}

	public void Init(Vector3 pos, Vector3 desitination)
	{
		trans.localPosition = pos;

		this.dest = desitination;

		direction = (desitination - pos).normalized;
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
