using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MSSimplePoolable))]
[RequireComponent (typeof(PZDestroySpecial))]
public class PZMolotovPart : MonoBehaviour {

	[SerializeField]
	UISprite sprite;

	[SerializeField]
	float speed;

	[SerializeField]
	float toleranceSqr = 1;

	MSSimplePoolable pool;

	PZDestroySpecial desSpec;

	Vector3 dest;

	Vector3 direction;

	[HideInInspector]
	public Transform trans;

	[SerializeField]
	float delayPerPart = .1f;

	[SerializeField]
	ParticleSystem particleTail;

	ParticleSystem particleExplode;

	void Awake()
	{
		trans = transform;
		pool = GetComponent<MSSimplePoolable>();
		desSpec = GetComponent<PZDestroySpecial>();
		sprite = GetComponent<UISprite> ();
	}

	void OnEnable(){
		particleTail.Play ();
		desSpec.onTrigger += explode;
		MSSoundManager.instance.PlayOneShot(MSSoundManager.instance.rainbowParticleFire);
	}

	void OnDisable(){
		desSpec.onTrigger -= explode;
	}

	void explode(){
//		transform.position = dest;
		StartCoroutine (DelayedDeath ());
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

	IEnumerator DelayedDeath(){
		float storeSpeed = speed;
		speed = 0;
		particleTail.Stop ();

		MSPoolManager.instance.Get (MSPrefabList.instance.molotovParticle, trans.position);

		yield return new WaitForSeconds (particleTail.startLifetime);

		speed = storeSpeed;
		pool.Pool ();
	}

	void Update()
	{
		trans.localPosition += direction * speed * Time.deltaTime;
		// This catches the Moltov particle, if for some reason it misses it's target
		if (Mathf.Abs (trans.localPosition.x) > MSMath.uiScreenWidth || Mathf.Abs(trans.localPosition.y) > MSMath.uiScreenHeight)
		{
			pool.Pool();
		}

	}
}
