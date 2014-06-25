using UnityEngine;
using System.Collections;

/// <summary>
/// @author Rob Giusti
/// Puzzle animation element for the "Make it Rain" animation
/// Bomb that falls from above screen to near the enemy monster
/// Triggered on collision with the plane sprite
/// </summary>
public class PZBomb : MonoBehaviour {

	public float targetHeight;

	[SerializeField]
	float gravity;

	float velocity;

	bool falling = false;

	Transform trans;

	MSSimplePoolable pool;

	public Transform planeTrans;

	void Awake()
	{
		trans = transform;
		pool = GetComponent<MSSimplePoolable>();
	}

	void OnEnable()
	{
		falling = false;
		velocity = 0;
	}

	void Update(){
		if (planeTrans.position.x > trans.position.x && !falling) {
			StartCoroutine(Fall());
		}
	}

//	void OnTriggerEnter(Collider other)
//	{
//		//Debug.LogWarning("Entered");
//		if (!falling)
//		{
//			StartCoroutine(Fall ());
//		}
//	}



	IEnumerator Fall()
	{
		falling = true;
		while (trans.localPosition.y > targetHeight)
		{
			velocity += gravity * Time.deltaTime;
			trans.localPosition += new Vector3(0, velocity, 0) * Time.deltaTime;
			yield return null;
		}
		MSPoolManager.instance.Get(MSPrefabList.instance.bombDropParticle, trans.position);
		pool.Pool();
	}
}
