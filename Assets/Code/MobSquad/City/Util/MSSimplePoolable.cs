using UnityEngine;
using System.Collections;
using System;

public class MSSimplePoolable : MonoBehaviour, MSPoolable {
	
	GameObject gameObj;
	Transform trans;
	MSSimplePoolable _prefab;
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	public Transform transf {
		get {
			return trans;
		}
	}
	public MSPoolable prefab {
		get {
			return _prefab;
		}
		set {
			_prefab = value as MSSimplePoolable;
		}
	}

	[SerializeField] bool setScale = true;
	[SerializeField] Vector3 scale = Vector3.one;
	
	void Awake()
	{
		trans = transform;
		gameObj = gameObject;
	}
	
	public MSPoolable Make (Vector3 origin)
	{
		MSSimplePoolable chunk = Instantiate(this, origin, Quaternion.identity) as MSSimplePoolable;
		chunk.prefab = this;
		return chunk;
	}

	void OnEnable()
	{
		//StartCoroutine(SetScale());
	}

	IEnumerator SetScale()
	{
		if (setScale)
		{
			yield return null;
			trans.localScale = scale;
		}
	}
	
	public void Pool ()
	{
		MSPoolManager.instance.Pool(this);
	}
}
